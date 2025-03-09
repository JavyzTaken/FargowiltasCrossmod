using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity.ItemDropRules
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public static class CalDLCConditions
    {
        public static Condition RevNotEmodeCondition = new("In Revengeance but not Eternity Mode", () => ModCompatibility.Calamity.Mod?.Code.GetType("CalamityMod.World.CalamityWorld")?.GetField("revenge")?.GetValue(null) is bool revenge && revenge && !WorldSavingSystem.EternityMode);
        public static Condition EmodeNotRevCondition = new("In Eternity Mode but not Revengeance", () => ModCompatibility.Calamity.Mod?.Code.GetType("CalamityMod.World.CalamityWorld")?.GetField("revenge")?.GetValue(null) is bool revenge && !revenge && WorldSavingSystem.EternityMode);
        public static Condition EmodeAndRevCondition = new("In Eternity Mode and Revengeance", () => ModCompatibility.Calamity.Mod?.Code.GetType("CalamityMod.World.CalamityWorld")?.GetField("revenge")?.GetValue(null) is bool revenge && revenge && WorldSavingSystem.EternityMode);
    }
}
