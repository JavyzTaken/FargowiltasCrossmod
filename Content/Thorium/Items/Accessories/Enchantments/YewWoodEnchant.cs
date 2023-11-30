using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using FargowiltasSouls;
using ThoriumMod.Items.ArcaneArmor;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class YewWoodEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.DarkGreen;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPLayer = player.ThoriumDLC();
            DLCPLayer.YewWoodEnch = true;
            DLCPLayer.YewWoodEnchItem = Item;
        }

        public override void Load()
        {
            LoadModdedAmmo();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<YewWoodHelmet>()
                .AddIngredient<YewWoodBreastguard>()
                .AddIngredient<YewWoodLeggings>()
                .AddIngredient<YewWoodFlintlock>()
                .AddIngredient<FalconeerCane>()
                .AddIngredient<ShadowflameStaff>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }

        public static void LoadModdedAmmo()
        {
            PreHMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.IcyArrow>());
            PreHMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.SteelArrow>());
            PreHMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.TalonArrowPro>());

            HMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.CrystalArrowPro>());
            HMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.DurasteelArrow>());
            HMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.GhostPulseArrowPro>());
            HMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.SpiritArrowPro>());

            HMBullets.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.IllumiteBullet>()); // add more
        }

        internal static readonly List<int> PreHMArrows = new()
        {
            ProjectileID.FireArrow,
            ProjectileID.UnholyArrow,
            ProjectileID.JestersArrow,
            ProjectileID.HellfireArrow,
            ProjectileID.BoneArrow,
            ProjectileID.FrostburnArrow,
            ProjectileID.BoneArrow,
            ProjectileID.ShimmerArrow,
        };

        internal static readonly List<int> HMArrows = new()
        {
            ProjectileID.HolyArrow,
            ProjectileID.CursedArrow,
            ProjectileID.IchorArrow,
            ProjectileID.VenomArrow,
            ProjectileID.ChlorophyteArrow,
        };

        internal static readonly List<int> PreHMBullets = new()
        {
            ProjectileID.SilverBullet,
            ProjectileID.PartyBullet,
            ProjectileID.MeteorShot
        };

        internal static readonly List<int> HMBullets = new()
        {
            ProjectileID.CrystalBullet,
            ProjectileID.IchorBullet,
            ProjectileID.CrystalBullet,
            ProjectileID.ExplosiveBullet,
            ProjectileID.GoldenBullet,
            ProjectileID.BulletHighVelocity,
            ProjectileID.ChlorophyteBullet,
            ProjectileID.NanoBullet,
            ProjectileID.MoonlordBullet
        };
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        /// <summary>
        /// from ModifyShootStats
        /// </summary>
        public void YewWoodEffect(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (item.useAmmo == AmmoID.Arrow && type == ProjectileID.WoodenArrowFriendly)
            {
                type = Main.rand.NextFromCollection(Main.hardMode ? Items.Accessories.Enchantments.YewWoodEnchant.HMArrows : Items.Accessories.Enchantments.YewWoodEnchant.PreHMArrows);
                Item arrow = new(type);
                damage += arrow.damage - 5;
            }

            if (item.useAmmo == AmmoID.Bullet && type == ProjectileID.Bullet && Player.FargoSouls().ForceEffect(YewWoodEnchItem.type))
            {
                type = Main.rand.NextFromCollection(Main.hardMode ? Items.Accessories.Enchantments.YewWoodEnchant.HMBullets : Items.Accessories.Enchantments.YewWoodEnchant.PreHMBullets);
                Item bullet = new(type);
                damage += bullet.damage - 7;
            }
        }
    }
}