using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    public sealed partial class DraedonEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The AI method that makes Draedon temporarily leave the Exo Mechs battle.
        /// </summary>
        public void DoBehavior_TemporarilyLeave()
        {
            HologramOverlayInterpolant = LumUtils.Saturate(HologramOverlayInterpolant + AnyDyingExoMechs.ToDirectionInt() * 0.02f);
            if (!AnyDyingExoMechs && HologramOverlayInterpolant <= 0f)
                ChangeAIState(DraedonAIState.MoveAroundDuringBattle);

            if (HologramOverlayInterpolant >= 1f)
                NPC.SmoothFlyNearWithSlowdownRadius(PlayerToFollow.Center - Vector2.UnitY * 540f, 0.075f, 0.9f, 300f);
            else
                NPC.velocity *= 0.7f;

            PerformStandardFraming();
        }
    }
}
