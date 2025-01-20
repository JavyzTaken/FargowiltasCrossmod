using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace FargowiltasCrossmod.Core.Common.InverseKinematics
{
    public class UpwardOnlyConstraint : IJointConstraint
    {
        private readonly Func<Vector2> upDirectionGetter;

        public UpwardOnlyConstraint(Func<Vector2>? upDirectionGetter = null)
        {
            upDirectionGetter ??= () => -Vector2.UnitY;
            this.upDirectionGetter = upDirectionGetter;
        }

        public double ApplyPenaltyLoss(Joint owner, float gradientDescentCompletion)
        {
            Vector2 jointDirection = owner.Offset.SafeNormalize(Vector2.Zero);
            float dot = MathHelper.Clamp(Vector2.Dot(jointDirection, upDirectionGetter().SafeNormalize(Vector2.Zero)), -1f, 1f);
            float angleOffset = MathF.Acos(dot);
            float penalty = MathF.Pow(angleOffset * 17.5f + 1e-7f, 0.95f);
            return penalty;
        }
    }
}
