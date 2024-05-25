using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PerfsEternity : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<PerforatorHive>();
        public override void SetDefaults()
        {
            if (!WorldSavingSystem.EternityMode) return;
            NPC.lifeMax = (int)(NPC.lifeMax * 1.6f);
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 5000000;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {

            legpos = NPC.Center;
            prevlegpos = NPC.Center;
            legpos2 = NPC.Center;
            prevlegpos2 = NPC.Center;

            legpos3 = NPC.Center;
            prevlegpos3 = NPC.Center;
            legpos4 = NPC.Center;
            prevlegpos4 = NPC.Center;
        }
        Vector2 legpos = Vector2.Zero;
        Vector2 prevlegpos = Vector2.Zero;
        Vector2 legpos2 = Vector2.Zero;
        Vector2 prevlegpos2 = Vector2.Zero;

        Vector2 legpos3 = Vector2.Zero;
        Vector2 prevlegpos3 = Vector2.Zero;
        Vector2 legpos4 = Vector2.Zero;
        Vector2 prevlegpos4 = Vector2.Zero;

        int legtime = 0;
        int legtime2 = 15;
        float legprog = 0;
        float legprog2 = 0;

        int legtime3 = 30;
        int legtime4 = 45;
        float legprog3 = 0;
        float legprog4 = 0;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            Asset<Texture2D> leg = TextureAssets.Chains[0];
            spriteBatch.Draw(leg.Value, NPC.Center - Main.screenPosition, null, drawColor, MathHelper.ToRadians(120), new Vector2(leg.Width() / 2, 0), new Vector2(1, 10), SpriteEffects.None, 0);
            spriteBatch.Draw(leg.Value, NPC.Center - Main.screenPosition, null, drawColor, MathHelper.ToRadians(-120), new Vector2(leg.Width() / 2, 0), new Vector2(1, 10), SpriteEffects.None, 0);
            spriteBatch.Draw(leg.Value, NPC.Center - Main.screenPosition, null, new Color(drawColor.R - 120, drawColor.G - 100, drawColor.B - 100), MathHelper.ToRadians(110), new Vector2(leg.Width() / 2, 0), new Vector2(1, 10), SpriteEffects.None, 0);
            spriteBatch.Draw(leg.Value, NPC.Center - Main.screenPosition, null, new Color(drawColor.R - 120, drawColor.G - 100, drawColor.B - 100), MathHelper.ToRadians(-110), new Vector2(leg.Width() / 2, 0), new Vector2(1, 10), SpriteEffects.None, 0);

            Vector2 start3 = NPC.Center + new Vector2(260, -90);
            Vector2 current3 = Vector2.Lerp(prevlegpos3, legpos3, 1 - (float)Math.Cos((legprog3 * Math.PI) / 2));
            float prog3 = Math.Abs(legprog3 - 0.5f) * 2;
            if (prevlegpos3.Distance(legpos3) < 100) prog3 = 1;
            spriteBatch.Draw(leg.Value, start3 - Main.screenPosition, null, new Color(drawColor.R - 120, drawColor.G - 100, drawColor.B - 100),
                (start3).AngleTo(current3) - MathHelper.PiOver2,
                new Vector2(leg.Width() / 2, 0), new Vector2(1, MathHelper.Lerp(start3.Distance(current3) / 50, start3.Distance(current3) / 28, prog3)), SpriteEffects.None, 0);

            Vector2 start4 = NPC.Center + new Vector2(-260, -90);
            Vector2 current4 = Vector2.Lerp(prevlegpos4, legpos4, 1 - (float)Math.Cos((legprog4 * Math.PI) / 2));
            float prog4 = Math.Abs(legprog4 - 0.5f) * 2;
            if (prevlegpos4.Distance(legpos4) < 100) prog4 = 1;
            spriteBatch.Draw(leg.Value, start4 - Main.screenPosition, null, new Color(drawColor.R - 120, drawColor.G - 100, drawColor.B - 100),
                (start4).AngleTo(current4) - MathHelper.PiOver2,
                new Vector2(leg.Width() / 2, 0), new Vector2(1, MathHelper.Lerp(start4.Distance(current4) / 50, start4.Distance(current4) / 28, prog4)), SpriteEffects.None, 0);

            Vector2 start = NPC.Center + new Vector2(240, -140);
            Vector2 current = Vector2.Lerp(prevlegpos, legpos, 1 - (float)Math.Cos((legprog * Math.PI) / 2));
            float prog = Math.Abs(legprog - 0.5f) * 2;
            if (prevlegpos.Distance(legpos) < 100) prog = 1;
            spriteBatch.Draw(leg.Value, start - Main.screenPosition, null, drawColor,
                (start).AngleTo(current) - MathHelper.PiOver2,
                new Vector2(leg.Width() / 2, 0), new Vector2(1, MathHelper.Lerp(start.Distance(current) / 50, start.Distance(current) / 28, prog)), SpriteEffects.None, 0);

            Vector2 start2 = NPC.Center + new Vector2(-240, -140);
            Vector2 current2 = Vector2.Lerp(prevlegpos2, legpos2, 1 - (float)Math.Cos((legprog2 * Math.PI) / 2));
            float prog2 = Math.Abs(legprog2 - 0.5f) * 2;
            if (prevlegpos2.Distance(legpos2) < 100) prog2 = 1;
            spriteBatch.Draw(leg.Value, start2 - Main.screenPosition, null, drawColor,
                (start2).AngleTo(current2) - MathHelper.PiOver2,
                new Vector2(leg.Width() / 2, 0), new Vector2(1, MathHelper.Lerp(start2.Distance(current2) / 50, start2.Distance(current2) / 28, prog2)), SpriteEffects.None, 0);



            return true;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return HitPlayer ? base.CanHitPlayer(target, ref cooldownSlot) : false;
        }
        public int lastAttack = 0;
        public int[] wormCycle = [5, 5, 6, 5, 7];
        public int attackCounter = -2;
        public bool HitPlayer = true;
        public Vector2 LockVector1 = Vector2.Zero;


        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            for (int i = 0; i < wormCycle.Length; i++)
            {
                binaryWriter.Write7BitEncodedInt(wormCycle[i]);
            }
            binaryWriter.Write7BitEncodedInt(lastAttack);
            binaryWriter.Write7BitEncodedInt(attackCounter);
            binaryWriter.Write(HitPlayer);
            binaryWriter.WriteVector2(LockVector1);
            binaryWriter.Write7BitEncodedInt(despawnTimer);
            binaryWriter.Write7BitEncodedInt(lineOfSiteTimer);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            for (int i = 0; i < wormCycle.Length; i++)
            {
                wormCycle[i] = binaryReader.Read7BitEncodedInt();
            }
            lastAttack = binaryReader.Read7BitEncodedInt();
            attackCounter = binaryReader.Read7BitEncodedInt();
            HitPlayer = binaryReader.ReadBoolean();
            LockVector1 = binaryReader.ReadVector2();
            despawnTimer = binaryReader.Read7BitEncodedInt();
            lineOfSiteTimer = binaryReader.Read7BitEncodedInt();
        }
        public int despawnTimer;
        public int lineOfSiteTimer;
        public override bool PreAI()
        {
            if (!WorldSavingSystem.EternityMode) return true;

            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
                NetSync(NPC);
            }
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.velocity.Y += 1;
                return false;
            }
            Player target = Main.player[NPC.target];
            Vector2 toTarget = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);

            //low ground
            if (Main.LocalPlayer.active && !Main.LocalPlayer.ghost && !Main.LocalPlayer.dead && NPC.Distance(Main.LocalPlayer.Center) < 2000)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<LowGroundBuff>(), 2);

            //NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center + new Vector2(0, -300)).SafeNormalize(Vector2.Zero) * 10, 0.03f);
            //Movement

            //Dust.NewDustPerfect(legpos, DustID.TerraBlade);
            //Right leg movement
            legtime++;
            if (legtime > 60)
            {
                prevlegpos = legpos;
                legpos = new Vector2(NPC.Center.X + 300, FindGround((int)NPC.Center.X + 300, (int)NPC.Center.Y));
                legprog = 0;
                legtime = 0;
            }
            if (legprog < 1)
                legprog += 0.05f;
            legtime2++;
            //Left leg movement
            if (legtime2 > 60)
            {
                prevlegpos2 = legpos2;
                legpos2 = new Vector2(NPC.Center.X + -300, FindGround((int)NPC.Center.X + -300, (int)NPC.Center.Y));
                legprog2 = 0;
                legtime2 = 0;
            }
            if (legprog2 < 1)
                legprog2 += 0.05f;

            //Right leg movement
            legtime3++;
            if (legtime3 > 60)
            {
                prevlegpos3 = legpos3;
                legpos3 = new Vector2(NPC.Center.X + 400, FindGround((int)NPC.Center.X + 400, (int)NPC.Center.Y));
                legprog3 = 0;
                legtime3 = 0;
            }
            if (legprog3 < 1)
                legprog3 += 0.05f;
            legtime4++;
            //Left leg movement
            if (legtime4 > 60)
            {
                prevlegpos4 = legpos4;
                legpos4 = new Vector2(NPC.Center.X + -400, FindGround((int)NPC.Center.X + -400, (int)NPC.Center.Y));
                legprog4 = 0;
                legtime4 = 0;
            }
            if (legprog4 < 1)
                legprog4 += 0.05f;
            if (NPC.ai[0] == 0)
            {
                Movement();
                PassiveRain();
                NPC.ai[1]++;
                int time = 150;
                if (NPC.GetLifePercent() <= 0.6f) time = 100;
                if (NPC.ai[1] >= time)
                {
                    NPC.ai[0] = ChooseAttack();
                    NPC.ai[1] = 0;
                    NetSync(NPC);
                }
            }
            if (!target.ZoneCrimson && !BossRushEvent.BossRushActive)
            {
                despawnTimer++;
                if (despawnTimer > 600)
                {
                    NPC.velocity.Y += 0.1f;
                    NPC.EncourageDespawn(30);
                    return false;
                }
            }
            else
            {
                despawnTimer = 0;
            }
            NPC.rotation = MathHelper.ToRadians(NPC.velocity.X * 2);
            if (NPC.ai[0] == 1)
            {
                //Balls();
                Spikes();
                //Movement();
            }
            if (NPC.ai[0] == 2)
            {
                Slam();
            }
            if (NPC.ai[0] == 3)
            {

                //Slam();
                Ichor();
                Movement();

            }
            if (NPC.ai[0] == 4)
            {
                Balls();
            }
            if (NPC.ai[0] == 5)
            {
                SmallWorm();
            }
            if (NPC.ai[0] == 6)
            {
                MediumWorm();
            }
            if (NPC.ai[0] == 7)
            {
                LargeWorm();
            }
            CalamityGlobalNPC.perfHive = NPC.whoAmI;
            int ChooseAttack()
            {
                attackCounter++;
                if (attackCounter % 3 == 0)
                {
                    int theyworm = wormCycle[attackCounter / 3];
                    if (attackCounter >= 12)
                    {
                        attackCounter = 0;
                    }
                    if (theyworm > 5 && NPC.GetLifePercent() > 0.7f) theyworm = 5;
                    if (theyworm > 6 && NPC.GetLifePercent() > 0.4f) theyworm = 5;
                    return theyworm;

                }
                List<int> possibilities = [1];
                if (NPC.GetLifePercent() <= 0.95f)
                {
                    possibilities.Add(2);
                }
                if (NPC.GetLifePercent() <= 0.75f)
                {
                    possibilities.Add(3);
                }
                if (NPC.GetLifePercent() <= 0.5f)
                {
                    possibilities.Add(4);
                }
                int attack = possibilities[Main.rand.Next(0, possibilities.Count)];
                int escape = 0;
                while (attack == lastAttack && escape < 10)
                {
                    escape++;
                    attack = possibilities[Main.rand.Next(0, possibilities.Count)];
                }
                lastAttack = attack;
                NetSync(NPC);
                return attack;

            }
            void Movement()
            {
                if (!Collision.CanHitLine(NPC.Center, 1, 1, target.Center, 1, 1))
                {
                    if (lineOfSiteTimer < 300)
                        lineOfSiteTimer++;
                    if (lineOfSiteTimer >= 300)
                    {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10, 0.05f);
                        return;
                    }
                }
                else
                {
                    if (lineOfSiteTimer > 0) lineOfSiteTimer -= 3;
                }
                if (target.Center.X > NPC.Center.X)
                {
                    NPC.velocity.X += 0.1f;
                    if (NPC.velocity.X < 0)
                    {
                        NPC.velocity.X += 0.2f;
                    }
                }
                else if (target.Center.X < NPC.Center.X)
                {
                    NPC.velocity.X -= 0.1f;
                    if (NPC.velocity.X > 0)
                    {
                        NPC.velocity.X -= 0.2f;
                    }
                }
                if (Math.Abs(NPC.Center.Y - FindGround((int)NPC.Center.X, (int)NPC.Center.Y)) > 350)
                {
                    NPC.velocity.Y += 0.2f;
                    if (NPC.velocity.Y < 0)
                    {
                        NPC.velocity.Y += 0.2f;
                    }
                }
                else
                {
                    NPC.velocity.Y -= 0.2f;
                    if (NPC.velocity.Y > 0)
                    {
                        NPC.velocity.Y -= 0.2f;
                    }

                }
                if (Math.Abs(NPC.Center.Y - FindGround((int)NPC.Center.X, (int)NPC.Center.Y)) < 75 && NPC.velocity.Y > 0)
                {
                    NPC.velocity.Y = 0;
                }
                else if (Math.Abs(NPC.Center.Y - FindGround((int)NPC.Center.X, (int)NPC.Center.Y)) > 500 && NPC.velocity.Y < 0)
                {
                    NPC.velocity.Y = 0;
                }
            }
            void PassiveRain()
            {
                ref float timer = ref NPC.ai[1];
                if (timer % 25 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item17 with { MaxInstances = 10 }, NPC.Center);
                    if (DLCUtils.HostCheck)
                    {
                        float shotSpeed = 6f;
                        Vector2 shotVel = -Vector2.UnitY.RotatedByRandom(MathHelper.Pi / 3.5f);
                        int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + shotVel * NPC.width / 2f, shotVel * shotSpeed, ModContent.ProjectileType<IchorShot>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        /*
                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].extraUpdates = 1;
                            Main.projectile[p].netUpdate = true;
                        }
                        */
                    }

                }
            }
            void LargeWorm()
            {
                if (NPC.AnyNPCs(ModContent.NPCType<LargePerforatorHead>()))
                {
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    ChooseAttack();

                }
                else
                {
                    if (DLCUtils.HostCheck)
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LargePerforatorHead>());
                    SoundEngine.PlaySound(SoundID.NPCDeath23, NPC.Center);
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    NetSync(NPC);
                }
            }
            void MediumWorm()
            {
                if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadMedium>()))
                {
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    ChooseAttack();

                }
                else
                {
                    if (DLCUtils.HostCheck)
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorHeadMedium>());
                    SoundEngine.PlaySound(SoundID.NPCDeath23, NPC.Center);
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    NetSync(NPC);
                }
            }
            void SmallWorm()
            {
                Movement();
                NPC.ai[1]++;
                int time = 30;
                if (NPC.GetLifePercent() <= 0.7f) time += 30;
                if (NPC.GetLifePercent() <= 0.25f) time += 30;
                if (NPC.ai[1] % 30 == 0)
                {
                    if (DLCUtils.HostCheck)
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorHeadSmall>());
                }
                if (NPC.ai[1] >= time)
                {
                    NPC.ai[1] = 0;
                    NPC.ai[0] = 0;
                    NetSync(NPC);
                }
            }
            void Balls()
            {

                NPC.velocity *= 0.95f;
                NPC.ai[1] += 1;
                if (NPC.ai[2] == 0 && NPC.ai[1] >= 30 && NPC.ai[1] <= 90)
                {
                    int i = (int)NPC.ai[1] - 60;
                    if (NPC.ai[1] % 3f == 0)
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit20, NPC.Center);
                        if (DLCUtils.HostCheck)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -100).RotatedBy(MathHelper.ToRadians(i * 3)), new Vector2(0, -8).RotatedBy(MathHelper.ToRadians(i * 3)), ModContent.ProjectileType<ToothBall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    }

                }
                if (NPC.ai[1] > 90)
                {
                    NPC.velocity.Y = 10;
                    NPC.velocity.X = 0;
                }
                if (NPC.ai[1] >= 120 || WorldGen.SolidTile(NPC.Center.ToTileCoordinates()))
                {
                    NPC.ai[1] = 0;
                    NPC.ai[0] = 0;
                    NPC.ai[2] = 0;
                    NetSync(NPC);
                }

            }
            void Ichor()
            {
                ref float timer = ref NPC.ai[1];
                NPC.velocity /= 1.05f;
                if (NPC.ai[2] == 0 && timer == 0)
                {
                    timer = 10;
                }
                if (NPC.ai[2] % 2 == 0)
                {
                    timer += 1;
                    if (timer >= 20)
                    {
                        NPC.ai[2]++;
                    }
                }
                if (NPC.ai[2] % 2 != 0)
                {
                    timer -= 1;
                    if (timer <= 0)
                    {
                        NPC.ai[2]++;
                    }
                }
                if (NPC.ai[2] > 3 && timer % 12 == 0)
                {
                    Vector2 off = new Vector2(0, -70).RotatedBy(Main.rand.NextFloat(-0.9f, 0.9f));
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + off, (NPC.Center + off).AngleFrom(NPC.Center).ToRotationVector2() * 10, ModContent.ProjectileType<IchorBlob>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);

                }
                if (NPC.ai[2] > 3 && timer % 2 == 0)
                {
                    int shotSide = Main.rand.NextBool() ? 1 : -1;
                    float shotSpeed = 10f;
                    Vector2 shotVel = -Vector2.UnitY.RotatedBy(shotSide * MathHelper.Pi / 3f).RotatedByRandom(MathHelper.Pi / 8f);
                    if (DLCUtils.HostCheck)
                    {
                        int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + shotVel * NPC.width / 2f, shotVel * shotSpeed, ModContent.ProjectileType<IchorShot>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                        if (p != Main.maxProjectiles)
                        {
                            Main.projectile[p].extraUpdates = 1;
                            Main.projectile[p].netUpdate = true;
                        }
                    }
                }
                float rotationTimer = timer * 0.05f;
                NPC.rotation = Utils.AngleLerp(-0.5f, 0.5f, -(float)(Math.Cos(Math.PI * rotationTimer) - 1) / 2);
                if (NPC.ai[2] >= 8)
                {
                    NPC.ai[2] = 0;
                    NPC.ai[1] = 0;
                    NPC.ai[0] = 0;
                    NetSync(NPC);
                }
            }
            void Spikes()
            {
                if (NPC.ai[1] <= 0)
                {
                    LockVector1 = Vector2.UnitX * Math.Sign(target.Center.X - NPC.Center.X) * 400;
                }
                if (NPC.ai[1] < 60 && NPC.ai[1] % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit20, NPC.Center);
                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustDirect(NPC.Center, 0, 0, DustID.CrimsonPlants, 0, 2, Scale: 1.5f);
                    }

                }
                NPC.ai[1]++;
                if (NPC.ai[1] <= 60)
                {
                    SpikeMovement(target.Center - LockVector1, 2f);
                }
                else
                {
                    SpikeMovement(target.Center + LockVector1, 2.5f);
                }
                if (NPC.ai[1] > 60 && NPC.ai[1] % 5 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item17 with { MaxInstances = 10 }, NPC.Center);
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), ModContent.ProjectileType<BloodGeyser>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                }
                if (NPC.ai[1] > 60)
                {
                    if (Math.Abs(NPC.Center.X - (target.Center.X + LockVector1.X)) < 50)
                    {
                        LockVector1 = -LockVector1;
                    }
                }
                if (NPC.ai[1] >= 180)
                {
                    NPC.ai[1] = 0;
                    NPC.ai[0] = 0;
                    NPC.ai[2] = 0;
                    NetSync(NPC);
                }
            }
            void SpikeMovement(Vector2 targetPos, float Xmodifier = 1f)
            {
                if (targetPos.X > NPC.Center.X)
                {
                    NPC.velocity.X += 0.1f * Xmodifier;
                    if (NPC.velocity.X < 0)
                    {
                        NPC.velocity.X += 0.2f * Xmodifier;
                    }
                }
                else if (targetPos.X < NPC.Center.X)
                {
                    NPC.velocity.X -= 0.1f * Xmodifier;
                    if (NPC.velocity.X > 0)
                    {
                        NPC.velocity.X -= 0.2f * Xmodifier;
                    }
                }
                if (Math.Abs(NPC.Center.Y - FindGround((int)NPC.Center.X, (int)NPC.Center.Y)) > 350)
                {
                    NPC.velocity.Y += 0.2f;
                    if (NPC.velocity.Y < 0)
                    {
                        NPC.velocity.Y += 0.2f;
                    }
                }
                else
                {
                    NPC.velocity.Y -= 0.2f;
                    if (NPC.velocity.Y > 0)
                    {
                        NPC.velocity.Y -= 0.2f;
                    }
                }
            }
            void Slam()
            {
                NPC.ai[1]++;
                if (NPC.ai[1] == 1 && NPC.ai[2] == 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath23, NPC.Center);
                    const int dustCount = 40;
                    for (int i = 0; i < dustCount; i++)
                    {
                        Vector2 vel = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * ((float)i / dustCount));
                        int d = Dust.NewDust(NPC.Center + vel * NPC.width / 2, 0, 0, DustID.CrimsonTorch, vel.X, vel.Y, Scale: 2f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = vel * 10f;
                    }

                }
                if (NPC.ai[1] < 30 && NPC.ai[2] == 0)
                {
                    NPC.velocity /= 1.1f;
                    NPC.Center = Vector2.Lerp(NPC.Center, target.Center + new Vector2(target.velocity.X * 40, -400), 0.04f);
                    HitPlayer = false;
                    NetSync(NPC);
                }
                if (NPC.ai[2] == 0 && NPC.ai[1] > 30)
                {
                    NPC.velocity.Y = 15;
                    NPC.velocity.X = 0;
                    HitPlayer = true;
                }
                if (WorldGen.SolidTile((NPC.Center + new Vector2(0, 20)).ToTileCoordinates()) && NPC.ai[2] == 0)
                {
                    NPC.velocity.Y = 0;
                    NPC.ai[2] = 1;
                    NPC.ai[1] = 0;
                }
                if (NPC.ai[2] == 1)
                {
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.ai[1] >= 30)
                    {
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NetSync(NPC);
                    }
                }
            }
            int FindGround(int x, int y)
            {
                int escape = 0;

                while (escape < 100 && !WorldGen.SolidTile(new Vector2(x, y).ToTileCoordinates()))
                {
                    y += 8;
                    escape++;

                }
                return y;
            }
            return false;
        }
    }
}
