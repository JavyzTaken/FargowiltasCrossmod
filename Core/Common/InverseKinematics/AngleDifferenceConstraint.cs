using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace FargowiltasCrossmod.Core.Common.InverseKinematics
{
    public class AngleDifferenceConstraint(float maxAngleDifference) : IJointConstraint
    {
        /// <summary>
        /// The maximum angular quantity by which angles can exceed.
        /// </summary>
        public readonly float MaxAngleDifference = maxAngleDifference;

        public double ApplyPenaltyLoss(Joint owner, float gradientDescentCompletion)
        {
            if (owner.previousJoint is null)
                return 0f;

            Vector2 jointDirection = owner.Offset.SafeNormalize(Vector2.UnitY);
            Vector2 previousJointDirection = owner.previousJoint.Offset.SafeNormalize(Vector2.UnitY);
            float penalty = LumUtils.InverseLerp(MaxAngleDifference, MathF.PI, jointDirection.AngleBetween(previousJointDirection)) * 9f;

            if (double.IsNaN(penalty))
                return 0f;

            return penalty;
        }
    }
}
