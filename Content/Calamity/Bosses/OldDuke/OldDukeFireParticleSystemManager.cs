using FargowiltasCrossmod.Assets;
using FargowiltasCrossmod.Core.Common.Graphics.FastParticleSystems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

[Autoload(Side = ModSide.Client)]
public class OldDukeFireParticleSystemManager : ModSystem
{
    /// <summary>
    /// The particle system used to render Nameless' fire.
    /// </summary>
    public static FireParticleSystem ParticleSystem
    {
        get;
        private set;
    }

    public override void OnModLoad()
    {
        ParticleSystem = new FireParticleSystem(NoiseTexturesRegistry.FireParticleA, 23, 1024, PrepareShader, UpdateParticle);
        On_Main.DrawDust += RenderParticles;
    }

    private static void PrepareShader()
    {
        Matrix world = Matrix.CreateTranslation(-Main.screenPosition.X, -Main.screenPosition.Y, 0f);
        Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, -100f, 100f);

        Main.instance.GraphicsDevice.BlendState = BlendState.Additive;

        ManagedShader overlayShader = ShaderManager.GetShader("FargowiltasCrossmod.FireParticleDissolveShader");
        overlayShader.TrySetParameter("pixelationLevel", 1485f);
        overlayShader.TrySetParameter("turbulence", 0.023f);
        overlayShader.TrySetParameter("screenPosition", Main.screenPosition);
        overlayShader.TrySetParameter("uWorldViewProjection", world * Main.GameViewMatrix.TransformationMatrix * projection);
        overlayShader.TrySetParameter("imageSize", NoiseTexturesRegistry.FireParticleA.Value.Size());
        overlayShader.TrySetParameter("initialGlowIntensity", 0.81f);
        overlayShader.TrySetParameter("initialGlowDuration", 0.285f);
        overlayShader.SetTexture(NoiseTexturesRegistry.FireParticleA.Value, 1, SamplerState.LinearClamp);
        overlayShader.SetTexture(NoiseTexturesRegistry.FireParticleB.Value, 2, SamplerState.LinearClamp);
        overlayShader.SetTexture(NoiseTexturesRegistry.PerlinNoise.Value, 3, SamplerState.LinearWrap);
        overlayShader.Apply();
    }

    private static void UpdateParticle(ref FastParticle particle)
    {
        float growthRate = 0.02f;
        particle.Size.X *= 1f + growthRate * 0.85f;
        particle.Size.Y *= 1f + growthRate;

        Vector3 hslVector = Main.rgbToHsl(particle.Color);
        Color lowValueColor = Main.hslToRgb(hslVector.X - 0.06f, hslVector.Y, 0.2f);
        particle.Color = Color.Lerp(particle.Color, lowValueColor, 0.07f);
        particle.Velocity *= 0.7f;

        if (particle.Time >= ParticleSystem.ParticleLifetime + 15)
            particle.Active = false;
    }

    public override void PreUpdateEntities()
    {
        ParticleSystem.UpdateAll();
    }

    private static void RenderParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (ParticleSystem.particles.Any(p => p.Active))
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            ParticleSystem.RenderAll();
            Main.spriteBatch.End();
        }
    }
}
