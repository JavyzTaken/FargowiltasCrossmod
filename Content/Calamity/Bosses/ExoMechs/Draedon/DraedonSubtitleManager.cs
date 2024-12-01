using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    public class DraedonSubtitleManager : ModSystem
    {
        private static LoopedSoundInstance subtitleLoop;

        // Debug thing. Will be replaced with serious things later.
        public static SubtitlesSequence ShadowTheHedgehogRant
        {
            get;
            private set;
        }

        /// <summary>
        /// The current subtitles sequence that's being played.
        /// </summary>
        ///
        /// <remarks>
        /// Defaults to null, indicating that nothing is being played.
        /// </remarks>
        public static SubtitlesSequence? CurrentSequence
        {
            get;
            private set;
        }

        /// <summary>
        /// The general purpose timer used to determine how far along the <see cref="CurrentSequence"/> is.
        /// </summary>
        ///
        /// <remarks>
        /// Once this elapses the <see cref="SubtitlesSequence.Duration"/> of <see cref="CurrentSequence"/> it is zeroed out.
        /// </remarks>
        public static int SequenceTimer
        {
            get;
            private set;
        }

        /// <summary>
        /// A simple shorthand that calculates a timeframe, in frames, from minutes, seconds, and milliseconds.
        /// </summary>
        /// <param name="minutes">The amount of minutes.</param>
        /// <param name="seconds">The amount of seconds.</param>
        /// <param name="milliseconds">The amount of milliseconds.</param>
        public static int FromTimeFrame(int minutes, int seconds, int milliseconds) =>
            minutes * 3600 + seconds * 60 + (int)(milliseconds * 0.06f);

        /// <summary>
        /// The color of Draedon's text in the subtitles.
        /// </summary>
        public static readonly Color SubtitleColor = new(155, 255, 255);

        public override void OnModLoad()
        {
            SubtitleSection[] sections =
            [
                new(FromTimeFrame(00, 00, 000), "I've come to make an announcement."),
                new(FromTimeFrame(00, 03, 391), "You are a bitch-ass motherfucker!"),
                new(FromTimeFrame(00, 06, 610), "You pissed on my fucking machines!"),
                new(FromTimeFrame(00, 10, 601), "That's right. You took your tiny fucking dick out and you pissed on my fucking machines!"),
                new(FromTimeFrame(00, 18, 928), "And I say, that's disgusting!"),
                new(FromTimeFrame(00, 21, 975), "So I'm making a callout post on my twitter.com."),
                new(FromTimeFrame(00, 26, 825), "You've got a small dick!"),
                new(FromTimeFrame(00, 29, 830), "It's the size of this exo prism except way smaller!"),
                new(FromTimeFrame(00, 34, 637), "And guess what?"),
                new(FromTimeFrame(00, 37, 470), "Here's what my dong looks like!"),
                new(FromTimeFrame(00, 41, 805), "All points, no flesh, no blood."),
                new(FromTimeFrame(00, 46, 054), "Look at it. It looks like two balls and a bong!"),
                new(FromTimeFrame(00, 51, 548), "You fucked my machines, so guess what?"),
                new(FromTimeFrame(00, 55, 883), "I'm going to fuck Terraria!"),
                new(FromTimeFrame(01, 00, 261), "That's right! This is what you get! My super exo piss!"),
                new(FromTimeFrame(01, 08, 544), "Except I'm not going to piss on Terraria. I'm aiming higher!"),
                new(FromTimeFrame(01, 14, 725), "I'm pissing on the twisted garden!"),
                new(FromTimeFrame(01, 18, 201), "How do you like that, Terrarian?!"),
                new(FromTimeFrame(01, 21, 764), "I pissed on the twisted garden, you fucking idiot!"),
                new(FromTimeFrame(01, 26, 142), "You have 23 hours before the piss DROPLETS hit the fucking world!"),
                new(FromTimeFrame(01, 33, 524), "Now get out of my fucking sight before I piss on you too!")
            ];
            ShadowTheHedgehogRant = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/DraedonDialogue/ShadowTheHedgehogRant", FromTimeFrame(1, 40, 606), sections);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (CurrentSequence is null)
            {
                SequenceTimer = 0;
                subtitleLoop?.Stop();
                subtitleLoop = null;

                return;
            }

            if (!subtitleLoop.LoopIsBeingPlayed && subtitleLoop.HasLoopSoundBeenStarted)
            {
                CurrentSequence = null;
                return;
            }

            subtitleLoop?.Update(Main.screenPosition + Main.ScreenSize.ToVector2() * 0.5f);

            SequenceTimer++;
            if (SequenceTimer >= CurrentSequence.Duration)
                CurrentSequence = null;
        }

        /// <summary>
        /// Starts a given subtitles sequence.
        /// </summary>
        /// <param name="sequence">The subtitles sequence to start.</param>
        public static void Start(SubtitlesSequence sequence)
        {
            CurrentSequence = sequence;
            SequenceTimer = 0;
            subtitleLoop = LoopedSoundManager.CreateNew(new SoundStyle(sequence.SoundPath), () => CurrentSequence is null);
        }
    }
}
