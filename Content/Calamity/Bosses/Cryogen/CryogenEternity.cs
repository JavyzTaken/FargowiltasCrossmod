
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasCrossmod.Core.Common.Systems;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CryogenEternity : EModeCalBehaviour
    {
        public const bool Enabled = true;
        public override bool IsLoadingEnabled(Mod mod) => Enabled;

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<CalamityMod.NPCs.Cryogen.Cryogen>());
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> shield = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/CryogenShield");
            float x = shieldDrawTimer / 200f;
            x = (float)-(Math.Cos(Math.PI * x) - 1) / 2;
            float scaleAdd = MathHelper.Lerp(-0.2f, 0.2f, x);

            if (npc.ai[0] != 3)
            {
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                    Color glowColor = Color.Blue with { A = 0 } * 0.7f;


                    spriteBatch.Draw(shield.Value, npc.Center + afterimageOffset - screenPos, null, glowColor, 0, shield.Size() / 2, npc.scale + scaleAdd, SpriteEffects.None, 0);
                }
                spriteBatch.Draw(shield.Value, npc.Center - screenPos, null, drawColor, 0, shield.Size() / 2, npc.scale + scaleAdd, SpriteEffects.None, 0);
            }
            if (npc.ai[0] == 3)
            {
                Asset<Texture2D> cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase1");
                if (npc.ai[2] == 1) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase2");
                if (npc.ai[2] == 2) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase3");
                if (npc.ai[2] == 3) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase4");
                if (npc.ai[2] == 4) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase5");
                if (npc.ai[2] == 5) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase6");
                Vector2 offset = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)) * npc.ai[1] / 120;
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                    Color glowColor = Color.Blue with { A = 0 } * 0.7f;


                    spriteBatch.Draw(cryo1.Value, npc.Center + afterimageOffset - screenPos + offset, null, glowColor, 0, cryo1.Size() / 2, npc.scale, SpriteEffects.None, 0);
                }
                spriteBatch.Draw(cryo1.Value, npc.Center - screenPos + offset, null, drawColor, 0, cryo1.Size() / 2, npc.scale, SpriteEffects.None, 0);
                return false;
            }
            return true;
        }
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            base.HitEffect(npc, hit);
        }
        public int[] Chains = new int[] { -1, -1, -1, -1 , -1, -1};
        public float shieldDrawTimer;
        public float shieldDrawCounter;

        public int RitualProj;

        public const int ArenaRadius = 1200;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt((int)npc.localAI[0]);
            binaryWriter.Write7BitEncodedInt((int)npc.localAI[1]);
            binaryWriter.Write7BitEncodedInt((int)npc.localAI[2]);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            npc.localAI[0] = binaryReader.Read7BitEncodedInt();
            npc.localAI[1] = binaryReader.Read7BitEncodedInt();
            npc.localAI[2] = binaryReader.Read7BitEncodedInt();
        }
        private enum Attacks
        {
            Idle,
            HomingShards,
            ShardSweep,
            ShardStorm
        };
        private List<Attacks> AttackChoices = new List<Attacks>
        {
            //Attacks.HomingShards,
            Attacks.ShardSweep,
            Attacks.ShardStorm
        };

        const int chainTime = IceChain.ActiveTime;
        const int chainStartTime = 62;
        bool evenChain(NPC npc) => npc.ai[1] % (chainTime * 2) >= chainTime;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget || !DLCCalamityConfig.Instance.EternityPriorityOverRev || !DLCWorldSavingSystem.EternityRev) return true;

            Player target = Main.player[npc.target];
            ref float attack = ref npc.ai[0];
            ref float timer = ref npc.ai[1];
            ref float data = ref npc.ai[2];
            ref float data2 = ref npc.ai[3];

            ref float attackChoice = ref npc.localAI[0];
            ref float attackTimer = ref npc.localAI[1];
            ref float data3 = ref npc.localAI[2];

            

            npc.scale = 1.5f;
            npc.dontTakeDamage = false;
            if (shieldDrawCounter == 0)
            {
                shieldDrawTimer++;
                if (shieldDrawTimer >= 200) shieldDrawCounter = 1;
            }
            else
            {
                shieldDrawTimer--;
                if (shieldDrawTimer <= 0) shieldDrawCounter = 0;
            }
            //move towards player
            if (npc.ai[0] == 0 || npc.ai[0] == 1)
            {
                npc.ai[0] = 0;
                npc.rotation = npc.velocity.X / 15f;
                timer++;
                if (timer == 1)
                {
                    //Main.NewText("j");
                    Vector2 pos = target.Center + new Vector2(0, -400);
                    data = pos.X; data2 = pos.Y;
                    NetSync(npc);
                }
                else if (npc.Distance(new Vector2(data, data2)) > 300){
                    Vector2 pos = new Vector2(data, data2);
                    npc.velocity = Vector2.Lerp(npc.velocity, (pos - npc.Center).SafeNormalize(Vector2.Zero) * 30, 0.05f);
                    
                }
                if (npc.Distance(new Vector2(data, data2)) < 300)
                {
                    npc.velocity /= 1.1f;
                }
                    if (npc.velocity.Length() < 1 && timer > 20)
                {
                    npc.rotation = 0;
                    data = 0;
                    data2 = 0;
                    timer = 0;
                    attack = 2;
                    npc.velocity *= 0;
                    NetSync(npc);
                }
                
            }
            if (npc.ai[0] == 2)
            {
                
                npc.velocity = Vector2.Zero;
                timer++;
                if (timer == 2)
                {
                    SoundEngine.PlaySound(CalamityMod.NPCs.Cryogen.Cryogen.ShieldRegenSound, npc.Center);
                    if (DLCUtils.HostCheck)
                    {
                        Chains[0] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: npc.whoAmI, ai1: MathHelper.ToRadians(-149));
                        Chains[1] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: npc.whoAmI, ai1: MathHelper.ToRadians(-90));
                        Chains[2] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: npc.whoAmI, ai1: MathHelper.ToRadians(-31));
                        Chains[3] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: npc.whoAmI, ai1: MathHelper.ToRadians(31));
                        Chains[4] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: npc.whoAmI, ai1: MathHelper.ToRadians(90));
                        Chains[5] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: npc.whoAmI, ai1: MathHelper.ToRadians(149));

                        RitualProj = Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<CryogenRitual>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, 0f, npc.whoAmI);
                    }
                    DustExplode(npc);
                }
                float chainCycleTime = timer % chainTime;
                if (chainCycleTime > chainStartTime - 10 && chainCycleTime < chainStartTime && attackChoice == (float)Attacks.ShardSweep)
                {
                    timer--;
                }
                if (timer % chainTime == chainStartTime)
                {
                    int even = evenChain(npc) ? 1 : 0;
                    for (int i = 0; i < Chains.Length; i++)
                    {
                        if (i % 2 == even)
                        {
                            int p = Chains[i];
                            if (p.WithinBounds(Main.maxProjectiles))
                                Main.projectile[p].ai[2] = 200;
                        }
                    }
                }
                void ToIdle()
                {
                    ref float attackChoice = ref npc.localAI[0];
                    ref float attackTimer = ref npc.localAI[1];
                    attackChoice = (int)Attacks.Idle;
                    attackTimer = 0;
                }
                switch ((Attacks)attackChoice)
                {
                    case Attacks.Idle:
                        {
                            if (attackTimer == 120)
                            {
                                SoundEngine.PlaySound(SoundID.Item30, npc.Center);
                                if (DLCUtils.HostCheck)
                                {
                                    float randSpread = Main.rand.NextFloat(5f, 8f);
                                    float randSpeedMod = Main.rand.NextFloat(65f, 90f);
                                    for (int i = 0; i < 6; i++) //hexagon of bombs
                                    {
                                        Vector2 toPlayer = target.Center - npc.Center;
                                        Vector2 offset = (Vector2.UnitY * npc.height / 2f).RotatedBy(MathHelper.TwoPi * i / 6f);
                                        Vector2 vel = (toPlayer).SafeNormalize(Vector2.Zero) * toPlayer.Length() / randSpeedMod;
                                        vel += Vector2.Normalize(offset) * randSpread;
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + offset, vel, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.IceBomb>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                                    }
                                }
                            }

                            attackTimer++;
                            if (attackTimer > 120 + 60)
                            {
                                attackChoice = (int)Main.rand.NextFromCollection(AttackChoices);
                                attackTimer = 0;
                                NetSync(npc);
                                npc.netUpdate = true;
                            }
                        }
                        break;
                    case Attacks.HomingShards:
                        {
                            attackTimer++;
                            if (attackTimer > 30)
                            {
                                ToIdle();
                            }
                        }
                        break;
                    case Attacks.ShardSweep:
                        {
                            if (attackTimer == 0)
                            {
                                // Find closest active ice chain in terms of rotation, to originate sweep from
                                int closestChain = -1;
                                int even = evenChain(npc) ? 1 : 0;
                                for (int i = 0; i < Chains.Length; i++)
                                {
                                    if (i % 2 != even)
                                        continue;
                                    int p = Chains[i];

                                    if (closestChain == -1)
                                        closestChain = p;
                                    else
                                    {
                                        Vector2 oldRotVec = Main.projectile[closestChain].Center - npc.Center;
                                        Vector2 newRotVec = Main.projectile[p].Center - npc.Center;
                                        Vector2 toPlayer = npc.DirectionTo(target.Center);
                                        float oldDif = Math.Abs(FargoSoulsUtil.RotationDifference(oldRotVec, toPlayer));
                                        float newDif = Math.Abs(FargoSoulsUtil.RotationDifference(newRotVec, toPlayer));

                                        if (newDif < oldDif)
                                            closestChain = p;
                                    }
                                }

                                if (closestChain == -1)
                                {
                                    ToIdle();
                                    //Main.NewText("oops 1");
                                    break;
                                }
                                else
                                {
                                    data3 = closestChain;
                                }
                                    
                            }
                            const float totalRotation = MathHelper.TwoPi / 6;
                            const int AttackTime = 60 * 3;
                            if (attackTimer > 0 && attackTimer % 30 == 0)
                                SoundEngine.PlaySound(SoundID.Item27);

                            if (attackTimer > 0 && attackTimer % 5 == 0)
                            {
                                
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    float progress = attackTimer / AttackTime;
                                    int cIndex = (int)data3;
                                    if (!cIndex.WithinBounds(Main.maxProjectiles))
                                    {
                                        ToIdle();
                                        //Main.NewText("oops 2");
                                        break;
                                    }
                                    Projectile chain = Main.projectile[cIndex];
                                    Vector2 chainDir = npc.DirectionTo(chain.Center);
                                    float rotDir = Math.Sign(FargoSoulsUtil.RotationDifference(chainDir, npc.DirectionTo(target.Center)));

                                    Vector2 velDir = chainDir.RotatedBy(rotDir * totalRotation * progress);
                                    float speed = 1;
                                    Vector2 vel = velDir * speed;
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, vel, ModContent.ProjectileType<IceRain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: 1);
                                }
                            }
                            attackTimer++;
                            if (attackTimer >= AttackTime)
                            {
                                attackChoice = (int)Attacks.Idle;
                                attackTimer = 0;
                            }
                        }
                        break;
                    case Attacks.ShardStorm:
                        {
                            if (attackTimer == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item28);
                                if (DLCUtils.HostCheck)
                                {
                                    for (int i = 0; i < 12; i++)
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(0, 1).RotatedBy(MathHelper.ToRadians(360f / 12 * i)), ModContent.ProjectileType<IceRain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: 1);
                                    }
                                    for (int i = 0; i < 12; i++)
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(0, 3).RotatedBy(MathHelper.ToRadians(360f / 12 * i)), ModContent.ProjectileType<IceRain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: 1);
                                    }
                                    for (int i = 0; i < 12; i++)
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(0, 2).RotatedBy(MathHelper.ToRadians(360f / 12 * i + 12)), ModContent.ProjectileType<IceRain>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: 1);
                                    }
                                }
                            }
                            attackTimer++;
                            if (attackTimer > 30)
                            {
                                ToIdle();
                            }
                                
                        }
                        break;
                }
                if (npc.GetLifePercent() <= 0.8f && !BossRushEvent.BossRushActive)
                {
                    attack = 3;
                    timer = 0;
                    data = -1;
                    data2 = 0;
                }
                
            }
            if (attack == 3) {


                if (DLCWorldSavingSystem.PermafrostPhaseSeen && timer % 60 < 55 && timer % 60 > 0)
                    timer += 2;

                //Main.musicVolume -= 0.003f;
                if (timer % 60 == 0)
                {
                    data++;
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenShieldBreak") with { Volume = 2}, target.Center);
                    if (!Main.dedServ && Main.netMode != NetmodeID.Server)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Gore.NewGore(npc.GetSource_FromAI(), npc.Center, new Vector2(0, -5).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), ModCompatibility.Calamity.Mod.Find<ModGore>("CryoDeathGore" + Main.rand.Next(2, 4)).Type, 1.5f);
                        }
                    }
                }
                if (data == 6)
                {
                    DustExplode(npc);
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenDeath") with { Volume = 2}, target.Center);
                    if (!Main.dedServ && Main.netMode != NetmodeID.Server)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Gore.NewGore(npc.GetSource_FromAI(), npc.Center, new Vector2(0, -5).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), ModCompatibility.Calamity.Mod.Find<ModGore>("CryoDeathGore" + Main.rand.Next(2, 4)).Type, 1.5f);
                        }
                    }
                    
                    //Main.musicVolume = 1;
                    if (DLCUtils.HostCheck)
                    {
                        int n = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y + 50, ModContent.NPCType<PermafrostBoss>());
                        if (n != Main.maxNPCs)
                        {
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                    DLCWorldSavingSystem.PermafrostPhaseSeen = true;
                    npc.active = false;
                }
                timer++;
            }

            return false;
        }
        public void DustExplode(NPC npc)
        {
            for (int i = 0; i < 200; i++)
            {
                Vector2 speed = new Vector2(0, Main.rand.Next(0, 15)).RotatedByRandom(MathHelper.TwoPi);
                Dust d = Dust.NewDustDirect(npc.Center, 0, 0, DustID.SnowflakeIce, speed.X, speed.Y, Scale:1.5f);
                d.noGravity = true;
                
            }
        }
    }
}
