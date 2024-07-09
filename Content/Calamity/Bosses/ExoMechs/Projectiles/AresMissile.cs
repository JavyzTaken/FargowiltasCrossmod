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
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AresMissile : ModProjectile, IProjOwnedByBoss<Apollo>, IPixelatedPrimitiveRenderer, IExoMechProjectile
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
        /// The Y position that determines whether this missile can do damage. Once the Y position of this projectile's center exceeds this value, tile collisions are enabled again.
        /// </summary>
        public ref float ExplodeLineY => ref Projectile.ai[0];

        /// <summary>
        /// How long this missile has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[1];

        /// <summary>
        /// The maximum speed that this missile can reach after it begins accelerating.
        /// </summary>
        public static float MaxSpeedup => 15.75f;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Thermal;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
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

            LoopInstance = LoopedSoundManager.CreateNew(ApolloMissile.LoopSound with { Volume = 0.21f }, () => !Projectile.active);
        }

        public override void AI()
        {
            Vector2 sinusoidalOffset = Vector2.UnitX * MathF.Sin(MathHelper.TwoPi * Time / 56f + Projectile.identity * 10f) * 5f;

            Projectile.position += sinusoidalOffset;
            Projectile.velocity.Y += 0.32f;
            Projectile.velocity = (Projectile.velocity * 1.024f).ClampLength(0f, MaxSpeedup);

            Projectile.tileCollide = Projectile.Center.Y >= ExplodeLineY;
            Projectile.rotation = (Projectile.velocity + sinusoidalOffset).ToRotation() + MathHelper.PiOver2;
            Projectile.frame = (int)Time / 5 % Main.projFrames[Type];

            EmitBackSmoke();

            LoopInstance?.Update(Projectile.Center);

            Time++;
        }

        public void EmitBackSmoke()
        {
            for (int i = 0; i < 2; i++)
            {
                int smokeLifetime = Main.rand.Next(12, 24);
                float smokeScale = Main.rand.NextFloat(0.1f, 0.2f);
                Color smokeColor = Color.Lerp(Color.Gray, Color.Black, Main.rand.NextFloat(0.65f)) * 0.2f;
                Vector2 smokeVelocity = -Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.31f) * Main.rand.NextFloat(6f, 12f) + Projectile.velocity;
                SmokeParticle smoke = new(Projectile.Center - Projectile.velocity * 1.05f, smokeVelocity, smokeColor, smokeLifetime, smokeScale, 0.2f);
                smoke.Spawn();
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaExplosionSound with { Volume = 0.5f, MaxInstances = 0 }, Projectile.Center);

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

            StrongBloom bloom = new(Projectile.Center, Vector2.Zero, Color.Orange, 1f, 12);
            GeneralParticleHandler.SpawnParticle(bloom);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = MyTexture.Value;
            Texture2D glowmask = Glowmask.Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, 0, 0f);
            Main.spriteBatch.Draw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, 0, 0f);
            return false;
        }

        public static float FlameTrailWidthFunction(float completionRatio, float widthFactor)
        {
            float baseWidth = MathHelper.SmoothStep(8f, 2f, completionRatio);
            return baseWidth * widthFactor;
        }

        public static Color FlameTrailColorFunction(float completionRatio, float opacityFactor)
        {
            float trailOpacity = LumUtils.InverseLerp(0.8f, 0.27f, completionRatio) * LumUtils.InverseLerp(0f, 0.067f, completionRatio);
            Color startingColor = Color.Lerp(Color.SkyBlue, Color.White, 0.6f);
            Color middleColor = Color.Lerp(Color.Orange, Color.Yellow, 0.32f);
            Color endColor = Color.Lerp(Color.Orange, Color.Red, 0.29f);
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
