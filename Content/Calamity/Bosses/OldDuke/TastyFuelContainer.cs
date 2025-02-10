using FargowiltasCrossmod.Assets;
using FargowiltasCrossmod.Assets.Particles;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

public class TastyFuelContainer : ModProjectile
{
    /// <summary>
    /// The glowmask asset associated with this fuel container.
    /// </summary>
    public static Asset<Texture2D> GlowmaskAsset
    {
        get;
        private set;
    }

    /// <summary>
    /// The alt texture asset associated with this fuel container.
    /// </summary>
    public static Asset<Texture2D> TextureAssetAlt
    {
        get;
        private set;
    }

    /// <summary>
    /// The alt glowmask asset associated with this fuel container.
    /// </summary>
    public static Asset<Texture2D> GlowmaskAssetAlt
    {
        get;
        private set;
    }

    /// <summary>
    /// How long this fuel container has existed for, in frames.
    /// </summary>
    public ref float Time => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 7;

        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            GlowmaskAsset = ModContent.Request<Texture2D>($"{Texture}Glowmask");
            TextureAssetAlt = ModContent.Request<Texture2D>($"{Texture}Alt");
            GlowmaskAssetAlt = ModContent.Request<Texture2D>($"{Texture}AltGlowmask");
        }
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 900;
        Projectile.scale = Main.rand?.NextFloat(1f, 1.45f) ?? 1f;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        UpdateVelocity(ref Projectile.velocity);

        // Wait before colliding with tiles.
        Projectile.tileCollide = Time >= 60f && Projectile.velocity.Y > 0f;

        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

        Time++;
    }

    /// <summary>
    /// Performs velocity updates for a given fuel container, making it adhere to gravity and gradually accumulate horizontal drag.
    /// </summary>
    /// <param name="velocity">The velocity to modify.</param>
    public static void UpdateVelocity(ref Vector2 velocity)
    {
        velocity.X *= 0.984f;
        if (velocity.Y < 24f)
            velocity.Y += 0.56f;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);
        ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 4f);

        // Emit a radial spray of short-lived gas particles that compose a cloud.
        for (int i = 0; i < 80; i++)
        {
            float gasScale = Main.rand.NextFloat(1f, 1.4f);
            float gasOpacity = Main.rand.NextFloat(0.15f, 0.2f);
            Color gasColor = Color.Lerp(new Color(191, 199, 9), new Color(124, 239, 87), Main.rand.NextFloat()) * gasOpacity;
            gasColor.A /= 2;

            Vector2 gasVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.4f, 15f);
            NuclearGasParticle gas = new NuclearGasParticle(Projectile.Center, gasVelocity, gasColor, gasScale, Main.rand.Next(40, 90));
            gas.Spawn();
        }

        // Emit bits of slightly lime-tinted glass shards, since this is a fuel container that has been shattered.
        for (int i = 0; i < 24; i++)
        {
            Vector2 glassSpawnPosition = Projectile.Center + Main.rand.NextVector2Circular(10f, 10f);
            Vector2 glassVelocity = Main.rand.NextVector2Circular(20f, 20f);
            Dust glass = Dust.NewDustPerfect(glassSpawnPosition, DustID.Glass, glassVelocity);
            glass.color = Color.Lerp(Color.White, Color.YellowGreen, Main.rand.NextFloat(0.25f));
            glass.scale = Main.rand.NextFloat(1f, 1.6f);
            glass.noGravity = glass.velocity.Length() >= 13.5f;
        }

        // Emit fire.
        for (int i = 0; i < 5; i++)
        {
            float squish = Main.rand.NextFloat(0f, 0.35f);
            float fireScale = Main.rand.NextFloat(50f, 140f);
            Vector2 fireVelocity = Main.rand.NextVector2Circular(30f, 30f);
            Color fireColor = new Color(Main.rand.Next(91, 170), 255, 9);
            OldDukeFireParticleSystemManager.ParticleSystem.CreateNew(Projectile.Center, fireVelocity, new Vector2(1f - squish, 1f) * fireScale, fireColor);
        }

        // Emit sparks.
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            for (int i = 0; i < 3; i++)
            {
                int sparkLifetime = Main.rand.Next(9, 20);
                Vector2 sparkRange = Main.rand.NextVector2Unit() * Main.rand.NextFloat(50f, 210f);
                LumUtils.NewProjectileBetter(Projectile.GetSource_FromAI(), Projectile.Center, sparkRange, ModContent.ProjectileType<FuelSpark>(), 0, 0f, -1, sparkLifetime);
            }
        }
    }

    public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.65f) * Projectile.Opacity;

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Texture2D glowmask = GlowmaskAsset.Value;
        if (Projectile.identity % 2 == 0)
        {
            texture = TextureAssetAlt.Value;
            glowmask = GlowmaskAssetAlt.Value;
        }

        for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            float opacity = (1f - i / (float)Projectile.oldPos.Length).Cubed();
            Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor) * opacity, Projectile.oldRot[i], texture.Size() * 0.5f, Projectile.scale, 0, 0f);
        }

        Main.spriteBatch.PrepareForShaders();

        ManagedShader overlayShader = ShaderManager.GetShader("FargowiltasCrossmod.OldDukeRadioactiveOverlayShader");
        overlayShader.TrySetParameter("blurInterpolant", 0f);
        overlayShader.TrySetParameter("textureSize", texture.Size());
        overlayShader.TrySetParameter("glowColor", new Vector3(4f, 1f, 0.03f));
        overlayShader.TrySetParameter("frame", new Vector4(0f, 0f, texture.Width, texture.Height));
        overlayShader.TrySetParameter("turbulence", 0.2f);
        overlayShader.TrySetParameter("glowInterpolant", 1.5f);
        overlayShader.TrySetParameter("pixelationLevel", 16f);
        overlayShader.SetTexture(NoiseTexturesRegistry.PerlinNoise.Value, 1, SamplerState.LinearWrap);
        overlayShader.Apply();

        Main.spriteBatch.Draw(glowmask, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, glowmask.Size() * 0.5f, Projectile.scale, 0, 0f);

        Main.spriteBatch.ResetToDefault();

        return false;
    }
}
