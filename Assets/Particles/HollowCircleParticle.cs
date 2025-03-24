using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace FargowiltasCrossmod.Assets.Particles
{
    public class HollowCircleParticle : Particle
    {
        /// <summary>
        /// The starting scale of this particle.
        /// </summary>
        public float StartingScale;

        /// <summary>
        /// The ending scale of this particle.
        /// </summary>
        public float EndingScale;

        /// <summary>
        /// The converge power of scaling effects.
        /// </summary>
        public float ScaleConvergePower;

        public override string AtlasTextureName => "FargowiltasCrossmod.HollowCircle.png";

        public override BlendState BlendState => BlendState.Additive;

        public HollowCircleParticle(Vector2 position, Vector2 velocity, Color color, int lifetime, float startingScale, float endingScale, float scaleConvergePower)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            StartingScale = startingScale;
            EndingScale = endingScale;
            Lifetime = lifetime;
            ScaleConvergePower = scaleConvergePower;
        }

        public override void Update()
        {
            Scale = Vector2.One * MathHelper.Lerp(StartingScale, EndingScale, MathF.Pow(LifetimeRatio, ScaleConvergePower));
            Opacity = Utilities.InverseLerp(0f, 0.5f, LifetimeRatio) * 0.7f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position - Main.screenPosition, Frame, DrawColor * Opacity, Rotation, null, Scale, Direction.ToSpriteDirection());
        }
    }
}
