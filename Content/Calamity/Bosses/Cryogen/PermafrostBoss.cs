
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
using CalamityMod.CalPlayer;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod;
using CalamityMod;
using CalamityMod.NPCs.TownNPCs;
using Terraria.DataStructures;
using CalamityMod.World;
using CalamityMod.Particles;
using FargowiltasSouls.Core.Systems;
using System.IO;
using FargowiltasCrossmod.Core.Common;
using FargowiltasCrossmod.Core.Calamity.Globals;
using CalamityMod.Events;
using FargowiltasSouls.Content.Buffs.Masomode;

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

            Main.npcFrameCount[Type] = Main.npcFrameCount[ModContent.NPCType<DILF>()];

        }
        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.lifeMax = 32500;
            NPC.knockBackResist = 0;
            NPC.HitSound = new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenHit", 3);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.damage = 70;
            Music = MusicLoader.GetMusicSlot("FargowiltasCrossmod/Assets/Music/Niflheimr");

            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 4000000;
            }
                

            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.coldDamage = true;

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
            if (Phase == 0 || Despawning)
            {
                Color encasementColor = drawColor * 0.7f;
                Vector2 offset = new Vector2(Main.rand.NextFloat(-Timer, Timer), Main.rand.NextFloat(-Timer, Timer));
                if (Despawning)
                {
                    float progress = 1f - NPC.Opacity;
                    encasementColor = drawColor * progress;
                    offset = new Vector2(Main.rand.NextFloat(-NPC.Opacity * 10, NPC.Opacity * 10), Main.rand.NextFloat(-NPC.Opacity * 10, NPC.Opacity * 10));
                }
                spriteBatch.Draw(encasement.Value, NPC.Center - screenPos + offset, null, encasementColor, NPC.rotation, encasement.Size()/2, NPC.scale, SpriteEffects.None, 0);
            }
            for (int i = 0; i < 15; i++)
            {
                float rotation = MathHelper.ToRadians(360f / 15 * i) + shieldRot;
                if (Phase == 0.5f || Despawning)
                {
                    float x = (Timer / 120);
                    if (Despawning)
                        x = NPC.Opacity;
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

        public const float PhaseTwoPercent = 0.7f;
        public const float PhaseThreePercent = PhaseTwoPercent / 2f;
        public bool PhaseTwoHealth => NPC.GetLifePercent() <= PhaseTwoPercent || WorldSavingSystem.MasochistModeReal;
        public bool PhaseThreeHealth => NPC.GetLifePercent() <= (WorldSavingSystem.MasochistModeReal ? PhaseTwoPercent : PhaseThreePercent);

        public bool Despawning = false;

        public bool DidPaw = false;

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
        public static List<Attacks> SetupAttacks =
        [
            Attacks.PredictiveToss,
            Attacks.FrostFlares,
            Attacks.IceStar
        ];
        public static List<Attacks> FollowupAttacks =
        [
            Attacks.PawCharge,
            Attacks.IceShotgun,
            Attacks.IceArrows,
            Attacks.Blizzard,
            Attacks.TridentToss
        ];
        public static Queue<Attacks> SetupAttackQueue = new Queue<Attacks>();
        public static Queue<Attacks> FollowupAttackQueue = new Queue<Attacks>();

        public Luminance.Core.Graphics.Particle TelegraphParticle;

        private void SpawnTownNPC(bool defeat = false)
        {
            if (!DLCUtils.HostCheck)
                return;
            int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DILF>());
            if (n != Main.maxNPCs)
            {
                Main.npc[n].homeless = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);

                int line = DownedBossSystem.downedCryogen ? 2 : 1;
                if (defeat)
                    Main.npc[n].GetGlobalNPC<CalDLCNPCChanges>().PermafrostDefeatLine = line;
            }
            
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var cryo = new CalamityMod.NPCs.Cryogen.Cryogen();
            cryo.ModifyNPCLoot(npcLoot);
        }
        public override void OnSpawn(IEntitySource source)
        {
            int n = NPC.FindFirstNPC(ModContent.NPCType<DILF>());
            if (n != -1 && n != Main.maxNPCs)
            {
                //NPC.Bottom = Main.npc[n].Bottom;

                Main.npc[n].life = 0;
                Main.npc[n].active = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
            }
        }
        public override void OnKill()
        {
            SpawnTownNPC(true);
            var cryo = new CalamityMod.NPCs.Cryogen.Cryogen();
            cryo.OnKill();
        }
        public override void AI()
        {
            Main.LocalPlayer.ZoneSnow = true;
            if (Main.IsItRaining && DLCUtils.HostCheck)
                Main.StopRain();
            Main.LocalPlayer.buffImmune[ModContent.BuffType<HypothermiaBuff>()] = true;

            int n = NPC.FindFirstNPC(ModContent.NPCType<DILF>());
            if (n != -1 && n != Main.maxNPCs)
            {
                Main.npc[n].life = 0;
                Main.npc[n].active = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
            }

            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
                NetSync(NPC);
            }
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                Despawning = true;
            }
            if (Despawning)
            {
                if (NPC.Opacity == 0.5f)
                    SoundEngine.PlaySound(CalamityMod.NPCs.Cryogen.Cryogen.ShieldRegenSound, NPC.Center);

                NPC.Opacity -= 1f / 120;
                if (NPC.Opacity < 0.2f)
                    NPC.velocity.Y -= 2f;
                else
                    NPC.velocity *= 0.96f;
                if (NPC.Opacity <= 0)
                    NPC.active = false;
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
                    if (!Main.dedServ && Main.netMode != NetmodeID.Server)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Gore.NewGoreDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -4).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)), ModCompatibility.Calamity.Mod.Find<ModGore>("CryoDeathGore" + Main.rand.Next(2, 4)).Type);
                            Gore.NewGoreDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModCompatibility.Calamity.Mod.Find<ModGore>("CryoShieldGore" + Main.rand.Next(1, 5)).Type);
                            Gore.NewGoreDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModCompatibility.Calamity.Mod.Find<ModGore>("CryoShieldGore" + Main.rand.Next(1, 5)).Type);
                        }
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
                accel *= 16;
                decel *= 16;

                float resistance = NPC.velocity.Length() * accel / (maxSpeed);
                NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, pos, NPC.velocity, accel - resistance, decel + resistance);
                /*
                decel *= 2;
                if (NPC.Distance(pos) > slowdown)
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * maxSpeed, accel);
                }
                else
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * lowspeed, decel);
                }
                */
            }
            void Reset()
            {
                if (PhaseTwoHealth && Phase < 2)
                {
                    Attack = (float)Attacks.PhaseTransition;
                    Phase = 2;
                    FollowupAttackQueue.Clear();
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
                        if (Phase < 2)
                            FollowupAttackQueue.RandomFromListExcept(FollowupAttacks, (Attacks)Attack);
                        else
                            FollowupAttackQueue.RandomFromListExcept(FollowupAttacks, (Attacks)Attack, Attacks.PawCharge);
                    }
                    if (Phase >= 2 && Attack == (int)Attacks.FrostFlares) // frost flare -> paw followup, every other frost flare
                    {
                        if (!DidPaw)
                            Attack = (float)Attacks.PawCharge;
                        else
                            Attack = (float)FollowupAttackQueue.Dequeue();
                        DidPaw = !DidPaw;
                    }
                    else
                        Attack = (float)FollowupAttackQueue.Dequeue();
                }
                Timer = 0;
                Data = 0;
                NPC.netUpdate = true;
                NetSync(NPC);
            }
            void SpawnWeapon(float weapon, float ai1 = 0, int dmg = 0, int kb = 0)
            {
                int type = ModContent.ProjectileType<PermafrostHeldWeapon>();
                float ai2 = NPC.whoAmI;
                foreach (Projectile oldWep in Main.projectile.Where(p => p.active && p.type == type && p.ai[2] == ai2))
                    oldWep.Kill();

                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostHeldWeapon>(), dmg, kb, Main.myPlayer, weapon, ai1, ai2);
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
                    return;
                }
            }
            void PhaseTransition()
            {
                Vector2 desiredPos = target.Center - (toTarget * 400);
                Movement(desiredPos, maxSpeed: 50);

                //if (Timer == 10)
                    //RitualProj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostRitual>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, Main.myPlayer, 0f, NPC.whoAmI);
                if (Timer == 1)
                    PhaseTransitionSound();
                if (Timer > 60)
                {
                    Phase = 2;
                    Reset();
                    return;
                }
                Timer++;
            }
            void TridentToss()
            {
                Timer++;
                if (Timer < 30)
                {
                    //Vector2 targetpos = target.Center + new Vector2(NPC.Center.X > target.Center.X ? 400 : -400, -300);

                    Vector2 targetpos = target.Center - toTarget * 400;
                    Movement(targetpos, maxSpeed: 50, slowdown: 100, decel: 0.05f);

                    if (NPC.Distance(targetpos) > 400 && Timer > 25)
                        Timer--;
                }
                else
                {
                    NPC.velocity /= 1.05f;
                    if (Timer == 30 && DLCUtils.HostCheck)
                    {
                        SpawnWeapon(0, toTarget.ToRotation());
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
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + toTarget * 50, vel * i * 15, ModContent.ProjectileType<IceTrident>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: i * 5, ai2: toTarget.ToRotation());
                            }
                        }
                    }
                    if (Timer > 60)
                    {
                        Vector2 targetpos = target.Center - toTarget * 400;
                        Movement(targetpos, slowdown: 100, decel: 0.05f);
                    }
                    if (Timer >= 100)
                    {
                        Reset();
                        return;
                    }
                }
            }
            void PawCharge()
            {
                int WindupTime = 50;
                if (WorldSavingSystem.MasochistModeReal)
                    WindupTime = 40;
                const int DashTime = 30;
                int TotalTime = WindupTime + DashTime;

                int Dashes = Phase >= 2 ? 4 : 3;
                int partialTimer = (int)Timer % TotalTime;

                if (Timer >= TotalTime * Dashes + 30)
                {
                    Reset();
                    return;
                }

                if (partialTimer == 1 && Timer < TotalTime * Dashes)
                {
                    if (DLCUtils.HostCheck)
                        SpawnWeapon(2, dmg: FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage));

                    float dif = FargoSoulsUtil.RotationDifference(toTarget, target.velocity);
                    dif = MathHelper.Clamp(dif, -MathHelper.PiOver2, MathHelper.PiOver2);
                    if (Timer < TotalTime) //first dash
                        dif = MathHelper.Clamp(dif, -MathHelper.Pi * 0.2f, MathHelper.Pi * 0.2f);
                    Data = toTarget.ToRotation() + dif;
                    Data += Main.rand.NextFloat(-MathHelper.PiOver2 / 10f, MathHelper.PiOver2 / 10f); //some randomness to make it less static
                    WindupSound();
                    /*
                    TelegraphParticle = null;
                    TelegraphParticle = new ExpandingBloomParticle(NPC.Center, Vector2.Zero, Color.DarkCyan, Vector2.One * 20, Vector2.One, WindupTime, true, Color.LightBlue);
                    TelegraphParticle.Spawn();
                    */

                    NPC.netUpdate = true;
                    NetSync(NPC);
                }
                /*
                if (TelegraphParticle != null)
                {
                    TelegraphParticle.Position = NPC.Center;
                }
                */
                if (Timer > TotalTime * Dashes)
                {
                    Vector2 desiredPos = target.Center - toTarget * 400;
                    Movement(desiredPos, 0.1f, 40, 10, 0.1f, 50f);
                }
                else if (Timer > 0)
                {
                    if (partialTimer < WindupTime)
                    {
                        Vector2 desiredPos = target.Center - (Data.ToRotationVector2() * 480);

                        Movement(desiredPos, 0.15f, 40, 10, 0.15f, 50f);
                    }
                    else if (partialTimer == WindupTime)
                    {
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/SCalSounds/SCalDash"), NPC.Center);
                        AttackSound();

                        Data = toTarget.ToRotation();
                    }
                    else
                    {
                        Vector2 desiredPos = NPC.Center + Data.ToRotationVector2() * 400;
                        Movement(desiredPos, 0.2f, 40, 10, 0.1f, 50);

                        if (Timer % 7 == 0 && DLCUtils.HostCheck)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, Main.rand.Next(2, 5)).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<ArcticPaw>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    }
                }
                Timer++;
            }
            void Blizzard()
            {
                Movement(target.Center - toTarget * 400f);

                if (Timer == 0)
                {
                    Data = Main.rand.NextBool() ? 1 : -1;
                    SpawnWeapon(3);

                    NPC.netUpdate = true;
                    NetSync(NPC);
                }
                const int up = 2100; //1600
                Vector2 pos = new Vector2(0, -up).RotatedBy(MathHelper.Lerp(-0.9f * Data, 0.9f * Data, Timer / 180f));
                pos.Y /= 1.78f;
                if (DLCUtils.HostCheck)
                {
                    Vector2 position = target.Center + pos + pos.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * Main.rand.Next(-500, 500);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), position, -pos.SafeNormalize(Vector2.Zero) * 30, ModContent.ProjectileType<Blizzard>(), 0, 0, ai0: Main.rand.Next(0, 4));

                    
                    const int Snows = 5;
                    for (int i = 0; i < Snows; i++)
                    {
                        CalamityMod.Particles.Particle snowflake = new SnowflakeSparkle(position + Vector2.UnitY * (up - 1600), -pos.SafeNormalize(Vector2.Zero) * 15, Color.White, new Color(75, 177, 250), Main.rand.NextFloat(0.3f, 1.5f), 40, 0.5f);
                        GeneralParticleHandler.SpawnParticle(snowflake);
                    }
                    
                    if (Timer % 5 == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), position, -pos.SafeNormalize(Vector2.Zero) * 12, ModContent.ProjectileType<FrostShard>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
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
                    return;
                }
            }
            void IceShotgun()
            {
                ref float Corner = ref Data;

                const int AttackTime = 64; //keep divisible by 2
                int Attacks = Phase >= 2 ? 3 : 2;

                
                if (Timer == 0)
                {
                    

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

                    if (Timer > 5 && Timer < AttackTime && NPC.Distance(target.Center + desiredDir) > 400)
                        Timer--;
                }
                /*
                if (TelegraphParticle != null)
                    TelegraphParticle.Position = NPC.Center;
                */
                if (Timer % AttackTime == 10 && Timer < AttackTime * Attacks)
                    if (DLCUtils.HostCheck)
                        SpawnWeapon(1);

                if (Timer % AttackTime == AttackTime - 1)
                {
                    SoundEngine.PlaySound(SoundID.Item36, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, toTarget.RotatedByRandom(MathHelper.Pi / 8.5f) * Main.rand.NextFloat(15, 20), ModContent.ProjectileType<FrostShard>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        }
                        if (WorldSavingSystem.MasochistModeReal)
                        {
                            for (int dir = -1; dir < 2; dir += 2)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, toTarget.RotatedBy(MathHelper.Pi * 0.25f * dir).RotatedByRandom(MathHelper.Pi / 16f) * Main.rand.NextFloat(15, 20), ModContent.ProjectileType<FrostShard>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                                }
                            }
                        }
                    }
                    Corner += Main.rand.NextBool() ? 1 : -1;
                    Corner %= 4;
                    if (Corner < 0)
                        Corner = 3;

                    NPC.netUpdate = true;
                    NetSync(NPC);
                }
                if (Timer == AttackTime * Attacks + (AttackTime / 2))
                {
                    Reset();
                    return;
                }
                Timer++;
            }
            void PredictiveToss()
            {
                if (Data != 1)
                    Movement(target.Center + new Vector2(NPC.Center.X > target.Center.X ? 300 : -300, 0), maxSpeed: 35, slowdown: 100);
                else
                    Movement(target.Center + new Vector2(0, NPC.Center.Y > target.Center.Y ? 300 : -300), maxSpeed: 35, slowdown: 100);

                Timer++;
                if (Timer == 1 && DLCUtils.HostCheck)
                {
                    SpawnWeapon(4);
                    for (int i = -1; i < 1; i++)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DarkIceCrystal>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage, 1.3f), 0, Main.myPlayer, ai0: NPC.whoAmI, ai1: target.whoAmI, ai2: i);
                    TelegraphParticle = null;
                    TelegraphParticle = new ExpandingBloomParticle(NPC.Center, Vector2.Zero, Color.LightBlue, Vector2.One * 20, Vector2.One, 100, true, Color.White);
                    TelegraphParticle.Spawn();
                }
                if (TelegraphParticle != null)
                {
                    TelegraphParticle.Position = NPC.Center;
                }
                if (Timer == 90)
                {
                    SoundEngine.PlaySound(SoundID.Item109 with { Pitch = 0.5f }, NPC.Center);
                    AttackSound();
                }
                if (Timer == 100)
                {
                    NPC.velocity = toTarget * 30;
                    Main.LocalPlayer.Calamity().GeneralScreenShakePower = 10f;

                    if (PhaseThreeHealth && Data != 1) //do another swing, from vertical, on masomode
                    {
                        Data = 1;
                        Timer = 0;
                    }
                }
                if (Timer >= 120)
                {
                    Reset();
                    return;
                }
            }
            void IceArrows()
            {
                
                Timer++;

                const int Attack1Time = 90;
                const int DelayTime = 30;
                const int Attack2Time = 20;
                const int AttackTime = Attack1Time + DelayTime + Attack2Time;
                const int Endlag = 30;

                float progress = Timer / Attack1Time;

                bool part1 = Timer <= Attack1Time;
                float distance = part1 ? 400f : 200f;
                float accel = part1 ? 0.1f : 0.025f;
                float decel = part1 ? 0.1f : 0.025f;
                Vector2 desiredPos = target.Center - (toTarget * distance);
                Movement(desiredPos, accel: accel, lowspeed: 5, decel: decel, slowdown: 300);

                if (Timer == 1 && DLCUtils.HostCheck)
                {
                    SpawnWeapon(5);
                }
                if (Timer % 15 == 0 && Timer <= Attack1Time)
                {
                    SoundEngine.PlaySound(SoundID.Item5, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        float offset = ((1 - progress) * MathHelper.Pi / 3.5f) + (MathHelper.Pi / 8);
                        for (int i = -1; i < 2; i += 2)
                        {
                            float speed = Main.rand.NextFloat(15, 20) + (Timer / Attack1Time) * 4;
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + toTarget * 50, toTarget.RotatedBy((offset * i) + Main.rand.NextFloat(-0.1f, 0.1f)) * speed, ModContent.ProjectileType<IceArrow>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
                        }
                        //if (WorldSavingSystem.MasochistModeReal)
                            //Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + toTarget * 50, toTarget * Main.rand.NextFloat(18, 22), ModContent.ProjectileType<IceArrow>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
                    }
                }
                if (Timer >= Attack1Time && Timer <= Attack1Time + DelayTime)
                {
                    Vector2 pos = NPC.Center + toTarget * 50;
                    Vector2 vel = toTarget.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(18, 22);
                    Dust.NewDustDirect(pos, 0, 0, DustID.SnowflakeIce, vel.X, vel.Y).noGravity = true;
                }
                if (Timer % 5 == 0 && Timer >= Attack1Time + DelayTime)
                {
                    SoundEngine.PlaySound(SoundID.Item5, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        float offset = 0;
                        const int projs = 1;
                        for (int i = -0; i < projs; i ++)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + toTarget * 50, toTarget.RotatedBy((offset * i) + Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(18, 22), ModContent.ProjectileType<IceArrow>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
                    }
                }
                if (Timer >= AttackTime + Endlag)
                {
                    Reset();
                    return;
                }
            }
            void FrostFlares()
            {
                const int StartTime = 30;
                const int AttackTime = 20;
                int side = Math.Sign(toTarget.Y);
                Vector2 desiredPos = target.Center - (side * Vector2.UnitY * 400);
                desiredPos.X += target.velocity.X * 25;
                if (Math.Abs(desiredPos.X - NPC.Center.X) > 100 && Timer < 5)
                    Timer--;
                Timer++;
                if (Timer < StartTime)
                    Movement(desiredPos, accel: 0.1f, decel: 0.1f, maxSpeed: 50);
                else
                    NPC.velocity *= 0.92f;
                if (Timer % 10 == 0 && Timer <= StartTime + AttackTime && Timer > StartTime)
                {
                    SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
                    int dir = Timer % 20 == 0 ? 1 : -1;
                    if (DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX * (24 * dir), ModContent.ProjectileType<FrostFlare>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        if (PhaseThreeHealth)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY * (28 * dir), ModContent.ProjectileType<FrostFlare>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    }
                }
                if (Timer >= StartTime + AttackTime + 30)
                {
                    Reset();
                    return;
                }
            }
            void IceStar()
            {
                Timer++;
                Vector2 desiredPos = target.Center - (toTarget * 300);
                Movement(desiredPos, lowspeed: 5, decel: 0.1f, slowdown: 300);
                if (Timer > 30 && Timer % 10 == 0 && Timer <= 60)
                {
                    if (Math.Sign(Data) == 0)
                        Data = Main.rand.NextBool() ? 1 : -1;

                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, toTarget.RotatedBy(Math.Sign(Data) * MathHelper.PiOver2 * 0.8f + Main.rand.NextFloat(-0.15f, 0.15f)) * 10, ModContent.ProjectileType<IceStar>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer, ai0: NPC.whoAmI);
                        if (PhaseThreeHealth)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, toTarget.RotatedBy(-Math.Sign(Data) * MathHelper.PiOver2 * 0.8f + Main.rand.NextFloat(-0.15f, 0.15f)) * 10, ModContent.ProjectileType<IceStar>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer, ai0: NPC.whoAmI);
                    }

                    NPC.netUpdate = true;
                    NetSync(NPC);
                }
                if (Timer >= 90)
                {
                    Reset();
                    return;
                }
            }
            #endregion
        }

        public void WindupSound()
        {
            SoundEngine.PlaySound(new SoundStyle($"FargowiltasCrossmod/Assets/Sounds/PermafrostWindup{Main.rand.Next(1, 3)}") { Volume = 0.2f, PitchVariance = 0.2f }, NPC.Center);
        }
        public void AttackSound()
        {
            SoundEngine.PlaySound(new SoundStyle($"FargowiltasCrossmod/Assets/Sounds/PermafrostAttack{Main.rand.Next(1, 3)}") { Volume = 0.25f, PitchVariance = 0.2f }, NPC.Center);
        }
        public void PhaseTransitionSound()
        {
            SoundEngine.PlaySound(new SoundStyle($"FargowiltasCrossmod/Assets/Sounds/PermafrostPhaseTransition") { Volume = 0.3f }, NPC.Center);
        }

    }
}
