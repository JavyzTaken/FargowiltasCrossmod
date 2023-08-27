using Terraria;
using Terraria.ModLoader;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;

using Terraria.DataStructures;
using System;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod("ThoriumMod")]
    public class DragonEnchant : BaseEnchant
    {
        public override string wizardEffect => "";
        protected override Color nameColor => Color.Green;

        public override void SetStaticDefaults()
        {

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.whoAmI != Main.myPlayer) return;

            var modplayer = player.GetModPlayer<CrossplayerThorium>();
            modplayer.DragonEnch = true;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<DragonMinionHead>()] != 1)
            {
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item),
                                        player.Center,
                                        Vector2.Zero,
                                        ModContent.ProjectileType<DragonMinionHead>(),
                                        45,
                                        0,
                                        player.whoAmI);
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
