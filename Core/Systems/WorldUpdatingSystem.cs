using FargowiltasCrossmod.Core;
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
            CalamityConfig clamconfig = ModContent.GetInstance<CalamityConfig>();
            if (clamconfig == null) return;
            ModLoader.GetMod(ModCompatibility.SoulsMod.Name).Call("EternityVanillaBossBehaviour", !clamconfig.EternityVanillaAIDisabled);
        }
    }
}
