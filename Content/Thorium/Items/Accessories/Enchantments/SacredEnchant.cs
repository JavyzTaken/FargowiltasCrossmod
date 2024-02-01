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
    public class SacredEnchant : BaseSynergyEnchant<WarlockEffect>
    {
        public override Color nameColor => Color.Orange;
        internal override int SynergyEnch => ModContent.ItemType<WarlockEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SacredEffect>(Item);
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SacredEffect : SynergyEffect<WarlockEffect>
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.AlfheimHeader>();

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (hitInfo.Crit)
            {
                int projType = ModContent.ProjectileType<DLCShadowWisp>();

                if (player.ownedProjectileCounts[projType] < 15)
                {
                    bool synergy = SynergyActive(player);

                    int shadowWispType = synergy ? 2 : 1; // warlock 0

                    int damage = synergy ? 24 : 0; // warlock 16
                    float kb = 0f; // warlock 0

                    Projectile.NewProjectile(GetSource_EffectItem(player), target.Center, Vector2.Zero, projType, damage, kb, player.whoAmI, 0, 0, shadowWispType);
                }
            }
        }
    }
}