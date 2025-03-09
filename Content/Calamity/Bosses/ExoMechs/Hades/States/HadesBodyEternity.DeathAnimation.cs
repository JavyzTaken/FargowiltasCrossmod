using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Assets.Particles.Metaballs;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades
{
    public sealed partial class HadesHeadEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// How long Hades spends redirecting towards the player at a slow pace during his death animation.
        /// </summary>
        public static int DeathAnimation_HomeInTime => Variables.GetAIInt("DeathAnimation_HomeInTime", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How long Hades spends becoming unstable and emitting smoke during his death animation.
        /// </summary>
        public static int DeathAnimation_BecomeUnstableTime => Variables.GetAIInt("DeathAnimation_BecomeUnstableTime", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How long Hades spends creating small explosions during his death animation.
        /// </summary>
        public static int DeathAnimation_SmallExplosionsTime => Variables.GetAIInt("DeathAnimation_SmallExplosionsTime", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How long Hades spends waiting for his big explosion during his death animation.
        /// </summary>
        public static int DeathAnimation_BigExplosionDelay => Variables.GetAIInt("DeathAnimation_BigExplosionDelay", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The sound played as Hades' death animation progresses.
        /// </summary>
        public static readonly SoundStyle DeathBuildupSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/DeathBuildup");

        /// <summary>
        /// AI update loop method for the death animation.
        /// </summary>
        public void DoBehavior_DeathAnimation()
        {
            NPC.damage = 0;
            NPC.dontTakeDamage = true;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            SegmentReorientationStrength = 0f;

            // Stop any residual laserbeam sounds, since it goes on for a while.
            if (SoundEngine.TryGetActiveSound(SuperDeathraySoundInstance, out ActiveSound? superDeathraySound))
                superDeathraySound?.Stop();

            float instabilityInterpolant = LumUtils.InverseLerp(0f, DeathAnimation_BecomeUnstableTime, AITimer - DeathAnimation_HomeInTime);
            float explosionInterpolant = LumUtils.InverseLerp(0f, DeathAnimation_SmallExplosionsTime, AITimer - DeathAnimation_HomeInTime - DeathAnimation_BecomeUnstableTime);
            float explosionDelayInterpolant = LumUtils.InverseLerp(0f, DeathAnimation_BigExplosionDelay, AITimer - DeathAnimation_HomeInTime - DeathAnimation_BecomeUnstableTime - DeathAnimation_SmallExplosionsTime);
            float maxScreenRumble = (1f - explosionDelayInterpolant).Squared() * 6f;

            if (AITimer == 1)
                SoundEngine.PlaySound(DeathBuildupSound).WithVolumeBoost(2.65f);

            // Home towards the player at first, keeping segments closed.
            if (AITimer <= DeathAnimation_HomeInTime)
            {
                float currentSpeed = NPC.velocity.Length();
                if (currentSpeed > 40f)
                    NPC.velocity *= 0.925f;
                if (currentSpeed > 25f)
                    NPC.velocity *= 0.925f;
                if (currentSpeed > 7.5f)
                    NPC.velocity *= 0.925f;

                float turnSpeed = LumUtils.InverseLerp(90f, 210f, NPC.Distance(Target.Center)) * 0.05f;
                turnSpeed *= LumUtils.InverseLerp(DeathAnimation_HomeInTime, DeathAnimation_HomeInTime * 0.67f, AITimer);

                NPC.Center = Vector2.Lerp(NPC.Center, Target.Center, turnSpeed * 0.24f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(Target.Center) * 7.4f, turnSpeed);
                BodyBehaviorAction = new(AllSegments(), CloseSegment());
            }

            // Scream as Hades goes unstable.
            JawRotation = MathHelper.Lerp(JawRotation, instabilityInterpolant * 1.5f, 0.06f);
            if (Main.rand.NextBool(instabilityInterpolant.Cubed() * 0.67f))
                JawRotation += Main.rand.NextFloatDirection() * 0.15f;

            SegmentOpenInterpolant = LumUtils.InverseLerp(0f, 0.4f, instabilityInterpolant);
            BodyBehaviorAction = new(AllSegments(), UnstableOpenSegment(instabilityInterpolant, explosionInterpolant.Squared() * 0.0144f));
            if (AITimer == DeathAnimation_HomeInTime + DeathAnimation_BecomeUnstableTime + DeathAnimation_SmallExplosionsTime + DeathAnimation_BigExplosionDelay)
            {
                BodyBehaviorAction = new(EveryNthSegment(10), behaviorOverride =>
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        LumUtils.NewProjectileBetter(behaviorOverride.NPC.GetSource_FromAI(), behaviorOverride.TurretPosition, Vector2.Zero, ModContent.ProjectileType<BigHadesSegmentExplosion>(), 0, 0f);
                });

                ScreenShakeSystem.StartShake(30f);
            }

            if (AITimer == DeathAnimation_HomeInTime + DeathAnimation_BecomeUnstableTime + DeathAnimation_SmallExplosionsTime + DeathAnimation_BigExplosionDelay + 2)
                Die();

            CustomExoMechsSky.RedSkyInterpolant = instabilityInterpolant;
            ScreenShakeSystem.SetUniversalRumble(MathF.Pow(instabilityInterpolant, 1.5f) * maxScreenRumble, MathHelper.TwoPi, null, 0.2f);
        }

        /// <summary>
        /// Destroys Hades and creates gores for his segments.
        /// </summary>
        public void Die()
        {
            if (NPC.DeathSound.HasValue)
                SoundEngine.PlaySound(NPC.DeathSound.Value with { Volume = 3f }).WithVolumeBoost(1.5f);

            NPC.life = 0;
            if (AresBody.CanDropLoot())
                NPC.checkDead();

            foreach (NPC segment in Main.ActiveNPCs)
            {
                if (segment.realLife != NPC.whoAmI && segment.realLife != NPC.whoAmI)
                    continue;

                segment.life = 0;
                CreateGores(segment);
            }

            NPC.active = false;
        }

        /// <summary>
        /// Creates gores for a given Hades body segment.
        /// </summary>
        /// <param name="segment">The segment to create gores for.</param>
        public static void CreateGores(NPC segment)
        {
            if (!segment.TryGetDLCBehavior(out HadesBodyEternity body))
                return;

            if (body.IsSecondaryBodySegment)
            {
                Gore.NewGore(segment.GetSource_Death(), segment.Center, segment.velocity + Main.rand.NextVector2Circular(25f, 25f), ModContent.Find<ModGore>("CalamityMod/ThanatosBody2").Type, segment.scale);
                Gore.NewGore(segment.GetSource_Death(), segment.Center, segment.velocity + Main.rand.NextVector2Circular(25f, 25f), ModContent.Find<ModGore>("CalamityMod/ThanatosBody2_2").Type, segment.scale);
                Gore.NewGore(segment.GetSource_Death(), segment.Center, segment.velocity + Main.rand.NextVector2Circular(25f, 25f), ModContent.Find<ModGore>("CalamityMod/ThanatosBody2_3").Type, segment.scale);
            }
            else if (body.IsTailSegment)
            {
                Gore.NewGore(segment.GetSource_Death(), segment.Center, segment.velocity + Main.rand.NextVector2Circular(25f, 25f), ModContent.Find<ModGore>("CalamityMod/ThanatosTail1").Type, segment.scale);
                Gore.NewGore(segment.GetSource_Death(), segment.Center, segment.velocity + Main.rand.NextVector2Circular(25f, 25f), ModContent.Find<ModGore>("CalamityMod/ThanatosTail2").Type, segment.scale);
                Gore.NewGore(segment.GetSource_Death(), segment.Center, segment.velocity + Main.rand.NextVector2Circular(25f, 25f), ModContent.Find<ModGore>("CalamityMod/ThanatosTail3").Type, segment.scale);
                Gore.NewGore(segment.GetSource_Death(), segment.Center, segment.velocity + Main.rand.NextVector2Circular(25f, 25f), ModContent.Find<ModGore>("CalamityMod/ThanatosTail4").Type, segment.scale);
            }

            else
            {
                Gore.NewGore(segment.GetSource_Death(), segment.Center, segment.velocity + Main.rand.NextVector2Circular(25f, 25f), ModContent.Find<ModGore>("CalamityMod/ThanatosBody1").Type, segment.scale);
                Gore.NewGore(segment.GetSource_Death(), segment.Center, segment.velocity + Main.rand.NextVector2Circular(25f, 25f), ModContent.Find<ModGore>("CalamityMod/ThanatosBody1_2").Type, segment.scale);
                Gore.NewGore(segment.GetSource_Death(), segment.Center, segment.velocity + Main.rand.NextVector2Circular(25f, 25f), ModContent.Find<ModGore>("CalamityMod/ThanatosBody1_3").Type, segment.scale);
            }
        }

        /// <summary>
        /// An action that opens a segment's vents and creates smoke, sparks, and other effects that indicate an impending explosion. Meant to be used in conjunction with <see cref="BodyBehaviorAction"/>.
        /// </summary>
        /// <param name="segmentOpenRate">The amount by which the segment open interpolant changes every frame.</param>
        /// <param name="smokeQuantityInterpolant">A multiplier for how much smoke should be released.</param>
        public static BodySegmentAction UnstableOpenSegment(float instabilityInterpolant, float explodeChance, float segmentOpenRate = StandardSegmentOpenRate, float smokeQuantityInterpolant = 1f)
        {
            return new(behaviorOverride =>
            {
                float oldInterpolant = behaviorOverride.SegmentOpenInterpolant;
                behaviorOverride.SegmentOpenInterpolant = Utilities.Saturate(behaviorOverride.SegmentOpenInterpolant + segmentOpenRate);

                bool segmentJustOpened = behaviorOverride.SegmentOpenInterpolant > 0f && oldInterpolant <= 0f;
                if (segmentJustOpened)
                    SoundEngine.PlaySound(ThanatosHead.VentSound with { MaxInstances = 8, Volume = 0.3f }, behaviorOverride.NPC.Center);

                float bigInterpolant = Utilities.InverseLerp(1f, 0.91f, behaviorOverride.SegmentOpenInterpolant);
                if (Main.rand.NextBool(60))
                    bigInterpolant = 1f;

                if (behaviorOverride.SegmentOpenInterpolant >= 0.91f)
                {
                    CreateSmoke(behaviorOverride, bigInterpolant, smokeQuantityInterpolant);

                    if (Main.rand.NextBool(smokeQuantityInterpolant))
                        ModContent.GetInstance<HeatDistortionMetaball>().CreateParticle(behaviorOverride.TurretPosition, Main.rand.NextVector2Circular(3f, 3f), 70f);
                }

                if (Main.rand.NextBool(instabilityInterpolant.Squared() * 0.2f) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 arcSpawnPosition = behaviorOverride.TurretPosition + Main.rand.NextVector2Circular(8f, 8f);
                    Vector2 arcDestination = arcSpawnPosition + Main.rand.NextVector2Unit() * Main.rand.NextFloat(56f, 350f);
                    Vector2 arcLength = (arcDestination - arcSpawnPosition).RotatedByRandom(0.12f) * Main.rand.NextFloat(0.9f, 1f);
                    LumUtils.NewProjectileBetter(behaviorOverride.NPC.GetSource_FromAI(), arcSpawnPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(6, 9));
                }

                if (Main.rand.NextBool(instabilityInterpolant * 0.1f) && behaviorOverride.PlatingOffset <= 32f)
                    behaviorOverride.PlatingOffsetVelocity = Main.rand.NextFloat(4f, 6f);

                if (Main.rand.NextBool(explodeChance))
                {
                    Vector2 burstDirection = Main.rand.NextVector2Unit();
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 sparkVelocity = burstDirection.RotatedByRandom(0.51f) * Main.rand.NextFloat(4f, 11f);
                        Dust spark = Dust.NewDustPerfect(behaviorOverride.TurretPosition, 264, sparkVelocity);
                        spark.color = Color.Lerp(Color.Yellow, Color.Crimson, Main.rand.NextFloat());
                        spark.scale = 1.35f;
                        spark.noLight = true;
                    }

                    behaviorOverride.PlatingOffsetVelocity += 14f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        behaviorOverride.NPC.velocity += behaviorOverride.NPC.rotation.ToRotationVector2() * Main.rand.NextFromList(-1f, 1f) * 20f;
                        behaviorOverride.NPC.netUpdate = true;

                        LumUtils.NewProjectileBetter(behaviorOverride.NPC.GetSource_FromAI(), behaviorOverride.TurretPosition, Vector2.Zero, ModContent.ProjectileType<SmallHadesSegmentExplosion>(), 0, 0f);
                    }
                }
            });
        }
    }
}
