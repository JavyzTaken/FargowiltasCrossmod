using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargowiltasCrossmod.Core.Calamity
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DLCCalamityConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public static DLCCalamityConfig Instance => ModContent.GetInstance<DLCCalamityConfig>();

        [Header("Modes")] // Title

        [DefaultValue(true)] // This sets the configs default value.
        public bool EternityPriorityOverRev; 

        [Header("Balance")] // Title

        [ReloadRequired]
        [DefaultValue(true)] // This sets the configs default value.
        public bool BalanceRework;

    }
}
