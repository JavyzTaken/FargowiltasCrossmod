using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    /// <summary>
    /// Represents a collection of <see cref="DraedonDialogue"/> instances in a chain.
    /// </summary>
    public class DraedonDialogueChain
    {
        /// <summary>
        /// The set of all dialogue this chain has.
        /// </summary>
        internal readonly List<DraedonDialogue> Dialogue = new(4);

        /// <summary>
        /// How long, in frames, this dialogue takes to be said.
        /// </summary>
        public int OverallDuration
        {
            get;
            private set;
        }

        /// <summary>
        /// The amount of dialogue instances contained within this chain.
        /// </summary>
        public int Count => Dialogue.Count;

        /// <summary>
        /// An optional prefix that is applied to all dialogue added via the <see cref="Add(string, Color, int)"/> method. Useful for eliminating redundancy.
        /// </summary>
        public readonly string LocalizationPrefix;

        public DraedonDialogueChain(string localizationPrefix = "")
        {
            LocalizationPrefix = localizationPrefix;
        }

        /// <summary>
        /// Adds a new dialogue instance to this chain.
        /// </summary>
        /// <param name="localizationKey">The dialogue's localization key.</param>
        /// <param name="textColor">The dialogue's color.</param>
        /// <param name="speakTime">How much time, in frames, Draedon should allocate towards saying this dialogue.</param>
        /// <returns>The original chain, for the purpose of easy method chaining.</returns>
        public DraedonDialogueChain Add(string localizationKey, Color? textColor = null, int? speakTime = null)
        {
            speakTime ??= DraedonEternity.StandardSpeakTime;
            textColor ??= CalamityMod.NPCs.ExoMechs.Draedon.TextColor;

            Dialogue.Add(new(() => $"{LocalizationPrefix}{localizationKey}", textColor.Value, speakTime.Value, OverallDuration));
            OverallDuration += speakTime.Value;

            return this;
        }

        /// <summary>
        /// Adds a new dialogue instance to this chain.
        /// </summary>
        /// <param name="localizationKey">The dialogue's localization key.</param>
        /// <param name="textColor">The dialogue's color.</param>
        /// <param name="speakTime">How much time, in frames, Draedon should allocate towards saying this dialogue.</param>
        /// <returns>The original chain, for the purpose of easy method chaining.</returns>
        public DraedonDialogueChain Add(Func<string> localizationKey, Color? textColor = null, int? speakTime = null)
        {
            speakTime ??= DraedonEternity.StandardSpeakTime;
            textColor ??= CalamityMod.NPCs.ExoMechs.Draedon.TextColor;

            Dialogue.Add(new(() => $"{LocalizationPrefix}{localizationKey()}", textColor.Value, speakTime.Value, OverallDuration));
            OverallDuration += speakTime.Value;

            return this;
        }

        public DraedonDialogue this[int index]
        {
            get => Dialogue[index];
        }
    }
}
