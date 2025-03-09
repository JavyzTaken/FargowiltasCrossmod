using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    public sealed partial class DraedonEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// How long Draedon spends appearing as a hologram.
        /// </summary>
        public static int HologramAppearTime => Utilities.SecondsToFrames(1f);

        /// <summary>
        /// The AI method that makes Draedon appear as a hologram in front of the player.
        /// </summary>
        public void DoBehavior_AppearAsHologram()
        {
            if (AITimer == 1f)
                SoundEngine.PlaySound(CalamityMod.NPCs.ExoMechs.Draedon.TeleportSound, PlayerToFollow.Center);

            HologramOverlayInterpolant = Utilities.InverseLerp(HologramAppearTime, 0f, AITimer);

            if (AITimer >= HologramAppearTime)
                ChangeAIState(DraedonAIState.StartingMonologue);
        }
    }
}
