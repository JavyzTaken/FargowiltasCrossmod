using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Thorium.Buffs
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GildedSightDB : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderDebuff";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;

        }

        public static readonly int[] GildedItems =
        {
            ItemID.GoldOre, ItemID.PlatinumOre, ModContent.ItemType<ThoriumMod.Items.Thorium.ThoriumOre>()
        };
    }
}
