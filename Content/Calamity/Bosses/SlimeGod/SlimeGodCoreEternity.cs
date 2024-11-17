using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.NPCs.SlimeGod;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SlimeGodCoreEternity : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<SlimeGodCore>();


        #region Variables

        public static readonly SoundStyle PossessionSound = new SoundStyle("CalamityMod/Sounds/Custom/SlimeGodPossession", (SoundType)0);

        public static readonly SoundStyle ExitSound = new SoundStyle("CalamityMod/Sounds/Custom/SlimeGodExit", (SoundType)0);

        public static readonly SoundStyle ShotSound = new SoundStyle("CalamityMod/Sounds/Custom/SlimeGodShot", 2, (SoundType)0);

        public static readonly SoundStyle BigShotSound = new SoundStyle("CalamityMod/Sounds/Custom/SlimeGodBigShot", 2, (SoundType)0);

        public static int CrimsonGodType => ModContent.NPCType<CrimulanPaladin>();
        public static int CorruptionGodType => ModContent.NPCType<EbonianPaladin>();

        public static List<int> SlimesToKill =
        [
            CrimsonGodType,
            CorruptionGodType,
            ModContent.NPCType<CorruptSlimeSpawn>(),
            ModContent.NPCType<CorruptSlimeSpawn2>(),
            ModContent.NPCType<CrimsonSlimeSpawn>(),
            ModContent.NPCType<CorruptSlimeSpawn2>(),
            ModContent.NPCType<SplitCrimulanPaladin>(),
            ModContent.NPCType<SplitEbonianPaladin>(),
        ];


        public bool AttachToCrimson = WorldGen.crimson; //start off with the current world evil
        public int AttachedSlime;
        public bool ContactDamage = false;
        public int ReattachTimer = 0;
        public Vector2 LockVector = Vector2.Zero;
        public int LatestAttack = 0;
        public enum Phases
        {
            ShouldAttach,
            TryAttach,
            Attached,
            Attacking,
            Final
        }
        public enum Attacks
        {
            SlimeBreakout = -1,
            Drift,
            MettatonHeart,
            SpinDash,
        }
        public List<int> AttackCycle =
        [
            (int)Attacks.Drift,
            (int)Attacks.MettatonHeart,
            (int)Attacks.SpinDash
        ];
        #endregion
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(AttachedSlime);
            binaryWriter.Write(AttachToCrimson);
            binaryWriter.Write(ContactDamage);
            binaryWriter.Write7BitEncodedInt(ReattachTimer);
            binaryWriter.WriteVector2(LockVector);
            binaryWriter.Write7BitEncodedInt(LatestAttack);

        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            AttachedSlime = binaryReader.Read7BitEncodedInt();
            AttachToCrimson = binaryReader.ReadBoolean();
            ContactDamage = binaryReader.ReadBoolean();
            ReattachTimer = binaryReader.Read7BitEncodedInt();
            LockVector = binaryReader.ReadVector2();
            LatestAttack = binaryReader.Read7BitEncodedInt();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => ContactDamage;


        public override void SetDefaults()
        {
            NPC.lifeMax = 10000;
        }
        public override bool PreAI()
        {
            if (!WorldSavingSystem.EternityMode) return true;
            #region Passive
            //FargoSoulsUtil.PrintAI(NPC);
            CalamityGlobalNPC.slimeGod = NPC.whoAmI;
            ref float timer = ref NPC.ai[0];
            ref float attack = ref NPC.ai[2];
            ref float phase = ref NPC.ai[3];
            if (NPC.life < NPC.lifeMax * 0.15f && phase != (int)Phases.Final)
            {
                FullReset();
                phase = (int)Phases.Final;
            }

            NPC.dontTakeDamage = !(phase == (int)Phases.Attacking);
            ContactDamage = phase == (int)Phases.Attacking;

            if (phase != (int)Phases.Attached)
            {
                NPC.rotation += NPC.velocity.Length() * (MathHelper.Pi / 300f);
            }
            else //attached
            {
                NPC.Opacity = 0.3f;
            }

            if (!Targeting())
                return false;

            switch (phase)
            {
                case (int)Phases.ShouldAttach:
                    {
                        SummonSlimes();
                    }
                    break;
                case (int)Phases.TryAttach:
                    {
                        TryAttaching();
                    }
                    break;
                case (int)Phases.Attached:
                    {
                        Attached();
                    }
                    break;
                case (int)Phases.Attacking:
                    {
                        switch (attack)
                        {
                            case (int)Attacks.SlimeBreakout:
                                {
                                    SlimeBreakout();
                                }
                                break;
                            case (int)Attacks.MettatonHeart:
                                {
                                    MettatonHeart();
                                }
                                break;
                            case (int)Attacks.Drift:
                                {
                                    Drift();
                                }
                                break;
                            case (int)Attacks.SpinDash:
                                {
                                    SpinDash();
                                }
                                break;
                        }
                        if (++ReattachTimer > 60 * 22)
                        {
                            phase = (int)Phases.ShouldAttach;
                            FullReset();
                            NetSync(NPC);
                            NPC.netUpdate = true;
                        }
                    }
                    break;
                case (int)Phases.Final:
                    {
                        FinalPhase();
                    }
                    break;
            }
            timer++;
            return false;
            #endregion
            #region Common Methods
            bool Targeting()
            {
                const float despawnRange = 5000f;
                Player p = Main.player[NPC.target];
                if (!p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > despawnRange)
                {
                    NPC.TargetClosest();
                    p = Main.player[NPC.target];
                    if (!p.active || p.dead || Vector2.Distance(NPC.Center, p.Center) > despawnRange)
                    {
                        NPC.noTileCollide = true;
                        if (NPC.timeLeft > 30)
                            NPC.timeLeft = 30;
                        NPC.velocity.Y -= 1f;
                        if (NPC.timeLeft == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                            }
                        }
                        return false;
                    }
                }
                return true;
            }
            void FullReset()
            {
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
                ReattachTimer = 0;
                NetSync(NPC);
                NPC.netUpdate = true;
            }
            void AttackReset()
            {
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
                LockVector = Vector2.Zero;
                NetSync(NPC);
                NPC.netUpdate = true;
            }
            void ResetToDrift()
            {
                ref float attack = ref NPC.ai[2];
                LatestAttack = (int)attack;
                attack = (int)Attacks.Drift;
                AttackReset();
            }
            #endregion
            #region Special Actions
            void SummonSlimes()
            {
                ref float timer = ref NPC.ai[0];
                ref float phase = ref NPC.ai[3];
                if (timer < 120)
                {
                    Vector2 desiredPos = Main.player[NPC.target].Center - Vector2.UnitY * 300;
                    NPC.velocity = (desiredPos - NPC.Center) * 0.05f;
                }
                if (timer >= 120)
                {
                    if (DLCUtils.HostCheck)
                    {
                        const int spawnTossSpeed = 30;
                        if (!NPC.AnyNPCs(CrimsonGodType))
                        {
                            int crim = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, CrimsonGodType, Target: NPC.target);
                            if (crim != Main.maxNPCs)
                            {
                                Main.npc[crim].velocity = Vector2.UnitX * -spawnTossSpeed;
                                if (AttachToCrimson)
                                {
                                    AttachedSlime = crim;
                                    phase = (int)Phases.TryAttach;
                                    Main.npc[crim].ai[0] = 2;
                                    Main.npc[crim].GetGlobalNPC<SlimeGodsEternity>().Empowered = true;
                                }
                            }
                        }
                        if (!NPC.AnyNPCs(CorruptionGodType))
                        {
                            int corr = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, CorruptionGodType, Target: NPC.target);
                            if (corr != Main.maxNPCs)
                            {
                                Main.npc[corr].velocity = Vector2.UnitX * spawnTossSpeed;
                                if (!AttachToCrimson)
                                {
                                    AttachedSlime = corr;
                                    phase = (int)Phases.TryAttach;
                                    Main.npc[corr].ai[0] = 2;
                                    Main.npc[corr].GetGlobalNPC<SlimeGodsEternity>().Empowered = true;
                                }
                            }
                        }
                        AttachToCrimson = !AttachToCrimson;
                        FullReset();
                    }
                    SoundEngine.PlaySound(BigShotSound, NPC.Center);
                }
            }
            void TryAttaching()
            {
                ref float timer = ref NPC.ai[0];
                ref float phase = ref NPC.ai[3];
                ref float attack = ref NPC.ai[2];

                NPC slime = Main.npc[AttachedSlime];
                ContactDamage = false;
                if (slime != null && slime.active)
                {
                    float modifier = timer / 60f;
                    NPC.velocity = NPC.DirectionTo(slime.Center) * 20f * modifier;
                    if (NPC.Distance(slime.Center) <= NPC.velocity.Length() * 2f)
                    {
                        phase = (int)Phases.Attached;
                        SoundEngine.PlaySound(PossessionSound, NPC.Center);
                        FullReset();
                    }
                }
                else
                {
                    FullReset();
                    phase = (int)Phases.Attacking;
                    attack = (int)Attacks.SlimeBreakout;
                }
            }
            void Attached()
            {
                ref float timer = ref NPC.ai[0];
                ref float phase = ref NPC.ai[3];
                ref float attack = ref NPC.ai[2];
                NPC slime = Main.npc[AttachedSlime];
                if (slime != null && slime.active && (slime.type == CrimsonGodType || slime.type == CorruptionGodType))
                {
                    NPC.velocity = (slime.Center - NPC.Center) + slime.velocity;
                }
                else
                {
                    attack = (int)Attacks.SlimeBreakout;
                    phase = (int)Phases.Attacking;
                    FullReset();
                }
            }
            #endregion
            #region Attacks
            void SlimeBreakout()
            {
                ref float timer = ref NPC.ai[0];
                ref float attack = ref NPC.ai[2];
                if (timer < 22)
                {
                    foreach (NPC minion in Main.npc.Where(n => n != null && n.active && SlimesToKill.Contains(n.type)))
                    {
                        if (minion.TryGetGlobalNPC(out SlimeGodMinionEternity lobotomizer))
                        {
                            lobotomizer.LobotomizeAndSuck = true;
                            lobotomizer.CoreNPCID = NPC.whoAmI;
                        }
                        else
                        {
                            minion.active = false;
                        }
                    }
                    timer = 22;
                }
                if (NPC.Opacity < 1)
                {
                    NPC.Opacity += 0.7f * (1 / 180f); //takes 3 seconds
                    NPC.velocity *= 0.95f;
                }
                else
                {

                    NPC.velocity = Vector2.Zero;
                    NPC.Opacity = 1;
                    const int ShotCount = 16;
                    SoundEngine.PlaySound(ExitSound, NPC.Center);
                    NPC.SimpleStrikeNPC((int)Math.Round(NPC.lifeMax * 0.125f), 1);
                    //screenshake
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < ShotCount; i++)
                        {
                            int type = i % 2 == 0 ? ModContent.ProjectileType<AcceleratingCrimulanGlob>() : ModContent.ProjectileType<AcceleratingEbonianGlob>();
                            float speed = Main.rand.NextFloat(2f, 2.6f);
                            Vector2 dir = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / ShotCount).RotatedByRandom(MathHelper.TwoPi / (ShotCount * 4));
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + dir * NPC.width / 3, dir * speed, type, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer);
                        }
                    }
                    AttackReset();
                    attack = Main.rand.NextFromCollection(AttackCycle);
                    NPC.netUpdate = true;
                    NetSync(NPC);
                }
            }
            void MettatonHeart()
            {
                ref float timer = ref NPC.ai[0];
                ref float partialTimer = ref NPC.ai[1];
                ref float attack = ref NPC.ai[2];
                Player player = Main.player[NPC.target];

                const int partialAttackTime = 100;
                const int partialAttacks = 3;
                if (partialTimer <= 0)
                {
                    const int minDistance = 230;
                    float dir = Main.rand.NextBool() ? 1 : -1;
                    LockVector = (player.DirectionTo(NPC.Center) * minDistance).RotatedBy(dir * MathHelper.Pi / 3f);
                    NetSync(NPC);
                    NPC.netUpdate = true;
                    partialTimer++;
                }
                else
                {
                    if (partialTimer < partialAttackTime / 4) //first quarter of attack
                    {
                        NPC.velocity = (partialTimer / partialAttackTime) * (player.Center + LockVector - NPC.Center) * 0.65f; //go to pos, relative player
                    }
                    else //latter three quarters
                    {
                        NPC.velocity *= 0.92f; //decel
                        if (partialTimer % (partialAttackTime / 4) == 1) //first frame of cycle
                        {
                            float rot = NPC.DirectionTo(player.Center).ToRotation();
                            if (partialTimer >= partialAttackTime / 2)
                            {
                                int dir = partialTimer >= partialAttackTime * 0.7f ? 1 : -1;
                                rot = rot + dir * MathHelper.Pi / 7.65f; //funny weird number

                            }
                            NPC.rotation = rot;
                        }
                        if (partialTimer % (partialAttackTime / 10) == 5 && partialTimer > (partialAttackTime / 4) + (partialAttackTime / 20)) //shoot
                        {
                            SoundEngine.PlaySound(ShotSound, NPC.Center);
                            if (DLCUtils.HostCheck)
                            {
                                Vector2 dir = NPC.rotation.ToRotationVector2();
                                float speed = 5f;
                                bool crim = partialTimer >= partialAttackTime / 2;
                                if (!AttachToCrimson)
                                {
                                    crim = !crim;
                                }
                                int type = crim ? ModContent.ProjectileType<AcceleratingCrimulanGlob>() : ModContent.ProjectileType<AcceleratingEbonianGlob>();
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + dir * NPC.width / 3, dir * speed, type, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer);
                            }
                        }
                    }
                    if (++partialTimer > partialAttackTime)
                    {
                        partialTimer = 0;
                    }
                }
                if (timer > partialAttackTime * partialAttacks)
                {
                    ResetToDrift();
                }
            }
            void SpinDash()
            {
                ref float timer = ref NPC.ai[0];
                ref float partialTimer = ref NPC.ai[1];
                ref float attack = ref NPC.ai[2];
                Player player = Main.player[NPC.target];
                const int DashTime = 108; //divisible by 4 pls
                const int Dashes = 3;
                float speedmod = 1.2f;
                if (partialTimer < DashTime * 0.7f)
                {
                    NPC.velocity *= 0.95f;
                    speedmod /= 6f;
                    float modifier = 3f;
                    NPC.rotation += modifier * MathHelper.Pi / DashTime;
                    const int shotDelay = 3;
                    if (partialTimer % shotDelay == shotDelay - 1 && partialTimer > DashTime / 4)
                    {
                        SoundEngine.PlaySound(ShotSound, NPC.Center);
                        if (DLCUtils.HostCheck)
                        {
                            Vector2 dir = NPC.rotation.ToRotationVector2();
                            float speed = 3.25f;
                            bool crim = timer > DashTime && timer < DashTime * 2;
                            if (!AttachToCrimson)
                            {
                                crim = !crim;
                            }
                            int type = crim ? ModContent.ProjectileType<AcceleratingCrimulanGlob>() : ModContent.ProjectileType<AcceleratingEbonianGlob>();
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + dir * NPC.width / 3, dir * speed, type, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer);
                        }
                    }
                }
                NPC.velocity += NPC.DirectionTo(player.Center) * speedmod;
                NPC.velocity.ClampMagnitude(0, 35);
                if (++partialTimer > DashTime)
                {
                    partialTimer = 0;
                }
                if (timer > DashTime * Dashes + DashTime / 4)
                {
                    ResetToDrift();
                }
            }
            void Drift()
            {
                ref float timer = ref NPC.ai[0];
                ref float driftDir = ref NPC.ai[1];
                ref float attack = ref NPC.ai[2];
                Player player = Main.player[NPC.target];
                const int DriftDuration = 60;
                const int MaxDistance = 300;
                const int MinDistance = 100;
                if (timer <= 1)
                {
                    LockVector = (NPC.Center - player.Center).ClampMagnitude(MinDistance, MaxDistance);
                    driftDir = Main.rand.NextBool() ? 1 : -1;
                }
                else if (timer < DriftDuration)
                {
                    float modifier = 1.2f; //fraction of half circle to drift
                    LockVector = LockVector.RotatedBy(driftDir * MathHelper.Pi * modifier / DriftDuration);
                    NPC.velocity = (player.Center + LockVector - NPC.Center) * (timer / DriftDuration);

                    const int shotDelay = 9;
                    if (timer % shotDelay == shotDelay - 1)
                    {
                        SoundEngine.PlaySound(ShotSound, NPC.Center);
                        if (DLCUtils.HostCheck)
                        {
                            int shotSide = -Math.Sign(FargoSoulsUtil.RotationDifference(NPC.DirectionTo(player.Center), NPC.velocity));
                            Vector2 dir = Vector2.Normalize(NPC.velocity).RotatedBy(MathHelper.PiOver2 * shotSide);
                            float speed = 3.25f;
                            bool crim = timer % (shotDelay * 2) >= shotDelay;
                            int type = crim ? ModContent.ProjectileType<AcceleratingCrimulanGlob>() : ModContent.ProjectileType<AcceleratingEbonianGlob>();
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + dir * NPC.width / 3, dir * speed, type, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 3f, Main.myPlayer);
                        }
                    }

                    if (NPC.velocity.Length() > 20)
                    {
                        NPC.velocity = Vector2.Normalize(NPC.velocity) * 20;
                    }
                }
                else
                {
                    NPC.velocity *= 0.96f;
                    if (NPC.velocity.Length() < 2)
                    {
                        attack = LatestAttack == (int)Attacks.MettatonHeart ? (int)Attacks.SpinDash : (int)Attacks.MettatonHeart; //defaults to latter for first attack
                        AttackReset();
                    }
                }
            }
            void FinalPhase()
            {
                ref float timer = ref NPC.ai[0];
                ref float phase = ref NPC.ai[3];
                Vector2 desiredPos = Main.player[NPC.target].Center - Vector2.UnitY * 300;
                NPC.velocity = (desiredPos - NPC.Center) * 0.05f;
                if (NPC.life < NPC.lifeMax * 0.15f)
                {
                    NPC.life = (int)(NPC.lifeMax * 0.15f);
                }
                if (timer < 120)
                {
                    if (timer == 10)
                    {
                        SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                    }
                    if (timer % 30 == 10)
                    {
                        Color color = (timer % 60 == 10 ? Color.Crimson : Color.Magenta);
                        Particle p = new ExpandingBloomParticle(NPC.Center, Vector2.Zero, color, Vector2.One, Vector2.One * 60, 30, true, Color.Transparent);
                        p.Spawn();
                    }
                }
                if (timer == 120)
                {
                    if (DLCUtils.HostCheck)
                    {
                        const int spawnTossSpeed = 30;
                        if (!NPC.AnyNPCs(CrimsonGodType))
                        {
                            int crim = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, CrimsonGodType, Target: NPC.target);
                            if (crim != Main.maxNPCs)
                            {
                                Main.npc[crim].velocity = Vector2.UnitX * -spawnTossSpeed;
                                Main.npc[crim].ai[0] = 2;
                                Main.npc[crim].GetGlobalNPC<SlimeGodsEternity>().Empowered = true;
                            }
                        }
                        if (!NPC.AnyNPCs(CorruptionGodType))
                        {
                            int corr = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, CorruptionGodType, Target: NPC.target);
                            if (corr != Main.maxNPCs)
                            {
                                Main.npc[corr].velocity = Vector2.UnitX * spawnTossSpeed;
                                Main.npc[corr].ai[0] = 2;
                                Main.npc[corr].GetGlobalNPC<SlimeGodsEternity>().Empowered = true;
                            }
                        }
                    }
                    NetSync(NPC);
                    NPC.netUpdate = true;
                    SoundEngine.PlaySound(BigShotSound, NPC.Center);
                    timer = 300;
                }
                if (timer > 120 && !NPC.AnyNPCs(CrimsonGodType) && !NPC.AnyNPCs(CorruptionGodType))
                {
                    foreach (NPC minion in Main.npc.Where(n => n != null && n.active && SlimesToKill.Contains(n.type)))
                    {
                        if (minion.TryGetGlobalNPC(out SlimeGodMinionEternity lobotomizer))
                        {
                            lobotomizer.LobotomizeAndSuck = true;
                            lobotomizer.CoreNPCID = NPC.whoAmI;
                        }
                        else
                        {
                            minion.active = false;
                        }
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        Color newColor = (Main.rand.NextBool() ? Color.Lavender : Color.Crimson);
                        newColor.A = 150;
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, newColor);
                    }

                    NPC.velocity *= 0.97f;
                    NPC.rotation += (float)NPC.direction * 0.3f;
                    NPC.Opacity -= 0.005f;
                    if (!(NPC.Opacity <= 0f))
                    {
                        return;
                    }

                    NPC.Opacity = 0f;
                    SoundEngine.PlaySound(in PossessionSound, NPC.Center);
                    NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                    NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                    NPC.width = 40;
                    NPC.height = 40;
                    NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                    NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                    for (int j = 0; j < 40; j++)
                    {
                        Color newColor2 = (Main.rand.NextBool() ? Color.Lavender : Color.Crimson);
                        newColor2.A = 150;
                        int num = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, newColor2, 2f);
                        Main.dust[num].velocity *= 3f;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num].scale = 0.5f;
                            Main.dust[num].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }

                    for (int k = 0; k < 70; k++)
                    {
                        Color newColor3 = (Main.rand.NextBool() ? Color.Lavender : Color.Crimson);
                        newColor3.A = 150;
                        int num2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, newColor3, 3f);
                        Main.dust[num2].noGravity = true;
                        Main.dust[num2].velocity *= 5f;
                        num2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, NPC.alpha, newColor3, 2f);
                        Main.dust[num2].velocity *= 2f;
                    }

                    if (!DownedBossSystem.downedSlimeGod)
                    {
                        Color magenta = Color.Magenta;
                        CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.SlimeGodRun", magenta);
                    }

                    for (int num3 = 254; num3 >= 0; num3--)
                    {
                        NPC.ApplyInteraction(num3);
                    }

                    NPC.active = false;
                    NPC.HitEffect();
                    NPC.NPCLoot();
                    NPC.netUpdate = true;
                }
            }
            #endregion
        }
    }
}