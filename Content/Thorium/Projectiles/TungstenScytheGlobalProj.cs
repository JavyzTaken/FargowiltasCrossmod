using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Projectiles;
using Terraria.DataStructures;
using FargowiltasSouls;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class TungstenScytheGlobalProj : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.ModProjectile != null && entity.ModProjectile is ThoriumMod.Projectiles.Scythe.ScythePro;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            base.OnSpawn(projectile, source);
            if (!ThoriumMod.Items.HealerItems.ScytheItem.ProToScytheCharge.ContainsKey(projectile.type)) return;

            projectile.knockBack *= 0.5f;
            projectile.damage = (int)(projectile.damage * 0.8f);

            FargoSoulsGlobalProjectile gProj = projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>();
            if (gProj.TungstenScale == 2)
            {
                projectile.ModProjectile.DrawOffsetX = (int)projectile.Size.X / 4;
                projectile.ModProjectile.DrawOriginOffsetY = (int)projectile.Size.X / 4;
            }
            else if (gProj.TungstenScale == 3)
            {
                projectile.ModProjectile.DrawOffsetX = (int)projectile.Size.X / 3;
                projectile.ModProjectile.DrawOriginOffsetY = (int)projectile.Size.X / 3;
            }
        }

        // circular hitboxes
        public override bool? Colliding(Projectile projectile, Rectangle projHitbox, Rectangle targetHitbox)
        {
            static float SQ(float n) => n * n; 
            Vector2 Closest = new(MathHelper.Clamp(projectile.Center.X, targetHitbox.Left, targetHitbox.Right), MathHelper.Clamp(projectile.Center.Y, targetHitbox.Top, targetHitbox.Bottom));
            float distSQ = Closest.DistanceSQ(projectile.Center);
            return distSQ <= SQ(projectile.width / 2 + 16 * projectile.scale);
        }
    }
}