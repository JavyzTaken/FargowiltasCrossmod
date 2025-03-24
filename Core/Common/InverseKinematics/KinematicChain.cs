using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace FargowiltasCrossmod.Core.Common.InverseKinematics
{
    public class KinematicChain
    {
        /// <summary>
        /// The starting point of this kinematic chain.
        /// </summary>
        public Vector2 StartingPoint;

        /// <summary>
        /// The position of the end effector of this kinematic chain.
        /// </summary>
        public Vector2 EndEffectorPosition
        {
            get;
            private set;
        }

        /// <summary>
        /// The set of all joints that compose this kinematic chain.
        /// </summary>
        private readonly List<Joint> joints;

        /// <summary>
        /// The amount of joints this kinematic chain is composed of.
        /// </summary>
        public int JointCount
        {
            get;
            private set;
        }

        /// <summary>
        /// The nudge factor when updating rotations for gradient descent.
        /// </summary>
        public const float UpdateStep = 0.02f;

        /// <summary>
        /// The offset by which inputs are nudged when performing derivatives.
        /// </summary>
        /// <remarks>
        /// The cube root of machine epsilon is optimal for central-difference methods. In this case, for 32-bit floats, the machine epsilon can be calculated as 2^-23.
        /// </remarks>
        public const float HalfDerivativeOffset = 0.00034526f;

        /// <summary>
        /// Half of the inverse of <see cref="HalfDerivativeOffset"/>.
        /// </summary>
        public const float InverseDerivativeOffset = 1448.1546f;

        public KinematicChain(params float[] jointLengths)
        {
            JointCount = jointLengths.Length;
            joints = [];
            for (int i = 0; i < JointCount; i++)
                joints.Add(new(jointLengths[i], i == 0 ? null : joints.Last()));
        }

        /// <summary>
        /// Adds a new joint to the chain.
        /// </summary>
        /// <param name="joint">The joint to add.</param>
        public void Add(Joint joint)
        {
            joints.Add(joint);
            JointCount = joints.Count;

            // Reconnect joints.
            joints[0].previousJoint = joints[0];
            for (int i = 1; i < JointCount; i++)
                joints[i].previousJoint = joints[i - 1].previousJoint;
        }

        /// <summary>
        /// Updates the position of the end effector by performing forward-kinematics across all joints.
        /// </summary>
        public void UpdateEndEffector()
        {
            // Initialize the end effector position at the starting point.
            EndEffectorPosition = StartingPoint;

            // Propagate through the joint offsets, adding onto the end effector position until it's at the position of the final joint.
            for (int i = 0; i < JointCount; i++)
                EndEffectorPosition += joints[i].Offset;
        }

        /// <summary>
        /// The loss function to use when calculating the gradient during update steps. Higher outputs correspond to a greater degree of incorrect-ness.
        /// </summary>
        /// <param name="idealEndEffectorPosition">The ideal end effector position that should be approached.</param>
        /// <param name="gradientDescentCompletion">How far along the gradient descent iteration steps are.</param>
        private double LossFunction(Vector2 idealEndEffectorPosition, float gradientDescentCompletion) =>
            Math.Pow(EndEffectorPosition.Distance(idealEndEffectorPosition) + (float)joints.Sum(j => j.ConstraintPenalties(gradientDescentCompletion)), 0.1f);

        /// <summary>
        /// Calculates the partial derivative of the <see cref="LossFunction(Vector2)"/> with respect to the <paramref name="inputIndex"/>-th input angle.
        /// </summary>
        /// <param name="idealEndEffectorPosition">The ideal end effector position that should be approached.</param>
        /// <param name="inputIndex">The input to take the partial derivative with respect to.</param>
        /// <param name="gradientDescentCompletion">How far along the gradient descent iteration steps are.</param>
        private double CalculateLossGradient(Vector2 idealEndEffectorPosition, int inputIndex, float gradientDescentCompletion)
        {
            // Cache the original joint configurations.
            Joint joint = joints[inputIndex];
            float originalRotation = joint.Rotation;
            Vector2 originalEndEffectorPosition = EndEffectorPosition;

            // This is kind of inefficient but the exact solution gets incredibly unwieldy as more variables are introduced.

            // Calculate the left side for the symmetric derivative.
            joint.Rotation = originalRotation + HalfDerivativeOffset;
            UpdateEndEffector();
            double lossLeft = LossFunction(idealEndEffectorPosition, gradientDescentCompletion);

            // Calculate the right side for the symmetric derivative.
            joint.Rotation = originalRotation - HalfDerivativeOffset;
            UpdateEndEffector();
            double lossRight = LossFunction(idealEndEffectorPosition, gradientDescentCompletion);

            // Reset the joint back to their original values.
            joint.Rotation = originalRotation;
            EndEffectorPosition = originalEndEffectorPosition;

            // Approximate the symmetric derivative by combining the aforementioned loss terms.
            return (lossLeft - lossRight) * InverseDerivativeOffset;
        }

        private static double SoftClamp(double x, double maxAbsoluteValue)
        {
            return Math.Tanh(x / maxAbsoluteValue) * maxAbsoluteValue;
        }

        /// <summary>
        /// Updates the joint rotations in an attempt to reach the <paramref name="idealEndEffectorPosition"/>.
        /// </summary>
        /// <param name="idealEndEffectorPosition">The ideal end effector position that should be approached.</param>
        public void Update(Vector2 idealEndEffectorPosition)
        {
            // Since the end effector is a result of repeated vector additions across angle -> vector equations, the configuration of outputs is as follows:
            // u_x(a, b, c, ...) = startingX + cos(a) + cos(b) + cos(c) + ...
            // u_y(a, b, c, ...) = startingY + sin(a) + sin(b) + sin(c) + ...

            // Rewriting this to transform the equation into one of loss rather than position, where values are minimized when the end effector is at the ideal position, can be achieved
            // via the following manipulations:
            // L(a, b, c, ...) = distance(startingPoint + (u_x(a, b, c, ...), u_y(a, b, c, ...)) - idealPosition)

            // Lastly, external penalties can be applied to the loss function to discourage certain behaviors, such as overlapping joint angles.
            // Adding this is simply a matter of adding an extra term to the loss function like so:
            // L(a, b, c, ...) = distance(startingPoint + (u_x(a, b, c, ...), u_y(a, b, c, ...)) - idealPosition) + P(a, b, c, ...)

            // From these equations we can repeatedly tweak the input parameters via gradient descent.
            // This will yield an approximate solution that minimizes the possible distance between the end effector and the ideal end position.
            // The iteration process is the following:
            // x_(n+1) = x_n - alpha * ∇L(x_n)
            // Where x_n represents a timestep of the angles in the form of a vector.

            // Cache the gradient in an array outside of the loop.
            float[] gradient = new float[JointCount];

            int iterations = 110;
            for (int i = 0; i < iterations; i++)
            {
                float iterProgress = (float)i / iterations;
                float relaxationFactor = MathF.Sqrt(1f - iterProgress);

                // Initialize the gradient vector based on the aforementioned definitions.
                for (int j = 0; j < JointCount; j++)
                {
                    // Calculate the partial derivative and cache it in the gradient.
                    // For the sake of numerical stability, the gradient values are clamped.
                    // This practice is common in the context of machine learning tasks where exploding gradients are a concern.
                    gradient[j] = (float)SoftClamp(CalculateLossGradient(idealEndEffectorPosition, j, iterProgress), 0.17);
                }

                // Apply the iteration step.
                // No, you cannot do this in the above loop. Doing so will throw off gradient calculations for the second iteration and onward, since different rotations mean different loss values.
                for (int j = 0; j < JointCount; j++)
                    joints[j].Rotation = LumUtils.WrapAngle360(joints[j].Rotation - gradient[j] * relaxationFactor * 0.12f);

                // Update the end effector position, now that the joint configurations have been updated.
                UpdateEndEffector();
            }
        }

        public Joint this[int index]
        {
            get => joints[index];
        }
    }
}
