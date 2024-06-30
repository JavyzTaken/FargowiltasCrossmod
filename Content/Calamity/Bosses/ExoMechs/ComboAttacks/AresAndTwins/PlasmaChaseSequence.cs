using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PlasmaChaseSequence : ExoMechComboHandler
    {
        /// <summary>
        /// How long Ares spends charging up energy for his plasma cannons.
        /// </summary>
        public static int PlasmaCannonChargeUpTime => LumUtils.SecondsToFrames(2.62f);

        /// <summary>
        /// How much damage plasma flame jets from Ares' plasma cannons do.
        /// </summary>
        public static int PlasmaJetDamage => Main.expertMode ? 500 : 360;

        /// <summary>
        /// The speed at which Ares chases his target.
        /// </summary>
        public static float AresChaseSpeedInterpolant => 0.033f;

        public override int[] ExpectedManagingExoMechs => [ModContent.NPCType<AresBody>(), ModContent.NPCType<Apollo>()];

        public override bool Perform(NPC npc)
        {
            if (npc.type == ExoMechNPCIDs.AresBodyID)
                Perform_Ares(npc);

            return false;
        }

        /// <summary>
        /// Performs Ares' part in the PlasmaChaseSequence attack.
        /// </summary>
        /// <param name="npc">Ares' NPC instance.</param>
        public static void Perform_Ares(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out AresBodyEternity ares))
            {
                npc.active = false;
                return;
            }

            if (Main.mouseRight && Main.mouseRightRelease)
            {
                ExoMechFightStateManager.ClearExoMechProjectiles();
                AITimer = 0;
            }

            if (AITimer == Math.Max(1, PlasmaCannonChargeUpTime - LumUtils.SecondsToFrames(2.6f)))
                SoundEngine.PlaySound(AresPlasmaFlamethrower.TelSound);

            if (AITimer <= PlasmaCannonChargeUpTime)
            {
                npc.velocity *= 0.95f;
                ares.AnimationState = AresBodyEternity.AresFrameAnimationState.Default;
            }
            else
            {
                float moveSpeedInterpolant = LumUtils.InverseLerp(0f, 45f, AITimer - PlasmaCannonChargeUpTime);
                Vector2 hoverDestination = Target.Center + new Vector2(npc.OnRightSideOf(Target).ToDirectionInt() * 150f, -450f);
                npc.SmoothFlyNear(hoverDestination, moveSpeedInterpolant * AresChaseSpeedInterpolant, 0.92f);

                float moveAbovePlayerInterpolant = LumUtils.InverseLerp(400f, 150f, Target.Center.Y - npc.Center.Y) + 0.2f;
                npc.Center = Vector2.Lerp(npc.Center, new(npc.Center.X, hoverDestination.Y), moveAbovePlayerInterpolant * 0.2f);
                ares.AnimationState = AresBodyEternity.AresFrameAnimationState.Laugh;
            }

            if (AITimer == PlasmaCannonChargeUpTime)
            {
                SoundEngine.PlaySound(AresBodyEternity.LaughSound);
                SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaShootSound);
                SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaExplosionSound);
                ScreenShakeSystem.StartShake(10f);

                npc.velocity.Y -= 54f;
                npc.netUpdate = true;
            }

            npc.rotation = npc.velocity.X * 0.015f;

            float chargeUpInterpolant = LumUtils.InverseLerp(0f, PlasmaCannonChargeUpTime, AITimer);
            float handRepositionInterpolant = chargeUpInterpolant.Squared();
            Vector2 farLeftHandOffset = Vector2.SmoothStep(new Vector2(-448f, 90f), new Vector2(-400f, 50f), handRepositionInterpolant);
            Vector2 leftHandOffset = Vector2.SmoothStep(new Vector2(-250f, 220f), new Vector2(-140f, 274f), handRepositionInterpolant);
            Vector2 rightHandOffset = Vector2.SmoothStep(new Vector2(250f, 220f), new Vector2(140f, 274f), handRepositionInterpolant);
            Vector2 farRightHandOffset = Vector2.SmoothStep(new Vector2(448f, 90f), new Vector2(400f, 50f), handRepositionInterpolant);

            ares.InstructionsForHands[0] = new(h => AresHandUpdate(npc, h, farLeftHandOffset, 0));
            ares.InstructionsForHands[1] = new(h => AresHandUpdate(npc, h, leftHandOffset, 1));
            ares.InstructionsForHands[2] = new(h => AresHandUpdate(npc, h, rightHandOffset, 2));
            ares.InstructionsForHands[3] = new(h => AresHandUpdate(npc, h, farRightHandOffset, 3));
        }

        /// <summary>
        /// Handles updating for Ares' hands during the PlasmaChaseSequence attack.
        /// </summary>
        /// <param name="aresBody">Ares' NPC instance.</param>
        /// <param name="hand">The hand ModNPC instance.</param>
        /// <param name="hoverOffset">The offset for the hand relative to the body's position.</param>
        /// <param name="armIndex">The arm's index in the overall set.</param>
        public static void AresHandUpdate(NPC aresBody, AresHand hand, Vector2 hoverOffset, int armIndex)
        {
            NPC handNPC = hand.NPC;
            Vector2 hoverDestination = aresBody.Center + hoverOffset * aresBody.scale;

            hand.KatanaInUse = true;
            hand.UsesBackArm = armIndex == 0 || armIndex == AresBodyEternity.ArmCount - 1;
            hand.ArmSide = (armIndex >= AresBodyEternity.ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.PlasmaCannon;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.GlowmaskDisabilityInterpolant = 0f;
            hand.Frame = AITimer / 4 % 12;
            handNPC.spriteDirection = -hand.ArmSide;
            handNPC.Opacity = LumUtils.Saturate(handNPC.Opacity + 0.3f);

            hand.EnergyDrawer.chargeProgress = LumUtils.InverseLerp(0f, PlasmaCannonChargeUpTime * 0.85f, AITimer);

            float idealRotation = handNPC.AngleTo(aresBody.Center + Vector2.UnitY * 400f).AngleLerp(MathHelper.PiOver2, hand.EnergyDrawer.chargeProgress);
            if (handNPC.spriteDirection == -1)
                idealRotation += MathHelper.Pi;

            handNPC.rotation = handNPC.rotation.AngleLerp(idealRotation, 0.1f);

            if (AITimer >= PlasmaCannonChargeUpTime)
            {
                hand.Frame += 24;
                hand.EnergyDrawer.chargeProgress = 0f;
            }

            handNPC.SmoothFlyNear(hoverDestination, 0.35f, 0.65f);

            Vector2 cannonEnd = handNPC.Center + new Vector2(handNPC.spriteDirection * 54f, 8f).RotatedBy(handNPC.rotation);
            if (Main.netMode != NetmodeID.MultiplayerClient && AITimer == PlasmaCannonChargeUpTime)
                LumUtils.NewProjectileBetter(handNPC.GetSource_FromAI(), cannonEnd, Vector2.UnitY, ModContent.ProjectileType<PlasmaFlameJet>(), PlasmaJetDamage, 0f, -1, handNPC.whoAmI);
        }
    }
}
