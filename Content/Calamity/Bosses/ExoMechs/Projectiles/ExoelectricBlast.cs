using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.DataStructures;
using Luminance.Common.Easings;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExoelectricBlast : ModProjectile, IPixelatedPrimitiveRenderer, IProjOwnedByBoss<ThanatosHead>, IExoMechProjectile
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterProjectiles;

        /// <summary>
        /// How long this sphere has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        /// <summary>
        /// How long this laserbeam currently is.
        /// </summary>
        public ref float LaserbeamLength => ref Projectile.ai[1];

        /// <summary>
        /// How long the laserbeam exists for.
        /// </summary>
        public static int Lifetime => Utilities.SecondsToFrames(4.2f);

        /// <summary>
        /// The maximum length of this laserbeam.
        /// </summary>
        public static float MaxLaserbeamLength => 6700f;

        /// <summary>
        /// How long the beam waits before beginning to expand.
        /// </summary>
        public static int ExpandDelay => Utilities.SecondsToFrames(0.0667f);

        /// <summary>
        /// How long the beam spends expanding.
        /// </summary>
        public static int ExpandTime => Utilities.SecondsToFrames(0.2f);

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Electricity;

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 6000;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = Lifetime;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            int hadesIndex = CalamityGlobalNPC.draedonExoMechWorm;
            if (hadesIndex < 0 || hadesIndex >= Main.maxNPCs || Main.npc[hadesIndex].type != ModContent.NPCType<ThanatosHead>())
            {
                Projectile.Kill();
                return;
            }

            NPC hades = Main.npc[hadesIndex];
            Vector2 beamStart = hades.Center + hades.velocity.SafeNormalize(Vector2.UnitY) * 14f;
            Projectile.Center = beamStart;
            Projectile.velocity = hades.SafeDirectionTo(beamStart);
            LaserbeamLength = MathHelper.Clamp(LaserbeamLength + 189f, 0f, MaxLaserbeamLength);

            float expandInterpolant = Utilities.InverseLerp(0f, ExpandTime, Time - ExpandDelay);
            Projectile.width = (int)(MathHelper.Lerp(Time / 42f * 8f, 180f, expandInterpolant.Squared()) * Utilities.InverseLerp(0f, 10f, Projectile.timeLeft));

            CreateVisuals(expandInterpolant);

            Time++;
        }

        /// <summary>
        /// Handles various visuals for this blast, such as calculating the overheat, creating sparks, making the screen shake, etc.
        /// </summary>
        /// <param name="expandInterpolant"></param>
        public void CreateVisuals(float expandInterpolant)
        {
            // Darken the sky to increase general contrast with everything.
            CustomExoMechsSky.CloudExposure = MathHelper.Lerp(CustomExoMechsSky.DefaultCloudExposure, 0.085f, expandInterpolant);

            for (int i = 0; i < Projectile.width / 21; i++)
            {
                float laserbeamLengthInterpolant = Main.rand.NextFloat(0.07f, 1f);
                Vector2 randomLinePosition = Projectile.Center + Projectile.velocity * laserbeamLengthInterpolant * LaserbeamLength + Main.rand.NextVector2CircularEdge(Projectile.width, Projectile.width) * 0.5f;
                CreateElectricSpark(randomLinePosition);
            }
            if (Main.rand.NextBool(Projectile.width / 300f))
                CreateElectricSpark(Projectile.Center + Projectile.velocity * 20f);

            ScreenShakeSystem.SetUniversalRumble(Projectile.width / 60f, MathHelper.TwoPi, null, 0.2f);

            if (Time % 14f == 13f)
                CustomExoMechsSky.CreateLightning(Projectile.Center.ToScreenPosition());
        }

        /// <summary>
        /// Creates a single electric spark around the sphere's edge.
        /// </summary>
        public void CreateElectricSpark(Vector2 arcSpawnPosition)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int arcLifetime = Main.rand.Next(10, 19);
            Vector2 arcLength = Projectile.SafeDirectionTo(arcSpawnPosition).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(120f, Projectile.width * 0.034f + 150f);

            if (Main.rand.NextBool(2))
                arcLength *= 1.35f;
            if (Main.rand.NextBool(2))
                arcLength *= 1.35f;
            if (Main.rand.NextBool(4))
                arcLength *= 1.6f;
            if (Main.rand.NextBool(4))
                arcLength *= 1.6f;

            Utilities.NewProjectileBetter(Projectile.GetSource_FromThis(), arcSpawnPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, arcLifetime, 0f);
        }

        public float LaserWidthFunction(float completionRatio)
        {
            float frontExpansionInterpolant = Utilities.InverseLerp(0.015f, 0.14f, completionRatio);
            float maxSize = Projectile.width + completionRatio * Projectile.width * 1.2f;
            return EasingCurves.Quadratic.Evaluate(EasingType.Out, 2f, maxSize, frontExpansionInterpolant);
        }

        public Color LaserColorFunction(float completionRatio)
        {
            Color electricColor = new(0.4f, 1f, 1f);
            return Projectile.GetAlpha(electricColor);
        }

        public float BloomWidthFunction(float completionRatio) => LaserWidthFunction(completionRatio) * 1.5f;

        public Color BloomColorFunction(float completionRatio)
        {
            Color electricColor = new(0.67f, 0.7f, 1f, 0f);
            float opacity = Utilities.InverseLerp(0.01f, 0.065f, completionRatio) * Utilities.InverseLerp(0.9f, 0.7f, completionRatio) * 0.32f;
            return Projectile.GetAlpha(electricColor) * opacity;
        }

        public void DrawBackBloom()
        {
            Color outerBloomColor = new(0.34f, 0.75f, 1f, 0f);
            float bloomScale = Projectile.width / 300f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Projectile.velocity * Projectile.width * 0.21f;
            Texture2D bloom = MiscTexturesRegistry.BloomCircleSmall.Value;
            Main.spriteBatch.Draw(bloom, drawPosition, null, Projectile.GetAlpha(new(1f, 1f, 1f, 0f)) * 0.5f, 0f, bloom.Size() * 0.5f, bloomScale * 2f, 0, 0f);
            Main.spriteBatch.Draw(bloom, drawPosition, null, Projectile.GetAlpha(outerBloomColor) * 0.4f, 0f, bloom.Size() * 0.5f, bloomScale * 5f, 0, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBackBloom();
            List<Vector2> laserPositions = Projectile.GetLaserControlPoints(12, LaserbeamLength);

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.PrimitiveBloomShader");
            shader.TrySetParameter("innerGlowIntensity", 0.45f);

            PrimitiveSettings bloomSettings = new(BloomWidthFunction, BloomColorFunction, Shader: shader);
            PrimitiveRenderer.RenderTrail(laserPositions, bloomSettings, 60);

            return false;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            List<Vector2> laserPositions = Projectile.GetLaserControlPoints(12, LaserbeamLength);

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.HadesExoEnergyBlastShader");
            shader.TrySetParameter("laserDirection", Projectile.velocity);
            shader.TrySetParameter("edgeColorSubtraction", new Vector3(0.7f, 0.4f, 0));
            shader.TrySetParameter("edgeGlowIntensity", 0.2f);
            shader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 1, SamplerState.LinearWrap);
            shader.SetTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise"), 2, SamplerState.LinearWrap);

            PrimitiveSettings laserSettings = new(LaserWidthFunction, LaserColorFunction, Pixelate: true, Shader: shader);
            PrimitiveRenderer.RenderTrail(laserPositions, laserSettings, 60);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Measure how far along the laser's length the target is.
            // If the signed distance is negative (a.k.a. they're behind the laser) or above the laser length (a.k.a. they're beyond the laser), terminate this
            // method immediately.
            float signedDistanceAlongLaser = Utilities.SignedDistanceToLine(targetHitbox.Center(), Projectile.Center, Projectile.velocity);
            if (signedDistanceAlongLaser < 0f || signedDistanceAlongLaser >= LaserbeamLength)
                return false;

            // Now that the point on the laser is known from the distance, evaluate the exact width of the laser at said point for use with a AABB/line collision check.
            float laserWidth = LaserWidthFunction(signedDistanceAlongLaser / LaserbeamLength) * 0.45f;
            Vector2 perpendicular = new(-Projectile.velocity.Y, Projectile.velocity.X);
            Vector2 laserPoint = Projectile.Center + Projectile.velocity * signedDistanceAlongLaser;
            Vector2 left = laserPoint - perpendicular * laserWidth;
            Vector2 right = laserPoint + perpendicular * laserWidth;

            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), left, right, 16f, ref _);
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
