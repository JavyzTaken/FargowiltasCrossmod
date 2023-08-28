using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofAnnihilation
{
    
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class GemProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/GemTechRedGem";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.scale = 3;

            Projectile.width = 10;
            Projectile.height = 10;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechBlueGem");
            if (Projectile.ai[0] == 1)
            {
                t = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechGreenGem");
            }
            else if (Projectile.ai[0] == 2)
            {
                t = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechPinkGem");
            }
            else if (Projectile.ai[0] == 3)
            {
                t = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechPurpleGem");
            }
            else if (Projectile.ai[0] == 4)
            {
                t = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechRedGem");
            }
            else if (Projectile.ai[0] == 5)
            {
                t = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechYellowGem");
            }
            else if (Projectile.ai[0] == 6)
            {
                t = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/GemTechGreenFlechette");
                lightColor = new Color(255, 255, 255, 100);
            }
            else if (Projectile.ai[0] == 7)
            {
                t = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechYellowGem");
                lightColor = new Color(255, 255, 255, 100);
            }

            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 120)
            {
                if (Projectile.ai[0] == 0 && Projectile.ai[1] == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.ai[0] = Main.rand.Next(0, 6);
                    SoundEngine.PlaySound(SoundID.Item61, Projectile.Center);
                    Projectile.netUpdate = true;
                }
                if (Projectile.ai[0] > 5)
                {
                    Projectile.scale = 1.5f;
                    SoundEngine.PlaySound(SoundID.Item118, Projectile.Center);
                }
                if (Projectile.ai[0] == 0 && Projectile.ai[1] == 1)
                {
                    Projectile.timeLeft = 300;
                    Projectile.ai[1] = 2;
                }

            }
            Player target = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];

            if (Projectile.timeLeft == 2 && Projectile.ai[0] == 4)
            {
                Vector2 center = Projectile.Center;
                Projectile.scale *= 10;
                Projectile.width *= 10;
                Projectile.height *= 10;
                Projectile.Center = center;
                Projectile.Opacity = 0;
                for (int i = 0; i < 100; i++)
                {
                    Dust.NewDustDirect(Projectile.Center + new Vector2(0, Main.rand.Next(0, (int)(Projectile.height * 0.7f))).RotatedByRandom(MathHelper.TwoPi), 0, 0, DustID.RedTorch, Scale: 3).noGravity = true;
                }
                //Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FargowiltasSouls.Projectiles.GlowRing>(), 0, 0, Main.myPlayer, 0, -7);
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }
            if (Projectile.timeLeft == 1)
            {
                if (Projectile.ai[0] == 0)
                {
                    if (Projectile.ai[1] == 0)
                    {
                        for (int i = -3; i < 5; i += 2)
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, -10).RotatedBy(MathHelper.ToRadians(10 * i)), Type, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(400), 0, Main.myPlayer, 0, 1);
                        SoundEngine.PlaySound(SoundID.Item118, Projectile.Center);
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 speed = new Vector2(0, 5).RotatedBy(MathHelper.ToRadians(36 * i));
                        Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.BubbleBurst_Blue, (int)speed.X, (int)speed.Y, Scale: 2).noGravity = true;
                    }
                }
                else if (Projectile.ai[0] == 1)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 speed = new Vector2(0, 5).RotatedBy(MathHelper.ToRadians(36 * i));
                        Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.BubbleBurst_Green, (int)speed.X, (int)speed.Y, Scale: 2).noGravity = true;
                    }
                }
                else if (Projectile.ai[0] == 6)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 speed = new Vector2(0, 5).RotatedBy(MathHelper.ToRadians(36 * i));
                        Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.BubbleBurst_Green, (int)speed.X, (int)speed.Y, Scale: 2).noGravity = true;
                    }
                }
                else if (Projectile.ai[0] == 2)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AnnihilationLaser>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(400), 0, Main.myPlayer);
                        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AnnihilationLaser>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(400), 0, Main.myPlayer, 0, (float)Math.PI);
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 speed = new Vector2(0, 5).RotatedBy(MathHelper.ToRadians(36 * i));
                        Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.BubbleBurst_Pink, (int)speed.X, (int)speed.Y, Scale: 2).noGravity = true;
                    }
                }
                else if (Projectile.ai[0] == 3)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AnnihilationLaser>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(400), 0, Main.myPlayer, 1, (float)-Math.PI / 2);
                        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AnnihilationLaser>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(400), 0, Main.myPlayer, 1, (float)Math.PI / 2);
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 speed = new Vector2(0, 5).RotatedBy(MathHelper.ToRadians(36 * i));
                        Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.BubbleBurst_Purple, (int)speed.X, (int)speed.Y, Scale: 2).noGravity = true;
                    }
                }
                else if (Projectile.ai[0] == 5)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, new Vector2(0, 20).RotatedBy(MathHelper.ToRadians(360f / 7 * i)), Type, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(400), 0, Main.myPlayer, 7);
                    }
                    SoundEngine.PlaySound(SoundID.Item118, Projectile.Center);
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 speed = new Vector2(0, 5).RotatedBy(MathHelper.ToRadians(36 * i));
                        Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Firework_Yellow, (int)speed.X, (int)speed.Y, Scale: 2).noGravity = true;
                    }
                }
                else if (Projectile.ai[0] == 7)
                {

                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 speed = new Vector2(0, 5).RotatedBy(MathHelper.ToRadians(36 * i));
                        Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Firework_Yellow, (int)speed.X, (int)speed.Y, Scale: 2).noGravity = true;
                    }
                }
            }
            if (Projectile.ai[0] == 6)
            {
                if (Projectile.timeLeft > 90)
                {
                    Projectile.velocity /= 1.1f;
                    Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length() * 2);
                }
                else if (Projectile.timeLeft == 90)
                {
                    Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 15;
                    SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                    Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                }
                return;
            }
            else if (Projectile.ai[0] == 7)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                if (Projectile.timeLeft > 90)
                {
                    Projectile.velocity /= 1.1f;
                }
                else if (Projectile.timeLeft < 90)
                {
                    Projectile.velocity *= 1.05f;
                }
                return;
            }
            if (Projectile.timeLeft < 20 && Projectile.ai[0] == 1 && Projectile.timeLeft % 2 == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, 10).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<GemProjectile>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(400), 0, Main.myPlayer, 6);
            }
            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length() * 2);
            if (Projectile.velocity.Y < 10)
                Projectile.velocity.Y += 0.15f;
        }
    }
}
