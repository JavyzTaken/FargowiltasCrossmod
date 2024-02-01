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
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class YewWoodEnchant : BaseEnchant
    {
        public override Color nameColor => Color.DarkGreen;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<YewWoodEffect>(Item);
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

    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class YewWoodEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.MuspelheimHeader>();

        public override void Load()
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
        public void YewWoodShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (item.useAmmo == AmmoID.Arrow && type == ProjectileID.WoodenArrowFriendly)
            {
                type = Main.rand.NextFromCollection(Main.hardMode ? YewWoodEffect.HMArrows : YewWoodEffect.PreHMArrows);
                Item arrow = new(type);
                damage += arrow.damage - 5;
            }

            if (item.useAmmo == AmmoID.Bullet && type == ProjectileID.Bullet && Player.ForceEffect<YewWoodEffect>())
            {
                type = Main.rand.NextFromCollection(Main.hardMode ? YewWoodEffect.HMBullets : YewWoodEffect.PreHMBullets);
                Item bullet = new(type);
                damage += bullet.damage - 7;
            }
        }
    }
}