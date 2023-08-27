using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargowiltasCrossmod.Core.Calamity
{
    public class CrossmodConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        public static CrossmodConfig Instance => ModContent.GetInstance<CrossmodConfig>();

        [Header("Modes")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category. 
        [DefaultValue(true)] // This sets the configs default value.
        public bool RevVanillaAIDisabled; // To see the implementation of this option, see ExampleWings.cs

    }
}
