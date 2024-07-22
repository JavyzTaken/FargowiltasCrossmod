using Luminance.Assets;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace FargowiltasCrossmod.Assets.Particles.Metaballs
{
    public class HeatDistortionMetaball : MetaballType
    {
        public override string MetaballAtlasTextureToUse => "FargowiltasCrossmod.BasicMetaballCircle.png";

        public override Color EdgeColor => Color.Transparent;

        public override bool ShouldRender => ActiveParticleCount >= 1;

        public override bool DrawnManually => true;

        public override Func<Texture2D>[] LayerTextures => [() => MiscTexturesRegistry.Pixel.Value];

        public override bool LayerIsFixedToScreen(int layerIndex) => true;

        public override bool PerformCustomSpritebatchBegin(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
            return true;
        }

        public override void UpdateParticle(MetaballInstance particle)
        {
            particle.Velocity.X *= 0.98f;
            particle.Size *= 0.93f;
            particle.Size -= particle.ExtraInfo[0];
        }

        public override bool ShouldKillParticle(MetaballInstance particle) => particle.Size <= 2f;
    }
}
