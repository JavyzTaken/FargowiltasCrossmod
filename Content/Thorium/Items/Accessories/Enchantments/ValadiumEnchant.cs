using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class ValadiumEnchant : BaseEnchant
    {
        public override Color nameColor => Color.Purple;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;


        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.ThoriumDLC();
            modPlayer.ValadiumEnch = true;
            modPlayer.ValadiumEnchItem = Item;

            if (modPlayer.ValadiumCD > 0)
            {
                modPlayer.ValadiumCD--;
            }
            else if (player.ownedProjectileCounts[ModContent.ProjectileType<Valadium_Chunk>()] < 20)
            {
                modPlayer.ValadiumCD = 120;
                SummonChunk(player);
            }
        }

        public static void SummonChunk(Player player)
        {
            //Main.NewText("chunk spawned");
            var modPlayer = player.ThoriumDLC();
            float oneOnSqrt2 = 0.707106781187f;
            // doing this gives an elipse that surrounds the edge of the screen.
            Vector2 spawnPos = Main.rand.NextVector2CircularEdge(oneOnSqrt2 * Main.screenWidth, oneOnSqrt2 * Main.screenHeight);
            Projectile.NewProjectile(player.GetSource_Accessory(modPlayer.ValadiumEnchItem),
                                     spawnPos + player.Center,
                                     Main.rand.NextVector2Circular(4, 4),
                                     ModContent.ProjectileType<Valadium_Chunk>(),
                                     50,
                                     3,
                                     player.whoAmI,
                                     Main.rand.Next(1, 4));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThoriumMod.Items.Valadium.ValadiumHelmet>()
                .AddIngredient<ThoriumMod.Items.Valadium.ValadiumBreastPlate>()
                .AddIngredient<ThoriumMod.Items.Valadium.ValadiumGreaves>()
                .Register();
        }
    }
}