using Terraria;
using Terraria.ModLoader;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DragonEnchant : BaseEnchant
    {
        public override Color nameColor => Color.Green;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<DragonEffect>(Item);
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

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DragonEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.HelheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<DragonEnchant>();

        public override void PostUpdateEquips(Player player)
        {
            int numHeads = player.ForceEffect<DragonEffect>() ? 2 : 1;
            int projType = ModContent.ProjectileType<DragonHead>();

            if (player.ownedProjectileCounts[projType] != numHeads)
            {
                player.KillOwnedProjectilesOfType(projType);

                if (numHeads == 1)
                {
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, projType, 0, 0, player.whoAmI, 0);
                }
                else
                {
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, projType, 0, 0, player.whoAmI, 1);
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, projType, 0, 0, player.whoAmI, 2);
                }
            }
        }
    }
}