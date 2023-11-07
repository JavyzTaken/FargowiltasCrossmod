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
using ThoriumMod.Items.SummonItems;
using ThoriumMod.Items.ThrownItems;
using ThoriumMod.Items.Consumable;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class LivingWoodEnchant : BaseEnchant
    {
        
        protected override Color nameColor => Color.Brown;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.ThoriumDLC();
            modPlayer.LivingWoodEnch = true;
            modPlayer.LivingWoodEnchItem = Item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LivingWoodMask>()
                .AddIngredient<LivingWoodChestguard>()
                .AddIngredient<LivingWoodBoots>()
                .AddIngredient<LivingWoodAcorn>()
                .AddIngredient<ChiTea>(5)
                .AddIngredient<Wreath>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }

        public static void KillLivingWoodRoots(int owner)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == ModContent.ProjectileType<LivingWood_Roots>() && proj.owner == owner)
                {
                    proj.Kill();
                }
            }
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void LivingWoodKey()
        {
            if (!LivingWoodEnch || LivingWoodEnchItem == null || Main.myPlayer != Player.whoAmI) return;

            if (!Player.HasBuff<LivingWood_Root_DB>() && !Player.HasBuff<LivingWood_Root_B>())
            {
                Player.ClearBuff(ModContent.BuffType<LivingWood_Root_B>());
                LivingWoodEnchant.KillLivingWoodRoots(Player.whoAmI);

                Player.AddBuff(ModContent.BuffType<LivingWood_Root_DB>(), 1200);
                Player.AddBuff(ModContent.BuffType<LivingWood_Root_B>(), 300);

                Projectile.NewProjectile(Player.GetSource_Misc(""),
                                         Player.position,
                                         Vector2.Zero,
                                         ModContent.ProjectileType<LivingWood_Roots>(),
                                         0,
                                         0,
                                         Player.whoAmI);
            }
            else
            {
                Player.ClearBuff(ModContent.BuffType<LivingWood_Root_B>());
                LivingWoodEnchant.KillLivingWoodRoots(Player.whoAmI);
            }
        }
    }
}
