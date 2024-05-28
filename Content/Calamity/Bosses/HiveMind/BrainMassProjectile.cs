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
using Terraria.GameContent;
using Luminance.Common.Utilities;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BrainMassProjectile : ModProjectile
    {
        public bool StartFading = false;
        public float AccelerationTimer = 0;
        public float Timer = 0;
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

            Projectile.scale = 1.5f;
            Projectile.light = 1;
        }

        public override void AI()
        {
            if (++AccelerationTimer > 60)
                Projectile.velocity *= 1.007f;

            if (Projectile.ai[1] == 0)
                HomingAI();
            else
                DeceleratingAI();
        }
        public void DeceleratingAI()
        {
            if (Projectile.ai[2] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.ai[2] = 1;
            }

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
            Projectile.scale = 1.5f + 0.1f * sin;

            if (Timer == 0f)
            {
                Timer = 1f;
                SoundEngine.PlaySound(SoundID.NPCHit18 with { Volume = 0.5f, PitchVariance = 0.2f }, Projectile.position);
            }
            else
            {
                Projectile.velocity *= 0.96f;
                if (Projectile.velocity.Length() < 0.3f)
                    Projectile.velocity *= 0;
            }
            if (Main.rand.NextBool(20))
            {
                Vector2 offset = Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.height / 3);
                Particle p = new BloodParticle(Projectile.Center + offset, offset.SafeNormalize(-Vector2.UnitY) * Main.rand.NextFloat(2, 5), 20, 1, Color.Green);
                GeneralParticleHandler.SpawnParticle(p);
            }
        }
        public void HomingAI()
        {
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
            Projectile.scale = 1.5f + 0.1f * sin;

            if (Timer == 0f)
            {
                Timer = 1f;
                SoundEngine.PlaySound(SoundID.NPCHit18 with { Volume = 0.5f, PitchVariance = 0.2f }, Projectile.position);
            }
            else if (Timer == 1f && Main.netMode != 1)
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
                    Timer = 21f;
                    Projectile.ai[0] = num13;
                    Projectile.netUpdate = true;
                }
            }
            else if (Timer > 20f && Timer < 200f)
            {
                Timer += 1f;
                int num17 = (int)Projectile.ai[0];
                if (!Main.player[num17].active || Main.player[num17].dead)
                {
                    Timer = 1f;
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
            if (Timer >= 1f && Timer < 20f)
            {
                Timer += 1f;
                if (Timer == 20f)
                {
                    Timer = 1f;
                }
            }
            if (Main.rand.NextBool(20))
            {
                Vector2 offset = Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.height / 3);
                Particle p = new BloodParticle(Projectile.Center + offset, offset.SafeNormalize(-Vector2.UnitY) * Main.rand.NextFloat(2, 5), 20, 1, Color.Green);
                GeneralParticleHandler.SpawnParticle(p);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int num = texture.Height / Main.projFrames[Type];
            int y = Projectile.frame * num;
            Rectangle rectangle = new(0, y, texture.Width, num);
            Vector2 origin = rectangle.Size() / 2f;
            SpriteEffects effects = ((Projectile.spriteDirection <= 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < trailLength; i++)
            {
                Color oldColor = Color.Green * 0.75f;
                oldColor *= (float)(trailLength - i) / trailLength;
                Vector2 oldPos = Projectile.oldPos[i] + Projectile.Size / 2;
                float oldRot = Projectile.oldRot[i];
                Main.spriteBatch.Draw(texture, oldPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(oldColor),
                    oldRot, origin, Projectile.scale, effects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, texture);
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // Makes a dust effect.
            for (int dustIndex = 0; dustIndex < 40; dustIndex++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Demonite, 0, 0, 0, default, 0.5f);
            }
            target.AddBuff(ModContent.BuffType<BrainRot>(), 60 * 4);
        }
    }
}
