﻿using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
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
        /// The amount of damage the Exo Energy Blast from Hades does.
        /// </summary>
        public static int ExoEnergyBlastDamage => Main.expertMode ? 515 : 400;

        /// <summary>
        /// The maximum time Hades spends opening his jaw the ExoEnergyBlast attack.
        /// </summary>
        public static int ExoEnergyBlast_JawOpenTime => Utilities.SecondsToFrames(1.5f);

        /// <summary>
        /// The maximum time Hades spends redirecting during the ExoEnergyBlast attack.
        /// </summary>
        public static int ExoEnergyBlast_InitialRedirectTime => Utilities.SecondsToFrames(6f);

        /// <summary>
        /// The maximum time Hades spends accelerating after the ExoEnergyBlast attack's laserbeam.
        /// </summary>
        public static int ExoEnergyBlast_PostBeamAccelerationTime => Utilities.SecondsToFrames(1.2f);

        /// <summary>
        /// The delay of the blast during the ExoEnergyBlast attack.
        /// </summary>
        public static int ExoEnergyBlast_BlastDelay => Utilities.SecondsToFrames(2.45f);

        /// <summary>
        /// The rate at which bursts of electricity are shot from the orb during the ExoEnergyBlast attack.
        /// </summary>
        public static int ExoEnergyBlast_ProjectileBurstReleaseRate => Utilities.SecondsToFrames(0.75f);

        /// <summary>
        /// The speed at which Hades turns the laser during the ExoEnergyBlast attack.
        /// </summary>
        public static float ExoEnergyBlast_LaserTurnSpeed => MathHelper.ToRadians(0.7f);

        /// <summary>
        /// The sound Hades plays when charging up energy for his deathray.
        /// </summary>
        public static readonly SoundStyle DeathrayChargeUpSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/DeathrayChargeUp");

        /// <summary>
        /// The sound Hades plays when firing his deathray.
        /// </summary>
        public static readonly SoundStyle DeathrayFireSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/DeathrayFire");

        /// <summary>
        /// AI update loop method for the ExoEnergyBlast attack.
        /// </summary>
        public void DoBehavior_ExoEnergyBlast()
        {
            bool beamIsOverheating = AITimer >= ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay + ExoEnergyBlast.Lifetime - ExoEnergyBlast.OverheatStartingTime;
            float pointAtTargetSpeed = 4f;
            Vector2 outerHoverDestination = Target.Center + new Vector2(NPC.OnRightSideOf(Target).ToDirectionInt() * 1050f, -400f);

            BodyBehaviorAction = new(AllSegments(), beamIsOverheating ? OpenSegment() : CloseSegment());
            SegmentOpenInterpolant = Utilities.Saturate(SegmentOpenInterpolant + (beamIsOverheating ? 2f : -1f) * StandardSegmentOpenRate);

            JawRotation = MathHelper.Lerp(JawRotation, Utilities.InverseLerp(0f, ExoEnergyBlast_JawOpenTime, AITimer).Squared() * 1.3f, 0.07f);

            // Attempt to get into position for the light attack.
            if (AITimer < ExoEnergyBlast_InitialRedirectTime)
            {
                float idealHoverSpeed = MathHelper.Lerp(43.5f, 72.5f, AITimer / (float)ExoEnergyBlast_InitialRedirectTime);
                idealHoverSpeed *= LumUtils.InverseLerp(35f, 300f, NPC.Distance(outerHoverDestination));

                Vector2 idealVelocity = NPC.SafeDirectionTo(outerHoverDestination) * MathHelper.Lerp(NPC.velocity.Length(), idealHoverSpeed, 0.135f);
                NPC.velocity = NPC.velocity.RotateTowards(idealVelocity.ToRotation(), 0.045f);
                NPC.velocity = NPC.velocity.MoveTowards(idealVelocity, AITimer / (float)ExoEnergyBlast_InitialRedirectTime * 8f);

                // Stop hovering if close to the hover destination and prepare to move towards the target.
                if (NPC.WithinRange(outerHoverDestination, 105f) && AITimer >= 30)
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
                    SoundEngine.PlaySound(DeathrayChargeUpSound);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Utilities.NewProjectileBetter(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HadesExoEnergyOrb>(), 0, 0f, -1, ExoEnergyBlast_BlastDelay);
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

                // Approach the ideal position.
                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY) * MathHelper.Lerp(NPC.velocity.Length(), pointAtTargetSpeed, 0.061f);
            }

            // Fire the Biden Blast.
            if (AITimer == ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay)
            {
                SoundEngine.PlaySound(DeathrayFireSound);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Utilities.NewProjectileBetter(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ExoEnergyBlast>(), ExoEnergyBlastDamage, 0f, NPC.target);
            }

            // SPEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEN
            if (AITimer >= ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay && AITimer < ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay + ExoEnergyBlast.Lifetime)
            {
                NPC.velocity = NPC.velocity.RotateTowards(NPC.AngleTo(Target.Center), ExoEnergyBlast_LaserTurnSpeed);
                if (NPC.velocity.Length() >= 3f)
                    NPC.velocity *= 0.99f;
            }

            // Accelerate after the blast.
            if (AITimer >= ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay + ExoEnergyBlast.Lifetime)
            {
                NPC.velocity = NPC.velocity.ClampLength(3.6f, 32f);
                NPC.velocity *= 1.024f;
            }

            if (AITimer >= ExoEnergyBlast_InitialRedirectTime + ExoEnergyBlast_BlastDelay + ExoEnergyBlast.Lifetime + ExoEnergyBlast_PostBeamAccelerationTime)
                SelectNewState();

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
