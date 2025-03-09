using FargowiltasCrossmod.Assets;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Assets.Particles.Metaballs;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.Easings;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    // Note to self: If you copypaste this projectile don't forget to add the appropriate IProjOwnedByBoss interface.
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CodebreakerDataStream : ModProjectile
    {
        /// <summary>
        /// How long this sphere has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        /// <summary>
        /// How long this beam currently is.
        /// </summary>
        public ref float LaserbeamLength => ref Projectile.ai[1];

        /// <summary>
        /// How long this beam should exist for, in frames.
        /// </summary>
        public static int Lifetime => LumUtils.SecondsToFrames(3f);

        /// <summary>
        /// How long it takes for the beam to appear on the Codebreaker.
        /// </summary>
        public static int AppearDelay => LumUtils.SecondsToFrames(0.6f);

        /// <summary>
        /// The maximum length of this beam.
        /// </summary>
        public static float MaxLaserbeamLength => 8008f;

        /// <summary>
        /// The sound played when this beam appears and persists throughout its lifetime.
        /// </summary>
        public static readonly SoundStyle DroneSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/GeneralExoMechs/CodebreakerDrone") with { Volume = 1.4f };

        /// <summary>
        /// The sound played when this beam appears.
        /// </summary>
        public static readonly SoundStyle PulseSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/GeneralExoMechs/CodebreakerPulse") with { Volume = 0.95f };

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 6000;

        public override void SetDefaults()
        {
            Projectile.width = 27;
            Projectile.height = 27;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Lifetime;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (Time == 1f)
                SoundEngine.PlaySound(DroneSound, Projectile.Center);

            Projectile.Opacity = LumUtils.InverseLerp(0f, 12f, Time - AppearDelay) * LumUtils.InverseLerp(0f, 30f, Projectile.timeLeft).Squared();
            Projectile.scale = LumUtils.InverseLerp(0f, 4f, Time - AppearDelay) + LumUtils.InverseLerp(20f, 0f, Projectile.timeLeft);

            if (Time >= AppearDelay)
            {
                LaserbeamLength = MathHelper.Clamp(LaserbeamLength + 150f, 0f, MaxLaserbeamLength);
                CreateSinusoidalParticles();
                CreateSquareParticles();

                if (Time % 25 == 0)
                    SoundEngine.PlaySound(PulseSound with { MaxInstances = 0, Volume = 1.3f }, Projectile.Center);
            }
            ModContent.GetInstance<HeatDistortionMetaball>().CreateParticle(Projectile.Center, Main.rand.NextVector2Unit(12f, 12f), 80f);

            Time++;
        }

        /// <summary>
        /// Creates particles along the beam that loosely follow a sinusoidal motion.
        /// </summary>
        public void CreateSinusoidalParticles()
        {
            for (int i = 0; i < 18; i++)
            {
                float energyLengthInterpolant = (Main.rand.NextFloat(0.12f) + Time / 12f) % 1f;
                Vector2 energySpawnPosition = Projectile.Center + Projectile.velocity * energyLengthInterpolant * LaserbeamLength + Main.rand.NextVector2Circular(12f, 12f);
                Vector2 energyVelocity = Projectile.velocity.RotatedBy(MathF.Sin(energyLengthInterpolant * 120f + Time / 20f) * 0.7f) * new Vector2(3f, 8f);

                BloomPixelParticle energy = new(energySpawnPosition, energyVelocity, Color.White, Color.DeepSkyBlue * 0.4f, 27, Vector2.One * Main.rand.NextFloat(0.85f, 1.5f));
                energy.Spawn();
            }
        }

        /// <summary>
        /// Creates square particles along the beam.
        /// </summary>
        public void CreateSquareParticles()
        {
            if (Main.rand.NextBool(4))
            {
                float positionInterpolant = Main.rand.NextFloat();
                float scaleInterpolant = MathF.Pow(1f - positionInterpolant, 1.9f);
                float scale = MathHelper.Lerp(0.1f, 0.27f, scaleInterpolant);

                Vector2 squareSpawnPosition = Projectile.Center + Projectile.velocity * LaserbeamLength * positionInterpolant * 0.03f;
                Vector2 squareVelocity = (Main.rand.NextVector2Circular(6f, 3f) - Vector2.UnitY * 15f) * (1.332f - scaleInterpolant);
                Color squareColor = Color.Lerp(Color.DeepSkyBlue, Color.Cyan, Main.rand.NextFloat(0.4f, 0.8f));
                GlowySquareParticle square = new(squareSpawnPosition, squareVelocity, Projectile.GetAlpha(Color.White), Projectile.GetAlpha(squareColor), Main.rand.Next(35, 60), Vector2.One * scale);
                square.Spawn();
            }
        }

        public float BeamWidthFunction(float completionRatio)
        {
            return Projectile.width * Projectile.scale;
        }

        public Color BeamColorFunction(float completionRatio)
        {
            return Projectile.GetAlpha(new Color(0.4f, 1f, 1f, 0f));
        }

        public void DrawLight()
        {
            Texture2D light = MiscTexturesRegistry.BloomCircleSmall.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float scaleFadeIn = EasingCurves.Elastic.Evaluate(EasingType.Out, LumUtils.InverseLerp(0f, 55f, Time));
            float scaleFadeOut = LumUtils.InverseLerp(0f, 10f, Projectile.timeLeft);
            float opacityFadeIn = LumUtils.InverseLerp(0f, 32f, Time);
            float scale = MathHelper.Lerp(0.25f, 0.3f, LumUtils.Cos01(Main.GlobalTimeWrappedHourly * 60f)) * scaleFadeIn * scaleFadeOut * (1f + Time / Lifetime);

            Color lightColorA = Color.Aqua with { A = 0 } * opacityFadeIn * 0.5f;
            Color lightColorB = Color.White with { A = 0 } * opacityFadeIn;
            Main.spriteBatch.Draw(light, drawPosition, null, lightColorA, 0f, light.Size() * 0.5f, scale * 0.75f, 0, 0f);
            Main.spriteBatch.Draw(light, drawPosition, null, lightColorB, 0f, light.Size() * 0.5f, scale, 0, 0f);

            float pulse = Main.GlobalTimeWrappedHourly * 2.5f % 1f;
            float pulseScaleFactor = Time / Lifetime * 4f;
            Main.spriteBatch.Draw(light, drawPosition, null, lightColorB * (1f - pulse) * scaleFadeOut, 0f, light.Size() * 0.5f, pulse * pulseScaleFactor, 0, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawLight();

            List<Vector2> beamPositions = Projectile.GetLaserControlPoints(12, LaserbeamLength);

            ManagedShader dataShader = ShaderManager.GetShader("FargowiltasCrossmod.DataStreamShader");
            dataShader.SetTexture(NoiseTexturesRegistry.BinaryPoem.Value, 1, SamplerState.PointClamp);

            PrimitiveSettings laserSettings = new(BeamWidthFunction, BeamColorFunction, Shader: dataShader);
            PrimitiveRenderer.RenderTrail(beamPositions, laserSettings, 80);
            return false;
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
