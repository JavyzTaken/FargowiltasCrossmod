using Terraria;
using Terraria.ModLoader;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using FargowiltasSouls;

using Terraria.DataStructures;
using System;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class LodestoneEnchant : BaseEnchant
    {
        public override Color nameColor => Color.Brown;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<LodestoneEffect>(Item);
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

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class LodestoneEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.MidgardHeader>();
        public override int ToggleItemType => ModContent.ItemType<LodestoneEnchant>();

        public override void PostUpdate(Player player)
        {
            if (player.whoAmI != Main.myPlayer) return;
            var DLCPlayer = player.ThoriumDLC();

            int maxPlatforms = player.ForceEffect<LodestoneEffect>() ? 3 : 2;
            int currentPlatforms = player.ownedProjectileCounts[ModContent.ProjectileType<LodeStonePlatform>()];
            if (currentPlatforms != maxPlatforms)
            {
                DLCPlayer.LodeStonePlatforms = new();
                for (int i = 0; i < maxPlatforms; i++)
                {
                    DLCPlayer.LodeStonePlatforms.Add(Projectile.NewProjectile(GetSource_EffectItem(player),
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
    }
}
