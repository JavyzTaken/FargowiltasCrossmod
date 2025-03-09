using CalamityMod;
using CalamityMod.World;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    public sealed partial class DraedonEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The monologue that Draedon should use at the start of the battle. Once he's been spoken to, his dialogue is a lot lighter.
        /// </summary>
        public static DraedonDialogueChain StartingMonologueToUse => CalamityWorld.TalkedToDraedon ? StartingMonologueBrief : StartingMonologue;

        /// <summary>
        /// The AI method that makes Draedon speak to the player before the battle.
        /// </summary>
        public void DoBehavior_StartingMonologue()
        {
            int speakTimer = (int)AITimer - 90;
            StartingMonologueToUse.Process(speakTimer);

            bool playerHasSelectedExoMech = CalamityWorld.DraedonMechToSummon != ExoMech.None;
            if (StartingMonologueToUse.Finished(speakTimer))
            {
                if (!playerHasSelectedExoMech)
                    PlayerToFollow.Calamity().AbleToSelectExoMech = true;
                else
                    ChangeAIState(DraedonAIState.ExoMechSpawnAnimation);

                // Mark Draedon as talked to.
                if (!CalamityWorld.TalkedToDraedon)
                {
                    CalamityWorld.TalkedToDraedon = true;
                    CalamityNetcode.SyncWorld();
                }
            }

            if (Frame <= 10f)
            {
                if (FrameTimer % 7f == 6f)
                {
                    Frame++;
                    FrameTimer = 0f;
                }
            }
            else
                Frame = (int)MathHelper.Lerp(11f, 15f, FrameTimer / 30f % 1f);
        }
    }
}
