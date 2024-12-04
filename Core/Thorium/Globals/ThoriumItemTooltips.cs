
using System.Collections.Generic;
using FargowiltasSouls.Content.Items;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using ThoriumMod.Items;
using ThoriumMod.Items.BasicAccessories;
using ThoriumMod.Items.BossForgottenOne;
using ThoriumMod.Items.Donate;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Items.MeleeItems;
using ThoriumMod.Items.ThrownItems;

namespace FargowiltasCrossmod.Core.Thorium.Globals
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    class ThoriumItemBalance : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return true;
        }

        public override void SetDefaults(Item entity)
        {
            if (!FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode) return;
            
            ThoriumItem thoriumItem = entity.ModItem as ThoriumItem;

            if (entity.type == ModContent.ItemType<SilverSpearTip>())
                thoriumItem.accDamage = "25% base damage";
            if (entity.type == ModContent.ItemType<MoltenSpearTip>())
                thoriumItem.accDamage = "50% base damage";
            if (entity.type == ModContent.ItemType<CrystalSpearTip>())
                thoriumItem.accDamage = "50% base damage";
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode) return;
            
            const string BalanceLine = "Cross-mod Balance: ";
            const string BalanceUpLine = $"[c/00A36C:{BalanceLine}]";
            const string BalanceDownLine = $"[c/FF0000:{BalanceLine}]";
            
            if (ThoriumPotionNerfs.NerfedPotions.ContainsKey(item.type))
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDisable", Language.GetTextValue($"{BalanceDownLine}Disabled")));
            }
            if (item.ModItem is BardItem bardItem && item.damage > 0)
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Only 4 empowerments can be active at once")));
            }
            if (item.type == ModContent.ItemType<LeechBolt>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Will only add 1/4 of bonus healing")));
            }
            if (item.type == ModContent.ItemType<ShinobiSigil>() && !item.social)
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Critical strike effect can only be activated every 5 seconds")));
            }
            if (item.type == ModContent.ItemType<AbyssalShell>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Shell only lasts for 10 seconds and has a 60 second cooldown")));
            }
            if (item.type == ModContent.ItemType<SilverSpearTip>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Decreased damage to 25% of spear damage")));
            }
            if (item.type == ModContent.ItemType<MoltenSpearTip>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Decreased damage to 50% of spear damage")));
            }
            if (item.type == ModContent.ItemType<CrystalSpearTip>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Decreased damage to 50% of spear damage")));
            }
            if (item.ModItem is SwordSheathBase sheathBase)
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Halved sheath damage")));
                if (item.type == ModContent.ItemType<GardenersSheath>())
                {
                    tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Teleport effect has a 30 second cooldown")));
                }
            }

            if (item.type == ModContent.ItemType<WhiteDwarfMask>() ||
                item.type == ModContent.ItemType<WhiteDwarfGreaves>() ||
                item.type == ModContent.ItemType<WhiteDwarfGuard>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Ivory flares max damage 5000 and 5s cooldown")));
            }

            if (item.type == ModContent.ItemType<BalanceBloom>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", Language.GetTextValue($"{BalanceDownLine}Nerfed Damage by 33%")));
            }
        }
    }
}

