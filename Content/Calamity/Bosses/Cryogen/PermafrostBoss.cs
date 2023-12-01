
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls;
using FargowiltasCrossmod.Core.Utils;
using CalamityMod.CalPlayer;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod;
using CalamityMod;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [AutoloadBossHead]
    public class PermafrostBoss : ModNPC
    {
        //public override string Texture => "CalamityMod/NPCs/TownNPCs/DILF";

        public override bool IsLoadingEnabled(Mod mod) => CryogenEternity.Enabled;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.lifeMax = 30000;
            NPC.knockBackResist = 0;
            NPC.HitSound = new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenHit", 3);
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.damage = 50;
            Music = MusicLoader.GetMusicSlot("FargowiltasCrossmod/Assets/Music/Niflheimr");
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if ((Attacks)Attack != Attacks.PawCharge) return false;
            return base.CanHitPlayer(target, ref cooldownSlot);
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (Phase == 0)
                modifiers.FinalDamage *= 0.2f;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
            ref float shieldRot = ref NPC.localAI[0];
            shieldRot += 0.05f;
            Asset<Texture2D> t = TextureAssets.Npc[Type];
            Asset<Texture2D> encasement = ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/GlacialEmbraceBody");
            Asset<Texture2D> shield = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/IceClasperSummonProjectile");
            spriteBatch.Draw(t.Value, NPC.Center - screenPos, new Rectangle(6, 62, 32, 48), drawColor, NPC.rotation, new Vector2(16, 24), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            if (Phase == 0)
            {
                spriteBatch.Draw(encasement.Value, NPC.Center - screenPos + new Vector2(Main.rand.NextFloat(-Timer, Timer), Main.rand.NextFloat(-Timer, Timer)), null, drawColor * 0.7f, NPC.rotation, encasement.Size()/2, NPC.scale, SpriteEffects.None, 0);
            }
            for (int i = 0; i < 15; i++)
            {
                float rotation = MathHelper.ToRadians(360f / 15 * i) + shieldRot;
                if (Phase == 0.5f)
                {
                    float x = (Timer / 120);
                    spriteBatch.Draw(shield.Value, NPC.Center - screenPos + new Vector2(MathHelper.Lerp(1500, 60, (float)Math.Sin((x * Math.PI) / 2)), 0).RotatedBy(rotation), null, drawColor * 0.7f, rotation + MathHelper.PiOver2, shield.Size() / 2, NPC.scale *0.75f, SpriteEffects.None, 0);
                }
                if (Phase > 0.5f)
                {
                    spriteBatch.Draw(shield.Value, NPC.Center - screenPos + new Vector2(60, 0).RotatedBy(rotation), null, drawColor * 0.7f, rotation + MathHelper.PiOver2, shield.Size() / 2, NPC.scale * 0.75f, SpriteEffects.None, 0);
                }
            }
            /*
            for (int i = 0; i < 15; i++)
            {
                float rotation = -(MathHelper.ToRadians(360f / 15 * i) + shieldRot);
                if (Attack == (float)Attacks.PhaseTransition)
                {
                    float x = (Timer / 60);
                    spriteBatch.Draw(shield.Value, NPC.Center - screenPos + new Vector2(MathHelper.Lerp(1500, 120, (float)Math.Sin((x * Math.PI) / 2)), 0).RotatedBy(rotation), null, drawColor * 0.7f, rotation + MathHelper.PiOver2, shield.Size() / 2, NPC.scale * 0.75f, SpriteEffects.None, 0);
                }
                if (Phase >= 2f)
                {
                    spriteBatch.Draw(shield.Value, NPC.Center - screenPos + new Vector2(120, 0).RotatedBy(rotation), null, drawColor * 0.7f, rotation + MathHelper.PiOver2, shield.Size() / 2, NPC.scale * 0.75f, SpriteEffects.None, 0);
                }
            }
            */
            return false;
        }
        protected static void NetSync(NPC npc, bool onlySendFromServer = true)
        {
            if (onlySendFromServer && Main.netMode != NetmodeID.Server)
                return;

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }
        public ref float Phase => ref NPC.ai[0];
        public ref float Timer => ref NPC.ai[1];
        public ref float Attack => ref NPC.ai[2];
        public ref float Data => ref NPC.ai[3];

        public float PhaseTwoPercent = 0.6f;

        public int RitualProj;

        public enum Attacks
        {
            Idle,
            PhaseTransition,
            TridentToss,
            PawCharge,
            Blizzard,
            IceShotgun,
            PredictiveToss,
            IceArrows,
            FrostFlares,
            IceStar
        };
        public static List<Attacks> SetupAttacks = new List<Attacks>
        {
            Attacks.PredictiveToss,
            Attacks.FrostFlares,
            Attacks.IceStar
        };
        public static List<Attacks> FollowupAttacks = new List<Attacks>
        {
            Attacks.PawCharge,
            Attacks.IceShotgun,
            Attacks.IceArrows,
            Attacks.Blizzard,
            Attacks.TridentToss
        };
        public static Queue<Attacks> SetupAttackQueue = new Queue<Attacks>();
        public static Queue<Attacks> FollowupAttackQueue = new Queue<Attacks>();

        public Particle TelegraphParticle;

        public override void AI()
        {
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
                NetSync(NPC);
            }
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.velocity.Y += -1;
                return;
            }
            Player target = Main.player[NPC.target];
            Vector2 toTarget = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            if (Phase == 0)
            {
                Timer += 0.01f;
                if (Timer >= 3)
                {
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenShieldBreak"), NPC.Center);
                    for (int i = 0; i < 3; i++)
                    {
                        Gore.NewGoreDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -4).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)), ModCompatibility.Calamity.Mod.Find<ModGore>("CryoDeathGore" + Main.rand.Next(2, 4)).Type);
                        Gore.NewGoreDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModCompatibility.Calamity.Mod.Find<ModGore>("CryoShieldGore" + Main.rand.Next(1, 5)).Type);
                        Gore.NewGoreDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModCompatibility.Calamity.Mod.Find<ModGore>("CryoShieldGore" + Main.rand.Next(1, 5)).Type);
                    }

                    
                    Timer = 0;
                    Phase = 0.5f;
                }
            }
            if (Phase == 0.5f)
            {
                Timer++;
                if (Timer >= 120)
                {
                    Phase = 1;
                    Timer = 0;
                }
            }
            if (Phase >= 1)
            {
                NPC.spriteDirection = NPC.Center.X > target.Center.X ? 1 : -1;
                switch ((Attacks)Attack)
                {
                    case Attacks.Idle:
                        Idle();
                        break;
                    case Attacks.PhaseTransition:
                        PhaseTransition();
                        break;
                    case Attacks.TridentToss:
                        TridentToss();
                        break;
                    case Attacks.PawCharge:
                        PawCharge();
                        break;
                    case Attacks.Blizzard:
                        Blizzard();
                        break;
                    case Attacks.IceShotgun:
                        IceShotgun();
                        break;
                    case Attacks.PredictiveToss:
                        PredictiveToss();
                        break;
                    case Attacks.IceArrows:
                        IceArrows();
                        break;
                    case Attacks.FrostFlares:
                        FrostFlares();
                        break;
                    case Attacks.IceStar:
                        IceStar();
                        break;
                }
            }
            #region Help Methods
            void Movement(Vector2 pos, float accel = 0.03f, float maxSpeed = 20, float lowspeed = 5, float decel = 0.03f, float slowdown = 30)
            {
                if (NPC.Distance(pos) > slowdown)
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * maxSpeed, accel);
                }
                else
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * lowspeed, decel);
                }
            }
            void Reset()
            {
                if (NPC.GetLifePercent() < PhaseTwoPercent && Phase < 2)
                {
                    Attack = (float)Attacks.PhaseTransition;
                    Phase = 2;
                }
                else if (!SetupAttacks.Contains((Attacks)Attack) && Phase >= 2)
                {
                    if (SetupAttackQueue.Count <= 0)
                    {
                        List<Attacks> QueueAttacks = SetupAttacks;
                        SetupAttackQueue.RandomFromListExcept(SetupAttacks, (Attacks)Attack);
                    }
                    Attack = (float)SetupAttackQueue.Dequeue();
                }
                else
                {
                    if (FollowupAttackQueue.Count <= 0)
                    {
                        FollowupAttackQueue.RandomFromListExcept(FollowupAttacks, (Attacks)Attack);
                    }
                    Attack = (float)FollowupAttackQueue.Dequeue();
                }
                Timer = 0;
                Data = 0;
            }
            #endregion
            #region Attacks
            void Idle()
            {
                Vector2 targetpos = target.Center + new Vector2(NPC.Center.X > target.Center.X ? 400 : -400, 0);

                Movement(targetpos);
                Timer++;
                if (Timer >= 20)
                {
                    Timer = 0;
                    Reset();
                }
            }
            void PhaseTransition()
            {
                if (Timer == 10)
                    RitualProj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostRitual>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 0f, NPC.whoAmI);

                if (Timer > 60)
                {
                    Phase = 2;
                    Reset();
                }
                Timer++;
            }
            void TridentToss()
            {
                Timer++;
                if (Timer < 30)
                {
                    Vector2 targetpos = target.Center + new Vector2(NPC.Center.X > target.Center.X ? 400 : -400, -300);

                    Movement(targetpos, slowdown: 100, decel: 0.05f);
                }
                else
                {
                    NPC.velocity /= 1.05f;
                    if (Timer == 30 && DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostHeldWeapon>(), 0, 0, ai0: 0, ai1: toTarget.ToRotation(), ai2: NPC.whoAmI);
                    }
                    if (Timer == 60)
                    {
                        NPC.velocity = -toTarget * 20;
                        SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                        if (DLCUtils.HostCheck)
                        {

                            for (int i = -5; i < 6; i++)
                            {
                                Vector2 vel = toTarget;
                                if (i < 0) vel = vel.RotatedBy(MathHelper.PiOver2 - 1f);
                                if (i > 0) vel = vel.RotatedBy(MathHelper.PiOver2 + 1f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + toTarget * 50, vel * i * 15, ModContent.ProjectileType<IceTrident>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, ai0: i * 5, ai2: toTarget.ToRotation());
                            }
                        }
                    }
                    if (Timer >= 100)
                    {
                        Reset();
                    }
                }
            }
            void PawCharge()
            {
                if (Timer == 1 && DLCUtils.HostCheck)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostHeldWeapon>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, ai0: 2);

                const int WindupTime = 50;
                const int DashTime = 30;
                const int TotalTime = WindupTime + DashTime;
                const int Dashes = 3;
                int partialTimer = (int)Timer % TotalTime;

                if (Timer >= TotalTime * Dashes)
                {
                    Reset();
                    return;
                }

                if (partialTimer == 1)
                {
                    Data = toTarget.ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 6f, MathHelper.Pi / 6f);
                    TelegraphParticle = null;
                    TelegraphParticle = new ExpandingBloomParticle(NPC.Center, Vector2.Zero, Color.DarkCyan, Vector2.One * 20, Vector2.One, WindupTime, true, Color.LightBlue);
                    TelegraphParticle.Spawn();
                }
                if (TelegraphParticle != null)
                {
                    TelegraphParticle.Position = NPC.Center;
                }
                if (Timer <= 0)
                {
                    Timer++;
                    return;
                }
                if (partialTimer < WindupTime)
                {
                    Vector2 desiredPos = target.Center - (Data.ToRotationVector2() * 550);

                    Movement(desiredPos, 0.1f, 40, 10, 0.1f, 50f);
                }
                else if (partialTimer == WindupTime)
                {
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/SCalSounds/SCalDash"), NPC.Center);

                    Data = toTarget.ToRotation();
                }
                else
                {
                    Vector2 desiredPos = NPC.Center + Data.ToRotationVector2() * 400;
                    Movement(desiredPos, 0.2f, 40, 10, 0.1f, 50);

                    if (Timer % 10 == 0 && DLCUtils.HostCheck) 
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, Main.rand.Next(2, 5)).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<ArcticPaw>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0);
                }
                Timer++;
            }
            void Blizzard()
            {
                Movement(target.Center - toTarget * 400f);

                if (Timer == 0)
                {
                    Data = Main.rand.NextBool() ? 1 : -1;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostHeldWeapon>(), 0, 0, ai0: 3);
                }
                Vector2 pos = new Vector2(0, -1600).RotatedBy(MathHelper.Lerp(-0.9f * Data, 0.9f * Data, Timer / 180f));
                pos.Y /= 1.78f;
                if (DLCUtils.HostCheck)
                {
                    Vector2 position = target.Center + pos + pos.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * Main.rand.Next(-500, 500);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), position, -pos.SafeNormalize(Vector2.Zero) * 30, ModContent.ProjectileType<Blizzard>(), 0, 0, ai0: Main.rand.Next(0, 4));
                    if (Timer % 5 == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), position, -pos.SafeNormalize(Vector2.Zero) * 12, ModContent.ProjectileType<FrostShard>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer);
                    }
                }
                if (Timer % 5 == 0 && Main.player[Main.myPlayer].Distance(NPC.Center) < 5000)
                {
                    SoundEngine.PlaySound(SoundID.Item9, target.Center + pos);
                }
                Timer += 2;
                if (Timer >= 180)
                {
                    Reset();
                }
            }
            void IceShotgun()
            {
                ref float Corner = ref Data;

                const int AttackTime = 64; //keep divisible by 2
                const int Attacks = 2;

                
                if (Timer == 0)
                {
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostHeldWeapon>(), 0, 0, ai0: 1);

                    Vector2 fromTarget = -toTarget;
                    if (fromTarget.X >= 0 && fromTarget.Y >= 0)
                        Corner = 0;
                    else if (fromTarget.X >= 0 && fromTarget.Y < 0)
                        Corner = 1;
                    else if (fromTarget.X <= 0 && fromTarget.Y < 0)
                        Corner = 2;
                    else
                        Corner = 3;
                }
                else
                {
                    Vector2 desiredDir;
                    switch (Corner)
                    {
                        case 0:
                            desiredDir = Vector2.Normalize(Vector2.UnitX + Vector2.UnitY);
                            break;
                        case 1:
                            desiredDir = Vector2.Normalize(Vector2.UnitX - Vector2.UnitY);
                            break;
                        case 2:
                            desiredDir = Vector2.Normalize(-Vector2.UnitX - Vector2.UnitY);
                            break;
                        case 3:
                            desiredDir = Vector2.Normalize(-Vector2.UnitX + Vector2.UnitY);
                            break;
                        default:
                            desiredDir = Vector2.Normalize(Vector2.One);
                            break;
                    }
                    desiredDir *= 500;

                    Movement(target.Center + desiredDir, 0.1f, 40, 10, 0.1f, 50f);

                    if (Timer > 10 && Timer < AttackTime && NPC.Distance(target.Center + desiredDir) > 400)
                        Timer--;
                }
                /*
                if (TelegraphParticle != null)
                    TelegraphParticle.Position = NPC.Center;
                */

                if (Timer % AttackTime == AttackTime - 1)
                {
                    SoundEngine.PlaySound(SoundID.Item36, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, toTarget.RotatedByRandom(MathHelper.Pi / 7f) * Main.rand.NextFloat(15, 20), ModContent.ProjectileType<IceShot>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0);
                        }
                    }
                    Corner += Main.rand.NextBool() ? 1 : -1;
                    Corner %= 4;
                }
                if (Timer == AttackTime * Attacks + (AttackTime / 2))
                {
                    Reset();
                }
                Timer++;
            }
            void PredictiveToss()
            {
                Movement(target.Center + new Vector2(NPC.Center.X > target.Center.X ? 300 : -300, 0), maxSpeed: 35, slowdown: 100);
                Timer++;
                if (Timer == 1 && DLCUtils.HostCheck)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostHeldWeapon>(), 0, 0, Main.myPlayer, ai0: 4);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DarkIceCrystal>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer, ai0: NPC.whoAmI, ai1: target.whoAmI);
                    TelegraphParticle = null;
                    TelegraphParticle = new ExpandingBloomParticle(NPC.Center, Vector2.Zero, Color.DarkCyan, Vector2.One * 20, Vector2.One, 100, true, Color.LightBlue);
                    TelegraphParticle.Spawn();
                }
                if (TelegraphParticle != null)
                {
                    TelegraphParticle.Position = NPC.Center;
                }
                if (Timer == 90)
                {
                    SoundEngine.PlaySound(SoundID.Item109 with { Pitch = 0.5f }, NPC.Center);
                }
                if (Timer == 100)
                {
                    NPC.velocity = toTarget * 30;
                }
                if (Timer >= 120)
                {
                    Reset();
                }
            }
            void IceArrows()
            {
                Movement(target.Center, lowspeed: 0, slowdown: 300);
                Timer++;

                const int Attack1Time = 90;
                const int DelayTime = 30;
                const int Attack2Time = 20;
                const int AttackTime = Attack1Time + DelayTime + Attack2Time;
                const int Endlag = 30;

                float progress = Timer / Attack1Time;
                if (Timer == 1 && DLCUtils.HostCheck)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostHeldWeapon>(), 0, 0, Main.myPlayer, ai0: 5);
                }
                if (Timer % 15 == 0 && Timer <= Attack1Time)
                {
                    SoundEngine.PlaySound(SoundID.Item5, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        float offset = ((1 - progress) * MathHelper.Pi / 3.5f) + (MathHelper.Pi / 8);
                        for (int i = -1; i < 2; i += 2)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + toTarget * 50, toTarget.RotatedBy((offset * i) + Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(13, 20), ModContent.ProjectileType<IceArrow>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer);
                    }
                }
                if (Timer % 5 == 0 && Timer >= Attack1Time + DelayTime)
                {
                    SoundEngine.PlaySound(SoundID.Item5, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        float offset = 0;
                        const int projs = 1;
                        for (int i = -0; i < projs; i ++)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + toTarget * 50, toTarget.RotatedBy((offset * i) + Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(18, 22), ModContent.ProjectileType<IceArrow>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer);
                    }
                }
                if (Timer >= AttackTime + Endlag)
                {
                    Reset();
                }
            }
            void FrostFlares()
            {
                const int AttackTime = 60;
                Vector2 desiredPos = target.Center - (toTarget * 400);
                Movement(desiredPos, maxSpeed: 50);

                Timer++;
                if (Timer % 20 == 0 && Timer <= AttackTime)
                {
                    SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (toTarget * Main.rand.NextFloat(15, 25)).RotatedByRandom(MathHelper.PiOver4 * (Timer / AttackTime)), ModContent.ProjectileType<FrostFlare>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0);
                }
                if (Timer >= AttackTime + 30)
                {
                    Reset();
                }
            }
            void IceStar()
            {
                Timer++;
                Movement(target.Center, lowspeed: 0, slowdown: 300);
                if (Timer > 0 && Timer % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                    if (DLCUtils.HostCheck) Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, toTarget.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)) * 10, ModContent.ProjectileType<IceStar>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer, ai0: NPC.whoAmI);
                }
                if (Timer >= 30)
                {
                    Timer = -30;
                    Data++;
                }
                if (Data >= 1 && Timer >= 0)
                {
                    Reset();
                }
            }
            #endregion
        }
        
    }
}
