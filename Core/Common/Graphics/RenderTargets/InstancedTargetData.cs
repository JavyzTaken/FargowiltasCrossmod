using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace FargowiltasCrossmod.Core.Common.Graphics.RenderTargets;

/// <summary>
/// Represents an instance of target draw data for the purposes of use via <see cref="InstancedRequestableTarget"/>.
/// </summary>
/// <param name="DrawAction">The action to perform for the draw instance.</param>
/// <param name="TargetWidth">The width of the render target.</param>
/// <param name="TargetHeight">The height of the render target.</param>
/// <param name="Identifier">The identifier used to discriminate between each target instance.</param>
public class InstancedTargetData(Action DrawAction, int TargetWidth, int TargetHeight, int Identifier) : IDisposable
{
    public Action DrawAction
    {
        get;
        set;
    } = DrawAction;

    public int Identifier
    {
        get;
        init;
    } = Identifier;

    /// <summary>
    /// The render target associated with this draw instance.
    /// </summary>
    public RenderTarget2D Target
    {
        get;
        private set;
    } = new(Main.instance.GraphicsDevice, TargetWidth, TargetHeight);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Target.Dispose();
    }
}
