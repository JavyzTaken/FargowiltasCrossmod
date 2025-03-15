using FargowiltasCrossmod.Assets;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
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
        Projectile.width = 30;
        Projectile.height = 30;
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
        float swerveInterpolant = LumUtils.InverseLerp(5f, 10f, Projectile.velocity.Length());
        float swerveSpeed = swerveInterpolant * MathF.Sin(MathHelper.TwoPi * Time / 40f) * 6f;
        if (Projectile.identity % 2 == 1)
            swerveSpeed *= -1f;
        Vector2 perpendicular = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
        Projectile.Center += perpendicular * swerveSpeed;

        // Gain a slight bearing towards the nearest player alongside the swerve motion.
        Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
        if (!Projectile.WithinRange(target.Center, 80f))
            Projectile.Center += Projectile.SafeDirectionTo(target.Center) * swerveInterpolant * 3f;

        // Gradually slow down and transform into a lingering projectile.
        Projectile.velocity *= 0.985f;

        // Emit fire particles in accordance with this cinder's hitbox.
        float fireReleaseChance = Utils.Remap(Projectile.velocity.Length(), 2f, 9.5f, 1f, 0.04f);
        if (Main.rand.NextBool(fireReleaseChance))
        {
            for (int i = 0; i < 2; i++)
            {
                float squish = Main.rand.NextFloat(0.1f, 0.4f);
                float fireScale = Projectile.width * Projectile.scale;
                Vector2 fireSpawnPosition = Projectile.Center;

                float spinInterpolant = LumUtils.InverseLerp(8f, 4f, Projectile.velocity.Length());
                fireSpawnPosition += Main.rand.NextVector2Circular(0.4f, 0.4f) * Projectile.width * Projectile.scale * spinInterpolant;

                Vector2 fireVelocity = Projectile.SafeDirectionTo(fireSpawnPosition).RotatedBy(spinInterpolant * MathHelper.PiOver2) * Main.rand.NextFloat(10f);
                Color fireColor = new Color(Main.rand.Next(91, 170), 255, 9);
                OldDukeFireParticleSystemManager.ParticleSystem.CreateNew(fireSpawnPosition, fireVelocity, new Vector2(1f - squish, 1f) * fireScale, fireColor);
            }
        }
        if (Main.rand.NextBool(fireReleaseChance * 6f))
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 fireSpawnPosition = Projectile.Center + Main.rand.NextVector2Circular(12f, 12f) * Projectile.scale;
                Vector2 fireVelocity = Main.rand.NextVector2Circular(4f, 4f) * MathF.Pow(Projectile.scale, 0.61f);
                Dust nuclearFire = Dust.NewDustPerfect(fireSpawnPosition, 261, fireVelocity * MathF.Pow(Projectile.scale * 1.5f, 0.9f));
                nuclearFire.color = Color.Lerp(Color.Lime, Color.Yellow, Main.rand.NextFloat(0.2f, 0.8f));
                nuclearFire.noGravity = true;
                nuclearFire.scale *= Main.rand.NextFloat(0.7f, 1.15f);
            }
        }

        // Grow as this cinder slows down and becomes a lingering obstacle.
        float maxHitboxSize = MathHelper.Lerp(84f, 125f, Projectile.identity / 13f % 1f);
        if (Projectile.velocity.Length() <= 5.5f && Projectile.scale * Projectile.width < maxHitboxSize)
            Projectile.scale *= 1.022f;

        Time++;
    }

    public float FireWidthFunction(float completionRatio)
    {
        float tip = MathF.Sqrt(LumUtils.InverseLerp(0.034f, 0.14f, completionRatio));
        float squish = LumUtils.InverseLerp(3f, 6f, Projectile.velocity.Length());
        float width = Projectile.width * MathHelper.Lerp(1f, 1.35f, LumUtils.Cos01(completionRatio * MathHelper.Pi * 4f + Projectile.identity * 4f - Main.GlobalTimeWrappedHourly * 13f));
        return width * Projectile.scale * squish * (1f - completionRatio) * tip * 0.5f;
    }

    public Color FireColorFunction(float completionRatio)
    {
        Color fireColor = Color.Lerp(new Color(120, 255, 23), new Color(255, 255, 174), Projectile.identity / 7f % 1f);
        return Projectile.GetAlpha(fireColor) * LumUtils.InverseLerp(3f, 6f, Projectile.velocity.Length());
    }

    public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
    {
        ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.RadioactiveCinderShader");
        shader.SetTexture(NoiseTexturesRegistry.CrackedNoiseA.Value, 1, SamplerState.LinearWrap);
        shader.SetTexture(TextureAssets.Extra[ExtrasID.FlameLashTrailShape].Value, 2, SamplerState.LinearWrap);
        shader.Apply();

        PrimitiveSettings settings = new(FireWidthFunction, FireColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader);

        PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 33);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) =>
        LumUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * Projectile.scale * 0.5f, targetHitbox);
}