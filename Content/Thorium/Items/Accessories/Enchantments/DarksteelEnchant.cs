using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.AccessoryEffectSystem;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DarksteelEnchant : SteelEnchant
    {
        public override Color nameColor => Color.DarkRed;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.ThoriumDLC().SteelTeir = 2;
            player.AddEffect<SteelEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThoriumMod.Items.Darksteel.DarksteelFaceGuard>()
                .AddIngredient<ThoriumMod.Items.Darksteel.DarksteelBreastPlate>()
                .AddIngredient<ThoriumMod.Items.Darksteel.DarksteelGreaves>()
                .Register();
        }
    }
}
