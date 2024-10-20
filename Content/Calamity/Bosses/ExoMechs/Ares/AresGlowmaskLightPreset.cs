using Microsoft.Xna.Framework;
using System;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    /// <summary>
    /// Represents a conditional preset that customizes Ares' glowmask light color palettes.
    /// </summary>
    /// <param name="Priority">The priority of this preset.</param>
    /// <param name="Condition">The condition that dictates whether the preset is applied.</param>
    /// <param name="LightColors">The palette that Ares should use for his glowmask with this preset.</param>
    public record AresGlowmaskLightPreset(float Priority, Func<bool> Condition, params Color[] LightColors);
}
