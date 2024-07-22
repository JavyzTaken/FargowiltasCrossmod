using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    /// <summary>
    /// Represents a sequence of animation frames that the Exo Twins can perform.
    /// </summary>
    /// 
    /// <remarks>
    /// Since Artemis and Apollo have the same sheet layout, this type can be used for both of them without issue.
    /// </remarks>
    /// <param name="StartingFrame">The starting frame of the animation.</param>
    /// <param name="EndingFrame">The ending frame of the animation</param>
    public record ExoTwinAnimation(int StartingFrame, int EndingFrame)
    {
        /// <summary>
        /// The amount of frames that exist between phase 1 and 2. Incrementing a phase 1 frame index by this number will, with the exception of the phase 2 enter animation, return the equivalent frame in phase 2.
        /// </summary>
        public const int Phase2FrameOffset = 60;

        /// <summary>
        /// The frame that should be used when a lens pops off during the <see cref="EnteringSecondPhase"/> animation.
        /// </summary>
        public const int LensPopOffFrame = 37;

        /// <summary>
        /// The animation used when an Exo Twin is moving idly.
        /// </summary>
        public static readonly ExoTwinAnimation Idle = new(0, 9);

        /// <summary>
        /// The animation used when an Exo Twin is charging up in anticipation of an attack.
        /// </summary>
        public static readonly ExoTwinAnimation ChargingUp = new(10, 19);

        /// <summary>
        /// The animation used when an Exo Twin is attacking.
        /// </summary>
        public static readonly ExoTwinAnimation Attacking = new(20, 29);

        /// <summary>
        /// The animation used when an Exo Twin is entering its second phase.
        /// </summary>
        public static readonly ExoTwinAnimation EnteringSecondPhase = new(30, 59);

        /// <summary>
        /// Calculates the current frame of an Exo Twin based on a 0-1 animation interpolant.
        /// </summary>
        /// <param name="animationCompletion">The 0-1 animation completion value.</param>
        /// <param name="phase2">Whether the result should be offset by <see cref="Phase2FrameOffset"/> in order to use phase 2 frames.</param>
        public int CalculateFrame(float animationCompletion, bool phase2)
        {
            int frame = (int)MathF.Round(MathHelper.Lerp(StartingFrame, EndingFrame, Utilities.Saturate(animationCompletion)));
            if (phase2)
                frame += Phase2FrameOffset;

            return frame;
        }
    }
}
