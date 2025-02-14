using Luminance.Common.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue
{
    public class DraedonDialogueChain(int pauseBetweenDialogue = 30)
    {
        /// <summary>
        /// The tree of all dialogue this chain has.
        /// </summary>
        internal readonly LinkedList<Func<DraedonDialogue>> Chain = [];

        /// <summary>
        /// How much of a pause there should be, in frames, between spoken dialogue.
        /// </summary>
        public int PauseBetweenDialogue
        {
            get;
            init;
        } = pauseBetweenDialogue;

        /// <summary>
        /// Adds a new dialogue instance to the chain based on a given text key.
        /// </summary>
        /// <param name="dialogueKey">The dialogue's identifier key.</param>
        public DraedonDialogueChain Add(string dialogueKey)
        {
            // A lambda is used instead of direct access to ensure that access is deferred after mod loading.
            Add(() => DraedonDialogueManager.Dialogue[dialogueKey]);
            return this;
        }

        /// <summary>
        /// Adds a new dialogue instance to the chain.
        /// </summary>
        /// <param name="dialogue">The dialogue to add.</param>
        public DraedonDialogueChain Add(DraedonDialogue dialogue)
        {
            Add(() => dialogue);
            return this;
        }

        /// <summary>
        /// Adds a new dialogue selector function instance to the chain.
        /// </summary>
        /// <param name="dialogue">The dialogue to add.</param>
        public DraedonDialogueChain Add(Func<DraedonDialogue> dialogue)
        {
            Chain.AddLast(dialogue);
            return this;
        }

        /// <summary>
        /// Determines the currently active dialogue instance based on a given time state.
        /// </summary>
        /// 
        /// <remarks>
        /// Returns null if the dialogue chain has been completed.
        /// </remarks>
        /// 
        /// <param name="time">How much time has passed since the start of the dialogue chain's playback.</param>
        public DraedonDialogue? GetActiveDialogue(ref int time)
        {
            var evaluatedNode = Chain.First;
            int intermediateWait = DraedonDialogueManager.UseSubtitles ? 0 : PauseBetweenDialogue;
            if (time < 0)
                return evaluatedNode.Value();

            while (time > evaluatedNode.Value().Duration)
            {
                time -= evaluatedNode.Value().Duration + intermediateWait;
                evaluatedNode = evaluatedNode.Next;
                if (evaluatedNode is null)
                    break;
            }

            return evaluatedNode?.Value() ?? null;
        }

        /// <summary>
        /// Handles processing of this dialogue chain based on a given time state, making dialogue play out, returning important intermittent variables in the process.
        /// </summary>
        /// <param name="time">How much time has passed since the start of the dialogue chain's playback.</param>
        public void Process(int time, out DraedonDialogue dialogue, out int relativeTime)
        {
            // Relative time in this sense refers to how much time has elapsed since the given dialogue instance began.
            relativeTime = time;
            dialogue = GetActiveDialogue(ref relativeTime);

            if (dialogue is null)
            {
                DraedonSubtitleManager.Stop();
                return;
            }

            // Check if the dialogue just began.
            // If it has, and subtitles are disabled, create a chat text message.
            if (relativeTime == 1 && Main.netMode != NetmodeID.MultiplayerClient && !DraedonDialogueManager.UseSubtitles)
                Utilities.BroadcastLocalizedText(dialogue.LocalizationKey, dialogue.Text.TextColor);

            if (DraedonDialogueManager.UseSubtitles)
                DraedonSubtitleManager.Play(relativeTime, dialogue);
        }

        /// <summary>
        /// Handles processing of this dialogue chain based on a given time state, making dialogue play out.
        /// </summary>
        /// <param name="time">How much time has passed since the start of the dialogue chain's playback.</param>
        public void Process(int time) => Process(time, out _, out _);

        /// <summary>
        /// Determines whether this dialogue chain has been finished based on a given time state.
        /// </summary>
        /// <param name="time">How much time has passed since the start of the dialogue chain's playback.</param>
        public bool Finished(int time) => GetActiveDialogue(ref time) is null;
    }
}
