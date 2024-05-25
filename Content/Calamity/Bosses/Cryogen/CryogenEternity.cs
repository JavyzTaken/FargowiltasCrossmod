
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common;
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
    public class CryogenEternity : CalDLCEmodeBehavior
    {
        public const bool Enabled = true;
        public override bool IsLoadingEnabled(Mod mod) => Enabled;

        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.Cryogen.Cryogen>();
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> shield = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/CryogenShield");
            float x = shieldDrawTimer / 200f;
            x = (float)-(Math.Cos(Math.PI * x) - 1) / 2;
            float scaleAdd = MathHelper.Lerp(-0.2f, 0.2f, x);

            if (NPC.ai[0] != 3)
            {
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                    Color glowColor = Color.Blue with { A = 0 } * 0.7f;


                    spriteBatch.Draw(shield.Value, NPC.Center + afterimageOffset - screenPos, null, glowColor, 0, shield.Size() / 2, NPC.scale + scaleAdd, SpriteEffects.None, 0);
                }
                spriteBatch.Draw(shield.Value, NPC.Center - screenPos, null, drawColor, 0, shield.Size() / 2, NPC.scale + scaleAdd, SpriteEffects.None, 0);
            }
            if (NPC.ai[0] == 3)
            {
                Asset<Texture2D> cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase1");
                if (NPC.ai[2] == 1) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase2");
                if (NPC.ai[2] == 2) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase3");
                if (NPC.ai[2] == 3) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase4");
                if (NPC.ai[2] == 4) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase5");
                if (NPC.ai[2] == 5) cryo1 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase6");
                Vector2 offset = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)) * NPC.ai[1] / 120;
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                    Color glowColor = Color.Blue with { A = 0 } * 0.7f;


                    spriteBatch.Draw(cryo1.Value, NPC.Center + afterimageOffset - screenPos + offset, null, glowColor, 0, cryo1.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                }
                spriteBatch.Draw(cryo1.Value, NPC.Center - screenPos + offset, null, drawColor, 0, cryo1.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                return false;
            }
            return true;
        }
        public int[] Chains = [-1, -1, -1, -1 , -1, -1];
        public float shieldDrawTimer;
        public float shieldDrawCounter;

        public int RitualProj;

        public const int ArenaRadius = 1200;

        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt((int)NPC.localAI[0]);
            binaryWriter.Write7BitEncodedInt((int)NPC.localAI[1]);
            binaryWriter.Write7BitEncodedInt((int)NPC.localAI[2]);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            NPC.localAI[0] = binaryReader.Read7BitEncodedInt();
            NPC.localAI[1] = binaryReader.Read7BitEncodedInt();
            NPC.localAI[2] = binaryReader.Read7BitEncodedInt();
        }
        private enum Attacks
        {
            Idle,
            HomingShards,
            ShardSweep,
            ShardStorm
        };
        private List<Attacks> AttackChoices =
        [
            //Attacks.HomingShards,
            Attacks.ShardSweep,
            Attacks.ShardStorm
        ];

        const int chainTime = IceChain.ActiveTime;
        const int chainStartTime = 62;
        bool evenChain(NPC NPC) => NPC.ai[1] % (chainTime * 2) >= chainTime;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget || !CalDLCConfig.Instance.EternityPriorityOverRev || !CalDLCWorldSavingSystem.EternityRev) return true;

            Player target = Main.player[NPC.target];
            ref float attack = ref NPC.ai[0];
            ref float timer = ref NPC.ai[1];
            ref float data = ref NPC.ai[2];
            ref float data2 = ref NPC.ai[3];

            ref float attackChoice = ref NPC.localAI[0];
            ref float attackTimer = ref NPC.localAI[1];
            ref float data3 = ref NPC.localAI[2];


            NPC.damage = NPC.defDamage;
            NPC.scale = 1.5f;
            NPC.dontTakeDamage = false;
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
            if (NPC.ai[0] == 0 || NPC.ai[0] == 1)
            {
                NPC.ai[0] = 0;
                NPC.rotation = NPC.velocity.X / 15f;
                timer++;
                if (timer == 1)
                {
                    //Main.NewText("j");
                    Vector2 pos = target.Center + new Vector2(0, -400);
                    data = pos.X; data2 = pos.Y;
                    NetSync(NPC);
                }
                else if (NPC.Distance(new Vector2(data, data2)) > 300){
                    Vector2 pos = new Vector2(data, data2);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * 30, 0.05f);
                    
                }
                if (NPC.Distance(new Vector2(data, data2)) < 300)
                {
                    NPC.velocity /= 1.1f;
                }
                    if (NPC.velocity.Length() < 1 && timer > 20)
                {
                    NPC.rotation = 0;
                    data = 0;
                    data2 = 0;
                    timer = 0;
                    attack = 2;
                    NPC.velocity *= 0;
                    NetSync(NPC);
                }
                
            }
            if (NPC.ai[0] == 2)
            {
                
                NPC.velocity = Vector2.Zero;
                timer++;
                if (timer == 2)
                {
                    SoundEngine.PlaySound(CalamityMod.NPCs.Cryogen.Cryogen.ShieldRegenSound, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        Chains[0] = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: NPC.whoAmI, ai1: MathHelper.ToRadians(-149));
                        Chains[1] = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: NPC.whoAmI, ai1: MathHelper.ToRadians(-90));
                        Chains[2] = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: NPC.whoAmI, ai1: MathHelper.ToRadians(-31));
                        Chains[3] = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: NPC.whoAmI, ai1: MathHelper.ToRadians(31));
                        Chains[4] = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: NPC.whoAmI, ai1: MathHelper.ToRadians(90));
                        Chains[5] = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IceChain>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: NPC.whoAmI, ai1: MathHelper.ToRadians(149));

                        RitualProj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CryogenRitual>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, 0f, NPC.whoAmI);
                    }
                    DustExplode(NPC);
                }
                float chainCycleTime = timer % chainTime;
                if (chainCycleTime > chainStartTime - 10 && chainCycleTime < chainStartTime && attackChoice == (float)Attacks.ShardSweep)
                {
                    timer--;
                }
                if (timer % chainTime == chainStartTime)
                {
                    int even = evenChain(NPC) ? 1 : 0;
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
                    ref float attackChoice = ref NPC.localAI[0];
                    ref float attackTimer = ref NPC.localAI[1];
                    attackChoice = (int)Attacks.Idle;
                    attackTimer = 0;
                }
                switch ((Attacks)attackChoice)
                {
                    case Attacks.Idle:
                        {
                            if (attackTimer == 120)
                            {
                                SoundEngine.PlaySound(SoundID.Item30, NPC.Center);
                                if (DLCUtils.HostCheck)
                                {
                                    float randSpread = Main.rand.NextFloat(5f, 8f);
                                    float randSpeedMod = Main.rand.NextFloat(65f, 90f);
                                    for (int i = 0; i < 6; i++) //hexagon of bombs
                                    {
                                        Vector2 toPlayer = target.Center - NPC.Center;
                                        Vector2 offset = (Vector2.UnitY * NPC.height / 2f).RotatedBy(MathHelper.TwoPi * i / 6f);
                                        Vector2 vel = (toPlayer).SafeNormalize(Vector2.Zero) * toPlayer.Length() / randSpeedMod;
                                        vel += Vector2.Normalize(offset) * randSpread;
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, vel, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.IceBomb>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                                    }
                                }
                            }

                            attackTimer++;
                            if (attackTimer > 120 + 60)
                            {
                                attackChoice = (int)Main.rand.NextFromCollection(AttackChoices);
                                attackTimer = 0;
                                NetSync(NPC);
                                NPC.netUpdate = true;
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
                                int even = evenChain(NPC) ? 1 : 0;
                                for (int i = 0; i < Chains.Length; i++)
                                {
                                    if (i % 2 != even)
                                        continue;
                                    int p = Chains[i];

                                    if (closestChain == -1)
                                        closestChain = p;
                                    else
                                    {
                                        Vector2 oldRotVec = Main.projectile[closestChain].Center - NPC.Center;
                                        Vector2 newRotVec = Main.projectile[p].Center - NPC.Center;
                                        Vector2 toPlayer = NPC.DirectionTo(target.Center);
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
                                    Vector2 chainDir = NPC.DirectionTo(chain.Center);
                                    float rotDir = Math.Sign(FargoSoulsUtil.RotationDifference(chainDir, NPC.DirectionTo(target.Center)));

                                    Vector2 velDir = chainDir.RotatedBy(rotDir * totalRotation * progress);
                                    float speed = 1;
                                    Vector2 vel = velDir * speed;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<IceRain>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: 1);
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
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 1).RotatedBy(MathHelper.ToRadians(360f / 12 * i)), ModContent.ProjectileType<IceRain>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: 1);
                                    }
                                    for (int i = 0; i < 12; i++)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 3).RotatedBy(MathHelper.ToRadians(360f / 12 * i)), ModContent.ProjectileType<IceRain>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: 1);
                                    }
                                    for (int i = 0; i < 12; i++)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 2).RotatedBy(MathHelper.ToRadians(360f / 12 * i + 12)), ModContent.ProjectileType<IceRain>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: 1);
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
                if (NPC.GetLifePercent() <= 0.8f && !BossRushEvent.BossRushActive)
                {
                    attack = 3;
                    timer = 0;
                    data = -1;
                    data2 = 0;
                }
                
            }
            if (attack == 3) {


                if (CalDLCWorldSavingSystem.PermafrostPhaseSeen && timer % 60 < 55 && timer % 60 > 0)
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
                            Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -5).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), ModCompatibility.Calamity.Mod.Find<ModGore>("CryoDeathGore" + Main.rand.Next(2, 4)).Type, 1.5f);
                        }
                    }
                }
                if (data == 6)
                {
                    DustExplode(NPC);
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenDeath") with { Volume = 2}, target.Center);
                    if (!Main.dedServ && Main.netMode != NetmodeID.Server)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -5).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), ModCompatibility.Calamity.Mod.Find<ModGore>("CryoDeathGore" + Main.rand.Next(2, 4)).Type, 1.5f);
                        }
                    }
                    
                    //Main.musicVolume = 1;
                    if (DLCUtils.HostCheck)
                    {
                        int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 50, ModContent.NPCType<PermafrostBoss>());
                        if (n != Main.maxNPCs)
                        {
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                    CalDLCWorldSavingSystem.PermafrostPhaseSeen = true;
                    NPC.active = false;
                }
                timer++;
            }

            return false;
        }
        public void DustExplode(NPC NPC)
        {
            for (int i = 0; i < 200; i++)
            {
                Vector2 speed = new Vector2(0, Main.rand.Next(0, 15)).RotatedByRandom(MathHelper.TwoPi);
                Dust d = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.SnowflakeIce, speed.X, speed.Y, Scale:1.5f);
                d.noGravity = true;
                
            }
        }
    }
}
