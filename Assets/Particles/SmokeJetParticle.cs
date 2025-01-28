using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace FargowiltasCrossmod.Assets.Particles
{
    public class SmokeJetParticle : Particle
    {
        public Func<Vector2> AttachPointFunction;

        public override int FrameCount => 6;

        public override string AtlasTextureName => "FargowiltasCrossmod.SmokeJet.png";

        public override BlendState BlendState => BlendState.Additive;

        public SmokeJetParticle(Vector2 position, Vector2 velocity, Func<Vector2> attachPointFunction, float rotation, Color color, int lifetime, float scale)
        {
            Position = position;
            Velocity = velocity;
            Rotation = rotation;
            DrawColor = color;
            Scale = Vector2.One * scale;
            Lifetime = lifetime;
            AttachPointFunction = attachPointFunction;
        }

        public override void Update()
        {
            Scale *= 1.01f;

            int area = (int)(Scale.X * 50f);
            if (Collision.SolidCollision(Position - Vector2.One * area * 0.5f, area, area))
            {
                Time += 9;
                Velocity *= 0.75f;
            }

            DrawColor = Color.Lerp(DrawColor, Color.White * 0.6f, 0.07f);
            Opacity *= 0.95f;
            Position = AttachPointFunction() + Velocity * Time;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int overallFrame = (int)MathF.Round(MathHelper.Lerp(0f, 35f, LifetimeRatio));
            int width = 340;
            int height = 340;
            Rectangle frame = new(width * (overallFrame % 6), height * (overallFrame / 6), width, height);

            float x = MathHelper.Clamp(frame.X + Texture.Frame.X, Texture.Frame.X, Texture.Frame.X + Texture.Frame.Width - width);
            float y = MathHelper.Clamp(frame.Y + Texture.Frame.Y, Texture.Frame.Y, Texture.Frame.Y + Texture.Frame.Height - height);
            Rectangle frameOnAtlas = new((int)x, (int)y, width, height);
            Vector2 origin = frameOnAtlas.Size() * new Vector2(0f, 0.5f);

            SpriteEffects direction = MathF.Cos(Rotation) < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;

            spriteBatch.Draw(Texture.Atlas.Texture.Value, Position - Main.screenPosition, frameOnAtlas, DrawColor * Opacity, Rotation, origin, Scale * 2f, direction, 0f);
        }
    }
}
