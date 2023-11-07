using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SacredEnchant : BaseSynergyEnchant
    {
        protected override Color nameColor => Color.Orange;
        internal override bool SynergyActive(CrossplayerThorium DLCPlayer) => DLCPlayer.SacredEnch && DLCPlayer.WarlockEnch;

        internal override int SynergyEnch => ModContent.ItemType<WarlockEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.SacredEnch = true;
            DLCPlayer.SacredEnchItem = Item;
        }

    }
}