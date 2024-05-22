using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static FargowiltasSouls.Content.Bosses.Magmaw.Magmaw;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HMEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<CalamityMod.NPCs.HiveMind.HiveMind>());
        public override bool InstancePerEntity => true;

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            base.HitEffect(npc, hit);
        }
        public override void SetDefaults(NPC entity)
        {
            if (!WorldSavingSystem.EternityMode) return;
            base.SetDefaults(entity);
            entity.scale = 0.01f;
            if (BossRushEvent.BossRushActive)
            {
                entity.lifeMax = 5000000;
            }
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
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);
        }
        public Vector2 sprite = new Vector2(0, 0);
        public int frameCounter = 0;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            Asset<Texture2D> ground = ModContent.Request<Texture2D>("CalamityMod/NPCs/HiveMind/HiveMind");
            Asset<Texture2D> fly = ModContent.Request<Texture2D>("CalamityMod/NPCs/HiveMind/HiveMindP2");
            if (sprite.X == 0)
            {
                Main.EntitySpriteDraw(ground.Value, npc.Center - Main.screenPosition - new Vector2(0, 10), new Rectangle(0, 122 * (int)sprite.Y, 178, 122), npc.GetAlpha(drawColor), npc.rotation, new Vector2(178, 122) / 2, npc.scale, SpriteEffects.None);
            }
            else
            {
                for (int i = 0; i < (int)currentAfterimages; i++)
                {

                    Main.EntitySpriteDraw(fly.Value, npc.oldPos[i] + new Vector2(npc.width / 2, npc.height / 2) - Main.screenPosition + new Vector2(0, 10), new Rectangle(178 * ((int)sprite.X - 1), 142 * (int)sprite.Y, 178, 142), npc.GetAlpha(drawColor) * (1 - i / 10f), npc.rotation, new Vector2(178, 142) / 2, npc.scale, SpriteEffects.None);
                }
                Main.EntitySpriteDraw(fly.Value, npc.Center - Main.screenPosition + new Vector2(0, 10), new Rectangle(178 * ((int)sprite.X - 1), 142 * (int)sprite.Y, 178, 142), npc.GetAlpha(drawColor), npc.rotation, new Vector2(178, 142) / 2, npc.scale, SpriteEffects.None);
            }

            return false;
        }
        public override void DrawBehind(NPC npc, int index)
        {
            if (!WorldSavingSystem.EternityMode) return;
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }
        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (!WorldSavingSystem.EternityMode) return;
            frameCounter++;
            if (frameCounter >= 5)
            {
                sprite.Y++;
                frameCounter = 0;
                if (sprite.X == 0)
                {
                    if (sprite.Y >= 16)
                    {
                        sprite.Y = 0;
                    }
                }
                else
                {
                    if (sprite.Y >= 8)
                    {
                        sprite.Y = 0;
                        sprite.X++;
                        if (sprite.X >= 3)
                        {
                            sprite.X = 1;
                        }
                    }
                }
            }
        }
        public Vector2 LockVector1 = Vector2.Zero;
        public int Phase = 0;
        public float targetAfterimages = 0;
        public float currentAfterimages = 0;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(Phase);
            binaryWriter.WriteVector2(sprite);
            binaryWriter.WriteVector2(LockVector1);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            Phase = binaryReader.Read7BitEncodedInt();
            sprite = binaryReader.ReadVector2();
            LockVector1 = binaryReader.ReadVector2();
        }
        public enum P2States
        {
            Reset = -1,
            Idle,
            OffscreenDash1,
            OffscreenDash2,
            SpindashStart,
            Spindash,
            Decelerate

        };
        public override bool SafePreAI(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return true;


            CalamityGlobalNPC.hiveMind = npc.whoAmI;

            ref float timer = ref npc.ai[0];
            ref float currentAttack = ref npc.ai[1];
            ref float attackCounter = ref npc.ai[2];
            ref float ai3 = ref npc.ai[3];
            
            if (!Targeting())
            {
                return false;
            }
            Player target = Main.player[npc.target];
            Vector2 toTarget = (target.Center - npc.Center).SafeNormalize(Vector2.Zero);
            SoundStyle roar = new SoundStyle("CalamityMod/Sounds/Custom/HiveMindRoar");
            //(so i can turn off the afterimage without it instantly cutting off)
            if (targetAfterimages > currentAfterimages)
            {
                currentAfterimages += 1f;
            }
            else if (targetAfterimages < currentAfterimages)
            {
                currentAfterimages -= 1f;
            }
            targetAfterimages = Math.Min(10, npc.velocity.Length());
            Vector2 bottom = npc.Bottom;
            npc.width = (int)(150 * npc.scale);
            npc.height = (int)(100 * npc.scale);
            npc.Bottom = bottom;

            //ai :real:
            #region Phase 0: Spawn animation
            if (Phase == 0)
            {
                npc.defense = 200;
                npc.damage = 0;
                npc.alpha = 0;
                for (int i = 0; i < timer / 100f; i++)
                {
                    Dust.NewDustDirect(npc.Center - new Vector2(100, 0), 200, 0, DustID.Corruption);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Player player = Main.player[Main.myPlayer];
                    PunchCameraModifier modifier = new(player.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), (timer / 60f), 1f, 60, 100f, FullName);
                    Main.instance.CameraModifiers.Add(modifier);
                }
                if (timer < 300)
                {
                    npc.Center += npc.DirectionTo(target.Center) * 5;
                    npc.dontTakeDamage = true;
                }
                timer++;
                if (timer >= 300 && timer < 320)
                {
                    npc.scale += 0.05f;

                }
                if (timer == 300)
                {
                    SoundEngine.PlaySound(roar with { Pitch = -0.5f }, npc.Center);
                    if (DLCUtils.HostCheck)
                    {
                        int maxBlobs = 20;

                        for (int i = 0; i < maxBlobs; i++)
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HiveBlob>(), npc.whoAmI);
                    }
                    NetSync(npc);
                }
                if (timer >= 320)
                {
                    Phase = 1;
                    timer = 0;
                    ai3 = 60 * 7;
                    for (int i = 0; i < 100f; i++)
                    {
                        Dust.NewDustDirect(npc.Center - new Vector2(100, 0), 200, 0, DustID.Corruption, 0, -5);
                    }
                    npc.scale = 1;
                    NetSync(npc);
                }

            }
            else
            {

                if (Phase == 1 && ai3 > 0) // not during burrow
                {
                    npc.scale = 1; //to avoid mp issues
                    npc.alpha = 0;
                }
                    
            }
            #endregion
            #region Phase 1: Stationary
            if (Phase == 1)
            {
                timer++;

                npc.noGravity = false;
                npc.noTileCollide = false;
                npc.damage = 0;
                npc.defense = 200;

                float distance = npc.Distance(Main.LocalPlayer.Center);
                float threshold = 600f;
                Player player = Main.LocalPlayer;
                for (int l = 0; l < 16; l++)
                {
                    double rad2 = Main.rand.NextFloat(MathF.Tau);
                    double dustdist2 = Main.rand.NextFloat(threshold, threshold + 300);
                    int DustX2 = (int)npc.Center.X - (int)(Math.Cos(rad2) * dustdist2);
                    int DustY2 = (int)npc.Center.Y - (int)(Math.Sin(rad2) * dustdist2);
                    int DustType = Main.rand.NextFromList(DustID.Corruption);
                    int i = Dust.NewDust(new Vector2(DustX2, DustY2), 1, 1, DustType, Scale: Main.rand.NextFloat(1f, 1.5f));
                    Main.dust[i].noGravity = true;
                    Main.dust[i].velocity = Vector2.Normalize(npc.Center - Main.dust[i].position) * Main.rand.Next(2, 5);
                }
                if (player.active && !player.dead && !player.ghost) //pull into arena
                {
                    if (distance > threshold && distance < threshold * 4f)
                    {
                        if (distance > threshold * 2f)
                        {
                            player.controlLeft = false;
                            player.controlRight = false;
                            player.controlUp = false;
                            player.controlDown = false;
                            player.controlUseItem = false;
                            player.controlUseTile = false;
                            player.controlJump = false;
                            player.controlHook = false;
                            if (player.grapCount > 0)
                                player.RemoveAllGrapplingHooks();
                            if (player.mount.Active)
                                player.mount.Dismount(player);
                            player.velocity.X = 0f;
                            player.velocity.Y = -0.4f;
                            player.FargoSouls().NoUsingItems = 2;
                        }

                        Vector2 movement = npc.Center - player.Center;
                        float amp = ((distance - 600) / 100);
                        movement.Normalize();
                        movement *= 0.1f * amp;
                        player.velocity += movement;

                        for (int i = 0; i < 10; i++)
                        {
                            int DustType = Main.rand.NextFromList(DustID.Corruption);
                            int d = Dust.NewDust(player.position, player.width, player.height, DustType, 0f, 0f, 0, default, 1.25f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 5f;
                        }
                    }
                }
              

                //CalamityBurrow(npc, target);

                if (!NPC.AnyNPCs(ModContent.NPCType<HiveBlob>()) && !NPC.AnyNPCs(ModContent.NPCType<HiveBlob2>()))
                {
                    SoundEngine.PlaySound(roar with { Pitch = 0.5f }, npc.Center);
                    Phase = 2;
                    sprite.X = 1;
                    npc.velocity = -npc.DirectionTo(target.Center) * 10;
                    npc.noGravity = true;
                    npc.noTileCollide = true;
                    npc.defense = npc.defDefense;
                    npc.frame.Y = 0;
                    npc.scale = 1f;
                    npc.alpha = 0;
                    npc.dontTakeDamage = false;
                    npc.damage = 0;
                    npc.netSpam = 0;
                    npc.netUpdate = true;

                    if (npc.life > npc.lifeMax * 0.75f)
                    {
                        npc.SimpleStrikeNPC((int)Math.Round(npc.life - (npc.lifeMax * 0.75f)), 1);
                    }
                    foreach (NPC n in Main.npc)
                    {
                        if ((n.type == ModContent.NPCType<HiveBlob>() || n.type == ModContent.NPCType<HiveBlob2>()) && n.ai[0] == npc.whoAmI)
                        {
                            n.StrikeInstantKill();
                        }
                    }
                    SoundEngine.PlaySound(SoundID.NPCDeath23, npc.Center);
                    NetSync(npc);
                    for (int i = 0; i < 3; i++)
                    {
                        npc.ai[i] = 0;
                        npc.localAI[i] = 0;
                    }
                        
                }
            }
            #endregion
            #region Phase 2: Mobile
            const int MidwayIdleStart = 60 * 4 + 30;
            const int IdleEnd = 60 * 9 + 20;
            if (Phase >= 2)
            {
                npc.damage = npc.defDamage;
                switch ((P2States)currentAttack)
                {
                    case P2States.Reset: // go to idle
                        {
                            attackCounter++;
                            if (attackCounter > 3)
                            {
                                timer = 0;
                                attackCounter = 0;
                            }
                            else
                            {
                                timer = MidwayIdleStart; // start a bit into idle
                            }
                            currentAttack = (float)P2States.Idle;
                            ai3 = 0;
                            for (int i = 0; i < 3; i++)
                            {
                                npc.localAI[i] = 0;
                            }
                        }
                        break;
                    case P2States.Idle: // idle float, spawn some shit as a shield
                        {
                            targetAfterimages = 0;
                            npc.damage = npc.defDamage;

                            if (npc.alpha > 0)
                                npc.alpha -= 3;

                            int creeperCount = attackCounter == 0 ? 4 : 3;
                            var creepers = Main.npc.Where(n => n.TypeAlive<DankCreeper>());
                            if (timer % 40 == 0 && creepers.Count() < creeperCount)
                                SpawnCreepers(1);
                            void SpawnCreepers(int count)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    int n = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X + Main.rand.Next(-npc.width / 2, npc.width / 2), (int)npc.Center.Y + Main.rand.Next(-npc.height / 2, npc.height / 2), ModContent.NPCType<DankCreeper>(), ai0: npc.whoAmI);
                                    if (n.IsWithinBounds(Main.maxNPCs))
                                    {
                                        Main.npc[n].velocity = Main.rand.NextVector2Circular(3, 3);
                                    }
                                }
                            }
                            float speedMod = MathF.Min(1f, timer / 60f);
                            float speed = 12 * speedMod;
                            npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(target.Center - npc.Center) * speed, 0.02f);

                            if (timer == MidwayIdleStart + 10)
                            {
                                if (DLCUtils.HostCheck)
                                {
                                    if (attackCounter > 0)
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.velocity, ModContent.ProjectileType<ShadeLightningCloud>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                                    
                                    //SpawnCreepers(creeperCount);
                                }
                            }
                            if (timer > IdleEnd - 35)
                            {
                                foreach (NPC creeper in creepers)
                                {
                                    creeper.ai[3] = 1; // trigger to be released
                                }
                            }
  
                            if (timer > IdleEnd)
                            {
                                npc.netUpdate = true;
                                npc.netSpam = 0;

                                currentAttack = (float)P2States.SpindashStart;
                                timer = 0;
                            }
                        }
                        break;
                    case P2States.OffscreenDash1: // back off, dash,  go offscreen and dash in from offscreen at 90 degree angle
                        {
                            npc.damage = npc.defDamage;
                            targetAfterimages = 10;
                            if (timer == 1) // start of attack
                            {
                                npc.velocity = -npc.DirectionTo(target.Center).RotatedBy(MathF.PI * Main.rand.NextFloat(0.35f, 0.5f) * (Main.rand.NextBool() ? 1 : -1)) * 20;
                                SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound, npc.Center);
                            }
                            float attackTime = 120f;
                            float fadeTime = 100f;
                            bool decel = timer > 10 && timer < 30;
                            bool fade = timer > fadeTime;
                            bool accelerate = timer < fadeTime && timer > 20;
                            bool accelStraight = timer > 75f;

                            //npc.velocity = Vector2.Lerp(npc.velocity, LockVector1, 0.02f);
                            if (fade)
                            {
                                float fadeDuration = (attackTime - fadeTime);
                                //npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Zero, (timer - fadeTime) / (attackTime - fadeTime));
                                npc.Opacity -= 1f / fadeDuration;
                            }
                            if (decel)
                            {
                                npc.velocity *= 0.94f;
                            }
                            if (accelerate) //accelerate during dash towards player
                            {
                                const float accel = 0.95f;
                                const float maxSpeed = 30;
                                Vector2 accelDir = npc.DirectionTo(Main.player[npc.target].Center);
                                if (accelStraight)
                                    accelDir = Vector2.Normalize(npc.velocity);
                                npc.velocity += accelDir * accel;
                                if (npc.velocity.LengthSquared() > maxSpeed * maxSpeed)
                                {
                                    npc.velocity = Vector2.Normalize(npc.velocity) * maxSpeed;
                                }
                            }
                            if (timer >= attackTime)
                            {
                                currentAttack = 2;
                                timer = 0;
                            }
                        }
                        break;
                    case P2States.OffscreenDash2: // it's currently offscreen. shift 90 degrees random left or right,quickly fade in, and dash in with slight curving and leaving cursed flame trails diagonally backwards
                        {
                            npc.damage = npc.defDamage;
                            const float maxSpeed = 25f;
                            targetAfterimages = 10;
                            if (timer == 1)
                            {
                                Vector2 currentVelocity = npc.velocity;
                                Vector2 newVelocity = currentVelocity.RotatedBy(MathF.PI * 0.5f * (Main.rand.NextBool() ? 1 : -1));
                                Vector2 newPosition = target.Center - Vector2.Normalize(newVelocity) * 1200;
                                npc.Center = newPosition;
                                npc.velocity = newVelocity;
                                npc.netUpdate = true;
                                npc.velocity = npc.velocity.ClampMagnitude(0f, maxSpeed);

                                SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound with { Pitch = -0.5f }, npc.Center);
                            }
                            if (timer % 4 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item20, npc.Center);
                                for (int i = -1; i < 2; i += 2)
                                {
                                    if (FargoSoulsUtil.HostCheck)
                                    {
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), Main.rand.NextVector2FromRectangle(npc.Hitbox), -npc.velocity.RotatedBy(MathF.PI / 3f * i) * 0.8f, ModContent.ProjectileType<HiveMindFiretrail>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                                    }
                                }
                                
                            }
                            float dashTime = 80;
                            float decelTime = 25;
                            if (Math.Abs(FargoSoulsUtil.RotationDifference(npc.velocity, npc.DirectionTo(target.Center))) > MathHelper.PiOver2)
                                if (timer < dashTime)
                                    timer = dashTime;
                            bool decel = timer >= dashTime;
                            bool accelerate = timer < dashTime;

                            float fadeinTime = 15f;
                            if (npc.Opacity < 1)
                                npc.Opacity += 1f / fadeinTime;

                            if (decel)
                            {
                                npc.velocity *= 0.96f;
                            }
                            if (accelerate) //accelerate during dash towards player
                            {
                                const float accel = 0.85f;
                                Vector2 accelDir = npc.DirectionTo(Main.player[npc.target].Center);
                                npc.velocity += accelDir * accel;
                                if (npc.velocity.LengthSquared() > maxSpeed * maxSpeed)
                                {
                                    npc.velocity = Vector2.Normalize(npc.velocity) * maxSpeed;
                                }
                            }
                            if (timer > dashTime + decelTime)
                            {
                                currentAttack = (float)P2States.Reset;
                            }
                        }
                        break;
                    case P2States.SpindashStart: // start spin dash, by dashing away and fading
                        {
                            ref float rotation = ref npc.localAI[0];
                            ref float rotationDirection = ref npc.localAI[1];

                            float startTime = 30;

                            if (timer == 5) // before end to give time for netupdate
                            {
                                rotation = Main.rand.NextFloat(MathF.Tau);
                                rotationDirection = Main.rand.NextFromList(1, -1);
                                npc.netUpdate = true;
                            }
                            if (npc.Opacity > 0)
                                npc.Opacity -= 1f / startTime;
                            if (timer == startTime)
                            {
                                timer = 0;
                                currentAttack = (float)P2States.Spindash;
                            }
                        }
                        break;
                    case P2States.Spindash: // multi-cal spin
                        {
                            ref float rotation = ref npc.localAI[0];
                            ref float rotationDirection = ref npc.localAI[1];
                            ref float dashes = ref npc.localAI[3];
                            int totalDashes = 3;

                            int lungeFade = 15; // Divide 255 by this for duration of hive mind spin before slowing for lunge
                            double lungeRots = 0.4;
                            double rotationIncrement = 0.0246399424 * lungeRots * lungeFade;
                            int lungeDelay = 90; // # of ticks long hive mind spends sliding to a stop before lunging
                            int teleportRadius = 300;
                            float lungeTime = 23;

                            npc.netUpdate = true;
                            npc.netSpam = 0;
                            if (npc.alpha > 0)
                            {
                                npc.alpha -= lungeFade;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 desiredVel = (target.Center + new Vector2(teleportRadius, 0).RotatedBy(rotation)) - npc.Center;
                                    npc.velocity = desiredVel;

                                }
                                    

                                rotation += (float)(rotationIncrement * rotationDirection);
                                timer = lungeDelay;
                            }
                            else
                            {
                                timer -= 2;
                                if (ai3 != 1)
                                {
                                    if (timer <= 0)
                                    {
                                        // Set damage
                                        npc.damage = npc.defDamage;

                                        timer = lungeTime;
                                        npc.velocity = target.Center  - npc.Center;
                                        npc.velocity.Normalize();
                                        npc.velocity *= teleportRadius / (lungeTime);
                                        npc.velocity *= 1.2f;
                                        ai3 = 1;
                                        SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound, npc.Center);
                                    }
                                    else
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Vector2 desiredVel = (target.Center + new Vector2(teleportRadius, 0).RotatedBy(rotation)) - npc.Center;
                                            if (dashes <= 0)
                                                npc.velocity = desiredVel;
                                            else
                                            {
                                                npc.velocity = Vector2.Lerp(npc.velocity, desiredVel, 0.1f);
                                                npc.position += target.velocity;
                                            }
                                                
                                        }

                                        rotation += (float)(rotationIncrement * rotationDirection * timer / lungeDelay);
                                    }
                                }
                                else
                                {
                                    // Set damage
                                    npc.damage = npc.defDamage;
                                    
                                    if (timer <= 0)
                                    {
                                        dashes++;
                                        if (dashes >= 3)
                                        {
                                            dashes = 0;
                                            currentAttack = (float)P2States.Decelerate; // Deceleration phase
                                            timer = 0;
                                            ai3 = npc.velocity.Length() / 30;
                                        }
                                        else
                                        {
                                            ai3 = 0;
                                            rotation = target.DirectionTo(npc.Center).RotatedByRandom(MathF.PI * 0.2f).ToRotation();
                                            rotationDirection *= -1;
                                            timer = lungeDelay;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case P2States.Decelerate: // decelerate to halt, then go to idle
                        {
                            npc.velocity -= npc.velocity.SafeNormalize(Vector2.Zero) * ai3;
                            if (timer > 30)
                            {
                                npc.velocity *= 0;
                                currentAttack = (float)P2States.Idle; // Go to idle
                                npc.netUpdate = true;
                                npc.netSpam = 0;
                            }
                        }
                        break;

                        // Other attack ideas:
                        // Hover above player, drop worms that fall diagonally below in cones, then dash down (make player go in directly under then out when dashes)
                        // Calamity spin but thrice connected into eachother
                        // Calamity rain, possibly modified? but it's pretty good already
                        // Calamity enemy spinny with some special behaviour after, before idling again
                }
                timer++;
            }
            #endregion
            return false;

            bool Targeting()
            {
                const float despawnRange = 5000f;
                Player p = Main.player[npc.target];
                if (!p.active || p.dead || Vector2.Distance(npc.Center, p.Center) > despawnRange)
                {
                    npc.TargetClosest();
                    p = Main.player[npc.target];
                    if (!p.active || p.dead || Vector2.Distance(npc.Center, p.Center) > despawnRange)
                    {
                        npc.noTileCollide = true;
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;
                        npc.velocity.Y += 1f;
                        if (npc.timeLeft == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
                            }
                        }
                        return false;
                    }
                }
                return true;
            }
        }
        public void CalamityBurrow(NPC npc, Player target)
        {
            ref float burrowTimer = ref npc.ai[3];
            burrowTimer--;
            if (npc.Distance(target.Center) > 700 && burrowTimer > 5)
            {
                burrowTimer = 5;
            }
            if (burrowTimer < -120)
            {
                burrowTimer = 60 * 7;
                if (burrowTimer < 30)
                    burrowTimer = 30;

                npc.scale = 1f;
                npc.alpha = 0;
                npc.dontTakeDamage = false;
            }
            else if (burrowTimer < -60)
            {
                npc.scale += 0.0165f;
                npc.alpha -= 4;

                int burrowedDust = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, DustID.Demonite, 0f, -3f, 100, default, 2.5f * npc.scale);
                Main.dust[burrowedDust].velocity *= 2f;
                if (Main.rand.NextBool())
                {
                    Main.dust[burrowedDust].scale = 0.5f;
                    Main.dust[burrowedDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }

                for (int i = 0; i < 2; i++)
                {
                    int burrowedDust2 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, DustID.Demonite, 0f, -3f, 100, default, 3.5f * npc.scale);
                    Main.dust[burrowedDust2].noGravity = true;
                    Main.dust[burrowedDust2].velocity *= 3.5f;
                    burrowedDust2 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, DustID.Demonite, 0f, -3f, 100, default, 2.5f * npc.scale);
                    Main.dust[burrowedDust2].velocity *= 1f;
                }
            }
            else if (burrowTimer == -60)
            {
                npc.scale = 0.01f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.Center = target.Center;
                    npc.position.Y = target.position.Y - npc.height;
                    int tilePosX = (int)npc.Center.X / 16;
                    int tilePosY = (int)(npc.position.Y + npc.height) / 16 + 1;

                    while (!(Main.tile[tilePosX, tilePosY].HasUnactuatedTile && Main.tileSolid[Main.tile[tilePosX, tilePosY].TileType]))
                    {
                        tilePosY++;
                        npc.position.Y += 16;
                    }
                }
                npc.netUpdate = true;
                npc.netSpam = 0;
            }
            else if (burrowTimer < 0)
            {
                npc.scale -= 0.0165f;
                npc.alpha += 4;

                int burrowedDust = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, DustID.Demonite, 0f, -3f, 100, default, 2.5f * npc.scale);
                Main.dust[burrowedDust].velocity *= 2f;
                if (Main.rand.NextBool())
                {
                    Main.dust[burrowedDust].scale = 0.5f;
                    Main.dust[burrowedDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }

                for (int i = 0; i < 2; i++)
                {
                    int burrowedDust2 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, DustID.Demonite, 0f, -3f, 100, default, 3.5f * npc.scale);
                    Main.dust[burrowedDust2].noGravity = true;
                    Main.dust[burrowedDust2].velocity *= 3.5f;
                    burrowedDust2 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, DustID.Demonite, 0f, -3f, 100, default, 2.5f * npc.scale);
                    Main.dust[burrowedDust2].velocity *= 1f;
                }
            }
            else if (burrowTimer == 0)
            {
                if (!target.active || target.dead)
                {
                    burrowTimer = 30;
                }
                else
                {
                    npc.TargetClosest();
                    npc.dontTakeDamage = true;
                }
            }
        }
                
        #region Old
        /*
        public void Teleport(NPC npc, int startTime, int endTime, Vector2 location)
        {
            ref float timer = ref npc.ai[0];
            NPC heart = null;
            foreach (NPC n in Main.npc)
            {
                if (n != null && n.type == ModContent.NPCType<DarkHeart>() && n.ai[0] == npc.whoAmI && n.active)
                {
                    heart = n;
                    break;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Corruption);
            }
            timer++;
            if (timer <= startTime)
            {
                npc.Opacity = MathHelper.Lerp(1, 0, (timer / startTime));
                if (heart != null) heart.Opacity = npc.Opacity;
            }
            if (timer == startTime)
            {
                npc.Center = location;
                if (heart != null) heart.Center = npc.Center;
                NetSync(npc);
                npc.netUpdate = true; //doing double here for safetybecause net syncing whenever you teleport is VERY important
            }
            if (timer <= startTime + endTime && timer > startTime)
            {

                npc.Opacity = MathHelper.Lerp(0, 1, ((timer - startTime) / endTime));
                if (heart != null) heart.Opacity = npc.Opacity;
            }
            if (timer == startTime + endTime)
            {
                IncrementCycle(npc);
            }
        }
        public void RetractHeart(NPC npc, float percent, int fromPhase, int numCreeper, int numBlob, int[] attacks)
        {
            if (npc.GetLifePercent() <= percent && phase == fromPhase)
            {
                attackCycle = attacks;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                phase++;
                if (DLCUtils.HostCheck)
                {
                    for (int i = 0; i < numCreeper; i++)
                    {

                        NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DankCreeper>(), ai0: npc.whoAmI);

                    }
                    for (int i = 0; i < numBlob; i++)
                    {
                        if (Main.rand.NextBool())
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HiveBlob>(), ai0: npc.whoAmI);
                        else
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HiveBlob2>(), ai0: npc.whoAmI);
                    }
                }
                NetSync(npc);
            }
        }
        public void ReleaseHeart(NPC npc)
        {
            if (npc.ai[1] == 0 && !NPC.AnyNPCs(ModContent.NPCType<DankCreeper>()))
            {
                foreach (NPC n in Main.npc)
                {
                    if (n != null && (n.type == ModContent.NPCType<HiveBlob>() || n.type == ModContent.NPCType<HiveBlob2>()) && n.ai[0] == npc.whoAmI)
                    {
                        n.StrikeInstantKill();
                    }
                }
                npc.ai[1] = 1;
                SoundEngine.PlaySound(SoundID.NPCDeath23, npc.Center);
                if (npc.GetLifePercent() <= 0.8f)
                {
                    attackCycle = [0, 1, 2, -1, -1, -1, -1];
                }
                if (npc.GetLifePercent() <= 0.5f)
                {
                    attackCycle = [0, 1, 5, 3, 2, 4, -1];
                }
                if (npc.GetLifePercent() <= 0.2f)
                {
                    attackCycle = [0, 1, 3, 5, 2, 5, 4];
                }
                NetSync(npc);
            }
        }
        public void Follow(NPC npc, float speed, Vector2 toTarget, int time)
        {
            ref float timer = ref npc.ai[0];

            timer++;
            if (timer < 0)
            {
                npc.velocity /= 1.05f;
                return;
            }
            if (npc.GetLifePercent() <= 0.8f)
            {
                speed *= 1.7f;
                if (timer == time / 2 && npc.ai[1] == 1)
                {
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.velocity, ModContent.ProjectileType<ShadeLightningCloud>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0);
                }
            }
            npc.velocity = toTarget * speed;

            if (timer >= time)
            {
                IncrementCycle(npc);
            }
        }
        public void Dash(NPC npc, Vector2 vector, int time, int slow = 0, float force = 1)
        {
            ref float timer = ref npc.ai[0];
            int attack = attackCycle[(int)npc.ai[2]];

            if (timer == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/HiveMindRoar"), npc.Center);
                npc.velocity = vector;
            }

            timer++;
            npc.velocity = Vector2.Lerp(npc.velocity, vector, force);
            if (timer >= slow)
            {
                npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Zero, (timer - slow) / (time - slow));
            }
            else if (attack == 1) //accelerate during dash towards player
            {
                const float accel = 0.175f;
                const float maxSpeed = 20;
                npc.velocity += npc.DirectionTo(Main.player[npc.target].Center) * accel;
                if (npc.velocity.LengthSquared() > maxSpeed * maxSpeed)
                {
                    npc.velocity = Vector2.Normalize(npc.velocity) * maxSpeed;
                }
            }
            else if (attack == 7) //shade clouds during shade cloud attack
            {
                if (timer % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int type = ModContent.ProjectileType<ShadeNimbusHostile>();
                    int damage = FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage);
                    Vector2 cloudSpawnPos = new Vector2(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height));
                    Projectile.NewProjectile(npc.GetSource_FromAI(), cloudSpawnPos, Vector2.Zero, type, damage, 0, Main.myPlayer, 11f);
                }
            }
            if (timer >= time)
            {
                IncrementCycle(npc);
            }
        }
        public void IncrementCycle(NPC npc)
        {
            ref float timer = ref npc.ai[0];

            timer = 0;
            npc.ai[2]++;
            if (npc.ai[2] >= attackCycle.Length || attackCycle[(int)npc.ai[2]] < 0)
            {
                npc.ai[2] = 0;
            }
            NetSync(npc);
        }
        */
        #endregion
    }
}
