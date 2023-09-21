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

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls
{
    public class TyrantSoul : BaseSoul
    {
        //public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ModContent.RarityType<Violet>();
            Item.defense = 60;
        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().AncientsSoulEffect(player, hideVisual);
            player.GetModPlayer<CrossplayerCalamity>().AncestralCharmEffects(player, hideVisual);
            player.GetModPlayer<CrossplayerCalamity>().ElementalOpalEffects(player, hideVisual);
            player.GetModPlayer<CrossplayerCalamity>().VoidIconEffects(player, hideVisual);
            player.GetModPlayer<CrossplayerCalamity>().PlastralHideEffects(player, hideVisual);
            player.GetModPlayer<CrossplayerCalamity>().PolarThingEffects(player, hideVisual);
            player.GetModPlayer<CrossplayerCalamity>().MalEffects(player, hideVisual);
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
                .AddIngredient<AncientsSoul>()
                .AddIngredient<AncestralCharm>()
                .AddIngredient<ElementalOpal>()
                .AddIngredient<VoidIcon>()
                .AddIngredient<PlastralHide>()
                .AddIngredient<PolarThing>()
                .AddIngredient<SomethingMalicious>()
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
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        
    }
}
