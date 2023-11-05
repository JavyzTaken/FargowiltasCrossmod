using System;
using System.Collections.Generic;
using System.Linq;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class FleshEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.IndianRed;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.FleshEnch = true;
            DLCPlayer.FleshEnchItem = Item;
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void SpawnFlesh(NPC target)
        {
            Item.NewItem(Player.GetSource_Accessory(FleshEnchItem), target.Hitbox, ModContent.ItemType<ThoriumMod.Items.Flesh.GreatFlesh>(), 1, false, 0, false, false);
        }
    }
}
