using FargowiltasCrossmod.Assets;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
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
        public static int Lifetime => Utilities.SecondsToFrames(3f);

        /// <summary>
        /// The maximum length of this current is.
        /// </summary>
        public static float MaxLaserbeamLength => 8008f;

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
            LaserbeamLength = MathHelper.Clamp(LaserbeamLength + 300f, 0f, MaxLaserbeamLength);
            Projectile.Opacity = LumUtils.InverseLerp(0f, 18f, Time) * LumUtils.InverseLerp(0f, 30f, Projectile.timeLeft).Squared();
            Projectile.scale = LumUtils.InverseLerp(0f, 8f, Time) + LumUtils.InverseLerp(20f, 0f, Projectile.timeLeft);
            CreateSinusoidalParticles();

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

        public float BeamWidthFunction(float completionRatio)
        {
            return Projectile.width * Projectile.scale;
        }

        public Color BeamColorFunction(float completionRatio)
        {
            return Projectile.GetAlpha(new Color(0.4f, 1f, 1f, 0f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
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
