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
using CalamityMod.NPCs.Crags;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamitySoul : BaseSoul
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ModContent.RarityType<Violet>();
        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ModContent.GetInstance<DesolationForce>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<AnnihilationForce>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<DevastationForce>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<ExaltationForce>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<ExplorationForce>().UpdateAccessory(player, hideVisual);
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient<DesolationForce>()
                .AddIngredient<AnnihilationForce>()
                .AddIngredient<DevastationForce>()
                .AddIngredient<ExaltationForce>()
                .AddIngredient<ExplorationForce>()
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
