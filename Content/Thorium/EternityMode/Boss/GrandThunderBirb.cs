using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using ThoriumMod.NPCs.BossTheGrandThunderBird;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using FargowiltasSouls.Core.ItemDropRules.Conditions;
using FargowiltasSouls.Core.ItemDropRules;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories;
using FargowiltasCrossmod.Content.Thorium.Items.Weapons;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using ThoriumMod.Projectiles.Boss;

namespace FargowiltasCrossmod.Content.Thorium.EternityMode.Boss
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GrandThunderBirb : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(-1);
        //public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<TheGrandThunderBird>());

        internal enum AIMode : byte
        {
            Dash,
            Storm,
            Diamond,
            Circle,
            Follow,
            FollowC,
            FollowS,
        }

        AIMode currentMode;

        /* 
         * ai[0]: animation state
         * ai[1]: attack timer
         * Mode:   Follow       | Dash          | Storm         | Diamond 
         * ai[2]:  zap timer    | wait timer    | cloud timer   | attack center X
         * ai[3]:  unused       | orb drop timer|               | attack center Y
         * Lai[0]: unused       | unused        | range timer   | attack state
         * Lai[1]:                                              | attack time
         * Lai[2]:                                              | 
         */

        public bool hasDashed;
        public bool dashDir;
        public bool wasInP2;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            currentMode = AIMode.Follow;
            npc.ai[1] = 600f;
            wasInP2 = false;
        }

        private void CycleMode(NPC npc)
        {
            switch (currentMode)
            {
                case AIMode.Follow:
                    currentMode = AIMode.Dash;
                    dashDir = npc.Center.X - Main.player[npc.target].Center.X < 0;
                    hasDashed = false;
                    npc.ai[1] = 1600f;
                    npc.ai[2] = -120f;
                    break;
                case AIMode.Dash:
                    currentMode = AIMode.Storm;
                    npc.ai[1] = 900f;
                    npc.ai[2] = -1;
                    npc.localAI[0] = 0f;
                    break;
                case AIMode.Storm:
                    int cloudType = ModContent.ProjectileType<GTBCloud>();
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (proj.active && proj.type == cloudType)
                        {
                            proj.timeLeft = (int)MathF.Min(proj.timeLeft, 100);
                        }
                    }
                    //if (npc.life <= npc.lifeMax / 2)
                    //{
                    //    currentMode = AIMode.Diamond;
                    //    npc.ai[1] = 1200f;
                    //    npc.localAI[0] = 0f;
                    //}
                    //else
                    //{
                        currentMode = AIMode.Follow;
                        npc.ai[1] = 600f;
                    //}
                    break;
                default:
                    currentMode = AIMode.Follow;
                    npc.ai[1] = 600f; 
                    break;
            }
        }

        public override void SafePostAI(NPC npc)
        {
            if (--npc.ai[1] <= 0)
            {
                CycleMode(npc);
            }

            //if (npc.life <= npc.lifeMax / 2 && !wasInP2)
            //{
            //    int cloudType = ModContent.ProjectileType<GTBCloud>();
            //    for (int i = 0; i < Main.maxProjectiles; i++)
            //    {
            //        Projectile proj = Main.projectile[i];
            //        if (proj.active && proj.type == cloudType)
            //        {
            //            proj.timeLeft = (int)MathF.Min(proj.timeLeft, 100);
            //        }
            //    }
            //    currentMode = AIMode.Diamond;
            //    npc.ai[1] = 1200f;
            //    npc.localAI[0] = 0f;
            //    wasInP2 = true;
            //}
        }

        public int rangePunishVisualCD;

        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                npc.TargetClosest(true);
            }

            Player target = Main.player[npc.target];
            if (target.dead)
            {
                npc.EncourageDespawn(1);
            }

            switch (currentMode)
            {
                case AIMode.Follow:
                    {
                        npc.spriteDirection = MathF.Sign(npc.velocity.X);
                            //npc.velocity = new(4f * npc.direction, MathF.Sign(target.Center.Y - npc.Center.Y - 240) * 2f);
                            if (++npc.ai[2] >= 90f)
                        {
                            npc.ai[2] = -20f;

                            npc.ai[0] = 1;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int zapType = ModContent.ProjectileType<GrandThunderBirdZap>();
                                var source = npc.GetSource_FromAI();
                                int safeZoneMiddle = Main.rand.Next(160, 320) * (Main.rand.NextBool(2) ? -1 : 1);
                                for (int j = 0; j < 24; j++)
                                {
                                    float offset = Main.rand.Next(-600, 600);
                                    if (MathF.Abs(offset - safeZoneMiddle) < 80f) continue;
                                    Projectile.NewProjectile(source, target.Center.X + offset, target.Center.Y - 800f + Main.rand.Next(-30, 30), 0f, 10f, zapType, 12, 0f, Main.myPlayer, 0f, 0f, 0f);
                                }
                                Projectile.NewProjectile(source, target.Center.X, target.Center.Y - 800f + Main.rand.Next(-30, 30), 0f, 8f, zapType, 12, 0f, Main.myPlayer, 0f, 0f, 0f);
                                float speedX = -0.05f * Utils.ToDirectionInt(npc.velocity.X > 0f);
                                Projectile.NewProjectile(source, npc.Center.X, npc.Center.Y - 34f, speedX, 0f, ModContent.ProjectileType<ThunderBirdScreech>(), 0, 0f, Main.myPlayer, npc.rotation, 0f, 0f);

                                //Projectile.NewProjectile(source, npc.Center + Vector2.UnitX * 48f, Vector2.Zero, ModContent.ProjectileType<Projectiles.KluexOrb>(), 15, 3f, target.whoAmI, 0f, 180f);
                                //Projectile.NewProjectile(source, npc.Center - Vector2.UnitX * 48f, Vector2.Zero, ModContent.ProjectileType<Projectiles.KluexOrb>(), 15, 3f, target.whoAmI, 0f, 140f);
                                //Projectile.NewProjectile(source, npc.Center - Vector2.UnitY * 48f, Vector2.Zero, ModContent.ProjectileType<Projectiles.KluexOrb>(), 15, 3f, target.whoAmI, 0f, 100f);
                                }
                                npc.TargetClosest(true);
                            }
                        if (npc.ai[2] < 0f)
                        {
                            npc.velocity = Vector2.Zero;
                        }
                        else
                        {
                            npc.ai[0] = 0;
                        }

                        if (target.Center.X + 850f < npc.Center.X)
                        {
                            npc.velocity.X = ((npc.velocity.X < 0f) ? -6f : -4f);
                        }
                        if (target.Center.X - 850f > npc.Center.X)
                        {
                            npc.velocity.X = ((npc.velocity.X < 0f) ? 6f : 4f);
                        }

                        if (target.Center.Y < npc.Center.Y + 300f)
                        {
                            npc.velocity.Y -= (npc.velocity.Y > 0f) ? 0.8f : 0.07f;
                        }
                        if (target.Center.Y > npc.Center.Y + 300f)
                        {
                            npc.velocity.Y += (npc.velocity.Y < 0f) ? 0.8f : 0.07f;
                        }

                        npc.rotation = npc.velocity.X * 0.05f;
                        if (target.Center.X < npc.Center.X && npc.velocity.X > -4f)
                        {
                            npc.velocity.X -= 0.1f;
                            if (npc.velocity.X > 4f)
                            {
                                npc.velocity.X -= 0.1f;
                            }
                            else if (npc.velocity.X > 0f)
                            {
                                npc.velocity.X += 0.05f;
                            }
                            if (npc.velocity.X < -4f)
                            {
                                npc.velocity.X = -4f;
                            }
                        }
                        else if (target.Center.X > npc.Center.X && npc.velocity.X < 4f)
                        {
                            npc.velocity.X += 0.1f;
                            if (npc.velocity.X < -4f)
                            {
                                npc.velocity.X += 0.1f;
                            }
                            else if (npc.velocity.X < 0f)
                            {
                                npc.velocity.X -= 0.05f;
                            }
                            if (npc.velocity.X > 4f)
                            {
                                npc.velocity.X = 4f;
                            }
                        }
                        break;
                    }
                case AIMode.Dash:
                    {
                        Vector2 seekPos = target.Center + Vector2.UnitX * 800f * (dashDir ? -1 : 1);
                        npc.direction = dashDir ? 1 : -1;
                        npc.spriteDirection = npc.direction;
                        //if (!dashDir) seekPos.Y += 48;
                        if (++npc.ai[2] < 0f)
                        {
                            //float dist = npc.Distance(seekPos);
                            npc.velocity = npc.DirectionTo(seekPos) * 8;
                            npc.velocity *= (120 + npc.ai[2]) / 30f;

                            npc.ai[0] = 0;
                            if (npc.Distance(seekPos) <= 16f || npc.ai[2] == -1f)
                            {
                                npc.ai[2] = 0f;
                                npc.Center = seekPos;
                            }
                        }
                        else
                        {
                            const int dashSpeed = 24;
                            if (npc.ai[2] >= 90f)
                            {
                                npc.velocity = new Vector2(dashDir ? dashSpeed : -dashSpeed, 0f);
                                npc.ai[0] = 3;

                                //if (npc.ai[2] % 2 == 0) Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KluexOrb>(), 15, 3f, target.whoAmI, 0, 300);

                                if (npc.ai[2] - 90f >= 1600 / dashSpeed)
                                {
                                    npc.ai[0] = 0f;
                                    if (hasDashed)
                                    {
                                        npc.ai[1] = 0f;
                                        break;
                                    }

                                    npc.ai[2] = -120;
                                    dashDir = !dashDir;
                                    hasDashed = true;
                                }
                            }
                            else
                            {
                                if (npc.ai[2] >= 75)
                                {
                                    npc.velocity = Vector2.Zero;
                                }
                                else
                                {
                                    seekPos.Y += target.velocity.Y * (800f / dashSpeed);
                                    npc.velocity = Vector2.Zero;
                                    npc.Center = new(seekPos.X, MathHelper.Lerp(npc.Center.Y, seekPos.Y, 0.1f));
                                }
                            }
                        }
                        break;
                    }
                case AIMode.Storm:
                    {
                        Vector2 seekPos = target.Center - Vector2.UnitY * 450f;
                        if (npc.ai[2] == -1)
                        {
                            npc.velocity = npc.DirectionTo(seekPos) * 8;

                            if (npc.Distance(seekPos) < 16f)
                            {
                                npc.velocity = Vector2.Zero;
                                npc.ai[0] = 1;
                                npc.ai[2] = 0;
                            }
                        }
                        if (npc.ai[2] >= 0)
                        {
                            npc.ai[0] = 0;
                            if (++npc.ai[2] == 30f && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                npc.ai[2] = 0f;

                                Vector2 cloudVel = Main.rand.NextVector2CircularEdge(2, 0.125f);
                                cloudVel.Y = MathF.Abs(cloudVel.Y);
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.UnitY * 128f, cloudVel, ModContent.ProjectileType<GTBCloud>(), 20, 1.5f, Main.myPlayer, target.whoAmI);
                            }
                        }

                        if (Main.netMode != NetmodeID.Server)
                        {
                            npc.localAI[0]--;
                            int targetXDist = (int)MathF.Abs(Main.LocalPlayer.Center.X - npc.Center.X);
                            if (targetXDist > 1000)
                            {
                                if (npc.localAI[0] <= 0)
                                {
                                    npc.localAI[0] = 90;
                                    float speedX = -0.05f * Utils.ToDirectionInt(npc.velocity.X > 0f);
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), Main.LocalPlayer.Center.X, Main.LocalPlayer.Center.Y - 34f, speedX, 0f, ModContent.ProjectileType<ThunderBirdScreech>(), 0, 0f, Main.myPlayer, npc.rotation, 0f, 0f);
                                }
                                npc.ai[0] = 1f;
                                if (Main.rand.NextBool((int)MathF.Max(30 - ((npc.localAI[0]) / 30), 1)))
                                {
                                    Vector2 random = Main.rand.NextVector2CircularEdge(128f, 128f);
                                    if (MathF.Sign(random.X) != MathF.Sign(Main.LocalPlayer.Center.X - npc.Center.X)) random.X *= -1f;
                                    Vector2 spawnPos = Main.LocalPlayer.Center + Main.LocalPlayer.velocity * 15f + random;

                                    Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPos, Vector2.Zero, ModContent.ProjectileType<KluexOrb>(), 25, 3f, Main.myPlayer, 0, 180f);
                                }
                            }
                        }

                        break;
                    }
                case AIMode.Diamond:
                    {
                        const float playerOffset = 600f;
                        const float offsetDiagonal = 840f;
                        const float diamondSpeed = 12f;
                        if (npc.ai[1] == 1200f)
                        {
                            npc.velocity = Vector2.Zero;
                            npc.ai[0] = 1f;
                            float speedX = -0.05f * Utils.ToDirectionInt(npc.velocity.X > 0f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y - 34f, speedX, 0f, ModContent.ProjectileType<ThunderBirdScreech>(), 0, 0f, Main.myPlayer, npc.rotation, 0f, 0f);
                        }
                        if (npc.ai[1] > 1140f)
                        {
                            npc.velocity = Vector2.Zero;
                            npc.alpha = (int)((1200f - npc.ai[1]) * 4.25f);
                            if (npc.ai[1] >= 1180f)
                            {
                                npc.ai[0] = 0f;
                            }
                        }
                        else if (npc.ai[1] == 1140f)
                        {
                            npc.velocity = Vector2.Zero;
                            npc.Center = target.Center - Vector2.UnitX * 600f;
                            npc.ai[2] = target.Center.X;
                            npc.ai[3] = target.Center.Y;
                            npc.netUpdate = true;
                        }
                        else if (npc.ai[1] >= 1080f)
                        {
                            npc.velocity = Vector2.Zero;
                            npc.alpha = (int)((npc.ai[1] - 1080f) * 4.25f);
                        }

                        if (npc.ai[1] <= 1050f)
                        {
                            if (npc.ai[1] == 1050f)
                            {
                                npc.localAI[0] = 1f;
                                npc.localAI[1] = 1050f;
                                npc.localAI[2] = 0f;
                            }

                            if (npc.localAI[2] == 0f && npc.localAI[0] == 1)
                            {
                                switch ((int)((npc.localAI[1] - npc.ai[1]) / (playerOffset / diamondSpeed)))
                                {
                                    case 0:
                                        npc.velocity = new Vector2(1, -1) * diamondSpeed;
                                        break;
                                    case 1:
                                        npc.velocity = new Vector2(1, 1) * diamondSpeed;
                                        break;
                                    case 2:
                                        npc.velocity = new Vector2(-1, 1) * diamondSpeed;
                                        break;
                                    case 3:
                                        npc.velocity = new Vector2(-1, -1) * diamondSpeed;
                                        break;
                                    case 4:
                                        npc.velocity = Vector2.Normalize(new(180, -420)) * diamondSpeed;
                                        npc.localAI[0]++;
                                        npc.localAI[2] = 1f;
                                        break;
                                }
                            }

                            if (npc.localAI[2] == 0f && npc.localAI[0] == 2)
                            {
                                switch ((int)(npc.localAI[1] - (int)npc.ai[1]) / (int)(offsetDiagonal / (diamondSpeed * 1.4f)))
                                {
                                    case 0:
                                        npc.velocity = new Vector2(1, 0) * diamondSpeed * 1.4f;
                                        break;
                                    case 1:
                                        npc.velocity = new Vector2(0, 1) * diamondSpeed * 1.4f;
                                        break;
                                    case 2:
                                        npc.velocity = new Vector2(-1, 0) * diamondSpeed * 1.4f;
                                        break;
                                    case 3:
                                        npc.velocity = new Vector2(0, -1) * diamondSpeed * 1.4f;
                                        break;
                                    case 4:
                                        npc.velocity = Vector2.Normalize(new(420, -180)) * diamondSpeed;
                                        npc.localAI[0]++;
                                        npc.localAI[2] = 2f;
                                        break;
                                }
                            }

                            if (npc.localAI[2] == 0f && npc.localAI[0] == 3)
                            {
                                switch ((int)((npc.localAI[1] - npc.ai[1]) / (playerOffset / diamondSpeed)))
                                {
                                    case 0:
                                        npc.velocity = new Vector2(1, 1) * diamondSpeed;
                                        break;
                                    case 1:
                                        npc.velocity = new Vector2(-1, 1) * diamondSpeed;
                                        break;
                                    case 2:
                                        npc.velocity = new Vector2(-1, -1) * diamondSpeed;
                                        break;
                                    case 3:
                                        npc.velocity = new Vector2(1, -1) * diamondSpeed;
                                        break;
                                    case 4:
                                        if (target.Distance(new(npc.ai[2], npc.ai[3])) > playerOffset)
                                        {
                                            npc.ai[1] = 0f;
                                        }
                                        else
                                        {
                                            npc.ai[1] = 300f;
                                            npc.localAI[2] = -1f;
                                        }
                                        npc.velocity = Vector2.Zero;
                                        break;
                                }
                            }

                            //if (npc.localAI[2] == 0f && npc.ai[1] % 2 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                            //{
                            //    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KluexOrb>(), 12, 2f, Main.myPlayer, 0f, 420f, 0.75f);
                            //}

                            if (npc.localAI[2] == 1f)
                            {
                                if (npc.Distance(new(npc.ai[2] - 420f, npc.ai[3] - 420f)) < 32f)
                                {
                                    npc.velocity = Vector2.Zero;
                                    npc.localAI[1] = npc.ai[1];
                                    npc.localAI[2] = 0f;
                                    npc.Center = new(npc.ai[2] - 420f, npc.ai[3] - 420f);
                                }
                            }
                            else if (npc.localAI[2] == 2f)
                            {
                                if (npc.Distance(new(npc.ai[2], npc.ai[3] - 600f)) < 32f)
                                {
                                    npc.velocity = Vector2.Zero;
                                    npc.localAI[1] = npc.ai[1];
                                    npc.localAI[2] = 0f;
                                    npc.Center = new(npc.ai[2], npc.ai[3] - 600f);
                                }
                            }
                        }
                        else
                        {
                            npc.localAI[0] = 0f;
                        }
                        
                        break;
                    }
            }
            return false;
        }

        public override void SafeOnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            base.SafeOnHitByItem(npc, player, item, hit, damageDone);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            IItemDropRule BossDrop(int item) => new DropBasedOnEMode(ItemDropRule.Common(item, 3), ItemDropRule.Common(item, 10));

            LeadingConditionRule emodeRule = new(new EModeDropCondition());
            emodeRule.OnSuccess(FargowiltasSouls.FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<TempleCore>()));
            npcLoot.Add(emodeRule);
            npcLoot.Add(BossDrop(ModContent.ItemType<KluexStaff>()));
        }
    }
}
