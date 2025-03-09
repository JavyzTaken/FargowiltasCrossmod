using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    public sealed partial class DraedonEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// Whether there are any Exo Mechs in the process of dying.
        /// </summary>
        public static bool AnyDyingExoMechs
        {
            get
            {
                foreach (CalDLCEmodeBehavior behavior in ExoMechFightStateManager.ActiveManagingExoMechs)
                {
                    if (behavior.NPC.life <= 1)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// The AI method that makes Draedon move around idly during the Exo Mechs battle.
        /// </summary>
        public void DoBehavior_MoveAroundDuringBattle()
        {
            Vector2 hoverDestination = PlayerToFollow.Center + PlayerToFollow.SafeDirectionTo(NPC.Center) * new Vector2(820f, 560f);
            NPC.SmoothFlyNearWithSlowdownRadius(hoverDestination, 0.06f, 0.9f, 60f);

            if (AnyDyingExoMechs)
                ChangeAIState(DraedonAIState.TemporarilyLeave);

            PerformStandardFraming();
        }
    }
}
