using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SkeletronPrime
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HomingRocket : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RocketSkeleton;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.timeLeft = 1000;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.position.X += Projectile.width / 2;
            Projectile.position.Y += Projectile.height / 2;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.position.X -= Projectile.width / 2;
            Projectile.position.Y -= Projectile.height / 2;
            for (int num904 = 0; num904 < 30; num904++)
            {
                int num905 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust2 = Main.dust[num905];
                dust2.velocity *= 1.4f;
            }
            for (int num906 = 0; num906 < 20; num906++)
            {
                int num907 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[num907].noGravity = true;
                Dust dust2 = Main.dust[num907];
                dust2.velocity *= 7f;
                num907 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                dust2 = Main.dust[num907];
                dust2.velocity *= 3f;
            }
            for (int num908 = 0; num908 < 2; num908++)
            {
                float num909 = 0.4f;
                if (num908 == 1)
                {
                    num909 = 0.8f;
                }
                int num910 = Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X, Projectile.position.Y), default(Vector2), Main.rand.Next(61, 64));
                Gore gore2 = Main.gore[num910];
                gore2.velocity *= num909;
                Main.gore[num910].velocity.X += 1f;
                Main.gore[num910].velocity.Y += 1f;
                num910 = Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X, Projectile.position.Y), default(Vector2), Main.rand.Next(61, 64));
                gore2 = Main.gore[num910];
                gore2.velocity *= num909;
                Main.gore[num910].velocity.X -= 1f;
                Main.gore[num910].velocity.Y += 1f;
                num910 = Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X, Projectile.position.Y), default(Vector2), Main.rand.Next(61, 64));
                gore2 = Main.gore[num910];
                gore2.velocity *= num909;
                Main.gore[num910].velocity.X += 1f;
                Main.gore[num910].velocity.Y -= 1f;
                num910 = Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.position.X, Projectile.position.Y), default(Vector2), Main.rand.Next(61, 64));
                gore2 = Main.gore[num910];
                gore2.velocity *= num909;
                Main.gore[num910].velocity.X -= 1f;
                Main.gore[num910].velocity.Y -= 1f;
            }
        }
        public override void AI()
        {
            int p = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
            Projectile.ai[0]++;

            if (Projectile.ai[0] == 1)
            {
                SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
            }
            if (p >= 0 && Projectile.ai[0] > 30 && Projectile.ai[0] < 180)
            {
                if (Projectile.velocity.Length() < 12)
                {
                    Projectile.velocity *= 1.1f;
                }
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(Utils.AngleTowards(Projectile.velocity.ToRotation(), Projectile.AngleTo(Main.player[p].Center), 0.04f));
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            for (int l = 0; l < 2; l++)
            {
                float num14 = 0f;
                float num15 = 0f;
                if (l == 1)
                {
                    num14 = Projectile.velocity.X * 0.5f;
                    num15 = Projectile.velocity.Y * 0.5f;
                }
                if (Main.rand.Next(2) == 0)
                {
                    int num16 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num14, Projectile.position.Y + 3f + num15) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 6, 0f, 0f, 100);
                    Main.dust[num16].scale *= 1.4f + (float)Main.rand.Next(10) * 0.1f;
                    Main.dust[num16].velocity *= 0.2f;
                    Main.dust[num16].noGravity = true;
                    if (Main.dust[num16].type == 152)
                    {
                        Main.dust[num16].scale *= 0.5f;
                        Main.dust[num16].velocity += Projectile.velocity * 0.1f;
                    }
                    else if (Main.dust[num16].type == 35)
                    {
                        Main.dust[num16].scale *= 0.5f;
                        Main.dust[num16].velocity += Projectile.velocity * 0.1f;
                    }
                    else if (Main.dust[num16].type == Dust.dustWater())
                    {
                        Main.dust[num16].scale *= 0.65f;
                        Main.dust[num16].velocity += Projectile.velocity * 0.1f;
                    }

                }
                if (Main.rand.Next(2) == 0)
                {
                    int num17 = Dust.NewDust(new Vector2(Projectile.position.X + 3f + num14, Projectile.position.Y + 3f + num15) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 31, 0f, 0f, 100, default(Color), 0.5f);
                    Main.dust[num17].fadeIn = 0.5f + (float)Main.rand.Next(5) * 0.1f;
                    Main.dust[num17].velocity *= 0.05f;
                }
            }
        }
    }
}
