using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargowiltasCrossmod.Core.Calamity
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamityConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public static CalamityConfig Instance => ModContent.GetInstance<CalamityConfig>();

        [Header("Modes")] // Title

        [DefaultValue(true)] // This sets the configs default value.
        public bool RevVanillaAIDisabled; 

        [DefaultValue(false)] // This sets the configs default value.
        public bool EternityVanillaAIDisabled;

        [Header("Balance")] // Title

        [DefaultValue(true)] // This sets the configs default value.
        [ReloadRequired]
        public bool ProgressionChanges;

        [DefaultValue(true)] // This sets the configs default value.
        public bool BalanceChanges;

    }
}
