using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Thorium.Projectiles;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class WarlockEnchant : BaseSynergyEnchant<SacredEffect>
    {
        public override Color nameColor => Color.DarkGray;
        internal override int SynergyEnch => ModContent.ItemType<SacredEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<WarlockEffect>(Item);
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class WarlockEffect : SynergyEffect<SacredEffect>
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.AlfheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<WarlockEnchant>();
        public override bool MinionEffect => true;

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            bool synergy = SynergyActive(player);
            if (synergy) return;

            if (hitInfo.Crit)
            {
                int projType = ModContent.ProjectileType<DLCShadowWisp>();

                if (player.ownedProjectileCounts[projType] < 15)
                {
                    Projectile.NewProjectile(GetSource_EffectItem(player), target.Center, Vector2.Zero, projType, 16, 1, player.whoAmI, 0, 0, 0);
                }
            }
        }
    }
}