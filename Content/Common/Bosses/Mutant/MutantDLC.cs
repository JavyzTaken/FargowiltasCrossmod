using CalamityMod;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Common.Bosses.Mutant
{
    public class MutantDLC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        private static bool Thorium => ModCompatibility.ThoriumMod.Loaded;
        private static bool Calamity => ModCompatibility.Calamity.Loaded;
        private static bool ShouldDoDLC 
        {
            get => Calamity;
        }
        public override bool AppliesToEntity(NPC npc, bool lateInstantiation) => npc.type == ModContent.NPCType<MutantBoss>() && ShouldDoDLC  && DLCCalamityConfig.Instance.MutantDLC;

        public static void ManageMusic(NPC npc)
        {
            
            if (npc.localAI[3] >= 3 || npc.ai[0] == 10) //p2 or phase transition
            {
                npc.ModNPC.Music = MusicLoader.GetMusicSlot("FargowiltasCrossmod/Assets/Music/IrondustRePrologue");
            }
            else
            {
                
            }
        }
        public enum DLCAttack
        {
            PBGDrift = -2,
            PBGDash = -1,
            None = 0,
            PrepareAresNuke,
            AresNuke
        };
        public DLCAttack DLCAttackChoice = 0;
        public Vector2 LockVector1;
        public int Timer = 0;
        public int Counter = 0;
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write((int)DLCAttackChoice);
            binaryWriter.WriteVector2(LockVector1);
            binaryWriter.Write(Timer);
            binaryWriter.Write(Counter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            DLCAttackChoice = (DLCAttack)binaryReader.Read();
            LockVector1 = binaryReader.ReadVector2();
            Timer = binaryReader.Read();
            Counter = binaryReader.Read();
        }

        public override bool PreAI(NPC npc)
        {
            if (!ShouldDoDLC)
            {
                return true;
            }

            Player player = Main.player[npc.target];
            if (player == null || !player.active || player.dead)
            {
                return true;
            }

            ref float attackChoice = ref npc.ai[0];
            //DO NOT TOUCH npc.ai[1] DURING PHASE 1 CUSTOM ATTACKS

            switch (attackChoice) //attack reroutes
            {
                case 4:
                    if (Calamity)
                    {
                        DLCAttackChoice = DLCAttack.PBGDrift;
                        npc.netUpdate = true;
                    }
                    break;

                case 33:
                    if (Calamity)
                    {
                        DLCAttackChoice = DLCAttack.PrepareAresNuke;
                        npc.netUpdate = true;
                    }
                    break;
            }
            switch (attackChoice) //additions to normal attacks
            {
                case 38:
                case 30:
                    if (Calamity) CalamityFishron(); break;
            }
            #region Attack Additions
            [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
            void CalamityFishron()
            {
                const int fishronDelay = 3;
                int maxFishronSets = WorldSavingSystem.MasochistModeReal ? 3 : 2;
                if (npc.ai[1] == fishronDelay * maxFishronSets + 35)
                {
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/OldDukeHuff"), Main.LocalPlayer.Center);
                    if (DLCUtils.HostCheck)
                    {
                        for (int j = -1; j <= 1; j += 2) //to both sides of player
                        {
                            Vector2 offset = npc.ai[2] == 0 ? Vector2.UnitX * -450f * j : Vector2.UnitY * 475f * j;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<MutantOldDuke>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, offset.X, offset.Y);
                        }
                    }
                }
            }
            #endregion
            if (DLCAttackChoice < DLCAttack.None) //p1
            {
                attackChoice = 6; //p1 "while dashing", has very few effects
                npc.ai[1]--; //negate normal timer
            }
            if (DLCAttackChoice > DLCAttack.None) //p2
            {
                attackChoice = 28; //empty normal attack
            }
            switch (DLCAttackChoice) //new attacks
            {
                case DLCAttack.PBGDrift: PBGDrift(); break;
                case DLCAttack.PBGDash: PBGDash(); break;
                case DLCAttack.PrepareAresNuke: PrepareAresNuke(); break;
                case DLCAttack.AresNuke: AresNuke(); break;
                //case DLCAttack.PBGDash: PBGDash() break;
            }
            return base.PreAI(npc);

            #region Checks and Commons
            bool AliveCheck(Player p, bool forceDespawn = false)
            {
                if (WorldSavingSystem.SwarmActive || forceDespawn || (!p.active || p.dead || Vector2.Distance(npc.Center, p.Center) > 3000f) && npc.localAI[3] > 0)
                {
                    npc.TargetClosest();
                    p = Main.player[npc.target];
                    if (WorldSavingSystem.SwarmActive || forceDespawn || !p.active || p.dead || Vector2.Distance(npc.Center, p.Center) > 3000f)
                    {
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;
                        npc.velocity.Y -= 1f;
                        if (npc.timeLeft == 1)
                        {
                            if (npc.position.Y < 0)
                                npc.position.Y = 0;
                            if (DLCUtils.HostCheck && ModContent.TryFind("Fargowiltas", "Mutant", out ModNPC modNPC) && !NPC.AnyNPCs(modNPC.Type))
                            {
                                FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
                                int n = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, modNPC.Type);
                                if (n != Main.maxNPCs)
                                {
                                    Main.npc[n].homeless = true;
                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                                }
                            }
                        }
                        return false;
                    }
                }

                if (npc.timeLeft < 3600)
                    npc.timeLeft = 3600;

                if (player.Center.Y / 16f > Main.worldSurface)
                {
                    npc.velocity.X *= 0.95f;
                    npc.velocity.Y -= 1f;
                    if (npc.velocity.Y < -32f)
                        npc.velocity.Y = -32f;
                    return false;
                }

                return true;
            }

            bool Phase2Check()
            {
                if (Main.expertMode && npc.life < npc.lifeMax / 2)
                {
                    if (DLCUtils.HostCheck)
                    {
                        npc.ai[0] = 10;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                        npc.netUpdate = true;
                        FargoSoulsUtil.ClearHostileProjectiles(1, npc.whoAmI);
                        EdgyBossText("Time to stop playing around.");
                    }
                    return true;
                }
                return false;
            }
            void EdgyBossText(string text) //because it's FUCKING private
            {
                if (Main.zenithWorld) //edgy boss text
                {
                    Color color = Color.Cyan;
                    FargoSoulsUtil.PrintText(text, color);
                    CombatText.NewText(npc.Hitbox, color, text, true);
                    /*
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(text, Color.LimeGreen);
                    else if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.LimeGreen);
                    */
                }
            }
            void Movement(Vector2 target, float speed, float maxSpeed = 24, bool fastX = true, bool obeySpeedCap = true)
            {
                float turnaroundModifier = 1f;

                if (WorldSavingSystem.MasochistModeReal)
                {
                    speed *= 2;
                    turnaroundModifier *= 2f;
                    maxSpeed *= 1.5f;
                }

                if (Math.Abs(npc.Center.X - target.X) > 10)
                {
                    if (npc.Center.X < target.X)
                    {
                        npc.velocity.X += speed;
                        if (npc.velocity.X < 0)
                            npc.velocity.X += speed * (fastX ? 2 : 1) * turnaroundModifier;
                    }
                    else
                    {
                        npc.velocity.X -= speed;
                        if (npc.velocity.X > 0)
                            npc.velocity.X -= speed * (fastX ? 2 : 1) * turnaroundModifier;
                    }
                }
                if (npc.Center.Y < target.Y)
                {
                    npc.velocity.Y += speed;
                    if (npc.velocity.Y < 0)
                        npc.velocity.Y += speed * 2 * turnaroundModifier;
                }
                else
                {
                    npc.velocity.Y -= speed;
                    if (npc.velocity.Y > 0)
                        npc.velocity.Y -= speed * 2 * turnaroundModifier;
                }

                if (obeySpeedCap)
                {
                    if (Math.Abs(npc.velocity.X) > maxSpeed)
                        npc.velocity.X = maxSpeed * Math.Sign(npc.velocity.X);
                    if (Math.Abs(npc.velocity.Y) > maxSpeed)
                        npc.velocity.Y = maxSpeed * Math.Sign(npc.velocity.Y);
                }
            }
            void ChooseNextAttack(params int[] args)
            {
                MutantBoss mutantBoss = npc.ModNPC as MutantBoss;
                float buffer = npc.ai[0] + 1;
                npc.ai[0] = 48;
                npc.ai[1] = 0;
                npc.ai[2] = buffer;
                npc.ai[3] = 0;
                npc.localAI[0] = 0;
                npc.localAI[1] = 0;
                npc.localAI[2] = 0;
                //NPC.TargetClosest();
                npc.netUpdate = true;

                /*string text = "-------------------------------------------------";
                Main.NewText(text);

                text = "";
                foreach (float f in attackHistory)
                    text += f.ToString() + " ";
                Main.NewText($"history: {text}");*/

                if (WorldSavingSystem.EternityMode)
                {
                    //become more likely to use randoms as life decreases
                    bool useRandomizer = npc.localAI[3] >= 3 && (WorldSavingSystem.MasochistModeReal || Main.rand.NextFloat(0.8f) + 0.2f > (float)Math.Pow((float)npc.life / npc.lifeMax, 2));

                    if (DLCUtils.HostCheck)
                    {
                        Queue<float> recentAttacks = new(mutantBoss.attackHistory); //copy of attack history that i can remove elements from freely

                        //if randomizer, start with a random attack, else use the previous state + 1 as starting attempt BUT DO SOMETHING ELSE IF IT'S ALREADY USED
                        if (useRandomizer)
                            npc.ai[2] = Main.rand.Next(args);

                        //Main.NewText(useRandomizer ? "(Starting with random)" : "(Starting with regular next attack)");

                        while (recentAttacks.Count > 0)
                        {
                            bool foundAttackToUse = false;

                            for (int i = 0; i < 5; i++) //try to get next attack that isnt in this queue
                            {
                                if (!recentAttacks.Contains(npc.ai[2]))
                                {
                                    foundAttackToUse = true;
                                    break;
                                }
                                npc.ai[2] = Main.rand.Next(args);
                            }

                            if (foundAttackToUse)
                                break;

                            //couldn't find an attack to use after those attempts, forget 1 attack and repeat
                            recentAttacks.Dequeue();

                            //Main.NewText("REDUCE");
                        }

                        /*text = "";
                        foreach (float f in recentAttacks)
                            text += f.ToString() + " ";
                        Main.NewText($"recent: {text}");*/
                    }
                }

                if (DLCUtils.HostCheck)
                {
                    int maxMemory = WorldSavingSystem.MasochistModeReal ? 10 : 16;

                    if (mutantBoss.attackCount++ > maxMemory * 1.25) //after doing this many attacks, shorten queue so i can be more random again
                    {
                        mutantBoss.attackCount = 0;
                        maxMemory /= 4;
                    }

                    mutantBoss.attackHistory.Enqueue(npc.ai[2]);
                    while (mutantBoss.attackHistory.Count > maxMemory)
                        mutantBoss.attackHistory.Dequeue();
                }

                mutantBoss.endTimeVariance = WorldSavingSystem.MasochistModeReal ? Main.rand.NextFloat() : 0;

                /*text = "";
                foreach (float f in attackHistory)
                    text += f.ToString() + " ";
                Main.NewText($"after: {text}");*/
            }
            #endregion
            #region Phase 1 Attacks
            [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
            void PBGDrift()
            {
                if (!AliveCheck(player))
                    return;
                if (Phase2Check())
                    return;
                const int WindupTime = 60;
                const int DriftTime = 200;
                const float TotalDriftAngle = MathHelper.TwoPi * 2f;
                if (Timer == 0)
                {
                    LockVector1 = player.DirectionTo(npc.Center) * 520;
                }
                if (Timer == WindupTime)
                {
                    
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<DLCMutantSpearSpin>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, DriftTime);
                }
                if (npc.Distance(player.Center + LockVector1) < 400)
                {
                    LockVector1 = LockVector1.RotatedBy(TotalDriftAngle / DriftTime);
                }
                
                Vector2 targetPos = player.Center + LockVector1;
                Movement(targetPos, 2f, player.velocity.Length() / 2 + 30, false);
                if (Timer > WindupTime + DriftTime)
                {
                    Timer = 0;
                    DLCAttackChoice = DLCAttack.PBGDash;
                    return;
                }
                Timer++;
            }
            [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
            void PBGDash()
            {
                const int WindupTime = 30;

                if (Timer == 1)
                {
                    if (DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<DLCMutantSpearDash>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI);
                    }
                }
                if (Timer < WindupTime)
                {
                    npc.velocity *= 0.9f;
                }
                if (Timer == WindupTime)
                {
                    npc.netUpdate = true;
                    float speed = 45f;
                    npc.velocity = speed * npc.DirectionTo(player.Center + player.velocity * 10);
                    if (DLCUtils.HostCheck)
                    {
                        if (WorldSavingSystem.MasochistModeReal)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Normalize(npc.velocity), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, -Vector2.Normalize(npc.velocity), ModContent.ProjectileType<MutantDeathray2>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                        }
                    }
                }
                if (Timer > WindupTime + 30)
                {
                    DLCAttackChoice = 0;
                    Timer = 0;
                    npc.ai[0] = 5;
                    npc.ai[2] = 5;
                    return;
                }
                Timer++;
            }
            #endregion
            #region Phase 2 Attacks
            [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
            void PrepareAresNuke()
            {
                if (!AliveCheck(player))
                    return;
                /*
                Vector2 targetPos = player.Center;
                targetPos.X += 400 * (npc.Center.X < targetPos.X ? -1 : 1);
                targetPos.Y -= 400;
                */
                int nukeTime = (Counter > 0 ? 90 : 180);
                if (Timer == 0)
                {
                    MutantBoss mutantBoss = (npc.ModNPC as MutantBoss);
                    Vector2 pos = FargoSoulsUtil.ProjectileExists(mutantBoss.ritualProj, ModContent.ProjectileType<MutantRitual>()) == null ? npc.Center : Main.projectile[mutantBoss.ritualProj].Center;
                    
                    if (Counter <= 0)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                        Vector2 targetPos = player.Center;
                        targetPos.X += 400 * (npc.Center.X < targetPos.X ? -1 : 1);
                        targetPos.Y -= 400;
                        LockVector1 = targetPos;
                    }
                    else
                    {
                        Vector2 dir = Utils.SafeNormalize(npc.Center - pos, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi));
                        LockVector1 = pos + (-dir).RotatedByRandom(MathHelper.TwoPi / 4) * 500;
                    }
                        

                    if (DLCUtils.HostCheck)
                    {
                        float gravity = 0.2f;
                        float time = nukeTime;
                        Vector2 distance = player.Center - npc.Center;
                        distance.X /= time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        int ritual = ModContent.ProjectileType<DLCMutantFishronRitual>();
                        if (Main.LocalPlayer.ownedProjectileCounts[ritual] <= 0)
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ritual, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 3f), 0f, Main.myPlayer, npc.whoAmI);
                        int nuke = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, distance, ModContent.ProjectileType<MutantAresNuke>(), WorldSavingSystem.MasochistModeReal ? FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 3f) : 0, 0f, Main.myPlayer, gravity);
                        if (nuke.WithinBounds(Main.maxNPCs))
                        {
                            Main.npc[nuke].timeLeft = nukeTime;
                        }
                        
                    }
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 offset = new();
                    offset.Y -= 100;
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * 150);
                    offset.Y += (float)(Math.Cos(angle) * 150);
                    Dust dust = Main.dust[Dust.NewDust(npc.Center + offset - new Vector2(4, 4), 0, 0, DustID.Vortex, 0, 0, 100, Color.White, 1.5f)];
                    dust.velocity = npc.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * 5f;
                    dust.noGravity = true;
                }

                if (npc.Distance(LockVector1) > 80)
                {
                    Movement(LockVector1, 1.2f, fastX: false);
                }
                else
                {
                    npc.velocity *= 0.9f;
                }

                if (++Timer > nukeTime)
                {
                    foreach (Projectile projectile in Main.projectile.Where(p => p != null && p.active && p.type == ModContent.ProjectileType<MutantAresNuke>()))
                    {
                        projectile.Kill();
                    }
                    DLCAttackChoice = DLCAttack.AresNuke;
                    Timer = 0;

                    if (Math.Sign(player.Center.X - npc.Center.X) == Math.Sign(npc.velocity.X))
                        npc.velocity.X *= -1f;
                    if (npc.velocity.Y < 0)
                        npc.velocity.Y *= -1f;
                    npc.velocity.Normalize();
                    npc.velocity *= 3f;
                    
                    npc.netUpdate = true;
                    //NPC.TargetClosest();
                }
            }
            [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
            void AresNuke()
            {
                if (!AliveCheck(player))
                    return;

                if (WorldSavingSystem.MasochistModeReal)
                {
                    Vector2 target = npc.Bottom.Y < player.Top.Y
                        ? player.Center + 300f * Vector2.UnitX * Math.Sign(npc.Center.X - player.Center.X)
                        : npc.Center + 30 * npc.DirectionFrom(player.Center).RotatedBy(MathHelper.ToRadians(60) * Math.Sign(player.Center.X - npc.Center.X));
                    Movement(target, 0.1f);
                    if (npc.velocity.Length() > 2f)
                        npc.velocity = Vector2.Normalize(npc.velocity) * 2f;
                }
                if (!Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.FargoSouls().Screenshake = 2;

                if (DLCUtils.HostCheck)
                {
                    Vector2 safeZone = npc.Center;
                    safeZone.Y -= 100;
                    const float safeRange = 150 + 200;
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 spawnPos = npc.Center + Main.rand.NextVector2Circular(1200, 1200);
                        if (Vector2.Distance(safeZone, spawnPos) < safeRange)
                        {
                            Vector2 directionOut = spawnPos - safeZone;
                            directionOut.Normalize();
                            spawnPos = safeZone + directionOut * Main.rand.NextFloat(safeRange, 1200);
                        }
                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<MutantAresBomb>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 3f), 0f, Main.myPlayer);
                    }
                }

                if (++Timer > (Counter > 2 ? 80 : 30))
                {
                    if (++Counter > 3)
                    {
                        ChooseNextAttack(11, 13, 16, 19, 24, WorldSavingSystem.MasochistModeReal ? 26 : 29, 31, 35, 37, 39, 41, 42);
                        Counter = 0;
                        DLCAttackChoice = DLCAttack.None;
                    }
                    else
                    {
                        DLCAttackChoice = DLCAttack.PrepareAresNuke;
                    }
                    Timer = 0;
                    
                }
            }
            #endregion

        }

        public override void PostAI(NPC npc)
        {
            ManageMusic(npc);
            if (DLCAttackChoice < DLCAttack.None) //p1, negate "while dashing" code that makes him face his velocity
            {
                npc.direction = npc.spriteDirection = npc.Center.X < Main.player[npc.target].Center.X ? 1 : -1;
            }
        }

    }
}
