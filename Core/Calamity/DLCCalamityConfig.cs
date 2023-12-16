using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargowiltasCrossmod.Core.Calamity
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class DLCCalamityConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public static DLCCalamityConfig Instance => ModContent.GetInstance<DLCCalamityConfig>();

        [Header("Modes")] // Title

        [DefaultValue(true)]
        public bool EternityPriorityOverRev;
        
        [ReloadRequired]
        [DefaultValue(false)]
        public bool InfernumDisablesEternity;

        [Header("Balance")] // Title

        [ReloadRequired]
        [DefaultValue(true)]
        public bool BalanceRework;


    }
}
