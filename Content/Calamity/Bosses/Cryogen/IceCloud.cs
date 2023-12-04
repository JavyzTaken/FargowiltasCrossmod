
//using FargowiltasSouls.Common.Graphics.Particles;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    public class IceCloud : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Empty";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 60 * 4;
            Projectile.tileCollide = false;
            Projectile.coldDamage = true;
        }
        public override void AI()
        {
            //don't make particles offscreen, to save on particle limit.
            //distance to 1920x1080 screen corner is abt 1100
            int pIndex = Player.FindClosest(Projectile.Center, 0, 0);
            if (!(pIndex.WithinBounds(Main.maxPlayers) && Main.player[pIndex].Distance(Projectile.Center) < 1200f)) 
                return;
            if (Projectile.timeLeft % 25 == 0)
            {
                //Color color = Projectile.ai[0] > 0 ? Color.GhostWhite : Color.Blue;
                //Color color = Color.Lerp(Color.Cyan, Color.LightBlue, 0.4f);
                //Particle smoke = new FogPuff(Projectile.Center + Main.rand.NextVector2Circular(20, 20), Vector2.Zero, smokeColor, 0.5f, 30, 0.8f, Main.rand.NextFloat(MathHelper.TwoPi), Main.rand.NextFloat(-0.2f, 0.2f));
                //Particle smoke = new ExpandingBloomParticle(Projectile.Center + Main.rand.NextVector2Circular(20, 20), Vector2.Zero, Color.Cyan, Vector2.One, Vector2.Zero, 30, true, Color.LightBlue);
                //smoke.Spawn();

                Particle snowflake = new SnowflakeSparkle(Projectile.Center + Main.rand.NextVector2Circular(20, 20), Main.rand.NextVector2Circular(2, 2), Color.White, new Color(75, 177, 250), Main.rand.NextFloat(0.3f, 1.5f), 60, 0.5f);
                GeneralParticleHandler.SpawnParticle(snowflake);

            }
        }
    }
}
