using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class WarlockEnchant : BaseSynergyEnchant
    {
        protected override Color nameColor => Color.DarkGray;
        protected override bool SynergyActive
        {
            get
            {
                var DLCPlayer = Main.LocalPlayer.GetModPlayer<CrossplayerThorium>();
                return DLCPlayer.WarlockEnchItem == Item && DLCPlayer.SacredEnch;
            }
        }
        protected override Color SynergyColor1 => Color.Black with { A = 0 };
        protected override Color SynergyColor2 => Color.Orange with { A = 0 };

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.WarlockEnch = true;
            DLCPlayer.WarlockEnchItem = Item;
        }
    }
}