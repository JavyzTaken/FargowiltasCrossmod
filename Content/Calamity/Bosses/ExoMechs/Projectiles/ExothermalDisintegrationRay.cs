using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Artemis;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.DataStructures;
using Luminance.Common.Easings;
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
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExothermalDisintegrationRay : ModProjectile, IPixelatedPrimitiveRenderer, IProjOwnedByBoss<Artemis>, IExoMechProjectile
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterProjectiles;

        /// <summary>
        /// How long this sphere has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        /// <summary>
        /// How long this laserbeam currently is.
        /// </summary>
        public ref float LaserbeamLength => ref Projectile.ai[1];

        /// <summary>
        /// How long this laserbeam should exist for, in frames.
        /// </summary>
        public static int Lifetime => Utilities.SecondsToFrames(2.2f);

        /// <summary>
        /// The maximum length of this laserbeam.
        /// </summary>
        public static float MaxLaserbeamLength => 6000f;

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Thermal;

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 6000;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = Lifetime;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            int artemisIndex = CalamityGlobalNPC.draedonExoMechTwinRed;
            if (artemisIndex < 0 || artemisIndex >= Main.maxNPCs || Main.npc[artemisIndex].type != ModContent.NPCType<Artemis>())
            {
                Projectile.Kill();
                return;
            }

            NPC artemis = Main.npc[artemisIndex];
            Vector2 beamStart = artemis.Center + artemis.rotation.ToRotationVector2() * 68f;
            Projectile.Center = beamStart;
            Projectile.velocity = artemis.SafeDirectionTo(beamStart);
            LaserbeamLength = MathHelper.Clamp(LaserbeamLength + 400f, 0f, MaxLaserbeamLength);

            float scaleFactor = EasingCurves.Elastic.Evaluate(EasingType.Out, Utilities.InverseLerp(0f, 60f, Time)) * Utilities.InverseLerp(0f, 12f, Projectile.timeLeft);
            Projectile.width = (int)(scaleFactor * 100f);

            for (int i = 0; i < 18; i++)
            {
                Vector2 fireSpawnPosition = Projectile.Center + Projectile.velocity * Main.rand.NextFloat(LaserbeamLength * 0.9f);
                Vector2 fireVelocity = Main.rand.NextVector2Circular(14f, 14f) + artemis.velocity;
                Color fireGlowColor = Utilities.MulticolorLerp(Main.rand.NextFloat(0.75f), Color.Yellow, Color.Orange, Color.Red) * Main.rand.NextFloat(0.5f, 0.8f);
                Vector2 fireGlowScaleFactor = Vector2.One * Main.rand.NextFloat(0.095f, 0.175f);
                BloomPixelParticle fire = new(fireSpawnPosition, fireVelocity, Color.White, fireGlowColor, Main.rand.Next(17, 37), Vector2.One * 2f, null, fireGlowScaleFactor);
                fire.Spawn();
            }

            Time++;
        }

        public float LaserWidthFunction(float completionRatio)
        {
            float pulsation = MathF.Sin(MathHelper.Pi * completionRatio * 8f - MathHelper.TwoPi * Time / 10f) * 18f;
            float baseWidth = pulsation + Projectile.width;
            float startInterpolant = Utilities.InverseLerp(0.02f, 0.09f, completionRatio);
            float startingScaleFactor = MathHelper.SmoothStep(0f, 1f, MathF.Pow(startInterpolant, 0.45f));
            return baseWidth * startingScaleFactor;
        }

        public Color LaserColorFunction(float completionRatio)
        {
            return Projectile.GetAlpha(Color.Yellow);
        }

        public float BloomWidthFunction(float completionRatio) => LaserWidthFunction(completionRatio) * 1.8f;

        public Color BloomColorFunction(float completionRatio)
        {
            float opacity = Utilities.InverseLerp(0.01f, 0.065f, completionRatio) * Utilities.InverseLerp(0.9f, 0.7f, completionRatio) * 0.32f;
            return Projectile.GetAlpha(Color.OrangeRed) * opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> laserPositions = Projectile.GetLaserControlPoints(12, LaserbeamLength);

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.PrimitiveBloomShader");
            shader.TrySetParameter("innerGlowIntensity", 0.45f);

            PrimitiveSettings bloomSettings = new(BloomWidthFunction, BloomColorFunction, Shader: shader);
            PrimitiveRenderer.RenderTrail(laserPositions, bloomSettings, 60);

            return false;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            List<Vector2> laserPositions = Projectile.GetLaserControlPoints(12, LaserbeamLength);

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.ExothermalDisintegrationRayShader");
            shader.TrySetParameter("laserDirection", Projectile.velocity);
            shader.TrySetParameter("edgeColorSubtraction", new Vector3(0f, 0.9f, 0.9f));
            shader.TrySetParameter("edgeGlowIntensity", 0.15f);
            shader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 1, SamplerState.LinearWrap);
            shader.SetTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise"), 2, SamplerState.LinearWrap);

            PrimitiveSettings laserSettings = new(LaserWidthFunction, LaserColorFunction, Pixelate: true, Shader: shader);
            PrimitiveRenderer.RenderTrail(laserPositions, laserSettings, 60);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Measure how far along the laser's length the target is.
            // If the signed distance is negative (a.k.a. they're behind the laser) or above the laser length (a.k.a. they're beyond the laser), terminate this
            // method immediately.
            float signedDistanceAlongLaser = Utilities.SignedDistanceToLine(targetHitbox.Center(), Projectile.Center, Projectile.velocity);
            if (signedDistanceAlongLaser < 0f || signedDistanceAlongLaser >= LaserbeamLength)
                return false;

            // Now that the point on the laser is known from the distance, evaluate the exact width of the laser at said point for use with a AABB/line collision check.
            float laserWidth = LaserWidthFunction(signedDistanceAlongLaser / LaserbeamLength) * 0.45f;
            Vector2 perpendicular = new(-Projectile.velocity.Y, Projectile.velocity.X);
            Vector2 laserPoint = Projectile.Center + Projectile.velocity * signedDistanceAlongLaser;
            Vector2 left = laserPoint - perpendicular * laserWidth;
            Vector2 right = laserPoint + perpendicular * laserWidth;

            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), left, right, 16f, ref _);
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
