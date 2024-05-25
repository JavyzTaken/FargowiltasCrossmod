using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria.Localization;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    /// <summary>
    /// Represents a single unit of dialogue that Draedon can say.
    /// </summary>
    /// <param name="LocalizationKey">The dialogue's localization key.</param>
    /// <param name="TextColor">The dialogue's color.</param>
    /// <param name="SpeakTime">How much time, in frames, Draedon should allocate towards saying this dialogue instance.</param>
    /// <param name="SpeakDelay">How much time, in frames, there is before Draedon can say this dialogue instance in a sequence.</param>
    public record DraedonDialogue(Func<string> LocalizationKey, Color TextColor, int SpeakTime, int SpeakDelay)
    {
        /// <summary>
        /// A neat shorthand for calling <see cref="Language.GetTextValue"/> on the <see cref="LocalizationKey"/>.
        /// </summary>
        public Func<string> Text => () => Language.GetTextValue(LocalizationKey());

        /// <summary>
        /// Says this dialogue instance in the chat.
        /// </summary>
        public void SayInChat() => Utilities.BroadcastLocalizedText(LocalizationKey(), TextColor);
    }
}
