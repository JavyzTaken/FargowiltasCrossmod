using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.ThoriumMod.Name)]
    public class DreadEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.DarkOliveGreen;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.DreadEnch = true;
            DLCPlayer.DreadEnchItem = Item;

            DreadEffect(player, Item);
        }

        public static void DreadEffect(Player player, Item item)
        {
            int headType = ModContent.ProjectileType<DragonMinionHead>();

            if (player.ownedProjectileCounts[headType] != 1)
            {
                player.KillOwnedProjectilesOfType(headType);

                Projectile.NewProjectile(player.GetSource_Accessory(item), player.Center, Vector2.Zero, headType, 45, 0.2f, player.whoAmI);
            }
        }
    }
}