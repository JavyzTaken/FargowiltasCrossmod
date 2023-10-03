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
    public class BloodSpirit : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/BloodflareSoul";
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 300;
            Main.projFrames[Type] = 4;
            
        }
        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4) Projectile.frame = 0;
            }
            if (Projectile.timeLeft <= 298 && Projectile.ai[0] == 0)
            {
                NPC target = Projectile.FindTargetWithinRange(800);
                if (target != null && target.active)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20, 0.03f);
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner >= 0)
            {
                Player owner = Main.player[Projectile.owner];
                Vector2 toowner = (owner.Center - Projectile.Center);
                for (int i = 0; i < toowner.Length(); i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + toowner.SafeNormalize(Vector2.Zero) * i, DustID.LifeDrain);
                    d.noGravity = true;
                    d.velocity /= 3;
                }
                for (int i = 0; i < 100; i++)
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain).noGravity = true;
                }
                owner.HealEffect(Main.rand.Next(30, 51));
            }
        }
    }
}
