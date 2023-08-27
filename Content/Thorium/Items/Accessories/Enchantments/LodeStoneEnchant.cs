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
    public class LodeStoneEnchant : BaseEnchant
    {
        
        protected override Color nameColor => Color.Brown;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.whoAmI != Main.myPlayer) return;

            var modplayer = player.GetModPlayer<CrossplayerThorium>();
            modplayer.LodeStoneEnch = true;

            int maxPlatforms = player.GetModPlayer<FargowiltasSouls.Core.ModPlayers.FargoSoulsPlayer>().WizardEnchantActive ? 3 : 2;
            int currentPlatforms = player.ownedProjectileCounts[ModContent.ProjectileType<LodeStonePlatform>()];
            if (currentPlatforms != maxPlatforms)
            {
                modplayer.LodeStonePlatforms = new();
                for (int i = 0; i < maxPlatforms; i++)
                {
                    modplayer.LodeStonePlatforms.Add(Projectile.NewProjectile(new EntitySource_ItemUse(player, Item),
                                                                              player.Center,
                                                                              Vector2.Zero,
                                                                              ModContent.ProjectileType<LodeStonePlatform>(),
                                                                              0,
                                                                              0,
                                                                              player.whoAmI,
                                                                              i * (2 * MathF.PI / maxPlatforms)));
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThoriumMod.Items.Lodestone.LodeStoneFaceGuard>()
                .AddIngredient<ThoriumMod.Items.Lodestone.LodeStoneChestGuard>()
                .AddIngredient<ThoriumMod.Items.Lodestone.LodeStoneShinGuards>()
                .Register();
        }
    }
}
