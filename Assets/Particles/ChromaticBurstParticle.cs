using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace FargowiltasCrossmod.Assets.Particles
{
    public class ChromaticBurstParticle : Particle
    {
        /// <summary>
        /// The blend mode that should be used for this particle.
        /// </summary>
        public BlendState BlendMode
        {
            get;
            set;
        }

        public override string AtlasTextureName => "FargowiltasCrossmod.ChromaticBurstParticle.png";

        public override BlendState BlendState => BlendMode;

        public ChromaticBurstParticle(Vector2 position, Color color, BlendState blendMode, float scale, int lifetime)
        {
            Position = position;
            DrawColor = color;
            Scale = new(scale);
            Lifetime = lifetime;
            BlendMode = blendMode;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void Update()
        {
            Scale += Vector2.One * (LifetimeRatio.Cubed() * 16f + 0.05f);
        }
    }
}
