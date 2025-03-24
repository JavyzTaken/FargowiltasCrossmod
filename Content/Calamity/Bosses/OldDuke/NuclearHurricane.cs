using FargowiltasCrossmod.Assets;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common.Graphics.Meshes;
using FargowiltasCrossmod.Core.Common.Graphics.RenderTargets;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

[JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
[ExtendsFromMod(ModCompatibility.Calamity.Name)]
public class NuclearHurricane : ModProjectile
{
    /// <summary>
    /// The render target that holds all information for hurricanes.
    /// </summary>
    public static InstancedRequestableTarget HurricaneTarget
    {
        get;
        private set;
    }

    private static float MaxVisualBumpSquish => 0.4f;

    private static float WavinessFactor => 0.16f;

    /// <summary>
    /// The amount by which this hurricane has begun appearing.
    /// </summary>
    public float AppearanceInterpolant => 1f; // LumUtils.InverseLerp(0f, 120f, Time).Squared();

    /// <summary>
    /// The dissolve interpolant of this hurricane.
    /// </summary>
    public float DissolveInterpolant => LumUtils.InverseLerp(Lifetime - 60f, Lifetime, Time);

    // TODO -- Consider creating some sort of vfx config option that slows this down. Concerns were brought up about this being too fast by default for those with photosensitivity.
    /// <summary>
    /// A general-purpose timer which determines the animation of visuals for this hurricane.
    /// </summary>
    public float VisualsTime => Time / 32f + Projectile.identity * 0.33f;

    /// <summary>
    /// How long this hurricane should exist for, in frames.
    /// </summary>
    public int Lifetime => (int)Projectile.ai[1];

    /// <summary>
    /// The Z position of this hurricane.
    /// </summary>
    public ref float Z => ref Projectile.ai[0];

    /// <summary>
    /// A general purpose time for this hurricane. Increments faster as it approaches the foreground.
    /// </summary>
    public ref float Time => ref Projectile.ai[2];

    public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 8500;
        HurricaneTarget = new InstancedRequestableTarget();
        Main.ContentThatNeedsRenderTargets.Add(HurricaneTarget);
    }

    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.hostile = true;
        Projectile.timeLeft = 60000;
        Projectile.hide = true;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        Time += 1f / (Z + 1f);
        Z = MathHelper.Lerp(Z, 0.01f, 0.03f);

        Projectile.scale = 1f / (Z + 1f);
        Projectile.Opacity = 1f;
        Projectile.Resize(1400, 2600);

        MoveTowardsTarget();

        // Stay in the world.
        Vector2 shoveDistance = Projectile.Size * 0.6f;
        Projectile.Center = Vector2.Clamp(Projectile.Center, shoveDistance, new Vector2(Main.maxTilesX, Main.maxTilesY) * 16f - shoveDistance);

        ApplyParallaxPositioning();

        float dissolveInterpolant = DissolveInterpolant;
        for (int i = 0; i < Projectile.scale * (1f - dissolveInterpolant) * 25f; i++)
        {
            // Calculate the height and angle of the dust on the hurricane.
            float heightInterpolant = Main.rand.NextFloat(0.2f, 0.8f);
            float cylinderAngle = Main.rand.NextFloat(MathHelper.Pi);

            // Cache the bump value of the height separately for ease of use. This is used a couple times in the shaders that compose this hurricane.
            float heightBump = QuadraticBump(heightInterpolant);

            // Calculate the wave value (and its corresponding derivative for later velocity math) based on the math used by the vertex shader.
            float horizontalOffsetWave = MathF.Sin(heightInterpolant * -6.283f + VisualsTime);
            float horizontalOffsetWaveDerivative = MathF.Cos(heightInterpolant * -6.283f + VisualsTime);

            // Apply offsets based on the math that governs the hurricane's shape.
            // If you update the shaders, update this too.
            float horizontalSquishFactor = heightBump * MathHelper.Lerp(1f, MaxVisualBumpSquish, heightBump);
            Vector2 spawnPosition = Projectile.Center + Vector2.UnitY * (heightInterpolant - 0.5f) * Projectile.height * Projectile.scale;
            spawnPosition.X += horizontalOffsetWave * horizontalSquishFactor * Projectile.width * Projectile.scale * WavinessFactor;

            // Offsets based on height, accounting for the shape of the hurricane is now. Now offset based on the cylinder angle.
            spawnPosition.X += MathF.Cos(cylinderAngle) * Projectile.width * Projectile.scale * horizontalSquishFactor * 0.54f;

            Dust energy = Dust.NewDustPerfect(spawnPosition, 267);
            energy.color = Color.White;
            energy.velocity = new Vector2(horizontalOffsetWaveDerivative * 25f, -10f) * Projectile.scale;
            energy.scale *= Main.rand.NextFloat(0.6f, 0.925f) * Projectile.scale;
            energy.noGravity = true;
        }

        // Create projectiles around the hurricane.
        if (Main.netMode != NetmodeID.MultiplayerClient && Projectile.scale >= 0.9f && Projectile.timeLeft % 6 == 0 && dissolveInterpolant <= 0.1f)
        {
            float x = Main.rand.NextFloatDirection() * Projectile.width * 0.4f;
            float y = Main.rand.NextFloat(-0.4f, 0.2f) * Projectile.height;
            Vector2 spawnPosition = Projectile.Center + new Vector2(x, y);

            WeightedRandom<int> projectileSelector = new WeightedRandom<int>(Projectile.whoAmI + Projectile.timeLeft);
            projectileSelector.Add(ModContent.ProjectileType<AquaticUrchinProjectile>(), 1.05);
            projectileSelector.Add(ModContent.ProjectileType<ArsenalCrate>(), 0.5);
            projectileSelector.Add(ModContent.ProjectileType<ExplosiveCrate>(), 0.5);
            projectileSelector.Add(ModContent.ProjectileType<TrasherProjectile>(), 0.24);

            LumUtils.NewProjectileBetter(Projectile.GetSource_FromThis(), spawnPosition, Main.rand.NextVector2CircularEdge(15f, 4f), projectileSelector.Get(), 250, 0f);
        }

        if (Time >= Lifetime)
            Projectile.Kill();
    }

    private void MoveTowardsTarget()
    {
        float worldEdgeFluff = 2150f;

        // Don't force the player into a hit at the world border.
        Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
        Vector2 targetPos = target.Center;
        float spawnTime = 20f;
        float spawnLerp = Time / spawnTime;
        float speedX = 0.2f;
        float maxSpeed = AppearanceInterpolant * 11f;
        if (spawnLerp < 1)
        {
            Vector2 hurricaneSpawnPosition = target.Center;

            // Bias the hurricane towards the world border position.
            bool left = target.Center.X < Main.maxTilesX * 8f;
            hurricaneSpawnPosition.X -= left.ToDirectionInt() * 1100f;
            targetPos = hurricaneSpawnPosition;
            speedX = 2f;
            maxSpeed = 20f;
            Projectile.Opacity = LumUtils.InverseLerp(0, 0.3f, spawnLerp);
        }
        Vector2 acceleration = Projectile.SafeDirectionTo(targetPos) * new Vector2(speedX, 0.125f);
        bool movingTowardsBorder = (Projectile.Left.X < worldEdgeFluff && acceleration.X < 0f) ||
                                   (Projectile.Right.X > Main.maxTilesX * 16f - worldEdgeFluff && acceleration.X > 0f);


        if (movingTowardsBorder && spawnLerp >= 1)
            Projectile.velocity *= 0.993f;

        else
            Projectile.velocity = (Projectile.velocity + acceleration).ClampLength(0f, maxSpeed);
    }

    private void ApplyParallaxPositioning()
    {
        if (Main.netMode != NetmodeID.SinglePlayer)
            return;

        // God.
        // This ensures that the hurricane's apparent position isn't as responsive to camera movements if it's in the background, giving a pseudo-parallax visual.
        // Idea is basically the hurricane going
        // "Oh? You moved 30 pixels in this direction? Well I'm in the background bozo so I'm gonna follow you and go in the same direction by, say, 27 pixels. This will make it look like I only moved 3 pixels"
        float parallax = 1f - MathF.Pow(2f, Z * -0.8f);
        Vector2 targetOffset = Main.LocalPlayer.position - Main.LocalPlayer.oldPosition;
        Projectile.position += targetOffset * LumUtils.Saturate(parallax);
    }

    public static void AffectProjectileVelocity(Projectile projectile)
    {
        int hurricaneID = ModContent.ProjectileType<NuclearHurricane>();
        float maximumForce = 3.3f;
        float maximumFlySpeed = 23.5f;
        foreach (Projectile hurricane in Main.ActiveProjectiles)
        {
            if (hurricane.type != hurricaneID)
                continue;

            float heightInterpolant = LumUtils.InverseLerp(hurricane.Top.Y, hurricane.Bottom.Y, projectile.Center.Y);
            float horizontalAcceleration = -MathF.Sin(heightInterpolant * -6.283f + hurricane.As<NuclearHurricane>().VisualsTime); // Second derivative of wave value.
            float distanceFromCenter = projectile.Distance(hurricane.Center);
            float distanceCoefficient = hurricane.width * hurricane.width * 0.4f;
            float force = MathF.Min(maximumForce, distanceCoefficient / (distanceFromCenter.Squared() + 0.0001f)); // Faux inverse square distance proportionality law with a cap and division by zero safety check.
            Vector2 acceleration = new Vector2(horizontalAcceleration, -0.75f) * force;

            projectile.velocity = (projectile.velocity + acceleration).ClampLength(0f, maximumFlySpeed);
        }
    }

    private static float QuadraticBump(float x)
    {
        return x * (4 - x * 4);
    }

    private Matrix CreateScaleMatrix(float widthFactor = 1f)
    {
        return Matrix.CreateScale(Projectile.width * widthFactor, -Projectile.height, 1f);
    }

    private void RenderHurricaneToTarget()
    {
        var gd = Main.instance.GraphicsDevice;

        Matrix world = Matrix.CreateTranslation(gd.Viewport.Width * 0.5f, gd.Viewport.Height * 0.5f, 0.1f);
        Matrix view = Matrix.CreateLookAt(new Vector3(0f, 0f, 1500f), Vector3.Zero, Vector3.Up);
        Matrix projection = Matrix.CreateOrthographicOffCenter(0f, gd.Viewport.Width, gd.Viewport.Height, 0f, 0f, 10000f);

        gd.RasterizerState = RasterizerState.CullNone;

        float time = VisualsTime;
        float effectiveDissolveInterpolant = (1f - AppearanceInterpolant) + DissolveInterpolant;

        // Render the backglow.
        ManagedShader glowShader = ShaderManager.GetShader("FargowiltasCrossmod.NuclearHurricaneGlowShader");
        glowShader.TrySetParameter("localTime", time);
        glowShader.TrySetParameter("maxBumpSquish", MaxVisualBumpSquish);
        glowShader.TrySetParameter("wavinessFactor", WavinessFactor);
        glowShader.TrySetParameter("glowColor", new Vector4(0.81f, 0.9f, 0.1f, 1f) * Projectile.Opacity * (1f - effectiveDissolveInterpolant) * 1.5f);
        glowShader.TrySetParameter("uWorldViewProjection", CreateScaleMatrix(1.85f) * world * view * projection);
        glowShader.SetTexture(NoiseTexturesRegistry.PerlinNoise.Value, 1, SamplerState.LinearWrap);
        glowShader.Apply();
        gd.SetVertexBuffer(MeshRegistry.CylinderVertices);
        gd.Indices = MeshRegistry.CylinderIndices;
        gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, MeshRegistry.CylinderVertices.VertexCount, 0, MeshRegistry.CylinderIndices.IndexCount / 3);

        // Render the core.
        ManagedShader coreShader = ShaderManager.GetShader("FargowiltasCrossmod.NuclearHurricaneCoreShader");
        coreShader.TrySetParameter("localTime", time);
        coreShader.TrySetParameter("maxBumpSquish", MaxVisualBumpSquish);
        coreShader.TrySetParameter("wavinessFactor", WavinessFactor);
        coreShader.TrySetParameter("opacity", Projectile.Opacity);
        coreShader.TrySetParameter("uWorldViewProjection", CreateScaleMatrix(1f) * world * view * projection);
        coreShader.TrySetParameter("baseColor", new Vector3(0.7f, 0.95f, 0f));
        coreShader.TrySetParameter("additiveAccentColor", new Vector3(0.7f, 1f, 0f));
        coreShader.TrySetParameter("dissolveInterpolant", effectiveDissolveInterpolant);
        coreShader.SetTexture(MiscTexturesRegistry.DendriticNoiseZoomedOut.Value, 1, SamplerState.LinearWrap);
        coreShader.Apply();
        gd.SetVertexBuffer(MeshRegistry.CylinderVertices);
        gd.Indices = MeshRegistry.CylinderIndices;
        gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, MeshRegistry.CylinderVertices.VertexCount, 0, MeshRegistry.CylinderIndices.IndexCount / 3);

        // Render foam A.
        ManagedShader foamShader = ShaderManager.GetShader("FargowiltasCrossmod.NuclearHurricaneFoamShader");
        foamShader.TrySetParameter("localTime", time * 1.2f);
        foamShader.TrySetParameter("maxBumpSquish", MaxVisualBumpSquish);
        foamShader.TrySetParameter("wavinessFactor", WavinessFactor);
        foamShader.TrySetParameter("zoom", 0.4f);
        foamShader.TrySetParameter("uWorldViewProjection", CreateScaleMatrix(1.25f) * world * view * projection);
        foamShader.TrySetParameter("dissolveInterpolant", effectiveDissolveInterpolant);
        foamShader.SetTexture(NoiseTexturesRegistry.PerlinNoise.Value, 1, SamplerState.LinearWrap);
        foamShader.Apply();
        gd.SetVertexBuffer(MeshRegistry.CylinderVertices);
        gd.Indices = MeshRegistry.CylinderIndices;
        gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, MeshRegistry.CylinderVertices.VertexCount, 0, MeshRegistry.CylinderIndices.IndexCount / 3);

        // Render foam B.
        foamShader.TrySetParameter("zoom", 0.275f);
        foamShader.TrySetParameter("uWorldViewProjection", CreateScaleMatrix(1.5f) * world * view * projection);
        foamShader.SetTexture(MiscTexturesRegistry.DendriticNoiseZoomedOut.Value, 1, SamplerState.LinearWrap);
        foamShader.Apply();
        gd.SetVertexBuffer(MeshRegistry.CylinderVertices);
        gd.Indices = MeshRegistry.CylinderIndices;
        gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, MeshRegistry.CylinderVertices.VertexCount, 0, MeshRegistry.CylinderIndices.IndexCount / 3);

        // Render bottom.
        ManagedShader extremesShader = ShaderManager.GetShader("FargowiltasCrossmod.NuclearHurricaneExtremesShader");
        Matrix bottomMatrix = Matrix.CreateScale(Projectile.width * 1.2f, -Projectile.height * 0.25f, 0f) * Matrix.CreateTranslation(0f, Projectile.height * 0.35f, 0f);
        extremesShader.TrySetParameter("localTime", time);
        extremesShader.TrySetParameter("top", false);
        extremesShader.TrySetParameter("uWorldViewProjection", bottomMatrix * world * view * projection);
        extremesShader.TrySetParameter("color", new Vector4(0.59f, 0.84f, 0f, 1f) * (1f - effectiveDissolveInterpolant));
        extremesShader.TrySetParameter("glowColor", new Vector4(0.7f, 0.5f, 0.5f, 0f) * (1f - effectiveDissolveInterpolant) * Projectile.Opacity);
        extremesShader.SetTexture(MiscTexturesRegistry.DendriticNoiseZoomedOut.Value, 1, SamplerState.LinearWrap);
        extremesShader.Apply();
        gd.SetVertexBuffer(MeshRegistry.CylinderVertices);
        gd.Indices = MeshRegistry.CylinderIndices;
        gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, MeshRegistry.CylinderVertices.VertexCount, 0, MeshRegistry.CylinderIndices.IndexCount / 3);

        // Render top.
        Matrix topMatrix = Matrix.CreateScale(Projectile.width * 1.2f, -Projectile.height * 0.25f, 0f) * Matrix.CreateTranslation(0f, Projectile.height * -0.35f, 0f);
        extremesShader.TrySetParameter("localTime", time);
        extremesShader.TrySetParameter("top", true);
        extremesShader.TrySetParameter("uWorldViewProjection", topMatrix * world * view * projection);
        extremesShader.Apply();
        gd.SetVertexBuffer(MeshRegistry.CylinderVertices);
        gd.Indices = MeshRegistry.CylinderIndices;
        gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, MeshRegistry.CylinderVertices.VertexCount, 0, MeshRegistry.CylinderIndices.IndexCount / 3);

        gd.SetVertexBuffer(null);
        gd.Indices = null;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindNPCsAndTiles.Add(index);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        HurricaneTarget.Request(Projectile.width * 2, Projectile.width * 2, Projectile.whoAmI, RenderHurricaneToTarget);
        if (HurricaneTarget.TryGetTarget(Projectile.whoAmI, out RenderTarget2D? target) && target is not null)
        {
            Main.spriteBatch.PrepareForShaders();

            ManagedShader processingShader = ShaderManager.GetShader("FargowiltasCrossmod.NuclearHurricanePostProcessingShader");
            processingShader.TrySetParameter("pixelationFactor", Vector2.One * 3f / target.Size());
            processingShader.TrySetParameter("posterizationLevel", 56f);
            processingShader.TrySetParameter("blurriness", LumUtils.InverseLerp(1f, 10f, Z));
            processingShader.TrySetParameter("fadeToBackground", MathF.Pow(LumUtils.InverseLerp(0.67f, 10f, Z), 0.55f));
            processingShader.TrySetParameter("backgroundColor", Main.ColorOfTheSkies.ToVector4());
            processingShader.Apply();

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(target, drawPosition, null, Color.White, 0f, target.Size() * 0.5f, Projectile.scale, 0, 0f);

            Main.spriteBatch.ResetToDefault();
        }

        return false;
    }

    public override bool? CanDamage() => Z <= 0.05f && DissolveInterpolant <= 0.33f && AppearanceInterpolant >= 0.85f;

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        Rectangle top = Utils.CenteredRectangle(Projectile.Top + Vector2.UnitY * 300f, new Vector2(Projectile.width, 300f));
        if (targetHitbox.Intersects(top))
            return true;

        Rectangle bottom = Utils.CenteredRectangle(Projectile.Bottom - Vector2.UnitY * 300f, new Vector2(Projectile.width, 300f));
        if (targetHitbox.Intersects(bottom))
            return true;

        Rectangle center = Utils.CenteredRectangle(Projectile.Center, new Vector2(Projectile.width * MaxVisualBumpSquish * 0.85f, Projectile.height * 0.85f));
        if (targetHitbox.Intersects(center))
            return true;

        return false;
    }
}