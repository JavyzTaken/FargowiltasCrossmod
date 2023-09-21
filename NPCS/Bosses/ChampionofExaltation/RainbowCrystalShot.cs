using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.DataStructures;


namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExaltation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class RainbowCrystalShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RainbowCrystalExplosion;
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 122;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.friendly = false;

        }
        public override void SetStaticDefaults()
        {

        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void Kill(int timeLeft)
        {
            DoRainbowCrystalStaffExplosion();
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.hostile = true;
                Projectile.friendly = false;
                int width4 = Projectile.width;
                int height5 = Projectile.height;
                int num146 = Projectile.penetrate;
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 60;
                Projectile.Center = Projectile.position;
                Projectile.penetrate = -1;
                Projectile.maxPenetrate = -1;
                Projectile.Damage();
                Projectile.penetrate = num146;
                Projectile.position = Projectile.Center;
                Projectile.width = width4;
                Projectile.height = height5;
                Projectile.Center = Projectile.position;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.localAI[1] < 30)
            {
                return false;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            if (Projectile.type == 681 && Projectile.velocity.X > 0f)
            {
                spriteEffects ^= SpriteEffects.FlipHorizontally;
            }
            Color color32 = Lighting.GetColor((int)(Projectile.position.X + Projectile.width * 0.5) / 16, (int)((Projectile.position.Y + Projectile.height * 0.5) / 16.0));
            Vector2 vector74 = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D texture2D4 = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle31 = texture2D4.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame, 0, 0);
            Color color94 = Projectile.GetAlpha(color32);
            Vector2 origin24 = rectangle31.Size() / 2f;
            Color color98 = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f, byte.MaxValue).MultiplyRGBA(new Color(255, 255, 255, 0));
            Main.EntitySpriteDraw(texture2D4, vector74, new Microsoft.Xna.Framework.Rectangle?(rectangle31), color98, Projectile.rotation, origin24, Projectile.scale * 2f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture2D4, vector74, new Microsoft.Xna.Framework.Rectangle?(rectangle31), color98, 0f, origin24, Projectile.scale * 2f, spriteEffects, 0);
            if (Projectile.ai[1] != -1f && Projectile.Opacity > 0.3f)
            {
                Vector2 vector76 = Main.projectile[(int)Projectile.ai[1]].Center - Projectile.Center;
                Vector2 vector77 = new Vector2(1f, vector76.Length() / texture2D4.Height);
                float rotation30 = vector76.ToRotation() + 1.5707964f;
                float num366 = MathHelper.Distance(30f, Projectile.localAI[1]) / 20f;
                num366 = MathHelper.Clamp(num366, 0f, 1f);
                if (num366 > 0f)
                {
                    Main.EntitySpriteDraw(texture2D4, vector74 + vector76 / 2f, new Microsoft.Xna.Framework.Rectangle?(rectangle31), color98 * num366, rotation30, origin24, vector77, spriteEffects, 0);
                    Main.EntitySpriteDraw(texture2D4, vector74 + vector76 / 2f, new Microsoft.Xna.Framework.Rectangle?(rectangle31), color94 * num366, rotation30, origin24, vector77 / 2f, spriteEffects, 0);
                }
            }
            Main.EntitySpriteDraw(texture2D4, vector74, new Microsoft.Xna.Framework.Rectangle?(rectangle31), color94, 0f, origin24, Projectile.scale, spriteEffects, 0);
            return false;
        }
        public override void AI()
        {
            Color newColor2 = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f, byte.MaxValue);
            int owner = (int)Projectile.ai[1];
            if (owner < 0 || owner >= 1000 || !Main.projectile[owner].active && Main.projectile[owner].type != ModContent.ProjectileType<SilvaCrystal>())
            {
                Projectile.ai[1] = -1f;
            }
            else
            {
                DelegateMethods.v3_1 = newColor2.ToVector3() * 0.5f;
                Utils.PlotTileLine(Projectile.Center, Main.projectile[owner].Center, 8f, new Utils.TileActionAttempt(DelegateMethods.CastLight));
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = Main.rand.NextFloat() * 0.8f + 0.8f;
                Projectile.direction = Main.rand.Next(2) > 0 ? 1 : -1;
            }
            Projectile.rotation = Projectile.localAI[1] / 40f * 6.2831855f * Projectile.direction;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 8;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.alpha == 0)
            {
                Lighting.AddLight(Projectile.Center, newColor2.ToVector3() * 0.5f);
            }
            int num3;
            for (int num877 = 0; num877 < 2; num877 = num3 + 1)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value52 = Vector2.UnitY.RotatedBy((double)(num877 * 3.1415927f), default).RotatedBy(Projectile.rotation, default);
                    Dust dust43 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 225, newColor2, 1.5f)];
                    dust43.noGravity = true;
                    dust43.noLight = true;
                    dust43.scale = Projectile.Opacity * Projectile.localAI[0];
                    dust43.position = Projectile.Center;
                    dust43.velocity = value52 * 2.5f;
                }
                num3 = num877;
            }
            for (int num878 = 0; num878 < 2; num878 = num3 + 1)
            {
                if (Main.rand.NextBool(10))
                {
                    Vector2 value53 = Vector2.UnitY.RotatedBy((double)(num878 * 3.1415927f), default);
                    Dust dust44 = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 225, newColor2, 1.5f)];
                    dust44.noGravity = true;
                    dust44.noLight = true;
                    dust44.scale = Projectile.Opacity * Projectile.localAI[0];
                    dust44.position = Projectile.Center;
                    dust44.velocity = value53 * 2.5f;
                }
                num3 = num878;
            }
            if (Main.rand.NextBool(10))
            {
                float scaleFactor9 = 1f + Main.rand.NextFloat() * 2f;
                float fadeIn = 1f + Main.rand.NextFloat();
                float num879 = 1f + Main.rand.NextFloat();
                Vector2 vector121 = Utils.RandomVector2(Main.rand, -1f, 1f);
                if (vector121 != Vector2.Zero)
                {
                    vector121.Normalize();
                }
                vector121 *= 20f + Main.rand.NextFloat() * 100f;
                Vector2 vector122 = Projectile.Center + vector121;
                Point point3 = vector122.ToTileCoordinates();
                bool flag59 = true;
                if (!WorldGen.InWorld(point3.X, point3.Y, 0))
                {
                    flag59 = false;
                }
                if (flag59 && WorldGen.SolidTile(point3.X, point3.Y, false))
                {
                    flag59 = false;
                }
                if (flag59)
                {
                    Dust dust45 = Main.dust[Dust.NewDust(vector122, 0, 0, DustID.RainbowMk2, 0f, 0f, 127, newColor2, 1f)];
                    dust45.noGravity = true;
                    dust45.position = vector122;
                    dust45.velocity = -Vector2.UnitY * scaleFactor9 * (Main.rand.NextFloat() * 0.9f + 1.6f);
                    dust45.fadeIn = fadeIn;
                    dust45.scale = num879;
                    dust45.noLight = true;
                    if (dust45.dustIndex != 6000)
                    {
                        Dust dust46 = Dust.CloneDust(dust45);
                        Dust dust2 = dust46;
                        dust2.scale *= 0.65f;
                        dust2 = dust46;
                        dust2.fadeIn *= 0.65f;
                        dust46.color = new Color(255, 255, 255, 255);
                    }
                }
            }
            Projectile.scale = Projectile.Opacity / 2f * Projectile.localAI[0];
            Projectile.velocity = Vector2.Zero;
            ref float ptr = ref Projectile.localAI[1];
            ref float ptr64 = ref ptr;
            float num19 = ptr;
            ptr64 = num19 + 1f;
            if (Projectile.localAI[1] >= 60f)
            {
                Projectile.Kill();
            }
            if (Projectile.localAI[1] == 30f)
            {
                DoRainbowCrystalStaffExplosion();
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.friendly = true;
                    int width = Projectile.width;
                    int height = Projectile.height;
                    int num880 = Projectile.penetrate;
                    Projectile.position = Projectile.Center;
                    Projectile.width = Projectile.height = 60;
                    Projectile.Center = Projectile.position;
                    Projectile.penetrate = -1;
                    Projectile.maxPenetrate = -1;
                    Projectile.Damage();
                    Projectile.penetrate = num880;
                    Projectile.position = Projectile.Center;
                    Projectile.width = width;
                    Projectile.height = height;
                    Projectile.Center = Projectile.position;
                    Projectile.friendly = false;
                    return;
                }
            }
        }
        public void DoRainbowCrystalStaffExplosion()
        {
            Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(3.1415927410125732);
            float num = Main.rand.Next(7, 13);
            Vector2 value = new Vector2(2.1f, 2f);
            Color newColor = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f, byte.MaxValue);
            newColor.A = byte.MaxValue;
            for (float num2 = 0f; num2 < num; num2 += 1f)
            {
                int num3 = Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, newColor, 1f);
                Main.dust[num3].position = Projectile.Center;
                Main.dust[num3].velocity = spinningpoint.RotatedBy((double)(6.2831855f * num2 / num), default) * value * (0.8f + Main.rand.NextFloat() * 0.4f);
                Main.dust[num3].noGravity = true;
                Main.dust[num3].scale = 2f;
                Main.dust[num3].fadeIn = Main.rand.NextFloat() * 2f;
                if (num3 != 6000)
                {
                    Dust dust = Dust.CloneDust(num3);
                    dust.scale /= 2f;
                    dust.fadeIn /= 2f;
                    dust.color = new Color(255, 255, 255, 255);
                }
            }
            for (float num4 = 0f; num4 < num; num4 += 1f)
            {
                int num5 = Dust.NewDust(Projectile.Center, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, newColor, 1f);
                Main.dust[num5].position = Projectile.Center;
                Main.dust[num5].velocity = spinningpoint.RotatedBy((double)(6.2831855f * num4 / num), default) * value * (0.8f + Main.rand.NextFloat() * 0.4f);
                Main.dust[num5].velocity *= Main.rand.NextFloat() * 0.8f;
                Main.dust[num5].noGravity = true;
                Main.dust[num5].scale = Main.rand.NextFloat() * 1f;
                Main.dust[num5].fadeIn = Main.rand.NextFloat() * 2f;
                if (num5 != 6000)
                {
                    Dust dust2 = Dust.CloneDust(num5);
                    dust2.scale /= 2f;
                    dust2.fadeIn /= 2f;
                    dust2.color = new Color(255, 255, 255, 255);
                }
            }
        }
    }
}
