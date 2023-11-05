using System;
using System.Collections.Generic;
using FargowiltasSouls.Content.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GildedLamp : SoulsItem // slime drop
    {
        public override bool Eternity => true;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddBuff(BuffID.Shine, 2);
            player.buffImmune[ModContent.BuffType<Buffs.GildedSightDB>()] = true;
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GildedMonicle : SoulsItem // bat drop
    {
        public override bool Eternity => true;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerThorium>().GildedMonicle = true;
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GildedBinoculars : SoulsItem // lycan drop
    {
        public override bool Eternity => true;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerThorium>().GildedBinoculars = true;
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GildedNightVision : SoulsItem
    {
        public override bool Eternity => true;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            player.AddBuff(BuffID.Shine, 2);
            DLCPlayer.GildedBinoculars = true;
            DLCPlayer.GildedMonicle = true;
            player.buffImmune[ModContent.BuffType<Buffs.GildedSightDB>()] = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<GildedBinoculars>();
            recipe.AddIngredient<GildedLamp>();
            recipe.AddIngredient<GildedMonicle>();
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
    }
}
