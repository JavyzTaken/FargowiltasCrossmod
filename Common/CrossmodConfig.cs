using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargowiltasCrossmod.Common
{
    public class CrossmodConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public static CrossmodConfig Instance => ModContent.GetInstance<CrossmodConfig>();

        [Header("Difficulty Modes")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category. 
        [DefaultValue(true)] // This sets the configs default value.
        [ReloadRequired] // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        public bool RevVanillaAIDisabled; // To see the implementation of this option, see ExampleWings.cs

        [ReloadRequired]
        public bool WeaponWithGrowingDamageToggle;
    }
}
