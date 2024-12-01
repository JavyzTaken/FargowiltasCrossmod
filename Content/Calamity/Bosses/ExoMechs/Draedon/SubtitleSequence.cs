namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    /// <summary>
    /// Represents a sequence of subtitles that play out in full across a span of time.
    /// </summary>
    /// <param name="SoundPath">The path to the associated sound used with the subtitles.</param>
    /// <param name="Duration">The duration of the sequence, in frames.</param>
    /// <param name="Sections">The subtitle section instances.</param>
    public record SubtitlesSequence(string SoundPath, int Duration, params SubtitleSection[] Sections)
    {
        /// <summary>
        /// How long this sequence lasts overall, in frames.
        /// </summary>
        public int Duration
        {
            get;
            private set;
        } = Duration;
    }
}
