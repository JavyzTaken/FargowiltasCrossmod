using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue
{
    /// <summary>
    /// Represents an instance of a subtitle created by Draedon.
    /// </summary>
    /// <param name="SoundPath">The path to the sound file to be played out by this subtitle.</param>
    /// <param name="Duration">The total play duration of the subtitle, in frames.</param>
    public record DraedonSubtitle(string SoundPath, int Duration)
    {
        public DraedonSubtitle(string soundPath) : this(soundPath, 0)
        {
            SoundStyle sound = new(soundPath);
            double durationInSeconds = sound.GetRandomSound().Duration.TotalSeconds;
            int durationInFrames = (int)(durationInSeconds * 60D);
            Duration = durationInFrames;
        }
    }
}
