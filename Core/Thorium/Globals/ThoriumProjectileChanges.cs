using System;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ThoriumMod.Empowerments;
using ThoriumMod.Items.Donate;
using ThoriumMod.Projectiles;
using ThoriumMod.Projectiles.Thrower;

namespace FargowiltasCrossmod.Core.Thorium.Globals
{
    public class ThoriumProjectileChanges : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return true;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode) return;
            
            if (projectile.type == ModContent.ProjectileType<ShinobiSigilPro>())
            {
                projectile.damage /= 2;
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode) return;
            
            if (projectile.type == ModContent.ProjectileType<ShinobiSigilPro>() && projectile.owner == Main.myPlayer)
            {
                Main.player[projectile.owner].AddBuff(ModContent.BuffType<ShinobiSigilCD>(), 300);
            }

            if (projectile.type == ModContent.ProjectileType<SpearExtra>())
                projectile.damage /= 2;
            if (projectile.type == ModContent.ProjectileType<SpearExtraFlame>())
                projectile.damage = (int)(projectile.damage * 2f / 3f);
            if (projectile.type == ModContent.ProjectileType<SpearExtraCrystal>())
                projectile.damage /= 2;

            if (projectile.type == ModContent.ProjectileType<WhiteFlare>())
            {
                if (Main.player[projectile.owner].HasBuff<WhiteDwarfCooldown>())
                {
                    projectile.damage = 0;
                    projectile.hide = true;
                    projectile.timeLeft = 0;
                    projectile.netUpdate = true;
                }
                else
                {
                    projectile.damage = Math.Min(5000, projectile.damage);
                    Main.player[projectile.owner].AddBuff(ModContent.BuffType<WhiteDwarfCooldown>(), 300);
                }
            }
        }
    }
}
