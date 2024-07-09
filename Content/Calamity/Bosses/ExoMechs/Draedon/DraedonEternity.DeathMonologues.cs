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
        public static readonly DraedonDialogueChain StandardPlayerDeathMonologue = new DraedonDialogueChain("Mods.FargowiltasCrossmod.NPCs.Draedon.").
            Add("Death", null, Utilities.SecondsToFrames(1.85f));

        /// <summary>
        /// The monologue that Draedon uses if the player dies in the middle of him speaking.
        /// </summary>
        public static readonly DraedonDialogueChain FunnyPlayerDeathMonologue = new DraedonDialogueChain("Mods.FargowiltasCrossmod.NPCs.Draedon.").
            Add("DeathAtAmusingTime1", null, Utilities.SecondsToFrames(1.2f)).
            Add("DeathAtAmusingTime2", null, Utilities.SecondsToFrames(1.2f));

        /// <summary>
        /// The AI method that makes Draedon speak after the player dies.
        /// </summary>
        public void DoBehavior_PlayerDeathMonologue()
        {
            int speakTimer = (int)AITimer;
            var monologue = AIState == DraedonAIState.FunnyPlayerDeathMonologue ? FunnyPlayerDeathMonologue : StandardPlayerDeathMonologue;
            for (int i = 0; i < monologue.Count; i++)
            {
                if (speakTimer == monologue[i].SpeakDelay)
                    monologue[i].SayInChat();
            }

            NPC.velocity *= 0.9f;

            bool monologueIsFinished = speakTimer >= monologue.OverallDuration;
            if (monologueIsFinished)
            {
                HologramInterpolant = Utilities.Saturate(HologramInterpolant + 0.04f);
                MaxSkyOpacity = 1f - HologramInterpolant;
                if (HologramInterpolant >= 1f)
                    NPC.active = false;
            }
            else
                HologramInterpolant = 0f;

            PerformStandardFraming();
        }
    }
}
