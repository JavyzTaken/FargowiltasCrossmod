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
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class EbonEnchant : BaseEnchant
    {
        public override Color nameColor => Color.Purple;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<EbonHeartEffect>(Item);
            player.AddEffect<EbonSynEffect>(Item);
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

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class EbonHeartEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.AlfheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<EbonEnchant>();

        public override void PostUpdateEquips(Player player)
        {
            var thoriumPlayer = player.Thorium();
            thoriumPlayer.darkAura = true;

            if (thoriumPlayer.totalHealingDarkHeart > 40)
            {
                Projectile.NewProjectile(GetSource_EffectItem(player), player.Center - 50f * Vector2.UnitY, Vector2.Zero, ModContent.ProjectileType<ThoriumMod.Projectiles.Healer.DarkHeartPro>(), 40, 5f, player.whoAmI, 0f, 0f);
                thoriumPlayer.totalHealingDarkHeart = 0;
            }

            player.GetDamage(DamageClass.Generic) += 0.05f * thoriumPlayer.healBonus;
        }
    }
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class EbonSynEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.AlfheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<EbonEnchant>();
    }
}