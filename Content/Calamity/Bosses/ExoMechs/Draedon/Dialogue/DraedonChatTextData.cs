using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue
{
    /// <summary>
    /// Represents an instance of data associated with chat text speakable by Draedon.
    /// </summary>
    /// <param name="TextColor">The dialogue's color when spoken.</param>
    /// <param name="Duration">How much time, in frames, Draedon should allocate towards saying this dialogue instance.</param>
    public record DraedonChatTextData(Color TextColor, int Duration);
}
