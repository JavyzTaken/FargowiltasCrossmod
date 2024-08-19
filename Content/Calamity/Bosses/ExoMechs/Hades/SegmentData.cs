using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades
{
    public class SegmentData
    {
        /// <summary>
        /// The position of this segment.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The spring force to apply to this segment this frame.
        /// </summary>
        public float SpringForce;

        /// <summary>
        /// The dampening spring offset of this segment.
        /// </summary>
        public float SpringOffset;

        /// <summary>
        /// The rotation of this segment.
        /// </summary>
        public float Rotation;
    }
}
