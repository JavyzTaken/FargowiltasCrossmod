using System;
using System.Collections.Generic;
using System.Linq;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DemonBloodEnchant : BaseEnchant
    {
        public override Color nameColor => Color.Red;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<DemonBloodEffect>(Item);
            player.AddEffect<FleshEffect>(Item);
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DemonBloodEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.helheimHeader>();

        public override void PostUpdateEquips(Player player)
        {
            int bloodType = ModContent.ProjectileType<Projectiles.DemonBloodSpill>();
            int bloodProjs = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.type != bloodType || !proj.active || proj.Center.DistanceSQ(player.Center) > 25600f) continue;
                proj.timeLeft--;
                bloodProjs++;

                for (int j = 0; j < 2; j++)
                {
                    Dust dust = Dust.NewDustDirect(proj.position, proj.width, proj.height, DustID.Blood);
                    dust.velocity = dust.position.DirectionTo(player.Center) * 8;
                }
            }

            if (bloodProjs == 0) return;
            player.GetDamage(DamageClass.Generic) += MathF.Min(bloodProjs, 5) * 0.1f;

            for (int i = 0; i < MathF.Min(bloodProjs, 5); i++)
            {
                if (Main.rand.NextBool(15))
                {
                    Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Blood);
                    dust.noGravity = false;
                }
            }
        }

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (hitInfo.Damage >= target.life) // kills
            {
                if (Main.rand.NextBool(4))
                {
                    SpawnDemonBlood(player, target.Center);
                }
            }

            // (should) be true if the current hit reduced the boss's life over a 10% increment
            if (target.boss && (int)(target.life / (target.lifeMax / 10)) > (int)((target.life - hitInfo.Damage) / (target.lifeMax / 10)))
            {
                SpawnDemonBlood(player, target.Center);
            }
        }

        public void SpawnDemonBlood(Player player, Vector2 pos)
        {
            Projectile.NewProjectile(GetSource_EffectItem(player), pos, Vector2.Zero, ModContent.ProjectileType<Projectiles.DemonBloodSpill>(), 0, 0, player.whoAmI);
        }
    }
}
