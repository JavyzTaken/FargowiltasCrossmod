using Luminance.Assets;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Assets.Particles.Metaballs
{
    [Autoload(Side = ModSide.Client)]
    public class HeatDistortionSystem : ModSystem
    {
        public override void PostUpdateEverything()
        {
            ManagedScreenFilter distortionShader = ShaderManager.GetFilter("FargowiltasCrossmod.HeatDistortionFilter");

            bool shouldBeActive = ModContent.GetInstance<HeatDistortionMetaball>().ActiveParticleCount >= 1;
            if (!distortionShader.IsActive && shouldBeActive)
                ApplyDistortionParameters(distortionShader);
            else
            {
                distortionShader.Deactivate();
                while (distortionShader.Opacity > 0f)
                    distortionShader.Update();
            }
        }

        private static void ApplyDistortionParameters(ManagedScreenFilter distortionShader)
        {
            distortionShader.TrySetParameter("screenZoom", Main.GameViewMatrix.Zoom);
            distortionShader.SetTexture(ModContent.GetInstance<HeatDistortionMetaball>().LayerTargets[0], 2, SamplerState.LinearClamp);
            distortionShader.SetTexture(MiscTexturesRegistry.DendriticNoise.Value, 3, SamplerState.LinearWrap);
            distortionShader.Activate();
        }
    }
}
