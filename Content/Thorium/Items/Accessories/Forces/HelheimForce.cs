using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;

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

            DLCPlayer.SpiritTrapperEnch = true;
            DLCPlayer.SpiritTrapperEnchItem = Item;

            DLCPlayer.DragonEnch = true;
            DLCPlayer.DragonEnchItem = Item;
            DragonEnchant.DragonEffect(player);

            DLCPlayer.DreadEnch = true;
            DLCPlayer.DreadEnchItem = Item;
            DreadEnchant.DreadEffect(player, Item);

            DLCPlayer.WhiteKnightEnch = true;
            WhiteKnightEnchant.WhiteKnightEffect(player);

            DLCPlayer.SilkEnch = true; 

            DLCPlayer.FleshEnch = true;
            DLCPlayer.FleshEnchItem = Item;
            DLCPlayer.DemonBloodEnch = true;
            DLCPlayer.DemonBloodEnchItem = Item;
            DemonBloodEnchant.DemonBloodEffect(player);
        }
    }
}