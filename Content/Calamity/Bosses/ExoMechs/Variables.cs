using FargowiltasCrossmod.Core.Common.DataStructures;
using FargowiltasCrossmod.Core.Data;
using System;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs
{
    public static class Variables
    {
        /// <summary>
        /// Retrives a stored AI integer value with a given name.
        /// </summary>
        /// <param name="name">The value's named key.</param>
        /// <param name="prefix">The file name prefix.</param>
        public static int GetAIInt(string name, ExoMechAIVariableType prefix) => (int)MathF.Round(GetAIFloat(name, prefix));

        /// <summary>
        /// Retrives a stored AI floating point value with a given name.
        /// </summary>
        /// <param name="name">The value's named key.</param>
        /// <param name="prefix">The file name prefix.</param>
        public static float GetAIFloat(string name, ExoMechAIVariableType prefix) =>
            LocalDataManager.Read<DifficultyValue<float>>($"Content/Calamity/Bosses/ExoMechs/{prefix}AIValues.json")[name];
    }
}
