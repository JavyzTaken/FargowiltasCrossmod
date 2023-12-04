using CalamityMod.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using FargowiltasSouls.Content.Items.Materials;
using Terraria.ID;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using Fargowiltas.Items.Tiles;
using CalamityMod.Rarities;
using Terraria.Localization;
using CalamityMod.Items.Materials;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class TyrantSoul : BaseSoul
    {
        //public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ModContent.RarityType<Violet>();
            Item.defense = 50;
        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.GetToggleValue("HeartoftheElements"))
                ModContent.GetInstance<HeartoftheElements>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("OccultSkullCrown"))
                ModContent.GetInstance<OccultSkullCrown>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("NebulousCore"))
                ModContent.GetInstance<NebulousCore>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("YharimsGift"))
                ModContent.GetInstance<YharimsGift>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("DraedonsHeart"))
                ModContent.GetInstance<DraedonsHeart>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("Calamity"))
                ModContent.GetInstance<CalamityMod.Items.Accessories.Calamity>().UpdateAccessory(player, hideVisual);
            //player.GetModPlayer<CrossplayerCalamity>().AncestralCharm = true;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient<HeartoftheElements>()
                .AddIngredient<OccultSkullCrown>()
                .AddIngredient<NebulousCore>()
                .AddIngredient<YharimsGift>()
                .AddIngredient<DraedonsHeart>()
                .AddIngredient<CalamityMod.Items.Accessories.Calamity>()
                .AddIngredient<ShadowspecBar>(10)
                .AddTile<CrucibleCosmosSheet>()
                .Register();
        }
    }

}