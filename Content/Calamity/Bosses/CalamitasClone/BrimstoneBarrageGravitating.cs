using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    public class BrimstoneBarrageGravitating : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/BrimstoneBarrage";
        public int time = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 44;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 690;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {

            int target = Player.FindClosest(Projectile.Center, 1, 1);

            float targetDist;
            if (target != -1 && !Main.player[target].dead && Main.player[target].active && Main.player[target] != null)
                targetDist = Vector2.Distance(Main.player[target].Center, Projectile.Center);
            else
                targetDist = 1000;

            if (Projectile.velocity.Length() < 35)
            {
                Projectile.velocity += Projectile.ai[2].ToRotationVector2() * 0.5f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            if (Projectile.timeLeft < 60)
                Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft / 60f, 0f, 1f);


            if (Projectile.ai[1] == 2f || (Projectile.ai[1] == 4f && time > 10))
            {
                if (targetDist < 1400f)
                {
                    SparkParticle orb = new SparkParticle(Projectile.Center - Projectile.velocity * 0.5f, -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.6f), false, (int)MathHelper.Clamp(9 * Utils.GetLerpValue(630, 690, Projectile.timeLeft), 2, 9), 1.1f, (Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.5f)) * Projectile.Opacity * 0.85f);
                    GeneralParticleHandler.SpawnParticle(orb);
                }
            }

            if (Projectile.ai[1] == 3f)
            {
                if (Projectile.timeLeft > 600)
                    Projectile.velocity *= 1.015f;

                Projectile.scale = 0.85f;

                Dust trailDust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), 182);
                trailDust.noGravity = true;
                trailDust.velocity = (-Projectile.velocity * 0.5f) * Main.rand.NextFloat(0.1f, 0.9f);
                trailDust.scale = Main.rand.NextFloat(0.2f, 0.6f);
            }

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;

                if (Projectile.ai[0] == 0f)
                    Projectile.damage = Projectile.GetProjectileDamage(ModContent.NPCType<CalamityMod.NPCs.CalClone.CalamitasClone>());
            }

            Lighting.AddLight(Projectile.Center, 0.75f * Projectile.Opacity, 0f, 0f);
            time++;
        }

        public override bool CanHitPlayer(Player target) => Projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.Opacity != 1f)
                return;

            if (Projectile.ai[0] == 0f || Main.zenithWorld)
                target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 180);
            else
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);

            if (CalamityGlobalNPC.SCal != -1 && NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()) == true)
            {
                if (Main.npc[CalamityGlobalNPC.SCal].active)
                {
                    if (Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().cirrus)
                        lightColor.B = (byte)(255 * Projectile.Opacity);
                }
            }

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 18 * Projectile.scale, targetHitbox);
    }
}
