using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using Luminance.Assets;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PulseBlast : ModProjectile, IProjOwnedByBoss<AresBody>, IPixelatedPrimitiveRenderer
    {
        private readonly float[] lengthRatios = new float[10];

        /// <summary>
        /// Whether this blast has hit a player.
        /// </summary>
        public bool HasHitPlayer
        {
            get;
            set;
        }

        /// <summary>
        /// The <see cref="Owner"/> index in the NPC array.
        /// </summary>
        public int OwnerIndex => (int)Projectile.ai[0];

        /// <summary>
        /// The owner of this blast.
        /// </summary>
        public NPC Owner => Main.npc[OwnerIndex];

        /// <summary>
        /// How long this blast has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[1];

        /// <summary>
        /// How long this blast currently is.
        /// </summary>
        public ref float BlastLength => ref Projectile.ai[2];

        /// <summary>
        /// How long this blast should exist for, in frames.
        /// </summary>
        public static int Lifetime => 18;

        /// <summary>
        /// The maximum length of this blast.
        /// </summary>
        public static float MaxBlastLength => 4000f;

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 8000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = Lifetime * Projectile.MaxUpdates;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (OwnerIndex < 0 || OwnerIndex >= Main.maxNPCs || !Owner.active || Owner.type != ModContent.NPCType<AresHand>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.velocity = Owner.rotation.ToRotationVector2() * Owner.spriteDirection;
            Projectile.Center = Owner.Center + new Vector2(Owner.spriteDirection * 90f, 10f).RotatedBy(Owner.rotation);

            BlastLength = MathHelper.Clamp(BlastLength + 250f, 0f, MaxBlastLength);
            Projectile.scale = LumUtils.InverseLerpBump(0f, 6f, Lifetime - 10f, Lifetime - 1f, Time);

            ScreenShakeSystem.StartShake(LumUtils.InverseLerp(0f, 10f, Time) * 3f, MathHelper.TwoPi, null, 0.91f);

            if (Projectile.IsFinalExtraUpdate())
                Time++;

            CalculateLengthRatios();

            if (Time >= 2f)
            {
                EmitStartParticles();
                EmitEndParticles();
            }
        }

        /// <summary>
        /// Emits bloom at the start of the blast.
        /// </summary>
        public void EmitStartParticles()
        {
            float bloomScale = LumUtils.Cos01(MathHelper.TwoPi * Time / 5f) + 1f;

            StrongBloom bloom = new(Projectile.Center, Vector2.Zero, new Color(119, 9, 255) * 0.55f, bloomScale, 9);
            GeneralParticleHandler.SpawnParticle(bloom);

            bloom = new(Projectile.Center, Vector2.Zero, Color.Wheat * 0.65f, bloomScale * 0.4f, 9);
            GeneralParticleHandler.SpawnParticle(bloom);
        }

        /// <summary>
        /// Emits bloom and perpendicular particles at the end of the blast.
        /// </summary>
        public void EmitEndParticles()
        {
            float bloomScale = MathHelper.Lerp(0.85f, 1.3f, LumUtils.Cos01(MathHelper.TwoPi * Time / 6f)) * LaserWidthFunction(0f) * 0.1f;
            Vector2 perpendicular = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            Vector2 left = Projectile.Center - perpendicular * Projectile.width * Projectile.scale * 0.5f;
            Vector2 right = Projectile.Center + perpendicular * Projectile.width * Projectile.scale * 0.5f;
            for (int i = 0; i < lengthRatios.Length; i++)
            {
                Vector2 start = Vector2.Lerp(left, right, i / (float)(lengthRatios.Length - 1f));
                Vector2 end = start + Projectile.velocity * BlastLength * lengthRatios[i];
                StrongBloom bloom = new(end, Vector2.Zero, new Color(119, 9, 255) * 0.3f, bloomScale, 9);
                GeneralParticleHandler.SpawnParticle(bloom);

                bloom = new(end, Vector2.Zero, Color.Wheat * 0.25f, bloomScale * 0.4f, 9);
                GeneralParticleHandler.SpawnParticle(bloom);
            }
        }

        public float LaserWidthFunction(float completionRatio) => Projectile.width * Projectile.scale * MathHelper.Lerp(0.7f, 1f, LumUtils.Cos01(Main.GlobalTimeWrappedHourly * 72f));

        public float BloomWidthFunction(float completionRatio) => LaserWidthFunction(completionRatio) * 2.1f;

        public Color LaserColorFunction(float completionRatio) => Projectile.GetAlpha(new Color(216, 63, 255));

        public Color BloomColorFunction(float completionRatio) => Projectile.GetAlpha(new Color(255, 153, 249)) * LumUtils.InverseLerpBump(0.02f, 0.05f, 0.81f, 0.95f, completionRatio) * 0.3f;

        /// <summary>
        /// Attempts to calculate the end position of a line, accounting for intersections with players.
        /// </summary>
        /// <param name="start">The starting point of the line.</param>
        /// <param name="end">The ending point of the line, assuming no intersection obstruction.</param>
        public Vector2 AttemptPlayerIntersection(Vector2 start, Vector2 end)
        {
            Vector2 direction = (end - start).SafeNormalize(Vector2.Zero);

            int hitTargetIndex = -1;
            float minDistance = 9999f;
            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.dead)
                {
                    DLCUtils.LineEllipseIntersectionCheck(start, direction, player.Center, player.Size, player.fullRotation, out Vector2 a, out Vector2 b);

                    float aDistanceFromStart = start.Distance(a);
                    float bDistanceFromStart = start.Distance(b);
                    Vector2 candidate = aDistanceFromStart < bDistanceFromStart ? a : b;
                    float candidateDistance = MathF.Min(aDistanceFromStart, bDistanceFromStart);

                    if (candidateDistance < minDistance)
                    {
                        end = candidate;
                        minDistance = candidateDistance;
                        hitTargetIndex = player.whoAmI;
                    }
                }
            }

            // Knock back the player that got hit.
            if (hitTargetIndex != -1)
            {
                Player hitTarget = Main.player[hitTargetIndex];

                if (hitTarget.velocity.Length() < PulseCannonCage.MaxBlastPushSpeed)
                {
                    hitTarget.mount?.Dismount(hitTarget);
                    hitTarget.velocity += Projectile.velocity * 2.9f;
                }

                // Destroy the player's hooks.
                foreach (Projectile hook in Main.ActiveProjectiles)
                {
                    if (hook.aiStyle == 7 && hook.owner == hitTargetIndex)
                        hook.Kill();
                }

                for (int i = 0; i < 4; i++)
                {
                    Dust pulseEnergy = Dust.NewDustPerfect(hitTarget.Center + Main.rand.NextVector2Circular(40f, 40f), 267);
                    pulseEnergy.color = Color.Violet;
                    pulseEnergy.velocity = Main.rand.NextVector2Circular(20f, 20f) + hitTarget.velocity;
                    pulseEnergy.noGravity = true;
                }
            }

            return end;
        }

        /// <summary>
        /// Calculates the length ratios of this blast as a consequence of player interactions relative to the overall laserbeam length.
        /// </summary>
        private void CalculateLengthRatios()
        {
            Vector2 perpendicular = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            Vector2 left = Projectile.Center - perpendicular * Projectile.width * Projectile.scale * 0.5f;
            Vector2 right = Projectile.Center + perpendicular * Projectile.width * Projectile.scale * 0.5f;
            for (int i = 0; i < lengthRatios.Length; i++)
            {
                Vector2 start = Vector2.Lerp(left, right, i / (float)(lengthRatios.Length - 1f));
                Vector2 end = AttemptPlayerIntersection(start, start + Projectile.velocity * BlastLength);
                lengthRatios[i] = start.Distance(end) / BlastLength;
            }

            if (!HasHitPlayer && lengthRatios.Min() < 0.99f)
            {
                HasHitPlayer = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 perpendicular = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            Vector2 left = Projectile.Center - perpendicular * Projectile.width * Projectile.scale * 0.5f;
            Vector2 right = Projectile.Center + perpendicular * Projectile.width * Projectile.scale * 0.5f;
            for (int i = 0; i < lengthRatios.Length; i++)
            {
                float _ = 0f;
                float laserWidth = Projectile.width * Projectile.scale / lengthRatios.Length;
                Vector2 start = Vector2.Lerp(left, right, i / (float)(lengthRatios.Length - 1f));
                Vector2 end = start + Projectile.velocity * BlastLength * lengthRatios[i];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, laserWidth, ref _))
                    return true;
            }

            return false;
        }

        public override bool ShouldUpdatePosition() => false;

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            // Draw bloom.
            Vector2 perpendicular = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * LumUtils.InverseLerp(10f, 0f, Time);
            List<Vector2> beamPositions = Projectile.GetLaserControlPoints(24, BlastLength * MathHelper.Lerp(lengthRatios.Average(), lengthRatios.Min(), 0.67f) * 1.125f);
            for (int i = 0; i < beamPositions.Count; i++)
                beamPositions[i] += perpendicular * MathF.Sin(MathHelper.TwoPi * i / beamPositions.Count * 4f) * 24f;

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.PrimitiveBloomShader");
            shader.TrySetParameter("innerGlowIntensity", 0.45f);
            PrimitiveSettings bloomSettings = new(BloomWidthFunction, BloomColorFunction, Shader: shader, Pixelate: true);
            PrimitiveRenderer.RenderTrail(beamPositions, bloomSettings, 50);

            // Draw the beam.
            ManagedShader blastShader = ShaderManager.GetShader("FargowiltasCrossmod.PulseBlastShader");
            blastShader.TrySetParameter("lengthRatios", lengthRatios.ToArray());
            blastShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 1, SamplerState.LinearWrap);

            PrimitiveSettings laserSettings = new(LaserWidthFunction, LaserColorFunction, Shader: blastShader, Pixelate: true);
            PrimitiveRenderer.RenderTrail(beamPositions, laserSettings, 70);
        }
    }
}
