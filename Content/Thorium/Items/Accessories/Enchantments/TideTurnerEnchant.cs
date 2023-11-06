using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class TideTurnerEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.SkyBlue;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.TideTurnerEnch = true;
            DLCPlayer.TideTurnerEnchItem = Item;

            TideTurnerEffect(player);
        }

        public static void TideTurnerEffect(Player player)
        {
            var DLCPlayer = player.ThoriumDLC();
            if (!player.FargoSouls().IsInADashState) return;

            if (!DLCPlayer.WasInDashState)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(DLCPlayer.TideTurnerEnchItem), player.Center, player.velocity, ModContent.ProjectileType<Projectiles.TideTurnerWave>(), 125, 3f, player.whoAmI, 6);
            }
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
    }
}