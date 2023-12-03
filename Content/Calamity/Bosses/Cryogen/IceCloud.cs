
using FargowiltasSouls.Common.Graphics.Particles;
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
        private readonly Color smokeColor = new(122, 190, 255, 255);
        public override void AI()
        {
            if (Projectile.timeLeft % 10 == 0)
            {
                //Color color = Projectile.ai[0] > 0 ? Color.GhostWhite : Color.Blue;
                //Color color = Color.Lerp(Color.Cyan, Color.LightBlue, 0.4f);
                Particle smoke = new FogPuff(Projectile.Center + Main.rand.NextVector2Circular(20, 20), Vector2.Zero, smokeColor, 0.5f, 30, 0.8f, Main.rand.NextFloat(MathHelper.TwoPi), Main.rand.NextFloat(-0.2f, 0.2f));
                //Particle smoke = new ExpandingBloomParticle(Projectile.Center + Main.rand.NextVector2Circular(20, 20), Vector2.Zero, Color.Cyan, Vector2.One, Vector2.Zero, 30, true, Color.LightBlue);
                smoke.Spawn();

            }
        }
    }
}
