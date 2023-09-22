using System.ComponentModel;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria.ModLoader.Config;

namespace FargowiltasCrossmod.Core.Calamity
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamityConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public static CalamityConfig Instance => ModContent.GetInstance<CalamityConfig>();

        [Header("Modes")] // Title

        [DefaultValue(true)] // This sets the configs default value.
        public bool EternityPriorityOverRev; 

        [Header("Balance")] // Title

        [ReloadRequired]
        [DefaultValue(true)] // This sets the configs default value.
        public bool BalanceRework;

    }
}
