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

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories
{
    public class AncestralCharm : ModItem
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 5;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().AncestralCharmEffects(player, hideVisual);
            player.GetModPlayer<CrossplayerCalamity>().AncestralCharm = true;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient<TrinketofChi>()
                .AddIngredient<LuxorsGift>()
                .AddIngredient<GladiatorsLocket>()
                .AddIngredient<FungalSymbiote>()
                .AddIngredient<UnstableGraniteCore>()
                .AddIngredient<DeviatingEnergy>(10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
    
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void AncestralCharmEffects(Player player, bool hideVisual)
        {
            if (player.GetToggleValue("TrinketofChi"))
                ModContent.GetInstance<TrinketofChi>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("LuxorsGift"))
                ModContent.GetInstance<LuxorsGift>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("GladiatorsLocket"))
                ModContent.GetInstance<GladiatorsLocket>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("FungalSymbiote"))
                ModContent.GetInstance<FungalSymbiote>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("UnstableGraniteCore"))
                ModContent.GetInstance<UnstableGraniteCore>().UpdateAccessory(player, hideVisual);
        }
    }
}
