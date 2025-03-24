
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
using FargowiltasSouls.Content.Bosses.CursedCoffin;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Patreon.DanielTheRobot;
using FargowiltasSouls.Content.WorldGeneration;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
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
using static CalamityMod.World.CustomActions;

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

                if (brothers == 1 && !NPC.AnyNPCs(ModContent.NPCType<SoulSeeker>()))
                    return 1; // treat as in phase 1 if transition not done yet

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
            ChargedGigablast,
            Transition,
            Artillery,
            Gigablasts,
            Bombs,
        }
        public List<States> Attacks
        {
            get
            {
                List<States> attacks = [States.ChargedGigablast];
                if (Phase == 3)
                {
                    attacks = [
                        States.Artillery,
                        States.Gigablasts,
                        States.Bombs
                        ];
                }
                return attacks;
            }
        }
        public List<int> AvailableStates = [];
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
            NPC.BossBar = ModContent.GetInstance<BrothersBossBar>();
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
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override bool PreAI()
        {
            #region Standard
            // Emit light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1f, 0f, 0f);
            Main.dayTime = false;
            Main.time = Main.nightLength / 2;

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
            if (CustomRotation == 0)
            {
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
            }
            CustomRotation = 0;
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
                case States.Transition:
                    Transition();
                    break;
                case States.Artillery:
                    Artillery();
                    break;
                case States.Gigablasts:
                    Gigablasts();
                    break;
                case States.Bombs:
                    Bombs();
                    break;
            }

            return false;
        }
        #region States
        public void Intro()
        {
            NPC.dontTakeDamage = true;
            int introTime = 90;
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
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CalcloneFireBackground>(), 0, 0, Main.myPlayer, ai0: NPC.whoAmI);

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
            if (Phase >= 3)
            {
                Reset();
                State = (int)States.Transition;
                return;
            }
            Timer++;
            float distance = 660f;
            int telegraphStart = 60 * 6 + 30;
            int shot = 60 * 8;
            if (Phase >= 2)
            {
                distance = 660f;
                if (Timer > 60)
                {
                    int freq = WorldSavingSystem.MasochistModeReal ? 55 : 70;
                    if (Timer % freq == freq - 1)
                    {
                        if (DLCUtils.HostCheck)
                        {
                            float projectileVelocity = WorldSavingSystem.MasochistModeReal ? 12f : 9f;
                            int type = ModContent.ProjectileType<BrimstoneFireball>();
                            Vector2 fireballVelocity = Vector2.Normalize(Target.Center - NPC.Center) * projectileVelocity;
                            Vector2 balloffset = Vector2.Normalize(fireballVelocity) * 40f;
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + balloffset, fireballVelocity, type, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1f), 0f, Main.myPlayer, Target.position.X, Target.position.Y);
                            Main.projectile[proj].netUpdate = true;
                        }
                    }
                }
            }
            else
            {
                if (Timer > telegraphStart)
                {
                    distance = 350f;
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
                if (Timer > shot)
                {
                    Timer = 0;
                    ScreenShakeSystem.StartShake(8f);
                    Vector2 dir = (NPC.rotation + MathHelper.PiOver2).ToRotationVector2();
                    if (DLCUtils.HostCheck)
                    {
                        Vector2 projPos = NPC.Center;

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projPos, dir * 2f, ModContent.ProjectileType<ChargedGigablast>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1.25f), 1f, Main.myPlayer, ai2: Target.whoAmI);
                    }
                    NPC.velocity -= dir * 10;
                }
            }
            NeutralMovement(distance);
        }
        public void Transition()
        {
            Timer++;
            int transTime = 80;
            int endTime = 30;
            if (Timer < transTime - 15)
            {
                NeutralMovement();
            }
            else
                NPC.velocity *= 0.92f;
            if (Timer < transTime)
                NPC.dontTakeDamage = true;
            else
                NPC.dontTakeDamage = false;
            if (Timer == transTime)
            {
                NPC.netUpdate = true;
                SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                ScreenShakeSystem.StartShake(20f);
                if (DLCUtils.HostCheck)
                {
                    float speed = 36;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CalcloneWave>(), 0, 0, Main.myPlayer, ai2: speed);
                }
            }
            if (Timer >= transTime + endTime)
                GoToNeutral();

        }
        public void Artillery()
        {
            // movement
            float distance = 200f;
            Vector2 pos = Target.Center;
            float offsetDir = Target.DirectionTo(NPC.Center).ToRotation();

            Vector2 offset = offsetDir.ToRotationVector2() * distance;

            offset = offset.RotateTowards(-Vector2.UnitY.ToRotation(), 0.15f);
            pos += offset;
            float speed = 0.65f;
            if (Target.Center.Y < NPC.Center.Y - 50)
                speed = 1.4f;
            else if (Target.Distance(NPC.Center) < distance)
                NPC.velocity.Y *= 0.92f;
            Movement(pos, speed);

            // attack

            Vector2 CalculateAngle(float xPos)
            {
                float gravity = 0.68f;

                float xDif = xPos - NPC.Center.X;
                float yDif = Target.Center.Y - NPC.Center.Y;

                float velY = -18; // initial y vel
                float arcTop = yDif - 290;

                do
                {
                    arcTop -= 10;
                    if (yDif < 0) // if player is above
                    {

                        // calculate initial y vel that results in good arc above
                        if (-arcTop * gravity >= 0) // imaginary sqrt is BAD
                        {
                            float newVelY = -MathF.Sqrt(-arcTop * gravity) / 1.5f;
                            if (newVelY < velY)
                                velY = newVelY;
                        }
                    }
                }
                while (MathF.Pow(velY / gravity, 2) + (2 * yDif / gravity) < 0);

                float sqrtNum = MathF.Pow(velY / gravity, 2) + (2 * yDif / gravity);

                if (sqrtNum < 0) // imaginary sqrt is BAD
                    sqrtNum = 0;
                float t = -velY / gravity + MathF.Sqrt(sqrtNum);
                float velX = xDif / t;
                return velX * Vector2.UnitX + velY * Vector2.UnitY;
            }
            float targetX = Target.Center.X + Target.velocity.X * 58;
            Vector2 dir = CalculateAngle(targetX);

            CustomRotation = 1;
            NPC.rotation = NPC.rotation.ToRotationVector2().RotateTowards(dir.ToRotation() - MathHelper.PiOver2, 0.05f).ToRotation();
            //NPC.rotation = Vector2.Lerp(NPC.rotation.ToRotationVector2(), dir.RotatedBy(-MathHelper.PiOver2), 0.001f).ToRotation();

            int startup = 60;
            int frequency = 11;
            if (WorldSavingSystem.MasochistModeReal)
                frequency = 9;
            int totalTime = 60 * 6;
            int endTime = 20;
            Timer++;
            if (Timer > startup)
            {
                ScreenShakeSystem.SetUniversalRumble(2.4f, 6.28f, null, 0.2f);
            }
            if (Timer > startup && Timer % frequency == 0 && Timer < totalTime)
            {
                if (DLCUtils.HostCheck)
                {
                    float maxRandom = 550;
                    if (WorldSavingSystem.MasochistModeReal)
                        maxRandom = 610;
                    float random = Main.rand.NextFloat(-maxRandom, maxRandom);
                    if (Timer % (frequency * 3) == 0)
                        random = 0;
                    Vector2 randomDir = CalculateAngle(targetX + random);

                    Vector2 balloffset = dir.SafeNormalize(-Vector2.UnitY) * 40f;

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + balloffset, randomDir, ModContent.ProjectileType<BrimstoneArtillery>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 1f, Main.myPlayer, Target.position.X, Target.position.Y);

                }
            }
            if (Timer > totalTime + endTime)
                GoToNeutral();
        }
        public void Gigablasts()
        {
            Timer++;
            float startup = 30f;
            float ballTelegraph = WorldSavingSystem.MasochistModeReal ? 30f : 60f;
            float shotTime = 60 * 3f;
            float volleys = 3;
            float volleyTime = shotTime + ballTelegraph;
            float shotFrequency = 19f;
            float endlag = WorldSavingSystem.MasochistModeReal ? 0f : 20f;
            float timeToEndlag = startup + volleys * volleyTime;

            float distance = 660f;
            if (Timer < startup || Timer > timeToEndlag)
            {
                distance = 200f;
            }
            else
            {
                float volleyTimer = (Timer - startup) % volleyTime;
                if (volleyTimer < ballTelegraph)
                {
                    distance = 350f;
                    // particle telegraph
                    float progress = volleyTimer / ballTelegraph;
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
                else if (volleyTimer == ballTelegraph)
                {
                    Vector2 dir = (NPC.rotation + MathHelper.PiOver2).ToRotationVector2();
                    ScreenShakeSystem.StartShake(8f);
                    if (DLCUtils.HostCheck)
                    {
                        Vector2 projPos = NPC.Center;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projPos, dir * 2f, ModContent.ProjectileType<Gigablast>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1f), 1f, Main.myPlayer, ai2: Target.whoAmI);
                    }
                    NPC.velocity -= dir * 10;
                }
                else
                {
                    if (volleyTimer % shotFrequency == shotFrequency - 1)
                    {
                        if (DLCUtils.HostCheck)
                        {
                            float projectileVelocity = 14f;
                            int type = ModContent.ProjectileType<BrimstoneFireball>();
                            Vector2 fireballVelocity = Vector2.Normalize(Target.Center - NPC.Center) * projectileVelocity;
                            Vector2 balloffset = Vector2.Normalize(fireballVelocity) * 40f;
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + balloffset, fireballVelocity, type, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1f), 0f, Main.myPlayer, Target.position.X, Target.position.Y);
                            Main.projectile[proj].netUpdate = true;
                        }
                    }
                }
            }
            if (Timer > timeToEndlag + endlag)
            {
                GoToNeutral();
                return;
            }
            NeutralMovement(distance);
        }
        public void Bombs()
        {
            Timer++;
            float startup = 30f;
            float ballTelegraph = 50f;
            float shotTime = 60 * 0.8f;
            float volleys = 4;
            float volleyTime = shotTime + ballTelegraph;
            if (WorldSavingSystem.MasochistModeReal)
            {
                startup = 30f;
                ballTelegraph = 50f;
                shotTime = 60 * 0.65f;
                volleys = 6;
                volleyTime = shotTime + ballTelegraph;
            }

            float distance = 450f;
            if (Timer < startup)
            {
                distance = 450f;
            }
            else
            {
                float volleyTimer = (Timer - startup) % volleyTime;
                if (volleyTimer < ballTelegraph)
                {
                    // particle telegraph
                    float progress = volleyTimer / ballTelegraph;
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
                else if (volleyTimer == ballTelegraph)
                {
                    Vector2 dir = (NPC.rotation + MathHelper.PiOver2).ToRotationVector2();
                    ScreenShakeSystem.StartShake(8f);
                    if (DLCUtils.HostCheck)
                    {
                        Vector2 projPos = NPC.Center;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projPos, dir * 7f, ModContent.ProjectileType<Fireblast>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1f), 1f, Main.myPlayer);
                    }
                    for (int i = 0; i < 50; i++)
                    {
                        Dust failShotDust = Dust.NewDustPerfect(NPC.Center, Main.rand.NextBool(3) ? 60 : 114);
                        failShotDust.noGravity = true;
                        failShotDust.velocity = dir.RotatedByRandom(MathHelper.PiOver2 * 0.5f) * 30 * Main.rand.NextFloat(0.5f, 1.3f);
                        failShotDust.scale = Main.rand.NextFloat(0.9f, 1.8f);
                        Vector2 velocity = dir.RotatedByRandom(MathHelper.PiOver2 * 0.5f) * 26;
                        PointParticle spark2 = new(NPC.Center + velocity, velocity * Main.rand.NextFloat(0.3f, 1f), false, 15, 1.25f, (Main.rand.NextBool() ? Color.Lerp(Color.Red, Color.Magenta, 0.5f) : Color.Red) * 0.6f);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    NPC.velocity -= dir * 10;
                    /*
                    for (int i = -1; i <= 1; i++)
                    {
                        if (DLCUtils.HostCheck)
                        {
                            Vector2 projPos = NPC.Center;
                            Vector2 dir = (NPC.rotation + MathHelper.PiOver2 + i * MathHelper.PiOver4 * 1.4f).ToRotationVector2();
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), projPos, dir * 5f, ModContent.ProjectileType<Fireblast>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1f), 1f, Main.myPlayer);
                        }
                    }
                    */
                }
                else
                {
                    distance = 200f;
                }
            }
            if (Timer > startup + volleys * volleyTime)
            {
                GoToNeutral();
                return;
            }
            NeutralMovement(distance);
        }
        #endregion

        #region Help Methods
        public void GoToNeutral()
        {
            Reset();
            State = (int)((States)State switch
            {
                States.Artillery => States.Gigablasts,
                States.Gigablasts => States.Bombs,
                _ => States.Artillery
            });
            NPC.netUpdate = true;
        }
        public void Reset()
        {
            Timer = 0;
            NPC.netUpdate = true;
        }
        public void NeutralMovement(float distance = 660f)
        {
            Vector2 pos = Target.Center;
            float offsetDir = Target.DirectionTo(NPC.Center).ToRotation();

            // get far away from brothers so it doesn't hit them for free
            NPC[] brothers = [Cataclysm, Catastrophe];

            Vector2 offset = offsetDir.ToRotationVector2() * distance;
            pos += offset;
            for (int i = 0; i < brothers.Length; i++)
            {
                if (brothers[i] == null) continue;
                int minDistance = 660;
                if (pos.Distance(brothers[i].Center) < minDistance)
                    pos = brothers[i].Center + brothers[i].DirectionTo(pos) * minDistance;
            }
            pos -= offset;

            if (Math.Abs(FargoSoulsUtil.RotationDifference(offset, -Vector2.UnitY)) > MathHelper.PiOver2 * 0.7f)
                offset = offset.RotateTowards(-Vector2.UnitY.ToRotation(), 0.07f);
            pos += offset;
            float speed = 0.6f;
            if (Target.Distance(NPC.Center) < distance)
                speed /= 3;
            if (Phase == 2)
                speed *= 1.5f;
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
