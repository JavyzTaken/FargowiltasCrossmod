﻿using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    public sealed partial class AresBodyEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// AI update loop method for the DetachHands state.
        /// </summary>
        public void DoBehavior_DetachHands()
        {
            NPC.velocity *= new Vector2(0.5f, 0.93f);

            for (int i = 0; i < InstructionsForHands.Length; i++)
            {
                int copyForDelegate = i;
                InstructionsForHands[i] = new(h => DetachHandsUpdate(h, copyForDelegate));
            }

            if (AITimer >= 45)
            {
                do
                {
                    CurrentState = Main.rand.NextFromList(AresAIState.AimedLaserBursts, AresAIState.LargeTeslaOrbBlast, AresAIState.NukeAoEAndPlasmaBlasts);

                    if (Main.rand.NextBool() && ExoMechFightStateManager.CurrentPhase >= ExoMechFightDefinitions.BerserkSoloPhaseDefinition)
                        CurrentState = AresAIState.BackgroundCoreLaserBeams;
                    if (Main.rand.NextBool() && (NPC.life <= NPC.lifeMax * ExoMechFightDefinitions.FightAloneLifeRatio || ExoMechFightStateManager.CurrentPhase >= ExoMechFightDefinitions.BerserkSoloPhaseDefinition))
                        CurrentState = AresAIState.KatanaCycloneDashes;
                }
                while (CurrentState == PreviousState);

                PreviousState = CurrentState;

                if (WaitingToStartComboAttack)
                {
                    CurrentState = AresAIState.PerformComboAttack;
                    WaitingToStartComboAttack = false;
                }

                AITimer = 0;
                NPC.netUpdate = true;
            }
        }

        public void DetachHandsUpdate(AresHand hand, int armIndex)
        {
            NPC handNPC = hand.NPC;
            handNPC.Opacity = Utilities.Saturate(handNPC.Opacity - 0.025f);
            handNPC.velocity.X *= 0.84f;
            handNPC.velocity.Y += 0.36f;
            if (handNPC.velocity.Y < 0f)
                handNPC.velocity.Y *= 0.9f;

            hand.AttachedToArm = false;
            hand.UsesBackArm = armIndex == 0 || armIndex == ArmCount - 1;
            hand.ArmSide = (armIndex >= ArmCount / 2).ToDirectionInt();
            hand.Frame = AITimer / 3 % Math.Min(hand.HandType.TotalHorizontalFrames * hand.HandType.TotalVerticalFrames, 12);

            hand.ArmEndpoint = Vector2.Lerp(hand.ArmEndpoint, handNPC.Center + handNPC.velocity, handNPC.Opacity);
            hand.EnergyDrawer.chargeProgress *= 0.7f;

            if (handNPC.Opacity <= 0f)
                hand.GlowmaskDisabilityInterpolant = 0f;
        }
    }
}
