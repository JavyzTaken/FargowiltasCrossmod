using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BlobEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(ModContent.NPCType<HiveBlob>(), ModContent.NPCType<HiveBlob2>());

        Vector2 SavedCenter = Vector2.Zero;
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            base.HitEffect(npc, hit);
        }
        public override void SetDefaults(NPC entity)
        {
            float healthMult = 0.5f;
            int hiveMind = CalamityGlobalNPC.hiveMind;
            if (!(hiveMind < 0 || !Main.npc[hiveMind].TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>()))
            {
                NPC owner = Main.npc[hiveMind];
                if (owner.GetGlobalNPC<HMEternity>().Phase < 2)
                    healthMult *= 4;
            }
            entity.lifeMax = (int)(entity.lifeMax * healthMult);
            base.SetDefaults(entity);

        }
        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            base.ApplyDifficultyAndPlayerScaling(npc, numPlayers, balance, bossAdjustment);
        }
        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (!WorldSavingSystem.EternityMode) return;
            int hiveMind = CalamityGlobalNPC.hiveMind;
            if (hiveMind < 0 || !Main.npc[hiveMind].TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>())
            {
                return;
            }
            NPC owner = Main.npc[hiveMind];
            if (owner.GetGlobalNPC<HMEternity>().Phase < 2)
            {
                float maxRadians = owner.GetLifePercent() <= 0.9f ? MathHelper.TwoPi : MathHelper.Pi;
                npc.ai[1] = Main.rand.NextFloat(0f, maxRadians);
            }
            
        }
        public override void OnKill(NPC npc)
        {
            /*
            if (DLCUtils.HostCheck && WorldSavingSystem.EternityMode)
            {
                int hiveMind = CalamityGlobalNPC.hiveMind;
                if (hiveMind < 0 || !Main.npc[hiveMind].TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>())
                {
                    return;
                }
                NPC owner = Main.npc[hiveMind];
                int range = 20;
                if (owner.GetLifePercent() <= 0.9f) range = 40;
                int amount = 0;
                if (owner.GetGlobalNPC<HMEternity>().Phase < 2) amount = 2;
                else if (Main.rand.NextBool()) amount = 1;
                if (DLCUtils.HostCheck)
                    for (int i = 0; i < amount; i++)
                        Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, new Vector2(0, -10).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-range, range))), ModContent.ProjectileType<OldDukeGore>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(owner.damage), 0);
            }
            */
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            int hiveMind = CalamityGlobalNPC.hiveMind;
            if (hiveMind < 0 || !Main.npc[hiveMind].TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>())
            {
                return true;
            }
            NPC owner = Main.npc[hiveMind];

            if (owner.GetGlobalNPC<HMEternity>().Phase < 2)
            {
                if (npc.HasValidTarget)
                {
                    Player target = Main.player[npc.target];
                    Asset<Texture2D> line = TextureAssets.Extra[178];

                    float opacity = 0;
                    if (npc.localAI[1] >= 420f)
                    {
                        opacity = MathHelper.Lerp(0, 1, (npc.localAI[1] - 420f) / 60f);
                    }
                    Main.EntitySpriteDraw(line.Value, npc.Center - Main.screenPosition, null, Color.Lime * opacity, npc.DirectionTo(target.Center).ToRotation(), new Vector2(0, line.Height() * 0.5f), new Vector2(0.2f, npc.scale * 4), SpriteEffects.None);
                }
                return true;
            }
            else if (owner.ai[2] != 0)
                return true;


            Asset<Texture2D> t = TextureAssets.Chains[3];
            Vector2 pos = npc.Center + ((owner.Center - npc.Center).SafeNormalize(Vector2.Zero) * 20);
            while (pos.Distance(owner.Center) > 20)
            {
                Main.EntitySpriteDraw(t.Value, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()), pos.AngleTo(owner.Center) + MathHelper.Pi / 2, t.Size() / 2, npc.scale, SpriteEffects.None);
                pos += (owner.Center - pos).SafeNormalize(Vector2.Zero) * 30;
            }
            return true;
        }
        public override void FindFrame(NPC npc, int frameHeight)
        {
            base.FindFrame(npc, frameHeight);
        }
        public override bool SafePreAI(NPC npc)
        {
            if (!FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode) return true;


            
            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
            }
            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.velocity.Y += 1;
                return false;
            }
            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
            }
            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.velocity.Y += 1;
                return false;
            }
            Player target = Main.player[npc.target];
            int hiveMind = CalamityGlobalNPC.hiveMind;
            if (hiveMind < 0 || !Main.npc[hiveMind].TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>())
            {
                npc.StrikeInstantKill();
                return false;
            }
            NPC owner = Main.npc[hiveMind];

            if (owner.GetGlobalNPC<HMEternity>().Phase < 2)
                Phase1AI(npc, owner, target);
            else
                Phase2AI(npc, owner, target);
            

            return false;

        }
        float index = -1;
        public void Phase1AI(NPC npc, NPC owner, Player target)
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            npc.damage = 0;
            npc.chaseable = true;

            if (SavedCenter == Vector2.Zero || SavedCenter.Distance(target.Center) > 1500)
                SavedCenter = target.Center;

            SavedCenter = Vector2.Lerp(SavedCenter, target.Center, 0.4f);

            int hiveMind = owner.whoAmI;

            if (npc.ai[3] > 0f)
                hiveMind = (int)npc.ai[3] - 1;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] -= 1f; //Relocation rate
                float RandomPositionMultiplier = 1f;
                if (npc.localAI[0] <= 0f)
                {
                    npc.localAI[0] = Main.rand.Next(180, 361);
                    npc.ai[0] = Main.rand.Next(-10, 11) * RandomPositionMultiplier; //X position
                    npc.ai[1] = Main.rand.Next(-10, 11) * RandomPositionMultiplier; //Y position
                    npc.netUpdate = true;
                }
            }

            npc.TargetClosest(true);

            float relocateSpeed = 0.8f;
            Vector2 randomLocationVector = new Vector2(npc.ai[0] * 16f + 8f, npc.ai[1] * 16f + 8f);
            float targetX = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - (npc.width / 2) - randomLocationVector.X;
            float targetY = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - (npc.height / 2) - randomLocationVector.Y;
            float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);

            Vector2 circleCenter = SavedCenter; //Main.npc[hiveMind].Center;
            List<NPC> blobs = Main.npc.Where(n => n.TypeAlive<HiveBlob>()).ToList();
            if (index == -1)
                index = (float)blobs.IndexOf(npc) / blobs.Count;
            float timer = index + (Main.npc[hiveMind].ai[0] % 320f) / 320f;
            if (owner.GetGlobalNPC<HMEternity>().Phase == 1)
                timer = index + (Main.npc[hiveMind].ai[0] % 640f) / 640f;

            float distance = 500;
            float angle = timer * MathF.Tau;
            /*
            float angleFromBoss = owner.DirectionTo(npc.Center).ToRotation();
            float rotationDif = angle - angleFromBoss;
            const float difCap = MathF.PI * 0.1f;
            if (Math.Abs(rotationDif) > difCap && Math.Abs(npc.Distance(owner.Center) - distance) < 300)
                angle = angleFromBoss + Math.Sign(rotationDif) * difCap;
            */
            Vector2 circlePos = circleCenter + (angle.ToRotationVector2() * distance);

            Vector2 findPosition(Vector2 pos)
            {
                float hiveMindX = pos.X;
                float hiveMindY = pos.Y;
                Vector2 hiveMindPos = new Vector2(hiveMindX, hiveMindY);


                float randomPosX = hiveMindX + npc.ai[0];
                float randomPosY = hiveMindY + npc.ai[1];
                float finalRandPosX = randomPosX - hiveMindPos.X;
                float finalRandPosY = randomPosY - hiveMindPos.Y;
                float finalRandDistance = (float)Math.Sqrt(finalRandPosX * finalRandPosX + finalRandPosY * finalRandPosY);
                finalRandDistance = 128f / finalRandDistance;
                finalRandPosX *= finalRandDistance;
                finalRandPosY *= finalRandDistance;
                return new(hiveMindX + finalRandPosX, hiveMindY + finalRandPosY);
            }

            Vector2 desiredPosition = findPosition(circlePos);
            if (Collision.SolidCollision(desiredPosition - npc.Size / 2, npc.width, npc.height))
            {
                circlePos = circleCenter - (angle.ToRotationVector2() * distance);
                desiredPosition = findPosition(circlePos);
            }
                
            /*
            if (npc.position.X < desiredPosition.X)
            {
                npc.velocity.X += relocateSpeed;
                if (npc.velocity.X < 0f && finalRandPosX > 0f)
                    npc.velocity.X *= 0.8f;
            }
            else if (npc.position.X > desiredPosition.X)
            {
                npc.velocity.X -= relocateSpeed;
                if (npc.velocity.X > 0f && finalRandPosX < 0f)
                    npc.velocity.X *= 0.8f;
            }
            if (npc.position.Y < desiredPosition.Y)
            {
                npc.velocity.Y += relocateSpeed;
                if (npc.velocity.Y < 0f && finalRandPosY > 0f)
                    npc.velocity.Y *= 0.8f;
            }
            else if (npc.position.Y > desiredPosition.Y)
            {
                npc.velocity.Y -= relocateSpeed;
                if (npc.velocity.Y > 0f && finalRandPosY < 0f)
                    npc.velocity.Y *= 0.8f;
            }

            float velocityLimit = 8f;
            npc.velocity = npc.velocity.ClampMagnitude(0, velocityLimit);
            */
            if (npc.localAI[1] >= 380f && npc.localAI[1] < 480f && desiredPosition.Distance(npc.Center) > 5)
            {
                npc.velocity = (desiredPosition - npc.Center) * 0.05f;
            }
            else
            {
                npc.velocity *= 0.925f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.localAI[1] == 0) // initial
                    npc.localAI[1] = Main.rand.Next(0, 440);
                //if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    //npc.localAI[1] = Main.rand.Next(0, 440);

                //if (npc.Distance(desiredPosition) < 100)
                npc.localAI[1] += (Main.rand.Next(2) + 1f) * MathHelper.Lerp(4f, 0.22f, (float)blobs.Count / 20);

                if (npc.localAI[1] >= 480f && npc.velocity.Length() < 2f)// && Vector2.Distance(target.Center, npc.Center) > 400f)
                {
                    npc.localAI[1] = 1f;
                    npc.TargetClosest(true);
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float projSpeed = 5f;

                        Vector2 projDirection = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + (npc.height / 2));
                        float playerX = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - projDirection.X;
                        float playerY = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - projDirection.Y;
                        float playerDist = (float)Math.Sqrt(playerX * playerX + playerY * playerY);
                        playerDist = projSpeed / (playerDist + 1e-6f);
                        playerX *= playerDist;
                        playerY *= playerDist;
                        int type = ModContent.ProjectileType<VileClot>();
                        int damage = npc.GetProjectileDamage(type);
                        Vector2 projectileVelocity = new Vector2(playerX, playerY);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), projDirection, projectileVelocity * 0.7f, type, damage, 0f, Main.myPlayer);

                        npc.velocity -= projectileVelocity; // recoil
                        npc.netUpdate = true;
                    }
                }
            }
        }
        public void Phase2AI(NPC npc, NPC owner, Player target)
        {
            // Currently disabled in phase 2
            if (true)//owner.ai[1] != 0)
            {
                npc.active = false;
                return;
            }

            npc.velocity = Vector2.Lerp(npc.velocity, (owner.Center + new Vector2(-180, 0).RotatedBy(npc.ai[1]) - npc.Center).SafeNormalize(Vector2.Zero) * 10, 0.05f);
            npc.position += owner.velocity;
            npc.ai[2]++;
            float maxRadians = MathHelper.Pi;
            if (owner.GetLifePercent() <= 0.9f)
            {
                maxRadians = MathHelper.TwoPi;
            }
            if (npc.ai[2] == 120)
            {
                npc.ai[2] = 0;
                npc.ai[1] = Main.rand.NextFloat(0f, maxRadians);
                
                if (DLCUtils.HostCheck)
                {
                    Vector2 toPlayer = npc.DirectionTo(target.Center) * 7;
                    Vector2 aim = CalamityUtils.CalculatePredictiveAimToTarget(npc.Center, target, 7);
                    aim = Vector2.Lerp(aim, toPlayer, Main.rand.NextFloat(0.2f, 0.6f));
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, aim, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.VileClot>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(owner.damage), 0);
                }
            }
        }
        
    }
}
