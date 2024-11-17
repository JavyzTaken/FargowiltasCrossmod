using System;
using CalamityMod;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CursedLightning : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CultistBossLightningOrbArc;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 4;
            Projectile.scale = 1;
            Projectile.Calamity().DealsDefenseDamage = true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int n = 0; n < Projectile.oldPos.Length && (Projectile.oldPos[n].X != 0f || Projectile.oldPos[n].Y != 0f); n++)
            {
                projHitbox.X = (int)Projectile.oldPos[n].X;
                projHitbox.Y = (int)Projectile.oldPos[n].Y;
                if (projHitbox.Intersects(targetHitbox))
                {
                    return true;
                }
            }
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.CursedInferno, 300);
            //target.AddBuff(BuffID.Electrified, 60);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //copy pasted vanilla code for drawing lightning
            Vector2 end = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D value103 = TextureAssets.Extra[33].Value;
            Projectile.GetAlpha(lightColor);
            Vector2 vector96 = new Vector2(Projectile.scale) / 2f;
            for (int num387 = 0; num387 < 3; num387++)
            {
                switch (num387)
                {
                    case 0:
                        vector96 = new Vector2(Projectile.scale) * 0.6f;
                        DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(138, 219, 115, 0) * 0.5f;
                        break;
                    case 1:
                        vector96 = new Vector2(Projectile.scale) * 0.4f;
                        DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(30, 255, 0, 0) * 0.5f;
                        break;
                    default:
                        vector96 = new Vector2(Projectile.scale) * 0.2f;
                        DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(255, 255, 255, 0) * 0.5f;
                        break;
                }
                DelegateMethods.f_1 = 1f;
                for (int num388 = Projectile.oldPos.Length - 1; num388 > 0; num388--)
                {
                    if (!(Projectile.oldPos[num388] == Vector2.Zero))
                    {
                        Vector2 start = Projectile.oldPos[num388] + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                        Vector2 end2 = Projectile.oldPos[num388 - 1] + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                        Utils.DrawLaser(Main.spriteBatch, value103, start, end2, vector96, DelegateMethods.LightningLaserDraw);
                    }
                }
                if (Projectile.oldPos[0] != Vector2.Zero)
                {
                    DelegateMethods.f_1 = 1f;
                    Vector2 start2 = Projectile.oldPos[0] + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                    Utils.DrawLaser(Main.spriteBatch, value103, start2, end, vector96, DelegateMethods.LightningLaserDraw);
                }
            }
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            //for (int i = 0; i < Projectile.oldPos.Length; i++)
            //{
            //    Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, new Color(200, 200, 200, 100), Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            //}
            //Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, new Color(200, 200, 200, 100), Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            //copy pasted vanilla ai for lightning
            Projectile.frameCounter++;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.45f, 0.5f);
            if (Projectile.velocity == Vector2.Zero)
            {
                if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
                {
                    Projectile.frameCounter = 0;
                    bool flag33 = true;
                    for (int num771 = 1; num771 < Projectile.oldPos.Length; num771++)
                    {
                        if (Projectile.oldPos[num771] != Projectile.oldPos[0])
                        {
                            flag33 = false;
                        }
                    }
                    if (flag33)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
                if (Main.rand.NextBool(Projectile.extraUpdates))
                {
                    for (int num772 = 0; num772 < 2; num772++)
                    {
                        float num773 = Projectile.rotation + ((Main.rand.NextBool(2)) ? (-1f) : 1f) * ((float)Math.PI / 2f);
                        float num774 = (float)Main.rand.NextDouble() * 0.8f + 1f;
                        Vector2 vector90 = new Vector2((float)Math.Cos(num773) * num774, (float)Math.Sin(num773) * num774);
                        int num775 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Electric, vector90.X, vector90.Y);
                        Main.dust[num775].noGravity = true;
                        Main.dust[num775].scale = 1.2f;
                    }
                    if (Main.rand.NextBool(5))
                    {
                        Vector2 vector91 = Projectile.velocity.RotatedBy(1.5707963705062866) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                        int num776 = Dust.NewDust(Projectile.Center + vector91 - Vector2.One * 4f, 8, 8, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                        Dust dust2 = Main.dust[num776];
                        dust2.velocity *= 0.5f;
                        Main.dust[num776].velocity.Y = 0f - Math.Abs(Main.dust[num776].velocity.Y);
                    }
                }
            }
            else
            {
                if (Projectile.frameCounter < Projectile.extraUpdates * 2)
                {
                    return;
                }
                Projectile.frameCounter = 0;
                float num777 = Projectile.velocity.Length();
                UnifiedRandom unifiedRandom = new UnifiedRandom((int)Projectile.ai[1]);
                int num778 = 0;
                Vector2 spinningpoint15 = -Vector2.UnitY;
                while (true)
                {
                    int num779 = unifiedRandom.Next();
                    Projectile.ai[1] = num779;
                    num779 %= 100;
                    float f = (float)num779 / 100f * ((float)Math.PI * 2f);
                    Vector2 vector92 = f.ToRotationVector2();
                    if (vector92.Y > 0f)
                    {
                        vector92.Y *= -1f;
                    }
                    bool flag34 = false;
                    if (vector92.Y > -0.02f)
                    {
                        flag34 = true;
                    }
                    if (vector92.X * (float)(Projectile.extraUpdates + 1) * 2f * num777 + Projectile.localAI[0] > 40f)
                    {
                        flag34 = true;
                    }
                    if (vector92.X * (float)(Projectile.extraUpdates + 1) * 2f * num777 + Projectile.localAI[0] < -40f)
                    {
                        flag34 = true;
                    }
                    if (flag34)
                    {
                        if (num778++ >= 100)
                        {
                            Projectile.velocity = Vector2.Zero;
                            Projectile.localAI[1] = 1f;
                            break;
                        }
                        continue;
                    }
                    spinningpoint15 = vector92;
                    break;
                }
                if (Projectile.velocity != Vector2.Zero)
                {
                    Projectile.localAI[0] += spinningpoint15.X * (float)(Projectile.extraUpdates + 1) * 2f * num777;
                    Projectile.velocity = spinningpoint15.RotatedBy(Projectile.ai[0] + (float)Math.PI / 2f) * num777;
                    Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
                }
            }
        }
    }
}
