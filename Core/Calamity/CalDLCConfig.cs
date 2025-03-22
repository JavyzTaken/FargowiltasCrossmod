using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargowiltasCrossmod.Core.Calamity
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public static CalDLCConfig Instance => ModContent.GetInstance<CalDLCConfig>();

        [Header("Modes")] // Title

        [DefaultValue(true)]
        public bool EternityPriorityOverRev;

        //[DefaultValue(false)]
        //public bool VoiceActingEnabled;

        [ReloadRequired]
        [DefaultValue(true)]
        public bool InfernumDisablesEternity;
    }
}
