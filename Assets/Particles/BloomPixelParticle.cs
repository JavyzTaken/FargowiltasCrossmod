using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasCrossmod.Assets.Particles
{
    public class BloomPixelParticle : Particle
    {
        /// <summary>
        /// The color of bloom behind the pixel.
        /// </summary>
        public Color BloomColor;

        /// <summary>
        /// The scale factor of the back-bloom.
        /// </summary>
        public Vector2 BloomScaleFactor;

        /// <summary>
        /// The optional position that this pixel should try to home towards.
        /// </summary>
        public Vector2? HomeInDestination;

        /// <summary>
        /// The bloom texture.
        /// </summary>
        public static AtlasTexture BloomTexture
        {
            get;
            private set;
        }

        public override string AtlasTextureName => "FargowiltasCrossmod.Pixel.png";

        public override BlendState BlendState => BlendState.Additive;

        public BloomPixelParticle(Vector2 position, Vector2 velocity, Color color, Color bloomColor, int lifetime, Vector2 scale, Vector2? homeInDestination = null, Vector2? bloomScaleFactor = null)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            BloomColor = bloomColor;
            Scale = scale;
            Lifetime = lifetime;
            Opacity = 1f;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            HomeInDestination = homeInDestination;
            BloomScaleFactor = bloomScaleFactor ?? Vector2.One * 0.19f;
        }

        public override void Update()
        {
            if (Time >= Lifetime * 0.6f)
            {
                Opacity *= 0.91f;
                Scale *= 0.96f;
                Velocity *= 0.94f;
            }

            if (HomeInDestination is null)
                Velocity *= 0.96f;
            else
            {
                float currentDirection = Velocity.ToRotation();
                float idealDirection = (HomeInDestination.Value - Position).ToRotation();
                Velocity = currentDirection.AngleLerp(idealDirection, 0.03f).ToRotationVector2() * Velocity.Length();
                Velocity += (HomeInDestination.Value - Position) * 0.005f;

                if (Position.WithinRange(HomeInDestination.Value, 10f))
                    Kill();
            }

            Rotation += Velocity.X * 0.07f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            BloomTexture ??= AtlasManager.GetTexture("FargowiltasCrossmod.BasicMetaballCircle.png");
            spriteBatch.Draw(BloomTexture, Position - Main.screenPosition, null, BloomColor * Opacity, Rotation, null, Scale * BloomScaleFactor, 0);
            base.Draw(spriteBatch);
        }
    }
}
