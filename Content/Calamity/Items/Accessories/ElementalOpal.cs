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
using CalamityMod.Items.Materials;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories
{
    //no good name or unique affect yet
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ElementalOpal : ModItem
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().ElementalOpalEffects(player, hideVisual);
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient<AeroStone>()
                .AddIngredient<CryoStone>()
                .AddIngredient<ChaosStone>()
                .AddIngredient<BloomStone>()
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
    
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void ElementalOpalEffects(Player player, bool hideVisual)
        {
            if (player.GetToggleValue("AeroStone"))
                ModContent.GetInstance<AeroStone>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("CryoStone"))
                ModContent.GetInstance<CryoStone>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("ChaosStone"))
                ModContent.GetInstance<ChaosStone>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("BloomStone"))
                ModContent.GetInstance<BloomStone>().UpdateAccessory(player, hideVisual);
        }
    }
}
