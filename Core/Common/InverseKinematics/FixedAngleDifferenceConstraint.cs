using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace FargowiltasCrossmod.Core.Common.InverseKinematics
{
    public class FixedAngleDifferenceConstraint(float maxAngleDifference, float idealAngle) : IJointConstraint
    {
        /// <summary>
        /// The ideal angle to approach.
        /// </summary>
        public float IdealAngle
        {
            get;
            set;
        } = idealAngle;

        /// <summary>
        /// The maximum angular quantity by which angles can exceed.
        /// </summary>
        public readonly float MaxAngleDifference = maxAngleDifference;

        public double ApplyPenaltyLoss(Joint owner, float gradientDescentCompletion)
        {
            if (owner.previousJoint is null)
                return 0f;

            Vector2 jointDirection = owner.Offset.SafeNormalize(Vector2.UnitY);

            // Relax penalties the further into the gradient descent iterations the process is.
            // This allows for initial angles to properly unfold initially, and once they are unfolded, a solution can be reached based on that initial configuration, rather than having
            // angle corrections get in the way of the ultimate goal of having the end effector reach a desired point.
            float penaltyFactor = LumUtils.InverseLerp(0.4f, 0f, gradientDescentCompletion) * 1500f;
            float penalty = LumUtils.InverseLerp(MaxAngleDifference, MathF.PI, jointDirection.AngleBetween(IdealAngle.ToRotationVector2())) * penaltyFactor;

            if (double.IsNaN(penalty))
                return 0f;

            return penalty;
        }
    }
}
