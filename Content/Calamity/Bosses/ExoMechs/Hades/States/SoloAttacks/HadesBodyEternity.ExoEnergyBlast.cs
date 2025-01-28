using CalamityMod.NPCs.ExoMechs.Apollo;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using Luminance.Common.Easings;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
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
        /// The sound instance of Hades' super deathray.
        /// </summary>
        public static SlotId SuperDeathraySoundInstance
        {
            get;
            private set;
        }

        /// <summary>
        /// The amount of damage the Exo Energy Blast from Hades does.
        /// </summary>
        public static int ExoEnergyBlastDamage => Variables.GetAIInt("ExoEnergyBlastDamage", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The maximum time Hades spends opening his jaw the ExoEnergyBlast attack.
        /// </summary>
        public static int ExoEnergyBlast_JawOpenTime => Variables.GetAIInt("ExoEnergyBlast_JawOpenTime", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The maximum time Hades spends redirecting during the ExoEnergyBlast attack.
        /// </summary>
        public static int ExoEnergyBlast_InitialRedirectTime => Variables.GetAIInt("ExoEnergyBlast_InitialRedirectTime", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The maximum time Hades spends accelerating after the ExoEnergyBlast attack's laserbeam.
        /// </summary>
        public static int ExoEnergyBlast_PostBeamAccelerationTime => Variables.GetAIInt("ExoEnergyBlast_PostBeamAccelerationTime", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The delay of the blast during the ExoEnergyBlast attack.
        /// </summary>
        public static int ExoEnergyBlast_BlastDelay => Variables.GetAIInt("ExoEnergyBlast_BlastDelay", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The cycle time which dictates the rate at which Hades releases missiles from his body segments during the ExoEnergyBlast attack.
        /// </summary>
        public static int ExoEnergyBlast_ProjectileBurstReleaseRate => Variables.GetAIInt("ExoEnergyBlast_ProjectileBurstReleaseRate", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The speed at which Hades turns the laser during the ExoEnergyBlast attack.
        /// </summary>
        public static float ExoEnergyBlast_LaserTurnSpeed => MathHelper.ToRadians(Variables.GetAIFloat("ExoEnergyBlast_LaserTurnSpeedDegrees", ExoMechAIVariableType.Hades));

        /// <summary>
        /// The sound Hades plays when charging up energy for his weaker deathray.
        /// </summary>
        public static readonly SoundStyle DeathrayChargeUpSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/DeathrayChargeUp");

        /// <summary>
        /// The sound Hades plays when charging up energy for his stronger deathray.
        /// </summary>
        public static readonly SoundStyle DeathrayChargeUpStrongSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/SuperLaserBuildupSound");

        /// <summary>
        /// The sound Hades plays when firing his deathray.
        /// </summary>
        public static readonly SoundStyle DeathrayFireSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/DeathrayFire");

        /// <summary>
        /// The sound Hades plays when firing his strong deathray.
        /// </summary>
        public static readonly SoundStyle DeathrayFireStrongSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/SuperLaserFire");

        /// <summary>
        /// AI update loop method for the ExoEnergyBlast attack.
        /// </summary>
        public void DoBehavior_ExoEnergyBlast()
        {
            SegmentReorientationStrength = 0f;

            bool beamIsOverheating = AITimer >= ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay + HadesSuperLaserbeam.Lifetime - HadesSuperLaserbeam.OverheatStartingTime;
            float pointAtTargetSpeed = 4f;
            Vector2 outerHoverDestination = Target.Center + new Vector2(NPC.OnRightSideOf(Target).ToDirectionInt() * 1050f, -400f);

            BodyBehaviorAction = new(EveryNthSegment(2), b => DoBehavior_ExoEnergyBlast_UpdateSegment(b, beamIsOverheating));
            SegmentOpenInterpolant = LumUtils.Saturate(SegmentOpenInterpolant + (beamIsOverheating ? 2f : -1f) * StandardSegmentOpenRate);

            float jawOpenInterpolant = LumUtils.InverseLerp(0f, ExoEnergyBlast_JawOpenTime, AITimer - ExoEnergyBlast_InitialRedirectTime);
            float jawCloseInterpolant = LumUtils.InverseLerpBump(-45f, -20f, 0f, 4f, AITimer - ExoEnergyBlast_InitialRedirectTime - ExoEnergyBlast_BlastDelay);
            float jawJitter = LumUtils.InverseLerp(0f, 8f, AITimer - ExoEnergyBlast_InitialRedirectTime - ExoEnergyBlast_BlastDelay) * 0.12f;

            JawRotation = EasingCurves.Elastic.Evaluate(EasingType.Out, 0f, 1.3f, jawOpenInterpolant);
            JawRotation = JawRotation.AngleLerp(-0.2f, jawCloseInterpolant);
            JawRotation += Main.rand.NextFloatDirection() * jawJitter;

            // Attempt to get into position for the light attack.
            if (AITimer < ExoEnergyBlast_InitialRedirectTime)
            {
                float idealHoverSpeed = MathHelper.Lerp(43.5f, 72.5f, AITimer / (float)ExoEnergyBlast_InitialRedirectTime);
                idealHoverSpeed *= LumUtils.InverseLerp(35f, 300f, NPC.Distance(outerHoverDestination));

                if (MathHelper.Distance(NPC.Center.X, Target.Center.X) >= 200f)
                    outerHoverDestination.X = NPC.Center.X;

                // Ensure that Hades is near the target at first.
                float redirectInterpolant = MathF.Sqrt(LumUtils.InverseLerp(30f, 0f, AITimer)) * LumUtils.InverseLerp(300f, 450f, NPC.Distance(Target.Center));
                NPC.Center = Vector2.Lerp(NPC.Center, Target.Center, redirectInterpolant * 0.05f);

                Vector2 idealVelocity = NPC.SafeDirectionTo(outerHoverDestination) * MathHelper.Lerp(NPC.velocity.Length(), idealHoverSpeed, 0.135f);
                NPC.velocity = NPC.velocity.RotateTowards(idealVelocity.ToRotation(), 0.045f);
                NPC.velocity = NPC.velocity.MoveTowards(idealVelocity, AITimer / (float)ExoEnergyBlast_InitialRedirectTime * 8f);

                // Stop hovering if close to the hover destination and prepare to move towards the target.
                if (NPC.WithinRange(outerHoverDestination, 185f) && AITimer >= 30)
                {
                    AITimer = ExoEnergyBlast_InitialRedirectTime;
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(Target.Center) * NPC.velocity.Length(), 0.92f);
                    NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY) * MathHelper.Lerp(NPC.velocity.Length(), pointAtTargetSpeed, 0.4f);
                    NPC.netUpdate = true;
                }
            }

            // Slow down, move towards the target (while maintaining the current direction) and create the telegraph.
            if (AITimer >= ExoEnergyBlast_InitialRedirectTime && AITimer < ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay)
            {
                if (AITimer == ExoEnergyBlast_InitialRedirectTime + 1)
                {
                    SoundEngine.PlaySound(DeathrayChargeUpStrongSound).WithVolumeBoost(1.5f);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HadesExoEnergyOrb>(), 0, 0f, -1, ExoEnergyBlast_BlastDelay);
                }

                float chargeUpInterpolant = LumUtils.InverseLerp(0f, ExoEnergyBlast_BlastDelay, AITimer - ExoEnergyBlast_InitialRedirectTime);
                for (int i = 0; i < 2; i++)
                {
                    if (Main.rand.NextBool(MathF.Sqrt(chargeUpInterpolant)) && chargeUpInterpolant <= 0.72f)
                    {
                        Vector2 mouthPosition = NPC.Center + NPC.velocity.SafeNormalize(Vector2.Zero) * 150f;
                        Vector2 streakPosition = mouthPosition + Main.rand.NextVector2Unit() * Main.rand.NextFloat(100f, 500f);
                        Vector2 streakVelocity = (mouthPosition - streakPosition) * 0.085f;
                        Vector2 startingScale = new(0.3f, 0.01f);
                        Vector2 endingScale = new(1.7f, 0.035f);
                        LineStreakParticle energy = new(streakPosition, streakVelocity, Color.White, 13, streakVelocity.ToRotation(), startingScale, endingScale);
                        energy.Spawn();
                    }

                    if (Main.rand.NextBool(chargeUpInterpolant * 0.8f) && chargeUpInterpolant <= 0.5f)
                    {
                        Vector2 mouthPosition = NPC.Center + NPC.velocity.SafeNormalize(Vector2.Zero) * 90f;
                        Vector2 pixelPosition = mouthPosition + Main.rand.NextVector2Unit() * Main.rand.NextFloat(90f, 400f);
                        Vector2 pixelVelocity = (mouthPosition - pixelPosition).RotatedBy(MathHelper.PiOver2) * 0.06f;
                        BloomPixelParticle pixel = new(pixelPosition, pixelVelocity, Color.White, Color.Cyan * 0.53f, 90, Vector2.One * Main.rand.NextFloat(1f, 2f), NPC.Center + NPC.velocity * 85f);
                        pixel.Spawn();
                    }
                }

                ScreenShakeSystem.StartShake(chargeUpInterpolant.Cubed() * 2f);

                // Approach the ideal position.
                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY) * MathHelper.Lerp(NPC.velocity.Length(), pointAtTargetSpeed, 0.061f);
            }

            // Fire the Biden Blast.
            if (AITimer == ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay)
            {
                SuperDeathraySoundInstance = SoundEngine.PlaySound(DeathrayFireStrongSound).WithVolumeBoost(1.4f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HadesSuperLaserbeam>(), ExoEnergyBlastDamage, 0f);
            }

            // SPEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEN
            if (AITimer >= ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay && AITimer < ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay + HadesSuperLaserbeam.Lifetime)
            {
                NPC.velocity = NPC.velocity.RotateTowards(NPC.AngleTo(Target.Center), ExoEnergyBlast_LaserTurnSpeed);
                if (NPC.velocity.Length() >= 3f)
                    NPC.velocity *= 0.99f;
            }

            // Accelerate after the blast.
            if (AITimer >= ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay + HadesSuperLaserbeam.Lifetime)
            {
                NPC.velocity = NPC.velocity.ClampLength(3.6f, 32f);
                NPC.velocity *= 1.024f;
            }

            if (AITimer >= ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay + HadesSuperLaserbeam.Lifetime + ExoEnergyBlast_PostBeamAccelerationTime)
                SelectNewState();

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public void DoBehavior_ExoEnergyBlast_UpdateSegment(HadesBodyEternity behaviorOverride, bool beamIsOverheating)
        {
            NPC segment = behaviorOverride.NPC;

            if (beamIsOverheating)
                OpenSegment().Invoke(behaviorOverride);
            else
            {
                CloseSegment().Invoke(behaviorOverride);
                segment.defense = behaviorOverride.NPC.defDefense + 100;
            }

            segment.damage = segment.defDamage;

            if ((segment.whoAmI * 12 + AITimer) % ExoEnergyBlast_ProjectileBurstReleaseRate == 0 && beamIsOverheating)
            {
                bool createMine = Main.rand.NextBool(3);
                Vector2 projectileSpawnPosition = behaviorOverride.TurretPosition;

                if (createMine)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 mineVelocity = projectileSpawnPosition.SafeDirectionTo(Target.Center) * Main.rand.NextFloat(42f);
                        LumUtils.NewProjectileBetter(segment.GetSource_FromAI(), projectileSpawnPosition, mineVelocity, ModContent.ProjectileType<HadesMine>(), MineDamage, 0f, -1, LumUtils.SecondsToFrames(4f));
                    }
                }
                else if (Main.netMode != NetmodeID.MultiplayerClient && !projectileSpawnPosition.WithinRange(Target.Center, 400f))
                {
                    float missileSpeed = Main.rand.NextFloat(0.7f, 0.8f);
                    float missileOffsetAngle = Main.rand.NextGaussian(0.11f);
                    Vector2 missileVelocity = projectileSpawnPosition.SafeDirectionTo(Target.Center).RotatedBy(missileOffsetAngle) * missileSpeed;
                    LumUtils.NewProjectileBetter(segment.GetSource_FromAI(), projectileSpawnPosition, missileVelocity, ModContent.ProjectileType<HadesMissile>(), MissileDamage, 0f);
                }

                SoundEngine.PlaySound(Apollo.MissileLaunchSound with { Volume = 0.16f, MaxInstances = 0 }, projectileSpawnPosition);
                segment.netUpdate = true;
            }

            OpenSegment().Invoke(behaviorOverride);
        }
    }
}
