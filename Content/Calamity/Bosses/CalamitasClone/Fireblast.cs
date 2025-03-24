using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Particles;
using CalamityMod;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls;
using Luminance.Common.Utilities;
using FargowiltasSouls.Core.Systems;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    public class Fireblast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/SCalBrimstoneFireblast";
        public bool withinRange = false;
        public bool setLifetime = false;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.Opacity = 0f;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.scale = 1.5f;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 5)
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 0.9f * Projectile.Opacity, 0f, 0f);

            if (!withinRange)
            {
                if (Projectile.ai[2] == 1f)
                    Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft / 60f, 0f, 1f);
                else
                    Projectile.Opacity = MathHelper.Clamp(1f - ((Projectile.timeLeft - 130) / 20f), 0f, 1f);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SupremeCalamitas.BrimstoneBigShotSound with { Pitch = -0.25f }, Projectile.Center);
            }

            int target = (int)Projectile.ai[0];

            if (!withinRange)
            {
                float max = 25f;
                if (Projectile.timeLeft < 40)
                    max = 40f;
                if (Projectile.velocity.Length() < max)
                    Projectile.velocity *= 1.05f;
                if (Main.player[target] != null && Main.player[target].Alive())
                {
                    Vector2 toPlayer = Projectile.DirectionTo(Main.player[target].Center);

                    Projectile.velocity = Projectile.velocity.RotateTowards(toPlayer.ToRotation(), 0.008f);

                    float diff = FargoSoulsUtil.RotationDifference(Projectile.velocity, toPlayer);
                    if (Math.Abs(diff) > MathHelper.PiOver2 && Projectile.timeLeft > 10)
                    {
                        Projectile.timeLeft = 10;
                    }
                }
            }

            float targetDist;
            if (target != -1 && !Main.player[target].dead && Main.player[target].active && Main.player[target] != null)
                targetDist = Vector2.Distance(Main.player[target].Center, Projectile.Center);
            else
                targetDist = 1000;

            if (Projectile.ai[1] == 2f && !withinRange && Main.rand.NextBool())
            {
                SparkParticle orb = new SparkParticle(Projectile.Center - Projectile.velocity + Main.rand.NextVector2Circular(20, 20), -Projectile.velocity * Main.rand.NextFloat(0.1f, 1f), false, 14, Main.rand.NextFloat(0.35f, 0.6f), (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * Projectile.Opacity);
                GeneralParticleHandler.SpawnParticle(orb);
            }
            if ((Projectile.timeLeft == 1 && !withinRange)) // When within 14 blocks of player or when it runs out of time
            {
                Projectile.Kill();
                return;
                if (!setLifetime)
                {
                    Projectile.timeLeft = 60;
                    setLifetime = true;
                    Projectile.extraUpdates = 1;
                }
                withinRange = true;
            }
            if (withinRange && Projectile.ai[2] == 0f)
            {
                Projectile.velocity *= 0.99f;
                for (int i = 0; i < 2; i++)
                {
                    Dust failShotDust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 60 : 114);
                    failShotDust.noGravity = true;
                    failShotDust.velocity = new Vector2(4, 4).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.3f);
                    failShotDust.scale = Main.rand.NextFloat(0.7f, 1.8f);
                }
                if (Projectile.timeLeft <= 40)
                {
                    if (Projectile.Opacity > 0)
                        Projectile.Opacity -= 0.05f;
                }
                if (Projectile.timeLeft == 30)
                {
                    Projectile.Opacity = 0;
                    //Projectile.velocity *= 0;
                    for (int i = 0; i < 2; i++)
                    {
                        Particle bloom = new BloomParticle(Projectile.Center, Vector2.Zero, new Color(121, 21, 77), 0.1f, 0.7f, 30, false);
                        GeneralParticleHandler.SpawnParticle(bloom);
                        if (Projectile.ai[2] == 1f)
                            bloom.Lifetime = 0;
                    }
                }
                if (Projectile.timeLeft == 15)
                {
                    Particle bloom = new BloomParticle(Projectile.Center, Vector2.Zero, Color.Red, 0.1f, 0.65f, 15, false);
                    GeneralParticleHandler.SpawnParticle(bloom);
                }
                if (Projectile.timeLeft == 8)
                {
                    Particle bloom = new BloomParticle(Projectile.Center, Vector2.Zero, Color.White, 0.1f, 0.5f, 8, false);
                    GeneralParticleHandler.SpawnParticle(bloom);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int drawStart = frameHeight * Projectile.frame;
            lightColor.R = (byte)(255 * Projectile.Opacity);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.Opacity != 1f)
                return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SCalBrimstoneFireblast.ImpactSound, Projectile.Center);

            if (Projectile.ai[2] == 0f)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    int spread = 3;
                    int type = ModContent.ProjectileType<BrimstoneBarrageGravitating>();
                    float velocity = 24f;
                    Vector2 baseVel = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                    float spreadRot = MathHelper.PiOver2 * 0.25f;
                    if (WorldSavingSystem.MasochistModeReal)
                    {
                        spread = 4;
                        spreadRot = MathHelper.PiOver2 * 0.2f;
                    }
                    Vector2 aimPos = Projectile.Center - baseVel * 50;
                    for (int k = -spread; k < spread; k++)
                    {
                        Vector2 dir = baseVel.RotatedBy(spreadRot * k);
                        float aim = (Projectile.Center + dir * 80).DirectionTo(aimPos).ToRotation();
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, dir * velocity, type, (int)Math.Round(Projectile.damage * 0.75), 0f, Projectile.owner, 0f, Projectile.ai[1], ai2: aim);
                    }
                }
                for (int i = 0; i < 18; i++)
                {
                    Vector2 velocity = new Vector2(12, 12).RotatedByRandom(100);
                    PointParticle spark2 = new PointParticle(Projectile.Center + velocity, velocity * Main.rand.NextFloat(0.3f, 1f), false, 15, 1.1f, (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * 0.6f);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                for (int i = 0; i < 18; i++)
                {
                    Dust failShotDust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 60 : 114);
                    failShotDust.noGravity = true;
                    failShotDust.velocity = new Vector2(16, 16).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.3f);
                    failShotDust.scale = Main.rand.NextFloat(0.75f, 1.3f);
                }
            }
        }
    }
}
