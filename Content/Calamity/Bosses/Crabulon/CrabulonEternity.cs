using System;
using System.IO;
using CalamityMod.Events;
using CalamityMod.NPCs.Crabulon;
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
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Crabulon
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CrabulonEternity : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.Crabulon.Crabulon>();

        public override void SetDefaults()
        {
            NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 1.2f);
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 5000000;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (!WorldSavingSystem.EternityMode) return;
            attackCycle = [0, 1, 1, 2];

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode) return true;

            ref float ai_attackCycleIndex = ref NPC.ai[1];

            Asset<Texture2D> idle = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/Crabulon");
            Asset<Texture2D> walk = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonAlt");
            Asset<Texture2D> attack = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonAttack");

            Asset<Texture2D> idleGlow = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonGlow");
            Asset<Texture2D> walkGlow = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonAltGlow");
            Asset<Texture2D> attackGlow = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonAttackGlow");
            //make him put his arm up when "guarding" while fungal clump is alive
            if (attackCycle[(int)ai_attackCycleIndex] == -1)
            {
                spriteBatch.Draw(attack.Value, NPC.Center - Main.screenPosition, new Rectangle(NPC.frame.X, NPC.frame.Y, NPC.frame.Width, NPC.frame.Height), drawColor, NPC.rotation, new Vector2(attack.Width() / 2, (NPC.height / 2)), NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(attackGlow.Value, NPC.Center - Main.screenPosition, new Rectangle(NPC.frame.X, NPC.frame.Y, NPC.frame.Width, NPC.frame.Height), new Color(255, 255, 255), NPC.rotation, new Vector2(attack.Width() / 2, (NPC.height / 2)), NPC.scale, SpriteEffects.None, 0);
                return false;
            }
            return true;

        }

        public override void FindFrame(int frameHeight)
        {
            if (!WorldSavingSystem.EternityMode) return;

            ref float ai_attackCycleIndex = ref NPC.ai[1];
            //NPC.frameCounter++;
            if (attackCycle[(int)ai_attackCycleIndex] == -1)
            {
                NPC.frame.Y = 1 * NPC.frame.Height;
            }
        }
        public int[] attackCycle = new int[10];


        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            for (int i = 0; i < attackCycle.Length; i++)
            {
                binaryWriter.Write7BitEncodedInt(attackCycle[i]);
            }
            binaryWriter.Write7BitEncodedInt(enrageJumpTimer);
            binaryWriter.Write(enrageJumping);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            for (int i = 0; i < attackCycle.Length; i++)
            {
                attackCycle[i] = binaryReader.Read7BitEncodedInt();
            }
            enrageJumpTimer = binaryReader.Read7BitEncodedInt();
            enrageJumping = binaryReader.ReadBoolean();
        }

        
        public int enrageJumpTimer;
        public bool enrageJumping;
        //ai[] usage:
        //ai[0]: which animation in use (1: walking, 0: idle, 3: attacking)
        //ai[1]: index of attackCycle to read
        //ai[2]: timer
        //ai[3]: phase
        public override bool PreAI()
        {
            if (!WorldSavingSystem.EternityMode) return true;

            ref float ai_Animation = ref NPC.ai[0];
            ref float ai_attackCycleIndex = ref NPC.ai[1];
            ref float ai_Timer = ref NPC.ai[2];
            ref float ai_Phase = ref NPC.ai[3];

            //low ground
            if (Main.LocalPlayer.active && !Main.LocalPlayer.ghost && !Main.LocalPlayer.dead && NPC.Distance(Main.LocalPlayer.Center) < 2000)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<LowGroundBuff>(), 2);

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

            
            //Fungal clump phase 1
            if (ai_Phase == 0 && NPC.GetLifePercent() < 0.8f)
            {
                ai_Phase++;
                //if (DLCUtils.HostCheck)
                    //NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FungalClump>(), ai1: NPC.whoAmI);
                ai_attackCycleIndex = 0;
                //NPC.HealEffect(-50);
                attackCycle = [-1, 1];
                ai_Timer = 0;
                NPC.defense = 40;
                NPC.HitSound = SoundID.NPCHit4;
                NetSync(NPC);
            }


            //fungal clump phase 2
            if (ai_Phase == 2 && NPC.GetLifePercent() < 0.5f)
            {
                ai_Phase++;
                if (DLCUtils.HostCheck)
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FungalClump>(), ai1: NPC.whoAmI);
                ai_attackCycleIndex = 0;
                NPC.HealEffect(-50);
                attackCycle = [-1, 1, -1, 2];
                ai_Timer = 0;
                NPC.defense = 40;
                NPC.HitSound = SoundID.NPCHit4;
                NetSync(NPC);
            }
            
            //fungal clump phase
            if (ai_Phase == 4 && NPC.GetLifePercent() < 0.2f)
            {
                ai_Phase++;
                //if (DLCUtils.HostCheck)
                    //NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FungalClump>(), ai1: NPC.whoAmI);
                ai_attackCycleIndex = 0;
                //NPC.HealEffect(-50);
                attackCycle = [-1, 1, -1, 2, 1];
                ai_Timer = 0;
                NPC.defense = 40;
                NPC.HitSound = SoundID.NPCHit4;
                NetSync(NPC);
            }
            //exit fungal clump phases
            if ((ai_Phase == 1 || ai_Phase == 3 || ai_Phase == 5) && !NPC.AnyNPCs(ModContent.NPCType<FungalClump>()))
            {
                ai_Phase++;
                ai_attackCycleIndex = 0;
                ai_Timer = 0;
                NPC.defense = 8;
                NPC.HitSound = SoundID.NPCHit45;
                NetSync(NPC);
            }
            //Attack phase 2
            if (ai_Phase == 2)
            {
                attackCycle = [0, 1, 1, 2, 4, 1];
            }
            //attack phase 3
            if (ai_Phase == 4)
            {
                attackCycle = [0, 2, 1, 4, 5, 2, 3, 1, 1];
            }
            //attack phase 4
            if (ai_Phase == 6)
            {
                attackCycle = [5, 1, 4, 3, 1, 1, 4, 2, 3, 1];
            }
            Player target = Main.player[NPC.target];
            //high defense and stand still for a while (only does when fungal clump is alive)
            if (attackCycle[(int)ai_attackCycleIndex] == -1)
            {

                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 0, 0.05f);
                if (Math.Abs(NPC.velocity.X) < 0.1f)
                {
                    NPC.velocity.X = 0;
                }
                ai_Timer++;
                if (ai_Timer == 300)
                {
                    IncrementCycle(NPC);
                    ai_Timer = 0;
                }
            }
            //Making sure crabulon isnt stuck in blocks/above the player
            if (attackCycle[(int)ai_attackCycleIndex] != 2 && attackCycle[(int)ai_attackCycleIndex] != 3 && attackCycle[(int)ai_attackCycleIndex] != 4)
            {
                StayLevel(NPC);
            }
            //Crabulon falls naturally during the jump attack
            else
            {
                NPC.noGravity = false;
            }
            //despawn on the surface
            if (NPC.Center.Y < Main.worldSurface * 16 && !BossRushEvent.BossRushActive)
            {
                if (NPC.Center.X > Main.player[NPC.target].Center.X) NPC.velocity.X += 0.1f;
                else NPC.velocity.X -= 0.1f;
                NPC.EncourageDespawn(30);
                NPC.noTileCollide = true;
                ai_Animation = 1;
                return false;
            }
            if (target.Center.Y < NPC.Top.Y)
            {

                enrageJumpTimer++;
                if (enrageJumpTimer == 300 && !enrageJumping)
                {
                    NPC.velocity = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20;
                    enrageJumping = true;
                }
                //Main.NewText(enrageJumpTimer);
            }
            else if (enrageJumpTimer > 0)
            {
                enrageJumpTimer--;
            }

            if (enrageJumping)
            {
                Jump(NPC, 0);

                return false;
            }
            // jump with dust and sound when landing
            if (attackCycle[(int)ai_attackCycleIndex] == 2)
            {
                Jump(NPC, 0);
            }
            //jump with spears on landing
            if (attackCycle[(int)ai_attackCycleIndex] == 3)
            {
                Jump(NPC, 1);
            }
            //Jump with spore clouds on lander
            if (attackCycle[(int)ai_attackCycleIndex] == 4)
            {
                Jump(NPC, 2);
            }
            //walk towards the player
            if (attackCycle[(int)ai_attackCycleIndex] == 0)
            {
                Walk(NPC, 0);
            }
            //walk towards player and spawn crab shrooms as it walks
            if (attackCycle[(int)ai_attackCycleIndex] == 5)
            {
                Walk(NPC, 1);
            }
            //slow down quickly, then charge until hits edge of screen or wall and has passed the player
            if (attackCycle[(int)ai_attackCycleIndex] == 1)
            {
                Charge(NPC);


            }
            return false;
        }
        public void StayLevel(NPC NPC)
        {
            Player target = Main.player[NPC.target];
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            bool flag = false;
            if (Collision.SolidCollision(NPC.Bottom - new Vector2(2, 2), 4, 2) && NPC.Bottom.Y > target.position.Y)
            {
                flag = true;
                if (NPC.velocity.Y > -5)
                    NPC.velocity.Y -= 0.2f;
            }
            if (!flag && Collision.SolidCollision(NPC.BottomLeft, NPC.width, 6) && NPC.Bottom.Y > target.position.Y)
            {
                NPC.velocity.Y = 0;
            }

            if (!flag && (!Collision.SolidCollision(NPC.Bottom, 2, 2) || NPC.Bottom.Y < target.position.Y))
            {
                if (NPC.velocity.Y < 10)
                    NPC.velocity.Y += 0.2f;
            }
        }
        public void Charge(NPC NPC)
        {
            Player target = Main.player[NPC.target];

            ref float ai_Animation = ref NPC.ai[0];
            ref float ai_Timer = ref NPC.ai[2];

            ai_Timer++;
            if (ai_Timer == 1)
            {
                SoundEngine.PlaySound(SoundID.NPCHit45 with { Pitch = -0.75f });
                for (int i = 0; i < 10; i++)
                {

                    Vector2 toplayer = (target.Center - NPC.Center);
                    toplayer.Y = 0;
                    toplayer = toplayer.SafeNormalize(Vector2.Zero);
                    toplayer = toplayer.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-20, 20))) * Main.rand.Next(3, 10);
                    Dust dust = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.MushroomSpray, (int)toplayer.X, (int)toplayer.Y, Scale: 2, Alpha: 120);
                    dust.velocity = toplayer;
                    dust.noGravity = false;
                    NetSync(NPC);
                }
            }
            if (ai_Timer < 60)
            {
                NPC.velocity.X /= 1.03f;
                ai_Animation = 0;
            }
            if (ai_Timer == 60) NPC.velocity.X = 0;
            if (ai_Timer > 60 && ai_Timer < 120)
            {
                if ((target.Center.X > NPC.Center.X || NPC.velocity.X > 0) && NPC.velocity.X >= 0)
                {
                    NPC.velocity.X += 0.2f;
                }
                else if ((target.Center.X < NPC.Center.X || NPC.velocity.X < 0) && NPC.velocity.X <= 0)
                {
                    NPC.velocity.X -= 0.2f;
                }
                ai_Animation = 1;

            }
            if (ai_Timer > 60 && ai_Timer % 30 == 0)
            {
                if (DLCUtils.HostCheck)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat() / 2, 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<ShroomGas>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
            }
            if (ai_Timer > 30 && ai_Timer % 40 == 0 && NPC.CountNPCS(ModContent.NPCType<CrabShroom>()) < 8) //maximum of 6 so you don't enter the CBT shroom dungeon
            {
                if (DLCUtils.HostCheck)
                {
                    Vector2 position = NPC.Center - Vector2.UnitY * NPC.height / 3;
                    NPC shroom = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)position.X, (int)position.Y, ModContent.NPCType<CrabShroom>());
                    shroom.velocity = new Vector2(0, -5);
                    int dir = -Math.Sign(NPC.velocity.X);
                    shroom.velocity = shroom.velocity.RotatedBy(dir * MathHelper.Pi / 4f);
                    shroom.velocity = shroom.velocity.RotatedByRandom(MathHelper.Pi / 16f);
                }
                SoundEngine.PlaySound(SoundID.Item42, NPC.Center); //mourning wood pew sound
            }
            if (((Collision.SolidTiles(NPC.TopLeft, -6, NPC.height - 10) || Collision.SolidTiles(NPC.TopRight, 6, NPC.height - 10) || Math.Abs(NPC.Center.X - target.Center.X) > 900) && ((NPC.velocity.X > 0 && NPC.Center.X > target.Center.X) || (NPC.velocity.X < 0 && NPC.Center.X < target.Center.X))) || (NPC.velocity.X == 0 && ai_Timer > 140) && ai_Timer > 140)
            {
                if (NPC.velocity.X < 0)
                {
                    for (int i = 0; i < 30; i++)
                        Dust.NewDustDirect(NPC.TopLeft, 6, NPC.height, DustID.MushroomSpray, Alpha: 200, Scale: 2).noGravity = true;
                    if (DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(7, 0), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.MushBomb>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(7, 0).RotatedBy(MathHelper.ToRadians(-30)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.MushBomb>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
                    }
                }
                else if (NPC.velocity.X > 0)
                {
                    for (int i = 0; i < 30; i++)
                        Dust.NewDustDirect(NPC.TopRight, -6, NPC.height, DustID.MushroomSpray, Alpha: 200, Scale: 2).noGravity = true;
                    if (DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-7, 0), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.MushBomb>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-7, 0).RotatedBy(MathHelper.ToRadians(30)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.MushBomb>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
                    }
                }

                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                NPC.velocity.X = 0;
                ai_Timer = 0;
                IncrementCycle(NPC);
            }
        }
        public void Walk(NPC NPC, int type)
        {
            Player target = Main.player[NPC.target];
            ref float ai_Animation = ref NPC.ai[0];
            ref float ai_Timer = ref NPC.ai[2];

            ai_Animation = 1;
            if (Collision.SolidCollision(NPC.BottomLeft, NPC.width, 6) && NPC.collideY)
            {
                ai_Animation = 1;
            }
            ai_Timer++;
            if (ai_Timer == 240)
            {
                ai_Timer = 0;
                IncrementCycle(NPC);
            }

            if (target.Center.X > NPC.Center.X && NPC.velocity.X < 5)
            {

                NPC.velocity.X += 0.1f;
                if (NPC.velocity.X < 0) NPC.velocity.X += 0.2f;
            }
            else if (target.Center.X < NPC.Center.X && NPC.velocity.X > -5)
            {
                NPC.velocity.X -= 0.1f;
                if (NPC.velocity.X > 0) NPC.velocity.X -= 0.2f;
            }
            if (type == 1 && ai_Timer % 30 == 0)
            {
                Vector2 position = NPC.TopLeft + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height));
                if (DLCUtils.HostCheck)
                {
                    NPC shroom = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)position.X, (int)position.Y, ModContent.NPCType<CrabShroom>());
                    shroom.velocity = new Vector2(0, -5);
                }
            }
        }
        public void Jump(NPC NPC, int type)
        {
            Player target = Main.player[NPC.target];
            ref float ai_Animation = ref NPC.ai[0];
            ref float ai_Timer = ref NPC.ai[2];

            ai_Timer++;
            ai_Animation = 3;
            if (ai_Timer == 1)
            {
                NPC.velocity.Y = -15;
                NPC.noTileCollide = true;

                if (NPC.Center.X > target.Center.X)
                {
                    NPC.velocity.X = -5;
                }
                else if (NPC.Center.X < target.Center.X)
                {
                    NPC.velocity.X = 5;
                }
                NetSync(NPC);
            }
            if (enrageJumping && DLCUtils.HostCheck)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height)), Vector2.Zero, ModContent.ProjectileType<ShroomGas>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
            }
            if (Collision.SolidCollision(NPC.BottomLeft, NPC.width, 10) && ai_Timer > 20 && target.position.Y < NPC.BottomLeft.Y && NPC.velocity.Y >= 0)
            {
                NPC.noTileCollide = false;
                ai_Timer = 0;
                enrageJumping = false;
                enrageJumpTimer = 0;
                IncrementCycle(NPC);
                ai_Animation = 0;
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustDirect(NPC.BottomLeft, NPC.width, 6, DustID.MushroomSpray, Alpha: 200, Scale: 2).noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                if (type == 1)
                {
                    if (DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(150, 0), Vector2.Zero, ModContent.ProjectileType<MushroomSpear>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: 10);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-150, 0), Vector2.Zero, ModContent.ProjectileType<MushroomSpear>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: -10);
                    }
                }
                if (type == 2)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (DLCUtils.HostCheck)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.BottomLeft + new Vector2(Main.rand.Next(0, NPC.width), -5), new Vector2(Main.rand.NextFloat(), Main.rand.NextFloat()) / 5, ModContent.ProjectileType<ShroomGas>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    }
                }
            }
        }
        public void IncrementCycle(NPC NPC)
        {
            ref float ai_attackCycleIndex = ref NPC.ai[1];
            ai_attackCycleIndex++;
            if (ai_attackCycleIndex >= attackCycle.Length)
            {
                ai_attackCycleIndex = 0;
            }
            NetSync(NPC);
        }
    }
}
