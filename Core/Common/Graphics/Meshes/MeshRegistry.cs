using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Common.Graphics.Meshes;

public class MeshRegistry : ModSystem
{
    /// <summary>
    /// The vertices used by the cylinder mesh.
    /// </summary>
    public static VertexBuffer CylinderVertices
    {
        get;
        private set;
    }

    /// <summary>
    /// The indices used by the cylinder mesh.
    /// </summary>
    public static IndexBuffer CylinderIndices
    {
        get;
        private set;
    }

    public override void OnModLoad() => Main.QueueMainThreadAction(GenerateMeshes);

    public override void OnModUnload() => Main.QueueMainThreadAction(ClearUnmanagedResources);

    /// <summary>
    /// Generates all meshes, centered around the origin.
    /// </summary>
    private static void GenerateMeshes()
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        GenerateMeshes_Cylinder();
    }

    /// <summary>
    /// Generates the cylindrical mesh.
    /// </summary>
    private static void GenerateMeshes_Cylinder()
    {
        int width = 40;
        int height = 30;
        VertexPositionColorNormalTexture[] vertices = new VertexPositionColorNormalTexture[(width + 1) * (height + 1)];
        short[] indices = new short[width * height * 6];

        for (int i = 0; i <= width; i++)
        {
            float angle = MathHelper.TwoPi * i / width;
            for (int j = 0; j <= height; j++)
            {
                Vector2 uv = new(MathF.Cos(angle), j / (float)height);
                Vector3 position = new(MathF.Cos(angle) * 0.5f, uv.Y - 0.5f, MathF.Sin(angle) * 0.5f);
                Vector3 normal = new(MathF.Cos(angle), 0f, MathF.Sin(angle));

                vertices[i + (width + 1) * j] = new VertexPositionColorNormalTexture(position, Color.White, uv, normal);
            }
        }

        int index = 0;
        for (short y = 0; y < height; y++)
        {
            for (short x = 0; x < width; x++)
            {
                short topLeft = (short)(y * (width + 1) + x);
                short topRight = (short)(topLeft + 1);
                short bottomLeft = (short)((y + 1) * (width + 1) + x);
                short bottomRight = (short)(bottomLeft + 1);

                indices[index++] = topLeft;
                indices[index++] = bottomRight;
                indices[index++] = bottomLeft;

                indices[index++] = topLeft;
                indices[index++] = topRight;
                indices[index++] = bottomRight;
            }
        }

        CylinderVertices = new(Main.instance.GraphicsDevice, typeof(VertexPositionColorNormalTexture), vertices.Length, BufferUsage.None);
        CylinderVertices.SetData(vertices);

        CylinderIndices = new(Main.instance.GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
        CylinderIndices.SetData(indices);
    }

    /// <summary>
    /// Clears all unmanaged resources held by the index and vertex buffers.
    /// </summary>
    private static void ClearUnmanagedResources()
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        CylinderVertices?.Dispose();
        CylinderIndices?.Dispose();
    }
}
