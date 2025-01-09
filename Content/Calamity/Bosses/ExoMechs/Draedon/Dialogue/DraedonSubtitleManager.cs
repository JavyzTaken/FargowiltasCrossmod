using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue
{
    // TODO -- Add weak ref attributes to the various subtitle system classes.
    public class DraedonSubtitleManager : ModSystem
    {
        private static LoopedSoundInstance subtitleLoop;

        internal static int SequenceTimer;

        /// <summary>
        /// The current dialogue instance that's being played.
        /// </summary>
        ///
        /// <remarks>
        /// Defaults to null, indicating that nothing is being played.
        /// </remarks>
        public static DraedonDialogue? CurrentSequence
        {
            get;
            private set;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (!DraedonDialogueManager.UseSubtitles || !NPC.AnyNPCs(ModContent.NPCType<CalamityMod.NPCs.ExoMechs.Draedon>()))
            {
                Stop();
                return;
            }

            if (subtitleLoop is not null && SoundEngine.TryGetActiveSound(subtitleLoop.LoopingSoundSlot, out ActiveSound? sound))
            {
                if (Main.gamePaused && sound.IsPlaying)
                    sound.Pause();
                if (!Main.gamePaused && !sound.IsPlaying)
                    sound.Resume();
            }

            if (CurrentSequence is null)
            {
                Stop();
                return;
            }

            subtitleLoop?.Update(Main.screenPosition + Main.ScreenSize.ToVector2() * 0.5f);
        }

        /// <summary>
        /// Stops the current sequence, resetting everything.
        /// </summary>
        public static void Stop()
        {
            CurrentSequence = null;
            SequenceTimer = 0;
            subtitleLoop?.Stop();
            subtitleLoop = null;
        }

        /// <summary>
        /// Prepares a given subtitle for playing and display.
        /// </summary>
        /// <param name="time">The relative play timer for the subtitles.</param>
        /// <param name="subtitle">The subtitles to display.</param>
        public static void Play(int time, DraedonDialogue subtitle)
        {
            if (time < 0)
                return;

            SequenceTimer = time;
            if (CurrentSequence != subtitle)
            {
                CurrentSequence = subtitle;
                subtitleLoop?.Stop();
                subtitleLoop = LoopedSoundManager.CreateNew(new SoundStyle(subtitle.Subtitle.SoundPath) with { Volume = 2f }, () => CurrentSequence is null);
                subtitleLoop.Update(Main.screenPosition + Main.ScreenSize.ToVector2() * 0.5f, s => s.Volume = 1.8f);
            }
        }
    }
}
