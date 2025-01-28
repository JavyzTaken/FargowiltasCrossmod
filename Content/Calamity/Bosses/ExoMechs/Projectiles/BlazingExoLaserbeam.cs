using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
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
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BlazingExoLaserbeam : ModProjectile, IPixelatedPrimitiveRenderer, IProjOwnedByBoss<AresBody>, IExoMechProjectile
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterProjectiles;

        /// <summary>
        /// The local, looped instance of the <see cref="LaserLoopSound"/> for this laserbam.
        /// </summary>
        public LoopedSoundInstance LaserLoopSoundInstance
        {
            get;
            private set;
        }

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
        public static int Lifetime => Utilities.SecondsToFrames(10f);

        /// <summary>
        /// The maximum length of this laserbeam.
        /// </summary>
        public static float MaxLaserbeamLength => 4000f;

        /// <summary>
        /// The sound that this laserbeam loops.
        /// </summary>
        public static readonly SoundStyle LaserLoopSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/ExoTwins/ApolloLaserLoop");

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Thermal;

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
            Projectile.Calamity().DealsDefenseDamage = true;

            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (OwnerIndex < 0 || OwnerIndex >= Main.maxNPCs || !Owner.active)
            {
                Projectile.Kill();
                return;
            }
            if (Owner.type != ExoMechNPCIDs.ArtemisID && Owner.type != ExoMechNPCIDs.ApolloID)
            {
                Projectile.Kill();
                return;
            }

            Vector2 ownerEnd = Owner.Center + Owner.rotation.ToRotationVector2() * 76f;
            Projectile.Center = ownerEnd;
            Projectile.velocity = Owner.rotation.ToRotationVector2();

            LaserbeamLength = MathHelper.Clamp(LaserbeamLength + 167f, 0f, MaxLaserbeamLength);

            CreateSinusoidalParticles();
            UpdateSoundLoop();

            Time++;
        }

        /// <summary>
        /// Creates particles along the deathray that loosely follow a sinusoidal motion.
        /// </summary>
        public void CreateSinusoidalParticles()
        {
            for (int i = 0; i < 12; i++)
            {
                float energyLengthInterpolant = Main.rand.NextFloat();
                Vector2 energySpawnPosition = Projectile.Center + Projectile.velocity * energyLengthInterpolant * LaserbeamLength + Main.rand.NextVector2Circular(24f, 24f);
                Vector2 energyVelocity = Projectile.velocity.RotatedBy(MathF.Sin(energyLengthInterpolant * 60f + Time / 30f) * 0.7f) * 4f;

                BloomPixelParticle energy = new(energySpawnPosition, energyVelocity, Color.White, BloomColorFunction(0.5f) * 0.7f, 13, Vector2.One * Main.rand.NextFloat(1.5f, 2.5f));
                energy.Spawn();
            }
        }

        /// <summary>
        /// Updates the looping laser sound of this laser.
        /// </summary>
        public void UpdateSoundLoop()
        {
            Vector2 soundPosition = Utils.ClosestPointOnLine(Main.LocalPlayer.Center, Projectile.Center, Projectile.Center + Projectile.velocity * LaserbeamLength);
            LaserLoopSoundInstance ??= LoopedSoundManager.CreateNew(LaserLoopSound, () => !Projectile.active);
            LaserLoopSoundInstance?.Update(soundPosition);
        }

        public float LaserWidthFunction(float completionRatio)
        {
            float widthPulsation = MathF.Cos(completionRatio * 100f - Main.GlobalTimeWrappedHourly * 50f) * 3f;
            float initialBulge = Utilities.Convert01To010(Utilities.InverseLerp(0.15f, 0.85f, LaserbeamLength / MaxLaserbeamLength)) * Utilities.InverseLerp(0f, 0.05f, completionRatio) * 32f;
            float idealWidth = widthPulsation + initialBulge + 14f - Utilities.InverseLerp(0.05f, 0f, completionRatio) * 4f;
            float closureInterpolant = Utilities.InverseLerp(0f, 15f, Projectile.timeLeft);
            return Utils.Remap(LaserbeamLength, 0f, MaxLaserbeamLength, 4f, idealWidth) * closureInterpolant;
        }

        public float BloomWidthFunction(float completionRatio) => LaserWidthFunction(completionRatio) * 2.5f;

        public Color LaserColorFunction(float completionRatio)
        {
            float lengthOpacity = Utilities.InverseLerp(0f, 0.45f, LaserbeamLength / MaxLaserbeamLength);
            float endOpacity = Utilities.InverseLerp(0.95f, 0.81f, completionRatio);
            float opacity = lengthOpacity * endOpacity;
            Color startingColor = Projectile.GetAlpha(Owner.type == ExoMechNPCIDs.ApolloID ? new(0, 240, 0) : new(255, 83, 0));
            return startingColor * opacity;
        }

        public Color BloomColorFunction(float completionRatio) => LaserColorFunction(completionRatio) * Utilities.InverseLerp(0.01f, 0.065f, completionRatio) * 0.54f;

        public override bool PreDraw(ref Color lightColor)
        {
            float bloomScaleFactor = MathF.Cos(Main.GlobalTimeWrappedHourly * 48f) * 0.1f + 1f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Texture2D bloom = MiscTexturesRegistry.BloomCircleSmall.Value;
            Texture2D flare = MiscTexturesRegistry.ShineFlareTexture.Value;
            Main.spriteBatch.Draw(bloom, drawPosition, null, new(255, 255, 255, 0), 0f, bloom.Size() * 0.5f, bloomScaleFactor * 0.6f, 0, 0f);
            Main.spriteBatch.Draw(bloom, drawPosition, null, LaserColorFunction(0.5f) with { A = 0 }, 0f, bloom.Size() * 0.5f, bloomScaleFactor * 1.5f, 0, 0f);
            Main.spriteBatch.Draw(flare, drawPosition, null, Color.Wheat with { A = 0 }, 0f, flare.Size() * 0.5f, bloomScaleFactor * 1.1f, 0, 0f);

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

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.BlazingExoLaserbeamShader");
            shader.TrySetParameter("laserDirection", Projectile.velocity);
            shader.SetTexture(MiscTexturesRegistry.TurbulentNoise.Value, 1, SamplerState.LinearWrap);

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

        public override bool? CanDamage() => Time <= Lifetime - 15f;

        public override bool ShouldUpdatePosition() => false;
    }
}
