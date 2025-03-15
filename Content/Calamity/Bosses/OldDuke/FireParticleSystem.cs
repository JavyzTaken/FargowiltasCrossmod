using FargowiltasCrossmod.Core.Common.Graphics.FastParticleSystems;
using Luminance.Assets;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.CompilerServices;
using Terraria;
using MatrixSIMD = System.Numerics.Matrix4x4;
using Vector2SIMD = System.Numerics.Vector2;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

public class FireParticleSystem : FastParticleSystem
{
    private readonly LazyAsset<Texture2D> texture;

    public int ParticleLifetime;

    public FireParticleSystem(LazyAsset<Texture2D> texture, int particleLifetime, int maxParticles, Action renderPreparations, ParticleUpdateAction? extraUpdates = null) : base(maxParticles, renderPreparations, extraUpdates)
    {
        this.texture = texture;
        ParticleLifetime = particleLifetime;
    }

    protected override void PopulateVertexBufferIndex(VertexPosition2DColorTexture[] vertices, int particleIndex)
    {
        ref FastParticle particle = ref particles[particleIndex];
        int vertexIndex = particleIndex * 4;

        if (!particle.Active)
        {
            vertices[vertexIndex] = new VertexPosition2DColorTexture(default, default, default, 0f);
            vertices[vertexIndex + 1] = vertices[vertexIndex];
            vertices[vertexIndex + 2] = vertices[vertexIndex];
            vertices[vertexIndex + 3] = vertices[vertexIndex];
            return;
        }

        float scale = LumUtils.InverseLerp(0f, 1f, particle.Time);
        scale += LumUtils.InverseLerp(4f, 24f, particle.Time) * 0.75f;

        float fadeIn = LumUtils.InverseLerp(0f, 5f, particle.Time);
        float glow = 1f + LumUtils.InverseLerp(18f, 0f, particle.Time) * 0.5f;
        Color color = particle.Active ? particle.Color * glow * fadeIn : Color.Transparent;
        Vector2SIMD center = Unsafe.As<Vector2, Vector2SIMD>(ref particle.Position);
        Vector2SIMD size = Unsafe.As<Vector2, Vector2SIMD>(ref particle.Size) * scale;
        MatrixSIMD particleRotationMatrix = MatrixSIMD.CreateRotationZ(particle.Rotation);
        Vector2SIMD topLeftPosition = center + Vector2SIMD.Transform(topLeftOffset * size, particleRotationMatrix);
        Vector2SIMD topRightPosition = center + Vector2SIMD.Transform(topRightOffset * size, particleRotationMatrix);
        Vector2SIMD bottomLeftPosition = center + Vector2SIMD.Transform(bottomLeftOffset * size, particleRotationMatrix);
        Vector2SIMD bottomRightPosition = center + Vector2SIMD.Transform(bottomRightOffset * size, particleRotationMatrix);

        int lifetimeBoost = particleIndex * 313 % 10;
        float frameInterpolant = LumUtils.InverseLerp(0f, ParticleLifetime + lifetimeBoost, particle.Time);
        int frameIndex = (int)MathHelper.Lerp(9f, 36f, MathF.Pow(frameInterpolant, 0.67f));
        Rectangle frameRectangle = texture.Value.Frame(6, 6, frameIndex % 6, frameIndex / 6);
        Vector2 inverseTextureArea = Vector2.One / texture.Value.Size();

        // Scuffed way of determining whether the alt texture should be used.
        frameInterpolant += particleIndex % 2;

        vertices[vertexIndex] = new(Unsafe.As<Vector2SIMD, Vector2>(ref topLeftPosition), color, frameRectangle.TopLeft() * inverseTextureArea, frameInterpolant);
        vertices[vertexIndex + 1] = new(Unsafe.As<Vector2SIMD, Vector2>(ref topRightPosition), color, frameRectangle.TopRight() * inverseTextureArea, frameInterpolant);
        vertices[vertexIndex + 2] = new(Unsafe.As<Vector2SIMD, Vector2>(ref bottomRightPosition), color, frameRectangle.BottomRight() * inverseTextureArea, frameInterpolant);
        vertices[vertexIndex + 3] = new(Unsafe.As<Vector2SIMD, Vector2>(ref bottomLeftPosition), color, frameRectangle.BottomLeft() * inverseTextureArea, frameInterpolant);
    }
}
