using Terraria;
using Terraria.ModLoader;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DragonEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Green;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modplayer = player.ThoriumDLC();
            modplayer.DragonEnch = true;
            modplayer.DragonEnchItem = Item;

            DragonEffect(player);
        }

        public static void DragonEffect(Player player)
        {
            int numHeads = player.FargoSouls().ForceEffect(ModContent.ItemType<DragonEnchant>()) ? 2 : 1;
            int projType = ModContent.ProjectileType<DragonHead>();

            if (player.ownedProjectileCounts[projType] != numHeads)
            {
                player.KillOwnedProjectilesOfType(projType);

                var DLCPlayer = player.ThoriumDLC();
                if (numHeads == 1)
                {
                    Projectile.NewProjectile(player.GetSource_Accessory(DLCPlayer.DragonEnchItem), player.Center, Vector2.Zero, projType, 0, 0, player.whoAmI, 0);
                }
                else
                {
                    Projectile.NewProjectile(player.GetSource_Accessory(DLCPlayer.DragonEnchItem), player.Center, Vector2.Zero, projType, 0, 0, player.whoAmI, 1);
                    Projectile.NewProjectile(player.GetSource_Accessory(DLCPlayer.DragonEnchItem), player.Center, Vector2.Zero, projType, 0, 0, player.whoAmI, 2);
                }
            }
        } 

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThoriumMod.Items.Dragon.DragonMask>()
                .AddIngredient<ThoriumMod.Items.Dragon.DragonBreastplate>()
                .AddIngredient<ThoriumMod.Items.Dragon.DragonGreaves>()
                .Register();
        }
    }
}