using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasCrossmod.Assets.Particles
{
    public class NuclearGasParticle : Particle
    {
        public override BlendState BlendState => BlendState.AlphaBlend;

        public override string AtlasTextureName => "FargowiltasCrossmod.SmallSmokeParticle.png";

        public NuclearGasParticle(Vector2 position, Vector2 velocity, Color color, float scale, int lifetime)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Scale = new(scale);
            Lifetime = lifetime;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void Update()
        {
            Scale += Vector2.One * LumUtils.InverseLerp(0.25f, 1f, LifetimeRatio) * 0.04f;

            float opacityFadeOutInterpolant = LumUtils.InverseLerp(0.1f, 1f, LifetimeRatio);
            float opacityFadeOutDecay = MathHelper.SmoothStep(1f, 0.9f, opacityFadeOutInterpolant);
            Opacity *= opacityFadeOutDecay;

            Rotation += Velocity.X * 0.04f;
            Velocity *= 0.84f;
        }
    }
}
