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
        //public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(-1);
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<TheGrandThunderBird>());

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
         * Mode:  Follow        | Dash          | Storm         | Diamond 
         * ai[2]: zap timer     | wait timer    |               |
         * ai[3]: unused        | orb drop timer|               |
         */

        public bool hasDashed;
        public bool dashDir;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            currentMode = AIMode.Dash;
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
                default:
                    currentMode = AIMode.Follow;
                    npc.ai[1] = 120f; 
                    break;
            }
        }

        public override void SafePostAI(NPC npc)
        {
            if (--npc.ai[1] <= 0)
            {
                CycleMode(npc);
            }
        }
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                npc.TargetClosest(true);
            }

            Player target = Main.player[npc.target];

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

                                Projectile.NewProjectile(source, npc.Center + Vector2.UnitX * 48f, Vector2.Zero, ModContent.ProjectileType<Projectiles.KluexOrb>(), 30, 3f, target.whoAmI, 0f, 180f);
                                Projectile.NewProjectile(source, npc.Center - Vector2.UnitX * 48f, Vector2.Zero, ModContent.ProjectileType<Projectiles.KluexOrb>(), 30, 3f, target.whoAmI, 0f, 140f);
                                Projectile.NewProjectile(source, npc.Center - Vector2.UnitY * 48f, Vector2.Zero, ModContent.ProjectileType<Projectiles.KluexOrb>(), 30, 3f, target.whoAmI, 0f, 100f);
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

                                if (npc.ai[2] % 2 == 0) Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<KluexOrb>(), 25, 3f, target.whoAmI, 0, 300);

                                if (npc.ai[2] - 90f >= 1600 / dashSpeed)
                                {
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
                                //npc.Center = seekPos;
                                seekPos.Y += target.velocity.Y * (800f / dashSpeed);
                                npc.velocity = Vector2.Zero;
                                npc.Center = new(seekPos.X, MathHelper.Lerp(npc.Center.Y, seekPos.Y, 0.1f));
                            }
                        }
                        break;
                    }
            }
            return false;
        }

        //public override void SafeOnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        //{
        //    base.SafeOnHitByItem(npc, player, item, hit, damageDone);
        //}

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
