
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.Perforator;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Patreon.DanielTheRobot;
using FargowiltasSouls.Core.NPCMatching;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamitasCloneEternity : CalDLCEmodeBehavior
    {
        public const bool Enabled = true;
        #region Fields
        public Player Target => Main.player[NPC.target];
        public static float Acceleration => 0.8f;
        public static float MaxMovementSpeed => 18f;

        public ref float Timer => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];

        public ref float CustomRotation => ref NPC.localAI[0];

        public int Phase
        {
            get
            {
                if (State == (int)States.Intro)
                    return 1;
                int brothers = 0;
                if (Cataclysm != null && Cataclysm.Alive())
                    brothers++;
                if (Catastrophe != null && Catastrophe.Alive())
                    brothers++;
                return brothers switch
                {
                    2 => 1,
                    1 => 2,
                    _ => 3
                };
            }
        }

        public enum States
        {
            Intro,
            ChargedGigablast
        }
        public List<States> Attacks
        {
            get
            {
                List<States> attacks = [States.ChargedGigablast];
                return attacks;
            }
        }
        public NPC Cataclysm => FargoSoulsUtil.NPCExists(CalamityGlobalNPC.cataclysm, ModContent.NPCType<Cataclysm>());
        public NPC Catastrophe => FargoSoulsUtil.NPCExists(CalamityGlobalNPC.catastrophe, ModContent.NPCType<Catastrophe>());
        #endregion
        public override bool IsLoadingEnabled(Mod mod) => Enabled;
        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.CalClone.CalamitasClone>();
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            NPC.scale = 1f;

            NPC.defense = 25;
            NPC.DR_NERD(0.15f);
            int hp = 25000;
            NPC.LifeMaxNERB(hp, hp, 520000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
        }

        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            for (int i = 0; i < NPC.localAI.Length; i++)
            {
                binaryWriter.Write(NPC.localAI[i]);
            }
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            for (int i = 0; i < NPC.localAI.Length; i++)
            {
                NPC.localAI[i] = binaryReader.ReadSingle();
            }
        }

        public override bool PreAI()
        {
            #region Standard
            // Emit light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1f, 0f, 0f);

            // Don't take damage if any brothers are alive
            NPC.dontTakeDamage = Main.npc.Any(n => n.TypeAlive<Cataclysm>() || n.TypeAlive<Catastrophe>());

            CalamityGlobalNPC.calamitas = NPC.whoAmI;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Target.dead || !Target.active)
                NPC.TargetClosest();


            // Despawn
            if (!Target.active || Target.dead)
            {
                NPC.TargetClosest(false);
                if (!Target.active || Target.dead)
                {
                    if (NPC.velocity.Y > 3f)
                        NPC.velocity.Y = 3f;
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.velocity.Y < -12f)
                        NPC.velocity.Y = -12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[1] != 0f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.Calamity().newAI[2] = 0f;
                        NPC.Calamity().newAI[3] = 0f;
                        NPC.alpha = 0;
                        NPC.netUpdate = true;
                    }
                    return false;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;
            #endregion

            #region Rotation
            // Rotation
            Vector2 npcCenter = new Vector2(NPC.Center.X, NPC.position.Y + NPC.height - 59f);
            Vector2 lookAt = new Vector2(Target.position.X - (Target.width / 2), Target.position.Y - (Target.height / 2));
            Vector2 rotationVector = npcCenter - lookAt;


            float rotation = (float)Math.Atan2(rotationVector.Y, rotationVector.X) + MathHelper.PiOver2;
            if (rotation < 0f)
                rotation += MathHelper.TwoPi;
            else if (rotation > MathHelper.TwoPi)
                rotation -= MathHelper.TwoPi;

            float rotationAmt = 0.1f;
            if (NPC.rotation < rotation)
            {
                if ((rotation - NPC.rotation) > MathHelper.Pi)
                    NPC.rotation -= rotationAmt;
                else
                    NPC.rotation += rotationAmt;
            }
            else if (NPC.rotation > rotation)
            {
                if ((NPC.rotation - rotation) > MathHelper.Pi)
                    NPC.rotation += rotationAmt;
                else
                    NPC.rotation -= rotationAmt;
            }

            if (NPC.rotation > rotation - rotationAmt && NPC.rotation < rotation + rotationAmt)
                NPC.rotation = rotation;
            if (NPC.rotation < 0f)
                NPC.rotation += MathHelper.TwoPi;
            else if (NPC.rotation > MathHelper.TwoPi)
                NPC.rotation -= MathHelper.TwoPi;
            if (NPC.rotation > rotation - rotationAmt && NPC.rotation < rotation + rotationAmt)
                NPC.rotation = rotation;
            #endregion

            if (!NPC.HasPlayerTarget) // just in case
                return false;
            switch ((States)State)
            {
                case States.Intro:
                    Intro();
                    break;
                case States.ChargedGigablast:
                    ChargedGigablast();
                    break;
            }

            return false;
        }
        #region States
        public void Intro()
        {
            NPC.dontTakeDamage = true;
            int introTime = 60 * 3;
            if (Timer < introTime)
            {
                NeutralMovement();
            }
            if (Timer == introTime + 30)
            {
                SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                ScreenShakeSystem.StartShake(20f);
                if (DLCUtils.HostCheck)
                {
                    float speed = 20;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CalcloneWave>(), 0, 0, Main.myPlayer, ai2: speed);

                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Cataclysm>());
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Catastrophe>());
                }
            }
            if (++Timer > introTime + 90)
            {
                State = (int)States.ChargedGigablast;
                Timer = 0;
            }
        }
        public void ChargedGigablast()
        {
            NeutralMovement();
        }
        #endregion

        #region Help Methods
        public void NeutralMovement()
        {
            float distance = 660f;

            Timer++;
            int telegraphStart = 60 * 6 + 30;
            int shot = 60 * 8;
            if (Phase >= 2)
            {
                telegraphStart += 60 * 16;
                shot += 60 * 16;
            }
            if (Timer > telegraphStart)
            {
                distance = 400f;
                // particle telegraph
                float progress = (Timer - telegraphStart) / (shot - telegraphStart);
                float freq = 10f - 8f * progress;
                float rand = MathHelper.PiOver4 * 0.4f;
                Vector2 dir = (NPC.rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-rand, rand)).ToRotationVector2();
                if (Main.rand.NextBool((int)freq))
                {
                    Vector2 velocity = dir * 20;
                    PointParticle spark2 = new(NPC.Center + velocity, velocity * Main.rand.NextFloat(0.3f, 1f), false, 15, 1.25f, (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * 0.6f);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                if (Main.rand.NextBool((int)freq))
                {
                    Dust failShotDust = Dust.NewDustPerfect(NPC.Center, Main.rand.NextBool(3) ? 60 : 114);
                    failShotDust.noGravity = true;
                    failShotDust.velocity = dir * 22 * Main.rand.NextFloat(0.5f, 1.3f);
                    failShotDust.scale = Main.rand.NextFloat(0.9f, 1.8f);
                }
            }
            else
            {
                if (Phase >= 2 && Timer > 60)
                {
                    int freq = 40;
                    if (Phase == 3)
                        freq = 20;
                    if (Timer % freq == freq - 1)
                    {
                        float projectileVelocity = 14f;
                        int type = ModContent.ProjectileType<BrimstoneHellfireball>();
                        Vector2 fireballVelocity = Vector2.Normalize(Target.Center - NPC.Center) * projectileVelocity;
                        Vector2 balloffset = Vector2.Normalize(fireballVelocity) * 40f;
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + balloffset, fireballVelocity, type, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1f), 0f, Main.myPlayer, Target.position.X, Target.position.Y);
                        Main.projectile[proj].netUpdate = true;
                    }
                }
            }
            if (Timer > shot)
            {
                Timer = 0;
                if (DLCUtils.HostCheck)
                {
                    Vector2 projPos = NPC.Center;
                    Vector2 dir = (NPC.rotation + MathHelper.PiOver2).ToRotationVector2();
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projPos, dir * 2f, ModContent.ProjectileType<ChargedGigablast>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1.25f), 1f, Main.myPlayer, ai2: Target.whoAmI);
                }
            }

            Vector2 pos = Target.Center;
            float offsetDir = Target.DirectionTo(NPC.Center).ToRotation();

            // get far away from brothers so it doesn't hit them for free
            NPC[] brothers = [Cataclysm, Catastrophe];

            Vector2 offset = offsetDir.ToRotationVector2() * distance;
            pos += offset;
            for (int i = 0; i < brothers.Length; i++)
            {
                if (brothers[i] == null) continue;
                int minDistance = 700;
                if (pos.Distance(brothers[i].Center) < minDistance)
                    pos = brothers[i].Center + brothers[i].DirectionTo(pos) * minDistance;
            }
            pos -= offset;

            if (Math.Abs(FargoSoulsUtil.RotationDifference(offset, -Vector2.UnitY)) > MathHelper.PiOver2 * 0.7f)
                offset = offset.RotateTowards(-Vector2.UnitY.ToRotation(), 0.07f);
            pos += offset;
            float speed = 0.4f;
            if (Target.Distance(NPC.Center) < distance)
                speed /= 2;
            Movement(pos, speed);
        }
        public void Movement(Vector2 desiredPos, float speedMod)
        {
            speedMod *= 1.6f;
            float accel = Acceleration * speedMod;
            float decel = Acceleration * 2 * speedMod;
            float max = MaxMovementSpeed * speedMod;
            if (max > Target.velocity.Length() + MaxMovementSpeed)
                max = Target.velocity.Length() + MaxMovementSpeed;
            float resistance = NPC.velocity.Length() * accel / max;
            NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, desiredPos, NPC.velocity, accel - resistance, decel + resistance);
        }
        #endregion
    }
}
