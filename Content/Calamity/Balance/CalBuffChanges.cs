using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Buffs.Masomode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Balance
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalBuffChanges : GlobalBuff
    {
        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            /*
            if (type == ModContent.BuffType<JammedBuff>())
            {
                tip += ("\nAlso applies to Rogue weapons");
            }
            */
        }
    }
}
