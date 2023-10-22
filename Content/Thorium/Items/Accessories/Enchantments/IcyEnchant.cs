using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static FargowiltasCrossmod.Core.ModCompatibility;
using System.Numerics;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod("ThoriumMod")]
    public class IcyEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.DarkBlue;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.IcyEnch = true;
            DLCPlayer.IcyEnchItem = Item;
        }

        //public override void AddRecipes()
        //{
        //    CreateRecipe()
        //        .Register();
        //}
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
}
