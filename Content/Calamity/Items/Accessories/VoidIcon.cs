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
using CalamityMod.Rarities;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories
{
    //no good name or unique affect yet
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class VoidIcon : ModItem
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().VoidIconEffects(player, hideVisual);
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient<WarbanneroftheSun>()
                .AddIngredient<BlazingCore>()
                .AddIngredient<SpectralVeil>()
                .AddIngredient<TheEvolution>()
                .AddIngredient<Affliction>()
                .AddIngredient<CosmiliteBar>(10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
    
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void VoidIconEffects(Player player, bool hideVisual)
        {
            if (player.GetToggleValue("WarbanneroftheSun"))
                ModContent.GetInstance<WarbanneroftheSun>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("BlazingCore"))
                ModContent.GetInstance<BlazingCore>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("SpectralVeil"))
                ModContent.GetInstance<SpectralVeil>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("TheEvolution"))
                ModContent.GetInstance<TheEvolution>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("Affliction"))
                ModContent.GetInstance<Affliction>().UpdateAccessory(player, hideVisual);
        }
    }
}
