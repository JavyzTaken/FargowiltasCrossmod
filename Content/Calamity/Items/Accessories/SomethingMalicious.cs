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
    public class SomethingMalicious : ModItem
    {
        //fnny placeholder name & sprite dont kill me
        //probably revert to normal placeholder sprite and change item name on release
        //public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 44;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().MalEffects(player, hideVisual);
            
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient<EvasionScarf>()
                .AddIngredient<Regenator>()
                .AddIngredient<TheTransformer>()
                .AddIngredient<FlameLickedShell>()
                .AddIngredient<DeepDiver>()
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void MalEffects(Player player, bool hideVisual)
        {
            if (player.GetToggleValue("EvasionScarf"))
                ModContent.GetInstance<EvasionScarf>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("Regenator"))
                ModContent.GetInstance<Regenator>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("TheTransformer"))
                ModContent.GetInstance<TheTransformer>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("FlameLickedShell"))
                ModContent.GetInstance<FlameLickedShell>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("DeepDiver"))
                ModContent.GetInstance<DeepDiver>().UpdateAccessory(player, hideVisual);
        }
    }
}
