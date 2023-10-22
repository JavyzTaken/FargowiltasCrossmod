using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class ThoriumKeybinds : ModSystem
    {
        internal static ModKeybind LivingWoodBind;
        internal static ModKeybind SteelParryBind;
        public override void Load()
        {
            LivingWoodBind = KeybindLoader.RegisterKeybind(Mod, "Living wood roots", Keys.P);
            SteelParryBind = KeybindLoader.RegisterKeybind(Mod, "Steel parry", Keys.F);
        }
    }
}
