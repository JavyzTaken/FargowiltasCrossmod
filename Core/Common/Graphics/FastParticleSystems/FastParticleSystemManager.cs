using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using static FargowiltasCrossmod.Core.Common.Graphics.FastParticleSystems.FastParticleSystem;

namespace FargowiltasCrossmod.Core.Common.Graphics.FastParticleSystems;

[Autoload(Side = ModSide.Client)]
public class FastParticleSystemManager : ModSystem
{
    private static readonly List<FastParticleSystem> particleSystems = [];

    /// <summary>
    /// Creates a new particle system.
    /// </summary>
    /// <param name="maxParticles">The maximum amount of particles that can exist within the system.</param>
    /// <param name="renderPreparations">Preparations that should be performed when rendering particle quads.</param>
    /// <param name="extraUpdates">Extra update effects that should be applied to particles.</param>
    public static FastParticleSystem CreateNew(int maxParticles, Action renderPreparations, ParticleUpdateAction? extraUpdates = null)
    {
        FastParticleSystem system = new(maxParticles, renderPreparations, extraUpdates);
        particleSystems.Add(system);

        return system;
    }

    public override void OnModLoad()
    {
        foreach (FastParticleSystem system in particleSystems)
            system.Dispose();
    }
}
