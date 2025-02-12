using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

[JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
[ExtendsFromMod(ModCompatibility.Calamity.Name)]
public class SwervingRadioactiveCinder : ModProjectile, IPixelatedPrimitiveRenderer
{
    /// <summary>
    /// How long this cinder has existed, in frames.
    /// </summary>
    public ref float Time => ref Projectile.ai[2];

    public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.hostile = true;
        Projectile.timeLeft = 420;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        // Swerve about.
        float swerveInterpolant = LumUtils.InverseLerp(5f, 24f, Projectile.velocity.Length());
        float swerveSpeed = swerveInterpolant * MathF.Sin(MathHelper.TwoPi * Time / 40f) * 9.3f;
        if (Projectile.identity % 2 == 1)
            swerveSpeed *= -1f;
        Vector2 perpendicular = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
        Projectile.Center += perpendicular * swerveSpeed;

        // Gain a slight bearing towards the nearest player alongside the swerve motion.
        Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
        if (!Projectile.WithinRange(target.Center, 80f))
            Projectile.Center += Projectile.SafeDirectionTo(target.Center) * swerveInterpolant * 6f;

        // Gradually slow down and transform into a lingering projectile.
        Projectile.velocity *= 0.99f;

        // Emit fire particles in accordance with this cinder's hitbox.
        float fireReleaseChance = Utils.Remap(Projectile.velocity.Length(), 2f, 9.5f, 1f, 0.25f);
        if (Main.rand.NextBool(fireReleaseChance))
        {
            float squish = Main.rand.NextFloat(0.25f, 0.54f);
            float fireScale = Projectile.width * Projectile.scale;
            Vector2 fireVelocity = (Projectile.velocity * 1.15f + Main.rand.NextVector2Circular(10f, 10f)) * 2f;
            Color fireColor = new Color(Main.rand.Next(91, 170), 255, 9);
            OldDukeFireParticleSystemManager.ParticleSystem.CreateNew(Projectile.Center, fireVelocity, new Vector2(1f - squish, 1f) * fireScale, fireColor);
        }
        if (Main.rand.NextBool(fireReleaseChance * 1.6f))
        {
            Vector2 fireSpawnPosition = Projectile.Center + Main.rand.NextVector2Circular(12f, 12f) * Projectile.scale;
            Vector2 fireVelocity = Projectile.velocity.RotatedByRandom(0.67f) + Main.rand.NextVector2Circular(6f, 6f);
            Dust nuclearFire = Dust.NewDustPerfect(fireSpawnPosition, 261, fireVelocity);
            nuclearFire.color = Color.Lerp(Color.Lime, Color.Yellow, Main.rand.NextFloat(0.2f, 0.8f));
            nuclearFire.noGravity = nuclearFire.velocity.Length() >= 10f || Main.rand.NextBool(4);
        }

        // Grow as this cinder slows down and becomes a lingering obstacle.
        float maxHitboxSize = MathHelper.Lerp(84f, 125f, Projectile.identity / 13f % 1f);
        if (Projectile.velocity.Length() <= 5.5f && Projectile.scale * Projectile.width < maxHitboxSize)
            Projectile.scale *= 1.022f;
    }

    public float FireWidthFunction(float completionRatio)
    {
        return Projectile.width * Projectile.scale;
    }

    public Color FireColorFunction(float completionRatio) => Projectile.GetAlpha(Color.Lime);

    public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
    {
        ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.FuelSparkShader");
        shader.TrySetParameter("lifetimeRatio", 0.5f);
        shader.TrySetParameter("erasureThreshold", 0.7f);
        shader.SetTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons2"), 1, SamplerState.LinearWrap);
        shader.Apply();

        PrimitiveSettings settings = new(FireWidthFunction, FireColorFunction, Smoothen: false, Pixelate: true, Shader: shader);

        PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 32);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) =>
        LumUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * Projectile.scale * 0.5f, targetHitbox);
}