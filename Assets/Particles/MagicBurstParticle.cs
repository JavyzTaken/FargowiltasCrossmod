using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasCrossmod.Assets.Particles
{
    public class MagicBurstParticle : Particle
    {
        public float ScaleExpandRate;

        public override BlendState BlendState => BlendState.Additive;

        public override string AtlasTextureName => "FargowiltasCrossmod.MagicBurstParticle.png";

        public MagicBurstParticle(Vector2 position, Vector2 velocity, Color color, int lifetime, float scale, float scaleExpandRate = 0f)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Scale = Vector2.One * scale;
            Lifetime = lifetime;
            ScaleExpandRate = scaleExpandRate;
        }

        public override void Update()
        {
            Opacity = Utilities.InverseLerp(0f, 4f, Lifetime - Time);
            Scale += Vector2.One * ScaleExpandRate;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle frame = Subdivide(Texture.Frame, 1, 5, 0, (int)(LifetimeRatio * 4.999f));
            spriteBatch.Draw(Texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, null, Scale * 0.8f, 0);
        }

        public static Rectangle Subdivide(Rectangle rectangle, int horizontalFrames, int verticalFrames, int frameX, int frameY)
        {
            int width = rectangle.Width / horizontalFrames;
            int height = rectangle.Height / verticalFrames;
            return new Rectangle(rectangle.Left + width * frameX, rectangle.Top + height * frameY, width, height);
        }
    }
}
