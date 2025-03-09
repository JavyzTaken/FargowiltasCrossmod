using System;
using System.IO;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.NPCs.DevourerofGods;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Common.Bosses.Mutant
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MutantDoGHead : ModProjectile
    {
        public override string Texture => "CalamityMod/NPCs/DevourerofGods/DevourerofGodsHead";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("The Destroyer");
        }

        public override void SetDefaults()
        {
            Projectile.width = 104;
            Projectile.height = 104;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.netImportant = true;
            CooldownSlot = 1;
        }
        private Particle particle;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num214 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, num214),
                Projectile.GetAlpha(Color.White), Projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }

        public override void AI()
        {
            ref float timer = ref Projectile.localAI[1];
            //keep the head looking right
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57079637f;
            Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

            //const int homingDelay = 60;
            float desiredFlySpeedInPixelsPerFrame = 10 * Projectile.ai[1];
            float amountOfFramesToLerpBy = 25 / Projectile.ai[1]; // minimum of 1, please keep in full numbers even though it's a float!

            const int StartupTime = 30;
            const int DashWindup = 60;
            const int DashTime = 35;


            if (timer == StartupTime && Projectile.timeLeft > StartupTime)
            {
                particle = new ExpandingBloomParticle(Projectile.Center, Projectile.velocity, Color.DeepPink, 30f * Vector2.One, Vector2.Zero, DashWindup + 5, true, Color.Lavender);
                particle.Spawn();
            }
            if (particle != null)
            {
                particle.Position = Projectile.Center;
            }
            if (timer < StartupTime + DashWindup)
            {
                int foundTarget = (int)Projectile.ai[0];

                Player p = Main.player[foundTarget];
                
                double s = p.Center.X - Projectile.Center.X;
                Vector2 desiredPos = p.Center + Vector2.UnitY * Math.Sign(Projectile.Center.Y - p.Center.Y) * 450;
                Projectile.velocity = FargoSoulsUtil.SmartAccel(Projectile.Center, desiredPos, Projectile.velocity, 2f, 2f);
            }
            if (timer == StartupTime + DashWindup && Projectile.timeLeft > DashTime)
            {
                int foundTarget = (int)Projectile.ai[0];
                Player p = Main.player[foundTarget];
                //float speed = 40f;
                //Projectile.velocity = Projectile.DirectionTo(p.Center) * speed;
                Projectile.velocity *= 0.25f;
                Projectile.ai[2] = -Math.Sign(Projectile.Center.Y - p.Center.Y);

                SoundEngine.PlaySound(DevourerofGodsHead.AttackSound, Projectile.Center);
                /*
                Vector2 desiredVelocity = Projectile.DirectionTo(p.Center) * desiredFlySpeedInPixelsPerFrame;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                */
            }

            if (timer > StartupTime + DashWindup)
            {
                int foundTarget = (int)Projectile.ai[0];

                Player p = Main.player[foundTarget];
                Projectile.velocity.Y *= 0.96f;
                int signToPlayer = Math.Sign(p.Center.Y - Projectile.Center.Y);
                if (timer < StartupTime + DashWindup + 7 || Projectile.velocity.Y.NonZeroSign() == signToPlayer)
                    Projectile.velocity.Y += signToPlayer * 3f;

                Projectile.velocity.X *= 0.9f;
                /*
                if (Projectile.velocity.Length() > desiredFlySpeedInPixelsPerFrame)
                {
                    Projectile.velocity *= 0.97f;
                }
                */
            }
            if (timer > StartupTime + DashWindup + DashTime)
            {
                timer = StartupTime - 2;
            }
            timer++;


            const float IdleAccel = 0.05f;
            foreach (Projectile p in Main.projectile.Where(p => p.active && p.type == Projectile.type && p.whoAmI != Projectile.whoAmI && p.Distance(Projectile.Center) < Projectile.width))
            {
                Projectile.velocity.X += IdleAccel * (Projectile.position.X < p.position.X ? -1 : 1);
                Projectile.velocity.Y += IdleAccel * (Projectile.position.Y < p.position.Y ? -1 : 1);
                p.velocity.X += IdleAccel * (p.position.X < Projectile.position.X ? -1 : 1);
                p.velocity.Y += IdleAccel * (p.position.Y < Projectile.position.Y ? -1 : 1);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 200, true, false);
            target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 600, true, false);
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
        }

        public override void OnKill(int timeLeft)
        {
            particle = null;
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 2f;
                dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.PinkTorch, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100);
                Main.dust[dust].velocity *= 2f;
            }
            //int g = Gore.NewGore(Projectile.Center, Projectile.velocity / 2, mod.GetGoreSlot("Assets/Gores/DestroyerGun/DestroyerHead"), Projectile.scale);
            // Main.gore[g].timeLeft = 20;
            SoundEngine.PlaySound(DevourerofGodsHead.DeathExplosionSound, Projectile.Center);
        }
    }
}
