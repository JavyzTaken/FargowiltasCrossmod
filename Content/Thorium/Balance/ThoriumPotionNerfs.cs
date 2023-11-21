using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Items.Consumable;
using ThoriumMod.Buffs;
using System.Collections.Generic;

namespace FargowiltasCrossmod.Content.Thorium.Balance
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class ThoriumPotionNerfs : GlobalItem
    {
        private static readonly Dictionary<int, int> NerfedPotions = new()
        {
            { ModContent.ItemType<BatRepellent>(), ModContent.BuffType<BatRepellentBuff>() },
            { ModContent.ItemType<FishRepellent>(), ModContent.BuffType<FishRepellentBuff>() },
            { ModContent.ItemType<InsectRepellent>(), ModContent.BuffType<InsectRepellentBuff>() },
            { ModContent.ItemType<SkeletonRepellent>(), ModContent.BuffType<SkeletonRepellentBuff>() },
            { ModContent.ItemType<ZombieRepellent>(), ModContent.BuffType<ZombieRepellentBuff>() },
            { ModContent.ItemType<ThoriumMod.Items.NPCItems.CactusFruit>(), ModContent.BuffType<CactusFruitBuff>() },
            { ModContent.ItemType<ThoriumMod.Items.Donate.KineticPotion>(), ModContent.BuffType<KineticPotionBuff>() },
        };

        public static void MurderBuffs(Player player)
        {
            foreach (var pair in NerfedPotions)
            {
                if (player.HasBuff(pair.Value))
                {
                    //int index = player.FindBuffIndex(pair.Value);
                    //player.DelBuff(index);
                    player.ClearBuff(pair.Value);
                }
            }
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return NerfedPotions.ContainsKey(entity.type);
        }

        public override bool CanUseItem(Item item, Player player)
        {
            return !FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode)
                tooltips.Add(new TooltipLine(Mod, "ThoriumBalance", $"[c/FF0000:Thorium Crossmod Support:] Disabled"));
        }
    }
}