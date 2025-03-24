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
        /// How long Ares spends entering the foreground during his spawn animation.
        /// </summary>
        public static int SpawnAnimation_EnterForegroundTime => Variables.GetAIInt("SpawnAnimation_EnterForegroundTime", ExoMechAIVariableType.Ares);

        /// <summary>
        /// AI update loop method for the inactive state.
        /// </summary>
        public void DoBehavior_SpawnAnimation()
        {
            ZPosition = LumUtils.InverseLerp(SpawnAnimation_EnterForegroundTime, 0f, AITimer).Cubed() * 8f;
            NPC.Center = Target.Center - Vector2.UnitY * (ZPosition * 16f + 200f);
            NPC.velocity *= 0.6f;
            NPC.dontTakeDamage = true;
            NPC.damage = 0;

            if (AITimer == 1)
                StandardLightColorPalette = ChooseStandardLightPalette();

            BasicHandUpdateWrapper();

            if (AITimer >= SpawnAnimation_EnterForegroundTime)
                SelectNewState();
        }

        public void BasicHandUpdateWrapper()
        {
            InstructionsForHands[0] = new(h => BasicHandUpdate(h, new Vector2(-430f, 50f), 0));
            InstructionsForHands[1] = new(h => BasicHandUpdate(h, new Vector2(-280f, 224f), 1));
            InstructionsForHands[2] = new(h => BasicHandUpdate(h, new Vector2(280f, 224f), 2));
            InstructionsForHands[3] = new(h => BasicHandUpdate(h, new Vector2(430f, 50f), 3));
        }

        public void BasicHandUpdate(AresHand hand, Vector2 hoverOffset, int armIndex)
        {
            NPC handNPC = hand.NPC;
            handNPC.Opacity = LumUtils.Saturate(handNPC.Opacity + 0.025f);
            if (CurrentState == AresAIState.SpawnAnimation)
                handNPC.Center = NPC.Center + hoverOffset * NPC.scale;

            handNPC.SmoothFlyNear(NPC.Center + hoverOffset * NPC.scale, 0.7f, 0.5f);
            handNPC.rotation = handNPC.rotation.AngleLerp(handNPC.spriteDirection * MathHelper.PiOver2, 0.12f);
            handNPC.damage = 0;

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
