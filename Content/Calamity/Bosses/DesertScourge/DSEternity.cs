
using CalamityMod.Events;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using FargowiltasSouls;
using System.Security.Cryptography.X509Certificates;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.DesertScourge
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DSEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<DesertScourgeHead>());
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
            entity.lifeMax = (int)Math.Round(entity.lifeMax * 1.6f);
            if (BossRushEvent.BossRushActive)
            {
                entity.lifeMax = 7000000;
            }
        }
        public float[] drawInfo = new float[] { 0, 200, 200, 0 };
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Main.NewText(drawInfo[2]);
            Main.instance.LoadProjectile(ProjectileID.SandnadoHostile);
            Texture2D wind = Terraria.GameContent.TextureAssets.Projectile[ProjectileID.SandnadoHostile].Value;
            for (int i = (int)drawInfo[2]; i > (int)drawInfo[1]; i--)
            {
                float opacity = 1;
                if (i < 100)
                {
                    opacity = i / 100f;
                }
                if (i < drawInfo[2] + 50)
                {
                    opacity = i / (drawInfo[2] + 70);
                }
                Vector2 pos = npc.Center - screenPos + new Vector2(20, 0).RotatedBy(npc.rotation - MathHelper.PiOver2) + new Vector2(2, 0).RotatedBy(npc.rotation - MathHelper.PiOver2) * i * 5 * (i / 100f);
                Vector2 pos2 = npc.Center + new Vector2(20, 0).RotatedBy(npc.rotation - MathHelper.PiOver2) + new Vector2(2, 0).RotatedBy(npc.rotation - MathHelper.PiOver2) * i * 5 * (i / 100f);
                Color color = new Color(194, 178, 128, 120) * 0.3f * opacity;
                if (Lighting.GetColor(pos2.ToTileCoordinates()).Equals(Color.Black))
                {
                    color = Color.Black * 0;
                }
                //Main.NewText(wind + "" + pos + "" + color + "" + opacity);
                Main.EntitySpriteDraw(wind, pos, null, color, drawInfo[0] + MathHelper.ToRadians(i), wind.Size() / 2, 1 + i / 10f, SpriteEffects.None);
            }
            
            drawInfo[0] += MathHelper.ToRadians(3f);
            
            
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public float[] ai = new float[] { 0, 0, 0, 0, 0 };
        public const int attackCycleLength = 9;
        public int[] attackCycle = new int[attackCycleLength] { 0, 1, 0, 1, 1, 0, -1, -1, -1 };
        public int phase;
        public bool DoSlam = false;
        
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            for (int i = 0; i < ai.Length; i++)
            {
                binaryWriter.Write(ai[i]);
            }
            for (int i = 0; i < attackCycle.Length; i++)
            {
                binaryWriter.Write7BitEncodedInt(attackCycle[i]);
            }
            binaryWriter.Write7BitEncodedInt(phase);
            binaryWriter.Write(DoSlam);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            for (int i = 0; i < ai.Length; i++)
            {
                ai[i] = binaryReader.ReadSingle();
            }
            for (int i = 0; i < attackCycle.Length; i++)
            {
                attackCycle[i] = binaryReader.Read7BitEncodedInt();
            }
            phase = binaryReader.Read7BitEncodedInt();
            DoSlam = binaryReader.ReadBoolean();
        }
        
        public override bool SafePreAI(NPC npc)
        {
            if (npc == null || !npc.active)
            {
                return true;
            }
            if (npc.ai[2] < 20)
            {
                npc.ai[2]++;
                return true;

            }
            Player target = Main.player[npc.target];
            if (target == null || !target.active || target.dead)
            {
                return true;
            }
            Vector2 toplayer = (target.Center - npc.Center).SafeNormalize(Vector2.Zero);
            npc.realLife = -1;
            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
            int attack = attackCycle[(int)npc.ai[3]];
            if (DoSlam)
            {
                attack = 22;
            }
            if (attack < 0)
            {
                attack = 0;
            }

            npc.netUpdate = true; //fuck you worm mp code

            if (phase == 0 && (!NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHead>()) || npc.GetLifePercent() <= 0.75f))
            {
                phase++;
                attackCycle = new int[attackCycleLength] { 4, 0, 1, 3, attack, -1, -1, -1, -1 };
                npc.ai[3] = attackCycle.Length - 5;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].type == ModContent.NPCType<DesertNuisanceHead>() && Main.npc[i].active)
                    {
                        Main.npc[i].StrikeInstantKill();
                        Main.npc[i].netUpdate = true;
                    }
                }
                NetSync(npc);
            }
            if (phase == 1 && npc.GetLifePercent() <= 0.5f)
            {
                phase++;
                NetSync(npc);
                attackCycle = new int[attackCycleLength] { 5, 0, 2, 2, 3, 4, attack, -1, -1 };
                NetSync(npc);
                npc.ai[3] = attackCycle.Length - 3;
                NetSync(npc);
            }
            if (phase == 2 && npc.GetLifePercent() <= 0.2f)
            {
                phase++;
                NetSync(npc);
                attackCycle = new int[attackCycleLength] { 3, 2, 3, 5, 5, 3, 5, 2, attack };
                NetSync(npc);
                npc.ai[3] = attackCycle.Length - 1;
                NetSync(npc);
            }
            if (attack == 0)
            {
                bool collision = CheckCollision(npc);
                TooFarCheck(npc, ref collision);
                WormMovement(npc, collision);

                ai[0]++;
                if (ai[0] == 250 && phase > 0) //initiate slam, only after first phase (nuisances)
                {
                    DoSlam = true;
                    NetSync(npc);
                }
                if (ai[0] == 500)
                {
                    ai[0] = 0;
                    IncrementCycle(npc);
                }
            }
            else if (attack == 1)
            {
                SetupLunge(npc, true, false, false, new Vector2(700, 700), new Vector2(50, -300), new Vector2(700, 700));

            }
            else if (attack == 2)
            {
                SetupLunge(npc, true, true, false, new Vector2(700, 700), new Vector2(50, -300), new Vector2(700, 700));
            }
            else if (attack == 3)
            {
                SetupLunge(npc, true, false, true, new Vector2(700, 700), new Vector2(50, -300), new Vector2(700, 700));
            }
            else if (attack == 4)
            {
                suck(npc);
            }
            else if (attack == 5)
            {
                SetupLunge(npc, true, false, false, new Vector2(0, 900), new Vector2(0, -400), new Vector2(0, 500));
            }
            else if (attack == 22)
            {
                Slam(npc);
            }
            ai[1]++;
            if (ai[1] >= 300)
            {
                int dir = (Main.rand.NextBool() ? -1 : 1);
                //Projectile.NewProjectile(npc.GetSource_FromAI(), target.Center + new Vector2(800 * dir, 100), new Vector2(-5 * dir, 0), ModContent.ProjectileType<Sandnado>(), DLCUtlis.RealDamage(25), 0);
                ai[1] = 0;
                NetSync(npc);
            }
            return false;
        }
        public void suck(NPC npc)
        {
            Player target = Main.player[npc.target];
            if (ai[3] >= 0)
            {
                ai[3]++;
                npc.velocity = Vector2.Lerp(npc.velocity, (target.Center - npc.Center) / 50, 0.03f);
            }
            else
            {
                ai[3]--;
                npc.velocity = Vector2.Lerp(npc.velocity, (target.Center - npc.Center) / 80, 0.03f);
            }
            Vector2 toplayer = (target.Center - npc.Center).SafeNormalize(Vector2.Zero);
            
            if (ai[3] > 140 && ai[3] < 550)
            {
                target.velocity.X += ((npc.Center - target.Center).SafeNormalize(Vector2.Zero) * 0.1f).X;
                target.velocity.Y /= 1.1f;
                if (ai[3] % 20 == 0)
                {
                    Vector2 pos = npc.Center + new Vector2(1300, Main.rand.Next(-300, 300)).RotatedBy(npc.rotation - MathHelper.PiOver2);
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), pos, (npc.Center - pos).SafeNormalize(Vector2.Zero) * 7, ModContent.ProjectileType<SuckedSandBlast>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, ai0:npc.whoAmI);
                }
            }
            if (ai[3] == -2) //telegraph spit followup
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/DesertScourgeRoar") with { Volume = 0.5f, Pitch = 0.2f });
            }
            if (ai[3] % 70 == 0 && ai[3] < 0)
            {
                if (DLCUtils.HostCheck)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * 20, ModContent.ProjectileType<SandChunk>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    for (int i = -2; i < 3; i++)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2 + MathHelper.ToRadians(i * 10)) * 15, ModContent.ProjectileType<GreatSandBlast>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                }
                SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                NetSync(npc);
            }
            if (ai[3] == -150)
            {
                ai[3] = 0;
                IncrementCycle(npc);
            }
            if (ai[3] >= 0)
            {
                if (drawInfo[1] > 0)
                {
                    drawInfo[1]--;
                }
                if (drawInfo[1] <= 0)
                {
                    ai[2]++;
                    if (ai[2] > 200)
                    {
                        if (drawInfo[2] > 0)
                            drawInfo[2]--;
                        if (drawInfo[2] <= 0)
                        {
                            drawInfo[2] = 200;
                            drawInfo[1] = 200;
                            ai[2] = 0;
                            ai[3] = -1;
                        }
                    }
                }
            }

        }
        public void Slam(NPC npc)
        {
            ai[0] = 251; //keep timer on this until attack ends
            Player player = Main.player[npc.target];
            Vector2 targetPos = player.Center - Vector2.UnitY * 100;
            ref float timer = ref ai[3];
            ref float attackStep = ref ai[2];

            
            switch (attackStep)
            {
                case 0: //fly straight towards player until close enough
                    {
                        if (timer == 0)
                        {
                            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/DesertScourgeRoar") with { Volume = 0.5f, Pitch = -0.3f });
                        }
                        const float MaxSpeed = 25;
                        float inertia = 15f;
                        const float CloseEnough = 300;

                        Vector2 toPlayer = targetPos - npc.Center;
                        float distance = toPlayer.Length();
                        if (npc.velocity == Vector2.Zero)
                        {
                            npc.velocity.X = -0.15f;
                            npc.velocity.Y = -0.05f;
                        }
                        if (distance > CloseEnough)
                        {
                            toPlayer.Normalize();
                            toPlayer *= MaxSpeed;
                            npc.velocity = (npc.velocity * (inertia - 1f) + toPlayer) / inertia;
                        }
                        else
                        {
                            timer = 0;
                            attackStep = 1;
                        }
                       
                    }
                    break;
                case 1: //decelerate X, accelerate upwards Y, preparing for slam
                    {
                        const float accelUp = 1.5f;
                        if (npc.velocity.Y > -25)
                        {
                            npc.velocity.Y -= accelUp;
                        }
                        npc.velocity.X *= 0.97f;
                        if (npc.velocity.Y <= -25 && timer > 40)
                        {
                            timer = 0;
                            attackStep = 2;
                        }
                    }
                    break;
                case 2: //get gravity and fall, SLIGHTLY move towards player? prepare for crash
                    {
                        const float gravity = 1f;
                        const float xTracking = 0.25f;
                        npc.velocity.Y += gravity;
                        npc.velocity.X += Math.Sign(player.Center.X - npc.Center.X) * xTracking;
                        if (Collision.SolidCollision(npc.position, npc.width, npc.height) || timer > 60 * 5) //end attack on collision or after safety max time
                        {
                            npc.velocity /= 4;
                            SoundEngine.PlaySound(SoundID.Item62, npc.Center);
                            const int ShotCount = 10; //per side
                            const int MaxShotSpeed = 16;
                            for (int i = 0; i < ShotCount; i++)
                            {
                                for (int side = -1; side < 2; side += 2)
                                {
                                    
                                    float speed = MaxShotSpeed * (float)(i+1) / ShotCount;
                                    Vector2 dir = (-Vector2.UnitY).RotatedBy(side * MathHelper.Pi / 9.85f);
                                    float randfac = MathHelper.Pi / 18f;
                                    float randrot = Main.rand.NextFloat(-randfac, randfac);
                                    for (int j = -1; j < 2; j += 2)
                                    {
                                        float offset = randfac * npc.width / 5f;
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + (Vector2.UnitX * side * (offset + (npc.width / 3))), speed * dir.RotatedBy(randfac * j), ModContent.ProjectileType<GreatSandBlast>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 2f);
                                    }
                                    
                                }
                            }
                            timer = 0;
                            attackStep = 0;
                            DoSlam = false;
                            return;
                        }
                    }
                    break;
            }
            timer++;
        }
        public float[] lungeInfo = new float[] { 0, 0, 0, 0, 0};
        //Sets values for doing the lunge attack with configureable projectiles to accompany the attack
        //times is the number of times to lunge
        //blasts is the little sand projs when he comes out of the ground
        //chunk is the spike ball that splits into blasts
        // water projs fall straight down

        
        public void SetupLunge(NPC npc, bool blasts, bool chunk, bool water, Vector2 lungePos, Vector2 targetPos, Vector2 endPos)
        {
            
            if (lungeInfo[4] == 1)
            {
                PrepareLunge(npc, lungePos);
                return;
            }
            if (lungeInfo[4] == 2)
            {
                DoLunge(npc, targetPos, endPos);
                return;
            }
            lungeInfo[0] = 0;
            if (blasts)
            {
                lungeInfo[1] = 1;
            }
            if (chunk)
            {
                lungeInfo[2] = 1;
            }
            if (water)
            {
                lungeInfo[3] = 1;
            }
            lungeInfo[4] = 1;
            NetSync(npc);
        }
        //move to bottom right/left to be in position for lunge
        public void PrepareLunge(NPC npc, Vector2 lungePos)
        {
            Player target = Main.player[npc.target];
            Vector2 offset = lungePos;
            if (target.Center.X > npc.Center.X) offset.X = -offset.X;
            npc.velocity = Vector2.Lerp(npc.velocity, (target.Center + offset - npc.Center).SafeNormalize(Vector2.Zero)*15, 0.05f);
            if (npc.Distance(target.Center + offset) <= 50)
            lungeInfo[0]++;
            if (lungeInfo[0] == 10)
            {
                
                lungeInfo[0] = 0;
                lungeInfo[4] = 2;
                NetSync(npc);
            }
        }
        public void DoLunge(NPC npc, Vector2 targetPos, Vector2 endPos)
        {
            
            //play sound at start only once
            if (lungeInfo[0] == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/DesertScourgeRoar") with { Volume = 0.5f});
                lungeInfo[0]++;
                
            }
            Player target = Main.player[npc.target];
            //fire blasts first frame ds can see the player
            if (Collision.CanHit(npc, target) && lungeInfo[0] == 1 && lungeInfo[1] == 1)
            {
                lungeInfo[0]++;
                int j = -4;
                if (targetPos.X == 0) j = -2;
                if (DLCUtils.HostCheck)
                    for (int i = j; i < (-j) + 1; i++)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2 + MathHelper.ToRadians(i * 10)) * 15, ModContent.ProjectileType<GreatSandBlast>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
            }
            if (npc.velocity.X < 0)
            {
                targetPos.X = -targetPos.X;
                endPos.X = -endPos.X;
            }
            //if has ever been close to the position directly above the player, move downward and to the side of the player
            if (npc.Distance(target.Center + targetPos) < 100 || lungeInfo[0] > 3)
            {
                if (npc.velocity.X > 0)
                    npc.velocity = Vector2.Lerp(npc.velocity, (target.Center + endPos - npc.Center).SafeNormalize(Vector2.Zero) * 20, 0.08f);
                else
                {
                    npc.velocity = Vector2.Lerp(npc.velocity, (target.Center + endPos - npc.Center).SafeNormalize(Vector2.Zero) * 20, 0.08f);
                }
                if (lungeInfo[0] == 2 && lungeInfo[2] == 1)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(0, -8), ModContent.ProjectileType<SandChunk>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                }
                lungeInfo[0]++;
            }
            //move to directly above the player (slightly offset to avoid turning around)
            else
            {
               
                npc.velocity = Vector2.Lerp(npc.velocity, (target.Center + targetPos - npc.Center).SafeNormalize(Vector2.Zero) * 20, 0.1f);
                
            }
            if (lungeInfo[3] > 0)
            {
                lungeInfo[3]++;
                if (lungeInfo[3] % 15 == 0)
                {
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(0, 7), ModContent.ProjectileType<CalamityMod.Projectiles.Enemy.HorsWaterBlast>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                }
            }
            //if has been moving downard and to the side of the player (ending the lung), truely end the attack. Repeat the attack if lungeinfo 1 is not zero.
            if (lungeInfo[0] > 60)
            {
                
                IncrementCycle(npc);
                lungeInfo = new float[] { 0, 0, 0, 0, 0 };
                
            }
        }
        public void IncrementCycle(NPC npc)
        {
            npc.ai[3]++;
            if (npc.ai[3] >= attackCycle.Length-1 || attackCycle[(int)npc.ai[3]] < 0)
            {
                npc.ai[3] = 0;
            }
            NetSync(npc);
        }
        bool canSee = false;
        public void WormMovement(NPC npc, bool collision)
        {
            Player playerTarget = Main.player[npc.target];
            if (!canSee && Collision.CanHit(npc, playerTarget) && npc.GetLifePercent() <= 0.5f)
            {
                if (DLCUtils.HostCheck)
                    for (int i = -3; i < 4; i++)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2 + MathHelper.ToRadians(i * 10)) * 10, ModContent.ProjectileType<GreatSandBlast>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                    }
                canSee = true;
            }
            if (!Collision.CanHit(npc, playerTarget))
            {
                canSee = false;
            }
            float speed = 20;
            // acceleration is exactly what it sounds like. The speed at which this NPC accelerates.
            float acceleration = 0.2f;

            float targetXPos, targetYPos;

            

            Vector2 forcedTarget = new Vector2(10000, 10000);
            if (npc.target >= 0 && Main.player[npc.target] != null && Main.player[npc.target].active && !Main.player[npc.target].dead)
                forcedTarget = playerTarget.Center;
            // Using a ValueTuple like this allows for easy assignment of multiple values
            (targetXPos, targetYPos) = (forcedTarget.X, forcedTarget.Y);

            // Copy the value, since it will be clobbered later
            Vector2 npcCenter = npc.Center;

            float targetRoundedPosX = (float)((int)(targetXPos / 16f) * 16);
            float targetRoundedPosY = (float)((int)(targetYPos / 16f) * 16);
            npcCenter.X = (float)((int)(npcCenter.X / 16f) * 16);
            npcCenter.Y = (float)((int)(npcCenter.Y / 16f) * 16);
            float dirX = targetRoundedPosX - npcCenter.X;
            float dirY = targetRoundedPosY - npcCenter.Y;

            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);

            // If we do not have any type of collision, we want the NPC to fall down and de-accelerate along the X axis.
            if (!collision)
            {
                npc.TargetClosest(true);

                // Constant gravity of 0.11 pixels/tick
                npc.velocity.Y += 0.17f;

                // Ensure that the NPC does not fall too quickly
                if (npc.velocity.Y > speed)
                    npc.velocity.Y = speed;

                // The following behaviour mimicks vanilla worm movement
                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < speed * 0.4f)
                {
                    // Velocity is sufficiently fast, but not too fast
                    if (npc.velocity.X < 0.0f)
                        npc.velocity.X -= acceleration * 1.1f;
                    else
                        npc.velocity.X += acceleration * 1.1f;
                }
                else if (npc.velocity.Y == speed)
                {
                    // NPC has reached terminal velocity
                    if (npc.velocity.X < dirX)
                        npc.velocity.X += acceleration;
                    else if (npc.velocity.X > dirX)
                        npc.velocity.X -= acceleration;
                }
                else if (npc.velocity.Y > 4)
                {
                    if (npc.velocity.X < 0)
                        npc.velocity.X += acceleration * 0.9f;
                    else
                        npc.velocity.X -= acceleration * 0.9f;
                }
            }
            else
            {
                // Else we want to play some audio (soundDelay) and move towards our target.
                if (npc.soundDelay == 0)
                {
                    // Play sounds quicker the closer the NPC is to the target location
                    float num1 = length / 40f;

                    if (num1 < 10)
                        num1 = 10f;

                    if (num1 > 20)
                        num1 = 20f;

                    npc.soundDelay = (int)num1;

                    SoundEngine.PlaySound(SoundID.WormDig, npc.position);
                }

                float absDirX = Math.Abs(dirX);
                float absDirY = Math.Abs(dirY);
                float newSpeed = speed / length;
                dirX *= newSpeed;
                dirY *= newSpeed;

                if ((npc.velocity.X > 0 && dirX > 0) || (npc.velocity.X < 0 && dirX < 0) || (npc.velocity.Y > 0 && dirY > 0) || (npc.velocity.Y < 0 && dirY < 0))
                {
                    // The npc is moving towards the target location
                    if (npc.velocity.X < dirX)
                        npc.velocity.X += acceleration;
                    else if (npc.velocity.X > dirX)
                        npc.velocity.X -= acceleration;

                    if (npc.velocity.Y < dirY)
                        npc.velocity.Y += acceleration;
                    else if (npc.velocity.Y > dirY)
                        npc.velocity.Y -= acceleration;

                    // The intended Y-velocity is small AND the npc is moving to the left and the target is to the right of the npc or vice versa
                    if (Math.Abs(dirY) < speed * 0.2 && ((npc.velocity.X > 0 && dirX < 0) || (npc.velocity.X < 0 && dirX > 0)))
                    {
                        if (npc.velocity.Y > 0)
                            npc.velocity.Y += acceleration * 2f;
                        else
                            npc.velocity.Y -= acceleration * 2f;
                    }

                    // The intended X-velocity is small AND the npc is moving up/down and the target is below/above the npc
                    if (Math.Abs(dirX) < speed * 0.2 && ((npc.velocity.Y > 0 && dirY < 0) || (npc.velocity.Y < 0 && dirY > 0)))
                    {
                        if (npc.velocity.X > 0)
                            npc.velocity.X = npc.velocity.X + acceleration * 2f;
                        else
                            npc.velocity.X = npc.velocity.X - acceleration * 2f;
                    }
                }
                else if (absDirX > absDirY)
                {
                    // The X distance is larger than the Y distance.  Force movement along the X-axis to be stronger
                    if (npc.velocity.X < dirX)
                        npc.velocity.X += acceleration * 1.1f;
                    else if (npc.velocity.X > dirX)
                        npc.velocity.X -= acceleration * 1.1f;

                    if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < speed * 0.5)
                    {
                        if (npc.velocity.Y > 0)
                            npc.velocity.Y += acceleration;
                        else
                            npc.velocity.Y -= acceleration;
                    }
                }
                else
                {
                    // The X distance is larger than the Y distance.  Force movement along the X-axis to be stronger
                    if (npc.velocity.Y < dirY)
                        npc.velocity.Y += acceleration * 1.1f;
                    else if (npc.velocity.Y > dirY)
                        npc.velocity.Y -= acceleration * 1.1f;

                    if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < speed * 0.5)
                    {
                        if (npc.velocity.X > 0)
                            npc.velocity.X += acceleration;
                        else
                            npc.velocity.X -= acceleration;
                    }
                }
            }

            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;

            // Some netupdate stuff (multiplayer compatibility).
            if (collision)
            {
                if (npc.localAI[0] != 1)
                    npc.netUpdate = true;

                npc.localAI[0] = 1f;
            }
            else
            {
                if (npc.localAI[0] != 0)
                    npc.netUpdate = true;

                npc.localAI[0] = 0f;
            }

            // Force a netupdate if the npc's velocity changed sign and it was not "just hit" by a player
            if (((npc.velocity.X > 0 && npc.oldVelocity.X < 0) || (npc.velocity.X < 0 && npc.oldVelocity.X > 0) || (npc.velocity.Y > 0 && npc.oldVelocity.Y < 0) || (npc.velocity.Y < 0 && npc.oldVelocity.Y > 0)) && !npc.justHit)
                npc.netUpdate = true;
        }
        public void TooFarCheck(NPC npc, ref bool collision)
        {
            if (!collision)
            {
                Rectangle hitbox = npc.Hitbox;

                int maxDistance = 900;

                bool tooFar = true;

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Rectangle areaCheck;

                    Player player = Main.player[i];
                    
                    
                    if (player.active && !player.dead && !player.ghost)
                        areaCheck = new Rectangle((int)player.position.X - maxDistance, (int)player.position.Y - maxDistance, maxDistance * 2, maxDistance * 2);
                    else
                        continue;  // Not a valid player

                    if (hitbox.Intersects(areaCheck))
                    {
                        tooFar = false;
                        break;
                    }
                }

                if (tooFar)
                    collision = true;
            }
        }
        public bool CheckCollision(NPC npc)
        {
            int minTilePosX = (int)(npc.Left.X / 16) - 1;
            int maxTilePosX = (int)(npc.Right.X / 16) + 2;
            int minTilePosY = (int)(npc.Top.Y / 16) - 1;
            int maxTilePosY = (int)(npc.Bottom.Y / 16) + 2;

            // Ensure that the tile range is within the world bounds
            if (minTilePosX < 0)
                minTilePosX = 0;
            if (maxTilePosX > Main.maxTilesX)
                maxTilePosX = Main.maxTilesX;
            if (minTilePosY < 0)
                minTilePosY = 0;
            if (maxTilePosY > Main.maxTilesY)
                maxTilePosY = Main.maxTilesY;

            bool collision = false;

            // This is the initial check for collision with tiles.
            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    Tile tile = Main.tile[i, j];

                    // If the tile is solid or is considered a platform, then there's valid collision
                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0) || tile.LiquidAmount > 64)
                    {
                        Vector2 tileWorld = new Point16(i, j).ToWorldCoordinates(0, 0);

                        if (npc.Right.X > tileWorld.X && npc.Left.X < tileWorld.X + 16 && npc.Bottom.Y > tileWorld.Y && npc.Top.Y < tileWorld.Y + 16)
                        {
                            // Collision found
                            collision = true;

                            if (Main.rand.NextBool(100))
                                WorldGen.KillTile(i, j, fail: true, effectOnly: true, noItem: false);
                        }
                    }
                }
            }

            return collision;
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DSBodyBuff : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<DesertScourgeBody>());
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
            if (BossRushEvent.BossRushActive)
            {
                entity.lifeMax = 25000000;
            }
        }
        public static List<int> PierceResistExclude = new List<int>
        {
            ModContent.ProjectileType<SproutingAcorn>()
        };
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByProjectile(npc, projectile, ref modifiers);
            if (projectile.type == ProjectileID.SporeCloud)
            {
                modifiers.FinalDamage.Base = 1;
            }
            if ((projectile.penetrate != projectile.maxPenetrate || projectile.penetrate < 0) && !PierceResistExclude.Contains(projectile.type)) //for hits after the first, except infinite piercings
            {
                modifiers.FinalDamage /= 3;
            }
        }
        public override bool SafePreAI(NPC npc)
        {

            npc.netUpdate = true; //fuck you worm mp code

            return base.SafePreAI(npc);
        }
        public override void SafePostAI(NPC npc)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHead>()))
            {
                npc.defense = 30;
            }
            else
            {
                npc.defense = 10;
            }
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.lifeRegen < 0)
            {
                npc.lifeRegen = (int)Math.Round(npc.lifeRegen / 4f);
            }
        }
    }
}
