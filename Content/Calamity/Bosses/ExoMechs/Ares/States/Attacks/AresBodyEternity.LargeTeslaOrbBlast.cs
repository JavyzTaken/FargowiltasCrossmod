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
            Utilities.InverseLerp(0f, LargeTeslaOrbBlast_ExplodeAnticipationTime, AITimer - LargeTeslaOrbBlast_OrbChargeUpTime - LargeTeslaOrbBlast_HomingBurstReleaseDelay - LargeTeslaOrbBlast_HomingBurstReleaseTime);

        /// <summary>
        /// How much damage tesla bursts shot by Ares do.
        /// </summary>
        public static int TeslaBurstDamage => Main.expertMode ? 350 : 225;

        /// <summary>
        /// How long the tesla orb spends charging energy before firing during the LargeTeslaOrbBlast attack.
        /// </summary>
        public static int LargeTeslaOrbBlast_OrbChargeUpTime => Utilities.SecondsToFrames(2.5f);

        /// <summary>
        /// How long the tesla orb waits after charging energy before releasing homing bursts during the LargeTeslaOrbBlast attack.
        /// </summary>
        public static int LargeTeslaOrbBlast_HomingBurstReleaseDelay => Utilities.SecondsToFrames(1f);

        /// <summary>
        /// The rate at which homing bursts are shot from the tesla orb during the LargeTeslaOrbBlast attack.
        /// </summary>
        public static int LargeTeslaOrbBlast_BurstReleaseRate => Utilities.SecondsToFrames(0.3574f);

        /// <summary>
        /// How much time is spent releasing homing tesla bursts during the LargeTeslaOrbBlast attack.
        /// </summary>
        public static int LargeTeslaOrbBlast_HomingBurstReleaseTime => Utilities.SecondsToFrames(5f);

        /// <summary>
        /// How long the tesla orb spends collapsing in anticipation of its explosion during the LargeTeslaOrbBlast attack.
        /// </summary>
        public static int LargeTeslaOrbBlast_ExplodeAnticipationTime => Utilities.SecondsToFrames(1.1f);

        /// <summary>
        /// How long Ares waits before transitioning to the next state following the explosion during the LargeTeslaOrbBlast attack.
        /// </summary>
        public static int LargeTeslaOrbBlast_AttackTransitionDelay => Utilities.SecondsToFrames(3.2f);

        /// <summary>
        /// The default offset of Ares' orb during the LargeTeslaOrbBlast attack.
        /// </summary>
        public static Vector2 LargeTeslaOrbBlast_BaseOrbOffset => Vector2.UnitY * 360f;

        /// <summary>
        /// AI update loop method for the LargeTeslaOrbBlast attack.
        /// </summary>
        public void DoBehavior_LargeTeslaOrbBlast()
        {
            // Create the tesla sphere at first.
            var teslaSpheres = Utilities.AllProjectilesByID(ModContent.ProjectileType<LargeTeslaSphere>());
            float reelBackInterpolant = Utilities.InverseLerp(0f, 90f, AITimer - LargeTeslaOrbBlast_OrbChargeUpTime).Squared();
            if (!teslaSpheres.Any() && AITimer <= 10)
            {
                SoundEngine.PlaySound(AresTeslaCannon.TelSound);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Utilities.NewProjectileBetter(NPC.GetSource_FromAI(), NPC.Center + LargeTeslaOrbBlast_BaseOrbOffset, Vector2.Zero, ModContent.ProjectileType<LargeTeslaSphere>(), 500, 0f);
            }

            Projectile? teslaSphere = teslaSpheres.FirstOrDefault();
            if (teslaSphere is not null)
                DoBehavior_LargeTeslaOrbBlast_ManageSphere(teslaSphere, reelBackInterpolant);

            Vector2 flyDestination = Target.Center + new Vector2(reelBackInterpolant * NPC.HorizontalDirectionTo(Target.Center) * -300f, -275f);
            StandardFlyTowards(flyDestination);

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

            // Update hte tesla sphere's size as the animation goes on.
            float chargeUpInterpolant = Utilities.InverseLerp(0f, LargeTeslaOrbBlast_OrbChargeUpTime, AITimer);
            Vector2 teslaSphereSize = Vector2.Lerp(Vector2.One * 2f, Vector2.One * 750f, chargeUpInterpolant.Cubed());
            teslaSphereSize *= MathHelper.SmoothStep(1f, MathF.Cos(AITimer) * 0.07f + 0.4f, LargeTeslaOrbBlast_ExplodeAnticipationInterpolant);
            teslaSphere.Resize((int)teslaSphereSize.X, (int)teslaSphereSize.Y);

            // Add a tiny amount of screen shake throughout the attack.
            ScreenShakeSystem.SetUniversalRumble(LargeTeslaOrbBlast_ExplodeAnticipationInterpolant.Squared() * 3f);

            if (AITimer >= LargeTeslaOrbBlast_OrbChargeUpTime + LargeTeslaOrbBlast_HomingBurstReleaseDelay + LargeTeslaOrbBlast_HomingBurstReleaseTime + LargeTeslaOrbBlast_ExplodeAnticipationTime + 60)
            {
                teslaSphere.Kill();

                int handID = ModContent.NPCType<AresHand>();
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.type == handID)
                    {
                        n.velocity -= n.SafeDirectionTo(teslaSphere.Center) * 44f;
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
            Utilities.NewProjectileBetter(NPC.GetSource_FromAI(), burstSpawnPosition, burstShootDirection * 42f, ModContent.ProjectileType<HomingTeslaBurst>(), TeslaBurstDamage, 0f);
        }

        public void LargeTeslaOrbBlastHandUpdate(AresHand hand, Projectile? teslaSphere, Vector2 hoverOffset, int armIndex)
        {
            NPC teslaCannon = hand.NPC;
            teslaCannon.Center = NPC.Center + hoverOffset * NPC.scale;
            teslaCannon.Opacity = Utilities.Saturate(teslaCannon.Opacity + 0.2f);
            hand.UsesBackArm = armIndex == 0 || armIndex == ArmCount - 1;
            hand.ArmSide = (armIndex >= ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.TeslaCannon;
            ref float angularVelocity = ref teslaCannon.localAI[0];

            if (teslaSphere is not null)
            {
                teslaCannon.SmoothFlyNear(NPC.Center + hoverOffset * NPC.scale, 0.2f, 0.84f);

                hand.RotateToLookAt(teslaSphere.Center);
                hand.EnergyDrawer.chargeProgress = Utilities.InverseLerp(150f, 700f, teslaSphere.width) * 0.9999f;
                hand.EnergyDrawer.SpawnAreaCompactness = LargeTeslaOrbBlast_ExplodeAnticipationInterpolant * 100f;
                hand.GlowmaskDisabilityInterpolant = 0f;

                teslaCannon.velocity += Main.rand.NextVector2Circular(3f, 3f) * hand.EnergyDrawer.chargeProgress;

                // Create a bunch of arcs between the tesla cannon and the sphere.
                float arcCreationChance = Utils.Remap(teslaSphere.width, 175f, 700f, 0.05f, 1f) * MathHelper.Lerp(1f, 0.3f, LargeTeslaOrbBlast_ExplodeAnticipationInterpolant);
                if (AITimer >= LargeTeslaOrbBlast_OrbChargeUpTime)
                    arcCreationChance *= 0.4f;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 arcSpawnPosition = teslaCannon.Center + new Vector2(teslaCannon.spriteDirection * 54f, 8f).RotatedBy(teslaCannon.rotation);
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(arcCreationChance))
                    {
                        Vector2 arcLength = (teslaSphere.Center - arcSpawnPosition).RotatedByRandom(0.02f) * Main.rand.NextFloat(0.97f, 1.03f);
                        Utilities.NewProjectileBetter(teslaCannon.GetSource_FromAI(), arcSpawnPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(6, 9));
                    }
                }

                // Prepare angular velocity for later.
                // This will not affect the hand's orientation now, since this only affects things once the tesla sphere is eventually gone.
                angularVelocity = teslaSphere.HorizontalDirectionTo(teslaCannon.Center) * -4f;
            }
            else
            {
                teslaCannon.SmoothFlyNear(NPC.Center + hoverOffset * NPC.scale, 0.09f, 0.85f);

                hand.EnergyDrawer.chargeProgress = 0f;
                hand.GlowmaskDisabilityInterpolant = 1f;
                teslaCannon.spriteDirection = 1;

                // Adhere to angular velocity.
                // Recall that an initial impulse is in reserve from when the tesla sphere was still present, which will ensure that the hands spin wildly right after the explosion, before
                // settling doing and dangling.
                teslaCannon.rotation = Utilities.WrapAngle360(teslaCannon.rotation + angularVelocity);
                angularVelocity -= angularVelocity * 0.08f - MathF.Cos(teslaCannon.rotation) * 0.009f;
            }

            hand.ArmEndpoint = teslaCannon.Center + teslaCannon.velocity;

            if (AITimer % 20 == 19 && hand.EnergyDrawer.chargeProgress >= 0.4f)
            {
                int pulseCounter = (int)MathF.Round(hand.EnergyDrawer.chargeProgress * 5f);
                hand.EnergyDrawer.AddPulse(pulseCounter);
            }

            hand.Frame = AITimer / 3 % 12;
        }
    }
}
