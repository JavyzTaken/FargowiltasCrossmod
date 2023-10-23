using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Forces
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class HelheimForce : BaseForce
    {
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();

            DLCPlayer.HelheimForce = true;

            DLCPlayer.SpiritTrapperEnch = true;
            DLCPlayer.SpiritTrapperEnchItem = Item;
            SpiritTrapperEnchant.SpiritTrapperEffect(player, Item);

            DLCPlayer.DragonEnch = true;
            DLCPlayer.DragonEnchItem = Item;
            DragonEnchant.DragonEffect(player);

            DLCPlayer.DreadEnch = true;
            DLCPlayer.DreadEnchItem = Item;
            DreadEnchant.DreadEffect(player, Item);

            DLCPlayer.WhiteKnightEnch = true;
            WhiteKnightEnchant.WhiteKnightEffect(player);

            DLCPlayer.SilkEnch = true; 
            var ThoriumPlayer = player.GetModPlayer<ThoriumMod.ThoriumPlayer>();
            ThoriumPlayer.accArtificersShield = true;
            player.statDefense += 2 * (ThoriumPlayer.statEnchantersEnergy / 10);
            ThoriumPlayer.accArtificersFocus = true;

            DLCPlayer.FleshEnch = true;
            DLCPlayer.FleshEnchItem = Item;
            DLCPlayer.DemonBloodEnch = true;
            DLCPlayer.DemonBloodEnchItem = Item;
            DemonBloodEnchant.DemonBloodEffect(player);
        }
    }
}