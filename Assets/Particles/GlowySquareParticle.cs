using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasCrossmod.Assets.Particles
{
    public class GlowySquareParticle : Particle
    {
        /// <summary>
        /// The color of bloom behind the square.
        /// </summary>
        public Color BloomColor;

        /// <summary>
        /// The bloom texture.
        /// </summary>
        public static AtlasTexture BloomTexture
        {
            get;
            private set;
        }

        public override string AtlasTextureName => "FargowiltasCrossmod.GlowSquareParticle.png";

        public override BlendState BlendState => BlendState.Additive;

        public GlowySquareParticle(Vector2 position, Vector2 velocity, Color color, Color bloomColor, int lifetime, Vector2 scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            BloomColor = bloomColor;
            Scale = scale;
            Lifetime = lifetime;
            Opacity = 1f;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void Update()
        {
            if (Time >= Lifetime - 13)
                Opacity *= 0.92f;

            Velocity *= 0.935f;
            Rotation += Velocity.X * 0.091f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scaleFactor = LumUtils.InverseLerp(0f, 10f, Time);
            BloomTexture ??= AtlasManager.GetTexture("FargowiltasCrossmod.GlowSquareParticleBlurred.png");

            spriteBatch.Draw(BloomTexture, Position - Main.screenPosition, null, BloomColor * Opacity, Rotation, null, Scale * scaleFactor, 0);
            spriteBatch.Draw(Texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, null, Scale * scaleFactor, 0);
        }
    }
}
