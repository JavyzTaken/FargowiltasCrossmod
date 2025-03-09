using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HadesMissile : ModProjectile, IProjOwnedByBoss<Apollo>, IPixelatedPrimitiveRenderer, IExoMechProjectile
    {
        /// <summary>
        /// The loop sound for this missile.
        /// </summary>
        internal LoopedSoundInstance LoopInstance;

        /// <summary>
        /// The texture this missile should use.
        /// </summary>
        internal static LazyAsset<Texture2D> MyTexture;

        /// <summary>
        /// The glowmask texture of this missile.
        /// </summary>
        internal static LazyAsset<Texture2D> Glowmask;

        public bool SetActiveFalseInsteadOfKill => true;

        /// <summary>
        /// The amount by which this missile's acceleration factor should be boosted.
        /// </summary>
        public ref float AccelerationBoost => ref Projectile.ai[0];

        /// <summary>
        /// The amount by which this missile's max speed.
        /// </summary>
        public ref float MaxSpeedBoost => ref Projectile.ai[1];

        /// <summary>
        /// How long this missile has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[2];

        /// <summary>
        /// The base maximum speed that this missile can reach. Does not account for <see cref="MaxSpeedBoost"/>.
        /// </summary>
        public static float MaxSpeedup => 18.5f;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Thermal;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;

            if (Main.netMode != NetmodeID.Server)
            {
                MyTexture = LazyAsset<Texture2D>.Request(Texture);
                Glowmask = LazyAsset<Texture2D>.Request($"{Texture}Glow");
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.Calamity().DealsDefenseDamage = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            LoopInstance = LoopedSoundManager.CreateNew(ApolloMissile.LoopSound with { Volume = 0.15f }, () => !Projectile.active);
        }

        public override void AI()
        {
            float homeInAcceleration = LumUtils.InverseLerp(90f, 30f, Time) * 1.4f;
            Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
            Vector2 aimDestination = target.Center + (Projectile.identity / 3f).ToRotationVector2() * (Projectile.identity / 7f % 1f * 200f);

            if (Time <= 30f && Projectile.velocity.Length() > MaxSpeedup + MaxSpeedBoost)
                Projectile.velocity *= 0.93f;
            else
            {
                float acceleration = AccelerationBoost + 1.01f;
                Projectile.velocity = (Projectile.velocity * acceleration + Projectile.SafeDirectionTo(aimDestination) * homeInAcceleration).ClampLength(0f, MaxSpeedup + MaxSpeedBoost);
            }

            Projectile.tileCollide = Time >= 60f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.WithinRange(target.Center, Projectile.velocity.Length() + 5f))
                Projectile.Kill();

            EmitBackSmoke();

            LoopInstance?.Update(Projectile.Center);

            Time++;
        }

        public void EmitBackSmoke()
        {
            int smokeLifetime = Main.rand.Next(12, 24);
            float smokeScale = Main.rand.NextFloat(0.1f, 0.2f);
            Color smokeColor = Color.Lerp(Color.Gray, Color.Black, Main.rand.NextFloat(0.65f)) * 0.2f;
            Vector2 smokeVelocity = -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.31f) * Main.rand.NextFloat(4f, 8.75f) + Projectile.velocity;
            SmokeParticle smoke = new(Projectile.Center - Projectile.velocity * 1.05f, smokeVelocity, smokeColor, smokeLifetime, smokeScale, 0.2f);
            smoke.Spawn();
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaExplosionSound with { Volume = 0.5f, MaxInstances = 0, PitchVariance = 0.068f }, Projectile.Center);

            ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 2f);

            Vector2 blastDirection = Vector2.Lerp(-Projectile.velocity.SafeNormalize(Vector2.UnitY), -Vector2.UnitY, 0.5f);

            for (int i = 0; i < 16; i++)
            {
                int smokeLifetime = Main.rand.Next(30, 67);
                float smokeScale = Main.rand.NextFloat(0.1f, 0.6f);
                Color smokeColor = Color.Lerp(Color.Gray, Color.Black, Main.rand.NextFloat(0.65f));
                Vector2 smokeVelocity = blastDirection.RotatedByRandom(0.31f).SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(12f, 38f);
                SmokeParticle smoke = new(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), smokeVelocity, smokeColor, smokeLifetime, smokeScale, 0.125f);
                smoke.Spawn();
            }

            for (int i = 0; i < 45; i++)
            {
                Color fireColor = Color.Lerp(Color.OrangeRed, Color.Yellow, Main.rand.NextFloat(0.56f)) * Main.rand.NextFloat(0.12f, 0.4f);
                Vector2 fireVelocity = blastDirection.RotatedByRandom(0.54f).SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(4f, 36f);
                SmallSmokeParticle fire = new(Projectile.Center, fireVelocity, fireColor, Color.LightGoldenrodYellow, Main.rand.NextFloat(0.5f, 1f), 180f);
                GeneralParticleHandler.SpawnParticle(fire);
            }

            for (int i = 0; i < 20; i++)
            {
                Dust fire = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20f, 20f), 6);
                fire.fadeIn = 1f;
                fire.noGravity = true;
                fire.velocity = Main.rand.NextVector2Circular(4f, 4f);
                fire.scale = 0.6f;
            }

            StrongBloom bloom = new(Projectile.Center, Vector2.Zero, Color.OrangeRed, 1f, 12);
            GeneralParticleHandler.SpawnParticle(bloom);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = MyTexture.Value;
            Texture2D glowmask = Glowmask.Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            // Draw a bit of a backglow.
            Texture2D glow = MiscTexturesRegistry.BloomCircleSmall.Value;
            Main.spriteBatch.Draw(glow, drawPosition, null, Projectile.GetAlpha(Color.Red with { A = 0 }) * 0.5f, 0f, glow.Size() * 0.5f, Projectile.scale * 0.35f, 0, 0f);
            Main.spriteBatch.Draw(glow, drawPosition, null, Projectile.GetAlpha(Color.Crimson with { A = 0 }) * 0.27f, 0f, glow.Size() * 0.5f, Projectile.scale * 0.5f, 0, 0f);

            Main.spriteBatch.Draw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, 0, 0f);
            Main.spriteBatch.Draw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, 0, 0f);
            return false;
        }

        public float FlameTrailWidthFunction(float completionRatio, float widthFactor)
        {
            float squish = Utils.Remap(Projectile.velocity.Length(), 20f, 54f, 1f, 0.25f);
            float baseWidth = MathHelper.SmoothStep(9f, 2f, completionRatio);
            return baseWidth * widthFactor * squish;
        }

        public static Color FlameTrailColorFunction(float completionRatio, float opacityFactor)
        {
            float trailOpacity = LumUtils.InverseLerp(0.8f, 0.27f, completionRatio) * LumUtils.InverseLerp(0f, 0.067f, completionRatio);
            Color startingColor = Color.Lerp(Color.SkyBlue, Color.White, 0.6f);
            Color middleColor = Color.Lerp(Color.Red, Color.Yellow, 0.32f);
            Color endColor = Color.Lerp(Color.Orange, Color.Red, 0.7f);
            return Utilities.MulticolorLerp(completionRatio, startingColor, middleColor, endColor) * trailOpacity * opacityFactor;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader trailShader = ShaderManager.GetShader("FargowiltasCrossmod.MissileFlameTrailShader");
            trailShader.Apply();

            PrimitiveSettings settings = new(c => FlameTrailWidthFunction(c, 1f), c => FlameTrailColorFunction(c, 1f), _ => (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 16f + Projectile.Size * 0.5f, Pixelate: true, Shader: trailShader);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 14);
        }
    }
}
