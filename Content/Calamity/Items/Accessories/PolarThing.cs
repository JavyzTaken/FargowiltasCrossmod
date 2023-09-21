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
    //no good name or unique affect yet
    public class PolarThing : ModItem
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 7;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().PolarThingEffects(player, hideVisual);
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient<OceanCrest>()
                .AddIngredient<FungalClump>()
                .AddIngredient<BloodyWormTooth>()
                .AddIngredient<RottenBrain>()
                .AddIngredient<ManaPolarizer>()
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
        public void PolarThingEffects(Player player, bool hideVisual)
        {
            if (player.GetToggleValue("OceanCrest"))
                ModContent.GetInstance<OceanCrest>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("FungalClump"))
                ModContent.GetInstance<FungalClump>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("BloodyWormTooth"))
                ModContent.GetInstance<BloodyWormTooth>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("RottenBrain"))
                ModContent.GetInstance<RottenBrain>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("ManaPolarizer"))
                ModContent.GetInstance<ManaPolarizer>().UpdateAccessory(player, hideVisual);
        }
    }
}
