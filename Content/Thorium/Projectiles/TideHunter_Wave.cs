using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    public class TideHunter_Wave : ModProjectile // using steel parry sprite as placeholder
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 40;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 1000; 
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.alpha = 0;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            Projectile.position += Projectile.velocity;
            Projectile.velocity.X = Math.Max(Math.Abs(Projectile.velocity.X)-0.5f, 7) * Math.Sign(Projectile.velocity.X);
        }
    }
}