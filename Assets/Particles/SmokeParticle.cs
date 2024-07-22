using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace FargowiltasCrossmod.Assets.Particles
{
    public class SmokeParticle : Particle
    {
        public int Variant;

        public float ScaleGrowRate;

        public float RotationOffset;

        public override int FrameCount => 8;

        public override string AtlasTextureName => "FargowiltasCrossmod.SmokeParticle.png";

        public override BlendState BlendState => BlendState.NonPremultiplied;

        public SmokeParticle(Vector2 position, Vector2 velocity, Color color, int lifetime, float scale, float scaleGrowRate)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Scale = Vector2.One * scale;
            Variant = Main.rand.Next(FrameCount);
            Lifetime = lifetime;
            ScaleGrowRate = scaleGrowRate;
            RotationOffset = -MathHelper.PiOver2 + Main.rand.NextFloatDirection() * 0.51f;
        }

        public override void Update()
        {
            Rotation = Velocity.ToRotation() + RotationOffset;
            Velocity *= 0.89f;
            Scale += Vector2.One * LifetimeRatio * ScaleGrowRate;

            int area = (int)(Scale.X * 50f);
            if (Collision.SolidCollision(Position - Vector2.One * area * 0.5f, area, area))
            {
                Time += 9;
                Velocity *= 0.75f;
            }

            DrawColor = Color.Lerp(DrawColor, Color.White, 0.055f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float opacity = Utilities.InverseLerpBump(0f, 0.02f, 0.4f, 1f, LifetimeRatio) * 0.75f;
            int horizontalFrame = (int)MathF.Round(MathHelper.Lerp(0f, 2f, LifetimeRatio));
            int width = 28;
            int height = 28;
            Rectangle frame = new(width * horizontalFrame, height * Variant, width, height);

            Vector2 origin = Vector2.Zero;
            float x = MathHelper.Clamp(frame.X + Texture.Frame.X, Texture.Frame.X, Texture.Frame.X + Texture.Frame.Width - width);
            float y = MathHelper.Clamp(frame.Y + Texture.Frame.Y, Texture.Frame.Y, Texture.Frame.Y + Texture.Frame.Height - height);
            Rectangle frameOnAtlas = new((int)x, (int)y, (int)width, (int)height);

            spriteBatch.Draw(Texture.Atlas.Texture.Value, Position - Main.screenPosition, frameOnAtlas, DrawColor * opacity, Rotation, origin, Scale * 2f, 0, 0f);
        }
    }
}
