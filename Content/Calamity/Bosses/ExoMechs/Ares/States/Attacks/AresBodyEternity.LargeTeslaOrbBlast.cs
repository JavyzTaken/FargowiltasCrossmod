using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    public sealed partial class AresBodyEternity : CalDLCEmodeBehavior
    {
        public float LargeTeslaOrbBlast_ExplodeAnticipationInterpolant =>
            LumUtils.InverseLerp(0f, LargeTeslaOrbBlast_ExplodeAnticipationTime, AITimer - LargeTeslaOrbBlast_OrbChargeUpTime - LargeTeslaOrbBlast_HomingBurstReleaseDelay - LargeTeslaOrbBlast_HomingBurstReleaseTime);

        /// <summary>
        /// How much damage tesla bursts shot by Ares do.
        /// </summary>
        public static int TeslaBurstDamage => Variables.GetAIInt("TeslaBurstDamage", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long the tesla orb spends charging energy before firing during the Large Tesla Orb Blast attack.
        /// </summary>
        public static int LargeTeslaOrbBlast_OrbChargeUpTime => Variables.GetAIInt("LargeTeslaOrbBlast_OrbChargeUpTime", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long the tesla orb waits after charging energy before releasing homing bursts during the Large Tesla Orb Blast attack.
        /// </summary>
        public static int LargeTeslaOrbBlast_HomingBurstReleaseDelay => Variables.GetAIInt("LargeTeslaOrbBlast_HomingBurstReleaseDelay", ExoMechAIVariableType.Ares);

        /// <summary>
        /// The rate at which homing bursts are shot from the tesla orb during the Large Tesla Orb Blast attack.
        /// </summary>
        public static int LargeTeslaOrbBlast_BurstReleaseRate => Variables.GetAIInt("LargeTeslaOrbBlast_BurstReleaseRate", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How much time is spent releasing homing tesla bursts during the Large Tesla Orb Blast attack.
        /// </summary>
        public static int LargeTeslaOrbBlast_HomingBurstReleaseTime => Variables.GetAIInt("LargeTeslaOrbBlast_HomingBurstReleaseTime", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long the tesla orb spends collapsing in anticipation of its explosion during the Large Tesla Orb Blast attack.
        /// </summary>
        public static int LargeTeslaOrbBlast_ExplodeAnticipationTime => Variables.GetAIInt("LargeTeslaOrbBlast_ExplodeAnticipationTime", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long Ares waits before transitioning to the next state following the explosion during the Large Tesla Orb Blast attack.
        /// </summary>
        public static int LargeTeslaOrbBlast_AttackTransitionDelay => Variables.GetAIInt("LargeTeslaOrbBlast_AttackTransitionDelay", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How much recoil is applied to Ares' cannons upong firing during the Large Tesla Orb Blast attack.
        /// </summary>
        public static float LargeTeslaOrbBlast_RecoilSpeed => Variables.GetAIFloat("LargeTeslaOrbBlast_RecoilSpeed", ExoMechAIVariableType.Ares);

        /// <summary>
        /// The default offset of Ares' orb during the Large Tesla Orb Blast attack.
        /// </summary>
        public static Vector2 LargeTeslaOrbBlast_BaseOrbOffset => Vector2.UnitY * 360f;

        /// <summary>
        /// AI update loop method for the Large Tesla Orb Blast attack.
        /// </summary>
        public void DoBehavior_LargeTeslaOrbBlast()
        {
            // Create the tesla sphere at first.
            var teslaSpheres = LumUtils.AllProjectilesByID(ModContent.ProjectileType<LargeTeslaSphere>());
            float reelBackInterpolant = LumUtils.InverseLerp(0f, 90f, AITimer - LargeTeslaOrbBlast_OrbChargeUpTime).Squared();
            if (!teslaSpheres.Any() && AITimer <= 10)
            {
                SoundEngine.PlaySound(AresTeslaCannon.TelSound);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), NPC.Center + LargeTeslaOrbBlast_BaseOrbOffset, Vector2.Zero, ModContent.ProjectileType<LargeTeslaSphere>(), 500, 0f);
            }

            Projectile? teslaSphere = teslaSpheres.FirstOrDefault();
            if (teslaSphere is not null)
                DoBehavior_LargeTeslaOrbBlast_ManageSphere(teslaSphere, reelBackInterpolant);

            Vector2 flyDestination = Target.Center + new Vector2(reelBackInterpolant * NPC.HorizontalDirectionTo(Target.Center) * -300f, -275f);
            StandardFlyTowards(flyDestination);

            // Move less during the startup, to avoid cheap hits and give people time to read what's happening
            if (AITimer < LargeTeslaOrbBlast_OrbChargeUpTime / 2)
                NPC.velocity *= 0.9f;

            bool readyToShootBursts = AITimer >= LargeTeslaOrbBlast_OrbChargeUpTime + LargeTeslaOrbBlast_HomingBurstReleaseDelay;
            bool doneShootingBursts = AITimer >= LargeTeslaOrbBlast_OrbChargeUpTime + LargeTeslaOrbBlast_HomingBurstReleaseDelay + LargeTeslaOrbBlast_HomingBurstReleaseTime;
            bool shootingBursts = readyToShootBursts && !doneShootingBursts;
            if (AITimer % LargeTeslaOrbBlast_BurstReleaseRate == LargeTeslaOrbBlast_BurstReleaseRate - 1 && shootingBursts && teslaSphere is not null)
                DoBehavior_LargeTeslaOrbBlast_ReleaseBurst(teslaSphere);

            InstructionsForHands[0] = new(h => LargeTeslaOrbBlastHandUpdate(h, teslaSphere, new Vector2(-430f, 40f), 0));
            InstructionsForHands[1] = new(h => LargeTeslaOrbBlastHandUpdate(h, teslaSphere, new Vector2(-300f, 224f), 1));
            InstructionsForHands[2] = new(h => LargeTeslaOrbBlastHandUpdate(h, teslaSphere, new Vector2(300f, 224f), 2));
            InstructionsForHands[3] = new(h => LargeTeslaOrbBlastHandUpdate(h, teslaSphere, new Vector2(430f, 40f), 3));

            if (AITimer >= LargeTeslaOrbBlast_OrbChargeUpTime + LargeTeslaOrbBlast_HomingBurstReleaseDelay + LargeTeslaOrbBlast_HomingBurstReleaseTime + LargeTeslaOrbBlast_ExplodeAnticipationTime + LargeTeslaOrbBlast_AttackTransitionDelay)
                SelectNewState();
        }

        public void DoBehavior_LargeTeslaOrbBlast_ManageSphere(Projectile teslaSphere, float reelBackInterpolant)
        {
            // Keep the tesla sphere below Ares.
            Vector2 sphereHoverDestination = NPC.Center + LargeTeslaOrbBlast_BaseOrbOffset;
            sphereHoverDestination -= NPC.SafeDirectionTo(Target.Center) * reelBackInterpolant * new Vector2(200f, 50f);
            sphereHoverDestination.Y += reelBackInterpolant * 100f;
            teslaSphere.Center = Vector2.Lerp(teslaSphere.Center, sphereHoverDestination, 0.06f);
            teslaSphere.velocity += (sphereHoverDestination - teslaSphere.Center) * 0.0051f;

            // Update the tesla sphere's size as the animation goes on.
            float chargeUpInterpolant = LumUtils.InverseLerp(0f, LargeTeslaOrbBlast_OrbChargeUpTime, AITimer);
            Vector2 teslaSphereSize = Vector2.Lerp(Vector2.One * 2f, Vector2.One * 750f, chargeUpInterpolant.Cubed());
            teslaSphereSize *= MathHelper.SmoothStep(1f, MathF.Cos(AITimer) * 0.07f + 0.4f, LargeTeslaOrbBlast_ExplodeAnticipationInterpolant);
            teslaSphere.Resize((int)teslaSphereSize.X, (int)teslaSphereSize.Y);

            // Add a tiny amount of screen shake throughout the attack.
            ScreenShakeSystem.SetUniversalRumble(LargeTeslaOrbBlast_ExplodeAnticipationInterpolant.Squared() * 3f, MathHelper.TwoPi, null, 0.2f);

            if (AITimer >= LargeTeslaOrbBlast_OrbChargeUpTime + LargeTeslaOrbBlast_HomingBurstReleaseDelay + LargeTeslaOrbBlast_HomingBurstReleaseTime + LargeTeslaOrbBlast_ExplodeAnticipationTime + 60)
            {
                teslaSphere.Kill();

                int handID = ModContent.NPCType<AresHand>();
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.type == handID)
                    {
                        n.velocity -= n.SafeDirectionTo(teslaSphere.Center) * LargeTeslaOrbBlast_RecoilSpeed;
                        n.netUpdate = true;
                    }
                }
            }
        }

        public void DoBehavior_LargeTeslaOrbBlast_ReleaseBurst(Projectile teslaSphere)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            float burstOffsetAngle = MathF.Cos(MathHelper.TwoPi * AITimer / 120f) * MathHelper.PiOver2;
            Vector2 burstShootDirection = teslaSphere.SafeDirectionTo(Target.Center).RotatedBy(burstOffsetAngle);
            Vector2 burstSpawnPosition = teslaSphere.Center + burstShootDirection * teslaSphere.width * Main.rand.NextFloat(0.1f);
            LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), burstSpawnPosition, burstShootDirection * 54f, ModContent.ProjectileType<HomingTeslaBurst>(), TeslaBurstDamage, 0f);
        }

        public void LargeTeslaOrbBlastHandUpdate(AresHand hand, Projectile? teslaSphere, Vector2 hoverOffset, int armIndex)
        {
            NPC handNPC = hand.NPC;
            handNPC.SmoothFlyNear(NPC.Center + hoverOffset * NPC.scale, 0.3f, 0.8f);
            handNPC.Opacity = LumUtils.Saturate(handNPC.Opacity + 0.2f);
            hand.UsesBackArm = armIndex == 0 || armIndex == ArmCount - 1;
            hand.ArmSide = (armIndex >= ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.TeslaCannon;
            ref float angularVelocity = ref handNPC.localAI[0];

            if (teslaSphere is not null)
            {
                handNPC.SmoothFlyNear(NPC.Center + hoverOffset * NPC.scale, 0.2f, 0.84f);

                hand.RotateToLookAt(teslaSphere.Center);
                hand.EnergyDrawer.chargeProgress = LumUtils.InverseLerp(150f, 700f, teslaSphere.width) * 0.9999f;
                hand.EnergyDrawer.SpawnAreaCompactness = LargeTeslaOrbBlast_ExplodeAnticipationInterpolant * 100f;
                hand.GlowmaskDisabilityInterpolant = 0f;

                handNPC.velocity += Main.rand.NextVector2Circular(3f, 3f) * hand.EnergyDrawer.chargeProgress;

                // Create a bunch of arcs between the tesla cannon and the sphere.
                float arcCreationChance = Utils.Remap(teslaSphere.width, 175f, 700f, 0.05f, 1f) * MathHelper.Lerp(1f, 0.3f, LargeTeslaOrbBlast_ExplodeAnticipationInterpolant);
                if (AITimer >= LargeTeslaOrbBlast_OrbChargeUpTime)
                    arcCreationChance *= 0.4f;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 arcSpawnPosition = handNPC.Center + new Vector2(handNPC.spriteDirection * 54f, 8f).RotatedBy(handNPC.rotation);
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(arcCreationChance))
                    {
                        Vector2 arcLength = (teslaSphere.Center - arcSpawnPosition).RotatedByRandom(0.02f) * Main.rand.NextFloat(0.97f, 1.03f);
                        LumUtils.NewProjectileBetter(handNPC.GetSource_FromAI(), arcSpawnPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(6, 9));
                    }
                }

                // Prepare angular velocity for later.
                // This will not affect the hand's orientation now, since this only affects things once the tesla sphere is eventually gone.
                angularVelocity = teslaSphere.HorizontalDirectionTo(handNPC.Center) * -4f;
            }
            else
            {
                handNPC.SmoothFlyNear(NPC.Center + hoverOffset * NPC.scale, 0.09f, 0.85f);

                hand.EnergyDrawer.chargeProgress = 0f;
                hand.GlowmaskDisabilityInterpolant = 1f;
                handNPC.spriteDirection = 1;

                // Adhere to angular velocity.
                // Recall that an initial impulse is in reserve from when the tesla sphere was still present, which will ensure that the hands spin wildly right after the explosion, before
                // settling doing and dangling.
                handNPC.rotation = LumUtils.WrapAngle360(handNPC.rotation + angularVelocity);
                angularVelocity -= angularVelocity * 0.08f - MathF.Cos(handNPC.rotation) * 0.009f;
            }

            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;

            if (AITimer % 20 == 19 && hand.EnergyDrawer.chargeProgress >= 0.4f)
            {
                int pulseCounter = (int)MathF.Round(hand.EnergyDrawer.chargeProgress * 5f);
                hand.EnergyDrawer.AddPulse(pulseCounter);
            }

            hand.Frame = AITimer / 3 % 12;
        }
    }
}
