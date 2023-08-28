using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.DataStructures;
using System.IO;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExaltation
{
    
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class SilvaCrystal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/SilvaCrystal";
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 360;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.scale = 2;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.localAI[0] = 60;
            targetPosition = Main.player[(int)Projectile.ai[0]].Center;
            DoRainbowCrystalStaffExplosion(targetPosition);
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<SilvaCrystal>() && Main.projectile[i].localAI[1] == 1 && Main.projectile[i].active)
                {
                    return;
                }
            }
            Projectile.localAI[1] = 1;
        }
        public override void Kill(int timeLeft)
        {
            DoRainbowCrystalStaffExplosion(Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item110, Projectile.Center);

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(texture.Value, Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) / 2 - Main.screenPosition, null, new Color(200, 200, 200, 100) * (1f / (i + 1)), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            if (Projectile.timeLeft > 80 && Projectile.localAI[1] == 1)
            {
                Asset<Texture2D> light = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion];
                Player target = Main.player[(int)Projectile.ai[0]];
                Main.EntitySpriteDraw(light.Value, targetPosition - Main.screenPosition, null, new Color(200, 200, 200), MathHelper.ToRadians(Projectile.timeLeft * 3), light.Size() / 2, 2, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(light.Value, targetPosition - Main.screenPosition, null, new Color(200, 200, 200), 0, light.Size() / 2, 2, SpriteEffects.None, 0);
            }

            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(targetPosition);
            writer.Write7BitEncodedInt(timer);
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetPosition = reader.ReadVector2();
            timer = reader.Read7BitEncodedInt();
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }
        public Vector2 targetPosition;
        public int timer = 0;
        public override void AI()
        {
            Player target = Main.player[(int)Projectile.ai[0]];
            int damage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(310);
            if (timer == 0)
            {
                targetPosition = Vector2.Lerp(targetPosition, target.Center + target.velocity * 50, 0.08f);
            }
            else
            {
                timer--;
            }
            if (Projectile.timeLeft == 359)
            {
                DoRainbowCrystalStaffExplosion(Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
            }
            if (Projectile.timeLeft > 80)
            {
                if ((Projectile.timeLeft - 10) % 70 == 0 && Projectile.timeLeft != 350)
                {
                    timer = 20;
                    DoRainbowCrystalStaffExplosion(targetPosition);
                }
                if (Projectile.timeLeft % 70 == 0 && Projectile.timeLeft != 350)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), targetPosition + new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100)), Vector2.Zero, ModContent.ProjectileType<RainbowCrystalShot>(), damage, 0, Main.myPlayer, Main.rand.NextFloat(), Projectile.whoAmI);
                }
                Projectile.Center = target.Center + new Vector2(0, 400).RotatedBy(Projectile.ai[1]);
                Projectile.ai[1] += MathHelper.ToRadians(2);
            }
            else
            {
                if (Projectile.timeLeft > 20)
                {
                    if (Projectile.timeLeft == 80)
                    {
                        DoRainbowCrystalStaffExplosion(Projectile.Center);
                        DoRainbowCrystalStaffExplosion(targetPosition);
                        SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                        Projectile.rotation = Projectile.AngleTo(target.Center) + MathHelper.ToRadians(120);
                    }
                    Projectile.Center = target.Center + new Vector2(0, 400).RotatedBy(Projectile.ai[1]);
                    if (Projectile.localAI[0] > 0)
                    {
                        Projectile.localAI[0]--;
                    }
                    Projectile.rotation += MathHelper.ToRadians(Projectile.localAI[0]);
                }
                else if (Projectile.timeLeft == 20)
                {
                    Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20;
                }
            }
        }
        public void DoRainbowCrystalStaffExplosion(Vector2 position)
        {
            Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(3.1415927410125732);
            float num = Main.rand.Next(7, 13);
            Vector2 value = new Vector2(2.1f, 2f);
            Color newColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f, byte.MaxValue);
            newColor.A = byte.MaxValue;
            for (float num2 = 0f; num2 < num; num2 += 1f)
            {
                int num3 = Dust.NewDust(position, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, newColor, 1f);
                Main.dust[num3].position = position;
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
                int num5 = Dust.NewDust(position, 0, 0, DustID.RainbowMk2, 0f, 0f, 0, newColor, 1f);
                Main.dust[num5].position = position;
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
