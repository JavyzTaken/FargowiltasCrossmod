using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Terraria;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    public sealed partial class DraedonEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The monologue that Draedon uses if the player dies during the battle.
        /// </summary>
        public static readonly DraedonDialogueChain StandardPlayerDeathMonologue = new DraedonDialogueChain().
            Add("Death");

        /// <summary>
        /// The monologue that Draedon uses if the player dies in the middle of him speaking.
        /// </summary>
        public static readonly DraedonDialogueChain FunnyPlayerDeathMonologue = new DraedonDialogueChain().
            Add("PlayerDeathAtAmusingTime");

        /// <summary>
        /// The AI method that makes Draedon speak after the player dies.
        /// </summary>
        public void DoBehavior_PlayerDeathMonologue()
        {
            int speakTimer = (int)AITimer;
            var dialogue = AIState == DraedonAIState.FunnyPlayerDeathMonologue ? FunnyPlayerDeathMonologue : StandardPlayerDeathMonologue;
            dialogue.Process(speakTimer);

            NPC.velocity *= 0.9f;

            if (dialogue.Finished(speakTimer))
            {
                HologramOverlayInterpolant = Utilities.Saturate(HologramOverlayInterpolant + 0.04f);
                MaxSkyOpacity = 1f - HologramOverlayInterpolant;
                if (HologramOverlayInterpolant >= 1f)
                    NPC.active = false;
            }
            else
                HologramOverlayInterpolant = 0f;

            PerformStandardFraming();
        }
    }
}
