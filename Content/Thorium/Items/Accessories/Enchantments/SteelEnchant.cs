﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SteelEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.DarkGray;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.SteelEnch = true;
            DLCPlayer.SteelEnchItem = Item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThoriumMod.Items.Steel.SteelHelmet>()
                .AddIngredient<ThoriumMod.Items.Steel.SteelChestplate>()
                .AddIngredient<ThoriumMod.Items.Steel.SteelGreaves>()
                .Register();
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void ParryKey()
        {
            if (SteelEnchItem == null || Main.myPlayer != Player.whoAmI || Player.dead || !Player.active) return;
            int teir = DuraSteelEnch ? 3 : (DarkSteelEnch ? 2 : (SteelEnch ? 1 : 0));

            if (Player.GetModPlayer<FargowiltasSouls.Core.ModPlayers.FargoSoulsPlayer>().ForceEffect(SteelEnchItem.type)) teir++;

            if (teir == 0) return;

            if (!Player.HasBuff<SteelParry_CD>())
            {
                Player.AddBuff(ModContent.BuffType<SteelParry_CD>(), 900);

                float rot = Player.Center.DirectionTo(Main.MouseWorld).ToRotation();
                Projectile.NewProjectile(Player.GetSource_Accessory(SteelEnchItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<SteelParry>(), 0, 0, Player.whoAmI, teir, rot);
            }
        }
    }
}