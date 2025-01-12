using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
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
    public class LingeringPlasmaFireball : ModProjectile, IProjOwnedByBoss<Apollo>, IExoMechProjectile
    {
        /// <summary>
        /// How long this fireball has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Plasma;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 420;
            Projectile.Calamity().DealsDefenseDamage = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.96f;
            Projectile.frame = (int)Time / 5 % Main.projFrames[Type];
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Time++;

            if (Projectile.width < 140)
                Projectile.Resize(Projectile.width + 3, Projectile.height + 3);

            if (Projectile.WithinRange(Main.LocalPlayer.Center, 900f))
            {
                for (int i = 0; i < Projectile.width / 44; i++)
                {
                    Color gasColor = Color.Lerp(Color.Lime, Color.Yellow, Main.rand.NextFloat());
                    gasColor.A /= 6;

                    Vector2 plasmaSpawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * 0.707f, Projectile.height * 0.707f);
                    SmallSmokeParticle gas = new(plasmaSpawnPosition, Main.rand.NextVector2Circular(4f, 4f), gasColor, Color.YellowGreen with { A = 0 }, 2f, 60f);
                    GeneralParticleHandler.SpawnParticle(gas);
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            => Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < projHitbox.Width / 2;
        public override bool? CanDamage() => Projectile.Opacity >= 0.5f && Time >= 95f;

        public override bool PreDraw(ref Color lightColor)
        {
            Utilities.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], Color.White, positionClumpInterpolant: 0.45f);
            return false;
        }
    }
}
