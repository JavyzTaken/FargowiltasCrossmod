using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace FargowiltasCrossmod.Core.Common.Graphics.Meshes;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct VertexPositionColorNormalTexture : IVertexType
{
    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

    /// <summary>
    /// The position of this vertex.
    /// </summary>
    public readonly Vector3 Position;

    /// <summary>
    /// The color of this vertex.
    /// </summary>
    public readonly Color Color;

    /// <summary>
    /// The texture coordinate associated with the vertex.
    /// </summary>
    public readonly Vector2 TextureCoords;

    /// <summary>
    /// The normal vector of this vertex.
    /// </summary>
    public readonly Vector3 Normal;

    /// <summary>
    /// The vertex's unmanaged declaration.
    /// </summary>
    public static readonly VertexDeclaration VertexDeclaration;

    static VertexPositionColorNormalTexture()
    {
        VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );
    }

    public VertexPositionColorNormalTexture(Vector3 position, Color color, Vector2 textureCoordinates, Vector3 normal)
    {
        Position = position;
        Color = color;
        TextureCoords = textureCoordinates;
        Normal = normal;
    }
}
