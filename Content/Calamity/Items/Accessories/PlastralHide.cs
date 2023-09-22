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
using Terraria.Localization;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories
{
    //no good name or unique affect yet
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class PlastralHide : ModItem
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.defense = 32;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().PlastralHideEffects(player, hideVisual);
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient<FrostFlare>()
                .AddIngredient<AquaticEmblem>()
                .AddIngredient<VoidofExtinction>()
                .AddIngredient<LeviathanAmbergris>()
                .AddIngredient<GravistarSabaton>()
                .AddIngredient<ToxicHeart>()
                .AddIngredient<HideofAstrumDeus>()
                .AddIngredient(ItemID.LunarBar, 10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
    
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void PlastralHideEffects(Player player, bool hideVisual)
        {
            if (player.GetToggleValue("FrostFlare"))
                ModContent.GetInstance<FrostFlare>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("AquaticEmblem"))
                ModContent.GetInstance<AquaticEmblem>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("VoidofExtinction"))
                ModContent.GetInstance<VoidofExtinction>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("LeviathanAmbergris"))
                ModContent.GetInstance<LeviathanAmbergris>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("GravistarSabaton"))
                ModContent.GetInstance<GravistarSabaton>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("ToxicHeart"))
                ModContent.GetInstance<ToxicHeart>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("HideofAstrumDeus"))
                ModContent.GetInstance<HideofAstrumDeus>().UpdateAccessory(player, hideVisual);
        }
    }
}
