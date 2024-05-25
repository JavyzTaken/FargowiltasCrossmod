using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.NPCs.TownNPCs;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using FargowiltasSouls;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    public class HMShadeNimbus : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/ShadeNimbusCloud";
        public bool StartFading = false;
        public float timer = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 300;
            Projectile.width = Projectile.height = 28;

            Projectile.netImportant = true;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            Projectile.scale = 2;
            Projectile.light = 1;
        }

        public override void AI()
        {
            if (++timer > 60)
                Projectile.velocity *= 1.007f;

            if (Projectile.ai[2] == 0)
                Projectile.ai[2] = Main.rand.NextBool() ? 1 : -1;
            Projectile.rotation += Projectile.ai[2] * MathF.Tau / 137f;

            int fadeoutSpeed = 8;

            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 8 % Main.projFrames[Projectile.type];

            if (Collision.SolidCollision(Projectile.Center, Projectile.width, Projectile.height) || Projectile.timeLeft < (255 / fadeoutSpeed))
                StartFading = true;

            float sin = MathF.Sin(Projectile.timeLeft * MathHelper.Pi / 7f);
            if (StartFading)
            {
                Projectile.alpha += fadeoutSpeed;
            }
            else
            {
                
                Projectile.Opacity = 0.9f + 0.1f * sin;
                
            }
            Projectile.scale = 2f + 0.1f * sin;

            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item34, Projectile.position);
            }
            else if (Projectile.ai[1] == 1f && Main.netMode != 1)
            {
                int num13 = -1;
                float num14 = 2000f;
                for (int num15 = 0; num15 < 255; num15++)
                {
                    if (Main.player[num15].active && !Main.player[num15].dead)
                    {
                        Vector2 center2 = Main.player[num15].Center;
                        float num16 = Vector2.Distance(center2, Projectile.Center);
                        if ((num16 < num14 || num13 == -1) && Collision.CanHit(Projectile.Center, 1, 1, center2, 1, 1))
                        {
                            num14 = num16;
                            num13 = num15;
                        }
                    }
                }
                if (num14 < 20f)
                {
                    Projectile.Kill();
                    return;
                }
                if (num13 != -1)
                {
                    Projectile.ai[1] = 21f;
                    Projectile.ai[0] = num13;
                    Projectile.netUpdate = true;
                }
            }
            else if (Projectile.ai[1] > 20f && Projectile.ai[1] < 200f)
            {
                Projectile.ai[1] += 1f;
                int num17 = (int)Projectile.ai[0];
                if (!Main.player[num17].active || Main.player[num17].dead)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                else
                {
                    float num18 = Projectile.velocity.ToRotation();
                    Vector2 vector5 = Main.player[num17].Center - Projectile.Center;
                    if (vector5.Length() < 20f)
                    {
                        Projectile.Kill();
                        return;
                    }
                    float targetAngle2 = vector5.ToRotation();
                    if (vector5 == Vector2.Zero)
                    {
                        targetAngle2 = num18;
                    }
                    float num19 = num18.AngleLerp(targetAngle2, 0.01f);
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(num19);
                }
            }
            if (Projectile.ai[1] >= 1f && Projectile.ai[1] < 20f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] == 20f)
                {
                    Projectile.ai[1] = 1f;
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 12f)
            {
                Projectile.localAI[0] = 0f;
                for (int num20 = 0; num20 < 12; num20++)
                {
                    Vector2 spinningpoint2 = Vector2.UnitX * -Projectile.width / 2f;
                    spinningpoint2 += -Vector2.UnitY.RotatedBy((float)num20 * (float)Math.PI / 6f) * new Vector2(8f, 16f);
                    spinningpoint2 = spinningpoint2.RotatedBy(Projectile.rotation - (float)Math.PI / 2f);
                    int num21 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Corruption, 0f, 0f, 160);
                    Main.dust[num21].scale = 1.1f;
                    Main.dust[num21].noGravity = true;
                    Main.dust[num21].position = Projectile.Center + spinningpoint2;
                    Main.dust[num21].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num21].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num21].position) * 1.25f;
                }
            }
            if (Main.rand.NextBool(2))
            {
                for (int num26 = 0; num26 < 2; num26++)
                {
                    Vector2 vector8 = -Vector2.UnitX.RotatedByRandom(0.7853981852531433).RotatedBy(Projectile.velocity.ToRotation());
                    int num27 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, 0f, 0f, 0, default, 1.2f);
                    Main.dust[num27].velocity *= 0.3f;
                    Main.dust[num27].noGravity = true;
                    Main.dust[num27].position = Projectile.Center + vector8 * Projectile.width / 2f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num27].fadeIn = 1.4f;
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            FargoSoulsUtil.ProjectileWithTrailDraw(Projectile, lightColor);
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // Makes a dust effect.
            for (int dustIndex = 0; dustIndex < 40; dustIndex++)
            {
                // I choose .position (Which is the top left) instead of .Center because Dust.NewDust was made to spawn given .position.
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Demonite, 0, 0, 0, default, 0.5f);
            }
            target.AddBuff(ModContent.BuffType<BrainRot>(), 60 * 4);
        }
    }
}
