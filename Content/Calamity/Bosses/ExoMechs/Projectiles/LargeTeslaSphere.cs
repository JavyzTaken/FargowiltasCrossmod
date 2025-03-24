using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Particles;
using FargowiltasCrossmod.Assets;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
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
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class LargeTeslaSphere : ModProjectile, IProjOwnedByBoss<AresBody>, IExoMechProjectile
    {
        public bool SetActiveFalseInsteadOfKill => true;

        /// <summary>
        /// The loop sound instance for this sphere.
        /// </summary>
        public LoopedSoundInstance LoopSoundInstance
        {
            get;
            private set;
        }

        /// <summary>
        /// How long this sphere has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        /// <summary>
        /// The effective amount of spin that has elapsed thus far. Used by the sphere shader.
        /// </summary>
        public ref float SphereSpinScrollOffset => ref Projectile.localAI[0];

        /// <summary>
        /// The sound played idly by this sphere.
        /// </summary>
        public static readonly SoundStyle LoopSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/TeslaSphereLoop");

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Electricity;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 240000;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.draedonExoMechPrime == -1)
            {
                Projectile.Kill();
                return;
            }

            for (int i = 0; i < 2; i++)
                CreateElectricSpark();

            CreateElectricPixel();

            if (Time % 13 == 12 && Projectile.velocity.Length() <= 11f)
                CreateConvergingCircleParticle();

            LoopSoundInstance ??= LoopedSoundManager.CreateNew(LoopSound, () => !Projectile.active);
            LoopSoundInstance?.Update(Projectile.Center, sound =>
            {
                sound.Volume = LumUtils.InverseLerp(0f, 540f, Projectile.width) * 1.5f;
            });

            SphereSpinScrollOffset += Projectile.width * 0.000023f;

            Time++;
        }

        /// <summary>
        /// Creates a single electric spark around the sphere's edge.
        /// </summary>
        public void CreateElectricSpark()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int arcLifetime = Main.rand.Next(9, 16);
            Vector2 arcSpawnPosition = Projectile.Center + Main.rand.NextVector2Unit() * (Projectile.width * 0.41f - Main.rand.NextFloat(40f, 95f));
            Vector2 arcLength = Main.rand.NextVector2Unit() * Main.rand.NextFloat(40f, Projectile.width * 0.034f + 60f);

            if (Vector2.Dot(arcLength, Projectile.Center - arcSpawnPosition) > 0f)
                arcLength *= -1f;

            if (Main.rand.NextBool(3))
                arcLength *= 1.3f;
            if (Main.rand.NextBool(3))
                arcLength *= 1.3f;
            if (Main.rand.NextBool(5))
                arcLength *= 1.5f;
            if (Main.rand.NextBool(5))
                arcLength *= 1.5f;

            Utilities.NewProjectileBetter(Projectile.GetSource_FromThis(), arcSpawnPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, arcLifetime, 0f);
        }

        /// <summary>
        /// Creates a single electric pixel around the sphere's edge.
        /// </summary>
        public void CreateElectricPixel()
        {
            Vector2? pixelHomeDestination = Main.rand.NextBool(6) ? Projectile.Center : null;
            float pixelOffsetAngle = Main.rand.NextBool(10) || pixelHomeDestination is not null ? Main.rand.NextFloat(0.9f, 1.4f) : Main.rand.NextFloat(-0.3f, 0.3f);

            Vector2 pixelSpawnPosition = Projectile.Center + Main.rand.NextVector2Unit() * (Projectile.width * 0.5f + Main.rand.NextFloat(50f, 175f));

            float pixelSpeedFactor = Main.rand.NextFloat(0.019f, 0.07f);
            Vector2 pixelVelocity = (Projectile.Center - pixelSpawnPosition).RotatedBy(pixelOffsetAngle) * pixelSpeedFactor;

            BloomPixelParticle pixel = new(pixelSpawnPosition, pixelVelocity, Color.White, Color.DeepSkyBlue * 0.4f, 23, Vector2.One * Main.rand.NextFloat(1f, 1.85f), pixelHomeDestination);
            pixel.Spawn();
        }

        /// <summary>
        /// Creates a single converging circle particle that pulses inward onto the origin of the sphere.
        /// </summary>
        public void CreateConvergingCircleParticle()
        {
            HollowCircleParticle circle = new(Projectile.Center, Vector2.Zero, Color.CadetBlue, 11, Projectile.width / 85f, 0.6f, 0.72f);
            circle.Spawn();
        }

        public override void OnKill(int timeLeft)
        {
            ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 11f);
            SoundEngine.PlaySound(TeslaCannon.FireSound with { Volume = 3f });

            StrongBloom impactBloom = new(Projectile.Center, Vector2.Zero, Color.White, 3f, 5);
            GeneralParticleHandler.SpawnParticle(impactBloom);
            impactBloom = new(Projectile.Center, Vector2.Zero, Color.White, 3f, 16);
            GeneralParticleHandler.SpawnParticle(impactBloom);

            StrongBloom backBloom = new(Projectile.Center, Vector2.Zero, new(0.34f, 0.5f, 1f), 4f, 40);
            GeneralParticleHandler.SpawnParticle(backBloom);

            for (int i = 0; i < 35; i++)
            {
                Color streakColor = Color.Lerp(Color.Aqua, Color.DeepSkyBlue, Main.rand.NextFloat(0.5f));
                streakColor = Color.Lerp(streakColor, Color.White, 0.5f);

                Vector2 streakVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(7.5f, 40f);
                Vector2 streakScale = new(2f, 0.028f);
                Vector2 endingStreakScale = new(0.95f, 0.08f);

                LineStreakParticle streak = new(Projectile.Center, streakVelocity, streakColor, Main.rand.Next(25, 54), streakVelocity.ToRotation(), streakScale, endingStreakScale);
                streak.Spawn();
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            for (int i = 0; i < 20; i++)
            {
                Vector2 burstVelocity = (MathHelper.TwoPi * i / 20f).ToRotationVector2() * 0.54f;
                Utilities.NewProjectileBetter(Projectile.GetSource_FromThis(), Projectile.Center, burstVelocity, ModContent.ProjectileType<HomingTeslaBurst>(), AresBodyEternity.TeslaBurstDamage, 0f, -1, HomingTeslaBurst.HomeInTime);
            }

            for (int i = 0; i < 9; i++)
            {
                Vector2 burstVelocity = (MathHelper.TwoPi * i / 9f + MathHelper.Pi / 6f).ToRotationVector2() * 0.92f;
                Utilities.NewProjectileBetter(Projectile.GetSource_FromThis(), Projectile.Center, burstVelocity, ModContent.ProjectileType<HomingTeslaBurst>(), AresBodyEternity.TeslaBurstDamage, 0f, -1, HomingTeslaBurst.HomeInTime);
            }

            Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
            for (int i = 0; i < 7; i++)
            {
                Vector2 burstVelocity = Projectile.SafeDirectionTo(target.Center).RotatedBy(MathHelper.Lerp(-0.51f, 0.51f, i / 6f)) * 1.4f;
                Utilities.NewProjectileBetter(Projectile.GetSource_FromThis(), Projectile.Center, burstVelocity, ModContent.ProjectileType<HomingTeslaBurst>(), AresBodyEternity.TeslaBurstDamage, 0f, -1, HomingTeslaBurst.HomeInTime);
            }

            Utilities.NewProjectileBetter(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LargeTeslaSphereExplosion>(), 0, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Texture2D bloom = MiscTexturesRegistry.BloomCircleSmall.Value;
            Main.spriteBatch.Draw(bloom, drawPosition, null, Projectile.GetAlpha(new(1f, 1f, 1f, 0f)) * 0.5f, 0f, bloom.Size() * 0.5f, Projectile.Size / bloom.Size() * 1.8f, 0, 0f);
            Main.spriteBatch.Draw(bloom, drawPosition, null, Projectile.GetAlpha(new(0.34f, 0.5f, 1f, 0f)) * 0.4f, 0f, bloom.Size() * 0.5f, Projectile.Size / bloom.Size() * 3f, 0, 0f);

            Main.spriteBatch.PrepareForShaders();

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.LargeTeslaSphereShader");
            shader.SetTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/HarshNoise"), 1, SamplerState.LinearWrap);
            shader.SetTexture(NoiseTexturesRegistry.ElectricNoise.Value, 2, SamplerState.LinearWrap);
            shader.TrySetParameter("textureSize0", Projectile.Size);
            shader.TrySetParameter("posterizationPrecision", 14f);
            shader.SetTexture(MiscTexturesRegistry.DendriticNoiseZoomedOut.Value, 1, SamplerState.LinearWrap);
            shader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 2, SamplerState.LinearWrap);
            shader.Apply();

            Texture2D pixel = MiscTexturesRegistry.Pixel.Value;
            Main.spriteBatch.Draw(pixel, drawPosition, null, Projectile.GetAlpha(new(0.7f, 1f, 1f)), 0f, pixel.Size() * 0.5f, Projectile.Size * Projectile.scale / pixel.Size() * 1.2f, 0, 0f);

            Main.spriteBatch.ResetToDefault();

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) =>
            Utilities.CircularHitboxCollision(Projectile.Center, Projectile.width * 0.37f, targetHitbox);
    }
}
