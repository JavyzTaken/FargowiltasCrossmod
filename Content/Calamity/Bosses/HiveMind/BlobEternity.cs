using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
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
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BlobEternity : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<HiveBlob>();

        Vector2 SavedCenter = Vector2.Zero;

        public const int P1Blobs = 10;
        public override void SetDefaults()
        {
            float healthMult = 0.5f;
            int hiveMind = CalamityGlobalNPC.hiveMind;
            if (!(hiveMind < 0 || !Main.npc[hiveMind].TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>()))
            {
                NPC owner = Main.npc[hiveMind];
                if (owner.GetDLCBehavior<HMEternity>().Phase < 2)
                {
                    healthMult *= 10;
                    NPC.scale = 1.5f;
                }
            }
            NPC.lifeMax = (int)(NPC.lifeMax * healthMult);
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (!WorldSavingSystem.EternityMode) return;
            int hiveMind = CalamityGlobalNPC.hiveMind;
            if (hiveMind < 0 || !Main.npc[hiveMind].TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>())
            {
                return;
            }
            NPC owner = Main.npc[hiveMind];
            if (owner.GetDLCBehavior<HMEternity>().Phase < 2)
            {
                float maxRadians = owner.GetLifePercent() <= 0.9f ? MathHelper.TwoPi : MathHelper.Pi;
                NPC.ai[1] = Main.rand.NextFloat(0f, maxRadians);
            }
            
        }
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(NPC.localAI[1]);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            NPC.localAI[1] = binaryReader.ReadSingle();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            int hiveMind = CalamityGlobalNPC.hiveMind;
            if (hiveMind < 0 || !Main.npc[hiveMind].TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>())
            {
                return true;
            }
            NPC owner = Main.npc[hiveMind];

            if (owner.GetDLCBehavior<HMEternity>().Phase < 2)
            {
                if (NPC.HasValidTarget)
                {
                    Player target = Main.player[NPC.target];
                    Asset<Texture2D> line = TextureAssets.Extra[178];

                    float opacity = 0;
                    if (NPC.localAI[1] >= 420f)
                    {
                        opacity = MathHelper.Lerp(0, 1, (NPC.localAI[1] - 420f) / 60f);
                    }
                    Main.EntitySpriteDraw(line.Value, NPC.Center - Main.screenPosition, null, Color.Lime * opacity, NPC.DirectionTo(target.Center).ToRotation(), new Vector2(0, line.Height() * 0.5f), new Vector2(0.2f, NPC.scale * 4), SpriteEffects.None);
                }
                return true;
            }
            else if (owner.ai[2] != 0)
                return true;


            Asset<Texture2D> t = TextureAssets.Chains[3];
            Vector2 pos = NPC.Center + ((owner.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20);
            while (pos.Distance(owner.Center) > 20)
            {
                Main.EntitySpriteDraw(t.Value, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()), pos.AngleTo(owner.Center) + MathHelper.Pi / 2, t.Size() / 2, NPC.scale, SpriteEffects.None);
                pos += (owner.Center - pos).SafeNormalize(Vector2.Zero) * 30;
            }
            return true;
        }
        public override bool PreAI()
        {
            if (!FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode) return true;


            
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.velocity.Y += 1;
                return false;
            }
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.velocity.Y += 1;
                return false;
            }
            Player target = Main.player[NPC.target];
            int hiveMind = CalamityGlobalNPC.hiveMind;
            if (hiveMind < 0 || !Main.npc[hiveMind].TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>())
            {
                NPC.StrikeInstantKill();
                return false;
            }
            NPC owner = Main.npc[hiveMind];
            if (owner.GetDLCBehavior<HMEternity>().Phase < 2)
                Phase1AI(NPC, owner, target);
            else
                Phase2AI(NPC, owner, target);
            

            return false;

        }
        float index = -1;
        int InvumTime = 0;
        public void Phase1AI(NPC NPC, NPC owner, Player target)
        {
            // Setting this in SetDefaults will disable expert mode scaling, so put it here instead
            NPC.damage = 0;
            NPC.chaseable = true;

            if (InvumTime < 60)
            {
                NPC.dontTakeDamage = true;
                InvumTime++;
            }
            else
                NPC.dontTakeDamage = false;

            if (SavedCenter == Vector2.Zero || SavedCenter.Distance(target.Center) > 1500)
                SavedCenter = target.Center;

            SavedCenter = Vector2.Lerp(SavedCenter, target.Center, 0.4f);

            int hiveMind = owner.whoAmI;

            if (NPC.ai[3] > 0f)
                hiveMind = (int)NPC.ai[3] - 1;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] -= 1f; //Relocation rate
                float RandomPositionMultiplier = 1f;
                if (NPC.localAI[0] <= 0f)
                {
                    NPC.localAI[0] = Main.rand.Next(180, 361);
                    NPC.ai[0] = Main.rand.Next(-10, 11) * RandomPositionMultiplier; //X position
                    NPC.ai[1] = Main.rand.Next(-10, 11) * RandomPositionMultiplier; //Y position
                    NPC.netUpdate = true;
                }
            }

            NPC.TargetClosest(true);

            float relocateSpeed = 0.8f;
            Vector2 randomLocationVector = new Vector2(NPC.ai[0] * 16f + 8f, NPC.ai[1] * 16f + 8f);
            float targetX = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2) - (NPC.width / 2) - randomLocationVector.X;
            float targetY = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2) - (NPC.height / 2) - randomLocationVector.Y;
            float targetDistance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);

            Vector2 circleCenter = SavedCenter; //Main.NPC[hiveMind].Center;
            List<NPC> blobs = Main.npc.Where(n => n.TypeAlive<HiveBlob>()).ToList();
            if (index == -1)
                index = (float)blobs.IndexOf(NPC) / blobs.Count;
            float timer = index + (Main.npc[hiveMind].ai[0] % 320f) / 320f;
            if (owner.GetDLCBehavior<HMEternity>().Phase == 1)
                timer = index + (Main.npc[hiveMind].ai[0] % 640f) / 640f;

            float distance = 500;
            float angle = timer * MathF.Tau;
            /*
            float angleFromBoss = owner.DirectionTo(NPC.Center).ToRotation();
            float rotationDif = angle - angleFromBoss;
            const float difCap = MathF.PI * 0.1f;
            if (Math.Abs(rotationDif) > difCap && Math.Abs(NPC.Distance(owner.Center) - distance) < 300)
                angle = angleFromBoss + Math.Sign(rotationDif) * difCap;
            */
            Vector2 circlePos = circleCenter + (angle.ToRotationVector2() * distance);

            Vector2 findPosition(Vector2 pos)
            {
                float hiveMindX = pos.X;
                float hiveMindY = pos.Y;
                Vector2 hiveMindPos = new Vector2(hiveMindX, hiveMindY);


                float randomPosX = hiveMindX + NPC.ai[0];
                float randomPosY = hiveMindY + NPC.ai[1];
                float finalRandPosX = randomPosX - hiveMindPos.X;
                float finalRandPosY = randomPosY - hiveMindPos.Y;
                float finalRandDistance = (float)Math.Sqrt(finalRandPosX * finalRandPosX + finalRandPosY * finalRandPosY);
                finalRandDistance = 128f / finalRandDistance;
                finalRandPosX *= finalRandDistance;
                finalRandPosY *= finalRandDistance;
                return new(hiveMindX + finalRandPosX, hiveMindY + finalRandPosY);
            }

            Vector2 desiredPosition = findPosition(circlePos);
            if (Collision.SolidCollision(desiredPosition - NPC.Size / 2, NPC.width, NPC.height))
            {
                circlePos = circleCenter - (angle.ToRotationVector2() * distance);
                desiredPosition = findPosition(circlePos);
            }
                
            /*
            if (NPC.position.X < desiredPosition.X)
            {
                NPC.velocity.X += relocateSpeed;
                if (NPC.velocity.X < 0f && finalRandPosX > 0f)
                    NPC.velocity.X *= 0.8f;
            }
            else if (NPC.position.X > desiredPosition.X)
            {
                NPC.velocity.X -= relocateSpeed;
                if (NPC.velocity.X > 0f && finalRandPosX < 0f)
                    NPC.velocity.X *= 0.8f;
            }
            if (NPC.position.Y < desiredPosition.Y)
            {
                NPC.velocity.Y += relocateSpeed;
                if (NPC.velocity.Y < 0f && finalRandPosY > 0f)
                    NPC.velocity.Y *= 0.8f;
            }
            else if (NPC.position.Y > desiredPosition.Y)
            {
                NPC.velocity.Y -= relocateSpeed;
                if (NPC.velocity.Y > 0f && finalRandPosY < 0f)
                    NPC.velocity.Y *= 0.8f;
            }

            float velocityLimit = 8f;
            NPC.velocity = NPC.velocity.ClampMagnitude(0, velocityLimit);
            */
            if (NPC.localAI[1] >= 380f && NPC.localAI[1] < 480f && desiredPosition.Distance(NPC.Center) > 5)
            {
                NPC.velocity = (desiredPosition - NPC.Center) * 0.05f;
            }
            else
            {
                NPC.velocity *= 0.925f;
            }

            if (NPC.localAI[1] == 0) // initial
            {
                NPC.localAI[1] = Main.rand.Next(0, 440);
                NPC.netUpdate = true;
            }
                
            //if (!Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
            //NPC.localAI[1] = Main.rand.Next(0, 440);

            //if (NPC.Distance(desiredPosition) < 100)
            NPC.localAI[1] += (Main.rand.Next(2) + 1f) * MathHelper.Lerp(6f, 1f, (float)blobs.Count / P1Blobs);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.localAI[1] >= 480f && NPC.velocity.Length() < 2f)// && Vector2.Distance(target.Center, NPC.Center) > 400f)
                {
                    NPC.localAI[1] = 1f;
                    NPC.TargetClosest(true);
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        float projSpeed = 5f;

                        Vector2 projDirection = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + (NPC.height / 2));
                        float playerX = Main.player[NPC.target].position.X + Main.player[NPC.target].width * 0.5f - projDirection.X;
                        float playerY = Main.player[NPC.target].position.Y + Main.player[NPC.target].height * 0.5f - projDirection.Y;
                        float playerDist = (float)Math.Sqrt(playerX * playerX + playerY * playerY);
                        playerDist = projSpeed / (playerDist + 1e-6f);
                        playerX *= playerDist;
                        playerY *= playerDist;
                        int type = ModContent.ProjectileType<VileClotDrop>();
                        int damage = NPC.GetProjectileDamage(type);
                        Vector2 projectileVelocity = new Vector2(playerX, playerY);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projDirection, projectileVelocity * 0.2f, type, damage, 0f, Main.myPlayer);

                        NPC.velocity -= projectileVelocity; // recoil
                        NPC.netUpdate = true;
                    }
                }
            }
        }
        public void Phase2AI(NPC NPC, NPC owner, Player target)
        {
            // Currently disabled in phase 2
            if (true)//owner.ai[1] != 0)
            {
                NPC.active = false;
                return;
            }

            NPC.velocity = Vector2.Lerp(NPC.velocity, (owner.Center + new Vector2(-180, 0).RotatedBy(NPC.ai[1]) - NPC.Center).SafeNormalize(Vector2.Zero) * 10, 0.05f);
            NPC.position += owner.velocity;
            NPC.ai[2]++;
            float maxRadians = MathHelper.Pi;
            if (owner.GetLifePercent() <= 0.9f)
            {
                maxRadians = MathHelper.TwoPi;
            }
            if (NPC.ai[2] == 120)
            {
                NPC.ai[2] = 0;
                NPC.ai[1] = Main.rand.NextFloat(0f, maxRadians);
                
                if (DLCUtils.HostCheck)
                {
                    Vector2 toPlayer = NPC.DirectionTo(target.Center) * 7;
                    Vector2 aim = CalamityUtils.CalculatePredictiveAimToTarget(NPC.Center, target, 7);
                    aim = Vector2.Lerp(aim, toPlayer, Main.rand.NextFloat(0.2f, 0.6f));
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, aim, ModContent.ProjectileType<VileClotDrop>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(owner.damage), 0);
                }
            }
        }
        
    }
    public class Blob2Eternity : BlobEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<HiveBlob2>();
    }
}
