using CalamityMod.World;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using ThoriumMod.ModSupport;

namespace FargowiltasCrossmod.Core.ItemDropRules
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public static class CalamityConditions
    {
        public static Condition RevNotEmodeCondition = new Condition("In Revengeance but not Eternity Mode", () => ModLoader.GetMod("CalamityMod").Code.GetType("CalamityMod.World.CalamityWorld").GetField("revenge").GetValue(null) is bool fard && fard && !WorldSavingSystem.EternityMode);
        public static Condition PreHardmodeAndNotBalance = new Condition("Always", () => !Main.hardMode && !ModContent.GetInstance<CalamityConfig>().BalanceRework);
    }
}
