using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DurasteelEnchant : BaseEnchant
    {
        public override Color nameColor => Color.Gray;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.ThoriumDLC().SteelTeir = 3;
            player.AddEffect<SteelEffect>(Item);
        }
    }
}