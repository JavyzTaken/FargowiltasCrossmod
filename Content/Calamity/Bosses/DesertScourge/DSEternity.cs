using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.DesertScourge
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DSEternity : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<DesertScourgeHead>();
        public override void SetDefaults()
        {
            NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 1.125f);
            NPC.damage = 30;
        }
        public float[] drawInfo = [0, 200, 200, 0];
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
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
                Vector2 pos = NPC.Center - screenPos + new Vector2(20, 0).RotatedBy(NPC.rotation - MathHelper.PiOver2) + new Vector2(2, 0).RotatedBy(NPC.rotation - MathHelper.PiOver2) * i * 5 * (i / 100f);
                Vector2 pos2 = NPC.Center + new Vector2(20, 0).RotatedBy(NPC.rotation - MathHelper.PiOver2) + new Vector2(2, 0).RotatedBy(NPC.rotation - MathHelper.PiOver2) * i * 5 * (i / 100f);
                Color color = new Color(194, 178, 128, 120) * 0.3f * opacity;
                if (Lighting.GetColor(pos2.ToTileCoordinates()).Equals(Color.Black))
                {
                    color = Color.Black * 0;
                }
                //Main.NewText(wind + "" + pos + "" + color + "" + opacity);
                Main.EntitySpriteDraw(wind, pos, null, color, drawInfo[0] + MathHelper.ToRadians(i), wind.Size() / 2, 1 + i / 10f, SpriteEffects.None);
            }

            drawInfo[0] += MathHelper.ToRadians(3f);


            return true;
        }

        public float[] ai = [0, 0, 0, 0, 0];
        public const int attackCycleLength = 9;
        public int[] attackCycle = [0, 1, 0, 1, 1, 0, -1, -1, -1];
        public int phase;
        public bool CanDoSlam = false;
        public int SlamCooldown = 0;
        public bool DoSlam = false;
        public int AttackIndex = 0;

        public int DespawnTimer = 0;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
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
            binaryWriter.Write(CanDoSlam);
            binaryWriter.Write7BitEncodedInt(AttackIndex);
            binaryWriter.Write7BitEncodedInt(SlamCooldown);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
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
            CanDoSlam = binaryReader.ReadBoolean();
            AttackIndex = binaryReader.Read7BitEncodedInt();
            SlamCooldown = binaryReader.Read7BitEncodedInt();
        }

        public override bool PreAI()
        {
            if (NPC == null || !NPC.active)
            {
                return true;
            }
            Player target = Main.player[NPC.target];
            if (!Targeting())
            {
                return false;
            }

            if (NPC.GetLifePercent() < 0.5f && (NPC.localAI[2] == 0))
                return true;
            if (NPC.GetLifePercent() < 0.5f && (NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHead>()) || NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHeadYoung>()))) // Nuisance phase
            {
                drawInfo = [0, 200, 200, 0];
                for (int i = 0; i < ai.Length; i++)
                {
                    ai[i] = 0;
                }
                AttackIndex = 0;

                // Calamity burrow
                NPC.Calamity().newAI[0] = 0f;
                NPC.Calamity().newAI[1] = 0f;
                NPC.Calamity().newAI[3] = 0f;
                NPC.localAI[3] = 0f;
                NPC.As<DesertScourgeHead>().playRoarSound = false;
                NPC.dontTakeDamage = true;

                // Calamity variables condensed
                float maxChaseSpeed = 18f * 1.75f;
                float turnSpeed = 0.3f * 1.15f * 1.05f + 0.12f;
                float burrowDistance = 1080f;
                float burrowTarget = target.Center.Y + burrowDistance;

                NPC.alpha += 3;
                if (NPC.alpha > 255)
                {
                    NPC.alpha = 255;
                }
                else
                {
                    for (int dustIndex = 0; dustIndex < 2; dustIndex++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.UnusedBrown, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].noLight = true;
                    }
                }

                NPC.SimpleFlyMovement((new Vector2(target.Center.X, burrowTarget) - NPC.Center).SafeNormalize(Vector2.UnitY) * maxChaseSpeed, turnSpeed);
                NPC.damage = 0;


                return false;
            }
            
            if (NPC.localAI[2] == 0f)
                NPC.localAI[2] = 2f; // Cannot summon nuisances unless otherwise specified

            if (NPC.ai[2] < 20)
            {
                NPC.ai[2]++;
                return true;
            }

            NPC.alpha -= 42;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            Vector2 toplayer = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);

            NPC.realLife = -1;

            NPC.damage = NPC.defDamage;
            NPC.dontTakeDamage = false;

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            int attack = attackCycle[AttackIndex];
            if (SlamCooldown > 0)
                SlamCooldown--;
            if (DoSlam)
            {
                attack = 22;
            }
            if (attack < 0)
            {
                attack = 0;
            }
            NPC.netUpdate = true; //fuck you worm mp code
           
            if (phase == 0 && (!NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHead>()) || NPC.GetLifePercent() <= 0.75f))
            {
                phase++;
                attackCycle = [4, 0, 1, 3, -1, -1, -1, -1];
                AttackIndex = attackCycle.Length - 5;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].type == ModContent.NPCType<DesertNuisanceHead>() && Main.npc[i].active)
                    {
                        Main.npc[i].StrikeInstantKill();
                        Main.npc[i].netUpdate = true;
                    }
                }
                NetSync(NPC);
            }
            if (phase == 1 && NPC.GetLifePercent() <= 0.5f)
            {
                phase++;
                attackCycle = [5, 0, 2, 2, 3, 4, -1, -1];
                AttackIndex = attackCycle.Length - 3;
                NetSync(NPC);
            }
            if (phase == 2 && NPC.GetLifePercent() <= 0.2f)
            {
                phase++;
                attackCycle = [3, 2, 3, 5, 5, 3, 5, 2];
                AttackIndex = attackCycle.Length - 1;
                NetSync(NPC);
            }
            if (attack == 0)
            {
                bool collision = CheckCollision(NPC);
                TooFarCheck(NPC, ref collision);
                WormMovement(NPC, collision);

                int idleTime = WorldSavingSystem.MasochistModeReal ? 300 : 500;
                ai[0]++;
                if (ai[0] == idleTime / 2 && phase > 0 && CanDoSlam && SlamCooldown <= 0) //initiate slam, only after first phase (nuisances)
                {
                    CanDoSlam = false;
                    SlamCooldown = 60 * 14;
                    DoSlam = true;
                    NPC.netUpdate = true;
                    NetSync(NPC);
                }
                if (ai[0] >= idleTime)
                {
                    ai[0] = 0;
                    IncrementCycle(NPC);
                }
            }
            else if (attack == 1)
            {
                SetupLunge(NPC, true, false, false, new Vector2(700, 700), new Vector2(50, -300), new Vector2(700, 700));

            }
            else if (attack == 2)
            {
                SetupLunge(NPC, true, true, false, new Vector2(700, 700), new Vector2(50, -300), new Vector2(700, 700));
            }
            else if (attack == 3)
            {
                SetupLunge(NPC, true, false, true, new Vector2(700, 700), new Vector2(50, -300), new Vector2(700, 700));
            }
            else if (attack == 4)
            {
                suck(NPC);
            }
            else if (attack == 5)
            {
                SetupLunge(NPC, true, false, false, new Vector2(0, 900), new Vector2(0, -400), new Vector2(0, 500));
            }
            else if (attack == 22)
            {
                Slam(NPC);
            }
            if (attack != 4) // Not suck, reset suck variables
            {
                drawInfo = [0, 200, 200, 0]; 
            }

            ManageMusicFade(attack == 22 && ai[3] > 0);

            ai[1]++;
            if (ai[1] >= 300)
            {
                int dir = (Main.rand.NextBool() ? -1 : 1);
                //Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center + new Vector2(800 * dir, 100), new Vector2(-5 * dir, 0), ModContent.ProjectileType<Sandnado>(), DLCUtlis.RealDamage(25), 0);
                ai[1] = 0;
                NetSync(NPC);
            }
            return false;

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
                        NPC.velocity.Y += 1f;
                        if (NPC.timeLeft <= 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                            }
                        }
                        if (++DespawnTimer > 90)
                            NPC.active = false;
                        return false;
                    }
                }
                return true;
            }
        }
        public void suck(NPC NPC)
        {
            Player target = Main.player[NPC.target];

            CanDoSlam = true; //can do slam after this attack

            if (ai[3] >= 0) // sucking
            {
                ai[3]++;
                if (WorldSavingSystem.MasochistModeReal)
                    ai[3]++;
                NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center) / 50, 0.03f);

                // open mouth
                NPC.ai[3] = 1;
                NPC.frameCounter = 0;
            }
            else // not sucking
            {
                ai[3]--;
                NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center) / 80, 0.05f);
            }
            Vector2 toplayer = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);

            if (ai[3] > 140 && ai[3] < 550)
            {
                
                target.velocity.X += ((NPC.Center - target.Center).SafeNormalize(Vector2.Zero) * 0.1f).X;
                target.velocity.Y /= 1.1f;
                if (ai[3] % 20 == 0)
                {
                    Vector2 pos = NPC.Center + new Vector2(1300, Main.rand.Next(-300, 300)).RotatedBy(NPC.rotation - MathHelper.PiOver2);
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, (NPC.Center - pos).SafeNormalize(Vector2.Zero) * 7, ModContent.ProjectileType<SuckedSandBlast>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: NPC.whoAmI);
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
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * 20, ModContent.ProjectileType<SandChunk>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    float spread = 14f;
                    for (int i = -2; i < 3; i++)
                    {
                        float dir = -MathHelper.PiOver2 + MathHelper.ToRadians(i * spread);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2().RotatedBy(dir) * 15, ModContent.ProjectileType<GreatSandBlast>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    }
                }
                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
                NetSync(NPC);
            }
            if (ai[3] == -150)
            {
                ai[3] = 0;
                IncrementCycle(NPC);
            }
            if (ai[3] >= 0)
            {
                int increment = WorldSavingSystem.MasochistModeReal ? 2 : 1;
                if (drawInfo[1] > 0)
                {
                    drawInfo[1] -= increment;
                }
                if (drawInfo[1] <= 0)
                {
                    ai[2] += increment;
                    if (ai[2] > 200)
                    {
                        if (drawInfo[2] > 0)
                            drawInfo[2] -= increment;
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
        public void Slam(NPC NPC)
        {
            ai[0] = 255; //keep timer on this until attack ends
            Player player = Main.player[NPC.target];
            Vector2 targetPos = player.Center - Vector2.UnitY * 100;
            ref float timer = ref ai[3];
            ref float attackStep = ref ai[2];

            switch (attackStep)
            {
                case 0: //fly straight towards player until close enough
                    {
                        const float MaxSpeed = 25;
                        float inertia = 15f;
                        //float horizontalDist = MathF.Abs(NPC.Center.X - player.Center.X);
                        if (timer <= 0)
                        {
                            Vector2 startpos = new(player.Center.X + player.HorizontalDirectionTo(NPC.Center) * 500, player.Center.Y);
                            startpos = LumUtils.FindGroundVertical(startpos.ToTileCoordinates()).ToWorldCoordinates();
                            startpos.Y += 170;
                            if (Math.Abs(startpos.Y - player.Center.Y) > 850)
                                startpos.Y = player.Center.Y + 850;
                            if (NPC.Distance(startpos) > 50)
                            {
                                timer = -1;
                                if (NPC.velocity == Vector2.Zero)
                                {
                                    NPC.velocity.X = -0.15f;
                                    NPC.velocity.Y = -0.05f;
                                }
                                Vector2 toPos = startpos - NPC.Center;
                                toPos.Normalize();
                                toPos *= MaxSpeed;
                                NPC.velocity = (NPC.velocity * (inertia - 1f) + toPos) / inertia;
                                return;
                            }
                            else
                            {
                                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/DesertScourgeRoar") with { Volume = 0.5f, Pitch = -0.3f });
                                timer = 1;
                            }
                        }
                        const float CloseEnough = 430;

                        Vector2 toPlayer = targetPos - NPC.Center;
                        float distance = toPlayer.X;
                        if (NPC.velocity == Vector2.Zero)
                        {
                            NPC.velocity.X = -0.15f;
                            NPC.velocity.Y = -0.05f;
                        }
                        if (distance > CloseEnough)
                        {
                            toPlayer.Normalize();
                            toPlayer *= MaxSpeed;
                            NPC.velocity = (NPC.velocity * (inertia - 1f) + toPlayer) / inertia;
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
                        if (NPC.velocity.Y > -25)
                        {
                            NPC.velocity.Y -= accelUp;
                        }
                        NPC.velocity.X *= 0.97f;
                        if (NPC.velocity.Y <= -25 && timer > 40)
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
                        NPC.velocity.Y += gravity;
                        NPC.velocity.X += Math.Sign(player.Center.X - NPC.Center.X) * xTracking;
                        if ((timer < 60 * 5 && Collision.SolidCollision(NPC.position, NPC.width, NPC.height)) || (timer > 60 * 5 && timer < 60 * 6)) //end attack on collision or after safety max time
                        {
                            NPC.velocity /= 4;
                            SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                            Main.LocalPlayer.Calamity().GeneralScreenShakePower = 20f;
                            timer = 60 * 6;
                        }
                        if (timer >= 60 * 6)
                        {
                            const int ShotCount = 10;
                            const int MaxShotSpeed = 14;
                            for (int side = -1; side < 2; side += 2)
                            {
                                for (int c = 0; c < 3; c++)
                                {
                                    float i = Main.rand.NextFloat(0, 12f);
                                    float j = Main.rand.NextFloat(1, 3f);
                                    float speed = MaxShotSpeed * (float)(i + 1) / ShotCount;
                                    Vector2 dir = (-Vector2.UnitY).RotatedBy(side * MathHelper.Pi / 9.85f);
                                    float randfac = MathHelper.Pi / 12f;
                                    float randrot = Main.rand.NextFloat(-randfac, randfac);
                                    float offset = randfac * NPC.width / 5f;
                                    Vector2 vel = speed * dir.RotatedBy(randrot * j);

                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (Vector2.UnitX * side * (offset + (NPC.width / 3))), vel, ModContent.ProjectileType<GreatSandBlast>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 2f);
                                }
                            }

                            
                            if (timer >= 60 * 6 + 8)
                            {
                                timer = 0;
                                attackStep = 0;
                                DoSlam = false;
                                CanDoSlam = false;
                                //IncrementCycle(NPC);
                                return;
                            }
                        }
                    }
                    break;
            }
            timer++;
        }
        public float[] lungeInfo = [0, 0, 0, 0, 0];
        //Sets values for doing the lunge attack with configureable projectiles to accompany the attack
        //times is the number of times to lunge
        //blasts is the little sand projs when he comes out of the ground
        //chunk is the spike ball that splits into blasts
        // water projs fall straight down


        public void SetupLunge(NPC NPC, bool blasts, bool chunk, bool water, Vector2 lungePos, Vector2 targetPos, Vector2 endPos)
        {

            if (lungeInfo[4] == 1)
            {
                PrepareLunge(NPC, lungePos);
                return;
            }
            if (lungeInfo[4] == 2)
            {
                DoLunge(NPC, targetPos, endPos);
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
            NetSync(NPC);
        }
        //move to bottom right/left to be in position for lunge
        public void PrepareLunge(NPC NPC, Vector2 lungePos)
        {
            Player target = Main.player[NPC.target];
            Vector2 offset = lungePos;
            if (target.Center.X > NPC.Center.X) offset.X = -offset.X;
            int speed = WorldSavingSystem.MasochistModeReal ? 17 : 15;
            float lerp = WorldSavingSystem.MasochistModeReal ? 0.07f : 0.05f;
            NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center + offset - NPC.Center).SafeNormalize(Vector2.Zero) * speed, lerp);
            if (NPC.Distance(target.Center + offset) <= 50)
                lungeInfo[0]++;
            if (lungeInfo[0] == 10)
            {

                lungeInfo[0] = 0;
                lungeInfo[4] = 2;
                NetSync(NPC);
            }
        }
        public void DoLunge(NPC NPC, Vector2 targetPos, Vector2 endPos)
        {

            //play sound at start only once
            if (lungeInfo[0] == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/DesertScourgeRoar") with { Volume = 0.5f });
                lungeInfo[0]++;

            }
            Player target = Main.player[NPC.target];
            //fire blasts first frame ds can see the player
            if (Collision.CanHit(NPC, target) && lungeInfo[0] == 1 && lungeInfo[1] == 1)
            {
                lungeInfo[0]++;
                int j = -4;
                if (targetPos.X == 0) j = -2;
                if (DLCUtils.HostCheck)
                    for (int i = j; i < (-j) + 1; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2 + MathHelper.ToRadians(i * 10)) * 15, ModContent.ProjectileType<GreatSandBlast>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    }
            }
            if (NPC.velocity.X < 0)
            {
                targetPos.X = -targetPos.X;
                endPos.X = -endPos.X;
            }
            //if has ever been close to the position directly above the player, move downward and to the side of the player
            if (NPC.Distance(target.Center + targetPos) < 100 || lungeInfo[0] > 3)
            {
                if (NPC.velocity.X > 0)
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center + endPos - NPC.Center).SafeNormalize(Vector2.Zero) * 20, 0.08f);
                else
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center + endPos - NPC.Center).SafeNormalize(Vector2.Zero) * 20, 0.08f);
                }
                if (lungeInfo[0] == 2 && lungeInfo[2] == 1)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -8), ModContent.ProjectileType<SandChunk>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                }
                lungeInfo[0]++;
            }
            //move to directly above the player (slightly offset to avoid turning around)
            else
            {

                NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center + targetPos - NPC.Center).SafeNormalize(Vector2.Zero) * 20, 0.1f);

            }
            if (lungeInfo[3] > 0)
            {
                lungeInfo[3]++;
                if (lungeInfo[3] % 15 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item72, NPC.Center);
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 7), ModContent.ProjectileType<ScourgeSandstream>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                }
            }
            //if has been moving downard and to the side of the player (ending the lung), truely end the attack. Repeat the attack if lungeinfo 1 is not zero.
            if (lungeInfo[0] > 60)
            {

                IncrementCycle(NPC);
                lungeInfo = [0, 0, 0, 0, 0];

            }
        }
        public void IncrementCycle(NPC NPC)
        {
            AttackIndex++;
            if (AttackIndex >= attackCycle.Length - 1 || attackCycle[(int)AttackIndex] < 0)
            {
                AttackIndex = 0;
            }
            CanDoSlam = true;

            if (NPC.localAI[2] == 2f)
                NPC.localAI[2] = 0f; // Can summon nuisances
            NPC.netUpdate = true;
            NetSync(NPC);
        }
        bool canSee = false;
        public void WormMovement(NPC NPC, bool collision)
        {
            Player playerTarget = Main.player[NPC.target];
            if (!canSee && Collision.CanHit(NPC, playerTarget) && NPC.GetLifePercent() <= 0.5f)
            {
                if (DLCUtils.HostCheck)
                    for (int i = -3; i < 4; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2 + MathHelper.ToRadians(i * 10)) * 10, ModContent.ProjectileType<GreatSandBlast>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    }
                canSee = true;
            }
            if (!Collision.CanHit(NPC, playerTarget))
            {
                canSee = false;
            }
            float speed = 20;
            // acceleration is exactly what it sounds like. The speed at which this NPC accelerates.
            float acceleration = 0.2f;

            float targetXPos, targetYPos;



            Vector2 forcedTarget = new Vector2(10000, 10000);
            if (NPC.target >= 0 && Main.player[NPC.target] != null && Main.player[NPC.target].active && !Main.player[NPC.target].dead)
                forcedTarget = playerTarget.Center;
            // Using a ValueTuple like this allows for easy assignment of multiple values
            (targetXPos, targetYPos) = (forcedTarget.X, forcedTarget.Y);

            // Copy the value, since it will be clobbered later
            Vector2 NPCCenter = NPC.Center;

            float targetRoundedPosX = (float)((int)(targetXPos / 16f) * 16);
            float targetRoundedPosY = (float)((int)(targetYPos / 16f) * 16);
            NPCCenter.X = (float)((int)(NPCCenter.X / 16f) * 16);
            NPCCenter.Y = (float)((int)(NPCCenter.Y / 16f) * 16);
            float dirX = targetRoundedPosX - NPCCenter.X;
            float dirY = targetRoundedPosY - NPCCenter.Y;

            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);

            // If we do not have any type of collision, we want the NPC to fall down and de-accelerate along the X axis.
            if (!collision)
            {
                NPC.TargetClosest(true);

                // Constant gravity of 0.11 pixels/tick
                NPC.velocity.Y += 0.17f;

                // Ensure that the NPC does not fall too quickly
                if (NPC.velocity.Y > speed)
                    NPC.velocity.Y = speed;

                // The following behaviour mimicks vanilla worm movement
                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4f)
                {
                    // Velocity is sufficiently fast, but not too fast
                    if (NPC.velocity.X < 0.0f)
                        NPC.velocity.X -= acceleration * 1.1f;
                    else
                        NPC.velocity.X += acceleration * 1.1f;
                }
                else if (NPC.velocity.Y == speed)
                {
                    // NPC has reached terminal velocity
                    if (NPC.velocity.X < dirX)
                        NPC.velocity.X += acceleration;
                    else if (NPC.velocity.X > dirX)
                        NPC.velocity.X -= acceleration;
                }
                else if (NPC.velocity.Y > 4)
                {
                    if (NPC.velocity.X < 0)
                        NPC.velocity.X += acceleration * 0.9f;
                    else
                        NPC.velocity.X -= acceleration * 0.9f;
                }
            }
            else
            {
                // Else we want to play some audio (soundDelay) and move towards our target.
                if (NPC.soundDelay == 0)
                {
                    // Play sounds quicker the closer the NPC is to the target location
                    float num1 = length / 40f;

                    if (num1 < 10)
                        num1 = 10f;

                    if (num1 > 20)
                        num1 = 20f;

                    NPC.soundDelay = (int)num1;

                    SoundEngine.PlaySound(SoundID.WormDig, NPC.position);
                }

                float absDirX = Math.Abs(dirX);
                float absDirY = Math.Abs(dirY);
                float newSpeed = speed / length;
                dirX *= newSpeed;
                dirY *= newSpeed;

                if ((NPC.velocity.X > 0 && dirX > 0) || (NPC.velocity.X < 0 && dirX < 0) || (NPC.velocity.Y > 0 && dirY > 0) || (NPC.velocity.Y < 0 && dirY < 0))
                {
                    // The NPC is moving towards the target location
                    if (NPC.velocity.X < dirX)
                        NPC.velocity.X += acceleration;
                    else if (NPC.velocity.X > dirX)
                        NPC.velocity.X -= acceleration;

                    if (NPC.velocity.Y < dirY)
                        NPC.velocity.Y += acceleration;
                    else if (NPC.velocity.Y > dirY)
                        NPC.velocity.Y -= acceleration;

                    // The intended Y-velocity is small AND the NPC is moving to the left and the target is to the right of the NPC or vice versa
                    if (Math.Abs(dirY) < speed * 0.2 && ((NPC.velocity.X > 0 && dirX < 0) || (NPC.velocity.X < 0 && dirX > 0)))
                    {
                        if (NPC.velocity.Y > 0)
                            NPC.velocity.Y += acceleration * 2f;
                        else
                            NPC.velocity.Y -= acceleration * 2f;
                    }

                    // The intended X-velocity is small AND the NPC is moving up/down and the target is below/above the NPC
                    if (Math.Abs(dirX) < speed * 0.2 && ((NPC.velocity.Y > 0 && dirY < 0) || (NPC.velocity.Y < 0 && dirY > 0)))
                    {
                        if (NPC.velocity.X > 0)
                            NPC.velocity.X = NPC.velocity.X + acceleration * 2f;
                        else
                            NPC.velocity.X = NPC.velocity.X - acceleration * 2f;
                    }
                }
                else if (absDirX > absDirY)
                {
                    // The X distance is larger than the Y distance.  Force movement along the X-axis to be stronger
                    if (NPC.velocity.X < dirX)
                        NPC.velocity.X += acceleration * 1.1f;
                    else if (NPC.velocity.X > dirX)
                        NPC.velocity.X -= acceleration * 1.1f;

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                    {
                        if (NPC.velocity.Y > 0)
                            NPC.velocity.Y += acceleration;
                        else
                            NPC.velocity.Y -= acceleration;
                    }
                }
                else
                {
                    // The X distance is larger than the Y distance.  Force movement along the X-axis to be stronger
                    if (NPC.velocity.Y < dirY)
                        NPC.velocity.Y += acceleration * 1.1f;
                    else if (NPC.velocity.Y > dirY)
                        NPC.velocity.Y -= acceleration * 1.1f;

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                    {
                        if (NPC.velocity.X > 0)
                            NPC.velocity.X += acceleration;
                        else
                            NPC.velocity.X -= acceleration;
                    }
                }
            }

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            // Some netupdate stuff (multiplayer compatibility).
            if (collision)
            {
                if (NPC.localAI[0] != 1)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 1f;
            }
            else
            {
                if (NPC.localAI[0] != 0)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 0f;
            }

            // Force a netupdate if the NPC's velocity changed sign and it was not "just hit" by a player
            if (((NPC.velocity.X > 0 && NPC.oldVelocity.X < 0) || (NPC.velocity.X < 0 && NPC.oldVelocity.X > 0) || (NPC.velocity.Y > 0 && NPC.oldVelocity.Y < 0) || (NPC.velocity.Y < 0 && NPC.oldVelocity.Y > 0)) && !NPC.justHit)
                NPC.netUpdate = true;
        }
        public void TooFarCheck(NPC NPC, ref bool collision)
        {
            if (!collision)
            {
                Rectangle hitbox = NPC.Hitbox;

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
        public bool CheckCollision(NPC NPC)
        {
            int minTilePosX = (int)(NPC.Left.X / 16) - 1;
            int maxTilePosX = (int)(NPC.Right.X / 16) + 2;
            int minTilePosY = (int)(NPC.Top.Y / 16) - 1;
            int maxTilePosY = (int)(NPC.Bottom.Y / 16) + 2;

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

                        if (NPC.Right.X > tileWorld.X && NPC.Left.X < tileWorld.X + 16 && NPC.Bottom.Y > tileWorld.Y && NPC.Top.Y < tileWorld.Y + 16)
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

        public void ManageMusicFade(bool fade)
        {
            if (fade)
            {
                Main.musicFade[Main.curMusic] = MathHelper.Lerp(Main.musicFade[Main.curMusic], 0.6f, 0.05f);
            }
            else
            {
                Main.musicFade[Main.curMusic] = MathHelper.Lerp(Main.musicFade[Main.curMusic], 1, 0.01f);
            }
        }
    }

    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DSBodyBuff : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<DesertScourgeBody>();
        public override void SetDefaults()
        {
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 25000000;
            }
            else
                NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 1.125f);
            NPC.damage = 30;
        }
        public static List<int> PierceResistExclude =
        [
            ModContent.ProjectileType<SproutingAcorn>()
        ];
        public void NullCoiledDamage(NPC.HitModifiers modifiers)
        {
            if (Main.npc.Count(n => n.active && n.type == NPC.type && n.Distance(NPC.Center) < NPC.width * 0.75) > 4)
            {
                modifiers.Null();
            }
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            NullCoiledDamage(modifiers);

        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            NullCoiledDamage(modifiers);
            if (projectile.type == ProjectileID.SporeCloud)
            {
                modifiers.FinalDamage.Base = 1;
            }
            if (projectile.maxPenetrate > 1 || projectile.maxPenetrate < 0)
                modifiers.FinalDamage *= 0.5f;
            DestroyerSegment.PierceResistance(projectile, ref modifiers);
        }
        public override bool PreAI()
        {
            NPC.netUpdate = true; //fuck you worm mp code
            return true;
        }
        public override void PostAI()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHead>()))
            {
                NPC.defense = 30;
            }
            else
            {
                NPC.defense = 10;
            }
        }
        public override void UpdateLifeRegen(ref int damage)
        {
            if (NPC.lifeRegen < 0)
            {
                NPC.lifeRegen = (int)Math.Round(NPC.lifeRegen / 4f);
            }
        }
    }
}
