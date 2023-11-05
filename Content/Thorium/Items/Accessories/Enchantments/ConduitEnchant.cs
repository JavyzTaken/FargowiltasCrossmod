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
    public class ConduitEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.LightSteelBlue;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.ConduitEnch = true;
            DLCPlayer.ConduitEnchItem = Item;
            DLCPlayer.AstroEnch = true;
            DLCPlayer.AstroEnchItem = Item;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.ConduitSaucer>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.ConduitSaucer>(), 0, 0, player.whoAmI);
            }
        }
    }
}
