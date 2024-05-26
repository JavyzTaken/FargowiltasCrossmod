using CalamityMod.Dusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    public class VileClotDrop : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.light = 0.6f;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            if (Projectile.velocity.Length() < 20f)
                Projectile.velocity *= 1.05f;
            if (Main.rand.NextBool(2))
            {
                int vileDust = Dust.NewDust(Projectile.position,
                Projectile.width, Projectile.height, (int)CalamityDusts.SulphurousSeaAcid, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 100, default, 1.5f);
                Main.dust[vileDust].noGravity = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float sin = MathF.Sin(Projectile.timeLeft * MathHelper.Pi / 7f);
            Projectile.Opacity = 0.85f + 0.15f * sin;
            Projectile.scale = 1f + 0.15f * sin;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(BuffID.CursedInferno, 60);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 6; i++)
            {
                int killDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.SulphurousSeaAcid, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, default, 2.5f);
                Main.dust[killDust].noGravity = true;
                Dust dust = Main.dust[killDust];
                dust.velocity *= 2f;
                killDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.SulphurousSeaAcid, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, default, 1.2f);
                dust = Main.dust[killDust];
                dust.velocity *= 2f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            FargoSoulsUtil.ProjectileWithTrailDraw(Projectile, lightColor, additiveTrail: true, alsoAdditiveMainSprite: false);
            return false;
        }
    }
}
