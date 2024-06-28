using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SmallTeslaSphere : ModProjectile, IExoMechProjectile, IProjOwnedByBoss<AresBody>
    {
        public ExoMechDamageSource DamageType => ExoMechDamageSource.Electricity;

        /// <summary>
        /// The general purpose frame timer of this sphere.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        /// <summary>
        /// How long this sphere should exist for, in frames.
        /// </summary>
        public static int Lifetime => Utilities.SecondsToFrames(5f);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.Calamity().DealsDefenseDamage = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 6 % Main.projFrames[Projectile.type];
            Projectile.velocity = (Projectile.velocity * 1.015f).ClampLength(0f, 30f);
            Projectile.Opacity = LumUtils.InverseLerp(0f, 45f, Projectile.timeLeft);

            DelegateMethods.v3_1 = new Vector3(0.24f, 0.86f, 1f);
            Utils.PlotTileLine(Projectile.Top, Projectile.Bottom, Projectile.width, DelegateMethods.CastLight);

            Time++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            LumUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], Color.White, 1);
            return false;
        }

        public override bool? CanDamage() => Projectile.Opacity >= 0.75f;
    }
}
