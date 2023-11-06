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
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SpiritTrapperEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Red;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.SpiritTrapperEnch = true;
            DLCPlayer.SpiritTrapperEnchItem = Item;
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium.Items
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SpiritTrapperGlobalItem : GlobalItem
    {
        public override void UseAnimation(Item item, Player player)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.soulEssenceHit = false;
        }
    }
}