using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

public class AquaticUrchinProjectile : ModProjectile
{
    /// <summary>
    /// How long this urchin has existed for, in frames.
    /// </summary>
    public ref float Time => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 42;
        Projectile.height = 42;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 360;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        StandardFlyMotion(Projectile, (int)Time);
        Time++;
    }

    public static void StandardFlyMotion(Projectile projectile, int time, bool standardRotation = true)
    {
        // Be affected by the nuclear hurricane.
        NuclearHurricane.AffectProjectileVelocity(projectile);

        // Horizontally decelerate.
        projectile.velocity.X *= 0.98f;

        // Be affected by gravity.
        if (projectile.velocity.Y < 16.5f)
            projectile.velocity.Y += 1.2f;

        // Rotate.
        if (standardRotation)
            projectile.rotation += projectile.velocity.X.NonZeroSign() * projectile.velocity.Length() * 0.016f;

        // Wait before permitting tile collisions.
        projectile.tileCollide = time >= 60;

        // Grow in size.
        projectile.scale = MathHelper.SmoothStep(0f, 1f, LumUtils.InverseLerp(0f, 16f, time));

    }

    public override bool? CanDamage() => Projectile.scale >= 1f;

    public override bool PreDraw(ref Color lightColor)
    {
        LumUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor);
        return false;
    }
}
