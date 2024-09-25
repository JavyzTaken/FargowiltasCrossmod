using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Assets.Particles.Dusts;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using Luminance.Common.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ApolloPlasmaFireball : ModProjectile, IProjOwnedByBoss<Apollo>, IExoMechProjectile
    {
        /// <summary>
        /// Whether this fireball has created its initial burst of particles or not.
        /// </summary>
        public bool HasCreatedInitialBurst
        {
            get => Projectile.localAI[0] == 1f;
            set => Projectile.localAI[0] = value.ToInt();
        }

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
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 60;
            Projectile.Calamity().DealsDefenseDamage = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.Opacity = LumUtils.InverseLerp(0f, 3f, Time);
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Type];
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (!HasCreatedInitialBurst)
            {
                CreateBurstDust();
                HasCreatedInitialBurst = true;
            }

            Time++;
        }

        /// <summary>
        /// Creates a burst of forward-firing dust for this fireball.
        /// </summary>
        public void CreateBurstDust()
        {
            for (int j = 0; j < 20; j++)
            {
                int dustID = Main.rand.NextBool() ? 107 : 110;
                Vector2 plasmaVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.35f) * Main.rand.NextFloat(2f, 3f) + Projectile.velocity * 0.25f;
                Dust plasma = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustID, plasmaVelocity.X, plasmaVelocity.Y, 0, default, 2f);
                plasma.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                plasma.noGravity = true;
                plasma.velocity *= 0.5f;
            }
            for (int i = 0; i < 40; i++)
            {
                Vector2 plasmaVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.35f) * Main.rand.NextFloat(2f, 3f) + Projectile.velocity * 0.25f;
                int dustID = Main.rand.NextBool() ? 107 : 110;

                Dust plasmaDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustID, plasmaVelocity.X, plasmaVelocity.Y, 200, default, 1.7f);
                plasmaDust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                plasmaDust.noGravity = true;
                plasmaDust.velocity *= 3f;

                plasmaDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustID, plasmaVelocity.X, plasmaVelocity.Y, 100, default, 0.8f);
                plasmaDust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                plasmaDust.velocity *= 2f;
                plasmaDust.noGravity = true;
                plasmaDust.fadeIn = 1f;
                plasmaDust.color = Color.Green * 0.5f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaExplosionSound, Projectile.Center);

            int sparkID = ModContent.DustType<TwinkleDust>();
            for (int i = 0; i < 20; i++)
            {
                Dust plasmaSpark = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(124f, 124f), sparkID);
                plasmaSpark.color = Color.Lerp(Color.Wheat, Color.Yellow, Main.rand.NextFloat());
                plasmaSpark.scale = Main.rand.NextFloat(0.4f, 0.5f);
                plasmaSpark.velocity = Main.rand.NextVector2Circular(1f, 1f);
            }

            // Create the generic burst explosion.
            MagicBurstParticle burst = new(Projectile.Center, Vector2.Zero, new(140, 221, 64), 12, 0.85f);
            burst.Spawn();

            // Create smoke.
            for (int i = 0; i < 45; i++)
            {
                float smokeRotationalVelocity = Main.rand.NextFloatDirection() * 0.012f;
                Vector2 smokeVelocity = Main.rand.NextVector2Circular(30f, 19.5f) - Vector2.UnitY * Main.rand.NextFloat(2f, 2.8f);
                if (Main.rand.NextBool())
                    smokeVelocity.Y *= -0.4f;

                HeavySmokeParticle smoke = new(Projectile.Center, smokeVelocity, Color.Lerp(Color.LightGray, Color.Brown, Main.rand.NextFloat(0.5f)), Main.rand.Next(120, 180), 0.7f, 1f, smokeRotationalVelocity, false);
                GeneralParticleHandler.SpawnParticle(smoke);
            }

            // Create inner lingering plasma fire.
            for (int i = 0; i < 85; i++)
            {
                int fireLifetime = Main.rand.Next(25, 50);
                float fireRotationalVelocity = Main.rand.NextFloatDirection() * 0.08f;
                float fireScale = Main.rand.NextFloat(0.5f, 0.75f);
                Vector2 fireVelocity = Main.rand.NextVector2Circular(12f, 12f);
                if (Main.rand.NextBool())
                    fireVelocity.Y *= -0.6f;

                HeavySmokeParticle fire = new(Projectile.Center, fireVelocity, Color.Lerp(Color.Green, Color.Wheat, Main.rand.NextFloat(0.75f)), fireLifetime, fireScale, 1f, fireRotationalVelocity, true);
                GeneralParticleHandler.SpawnParticle(fire);
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            LumUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
