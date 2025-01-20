using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace FargowiltasCrossmod.Core.Common.InverseKinematics
{
    public class Joint(float length, Joint? previousJoint = null, params IJointConstraint[] constraints)
    {
        /// <summary>
        /// The joint behind this one.
        /// </summary>
        protected internal Joint? previousJoint = previousJoint;

        /// <summary>
        /// This joint's rotation.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// The amount of penalty applied to the loss as a result of this joint's constraints.
        /// </summary>
        public double ConstraintPenalties(float gradientDescentCompletion) => Constraints.Sum(c => c.ApplyPenaltyLoss(this, gradientDescentCompletion));

        /// <summary>
        /// The offset of this joint from its starting point, either being the previous joint or the starting point of the kinematic chain.
        /// </summary>
        public Vector2 Offset => Rotation.ToRotationVector2() * Length;

        /// <summary>
        /// This joint's update constraints.
        /// </summary>
        public List<IJointConstraint> Constraints = constraints.ToList();

        /// <summary>
        /// This joint's length.
        /// </summary>
        public readonly float Length = length;
    }
}
