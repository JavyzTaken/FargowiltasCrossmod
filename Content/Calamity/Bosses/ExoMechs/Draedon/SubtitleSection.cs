namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    // TODO -- Use localization keys for this instead of raw text.
    /// <summary>
    /// A representation of a section of dialogue from Draedon that can be displayed by the titles.
    /// </summary>
    /// <param name="Start">The starting time of the section, in frames.</param>
    /// <param name="Text">The text associated with the subtitles.</param>
    public record SubtitleSection(int Start, string Text);
}
