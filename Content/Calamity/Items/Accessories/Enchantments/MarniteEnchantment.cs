using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.MarniteArchitect;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class MarniteEnchantment : BaseEnchant
    {
        public override Color nameColor => new Color(153, 200, 193);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 10);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<MarniteStatsEffect>(Item);
            player.AddEffect<MarniteLasersEffect>(Item);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<MarniteArchitectHeadgear>());
            recipe.AddIngredient(ModContent.ItemType<MarniteArchitectToga>());
            recipe.AddIngredient(ModContent.ItemType<MarniteRepulsionShield>());
            recipe.AddIngredient(ModContent.ItemType<UnstableGraniteCore>());
            recipe.AddIngredient(ModContent.ItemType<GladiatorsLocket>());
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class MarniteStatsEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ExplorationHeader>();
        public override int ToggleItemType => ModContent.ItemType<MarniteEnchantment>();
        public override void PostUpdateEquips(Player player)
        {
            player.tileSpeed += 0.1f;
            player.blockRange += 5;
            if (player.ForceEffect<MarniteStatsEffect>())
            {
                player.tileSpeed += 0.15f;
                player.blockRange += 5;
            }
        }

    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class MarniteLasersEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ExplorationHeader>();
        public override int ToggleItemType => ModContent.ItemType<MarniteEnchantment>();
        public static void MarniteTileEffect(Player player, Vector2 worldPos)
        {
            int n = FargoSoulsUtil.FindClosestHostileNPC(worldPos, 500);
            if (n >= 0)
            {
                if (Main.rand.NextBool(5)) {
                    NPC target = Main.npc[n];
                    Vector2 vel = (target.Center - (worldPos - new Vector2(8, 8))).SafeNormalize(Vector2.Zero) * 2;
                    float damage = 10;
                    damage += player.HeldItem.pick / 10;
                    if (player.ForceEffect<MarniteLasersEffect>()) damage *= 2.5f;
                    int index = Projectile.NewProjectile(player.GetSource_EffectItem<MarniteLasersEffect>(), worldPos - new Vector2(8, 8), vel, ModContent.ProjectileType<MarniteLaser>(), (int)damage, 1, player.whoAmI);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, index);
                }
            }
        }
    }
}
