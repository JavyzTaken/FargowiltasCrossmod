using CalamityMod.Items.PermanentBoosters;

using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls.Content.Items.Consumables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Balance
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalItemChanges : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<CelestialOnion>() && CalamityConfig.Instance.BalanceChanges)
            {
                return false;
            }
            return true;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<CelestialOnion>() && CalamityConfig.Instance.BalanceChanges)
            {
                tooltips.Add(new TooltipLine(Mod, "OnionDisabled", $"[c/FF0000:Disabled by the \"Calamity/Souls Balance changes\" config. \nUse the ][i:{ModContent.ItemType<MutantsPact>()}][c/FF0000:Mutant's Pact for an additional accessory slot.]"));
            }
        }
    }
}
