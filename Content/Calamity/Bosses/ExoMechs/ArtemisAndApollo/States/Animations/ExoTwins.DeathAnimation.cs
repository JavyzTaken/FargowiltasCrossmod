using CalamityMod;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public static partial class ExoTwinsStates
    {
        /// <summary>
        /// The spin angle of Artemis and Apollo during the death animation.
        /// </summary>
        public static ref float DeathAnimation_SpinAngle => ref SharedState.Values[0];

        /// <summary>
        /// AI update loop method for the death animation of the Exo Twins.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        public static void DoBehavior_DeathAnimation(NPC npc, IExoTwin twinAttributes)
        {
            npc.dontTakeDamage = true;
            npc.Calamity().ShouldCloseHPBar = true;

            if (Main.mouseRight && Main.mouseRightRelease)
                AITimer = 0;

            float hoverOffsetRadius = 540f;
            float hoverSpeedInterpolant = LumUtils.InverseLerp(0f, 120f, AITimer);

            float spinSpeed = MathHelper.Clamp(0f, MathHelper.ToRadians(10f), AITimer * 0.0002f);
            DeathAnimation_SpinAngle += spinSpeed;
            hoverOffsetRadius -= spinSpeed * 1100f;

            Vector2 hoverOffset = DeathAnimation_SpinAngle.ToRotationVector2() * hoverOffsetRadius;
            if (npc.type == ExoMechNPCIDs.ArtemisID)
                hoverOffset *= -1f;

            npc.SmoothFlyNear(Target.Center + hoverOffset, hoverSpeedInterpolant * 0.4f, 1f - hoverSpeedInterpolant * 0.5f);
            npc.rotation = npc.AngleTo(Target.Center);

            twinAttributes.Animation = ExoTwinAnimation.Idle;
            twinAttributes.Frame = twinAttributes.Animation.CalculateFrame(AITimer / 50f % 1f, twinAttributes.InPhase2);
        }
    }
}
