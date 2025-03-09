using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;

namespace FargowiltasCrossmod.Assets.Particles;

public class BileMetaball : MetaballType
{
    public override string MetaballAtlasTextureToUse => "FargowiltasCrossmod.DistancedMetaballCircle.png";

    public override Color EdgeColor => Color.Transparent;

    public override bool ShouldRender => ActiveParticleCount >= 1;

    public override bool DrawnManually => false;

    public override Func<Texture2D>[] LayerTextures => [() => Main.gameMenu ? MiscTexturesRegistry.InvisiblePixel.Value : NoiseTexturesRegistry.BubblyNoise.Value];

    public override void UpdateParticle(MetaballInstance particle)
    {
        bool inWater = Collision.WetCollision(particle.Center - Vector2.One * 8f, 16, 16);

        // Have the vomit linger in the water.
        // It's completely disgusting. Like, actually terrible. An extremely gross thing to imagine.
        // But it's a wonderful detail and thematically aligned.
        // And that's what counts.
        bool wasInWaterBefore = particle.ExtraInfo[1] == 1f;
        float verticalAcceleration = wasInWaterBefore ? 0.27f : 0.56f;
        if (inWater)
            particle.ExtraInfo[1] = 1f;
        else if (!wasInWaterBefore)
            particle.Velocity.Y += verticalAcceleration;

        if (wasInWaterBefore)
        {
            if (inWater)
            {
                particle.Velocity *= 0.98f;
                particle.Velocity.Y = MathHelper.Lerp(particle.Velocity.Y, -2.6f, 0.1f);
            }
            else
                particle.Velocity.Y = MathHelper.Clamp(particle.Velocity.Y + 0.4f, -36f, 10.5f);

            particle.ExtraInfo[2] = MathHelper.Lerp(particle.ExtraInfo[2], 1f, 0.03f);
        }

        particle.Size *= wasInWaterBefore ? 0.997f : 0.985f;
        particle.Velocity.X *= 0.97f;
        if (particle.Velocity.Y > 16f)
            particle.Velocity.Y = MathHelper.Lerp(particle.Velocity.Y, 16f, 0.04f);

        particle.Velocity = Collision.TileCollision(particle.Center, particle.Velocity, 1, 1);
    }

    public override bool ShouldKillParticle(MetaballInstance particle) => particle.Size <= 2f;

    public override void PrepareShaderForTarget(int layerIndex)
    {
        // Store the shader in an easy to use local variable.
        ManagedShader metaballShader = ShaderManager.GetShader("FargowiltasCrossmod.OldDukeBileMetaballShader");

        // Fetch the layer texture. This is the texture that will be overlaid over the greyscale contents on the screen.
        Texture2D layerTexture = LayerTextures[layerIndex]();

        // Calculate the layer scroll offset. This is used to ensure that the texture contents of the given metaball have parallax, rather than being static over the screen
        // regardless of world position.
        // This may be toggled off optionally by the metaball.
        Vector2 screenSize = Main.ScreenSize.ToVector2();
        Vector2 layerScrollOffset = Main.screenPosition / screenSize + CalculateManualOffsetForLayer(layerIndex);
        if (LayerIsFixedToScreen(layerIndex))
            layerScrollOffset = Vector2.Zero;

        Vector3[] palette =
        [
            new Color(148, 189, 3).ToVector3(),
            new Color(96, 95, 7).ToVector3(),
            new Color(53, 47, 0).ToVector3()
        ];

        metaballShader.TrySetParameter("layerSize", layerTexture.Size());
        metaballShader.TrySetParameter("screenSize", screenSize);
        metaballShader.TrySetParameter("layerOffset", layerScrollOffset);
        metaballShader.TrySetParameter("singleFrameScreenOffset", (Main.screenLastPosition - Main.screenPosition) / screenSize);
        metaballShader.TrySetParameter("gradient", palette);
        metaballShader.TrySetParameter("gradientCount", palette.Length);
        metaballShader.TrySetParameter("dissolvePersistence", 1.65f);
        metaballShader.TrySetParameter("maxDistortionOffset", new Vector2(20f, 8f));
        metaballShader.SetTexture(NoiseTexturesRegistry.PerlinNoise.Value, 1, SamplerState.LinearWrap);
        metaballShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 2, SamplerState.LinearWrap);
        metaballShader.SetTexture(MiscTexturesRegistry.DendriticNoise.Value, 3, SamplerState.LinearWrap);

        // Apply the metaball shader.
        metaballShader.Apply();
    }

    public override void DrawInstances()
    {
        AtlasTexture texture = AtlasManager.GetTexture(MetaballAtlasTextureToUse);
        var darknessOrderedParticles = Particles.OrderByDescending(p => p.ExtraInfo[0]);

        foreach (var particle in darknessOrderedParticles)
        {
            float darknessInterpolant = particle.ExtraInfo[0];
            float dissolveInterpolant = MathF.Pow(LumUtils.InverseLerp(2f, 55f, particle.Size), 1f);
            float squish = particle.Velocity.Length() * 0.024f;
            if (squish > 0.45f)
                squish = 0.45f;

            float rotation = particle.Velocity.ToRotation();
            Color color = new(1f, darknessInterpolant, dissolveInterpolant);
            Vector2 particleSize = new Vector2(1f + squish, 1f - squish + particle.ExtraInfo[2]) * particle.Size;

            Main.spriteBatch.Draw(texture, particle.Center - Main.screenPosition, null, color, rotation, null, particleSize / texture.Frame.Size(), SpriteEffects.None);
        }
    }
}
