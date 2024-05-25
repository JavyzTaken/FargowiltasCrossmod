using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    public sealed partial class DraedonEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The AI method that makes Draedon move around idly during the Exo Mechs battle.
        /// </summary>
        public void DoBehavior_MoveAroundDuringBattle()
        {
            Vector2 hoverDestination = PlayerToFollow.Center + PlayerToFollow.SafeDirectionTo(NPC.Center) * new Vector2(820f, 560f);
            NPC.SmoothFlyNearWithSlowdownRadius(hoverDestination, 0.06f, 0.9f, 60f);

            PerformStandardFraming();
        }
    }
}
