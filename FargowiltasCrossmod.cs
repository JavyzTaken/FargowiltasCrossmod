using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace FargowiltasCrossmod;

public class FargowiltasCrossmod : Mod
{
    public override void Load()
    {
        if (ModLoader.TryGetMod(ModCompatibility.Calamity.Name, out Mod calamity))
        {
            _ = new EternityRevDifficulty();
        }
    }
}