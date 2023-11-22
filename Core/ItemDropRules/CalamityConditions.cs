using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.ItemDropRules
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public static class CalamityConditions
    {
        public static Condition RevNotEmodeCondition = new("In Revengeance but not Eternity Mode", () => ModCompatibility.Calamity.Mod?.Code.GetType("CalamityMod.World.CalamityWorld")?.GetField("revenge")?.GetValue(null) is bool revenge && revenge && !WorldSavingSystem.EternityMode);
        public static Condition EmodeNotRevCondition = new("In Eternity Mode but not Revengeance", () => ModCompatibility.Calamity.Mod?.Code.GetType("CalamityMod.World.CalamityWorld")?.GetField("revenge")?.GetValue(null) is bool revenge && !revenge && WorldSavingSystem.EternityMode);
        public static Condition PreHardmodeAndNotBalance = new("Always", () => !Main.hardMode && !ModContent.GetInstance<DLCCalamityConfig>().BalanceRework);
    }
}
