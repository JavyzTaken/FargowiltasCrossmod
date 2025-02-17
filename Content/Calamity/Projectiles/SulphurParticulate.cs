using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    public class SulphurParticulate : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Enemy/SulphuricAcidBubble";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.Opacity = 0;
            Projectile.timeLeft = 200;
            base.SetDefaults();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void AI()
        {
            Vector2 pos = Projectile.Center + new Vector2(Main.rand.NextFloat(0, Projectile.width), 0).RotatedByRandom(MathHelper.TwoPi);
            Vector2 vel = new Vector2(Main.rand.NextFloat(0, 2), 0).RotatedByRandom(MathHelper.TwoPi);
            if (Main.rand.NextBool(5))
            {
                if (Main.rand.NextBool())
                {
                    GenericSparkle p = new GenericSparkle(pos, vel, Color.DarkSeaGreen, Color.Green, 0.7f, 40, Main.rand.NextFloat(-0.2f, 0.2f));
                    GeneralParticleHandler.SpawnParticle(p);
                }
                else
                {
                    SparkleParticle p = new SparkleParticle(pos, vel, Color.DarkSeaGreen, Color.Green, 0.7f, 60, Main.rand.NextFloat(-0.2f, 0.2f));
                    GeneralParticleHandler.SpawnParticle(p);
                }
            }
            base.AI();
        }
    }
}
