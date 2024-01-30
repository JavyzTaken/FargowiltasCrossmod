using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Items.Donate;
using ThoriumMod.Items.BossMini;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class EbonEnchant : BaseEnchant
    {
        public override Color nameColor => Color.Purple;
        //internal override bool SynergyActive(CrossplayerThorium DLCPlayer) => DLCPlayer.EbonEnch && DLCPlayer.NoviceClericEnch;
        
        //protected override Color SynergyColor1 => Color.White with { A = 0 };
        //protected override Color SynergyColor2 => Color.Purple with { A = 0 };
        //internal override int SynergyEnch => ModContent.ItemType<NoviceClericEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.EbonEnch = true;
            DLCPlayer.EbonEnchItem = Item;
        }

        public static void EbonEffect(Player player, CrossplayerThorium DLCPlayer)
        {
            if (!DLCPlayer.EbonEnch) return;

            var thoriumPlayer = player.Thorium();
            thoriumPlayer.darkAura = true;

            if (thoriumPlayer.totalHealingDarkHeart > 40)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(DLCPlayer.EbonEnchItem), player.Center - 50f * Vector2.UnitY, Vector2.Zero, ModContent.ProjectileType<ThoriumMod.Projectiles.Healer.DarkHeartPro>(), 40, 5f, player.whoAmI, 0f, 0f);
                thoriumPlayer.totalHealingDarkHeart = 0;
            }
            player.GetDamage(DamageClass.Generic) += 0.05f * thoriumPlayer.healBonus;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EbonHood>()
                .AddIngredient<EbonCloak>()
                .AddIngredient<EbonLeggings>()
                .AddIngredient<LeechBolt>()
                .AddIngredient<GraveBuster>()
                .AddIngredient<DarkHeart>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}