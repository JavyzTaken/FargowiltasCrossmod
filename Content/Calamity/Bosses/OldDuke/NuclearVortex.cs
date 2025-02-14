using Luminance.Assets;
using Luminance.Common.Easings;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

public class NuclearVortex : ModProjectile
{
    /// <summary>
    /// How long this vortex has existed for, in frames.
    /// </summary>
    public ref float Time => ref Projectile.ai[0];

    public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
    }

    public override void SetDefaults()
    {
        Projectile.width = 1376;
        Projectile.height = 1376;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 240;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        float scaleGrowthInterpolant = LumUtils.InverseLerp(30f, 120f, Time);
        float fadeOut = LumUtils.InverseLerp(60f, 0f, Projectile.timeLeft);
        Projectile.scale = EasingCurves.Cubic.Evaluate(EasingType.InOut, scaleGrowthInterpolant) + fadeOut * 1.1f;
        Projectile.Opacity = 1f - fadeOut;
        if (Projectile.Opacity <= 0.7f)
            Projectile.damage = 0;

        Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
        Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, Projectile.SafeDirectionTo(target.Center).X * 5f, 0.05f);

        if (Projectile.Opacity >= 0.85f)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 fireSpawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height) * Projectile.scale * 0.45f;
                Vector2 fireVelocity = Projectile.SafeDirectionTo(fireSpawnPosition).RotatedBy(-MathHelper.PiOver2) * 9f;
                Dust fire = Dust.NewDustPerfect(fireSpawnPosition, 261, fireVelocity);
                fire.scale = Main.rand.NextFloat(0.9f, 1.45f);
                fire.noGravity = true;
                fire.color = Color.YellowGreen;
            }
        }

        Time++;
    }

    public override Color? GetAlpha(Color lightColor) => lightColor * Projectile.Opacity;

    public override bool PreDraw(ref Color lightColor)
    {
        Main.spriteBatch.PrepareForShaders();

        ManagedShader vortexShader = ShaderManager.GetShader("FargowiltasCrossmod.OldDukeVortexShader");
        vortexShader.TrySetParameter("pixelationLevel", Projectile.width * 0.5f);
        vortexShader.SetTexture(MiscTexturesRegistry.DendriticNoiseZoomedOut.Value, 1, SamplerState.LinearWrap);
        vortexShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 2, SamplerState.LinearWrap);
        vortexShader.Apply();

        Texture2D pixel = MiscTexturesRegistry.InvisiblePixel.Value;
        Vector2 drawPosition = Projectile.Center - Main.screenPosition;
        Vector2 scale = Projectile.Size * Projectile.scale;
        Main.spriteBatch.Draw(pixel, drawPosition, null, Projectile.GetAlpha(new Color(160, 224, 40)), 0f, pixel.Size() * 0.5f, scale / pixel.Size(), 0, 0f);

        Main.spriteBatch.ResetToDefault();

        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return LumUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * Projectile.scale * 0.42f, targetHitbox) && Time >= 60f;
    }
}
