using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    public sealed partial class AresBodyEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// How long Ares spends charging up before firing his nuke during the NukeAoEAndPlasmaBlasts attack.
        /// </summary>
        public static int NukeAoEAndPlasmaBlasts_NukeChargeUpTime => Variables.GetAIInt("NukeAoEAndPlasmaBlasts_NukeChargeUpTime", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long Ares' nuke waits before detonating during the NukeAoEAndPlasmaBlasts attack.
        /// </summary>
        public static int NukeAoEAndPlasmaBlasts_NukeExplosionDelay => Variables.GetAIInt("NukeAoEAndPlasmaBlasts_NukeExplosionDelay", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long Ares waits before transitioning to the next state following the explosion during the NukeAoEAndPlasmaBlasts attack.
        /// </summary>
        public static int NukeAoEAndPlasmaBlasts_AttackTransitionDelay => Variables.GetAIInt("NukeAoEAndPlasmaBlasts_AttackTransitionDelay", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How big the nuke explosion should be during the NukeAoEAndPlasmaBlasts attack.
        /// </summary>
        public static float NukeAoEAndPlasmaBlasts_NukeExplosionDiameter => Variables.GetAIFloat("NukeAoEAndPlasmaBlasts_NukeExplosionDiameter", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How much Ares' nuke (not its resulting explosion!) does.
        /// </summary>
        public static int NukeWeaponDamage => Variables.GetAIInt("NukeWeaponDamage", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How much Ares' nuke explosion does.
        /// </summary>
        public static int NukeExplosionDamage => Variables.GetAIInt("NukeExplosionDamage", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How much damage Ares' lingering plasma blasts do.
        /// </summary>
        public static int LingeringPlasmaDamage => Variables.GetAIInt("LingeringPlasmaDamage", ExoMechAIVariableType.Ares);

        /// <summary>
        /// AI update loop method for the NukeAoEAndPlasmaBlasts attack.
        /// </summary>
        public void DoBehavior_NukeAoEAndPlasmaBlasts()
        {
            if (AITimer == 1)
            {
                SoundEngine.PlaySound(AresPlasmaFlamethrower.TelSound);
                SoundEngine.PlaySound(AresGaussNuke.TelSound);
            }

            float spinAngle = MathHelper.TwoPi * (AITimer - NukeAoEAndPlasmaBlasts_NukeChargeUpTime) / -150f;
            Vector2 flyDestination = Target.Center - Vector2.UnitY.RotatedBy(spinAngle) * 460f + Target.velocity * new Vector2(45f, 8f);
            NPC.Center = Vector2.Lerp(NPC.Center, flyDestination, 0.02f);
            NPC.SimpleFlyMovement(NPC.SafeDirectionTo(flyDestination) * 26f, 0.3f);

            InstructionsForHands[0] = new(h => NukeAoEAndPlasmaBlastsHandUpdate(h, new Vector2(-400f, 40f), 0));
            InstructionsForHands[1] = new(h => NukeAoEAndPlasmaBlastsHandUpdate(h, new Vector2(-280f, 224f), 1));
            InstructionsForHands[2] = new(h => NukeAoEAndPlasmaBlastsHandUpdate(h, new Vector2(280f, 224f), 2));
            InstructionsForHands[3] = new(h => NukeAoEAndPlasmaBlastsHandUpdate(h, new Vector2(400f, 40f), 3));

            if (AITimer >= NukeAoEAndPlasmaBlasts_NukeChargeUpTime + NukeAoEAndPlasmaBlasts_NukeExplosionDelay + NukeAoEAndPlasmaBlasts_AttackTransitionDelay)
                SelectNewState();
        }

        public void DoBehavior_NukeAoEAndPlasmaBlasts_ReleaseBurst(Projectile teslaSphere)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            float burstOffsetAngle = MathF.Cos(MathHelper.TwoPi * AITimer / 120f) * MathHelper.PiOver2;
            Vector2 burstShootDirection = teslaSphere.SafeDirectionTo(Target.Center).RotatedBy(burstOffsetAngle);
            Vector2 burstSpawnPosition = teslaSphere.Center + burstShootDirection * teslaSphere.width * Main.rand.NextFloat(0.1f);
            Utilities.NewProjectileBetter(NPC.GetSource_FromAI(), burstSpawnPosition, burstShootDirection * 42f, ModContent.ProjectileType<HomingTeslaBurst>(), TeslaBurstDamage, 0f);
        }

        public void NukeAoEAndPlasmaBlastsHandUpdate(AresHand hand, Vector2 hoverOffset, int armIndex)
        {
            NPC handNPC = hand.NPC;
            handNPC.SmoothFlyNear(NPC.Center + hoverOffset * NPC.scale, 0.3f, 0.8f);
            handNPC.Opacity = Utilities.Saturate(handNPC.Opacity + 0.2f);
            hand.UsesBackArm = armIndex == 0 || armIndex == ArmCount - 1;
            hand.ArmSide = (armIndex >= ArmCount / 2).ToDirectionInt();
            hand.HandType = armIndex == ArmCount - 1 ? AresHandType.GaussNuke : AresHandType.PlasmaCannon;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.EnergyDrawer.chargeProgress = Utilities.InverseLerp(0f, NukeAoEAndPlasmaBlasts_NukeChargeUpTime, AITimer);
            if (hand.EnergyDrawer.chargeProgress >= 1f)
                hand.EnergyDrawer.chargeProgress = 0f;
            hand.GlowmaskDisabilityInterpolant = 0f;

            if (AITimer % 20 == 19 && hand.EnergyDrawer.chargeProgress >= 0.4f)
            {
                int pulseCounter = (int)MathF.Round(hand.EnergyDrawer.chargeProgress * 5f);
                hand.EnergyDrawer.AddPulse(pulseCounter);
            }

            if (hand.HandType == AresHandType.GaussNuke)
                HandleGaussNukeShots(hand, handNPC, AITimer, NukeAoEAndPlasmaBlasts_NukeChargeUpTime, NukeAoEAndPlasmaBlasts_NukeExplosionDiameter);
            else
                NukeAoEAndPlasmaBlastsHandUpdate_PlasmaCannon(hand, handNPC);
        }

        public void NukeAoEAndPlasmaBlastsHandUpdate_PlasmaCannon(AresHand hand, NPC handNPC)
        {
            float idealRotation = handNPC.AngleTo(Target.Center) + MathHelper.Pi + MathF.Sin(MathHelper.TwoPi * AITimer / 90f) * MathHelper.PiOver2;
            hand.RotateToLookAt(idealRotation);
            hand.Frame = AITimer / 3 % 12;

            int shootRate = 2;
            int shootPeriod = NPC.whoAmI * 23 % shootRate;

            if (AITimer % 8 == 7 && AITimer < NukeAoEAndPlasmaBlasts_NukeChargeUpTime)
                SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaShootSound, handNPC.Center);

            // Release fireballs.
            if (AITimer % shootRate == shootPeriod && AITimer < NukeAoEAndPlasmaBlasts_NukeChargeUpTime)
            {
                Vector2 handDirection = handNPC.rotation.ToRotationVector2() * handNPC.spriteDirection;
                Vector2 plasmaSpawnPosition = handNPC.Center + new Vector2(handNPC.spriteDirection * 50f, 10f).RotatedBy(handNPC.rotation);

                if (handDirection.AngleBetween(handNPC.SafeDirectionTo(Target.Center)) <= 0.2f)
                    return;

                for (int i = 0; i < 14; i++)
                {
                    Color gasColor = Color.Lerp(Color.Lime, Color.Yellow, Main.rand.NextFloat());
                    gasColor.A /= 6;

                    SmallSmokeParticle gas = new(plasmaSpawnPosition, handDirection.RotatedByRandom(0.4f) * Main.rand.NextFloat(3f, 70f), gasColor, Color.YellowGreen with { A = 0 }, 2f, 120f);
                    GeneralParticleHandler.SpawnParticle(gas);

                    Dust plasma = Dust.NewDustPerfect(plasmaSpawnPosition, 107, handDirection.RotatedByRandom(0.5f) * Main.rand.NextFloat(2f, 40f));
                    plasma.scale = 1.2f;
                    plasma.noGravity = true;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 plasmaVelocity = handDirection * Main.rand.NextFloat(15f, 180f);
                    Utilities.NewProjectileBetter(handNPC.GetSource_FromAI(), plasmaSpawnPosition, plasmaVelocity, ModContent.ProjectileType<LingeringPlasmaFireball>(), LingeringPlasmaDamage, 0f);

                    handNPC.velocity -= handDirection * 6f;
                    handNPC.netSpam = 0;
                    handNPC.netUpdate = true;
                }
            }
        }

        /// <summary>
        /// Handles the shooting of one of Ares' gauss nukes.
        /// </summary>
        /// <param name="hand">The hand's NPC instance.</param>
        /// <param name="handNPC">The hand's ModNPC instance.</param>
        /// <param name="time">The timer used for the purposes of shooting.</param>
        /// <param name="chargeUpDuration">How long the nuke spends charging up before firing.</param>
        /// <param name="explosionDiameter">The big the diameter of the resulting explosion from the nuke should be.</param>
        public static void HandleGaussNukeShots(AresHand hand, NPC handNPC, int time, int chargeUpDuration, float explosionDiameter)
        {
            hand.RotateToLookAt(Target.Center);

            if (hand.EnergyDrawer.chargeProgress > 0f)
                hand.Frame = (int)(hand.EnergyDrawer.chargeProgress * 34f);
            else
            {
                if (time % 5 == 4)
                    hand.Frame++;

                // If the hand was already past frame 24 (which it is when starting this animation), it must complete its animation of rebuilding
                // a nuke crystal.
                if (hand.Frame >= 24)
                {
                    if (hand.Frame >= 80)
                        hand.Frame = 0;
                }

                // Otherwise, it should simply loop.
                else
                    hand.Frame %= 23;
            }

            if (time == chargeUpDuration - 12)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.LargeWeaponFireSound, handNPC.Center);
                ScreenShakeSystem.StartShakeAtPoint(handNPC.Center, 6f);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 handDirection = handNPC.rotation.ToRotationVector2() * handNPC.spriteDirection;
                    Vector2 nukeSpawnPosition = handNPC.Center + handDirection * 40f;
                    Vector2 nukeVelocity = handDirection * 72f;
                    Utilities.NewProjectileBetter(handNPC.GetSource_FromAI(), nukeSpawnPosition, nukeVelocity, ModContent.ProjectileType<GaussNuke>(), NukeWeaponDamage, 0f, -1, explosionDiameter);

                    handNPC.velocity -= handDirection * 35f;
                    handNPC.netUpdate = true;
                }
            }
        }
    }
}
