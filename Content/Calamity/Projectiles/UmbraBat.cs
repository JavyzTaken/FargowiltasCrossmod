using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class UmbraBat : ModProjectile
    {
        
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 300;
            Main.projFrames[Type] = 8;
            Projectile.penetrate = 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] == 0;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[0] = 1;
            Projectile.frame *= 2;
            Projectile.velocity *= 0;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4 && Projectile.ai[0] == 0) Projectile.frame = 0;
                else if (Projectile.frame >= 8) Projectile.frame = 4;
            }
            if (Projectile.timeLeft <= 280 && Projectile.ai[0] == 0)
            {
                NPC target = Projectile.FindTargetWithinRange(800);
                if (target != null && target.active)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20, 0.03f);
                }
            }
            if (Projectile.ai[0] == 1 && Projectile.owner >= 0)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.LifeDrain);
                d.velocity /= 3;
                Player owner = Main.player[Projectile.owner];
                if (owner != null && owner.active) {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20, 0.03f);
                    if (Projectile.Hitbox.Intersects(owner.Hitbox))
                    {
                        Projectile.Kill();
                        owner.HealEffect(Main.rand.Next(10, 21));
                    }
                }
            }
            Projectile.spriteDirection = 1;
            if (Projectile.velocity.X > 0)
            {
                Projectile.spriteDirection = -1;
            }
            base.AI();
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
    }
}
