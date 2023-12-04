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
using Terraria.DataStructures;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrandoftheBrimstoneWitch : SoulsItem
    {
        //public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(8, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
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
            if (player.GetToggleValue("OccultSkullCrown", false))
                ModContent.GetInstance<OccultSkullCrown>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("Purity"))
                ModContent.GetInstance<Purity>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("TheSponge"))
                ModContent.GetInstance<TheSponge>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("ChaliceOfTheBloodGod", false))
                ModContent.GetInstance<ChaliceOfTheBloodGod>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("NebulousCore", false))
                ModContent.GetInstance<NebulousCore>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("YharimsGift", false))
                ModContent.GetInstance<YharimsGift>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("DraedonsHeart", false))
                ModContent.GetInstance<DraedonsHeart>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("Calamity", false))
                ModContent.GetInstance<CalamityMod.Items.Accessories.Calamity>().UpdateAccessory(player, hideVisual);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HeartoftheElements>()
                .AddIngredient<OccultSkullCrown>()
                .AddIngredient<Purity>()
                .AddIngredient<TheSponge>()
                .AddIngredient<ChaliceOfTheBloodGod>()
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