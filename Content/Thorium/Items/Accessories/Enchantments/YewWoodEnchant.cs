using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod("ThoriumMod")]
    public class YewWoodEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.DarkGreen;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerThorium>().YewWoodEnch = true;
        }

        public static void LoadModdedArrows()
        {
            PreHMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.IcyArrow>());
            PreHMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.SteelArrow>());
            PreHMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.TalonArrowPro>());

            HMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.CrystalArrowPro>());
            HMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.DurasteelArrow>());
            HMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.GhostPulseArrowPro>());
            HMArrows.Add(ModContent.ProjectileType<ThoriumMod.Projectiles.SpiritArrowPro>());
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
            ProjectileID.ShimmerArrow
        };

        internal static readonly List<int> HMArrows = new()
        {
            ProjectileID.HolyArrow,
            ProjectileID.CursedArrow,
            ProjectileID.IchorArrow,
            ProjectileID.VenomArrow,
            ProjectileID.ChlorophyteArrow,
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
                bool useHM = Main.hardMode; // || wizard
                type = Main.rand.NextFromCollection(useHM ? Items.Accessories.Enchantments.YewWoodEnchant.HMArrows : Items.Accessories.Enchantments.YewWoodEnchant.PreHMArrows);
            }
        }
    }
}