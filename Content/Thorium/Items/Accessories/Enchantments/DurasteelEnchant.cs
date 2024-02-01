using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DurasteelEnchant : BaseEnchant
    {
        public override Color nameColor => Color.Gray;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SteelEffect>(Item);
            player.AddEffect<DarksteelEffect>(Item);
            player.AddEffect<DurasteelEffect>(Item);
            player.ThoriumDLC().SteelTeir = 3;
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DurasteelEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.SvartalfheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<DurasteelEnchant>();
    }
}