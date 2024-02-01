using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GeodeEnchant : BaseEnchant
    {
        public override Color nameColor => Color.LightPink;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<GeodeEffect>(Item);

            var modPlayer = player.FargoSouls();
            float speed = modPlayer.ForceEffect<GeodeEnchant>() ? .75f : .5f;
            MinerEnchant.AddEffects(player, speed, Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MinerEnchant>()
                .Register();
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GeodeEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<FargowiltasSouls.Core.Toggler.Content.WorldShaperHeader>();
        public override int ToggleItemType => ModContent.ItemType<GeodeEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            player.Thorium().setGeode = true;
        }
    }
}