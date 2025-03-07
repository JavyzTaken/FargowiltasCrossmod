using System;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Particles;
using FargowiltasSouls;
using CalamityMod.NPCs.CalClone;
using CalamityMod;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ChargedGigablast : ModProjectile
    {
        bool withinRange = false;
        public override string Texture => "CalamityMod/Projectiles/Boss/SCalBrimstoneGigablast";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.hostile = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60 * 6;
            Projectile.Opacity = 0f;
            Projectile.tileCollide = false;

            Projectile.scale = 1.5f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 5)
                Projectile.frame = 0;

            if (Projectile.Opacity < 1f)
                Projectile.Opacity += 0.05f;


            Lighting.AddLight(Projectile.Center, 0.9f * Projectile.Opacity, 0f, 0f);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            int target = (int)Projectile.ai[2];
            if (!target.IsWithinBounds(Main.maxPlayers))
            {
                Projectile.Kill();
                return;
            }

            

            if (!withinRange && Main.player[target].Alive())
            {
                if (Projectile.Distance(Main.player[target].Center) < 400 && Projectile.localAI[2] == 0)
                {
                    Projectile.velocity.Normalize();
                    Projectile.localAI[2] = 1;
                }
                    
                Vector2 vectorToIdlePosition = Main.player[target].Center - Projectile.Center;
                float speed = 14f;
                float inertia = 45f;
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
                const int MaxSpeed = 35;
                Projectile.velocity.ClampMagnitude(0, MaxSpeed);
            }

            float targetDist;
            if (target != -1 && !Main.player[target].dead && Main.player[target].active && Main.player[target] != null)
                targetDist = Vector2.Distance(Main.player[target].Center, Projectile.Center);
            else
                targetDist = 1000;

            if (!withinRange && Main.rand.NextBool())
            {
                SparkParticle orb = new SparkParticle(Projectile.Center - Projectile.velocity + Main.rand.NextVector2Circular(30, 30), -Projectile.velocity * Main.rand.NextFloat(0.1f, 1f), false, 14, Main.rand.NextFloat(0.5f, 0.75f), (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * Projectile.Opacity);
                GeneralParticleHandler.SpawnParticle(orb);
            }
            if ((Projectile.timeLeft < 120 && !withinRange) || (targetDist < 150))
            {
                withinRange = true;
            }
            if (withinRange)
            {
                if (Projectile.velocity.Length() < 20)
                    Projectile.velocity *= 1.02f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int drawStart = frameHeight * Projectile.frame;
            lightColor.R = (byte)(255 * Projectile.Opacity);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.type == ModContent.NPCType<Cataclysm>() || target.type == ModContent.NPCType<Catastrophe>())
                return base.CanHitNPC(target);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FlatBonusDamage += 1400;
        }
        public override bool CanHitPlayer(Player target) => Projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.Opacity != 1f)
                return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
            OnhitVisuals();
            Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SCalBrimstoneGigablast.ImpactSound, Projectile.Center);
            OnhitVisuals();

            var ai = target.GetDLCBehavior<CalamitasBrothersEternity>();
            ai.GetStunned();
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, target.whoAmI);

            Projectile.Kill();
        }
        public void OnhitVisuals()
        {
            for (int i = 0; i < 25; i++)
            {
                Vector2 velocity = new Vector2(15, 15).RotatedByRandom(100);
                PointParticle spark2 = new PointParticle(Projectile.Center + velocity, velocity * Main.rand.NextFloat(0.3f, 1f), false, 15, 1.25f, (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * 0.6f);
                GeneralParticleHandler.SpawnParticle(spark2);
            }
            for (int i = 0; i < 25; i++)
            {
                Dust failShotDust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 60 : 114);
                failShotDust.noGravity = true;
                failShotDust.velocity = new Vector2(20, 20).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.3f);
                failShotDust.scale = Main.rand.NextFloat(0.9f, 1.8f);
            }
        }
    }
}
