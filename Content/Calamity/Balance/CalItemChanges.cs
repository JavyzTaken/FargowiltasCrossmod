using CalamityMod.CalPlayer;
using CalamityMod.Items.PermanentBoosters;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls.Content.Items.Consumables;
using FargowiltasSouls.Core.ModPlayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Balance
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalItemChanges : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ModContent.ItemType<CelestialOnion>() && CalamityConfig.Instance.BalanceChanges)
            {
                return player.GetModPlayer<FargoSoulsPlayer>().MutantsPactSlot;
            }
            return true;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<CelestialOnion>() && CalamityConfig.Instance.BalanceChanges)
            {
                tooltips.Add(new TooltipLine(Mod, "OnionDisabled", $"[c/FF0000:\"Calamity/Souls Balance changes\":] Is now an upgrade to [i:{ModContent.ItemType<MutantsPact>()}]Mutant's Pact, that allows any accessory in the extra slot."));
            }
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalExtraSlotPlayer : ModPlayer
    {
        public bool MutantPactShouldBeEnabled = false;
        public override void PostUpdate()
        {
            ref bool MutantsPactSlot = ref Player.GetModPlayer<FargoSoulsPlayer>().MutantsPactSlot;
            if (Player.GetModPlayer<CalamityPlayer>().extraAccessoryML && CalamityConfig.Instance.BalanceChanges) 
            {
                if (MutantsPactSlot)
                {
                    MutantPactShouldBeEnabled = true; //store if the slot is enabled
                    MutantsPactSlot = false; //turn it off since celestial onion slot is replacing it
                }
            }
            else if (MutantPactShouldBeEnabled)
            {
                MutantsPactSlot = true;
            }
        }
        public override void SaveData(TagCompound tag)
        {
            tag.Add($"{Mod.Name}.{Player.name}.MutantPactShouldBeEnabled", MutantPactShouldBeEnabled);
        }
        public override void LoadData(TagCompound tag)
        {
            MutantPactShouldBeEnabled = tag.GetBool($"{Mod.Name}.{Player.name}.MutantPactShouldBeEnabled");
        }
    }
}
