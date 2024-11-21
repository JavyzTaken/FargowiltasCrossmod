using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasCrossmod.Assets.Particles
{
    public class ElectricSparkParticle : Particle
    {
        /// <summary>
        /// The color of bloom behind the pixel.
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

        public override int FrameCount => 4;

        public override string AtlasTextureName => "FargowiltasCrossmod.ElectricSparkParticle.png";

        public override BlendState BlendState => BlendState.Additive;

        public ElectricSparkParticle(Vector2 position, Vector2 velocity, Color color, Color bloomColor, int lifetime, Vector2 scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            BloomColor = bloomColor;
            Scale = scale;
            Lifetime = lifetime;
            Frame = new(0, Main.rand.Next(FrameCount) * 125, 125, 125);
            Opacity = 1f;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void Update()
        {
            if (Time >= Lifetime - 5)
                Opacity *= 0.84f;

            Velocity *= 0.89f;
            Rotation += Velocity.X * 0.041f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            BloomTexture ??= AtlasManager.GetTexture("FargowiltasCrossmod.BasicMetaballCircle.png");
            spriteBatch.Draw(BloomTexture, Position - Main.screenPosition, null, BloomColor * Opacity, 0f, null, Scale * 3f, 0);
            base.Draw(spriteBatch);
        }
    }
}
