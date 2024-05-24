using CalamityMod.Particles;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
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
            int dustTime = 6;
            if (Projectile.ai[0] % dustTime == 0)
            {
                int d = Dust.NewDust(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / MathF.Sqrt(2), Projectile.height / MathF.Sqrt(2)), 0, 0, DustID.CursedTorch,
                -Projectile.velocity.X + Main.rand.Next(-2, 2), -Projectile.velocity.Y + Main.rand.Next(-2, 2), Scale: 3);
                Main.dust[d].noGravity = true;
                //MediumMistParticle p = new MediumMistParticle(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / MathF.Sqrt(2), Projectile.height / MathF.Sqrt(2)), -Projectile.velocity * 0.05f, Main.rand.NextBool(3) ? Color.LimeGreen : Color.Lime, Color.Black, Main.rand.NextFloat(0.6f, 1f), 20);
                //CalamityMod.Particles.Particle p = new FlameParticle(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / MathF.Sqrt(2), Projectile.height / MathF.Sqrt(2)), 50, 0.25f, power, Color.LimeGreen, Color.DarkGreen);
                //GeneralParticleHandler.SpawnParticle(p);
            }
            
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.CursedInferno, 90);
            base.OnHitPlayer(target, info);
        }
    }
}
