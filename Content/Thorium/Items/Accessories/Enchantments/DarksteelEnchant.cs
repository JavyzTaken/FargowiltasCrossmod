using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DarksteelEnchant : SteelEnchant
    {
        public override Color nameColor => Color.DarkRed;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SteelEffect>(Item);
            player.AddEffect<DarksteelEffect>(Item);
            player.ThoriumDLC().SteelTeir = 2;
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

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DarksteelEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.SvartalfheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<DarksteelEnchant>();
    }
}
