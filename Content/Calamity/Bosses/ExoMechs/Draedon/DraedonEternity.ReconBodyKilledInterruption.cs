using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Easings;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    public sealed partial class DraedonEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The vertical offset of Draedon's projector.
        /// </summary>
        public float ProjectorVerticalOffset
        {
            get;
            set;
        } = MaxProjectorVerticalOffset;

        /// <summary>
        /// The opacity of Draedon's hologram.
        /// </summary>
        public float HologramOpacity
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum vertical offset of Draedon's projector.
        /// </summary>
        public static float MaxProjectorVerticalOffset => 640f;

        /// <summary>
        /// The monologue that Draedon uses upon the Exo Mechs battle concluding after being harmed.
        /// </summary>
        public static readonly DraedonDialogueChain PostBattleDeathInterjection = new DraedonDialogueChain().
            Add("EndOfBattle_FirstDefeatReconBodyKill1").
            Add("EndOfBattle_FirstDefeatReconBodyKill2").
            Add("EndOfBattle_FirstDefeatReconBodyKill3");

        /// <summary>
        /// The AI method that makes Draedon speak to the player after an Exo Mech has been defeated.
        /// </summary>
        public void DoBehavior_ReconBodyKilledInterruption()
        {
            ProjectorVerticalOffset = EasingCurves.Quartic.Evaluate(EasingType.InOut, LumUtils.InverseLerp(90f, 0f, AITimer)) * MaxProjectorVerticalOffset;
            HologramOpacity = LumUtils.InverseLerp(90f, 142f, AITimer) * 0.7f;

            Vector2 hoverDestination = PlayerToFollow.Center + new Vector2((PlayerToFollow.Center.X - NPC.Center.X).NonZeroSign() * -450f, -5f);
            NPC.SmoothFlyNear(hoverDestination, 0.05f, 0.94f);

            int speakTimer = (int)AITimer - 150;
            PostBattleDeathInterjection.Process(speakTimer);

            if (PostBattleDeathInterjection.Finished(speakTimer))
            {
                AIState = DraedonAIState.PostBattleInterjection;
                AITimer = PostBattleInterjectionTimer;
                NPC.netUpdate = true;
            }
        }
    }
}
