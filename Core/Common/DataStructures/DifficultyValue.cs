using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasSouls.Core.Systems;
using Newtonsoft.Json;

namespace FargowiltasCrossmod.Core.Common.DataStructures
{
    // I would ordinarily make this all readonly, but that causes problems with JSON deserialization.

    /// <summary>
    /// Represents a value that should vary based on the three main-line modes: E-Rev, E-Death, and Masomode.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public struct DifficultyValue<TValue> where TValue : struct
    {
        /// <summary>
        /// The value that should be selected in Revengeance Eternity mode.
        /// </summary>
        public TValue EternityRevValue;

        /// <summary>
        /// The value that should be selected in Death Eternity mode.
        /// </summary>
        public TValue EternityDeathValue;

        /// <summary>
        /// The value that should be selected in Masochist Mode.
        /// </summary>
        public TValue MasochistValue;

        /// <summary>
        /// The value that should be selected
        /// </summary>
        [JsonIgnore]
        public readonly TValue Value
        {
            get
            {
                if (WorldSavingSystem.MasochistModeReal)
                    return MasochistValue;
                if (CalDLCWorldSavingSystem.EternityDeath)
                    return EternityDeathValue;

                return EternityRevValue;
            }
        }

        public static implicit operator TValue(DifficultyValue<TValue> difficultyValue) => difficultyValue.Value;
    }
}
