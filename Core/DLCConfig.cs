using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FargowiltasCrossmod.Core
{
    public class ThoriumConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        private const string ModName = "FargoWiltasCrossmod";

        [LabelKey($"$Mods.{ModName}.Configs.ThoriumConfig.HideDevThorium")]
        [TooltipKey($"$Mods.{ModName}.Configs.ThoriumConfig.HideDevThoriumDescription")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool HideWIPThorium;
    }
}