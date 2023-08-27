using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using ThoriumMod;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using FargowiltasSouls.Common.Utilities;
using ThoriumMod.Items.Donate;
using ThoriumMod.Items.HealerItems;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments // shortest crossmod namespace
{
    [ExtendsFromMod("ThoriumMod")]
    public class TemplarEnchant : BaseEnchant
    {
        public override string wizardEffect => "TBD";
        protected override Color nameColor => Color.PaleVioletRed;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.GetModPlayer<CrossplayerThorium>();
            modPlayer.TemplarEnch = true;
            modPlayer.TemplarEnchItem = Item;

            if (modPlayer.TemplarCD > 0)
            {
                modPlayer.TemplarCD--;
            }
        }

        public static void summonHolyFire(Player player)
        {
            var modPlayer = player.GetModPlayer<CrossplayerThorium>();
            Projectile.NewProjectile(player.GetSource_Accessory(modPlayer.TemplarEnchItem),
                                     Main.MouseWorld.X,
                                     player.Center.Y - 500,
                                     0f,
                                     10f,
                                     ModContent.ProjectileType<Projectiles.Templar_Fire>(),
                                     FargoSoulsUtil.HighestDamageTypeScaling(player, 20),
                                     0f,
                                     player.whoAmI,
                                     Main.MouseWorld.X,
                                     Main.MouseWorld.Y);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<TemplarsCirclet>(), 1);
            recipe.AddIngredient(ModContent.ItemType<TemplarsTabard>(), 1);
            recipe.AddIngredient(ModContent.ItemType<TemplarsLeggings>(), 1);
            recipe.AddIngredient(ModContent.ItemType<TemplarJudgment>(), 1);
            recipe.AddIngredient(ModContent.ItemType<Recuperate>(), 1);
            recipe.AddIngredient(ModContent.ItemType<RichLeaf>(), 1);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}
