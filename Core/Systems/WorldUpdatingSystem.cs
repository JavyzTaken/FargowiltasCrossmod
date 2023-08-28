using FargowiltasCrossmod.Core.Calamity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Common.Systems
{
    public class WorldUpdatingSystem : ModSystem
    {
        public override void PreUpdateWorld()
        {
            ModLoader.GetMod("FargowiltasSouls").Call("EternityVanillaBossBehaviour", !ModContent.GetInstance<CalamityConfig>().EternityVanillaAIDisabled);
        }
    }
}
