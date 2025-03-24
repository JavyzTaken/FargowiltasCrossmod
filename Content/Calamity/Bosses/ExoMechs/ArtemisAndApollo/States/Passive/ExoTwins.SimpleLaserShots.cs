using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public static partial class ExoTwinsStates
    {
        /// <summary>
        /// The rate at which Artemis shoots lasers forward during the SimpleLaserShots attack.
        /// </summary>
        public static int SimpleLaserShots_ArtemisShootRate => Variables.GetAIInt("SimpleLaserShots_ArtemisShootRate", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The speed at which lasers fired by Artemis during the SimpleLaserShots attack should be.
        /// </summary>
        public static float SimpleLaserShots_ArtemisShootSpeed => Variables.GetAIFloat("SimpleLaserShots_ArtemisShootSpeed", ExoMechAIVariableType.Twins);

        /// <summary>
        /// AI update loop method for the SimpleLaserShots attack.
        /// </summary>
        /// <param name="npc">Artemis' NPC instance.</param>
        /// <param name="artemisAttributes">Artemis' designated generic attributes.</param>
        /// <param name="localAITimer">Artemis' local AI timer.</param>
        public static void DoBehavior_SimpleLaserShots(NPC npc, IExoTwin artemisAttributes, ref int localAITimer)
        {
            Vector2 hoverOffsetDirection = Target.SafeDirectionTo(npc.Center) * new Vector2(1f, 0.95f);
            Vector2 hoverDestination = Target.Center + hoverOffsetDirection * new Vector2(550f, 450f);
            npc.SmoothFlyNearWithSlowdownRadius(hoverDestination, 0.061f, 0.91f, 30f);

            if (Collision.SolidCollision(npc.TopLeft, npc.width, npc.height))
                npc.velocity.Y -= 0.7f;

            npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.23f);

            artemisAttributes.Animation = ExoTwinAnimation.Attacking;
            artemisAttributes.Frame = artemisAttributes.Animation.CalculateFrame(localAITimer / (float)SimpleLaserShots_ArtemisShootRate * 0.5f % 1f, artemisAttributes.InPhase2);

            // Make Artemis fire her lasers. Lasers are not fired if she's close to the player.
            // This would normally be a baffling design decision, since it would result in the player simply invalidating attacks by getting near the boss.
            // However, given that this is a passive attack, this comes with the interesting tradeoff of being able to negate the passive aspect if they divert their attention
            // from Apollo (who will be doing an active attack) a bit in favor of trying to maintain distance to Artemis. This also naturally lends itself to kiting movement as the
            // player attempts to stay near Artemis while she tries to fly away.
            if (localAITimer % SimpleLaserShots_ArtemisShootRate == SimpleLaserShots_ArtemisShootRate - 1 && !npc.WithinRange(Target.Center, 300f))
            {
                ShootArtemisLaser(npc, SimpleLaserShots_ArtemisShootSpeed);
                npc.velocity -= npc.rotation.ToRotationVector2() * 7.4f;
                npc.netUpdate = true;
            }
        }
    }
}
