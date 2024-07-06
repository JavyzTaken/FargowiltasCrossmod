using FargowiltasCrossmod.Core;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExoMechDamageRecorderPlayer : ModPlayer
    {
        private readonly Dictionary<ExoMechDamageSource, int> damageDonePerSource = [];

        /// <summary>
        /// Calculates how much damage has been done in the Exo Mechs fight to this player by a given damage source.
        /// </summary>
        /// <param name="source">The damage source to evaluate.</param>
        public int GetDamageBySource(ExoMechDamageSource source)
        {
            if (damageDonePerSource.TryGetValue(source, out int damage))
                return damage;

            return damageDonePerSource[source] = 0;
        }

        /// <summary>
        /// Adds damage to this player for a given source.
        /// </summary>
        /// <param name="source">The damage source.</param>
        /// <param name="damage">The damage to incur.</param>
        public void AddDamageFromSource(ExoMechDamageSource source, int damage)
        {
            if (damageDonePerSource.TryGetValue(source, out _))
            {
                damageDonePerSource[source] += damage;
                return;
            }

            damageDonePerSource[source] = damage;
        }

        /// <summary>
        /// The damage source that has thus far done the most damage to the player.
        /// </summary>
        public ExoMechDamageSource MostDamagingSource
        {
            get
            {
                // Fallback case.
                if (damageDonePerSource.Count <= 0)
                    return ExoMechDamageSource.Thermal;

                return damageDonePerSource.MaxBy(kv => kv.Value).Key;
            }
        }

        /// <summary>
        /// Resets all incurred damage by Exo Mech damage sources.
        /// </summary>
        public void ResetIncurredDamage()
        {
            for (int i = 0; i < (int)ExoMechDamageSource.Count; i++)
                damageDonePerSource[(ExoMechDamageSource)i] = 0;
        }
    }
}
