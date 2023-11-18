using System;
using System.IO;
using CalamityMod.Events;
using CalamityMod.NPCs.Crabulon;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
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
    public class CrabulonEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<CalamityMod.NPCs.Crabulon.Crabulon>());
        public override bool InstancePerEntity => true;

        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
            entity.lifeMax = (int)Math.Round(entity.lifeMax * 1.375f);
            if (BossRushEvent.BossRushActive)
            {
                entity.lifeMax = 5000000;
            }
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (!WorldSavingSystem.EternityMode) return;
            attackCycle = new int[] { 0, 1, 1, 2 };

        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            Asset<Texture2D> idle = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/Crabulon");
            Asset<Texture2D> walk = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonAlt");
            Asset<Texture2D> attack = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonAttack");

            Asset<Texture2D> idleGlow = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonGlow");
            Asset<Texture2D> walkGlow = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonAltGlow");
            Asset<Texture2D> attackGlow = ModContent.Request<Texture2D>("CalamityMod/NPCs/Crabulon/CrabulonAttackGlow");
            //make him put his arm up when "guarding" while fungal clump is alive
            if (attackCycle[(int)npc.ai[1]] == -1)
            {
                spriteBatch.Draw(attack.Value, npc.Center - Main.screenPosition, new Rectangle(npc.frame.X, npc.frame.Y, npc.frame.Width, npc.frame.Height), drawColor, npc.rotation, new Vector2(attack.Width() / 2, (npc.height / 2)), npc.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(attackGlow.Value, npc.Center - Main.screenPosition, new Rectangle(npc.frame.X, npc.frame.Y, npc.frame.Width, npc.frame.Height), new Color(255, 255, 255), npc.rotation, new Vector2(attack.Width() / 2, (npc.height / 2)), npc.scale, SpriteEffects.None, 0);
                return false;
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);

        }

        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (!WorldSavingSystem.EternityMode) return;
            //npc.frameCounter++;
            if (attackCycle[(int)npc.ai[1]] == -1)
            {
                npc.frame.Y = 1 * npc.frame.Height;
            }
        }
        public int[] attackCycle = new int[10];


        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            for (int i = 0; i < attackCycle.Length; i++)
            {
                binaryWriter.Write7BitEncodedInt(attackCycle[i]);
            }
            binaryWriter.Write7BitEncodedInt(enrageJumpTimer);
            binaryWriter.Write(enrageJumping);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            for (int i = 0; i < attackCycle.Length; i++)
            {
                attackCycle[i] = binaryReader.Read7BitEncodedInt();
            }
            enrageJumpTimer = binaryReader.Read7BitEncodedInt();
            enrageJumping = binaryReader.ReadBoolean();
        }

        //ai[] usage:
        //ai[0]: which animation in use (1: walking, 0: idle, 3: attacking)
        //ai[1]: index of attackCycle to read
        //ai[2]: timer
        //ai[3]: phase
        public int enrageJumpTimer;
        public bool enrageJumping;
        public override bool SafePreAI(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return true;

            //low ground
            if (Main.LocalPlayer.active && !Main.LocalPlayer.ghost && !Main.LocalPlayer.dead && npc.Distance(Main.LocalPlayer.Center) < 2000)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<LowGroundBuff>(), 2);

            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
                NetSync(npc);
            }
            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.velocity.Y += 1;
                return false;
            }

            /*
            //Fungal clump phase 1
            if (npc.ai[3] == 0 && npc.GetLifePercent() < 0.8f)
            {
                npc.ai[3]++;
                if (DLCUtils.HostCheck)
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<FungalClump>(), ai1: npc.whoAmI);
                npc.ai[1] = 0;
                npc.HealEffect(-50);
                attackCycle = new int[] { -1, 1 };
                npc.ai[2] = 0;
                npc.defense = 40;
                npc.HitSound = SoundID.NPCHit4;
                NetSync(npc);
            }


            //fungal clump phase 2
            if (npc.ai[3] == 2 && npc.GetLifePercent() < 0.5f)
            {
                npc.ai[3]++;
                if (DLCUtils.HostCheck)
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<FungalClump>(), ai1: npc.whoAmI);
                npc.ai[1] = 0;
                npc.HealEffect(-50);
                attackCycle = new int[] { -1, 1, -1, 2 };
                npc.ai[2] = 0;
                npc.defense = 40;
                npc.HitSound = SoundID.NPCHit4;
                NetSync(npc);
            }
            */
            //fungal clump phase
            if (npc.ai[3] == 4 && npc.GetLifePercent() < 0.5f)
            {
                npc.ai[3]++;
                if (DLCUtils.HostCheck)
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<FungalClump>(), ai1: npc.whoAmI);
                npc.ai[1] = 0;
                npc.HealEffect(-50);
                attackCycle = new int[] { -1, 1, -1, 2, 1 };
                npc.ai[2] = 0;
                npc.defense = 40;
                npc.HitSound = SoundID.NPCHit4;
                NetSync(npc);
            }
            //exit fungal clump phases
            if ((npc.ai[3] == 1 || npc.ai[3] == 3 || npc.ai[3] == 5) && !NPC.AnyNPCs(ModContent.NPCType<FungalClump>()))
            {
                npc.ai[3]++;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.defense = 8;
                npc.HitSound = SoundID.NPCHit45;
                NetSync(npc);
            }
            //Attack phase 2
            if (npc.ai[3] == 2)
            {
                attackCycle = new int[] { 0, 1, 1, 2, 4, 1 };
            }
            //attack phase 3
            if (npc.ai[3] == 4)
            {
                attackCycle = new int[] { 0, 2, 1, 4, 5, 2, 3, 1, 1 };
            }
            //attack phase 4
            if (npc.ai[3] == 6)
            {
                attackCycle = new int[] { 5, 1, 4, 3, 1, 1, 4, 2, 3, 1 };
            }
            Player target = Main.player[npc.target];
            //high defense and stand still for a while (only does when fungal clump is alive)
            if (attackCycle[(int)npc.ai[1]] == -1)
            {

                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, 0, 0.05f);
                if (Math.Abs(npc.velocity.X) < 0.1f)
                {
                    npc.velocity.X = 0;
                }
                npc.ai[2]++;
                if (npc.ai[2] == 300)
                {
                    IncrementCycle(npc);
                    npc.ai[2] = 0;
                }
            }
            //Making sure crabulon isnt stuck in blocks/above the player
            if (attackCycle[(int)npc.ai[1]] != 2 && attackCycle[(int)npc.ai[1]] != 3 && attackCycle[(int)npc.ai[1]] != 4)
            {
                StayLevel(npc);
            }
            //Crabulon falls naturally during the jump attack
            else
            {
                npc.noGravity = false;
            }
            //despawn on the surface
            if (npc.Center.Y < Main.worldSurface * 16 && !BossRushEvent.BossRushActive)
            {
                if (npc.Center.X > Main.player[npc.target].Center.X) npc.velocity.X += 0.1f;
                else npc.velocity.X -= 0.1f;
                npc.EncourageDespawn(30);
                npc.noTileCollide = true;
                npc.ai[0] = 1;
                return false;
            }
            if (target.Center.Y < npc.Top.Y)
            {

                enrageJumpTimer++;
                if (enrageJumpTimer == 300 && !enrageJumping)
                {
                    npc.velocity = (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 20;
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
                Jump(npc, 0);

                return false;
            }
            // jump with dust and sound when landing
            if (attackCycle[(int)npc.ai[1]] == 2)
            {
                Jump(npc, 0);
            }
            //jump with spears on landing
            if (attackCycle[(int)npc.ai[1]] == 3)
            {
                Jump(npc, 1);
            }
            //Jump with spore clouds on lander
            if (attackCycle[(int)npc.ai[1]] == 4)
            {
                Jump(npc, 2);
            }
            //walk towards the player
            if (attackCycle[(int)npc.ai[1]] == 0)
            {
                Walk(npc, 0);
            }
            //walk towards player and spawn crab shrooms as it walks
            if (attackCycle[(int)npc.ai[1]] == 5)
            {
                Walk(npc, 1);
            }
            //slow down quickly, then charge until hits edge of screen or wall and has passed the player
            if (attackCycle[(int)npc.ai[1]] == 1)
            {
                Charge(npc);


            }
            return false;
        }
        public void StayLevel(NPC npc)
        {
            Player target = Main.player[npc.target];
            npc.noTileCollide = true;
            npc.noGravity = true;
            bool flag = false;
            if (Collision.SolidCollision(npc.Bottom - new Vector2(2, 2), 4, 2) && npc.Bottom.Y > target.position.Y)
            {
                flag = true;
                if (npc.velocity.Y > -5)
                    npc.velocity.Y -= 0.2f;
            }
            if (!flag && Collision.SolidCollision(npc.BottomLeft, npc.width, 6) && npc.Bottom.Y > target.position.Y)
            {
                npc.velocity.Y = 0;
            }

            if (!flag && (!Collision.SolidCollision(npc.Bottom, 2, 2) || npc.Bottom.Y < target.position.Y))
            {
                if (npc.velocity.Y < 10)
                    npc.velocity.Y += 0.2f;
            }
        }
        public void Charge(NPC npc)
        {
            Player target = Main.player[npc.target];
            npc.ai[2]++;
            if (npc.ai[2] == 1)
            {
                SoundEngine.PlaySound(SoundID.NPCHit45 with { Pitch = -0.75f });
                for (int i = 0; i < 10; i++)
                {

                    Vector2 toplayer = (target.Center - npc.Center);
                    toplayer.Y = 0;
                    toplayer = toplayer.SafeNormalize(Vector2.Zero);
                    toplayer = toplayer.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-20, 20))) * Main.rand.Next(3, 10);
                    Dust dust = Dust.NewDustDirect(npc.Center, 0, 0, DustID.MushroomSpray, (int)toplayer.X, (int)toplayer.Y, Scale: 2, Alpha: 120);
                    dust.velocity = toplayer;
                    dust.noGravity = false;
                    NetSync(npc);
                }
            }
            if (npc.ai[2] < 60)
            {
                npc.velocity.X /= 1.03f;
                npc.ai[0] = 0;
            }
            if (npc.ai[2] == 60) npc.velocity.X = 0;
            if (npc.ai[2] > 60 && npc.ai[2] < 120)
            {
                if ((target.Center.X > npc.Center.X || npc.velocity.X > 0) && npc.velocity.X >= 0)
                {
                    npc.velocity.X += 0.2f;
                }
                else if ((target.Center.X < npc.Center.X || npc.velocity.X < 0) && npc.velocity.X <= 0)
                {
                    npc.velocity.X -= 0.2f;
                }
                npc.ai[0] = 1;

            }
            if (npc.ai[2] > 60 && npc.ai[2] % 30 == 0)
            {
                if (DLCUtils.HostCheck)
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.NextFloat() / 2, 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<ShroomGas>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);
            }
            if (npc.ai[2] > 30 && npc.ai[2] % 40 == 0 && NPC.CountNPCS(ModContent.NPCType<CrabShroom>()) < 8) //maximum of 6 so you don't enter the CBT shroom dungeon
            {
                if (DLCUtils.HostCheck)
                {
                    Vector2 position = npc.Center - Vector2.UnitY * npc.height / 3;
                    NPC shroom = NPC.NewNPCDirect(npc.GetSource_FromAI(), (int)position.X, (int)position.Y, ModContent.NPCType<CrabShroom>());
                    shroom.velocity = new Vector2(0, -5);
                    int dir = -Math.Sign(npc.velocity.X);
                    shroom.velocity = shroom.velocity.RotatedBy(dir * MathHelper.Pi / 4f);
                    shroom.velocity = shroom.velocity.RotatedByRandom(MathHelper.Pi / 16f);
                }
                SoundEngine.PlaySound(SoundID.Item42, npc.Center); //mourning wood pew sound
            }
            if (((Collision.SolidTiles(npc.TopLeft, -6, npc.height - 10) || Collision.SolidTiles(npc.TopRight, 6, npc.height - 10) || Math.Abs(npc.Center.X - target.Center.X) > 900) && ((npc.velocity.X > 0 && npc.Center.X > target.Center.X) || (npc.velocity.X < 0 && npc.Center.X < target.Center.X))) || (npc.velocity.X == 0 && npc.ai[2] > 140) && npc.ai[2] > 140)
            {
                if (npc.velocity.X < 0)
                {
                    for (int i = 0; i < 30; i++)
                        Dust.NewDustDirect(npc.TopLeft, 6, npc.height, DustID.MushroomSpray, Alpha: 200, Scale: 2).noGravity = true;
                    if (DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(7, 0), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.MushBomb>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(7, 0).RotatedBy(MathHelper.ToRadians(-30)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.MushBomb>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);
                    }
                }
                else if (npc.velocity.X > 0)
                {
                    for (int i = 0; i < 30; i++)
                        Dust.NewDustDirect(npc.TopRight, -6, npc.height, DustID.MushroomSpray, Alpha: 200, Scale: 2).noGravity = true;
                    if (DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(-7, 0), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.MushBomb>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(-7, 0).RotatedBy(MathHelper.ToRadians(30)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.MushBomb>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);
                    }
                }

                SoundEngine.PlaySound(SoundID.Item14, npc.Center);
                npc.velocity.X = 0;
                npc.ai[2] = 0;
                IncrementCycle(npc);
            }
        }
        public void Walk(NPC npc, int type)
        {
            Player target = Main.player[npc.target];
            npc.ai[0] = 1;
            if (Collision.SolidCollision(npc.BottomLeft, npc.width, 6) && npc.collideY)
            {
                npc.ai[0] = 1;
            }
            npc.ai[2]++;
            if (npc.ai[2] == 240)
            {
                npc.ai[2] = 0;
                IncrementCycle(npc);
            }

            if (target.Center.X > npc.Center.X && npc.velocity.X < 5)
            {

                npc.velocity.X += 0.1f;
                if (npc.velocity.X < 0) npc.velocity.X += 0.2f;
            }
            else if (target.Center.X < npc.Center.X && npc.velocity.X > -5)
            {
                npc.velocity.X -= 0.1f;
                if (npc.velocity.X > 0) npc.velocity.X -= 0.2f;
            }
            if (type == 1 && npc.ai[2] % 30 == 0)
            {
                Vector2 position = npc.TopLeft + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height));
                if (DLCUtils.HostCheck)
                {
                    NPC shroom = NPC.NewNPCDirect(npc.GetSource_FromAI(), (int)position.X, (int)position.Y, ModContent.NPCType<CrabShroom>());
                    shroom.velocity = new Vector2(0, -5);
                }
            }
        }
        public void Jump(NPC npc, int type)
        {
            Player target = Main.player[npc.target];
            npc.ai[2]++;
            npc.ai[0] = 3;
            if (npc.ai[2] == 1)
            {
                npc.velocity.Y = -15;
                npc.noTileCollide = true;

                if (npc.Center.X > target.Center.X)
                {
                    npc.velocity.X = -5;
                }
                else if (npc.Center.X < target.Center.X)
                {
                    npc.velocity.X = 5;
                }
                NetSync(npc);
            }
            if (enrageJumping && DLCUtils.HostCheck)
            {
                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.position + new Vector2(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height)), Vector2.Zero, ModContent.ProjectileType<ShroomGas>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
            }
            if (Collision.SolidCollision(npc.BottomLeft, npc.width, 10) && npc.ai[2] > 20 && target.position.Y < npc.BottomLeft.Y && npc.velocity.Y >= 0)
            {
                npc.noTileCollide = false;
                npc.ai[2] = 0;
                enrageJumping = false;
                enrageJumpTimer = 0;
                IncrementCycle(npc);
                npc.ai[0] = 0;
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustDirect(npc.BottomLeft, npc.width, 6, DustID.MushroomSpray, Alpha: 200, Scale: 2).noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.Item14, npc.Center);
                if (type == 1)
                {
                    if (DLCUtils.HostCheck)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + new Vector2(150, 0), Vector2.Zero, ModContent.ProjectileType<MushroomSpear>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: 10);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + new Vector2(-150, 0), Vector2.Zero, ModContent.ProjectileType<MushroomSpear>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0: -10);
                    }
                }
                if (type == 2)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (DLCUtils.HostCheck)
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.BottomLeft + new Vector2(Main.rand.Next(0, npc.width), -5), new Vector2(Main.rand.NextFloat(), Main.rand.NextFloat()) / 5, ModContent.ProjectileType<ShroomGas>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                }
            }
        }
        public void IncrementCycle(NPC npc)
        {
            npc.ai[1]++;
            if (npc.ai[1] >= attackCycle.Length)
            {
                npc.ai[1] = 0;
            }
            NetSync(npc);
        }
    }
}
