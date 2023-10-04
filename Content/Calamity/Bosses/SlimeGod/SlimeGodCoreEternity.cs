
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SlimeGodCoreEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<SlimeGodCore>());


        #region Variables

        public static readonly SoundStyle PossessionSound = new SoundStyle("CalamityMod/Sounds/Custom/SlimeGodPossession", (SoundType)0);

        public static readonly SoundStyle ExitSound = new SoundStyle("CalamityMod/Sounds/Custom/SlimeGodExit", (SoundType)0);

        public static readonly SoundStyle ShotSound = new SoundStyle("CalamityMod/Sounds/Custom/SlimeGodShot", 2, (SoundType)0);

        public static readonly SoundStyle BigShotSound = new SoundStyle("CalamityMod/Sounds/Custom/SlimeGodBigShot", 2, (SoundType)0);

        public static int CrimsonGodType => ModContent.NPCType<CrimulanPaladin>();
        public static int CorruptionGodType => ModContent.NPCType<EbonianPaladin>();

        private static List<int> SlimesToKill = new List<int>
        {
            CrimsonGodType,
            CorruptionGodType,
            ModContent.NPCType<CorruptSlimeSpawn>(),
            ModContent.NPCType<CorruptSlimeSpawn2>(),
            ModContent.NPCType<CrimsonSlimeSpawn>(),
            ModContent.NPCType<CorruptSlimeSpawn2>(),
            ModContent.NPCType<SplitCrimulanPaladin>(),
            ModContent.NPCType<SplitEbonianPaladin>(),
        };


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
            Attacking
        }
        public enum Attacks
        {
            SlimeBreakout = -1,
            Drift,
            MettatonHeart,
            SpinDash,
        }
        public List<int> AttackCycle = new List<int> 
        { 
            (int)Attacks.Drift,
            (int)Attacks.MettatonHeart,
            (int)Attacks.SpinDash
        };
        #endregion
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(AttachedSlime);
            binaryWriter.Write(AttachToCrimson);
            binaryWriter.Write(ContactDamage);
            binaryWriter.Write7BitEncodedInt(ReattachTimer);
            binaryWriter.WriteVector2(LockVector);
            binaryWriter.Write(LatestAttack);

        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            AttachToCrimson = binaryReader.ReadBoolean();
            AttachedSlime = binaryReader.Read7BitEncodedInt();
            ContactDamage = binaryReader.ReadBoolean();
            ReattachTimer = binaryReader.Read7BitEncodedInt();
            LockVector = binaryReader.ReadVector2();
            LatestAttack = binaryReader.Read7BitEncodedInt();
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) => ContactDamage;


        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            npc.lifeMax = 10000;
        }
        public override bool SafePreAI(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            #region Passive
            //FargoSoulsUtil.PrintAI(npc);
            CalamityGlobalNPC.slimeGod = npc.whoAmI;
            ref float timer = ref npc.ai[0];
            ref float attack = ref npc.ai[2];
            ref float phase = ref npc.ai[3];

            npc.dontTakeDamage = !(phase == (int)Phases.Attacking);

            if (phase != (int)Phases.Attached)
            {
                npc.rotation += npc.velocity.Length() * (MathHelper.Pi / 300f);
            }
            else //attached
            {
                npc.Opacity = 0.3f;
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
                            NetSync(npc);
                            npc.netUpdate = true;
                        }
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
                Player p = Main.player[npc.target];
                if (!p.active || p.dead || Vector2.Distance(npc.Center, p.Center) > despawnRange)
                {
                    npc.TargetClosest();
                    p = Main.player[npc.target];
                    if (!p.active || p.dead || Vector2.Distance(npc.Center, p.Center) > despawnRange)
                    {
                        npc.noTileCollide = true;
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;
                        npc.velocity.Y -= 1f;
                        if (npc.timeLeft == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
                            }
                        }
                        return false;
                    }
                }
                return true;
            }
            void FullReset()
            {
                npc.ai[0] = 0;
                npc.ai[1] = 0;
                ReattachTimer = 0;
                NetSync(npc);
                npc.netUpdate = true;
            }
            void AttackReset()
            {
                npc.ai[0] = 0;
                npc.ai[1] = 0;
                LockVector = Vector2.Zero;
                NetSync(npc);
                npc.netUpdate = true;
            }
            void ResetToDrift()
            {
                ref float attack = ref npc.ai[2];
                LatestAttack = (int)attack;
                attack = (int)Attacks.Drift;
                AttackReset();
            }
            #endregion
            #region Special Actions
            void SummonSlimes()
            {
                ref float timer = ref npc.ai[0];
                ref float phase = ref npc.ai[3];
                if (timer < 120)
                {
                    Vector2 desiredPos = Main.player[npc.target].Center - Vector2.UnitY * 300;
                    npc.velocity = (desiredPos - npc.Center) * 0.05f;
                }
                if (timer >= 120)
                {
                    if (DLCUtils.HostCheck)
                    {
                        const int spawnTossSpeed = 30;
                        if (!NPC.AnyNPCs(CrimsonGodType))
                        {
                            int crim = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, CrimsonGodType, Target: npc.target);
                            if (crim != Main.maxNPCs)
                            {
                                Main.npc[crim].velocity = Vector2.UnitX * -spawnTossSpeed;
                                if (AttachToCrimson)
                                {
                                    AttachedSlime = crim;
                                    phase = (int)Phases.TryAttach;
                                        
                                }
                            }
                        }
                        if (!NPC.AnyNPCs(CorruptionGodType))
                        {
                            int corr = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, CorruptionGodType, Target: npc.target);
                            if (corr != Main.maxNPCs)
                            {
                                Main.npc[corr].velocity = Vector2.UnitX * spawnTossSpeed;
                                if (!AttachToCrimson)
                                {
                                    AttachedSlime = corr;
                                    phase = (int)Phases.TryAttach;
                                }
                            }
                        }
                        AttachToCrimson = !AttachToCrimson;
                        FullReset();
                    }
                    SoundEngine.PlaySound(PossessionSound, npc.Center);
                }
            }
            void TryAttaching()
            {
                ref float timer = ref npc.ai[0];
                ref float phase = ref npc.ai[3];
                NPC slime = Main.npc[AttachedSlime];
                ContactDamage = false;
                if (slime != null && slime.active)
                {
                    float modifier = timer / 60f;
                    npc.velocity = npc.DirectionTo(slime.Center) * 20f * modifier;
                    if (npc.Distance(slime.Center) <= npc.velocity.Length() * 2f)
                    {
                        phase = (int)Phases.Attached;
                        FullReset();
                    }
                }
                else
                {
                    FullReset();
                }
            }
            void Attached()
            {
                ref float timer = ref npc.ai[0];
                ref float phase = ref npc.ai[3];
                ref float attack = ref npc.ai[2];
                NPC slime = Main.npc[AttachedSlime];
                ContactDamage = false;
                if (slime != null && slime.active && (slime.type == CrimsonGodType || slime.type == CorruptionGodType))
                {
                    npc.velocity = (slime.Center - npc.Center) + slime.velocity;
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
                ref float timer = ref npc.ai[0];
                ref float attack = ref npc.ai[2];
                if (npc.Opacity < 1)
                {
                    npc.Opacity += 0.7f * (1 / 120f); //takes 2 seconds
                    npc.velocity *= 0.95f;
                }
                else
                {
                    foreach (NPC npc in Main.npc.Where(n => n != null && n.active && SlimesToKill.Contains(n.type)))
                    {
                        npc.active = false;
                    }
                    npc.velocity = Vector2.Zero;
                    npc.Opacity = 1;
                    const int ShotCount = 16;
                    SoundEngine.PlaySound(ExitSound, npc.Center);
                    //screenshake
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = 0; i < ShotCount; i++)
                        {
                            int type = i % 2 == 0 ? ModContent.ProjectileType<AcceleratingCrimulanGlob>() : ModContent.ProjectileType<AcceleratingEbonianGlob>();
                            float speed = Main.rand.NextFloat(2f, 2.6f);
                            Vector2 dir = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / ShotCount).RotatedByRandom(MathHelper.TwoPi / (ShotCount * 4));
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + dir * npc.width / 3, dir * speed, type, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 3f, Main.myPlayer);
                        }
                    }
                    attack = Main.rand.NextFromCollection(AttackCycle);
                }
            }
            void MettatonHeart()
            {
                ref float timer = ref npc.ai[0];
                ref float partialTimer = ref npc.ai[1];
                ref float attack = ref npc.ai[2];
                Player player = Main.player[npc.target];

                const int partialAttackTime = 100;
                const int partialAttacks = 3;
                if (partialTimer <= 0)
                {
                    const int minDistance = 200;
                    float dir = Main.rand.NextBool() ? 1 : -1;
                    LockVector = player.Center + (player.DirectionTo(npc.Center) * minDistance).RotatedBy(dir * MathHelper.Pi / 3f);
                    NetSync(npc);
                    npc.netUpdate = true;
                    partialTimer++;
                }
                else
                {
                    npc.velocity = (partialTimer / partialAttackTime) * (LockVector - npc.Center) * 0.65f;

                    if (partialTimer >= partialAttackTime / 4) //latter 3/4s of partial attack
                    {
                        if (partialTimer % (partialAttackTime / 4) == 1) //first frame of cycle
                        {
                            float rot = npc.DirectionTo(player.Center).ToRotation();
                            if (partialTimer >= partialAttackTime / 2)
                            {
                                int dir = partialTimer >= partialAttackTime * 0.7f ? 1 : -1;
                                rot = rot + dir * MathHelper.Pi / 7.65f; //funny weird number
                                
                            }
                            npc.rotation = rot;
                        }
                        if (partialTimer % (partialAttackTime / 10) == 5) //shoot
                        {
                            SoundEngine.PlaySound(ShotSound, npc.Center);
                            if (DLCUtils.HostCheck)
                            {
                                Vector2 dir = npc.rotation.ToRotationVector2();
                                float speed = 5f;
                                bool crim = partialTimer >= partialAttackTime / 2;
                                if (!AttachToCrimson)
                                {
                                    crim = !crim;
                                }
                                int type = crim ? ModContent.ProjectileType<AcceleratingCrimulanGlob>() : ModContent.ProjectileType<AcceleratingEbonianGlob>();
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + dir * npc.width / 3, dir * speed, type, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 3f, Main.myPlayer);
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
                ref float timer = ref npc.ai[0];
                ref float partialTimer = ref npc.ai[1];
                ref float attack = ref npc.ai[2];
                Player player = Main.player[npc.target];
                const int DashTime = 100;
                const int Dashes = 3;
                float speedmod = 1.2f;
                if (partialTimer < DashTime * 0.7f)
                {
                    npc.velocity *= 0.95f;
                    speedmod /= 6f;
                    float modifier = 3f;
                    npc.rotation += modifier * MathHelper.Pi / DashTime;
                    const int shotDelay = 3;
                    if (partialTimer % shotDelay == shotDelay - 1 && partialTimer > DashTime / 4)
                    {
                        SoundEngine.PlaySound(ShotSound, npc.Center);
                        if (DLCUtils.HostCheck)
                        {
                            Vector2 dir = npc.rotation.ToRotationVector2();
                            float speed = 3.25f;
                            bool crim = timer > DashTime && timer < DashTime * 2;
                            if (!AttachToCrimson)
                            {
                                crim = !crim;
                            }
                            int type = crim ? ModContent.ProjectileType<AcceleratingCrimulanGlob>() : ModContent.ProjectileType<AcceleratingEbonianGlob>();
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + dir * npc.width / 3, dir * speed, type, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 3f, Main.myPlayer);
                        }
                    }
                }
                npc.velocity += npc.DirectionTo(player.Center) * speedmod;
                npc.velocity.ClampMagnitude(0, 35);
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
                ref float timer = ref npc.ai[0];
                ref float driftDir = ref npc.ai[1];
                ref float attack = ref npc.ai[2];
                Player player = Main.player[npc.target];
                const int DriftDuration = 60;
                const int MaxDistance = 300;
                const int MinDistance = 100;
                if (timer <= 1)
                {
                    LockVector = (npc.Center - player.Center).ClampMagnitude(MinDistance, MaxDistance);
                    driftDir = Main.rand.NextBool() ? 1 : -1;
                }
                else if (timer < DriftDuration)
                {
                    float modifier = 0.85f; //fraction of half circle to drift
                    LockVector = LockVector.RotatedBy(driftDir * MathHelper.Pi * modifier / DriftDuration);
                    npc.velocity = (player.Center + LockVector - npc.Center) * (timer / DriftDuration);

                    const int shotDelay = 12;
                    if (timer % shotDelay == shotDelay - 1)
                    {
                        SoundEngine.PlaySound(ShotSound, npc.Center);
                        if (DLCUtils.HostCheck)
                        {
                            int shotSide = -Math.Sign(FargoSoulsUtil.RotationDifference(npc.DirectionTo(player.Center), npc.velocity));
                            Vector2 dir = Vector2.Normalize(npc.velocity).RotatedBy(MathHelper.PiOver2 * shotSide);
                            float speed = 3.25f;
                            bool crim = timer % (shotDelay * 2 ) >= shotDelay;
                            int type = crim ? ModContent.ProjectileType<AcceleratingCrimulanGlob>() : ModContent.ProjectileType<AcceleratingEbonianGlob>();
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + dir * npc.width / 3, dir * speed, type, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 3f, Main.myPlayer);
                        }
                    }

                    if (npc.velocity.Length() > 20)
                    {
                        npc.velocity = Vector2.Normalize(npc.velocity) * 20;
                    }
                }
                else
                {
                    npc.velocity *= 0.96f;
                    if (npc.velocity.Length() < 2)
                    {
                        attack = LatestAttack == (int)Attacks.MettatonHeart ? (int)Attacks.SpinDash : (int)Attacks.MettatonHeart; //defaults to latter for first attack
                        AttackReset();
                    }
                }
            }
            #endregion
        }
    }
}