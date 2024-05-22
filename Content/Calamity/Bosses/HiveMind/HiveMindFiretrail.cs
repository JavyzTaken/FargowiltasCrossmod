using FargowiltasSouls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    public class HiveMindFiretrail : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.hostile = true;
            Projectile.timeLeft = 60 * 2;
            Projectile.tileCollide = false;

        }
        public override void AI()
        {
            Projectile.velocity *= 0.96f;
            if (Projectile.velocity.Length() < 0.3f)
                Projectile.velocity *= 0;
            int dustTime = 3;
            if (Projectile.ai[0] % dustTime == 0)
            {
                int d = Dust.NewDust(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / MathF.Sqrt(2), Projectile.height / MathF.Sqrt(2)), 0, 0, DustID.CursedTorch,
                    -Projectile.velocity.X + Main.rand.Next(-2, 2), -Projectile.velocity.Y + Main.rand.Next(-2, 2), Scale: 3);
                Main.dust[d].noGravity = true;
            }
            
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.CursedInferno, 90);
            base.OnHitPlayer(target, info);
        }
    }
}
