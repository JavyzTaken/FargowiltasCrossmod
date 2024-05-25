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
        }

        public override void Update()
        {
            Rotation = Velocity.ToRotation() - MathHelper.PiOver2;
            Velocity *= 0.89f;
            Scale += Vector2.One * LifetimeRatio * ScaleGrowRate;

            DrawColor = Color.Lerp(DrawColor, Color.White, 0.055f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float opacity = Utilities.InverseLerpBump(0f, 0.02f, 0.4f, 1f, LifetimeRatio) * 0.75f;
            int horizontalFrame = (int)MathF.Round(MathHelper.Lerp(0f, 2f, LifetimeRatio));
            int width = 28;
            int height = 28;
            Rectangle frame = new(width * horizontalFrame, height * Variant, width, height);
            spriteBatch.Draw(Texture, Position - Main.screenPosition, frame, DrawColor * opacity, Rotation, null, Scale * 2f, 0);
        }
    }
}
