
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
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 10 == 0)
            {
                Particle smoke = new ExpandingBloomParticle(Projectile.Center + Main.rand.NextVector2Circular(20, 20), Vector2.Zero, Color.Cyan, Vector2.One, Vector2.Zero, 30, true, Color.LightBlue);
                smoke.Spawn();

            }
        }
    }
}
