using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasCrossmod.Core.Common.Graphics.FastParticleSystems;

public struct FastParticle
{
    /// <summary>
    /// How long this particle has existed for, in frames.
    /// </summary>
    public int Time;

    /// <summary>
    /// Whether the particle is currently in use or not.
    /// </summary>
    public bool Active;

    /// <summary>
    /// The color of the particle.
    /// </summary>
    public Color Color;

    /// <summary>
    /// The particle's position in the world.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// The particle's velocity.
    /// </summary>
    public Vector2 Velocity;

    /// <summary>
    /// The particle's size.
    /// </summary>
    public Vector2 Size;

    /// <summary>
    /// The particle's rotation.
    /// </summary>
    public float Rotation;

    public FastParticle(Vector2 spawnPosition, Vector2 velocity, Vector2 size, Color color)
    {
        Position = spawnPosition;
        Velocity = velocity;
        Size = size;
        Rotation = velocity.ToRotation() + MathHelper.PiOver2;
        Color = color;
        Active = true;
    }
}
