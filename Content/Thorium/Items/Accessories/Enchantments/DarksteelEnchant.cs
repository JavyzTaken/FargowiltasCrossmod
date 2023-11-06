using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DarksteelEnchant : SteelEnchant
    {
        protected override Color nameColor => Color.DarkRed;

        public override void SetStaticDefaults()
        {

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.DarkSteelEnch = true;
            DLCPlayer.SteelEnchItem = Item;
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
