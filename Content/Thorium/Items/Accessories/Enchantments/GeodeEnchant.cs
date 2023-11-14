using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GeodeEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.LightPink;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            var ThoriumPlayer = player.Thorium();

            DLCPlayer.GeodeEnch = true;
            ThoriumPlayer.setGeode = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MinerEnchant>()
                .Register();
        }
    }
}