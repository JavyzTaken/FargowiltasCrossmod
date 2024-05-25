using CalamityMod.NPCs.ExoMechs.Artemis;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public static partial class ExoTwinsStates
    {
        /// <summary>
        /// How long Artemis and Apollo spend falling from the sky at rapid velocity during their spawn animation.
        /// </summary>
        public static int SpawnAnimation_FallFromSkyTime => Utilities.SecondsToFrames(0.2f);

        /// <summary>
        /// How long Artemis and Apollo have to wait to look at the player after their spawn animation.
        /// </summary>
        public static int SpawnAnimation_LookAtPlayerDelay => Utilities.SecondsToFrames(1f);

        /// <summary>
        /// How long Artemis and Apollo wait during their spawn animation before attacking.
        /// </summary>
        public static int SpawnAnimation_AttackTransitionDelay => Utilities.SecondsToFrames(2.5f);

        /// <summary>
        /// The speed at which Artemis and Apollo appear from the sky during their spawn animation.
        /// </summary>
        public static float SpawnAnimation_FallFromSkySpeed => 120f;

        /// <summary>
        /// AI update loop method for the spawn animation of the Exo Twins.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        public static void DoBehavior_SpawnAnimation(NPC npc, IExoTwin twinAttributes)
        {
            npc.dontTakeDamage = true;

            if (AITimer == SpawnAnimation_AttackTransitionDelay - 60f)
                SoundEngine.PlaySound(Artemis.AttackSelectionSound);

            if (AITimer >= SpawnAnimation_AttackTransitionDelay)
                ExoTwinsStateManager.TransitionToNextState();

            if (AITimer == 1f)
            {
                npc.velocity = Vector2.UnitY.RotatedByRandom(0.11f) * SpawnAnimation_FallFromSkySpeed;
                npc.netUpdate = true;
            }

            if (AITimer >= SpawnAnimation_FallFromSkyTime)
                npc.velocity *= 0.9f;
            else
                npc.rotation = npc.velocity.ToRotation();
            if (AITimer >= SpawnAnimation_LookAtPlayerDelay)
                npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.18f);

            twinAttributes.Animation = ExoTwinAnimation.Idle;
            twinAttributes.Frame = twinAttributes.Animation.CalculateFrame(AITimer / 50f % 1f, twinAttributes.InPhase2);
        }
    }
}
