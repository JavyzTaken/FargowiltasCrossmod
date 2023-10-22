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

namespace FargowiltasCrossmod.Content.Thorium.EternityMode.Boss
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GrandThunderBirb : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<TheGrandThunderBird>());

        internal enum AIMode
        {
            None,
            KluexMoving,
            HorizontalDashs,
            //Storm,
        }

        AIMode currentMode;
        List<AIMode> AvaliableModes = new();
        bool p1;
        int hatlingSpawnTimer;

        // Kluex attack
        int redAttackNum;
        const int RedAttSpeed = 48;
        Vector2? NextPosition = null;
        Vector2 LastPosition;

        // Dashes attack
        bool firstDash;
        bool waitingForDash;
        int dashWaitTimer;
        int dashTimer;

        // Storm attack

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);
            p1 = true;
            CycleMode();
        }

        private void CycleMode()
        {
            if (AvaliableModes.Count == 0)
            {
                AvaliableModes = new()
                {
                    AIMode.KluexMoving,
                    AIMode.HorizontalDashs,
                };
            }
            currentMode = AvaliableModes[Main.rand.Next(0, AvaliableModes.Count)];
            AvaliableModes.Remove(currentMode);
            switch (currentMode)
            {
                case AIMode.KluexMoving:
                    NextPosition = null;
                    redAttackNum = 24;
                    break;
                case AIMode.HorizontalDashs:
                    firstDash = true;
                    waitingForDash = true;
                    dashWaitTimer = 60;
                    break;
            }
        }

        const float MinDistSQFromPlayer = 16384f;
        const float MinDistSQFromLastPos = 36864f;
        private Vector2 GetNextPosition(Player target, NPC npc)
        {
            Vector2 vec = new(Main.rand.NextFloat(target.Center.X - Main.ScreenSize.X / 3, target.Center.X + Main.ScreenSize.X / 3),
                              Main.rand.NextFloat(target.Center.Y - Main.ScreenSize.Y / 2, target.Center.Y));
            if (vec.DistanceSQ(target.Center) <= MinDistSQFromPlayer || vec.DistanceSQ(npc.Center) <= MinDistSQFromLastPos) return GetNextPosition(target, npc);
            Vector2 closestPointToPlr = Vector2.Dot(target.Center - npc.Center, vec - npc.Center) / (vec - npc.Center).LengthSquared() * (vec - npc.Center);
            if (closestPointToPlr.X > (vec - npc.Center).X) return vec;
            if ((target.Center - npc.Center).LengthSquared() <= MinDistSQFromPlayer) return GetNextPosition(target, npc);
            return vec;
        }

        public override bool SafePreAI(NPC npc)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (p1 && npc.life <= npc.lifeMax * 0.5)
                {
                    // phase change
                    p1 = false;
                    for (int i = 0; i < 3; i++)
                    {
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<StormHatchling>(), Target: npc.target, ai0: npc.whoAmI);
                    }
                    hatlingSpawnTimer = 600;
                }

                if (!p1 && hatlingSpawnTimer == 0)
                {
                    hatlingSpawnTimer = npc.life <= npc.lifeMax * 0.2 ? 300 : 600;
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<StormHatchling>(), Target: npc.target, ai0: npc.whoAmI);
                }

                hatlingSpawnTimer--;
            }
            switch (currentMode)
            {
                case AIMode.KluexMoving:
                    {
                        npc.TargetClosest();
                        if (NextPosition.HasValue && npc.Center.DistanceSQ(NextPosition.Value) > 64f)
                        {
                            // go towards chosen location
                        }
                        else if (redAttackNum > 0 && npc.HasPlayerTarget && npc.HasValidTarget)
                        {
                            // choose new location if the attack is not finished
                            LastPosition = npc.Center;
                            NextPosition = GetNextPosition(Main.player[npc.target], npc);
                            npc.velocity = (NextPosition.Value - LastPosition) / RedAttSpeed;

                            // spawn projectile
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.KluexOrb>(), 16, 3f, Main.myPlayer, Projectiles.KluexOrb.GFBOrb);
                            redAttackNum--;
                        }
                        else if (redAttackNum == 0)
                        {
                            // finished spawning orbs
                            NextPosition = null;
                            npc.velocity = Vector2.Zero;
                            CycleMode();
                        }
                        npc.spriteDirection = npc.velocity.X > 0 ? 1 : -1;
                        break;
                    }
                case AIMode.HorizontalDashs:
                    {
                        // phase 1: dash once from right to left, then once from left to right, leaving orbs as it goes.
                        if (p1)
                        {
                            if (npc.HasPlayerTarget && npc.HasValidTarget)
                            {
                                Player target = Main.player[npc.target];
                                int dashDir = firstDash ? -1 : 1;
                                npc.spriteDirection = -dashDir;
                                if (waitingForDash)
                                {
                                    if (dashWaitTimer > 0)
                                    {
                                        // hover relative to player plus their vertical velocity
                                        npc.Center = target.Center + new Vector2((Main.ScreenSize.X / 2 - npc.width) * dashDir, target.velocity.Y * 16);
                                        dashWaitTimer--;
                                    }
                                    else
                                    {
                                        // initiate dash
                                        waitingForDash = false;
                                        dashTimer = Main.ScreenSize.X / 16;
                                        npc.velocity = new(16 * -dashDir, 0);
                                    }
                                }
                                else
                                {
                                    if (dashTimer > 0)
                                    {
                                        // move in dash, spawning orbs every 3 blocks
                                        if (dashTimer % 3 == 0)
                                        {
                                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.KluexOrb>(), 16, 3f, Main.myPlayer, Projectiles.KluexOrb.GFBOrb);
                                        }
                                        dashTimer--;
                                    }
                                    else
                                    {
                                        // switch dashes or attacks
                                        if (firstDash)
                                        {
                                            dashDir *= -1;
                                            firstDash = false;
                                            waitingForDash = true;
                                            dashWaitTimer = 60;
                                        }
                                        else
                                        {
                                            CycleMode();
                                        }
                                    }
                                }
                            }
                        }
                        // phase 2 version: look like its about to do normal p1 attack, then do a diamond shape around the player's position instead.
                        else
                        {
                            const int dashnum = 3;
                            if (npc.HasPlayerTarget && npc.HasValidTarget)
                            {
                                Player target = Main.player[npc.target];
                                npc.spriteDirection = -1;
                                if (waitingForDash)
                                {
                                    if (dashWaitTimer > 0)
                                    {
                                        // hover relative to player, not predictive 
                                        npc.Center = target.Center + new Vector2(Main.ScreenSize.X / dashnum, 0);
                                        dashWaitTimer--;
                                    }
                                    else
                                    {
                                        // initiate dash
                                        waitingForDash = false;
                                        dashTimer = Main.ScreenSize.X / (dashnum * 4);
                                        npc.velocity = new(-16, -16);
                                    }
                                }
                                else
                                {
                                    if (dashTimer > 0)
                                    {
                                        // move in dash, spawning orbs every 3 blocks
                                        if (dashTimer % 3 == 0)
                                        {
                                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.KluexOrb>(), 16, 3f, Main.myPlayer, Projectiles.KluexOrb.GFBOrb);
                                        }
                                        if (dashTimer % (Main.ScreenSize.X / (dashnum * 16)) == 0)
                                        {
                                            switch (dashTimer / (Main.ScreenSize.X / (dashnum * 16)))
                                            {
                                                case 3:
                                                    npc.velocity = new(-16, 16);
                                                    break;
                                                case 2:
                                                    npc.velocity = new(16, 16);
                                                    break;
                                                case 1:
                                                    npc.velocity = new(16, -16);
                                                    break;
                                            }
                                        }
                                        dashTimer--;
                                    }
                                    else
                                    {
                                        CycleMode();
                                    }
                                }
                            }
                        }

                        break;
                    }
                //case AIMode.Storm:
                //    {
                //        npc.TargetClosest();
                //        break;
                //    }
                default: break;
            }
            return false;
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
