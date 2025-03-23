using System;
using System.IO;
using System.Linq;
using CalamityMod.Events;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.Projectiles.Ranged;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Assets.Sounds;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
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
            attackCycle = [0, 1, 1, 2, 4, 1];

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
            Asset<Texture2D> dizzy = ModContent.Request<Texture2D>("FargowiltasSouls/Content/PlayerDrawLayers/DizzyStars");
            //manually draw the frame during the spin because crabulon is not centered by default
            if (attackCycle[(int)ai_attackCycleIndex] == 6)
            {
                float addRotation = 0;
                if (NPC.ai[2] < 1000 && NPC.ai[2] > 60)
                {
                    addRotation = MathF.Sin(NPC.ai[2] / 5) / 5;
                }
                else
                {
                    NPC.frame.Y = 0;
                }

                    spriteBatch.Draw(attack.Value, NPC.Center - Main.screenPosition, new Rectangle(NPC.frame.X, NPC.frame.Y, NPC.frame.Width, NPC.frame.Height), drawColor, NPC.rotation + addRotation, new Vector2(attack.Width() / 2, (NPC.height / 2)), NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(attackGlow.Value, NPC.Center - Main.screenPosition, new Rectangle(NPC.frame.X, NPC.frame.Y, NPC.frame.Width, NPC.frame.Height), new Color(255, 255, 255), NPC.rotation + addRotation, new Vector2(attack.Width() / 2, (NPC.height / 2)), NPC.scale, SpriteEffects.None, 0);
                int dizzyFrameHeight = dizzy.Height() / 6;
                if (NPC.ai[2] > 1000 && NPC.ai[2] < 1060)
                {
                    spriteBatch.Draw(dizzy.Value, NPC.Center - Main.screenPosition + new Vector2(0, -100).RotatedBy(NPC.rotation), new Rectangle(0, (int)(dizzyFrameHeight * (int)(NPC.localAI[0] / 5)), dizzy.Width(), dizzyFrameHeight), Color.White, NPC.rotation, new Vector2(dizzy.Width(), dizzy.Height() / 6) / 2, NPC.scale*2, SpriteEffects.None, 0);
                }
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
            binaryWriter.Write(NPC.localAI[0]);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            for (int i = 0; i < attackCycle.Length; i++)
            {
                attackCycle[i] = binaryReader.Read7BitEncodedInt();
            }
            enrageJumpTimer = binaryReader.Read7BitEncodedInt();
            enrageJumping = binaryReader.ReadBoolean();
            NPC.localAI[0] = binaryReader.ReadSingle();
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
                NPC.EncourageDespawn(30);
                return false;
            }

            //phase changing
            if ((ai_Phase == 0 && NPC.GetLifePercent() < 0.6f) || (ai_Phase == 1 && NPC.GetLifePercent() < 0.3f))
            {
                ai_Phase++;
                ai_attackCycleIndex = 0;
                ai_Timer = 0;
                NetSync(NPC);
            }

            //setting attack cycles
            //Attack phase 1
            if (ai_Phase == 0)
            {
                attackCycle = [0, 1, 1, 2, 4, 1];
            }
            //attack phase 2
            if (ai_Phase == 1)
            {
                //attackCycle = [6, 1, 6, 1, 6, 1, 6, 1, 6];
                attackCycle = [1, 6, 2, 1, 4, 5, 2, 3, 1];
            }
            //attack phase 3
            if (ai_Phase == 2)
            {
                attackCycle = [1, 6, 1, 4, 3, 1, 1, 4, 2, 3];
            }
            Player target = Main.player[NPC.target];
            
            //Making sure crabulon isnt stuck in blocks/above the player. does not apply the same during all attacks so StayLevel is called inside those attacks when necessary instead
            if (attackCycle[(int)ai_attackCycleIndex] != 2 && attackCycle[(int)ai_attackCycleIndex] != 3 && attackCycle[(int)ai_attackCycleIndex] != 4 && attackCycle[(int)ai_attackCycleIndex] != 6)
            {
                StayLevel(0);
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
                Jump(0);

                return false;
            }
            switch (attackCycle[(int)ai_attackCycleIndex])
            {
                case 0: //walk towards the player
                    Walk(0);
                    break;
                case 1://slow down quickly, then charge until hits edge of screen or wall and has passed the player
                    Charge();
                    break;
                case 2://jump with dust and sound when landing
                    Jump(0);
                    break;
                case 3: //jump with spears on landing
                    Jump(1);
                    break;
                case 4://Jump with spore clouds on lander
                    Jump(2);
                    break;
                case 5://walk towards player and spawn crab shrooms as it walks
                    Walk(1);
                    //ai_attackCycleIndex = 0;
                    break;
                case 6:
                    RollingSpikeBall();
                    break;
            }
            return false;
        }
        public void RollingSpikeBall()
        {
            Player target = Main.player[NPC.target];
            ref float ai_Timer = ref NPC.ai[2];
            ai_Timer++;
            NPC.ai[0] = 0;
            NPC.localAI[0]++;
            if (ai_Timer == 1)
            {
                for (int i = 0; i < 10; i++)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<MushroomSpear2>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 1, -1, NPC.whoAmI, MathHelper.ToRadians(360f / 10 * i + 15));
                }
                NPC.velocity.X = 0;
            }
            if (ai_Timer > 30 && ai_Timer < 1000)
            {
                StayLevel(220);
            }
            else
            {
                NPC.frame.Y = 0;
                StayLevel();
            }
            if (ai_Timer > 80 && ai_Timer < 1000)
            {
                if (NPC.velocity.X > 0 || (NPC.Center.X < target.Center.X && NPC.velocity.X >= 0) && NPC.velocity.X < 7)
                {
                    NPC.velocity.X += 0.1f;
                }
                if (NPC.velocity.X < 0 || (NPC.Center.X > target.Center.X && NPC.velocity.X <= 0) && NPC.velocity.X > -7)
                {
                    NPC.velocity.X -= 0.1f;
                }
                NPC.rotation += MathHelper.ToRadians(NPC.velocity.X / 4);
                if (NPC.velocity.X > 0 && NPC.Center.X - target.Center.X > 300 && (Collision.SolidCollision(NPC.position, NPC.width, NPC.height) || NPC.Distance(target.Center) > 1100))
                {
                    ai_Timer = 1000;
                }
                if (NPC.velocity.X < 0 && NPC.Center.X - target.Center.X < -300 && (Collision.SolidCollision(NPC.position, NPC.width, NPC.height) || NPC.Distance(target.Center) > 1100))
                {
                    ai_Timer = 1000;
                }
            }
            if (ai_Timer == 1000)
            {
                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Debuffs/DizzyBird"), NPC.Center);
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p != null && p.active && p.type == ModContent.ProjectileType<MushroomSpear2>() && p.ai[0] == NPC.whoAmI)
                    {
                        p.Kill();
                    }
                }
                NPC.velocity.X = 0;
            }
            if (ai_Timer > 1000 && ai_Timer < 1060)
            {
                NPC.frame.Y = 0;
                StayLevel();
                if (NPC.localAI[0] > 6 * 5)
                {
                    NPC.localAI[0] = 0;
                }
                
            }
            if (ai_Timer >= 1060)
            {
                NPC.frame.Y = 0;
                NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.08f);
                StayLevel();
            }
            if (ai_Timer >= 1080)
            {
                ai_Timer = 0;
                NPC.rotation = 0;
                NPC.localAI[0] = 0;
                IncrementCycle();
            }
        }
        public void StayLevel(int respectOffset = 0)
        {
            Player target = Main.player[NPC.target];
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            bool flag = false;
            if (Collision.SolidCollision(NPC.Bottom - new Vector2(2, 2), 4, 2 + respectOffset) && NPC.Bottom.Y + respectOffset > target.position.Y)
            {
                flag = true;
                if (NPC.velocity.Y > -5)
                    NPC.velocity.Y -= 0.2f;
                if (NPC.velocity.Y > 0) NPC.velocity.Y = 0;
            }
            if (!flag && Collision.SolidCollision(NPC.BottomLeft, NPC.width, 6 + respectOffset) && NPC.Bottom.Y + respectOffset > target.position.Y)
            {
                NPC.velocity.Y = 0;
            }

            if (!flag && (!Collision.SolidCollision(NPC.Bottom, 2, 2 + respectOffset) || NPC.Bottom.Y + respectOffset < target.position.Y))
            {
                if (NPC.velocity.Y < 10)
                    NPC.velocity.Y += 0.2f;
                if (NPC.velocity.Y < 0) NPC.velocity.Y = 0;
            }
        }
        public void Charge()
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
                float speedMod = WorldSavingSystem.MasochistModeReal ? 1.4f : 1f;
                if ((target.Center.X > NPC.Center.X || NPC.velocity.X > 0) && NPC.velocity.X >= 0)
                {
                    NPC.velocity.X += 0.2f * speedMod;
                }
                else if ((target.Center.X < NPC.Center.X || NPC.velocity.X < 0) && NPC.velocity.X <= 0)
                {
                    NPC.velocity.X -= 0.2f * speedMod;
                }
                ai_Animation = 1;

            }
            if (ai_Timer > 60 && ai_Timer % 30 == 0)
            {
                //if (DLCUtils.HostCheck)
                    //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat() / 2, 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<ShroomGas>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
            }
            int freq = WorldSavingSystem.MasochistModeReal ? 12 : 20;
            if (ai_Timer > 30 && ai_Timer % freq == 0) 
            {
                if (DLCUtils.HostCheck)
                {
                    Vector2 position = NPC.Center - Vector2.UnitY * NPC.height / 3;
                    Vector2 vel = new(0, -5);
                    int dir = -Math.Sign(NPC.velocity.X);
                    vel = vel.RotatedBy(dir * MathHelper.Pi / 4f);
                    vel = vel.RotatedByRandom(MathHelper.Pi / 16f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), position, vel, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.MushBomb>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
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
                IncrementCycle();
            }
        }
        public void Walk(int type)
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
                IncrementCycle();
            }

            if (target.Center.X > NPC.Center.X && NPC.velocity.X < 3)
            {

                NPC.velocity.X += 0.05f;
                if (NPC.velocity.X < 0) NPC.velocity.X += 0.1f;
            }
            else if (target.Center.X < NPC.Center.X && NPC.velocity.X > -3)
            {
                NPC.velocity.X -= 0.05f;
                if (NPC.velocity.X > 0) NPC.velocity.X -= 0.1f;
            }
            if (type == 1 && ai_Timer % 60 == 0 && NPC.CountNPCS(ModContent.NPCType<CrabShroom>()) < 3)
            {
                Vector2 position = NPC.TopLeft + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height));
                if (DLCUtils.HostCheck)
                {
                    NPC shroom = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)position.X, (int)position.Y, ModContent.NPCType<CrabShroom>());
                    shroom.velocity = new Vector2(0, -5);
                }
            }
            if (ai_Timer % 50 == 40)
            {
                if (DLCUtils.HostCheck)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center + Vector2.UnitX * target.velocity.X * 20, Vector2.Zero, ModContent.ProjectileType<MushroomSpear>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: 0);
                }
            }
        }
        public void Jump(int type)
        {
            Player target = Main.player[NPC.target];
            ref float ai_JumpDirection = ref NPC.localAI[0];
            ref float ai_Animation = ref NPC.ai[0];
            ref float ai_Timer = ref NPC.ai[2];

            float telegraphTime = WorldSavingSystem.MasochistModeReal ? 20 : 40;

            ai_Timer++;
            ai_Animation = 3;
            if (ai_Timer < telegraphTime)
            {
                if (ai_Timer == 1)
                {
                    float x = 5 * NPC.HorizontalDirectionTo(target.Center);
                    if (x == 0)
                        x = 5;
                    ai_JumpDirection = x;
                    Vector2 direction = new(x, -15);
                    direction.Normalize();
                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction, ModContent.ProjectileType<CrabulonJumpTelegraph>(), 0, 0, Main.myPlayer, ai0: NPC.whoAmI);
                    if (p.IsWithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[p].timeLeft = (int)telegraphTime - 1;
                    }
                    NPC.netUpdate = true;
                    NetSync(NPC);
                }
                NPC.velocity.X *= 0.96f;
                NPC.noTileCollide = false;
            }
            if (ai_Timer == telegraphTime)
            {
                NPC.velocity.Y = -15;
                NPC.noTileCollide = true;

                NPC.velocity.X = ai_JumpDirection;

                NetSync(NPC);
            }
            if (enrageJumping && DLCUtils.HostCheck)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.position + new Vector2(Main.rand.Next(0, NPC.width), Main.rand.Next(0, NPC.height)), Vector2.Zero, ModContent.ProjectileType<ShroomGas>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
            }
            if (Collision.SolidCollision(NPC.BottomLeft, NPC.width, 10) && ai_Timer > telegraphTime + 20 && target.position.Y < NPC.BottomLeft.Y && NPC.velocity.Y >= 0)
            {
                NPC.noTileCollide = false;
                ai_Timer = 0;
                enrageJumping = false;
                enrageJumpTimer = 0;
                IncrementCycle();
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
                else if (type == 2)
                {
                    for (int i = -20; i < 20; i++)
                    {
                        if (DLCUtils.HostCheck && (i < -1 || i > 1))
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.BottomLeft + new Vector2(NPC.width / 40 * (i + 20), -5), new Vector2(i * Main.rand.NextFloat(0.5f, 0.7f), Main.rand.NextFloat()/5), ModContent.ProjectileType<ShroomGas>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    }
                }
                else
                {
                    if (WorldSavingSystem.MasochistModeReal)
                    {
                        if (DLCUtils.HostCheck)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(150, 0), Vector2.Zero, ModContent.ProjectileType<MushroomSpear>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: 5);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-150, 0), Vector2.Zero, ModContent.ProjectileType<MushroomSpear>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: -5);
                        }
                    }
                }
            }
        }
        public void IncrementCycle()
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
