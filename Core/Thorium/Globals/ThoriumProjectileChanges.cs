using FargowiltasCrossmod.Content.Thorium.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ThoriumMod.Items.Donate;
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
            if (projectile.type == ModContent.ProjectileType<ShinobiSigilPro>())
            {
                projectile.damage /= 2;
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.type == ModContent.ProjectileType<ShinobiSigilPro>() && projectile.owner == Main.myPlayer)
            {
                Main.player[projectile.owner].AddBuff(ModContent.BuffType<ShinobiSigilCD>(), 300);
            }
        }
    }
}
