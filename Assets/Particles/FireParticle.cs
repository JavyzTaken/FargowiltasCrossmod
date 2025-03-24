using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace FargowiltasCrossmod.Assets.Particles
{
    public class FireParticle : Particle
    {
        public int Variant;

        /// <summary>
        /// The alternate texture of the particle.
        /// </summary>
        public AtlasTexture AltTexture
        {
            get;
            private set;
        }

        public override int FrameCount => 6;

        public override string AtlasTextureName => "FargowiltasCrossmod.FireA.png";

        public override BlendState BlendState => BlendState.Additive;

        public FireParticle(Vector2 position, Vector2 velocity, Color color, int lifetime, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Scale = new Vector2(0.8f, 1f) * scale;
            Variant = Main.rand.Next(2);
            Lifetime = lifetime;
        }

        public override void Update()
        {
            Rotation = (Velocity.ToRotation() + MathHelper.PiOver2).AngleLerp(MathHelper.Pi, 0.5f);

            if (LifetimeRatio < 0.2f)
                Scale += Vector2.One * 0.01f;
            else
                Scale *= 0.975f;

            Velocity *= 0.9f;
            Opacity = LumUtils.InverseLerp(0f, 0.28f, LifetimeRatio);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            AltTexture ??= AtlasManager.GetTexture("FargowiltasCrossmod.FireB.png");
            AtlasTexture texture = Variant % 2 == 0 ? AltTexture : Texture;

            int overallFrame = (int)MathF.Round(MathHelper.Lerp(0f, 35f, MathF.Pow(LifetimeRatio, 0.4f)));
            int width = 341;
            int height = 341;
            Rectangle frame = new(width * (overallFrame % 6), height * (overallFrame / 6), width, height);

            float x = MathHelper.Clamp(frame.X + texture.Frame.X, texture.Frame.X, texture.Frame.X + texture.Frame.Width - width);
            float y = MathHelper.Clamp(frame.Y + texture.Frame.Y, texture.Frame.Y, texture.Frame.Y + texture.Frame.Height - height);
            Rectangle frameOnAtlas = new((int)x, (int)y, width, height);
            Vector2 origin = frameOnAtlas.Size() * 0.5f;

            spriteBatch.Draw(texture.Atlas.Texture.Value, Position - Main.screenPosition, frameOnAtlas, DrawColor * Opacity, Rotation, origin, Scale, 0, 0f);
        }
    }
}
