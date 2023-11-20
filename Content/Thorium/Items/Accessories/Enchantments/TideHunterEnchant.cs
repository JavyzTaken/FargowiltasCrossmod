using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static FargowiltasCrossmod.Core.ModCompatibility;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasCrossmod.Content.Thorium.Projectiles;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class TideHunterEnchant : BaseEnchant
    {
        protected override Color nameColor => new(0, 85, 85);
        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.TideHunterEnch = true;
            DLCPlayer.TideHunterEnchItem = Item;
            TideHunterEffect(player);
        }

        public static void TideHunterEffect(Player player)
        {
            var DLCPlayer = player.ThoriumDLC();
            if (!player.FargoSouls().IsInADashState) return;

            if (!DLCPlayer.WasInDashState)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(DLCPlayer.TideHunterEnchItem), player.Center, player.velocity, ModContent.ProjectileType<Projectiles.TideTurnerWave>(), 125, 3f, player.whoAmI, 6);
            }
        }
    }
}

