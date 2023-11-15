using FargowiltasCrossmod.Core;
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
