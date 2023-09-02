using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod("ThoriumMod")]
    public class SpiritTrapperEnchant : BaseEnchant
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        protected override Color nameColor => Color.DarkBlue;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.SpiritTrapperEnch = true;
            DLCPlayer.SpiritTrapperEnchItem = Item;
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void SpawnSpiritTrapperSpirit(Vector2 position)
        {
            Projectile.NewProjectile(Player.GetSource_Accessory(SpiritTrapperEnchItem), position, Vector2.Zero, ModContent.ProjectileType<Projectiles.SpiritTrapperSpirit>(), 0, 0, Player.whoAmI);
        }
    }
}
