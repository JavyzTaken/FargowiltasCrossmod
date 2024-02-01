using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Forces
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class HelheimForce : BaseForce
    {

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();

            DLCPlayer.HelheimForce = true;

            player.AddEffect<SpiritTrapperEffect>(Item);
            player.AddEffect<DragonEffect>(Item);
            //player.AddEffect<DreadEffect>(Item);
            player.AddEffect<FleshEffect>(Item);
            player.AddEffect<DemonBloodEffect>(Item);
            player.AddEffect<SilkEffect>(Item);

            DLCPlayer.WhiteKnightEnch = true;
            WhiteKnightEnchant.WhiteKnightEffect(player);
        }
    }
}