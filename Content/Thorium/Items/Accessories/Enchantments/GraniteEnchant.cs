using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GraniteEnchant : BaseEnchant
    {
        public override Color nameColor => Color.DarkBlue;
        //internal override bool SynergyActive(CrossplayerThorium DLCPlayer) => DLCPlayer.GraniteEnch && DLCPlayer.BronzeEnch;

        //protected override Color SynergyColor1 => Color.DarkBlue with { A = 0 };
        //protected override Color SynergyColor2 => Color.Gold with { A = 0 };
        //internal override int SynergyEnch => ModContent.ItemType<BronzeEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<GraniteEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThoriumMod.Items.Granite.GraniteHelmet>()
                .AddIngredient<ThoriumMod.Items.Granite.GraniteChestGuard>()
                .AddIngredient<ThoriumMod.Items.Granite.GraniteGreaves>()
                .Register();
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GraniteEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.SvartalfheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<GraniteEnchant>();
        public override bool ExtraAttackEffect => true;

        public override void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Damage >= target.life && player.whoAmI == Main.myPlayer) 
            {
                if (player.HasEffect<BronzeEffect>() && !player.ForceEffect<GraniteEffect>()) return;

                if (proj.type != ModContent.ProjectileType<GraniteExplosion>() || !Main.rand.NextBool(3))
                {
                    Projectile.NewProjectileDirect(GetSource_EffectItem(player), target.Center, Vector2.Zero, ModContent.ProjectileType<GraniteExplosion>(), 0, 0f, player.whoAmI, 1f);
                }
            }
        }
    }
}