using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using ThoriumMod.NPCs.BossTheGrandThunderBird;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Reflection;
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
using MonoMod.RuntimeDetour;

namespace FargowiltasCrossmod.Content.Thorium.EternityMode.Boss
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GrandThunderBird : EModeNPCBehaviour
    {
        //public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(-1);
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<TheGrandThunderBird>());

        private static Hook PostDrawHook;
        internal static void LoadDetour()
        {
            Type baseGTB = ModContent.Find<ModNPC>("ThoriumMod/TheGrandThunderBird").GetType();

            if (baseGTB != null)
            {
                MethodInfo postDrawMethod = baseGTB.GetMethod("PostDraw", BindingFlags.Public | BindingFlags.Instance);

                PostDrawHook = new(postDrawMethod, PostDrawDetour);
                PostDrawHook.Apply();
            }
        }

        public override void Unload()
        {
            PostDrawHook?.Undo();
        }

        private delegate void orig_PostDraw(TheGrandThunderBird self, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
        private static void PostDrawDetour(orig_PostDraw orig, TheGrandThunderBird self, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode) // dont draw the sword indicator in things like the dash, where it isnt applicable
                return;
        }

        internal enum AIMode : byte
        {
            Dash,
            Storm,
            Follow,
            NewDash,
        }

        AIMode currentMode;

        /* 
         * ai[0]: animation state
         * ai[1]: attack timer
         * 
         * Mode:   Follow       | Dash          | Storm         | NewDash
         * ai[2]:  zap timer    | wait timer    | cloud timer   | Attack state (0 - 5)
         * ai[3]:  unused       | unused        |               | Dash timer
         * Lai[0]: unused       | unused        | range timer   | unused
         * Lai[1]:                                              | 
         * Lai[2]:                                              | 
         */

        public bool hasDashed;
        public bool dashDir;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            currentMode = AIMode.Follow;
            npc.ai[1] = 400f;
        }

        public override void SetDefaults(NPC npc)
        {
            npc.width = npc.height = 60;
            npc.lifeMax = (int)MathF.Round(npc.lifeMax * 1.2f / 100f) * 100; // I like the numbers to be pretty
            npc.damage = 40;
            npc.defense = 4;
        }

        private void CycleMode(NPC npc)
        {
            npc.rotation = 0f;
            npc.color = Color.White;
            npc.damage = 40;

            switch (currentMode)
            {
                case AIMode.Follow:
                    currentMode = AIMode.Dash;
                    break;
                case AIMode.Dash:
                    currentMode = AIMode.Storm;
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

                    currentMode = AIMode.NewDash;
                    break;
                case AIMode.NewDash:
                    currentMode = AIMode.Follow;
                    break;
                default:
                    currentMode = AIMode.Follow;
                    break;
            }

            switch (currentMode)
            {
                case AIMode.Follow:
                    npc.ai[1] = 400f;
                    break;
                case AIMode.Dash:
                    dashDir = npc.Center.X - Main.player[npc.target].Center.X < 0;
                    hasDashed = false;
                    npc.ai[1] = 1600f;
                    npc.ai[2] = -90f;
                    break;
                case AIMode.Storm:
                    npc.ai[1] = 900f;
                    npc.ai[2] = -1;
                    npc.localAI[0] = 0f;
                    break;
                case AIMode.NewDash:
                    npc.ai[1] = 1200f;
                    npc.ai[2] = 0;
                    dashDir = npc.Center.X - Main.player[npc.target].Center.X < 0;
                    break;
            }
        }

        public override void OnKill(NPC npc)
        {
            int cloudType = ModContent.ProjectileType<GTBCloud>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == cloudType)
                {
                    proj.timeLeft = (int)MathF.Min(proj.timeLeft, 100);
                }
            }

            int sandStormType = ModContent.ProjectileType<GTBSandStorm>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == sandStormType)
                {
                    proj.timeLeft = (int)MathF.Min(proj.timeLeft, 90);
                }
            }
        }

        public override void SafePostAI(NPC npc)
        {
            if (--npc.ai[1] <= 0)
            {
                CycleMode(npc);
            }
        }

        public int rangePunishVisualCD;

        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                npc.TargetClosest(true);

                if (!npc.HasValidTarget)
                {
                    npc.EncourageDespawn(1);
                }
            }

            Player target = Main.player[npc.target];
            if (target.dead)
            {
                npc.EncourageDespawn(1);
            }

            switch (currentMode)
            {
                case AIMode.Follow:
                    FollowAI(npc);
                    break;
                case AIMode.Dash:
                    DashAI(npc);
                    break;
                case AIMode.Storm:
                    StormAI(npc);
                    break;
                case AIMode.NewDash:
                    NewDashAI(npc);
                    break;
            }

            //if (npc.life < npc.lifeMax * 0.5f)
            //{
            //    if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[2]++ > 600)
            //    {
            //        npc.localAI[2] = 0;
            //        NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<StormHatchling>(), npc.whoAmI);
            //    } 
            //}

            return false;
        }

        private void FollowAI(NPC npc)
        {
            Player target = Main.player[npc.target];
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
                        if (MathF.Abs(offset - safeZoneMiddle) < 80f)
                        {
                            j--;
                            continue;
                        }
                        int zap = Projectile.NewProjectile(source, target.Center.X + offset, target.Center.Y - 800f + Main.rand.Next(-30, 30), 0f, 10f, zapType, 10, 0f, Main.myPlayer, 0f, 0f, 0f);
                        Main.projectile[zap].tileCollide = false;
                    }
                    Projectile.NewProjectile(source, target.Center.X, target.Center.Y - 800f + Main.rand.Next(-30, 30), 0f, 8f, zapType, 10, 0f, Main.myPlayer, 0f, 0f, 0f);
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
        }

        private void DashAI(NPC npc)
        {
            Player target = Main.player[npc.target];
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
                            return;
                        }

                        npc.ai[2] = -60;
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
        }
       
        private void StormAI(NPC npc)
        {
            Player target = Main.player[npc.target];
            Vector2 seekPos = target.Center - Vector2.UnitY * 450f;
            if (npc.ai[2] == -1)
            {
                npc.velocity = npc.DirectionTo(seekPos) * 10;
                npc.direction = MathF.Sign(npc.velocity.X);
                npc.spriteDirection = npc.direction;

                if (npc.Distance(seekPos) < 16f)
                {
                    npc.velocity = Vector2.Zero;
                    npc.ai[0] = 1;
                    npc.ai[2] = 0;
                }
            }
            if (npc.ai[2] >= 0)
            {
                npc.color = new Color(150, 150, 150, 255); // visual indicator for lowered damage

                npc.ai[0] = 0;
                if (++npc.ai[2] == 30f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[2] = 0f;

                    Vector2 cloudVel = Main.rand.NextVector2CircularEdge(2, 0.125f);
                    cloudVel.Y = MathF.Abs(cloudVel.Y);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.UnitY * 128f, cloudVel, ModContent.ProjectileType<GTBCloud>(), 20, 1.5f, Main.myPlayer, target.whoAmI, npc.whoAmI);
                }
            }

            if (Main.netMode != NetmodeID.Server)
            {
                npc.localAI[0]--;
                int targetXDist = (int)MathF.Abs(Main.LocalPlayer.Center.X - npc.Center.X);

                if (targetXDist > 700 && npc.ai[2] != -1)
                {
                    if (npc.localAI[0] <= 0)
                    {
                        npc.localAI[0] = 90;
                        float speedX = -0.05f * Utils.ToDirectionInt(npc.velocity.X > 0f);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), Main.LocalPlayer.Center.X, Main.LocalPlayer.Center.Y - 34f, speedX, 0f, ModContent.ProjectileType<ThunderBirdScreech>(), 0, 0f, Main.myPlayer, npc.rotation, 0f, 0f);
                    }
                    npc.ai[0] = 1f;
                    if (Main.rand.NextBool((int)MathF.Max(20 - ((npc.localAI[0]) / 20), 1)))
                    {
                        Vector2 random = Main.rand.NextVector2CircularEdge(128f, 128f);
                        if (MathF.Sign(random.X) != MathF.Sign(Main.LocalPlayer.Center.X - npc.Center.X)) random.X *= -1f;
                        Vector2 spawnPos = Main.LocalPlayer.Center + Main.LocalPlayer.velocity * 15f + random;

                        Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPos, Vector2.Zero, ModContent.ProjectileType<KluexOrb>(), 12, 3f, Main.myPlayer, 0, 180f);
                    }
                }

                if (Main.LocalPlayer.Center.Y - npc.Center.Y < 160 && npc.ai[2] != -1)
                {
                    if (npc.localAI[0] <= 0)
                    {
                        npc.localAI[0] = 90;
                        float speedX = -0.05f * Utils.ToDirectionInt(npc.velocity.X > 0f);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), Main.LocalPlayer.Center.X, Main.LocalPlayer.Center.Y - 34f, speedX, 0f, ModContent.ProjectileType<ThunderBirdScreech>(), 0, 0f, Main.myPlayer, npc.rotation, 0f, 0f);
                    }
                    npc.ai[0] = 1f;
                    if (Main.rand.NextBool((int)MathF.Max(20 - ((npc.localAI[0]) / 20), 1)))
                    {
                        Vector2 random = Main.rand.NextVector2CircularEdge(128f, 128f);
                        if (MathF.Sign(random.Y) != MathF.Sign(Main.LocalPlayer.Center.Y - npc.Center.Y + 160)) random.Y *= -1f;
                        Vector2 spawnPos = Main.LocalPlayer.Center + Main.LocalPlayer.velocity * 15f + random;

                        Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPos, Vector2.Zero, ModContent.ProjectileType<KluexOrb>(), 12, 3f, Main.myPlayer, 0, 180f);
                    }
                }
            }
        }

        private void NewDashAI(NPC npc)
        {
            Player target = Main.player[npc.target];
            Projectile storm = npc.ai[2] > 0 ? Main.projectile[(int)npc.localAI[0]] : null;

            switch ((int)npc.ai[2])
            {
                case 0: // seek pos
                    Vector2 seekPos = target.Center + Vector2.UnitX * 1000f * (dashDir ? -1 : 1);
                    npc.direction = dashDir ? 1 : -1;
                    npc.spriteDirection = npc.direction;
                    float dist = npc.Distance(seekPos);

                    npc.velocity = npc.DirectionTo(seekPos) * 8;

                    npc.ai[0] = 0;
                    if (dist <= 16f)
                    {
                        npc.ai[2]++;
                        npc.ai[3] = 54f;
                        npc.velocity = new Vector2(dashDir ? 18 : -18, 0f);
                        npc.Center = seekPos;
                        npc.rotation = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.localAI[0] = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + npc.velocity * 45f - Vector2.UnitY * 64f, Vector2.Zero, ModContent.ProjectileType<GTBSandStorm>(), 12, 4f);
                        }
                    }
                    break;
                case 1: // initial dash
                    npc.ai[0] = 3;
                    storm.scale = MathHelper.SmoothStep(0, 0.5f, (60f - npc.ai[3]) / 60f);

                    if (npc.ai[3]-- <= 0)
                    {
                        npc.ai[2]++;
                        npc.velocity = new Vector2(npc.velocity.X * -1.2f, -3f);
                        //npc.velocity = npc.Center + new Vector2(dashDir ? -240 : 240, -120);
                        npc.ai[3] = 150;
                        npc.direction *= -1;
                        npc.spriteDirection = npc.direction;
                    }
                    break;
                case 2: // swurl
                    npc.ai[0] = 3;
                    storm.scale = MathHelper.SmoothStep(0.5f, 3f, (150f - npc.ai[3]) / 150f);
                    npc.velocity = new Vector2(18f * 10f * MathF.Cos(MathF.PI * (npc.ai[3] / 30f + 0.5f)) * MathF.PI / 30f * (dashDir ? 1 : -1), -2f);
                    //npc.alpha = (int)(64f * MathF.Sin((MathF.PI / 15f) * (npc.ai[3] - 7.5f)) + 64f);
                    npc.direction = MathF.Sign(npc.velocity.X);
                    npc.rotation = npc.direction * -0.3f;
                    npc.spriteDirection = npc.direction;

                    if (npc.ai[3]-- <= 0)
                    {
                        npc.ai[2] = 5;
                        npc.velocity = new Vector2(dashDir ? -0.5f : 0.5f, -0.15f);
                        npc.rotation = dashDir ? -0.3f : 0.3f;
                        //npc.velocity = new Vector2(npc.velocity.X * -1f, npc.velocity.Y);
                        npc.ai[3] = 300;
                        npc.direction *= -1;
                        npc.spriteDirection = npc.direction;
                    }
                    break;
                case 5:
                    npc.ai[0] = 0;
                    if (npc.ai[3]-- > 0)
                    {
                        float x = (300f - npc.ai[3]) / 300f;
                        storm.scale = 3f + (MathF.Sin(10f * MathF.PI * x) / 20f) + x;
                    }
                    else
                    {
                        npc.ai[1] = 0;
                    }
                    break;
                case 6: // hit by item while flapping
                    storm.scale -= (4f / 90f);
                    npc.rotation = 0f;
                    npc.ai[0] = 2;
                    npc.velocity = Vector2.Zero;
                    npc.damage = 0;
                    break;
            };
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (currentMode == AIMode.NewDash && (int)npc.ai[2] == 5)
            {
                spriteBatch.Draw(Core.ModCompatibility.ThoriumMod.Mod.Assets.Request<Texture2D>("Textures/Sword_Indicator").Value, npc.Center - screenPos, default, Color.White, 0f, new Vector2(13f, 90f), 1f, 0, 0f);
            }
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (npc.ai[2] >= 0 && currentMode == AIMode.Storm)
            {
                modifiers.FinalDamage *= 0.60f;
            }
        }

        public override void SafeOnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (currentMode == AIMode.NewDash && (int)npc.ai[2] == 5)
            {
                npc.ai[2] = 6f;
                npc.ai[3] = 90f;
                npc.ai[1] = 90f;
                npc.ai[0] = 2;
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            static IItemDropRule BossDrop(int item) => new DropBasedOnEMode(ItemDropRule.Common(item, 3), ItemDropRule.Common(item, 10));

            LeadingConditionRule emodeRule = new(new EModeDropCondition());
            emodeRule.OnSuccess(FargowiltasSouls.FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<TempleCore>()));
            npcLoot.Add(emodeRule);
            npcLoot.Add(BossDrop(ModContent.ItemType<KluexStaff>()));
        }
    }
}
