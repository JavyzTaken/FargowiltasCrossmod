using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.DataStructures;
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
    public class CannonLaserbeam : ModProjectile, IPixelatedPrimitiveRenderer, IProjOwnedByBoss<AresBody>, IExoMechProjectile
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterProjectiles;

        /// <summary>
        /// The <see cref="Owner"/> index in the NPC array.
        /// </summary>
        public int OwnerIndex => (int)Projectile.ai[0];

        /// <summary>
        /// The owner of this laserbeam.
        /// </summary>
        public NPC Owner => Main.npc[OwnerIndex];

        /// <summary>
        /// How long this laserbeam has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[1];

        /// <summary>
        /// How long this laserbeam currently is.
        /// </summary>
        public ref float LaserbeamLength => ref Projectile.ai[2];

        /// <summary>
        /// How long this laserbeam should exist for, in frames.
        /// </summary>
        public static int Lifetime => LumUtils.SecondsToFrames(2.2f);

        /// <summary>
        /// The maximum length of this laserbeam.
        /// </summary>
        public static float MaxLaserbeamLength => 4000f;

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Electricity;

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 6000;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = Lifetime;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (OwnerIndex < 0 || OwnerIndex >= Main.maxNPCs || !Owner.active || Owner.type != ModContent.NPCType<AresHand>() || Owner.As<AresHand>().HandType != AresHandType.LaserCannon)
            {
                Projectile.Kill();
                return;
            }

            Vector2 ownerCannonEnd = Owner.Center + new Vector2(Owner.spriteDirection * 74f, 16f).RotatedBy(Owner.rotation);
            Projectile.Center = ownerCannonEnd;
            Projectile.velocity = Owner.rotation.ToRotationVector2() * Owner.spriteDirection;

            LaserbeamLength = MathHelper.Clamp(LaserbeamLength + 167f, 0f, MaxLaserbeamLength);

            CreateSinusoidalParticles();
            CreatePerpendicularDust();

            Time++;
        }

        /// <summary>
        /// Creates dust along the deathray that moves perpendicular to it.
        /// </summary>
        public void CreatePerpendicularDust()
        {
            Vector2 perpendicular = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
            for (int i = 0; i < 3; i++)
            {
                float energyLengthInterpolant = Main.rand.NextFloat();
                float perpendicularDirection = Main.rand.NextFromList(-1f, 1f);
                Vector2 energySpawnPosition = Projectile.Center + Projectile.velocity * energyLengthInterpolant * LaserbeamLength + perpendicular * perpendicularDirection * LaserWidthFunction(0.25f) * 0.7f;
                Dust energy = Dust.NewDustPerfect(energySpawnPosition, 261, perpendicular * perpendicularDirection * Main.rand.NextFloat(1f, 3f));
                energy.noGravity = true;
                energy.color = Color.Wheat;
            }
        }

        /// <summary>
        /// Creates particles along the deathray that loosely follow a sinusoidal motion.
        /// </summary>
        public void CreateSinusoidalParticles()
        {
            for (int i = 0; i < 3; i++)
            {
                float energyLengthInterpolant = Main.rand.NextFloat();
                Vector2 energySpawnPosition = Projectile.Center + Projectile.velocity * energyLengthInterpolant * LaserbeamLength + Main.rand.NextVector2Circular(24f, 24f);
                Vector2 energyVelocity = Projectile.velocity.RotatedBy(MathF.Sin(energyLengthInterpolant * 60f + Time / 30f) * 0.7f) * 4f;

                BloomPixelParticle energy = new(energySpawnPosition, energyVelocity, Color.White, Color.Red * 0.6f, 20, Vector2.One * Main.rand.NextFloat(0.9f, 1.4f));
                energy.Spawn();
            }
        }

        public float LaserWidthFunction(float completionRatio)
        {
            float widthPulsation = MathF.Cos(completionRatio * 100f - Main.GlobalTimeWrappedHourly * 50f) * 1.85f;
            float initialBulge = LumUtils.Convert01To010(LumUtils.InverseLerp(0.15f, 0.85f, LaserbeamLength / MaxLaserbeamLength)) * LumUtils.InverseLerp(0f, 0.05f, completionRatio) * 32f;
            float idealWidth = widthPulsation + initialBulge + 14f - LumUtils.InverseLerp(0.05f, 0f, completionRatio) * 4f;
            float closureInterpolant = LumUtils.InverseLerp(0f, 8f, Lifetime - Time);
            return Utils.Remap(LaserbeamLength, 0f, MaxLaserbeamLength, 4f, idealWidth) * closureInterpolant;
        }

        public float BloomWidthFunction(float completionRatio) => LaserWidthFunction(completionRatio) * 2.5f;

        public Color LaserColorFunction(float completionRatio)
        {
            float lengthOpacity = LumUtils.InverseLerp(0f, 0.45f, LaserbeamLength / MaxLaserbeamLength);
            float startOpacity = LumUtils.InverseLerp(0f, 0.032f, completionRatio);
            float endOpacity = LumUtils.InverseLerp(0.95f, 0.81f, completionRatio);
            float opacity = lengthOpacity * startOpacity * endOpacity;
            Color startingColor = Projectile.GetAlpha(new(255, 56, 35));
            return startingColor * opacity;
        }

        public Color BloomColorFunction(float completionRatio) => LaserColorFunction(completionRatio) * LumUtils.InverseLerp(0.05f, 0.065f, completionRatio) * 0.54f;

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> laserPositions = Projectile.GetLaserControlPoints(12, LaserbeamLength);

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.PrimitiveBloomShader");
            shader.TrySetParameter("innerGlowIntensity", 0.45f);

            PrimitiveSettings bloomSettings = new(BloomWidthFunction, BloomColorFunction, _ => Projectile.Size * 0.5f, Shader: shader);
            PrimitiveRenderer.RenderTrail(laserPositions, bloomSettings, 60);

            return false;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            List<Vector2> laserPositions = Projectile.GetLaserControlPoints(12, LaserbeamLength);

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.CannonLaserbeamShader");
            shader.TrySetParameter("laserDirection", Projectile.velocity);
            shader.SetTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise"), 1, SamplerState.LinearWrap);

            PrimitiveSettings laserSettings = new(LaserWidthFunction, LaserColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader);
            PrimitiveRenderer.RenderTrail(laserPositions, laserSettings, 60);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float laserWidth = LaserWidthFunction(0.25f) * 0.85f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity.SafeNormalize(Vector2.Zero) * LaserbeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, laserWidth, ref _);
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
