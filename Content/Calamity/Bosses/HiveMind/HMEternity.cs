using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.NPCs;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Extensions;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
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
            else
            {
                entity.lifeMax *= 2;
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
            npc.buffImmune[BuffID.Oiled] = true;
            npc.buffImmune[ModContent.BuffType<OiledBuff>()] = true;
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
                if (Subphase(npc) >= 3 && npc.ai[1] == (float)P2States.Spindash)
                    DLCUtils.DrawBackglow(fly, npc.GetAlpha(Color.Purple), npc.Center + new Vector2(0, 10), new Vector2(178, 142) / 2, npc.rotation, npc.scale, offsetMult: 2, sourceRectangle: new Rectangle(178 * ((int)sprite.X - 1), 142 * (int)sprite.Y, 178, 142));
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
        public int LastAttack = 0;
        public static int Subphase(NPC npc)
        {
            float life = npc.GetLifePercent();
            return life < 0.5f ? life < 0.175f ? 3 : 2 : 1;
        }

        public float targetAfterimages = 0;
        public float currentAfterimages = 0;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(Phase);
            binaryWriter.Write7BitEncodedInt(LastAttack);
            binaryWriter.WriteVector2(sprite);
            binaryWriter.WriteVector2(LockVector1);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            Phase = binaryReader.Read7BitEncodedInt();
            LastAttack = binaryReader.Read7BitEncodedInt();
            sprite = binaryReader.ReadVector2();
            LockVector1 = binaryReader.ReadVector2();
        }
        public enum P2States
        {
            Reset = -1,
            Idle,
            I_OffscreenDash1,
            OffscreenDash2,
            I_SpindashStart,
            Spindash,
            FinalSpindash,
            I_WormDrop,
            I_RainDashStart,
            RainDash,
            I_DiagonalDashes,
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
                    npc.dontTakeDamage = false;
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

                //Aura();
                void Aura()
                {
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
                }
              

                CalamityBurrow(npc, target);

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
            const int MidwayIdleStart = 60 * 4 + 10;
            const int IdleEnd = 60 * 9 - 20;
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

                                if (attackCounter == 3 || Subphase(npc) >= 3)
                                    currentAttack = (float)P2States.I_SpindashStart;
                                else
                                {
                                    List<P2States> attacks = [P2States.I_OffscreenDash1, P2States.I_WormDrop, P2States.I_DiagonalDashes];
                                    if (Subphase(npc) > 1)
                                        attacks.Add(P2States.I_RainDashStart);
                                    attacks.Remove((P2States)LastAttack);

                                    currentAttack = (float)Main.rand.NextFromCollection(attacks);
                                }
                                LastAttack = (int)currentAttack;
                                
                                timer = 0;
                                ai3 = 0;
                                npc.netUpdate = true;
                            }
                        }
                        break;
                    case P2States.I_OffscreenDash1: // back off, dash,  go offscreen and dash in from offscreen at 90 degree angle
                        {
                            npc.damage = npc.defDamage;
                            targetAfterimages = 10;
                            if (timer == 1) // start of attack
                            {
                                npc.velocity = -npc.DirectionTo(target.Center).RotatedBy(MathF.PI * Main.rand.NextFloat(0.35f, 0.5f) * (Main.rand.NextBool() ? 1 : -1)) * 25;
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
                                const float accel = 1.12f;
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
                    case P2States.I_SpindashStart: // start spin dash, by dashing away and fading
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
                            int totalDashes = Subphase(npc) switch
                            {
                                3 => 9999,
                                2 => 4,
                                _ => 2
                            };

                            int lungeFade = 15; // Divide 255 by this for duration of hive mind spin before slowing for lunge
                            double lungeRots = 0.4;
                            double rotationIncrement = 0.0246399424 * lungeRots * lungeFade;
                            int lungeDelay = 60; // # of ticks long hive mind spends sliding to a stop before lunging
                            int teleportRadius = 300;
                            float lungeTime = 23;

                            npc.netUpdate = true;
                            npc.netSpam = 0;
                            if (npc.alpha > 0)
                            {
                                npc.alpha -= lungeFade;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float distance = teleportRadius;
                                    if (dashes > 0)
                                        distance = MathHelper.Lerp(npc.Distance(target.Center), teleportRadius, 0.1f);
                                    Vector2 desiredVel = (target.Center + new Vector2(distance, 0).RotatedBy(rotation)) - npc.Center;
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
                                        SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.FastRoarSound, npc.Center);
                                    }
                                    else
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            float distance = teleportRadius;
                                            if (dashes > 0)
                                                distance = MathHelper.Lerp(npc.Distance(target.Center), teleportRadius, MathHelper.Lerp(1, 0.025f, timer / lungeDelay));
                                            Vector2 desiredPos = (target.Center + new Vector2(distance, 0).RotatedBy(rotation));
                                            npc.velocity = desiredPos - npc.Center;
                                        }

                                        if (dashes > 0)
                                            rotationIncrement *= 0.6f;

                                        rotation += (float)(rotationIncrement * rotationDirection * timer / lungeDelay);
                                    }
                                }
                                else
                                {
                                    // Set damage
                                    npc.damage = npc.defDamage;

                                    npc.velocity *= 1.02f;

                                    float dif = FargoSoulsUtil.RotationDifference(npc.velocity, npc.DirectionTo(target.Center));
                                    if (Math.Abs(dif) >= MathHelper.PiOver2 && timer <= lungeTime / 2)
                                    {
  
                                        if (dashes < totalDashes - 1)
                                        {
                                            dashes++;
                                            ai3 = 0;
                                            rotation = target.DirectionTo(npc.Center).ToRotation();
                                            rotationDirection = Math.Sign(FargoSoulsUtil.RotationDifference(npc.DirectionTo(target.Center), (npc.Center + npc.velocity).DirectionTo(target.Center)));
                                            timer = lungeDelay;
                                        }
                                        else if (timer <= 0)
                                        {
                                            currentAttack = (float)P2States.FinalSpindash; // Final spin
                                            LockVector1 = target.Center;
                                            dashes = 0;
                                            
                                            timer = 0;
                                            ai3 = 0;
                                            rotation = target.DirectionTo(npc.Center).ToRotation();
                                            rotationDirection = Math.Sign(FargoSoulsUtil.RotationDifference(npc.DirectionTo(target.Center), (npc.Center + npc.velocity).DirectionTo(target.Center)));
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case P2States.FinalSpindash: // Spin out, lock spin center, then spin in to center
                        {
                            ref float rotation = ref npc.localAI[0];
                            ref float rotationDirection = ref npc.localAI[1];
                            ref float dashes = ref npc.localAI[3];

                            int lungeFade = 15; // Divide 255 by this for duration of hive mind spin before slowing for lunge
                            double lungeRots = 0.4;
                            double rotationIncrement = 0.0246399424 * lungeRots * lungeFade;
                            int lungeDelay = 80 - (int)(dashes * 15); // # of ticks long hive mind spends sliding to a stop before lunging
                            int teleportRadius = 400;
                            float lungeTime = 23;

                            npc.netUpdate = true;
                            npc.netSpam = 0;

                            int totalTime = 160;
                            int spinoutTime = 40;

                            if (timer == spinoutTime)
                            {
                                SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound, npc.Center);
                            }

                            if (timer < totalTime)
                            {
                                npc.damage = npc.defDamage;
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    float progress = timer / totalTime;
                                    if (timer <= spinoutTime)
                                    {
                                        LockVector1 = target.Center;
                                        teleportRadius = (int)MathHelper.Lerp(0, teleportRadius, timer / spinoutTime);
                                        if (teleportRadius < npc.Distance(LockVector1))
                                            teleportRadius = (int)npc.Distance(LockVector1);
                                    }
                                    else
                                    {
                                        float spininProg = (timer - spinoutTime) / (totalTime - spinoutTime);
                                        teleportRadius = (int)MathHelper.Lerp(teleportRadius, 0, spininProg);

                                        if (timer % 25 == 0 && timer < totalTime - 40 && NPC.CountNPCS(ModContent.NPCType<DarkHeart>()) < 2)
                                        {
                                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), ModContent.NPCType<DarkHeart>());
                                        }
                                    }
                                    
                                    float distance = MathHelper.Lerp(npc.Distance(LockVector1), teleportRadius, 0.1f);

                                    Vector2 desiredPos = (LockVector1 + new Vector2(distance, 0).RotatedBy(rotation));
                                    npc.velocity = desiredPos - npc.Center;
                                }

                                rotationIncrement *= 0.8f;

                                rotation += (float)(rotationIncrement * rotationDirection * (totalTime - timer) / totalTime);
                            }
                            else if (timer < totalTime + 7)
                            {
                                npc.velocity *= 0.96f;
                            }
                            else 
                            {
                                // Set damage
                                npc.damage = npc.defDamage;
                                currentAttack = (float)P2States.Reset; // Go to idle
                                npc.netUpdate = true;
                                npc.netSpam = 0;

                            }
                        }
                        break;
                    case P2States.I_WormDrop:
                        {
                            ref float attackPhase = ref ai3;
                            ref float initialRotation = ref npc.localAI[0];

                            int aboveDistance = 450;
                            Vector2 abovePlayer = target.Center - Vector2.UnitY * aboveDistance;

                            if (attackPhase == 0) // get in position
                            {
                                int repositionTime = 40;
                                float lerp = MathF.Pow(timer / repositionTime, 3);
                                float distance = MathHelper.Lerp(npc.Distance(target.Center), aboveDistance, lerp);

                                Vector2 currentDirection = target.DirectionTo(npc.Center);

                                float rotation = currentDirection.ToRotation() + FargoSoulsUtil.RotationDifference(currentDirection, -Vector2.UnitY) * lerp;
                                Vector2 desiredPos = target.Center + rotation.ToRotationVector2() * distance;
                                npc.velocity = desiredPos - npc.Center;
                                if (timer >= repositionTime)
                                {
                                    timer = 0;
                                    attackPhase = 1;
                                    npc.netUpdate = true;
                                }
                            }
                            else if (attackPhase == 1)
                            {
                                int wormTime = 120;

                                Vector2 toNeutral = abovePlayer - npc.Center;
                                npc.velocity.Y = toNeutral.Y;
                                npc.velocity.X = toNeutral.X * 0.1f;

                                if (timer % 10 == 0)
                                {
                                    if (FargoSoulsUtil.HostCheck)
                                    {
                                        int side = timer % 20 == 0 ? 1 : -1;
                                        float progress = timer / wormTime;

                                        int n = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X + Main.rand.Next(-npc.width / 4, npc.width / 4), (int)npc.Center.Y + Main.rand.Next(-npc.height / 4, npc.height / 4), NPCID.DevourerHead);
                                        if (n.IsWithinBounds(Main.maxNPCs))
                                        {
                                            Main.npc[n].velocity = Vector2.UnitX * side * MathHelper.Lerp(16, 2, progress) + Vector2.UnitY * MathHelper.Lerp(0, 4, progress);
                                        }
                                    }
                                }

                                if (timer >= wormTime)
                                {
                                    SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound, npc.Center);
                                    timer = 0;
                                    attackPhase = 2;
                                    npc.velocity.Y = -7;
                                    npc.netUpdate = true;
                                }
                            }
                            else if (attackPhase == 2)
                            {
                                int dashTime = 75;
                                Vector2 toNeutral = abovePlayer - npc.Center;
                                npc.velocity.Y += 0.4f;
                                npc.velocity.X *= 0.92f;
                                //npc.velocity.X += Math.Sign(toNeutral.X) * 0.4f;
                                npc.velocity.Y = Math.Clamp(npc.velocity.Y, -15f, 15f);

                                if (timer > dashTime)
                                {
                                    timer = 0;
                                    attackPhase = 3;
                                    npc.netUpdate = true;
                                }
                            }

                            if (attackPhase == 3)
                            {
                                int endlagTime = 20;
                                npc.velocity *= 0.97f;
                                if (timer > endlagTime)
                                {
                                    npc.velocity *= 0;
                                    currentAttack = (float)P2States.Reset; // Go to idle
                                    npc.netUpdate = true;
                                    npc.netSpam = 0;
                                }
                                
                            }
                        }
                        break;
                    case P2States.I_RainDashStart:
                        {
                            ref float rotation = ref npc.localAI[0];
                            ref float rotationDirection = ref npc.localAI[1];

                            float startTime = 30;

                            if (timer == 1)
                            {
                                SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.FastRoarSound, npc.Center);
                                npc.velocity = -npc.DirectionTo(target.Center) * 10;
                            }
                            npc.velocity -= npc.velocity.SafeNormalize(Vector2.Zero) * 10f / startTime;

                            if (timer == 5) // before end to give time for netupdate
                            {
                                rotation = Main.rand.NextFloat(MathF.Tau);
                                rotationDirection = Main.rand.NextFromList(1, -1);
                                npc.netUpdate = true;
                            }
                            if (npc.Opacity > 0)
                                npc.Opacity -= 1f / startTime;
                            else
                            {
                                timer = 0;
                                ai3 = 0;
                                currentAttack = (float)P2States.RainDash;
                                npc.velocity = Vector2.Zero;
                            }
                        }
                        break;
                    case P2States.RainDash:
                        {
                            npc.damage = 0;

                            int teleportRadius = 300;
                            float arcTime = 45f; // Ticks needed to complete movement for spawn and rain attacks (DEATH ONLY)

                            ref float rotationDirection = ref npc.localAI[1];

                            if (npc.alpha > 0)
                            {
                                npc.alpha -= 5;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    npc.Center = target.Center;
                                    npc.position.Y -= teleportRadius;
                                    npc.position.X += teleportRadius * rotationDirection;
                                }
                                npc.netUpdate = true;
                                npc.netSpam = 0;
                            }
                            else
                            {
                                if (ai3 == 0)
                                {
                                    // Set damage
                                    npc.damage = npc.defDamage;

                                    ai3 = 1;
                                    SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound, npc.Center);
                                    npc.velocity.X = teleportRadius / arcTime * 3;
                                    npc.velocity *= -rotationDirection;
                                    npc.netUpdate = true;
                                    npc.netSpam = 0;
                                    timer = 0;
                                }
                                else
                                {
                                    // Set damage
                                    npc.damage = npc.defDamage;

                                    if (timer == 4)
                                    {
                                        timer = 0;
                                        ai3++;
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            int type = ModContent.ProjectileType<ShadeNimbusHostile>();
                                            int damage = npc.GetProjectileDamage(type);
                                            Vector2 cloudSpawnPos = new Vector2(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height));
                                            Vector2 randomVelocity = Vector2.Zero;
                                            Projectile.NewProjectile(npc.GetSource_FromAI(), cloudSpawnPos, randomVelocity, type, damage, 0, Main.myPlayer, 11f);
                                        }

                                        if (ai3 == 11)
                                        {
                                            currentAttack = (float)P2States.Decelerate;
                                            ai3 = npc.velocity.Length() / 30f;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case P2States.I_DiagonalDashes:
                        {
                            void Movement(Vector2 pos, float accel = 0.03f, float maxSpeed = 20, float lowspeed = 5, float decel = 0.03f, float slowdown = 30)
                            {
                                if (npc.Distance(pos) > slowdown)
                                {
                                    npc.velocity = Vector2.Lerp(npc.velocity, (pos - npc.Center).SafeNormalize(Vector2.Zero) * maxSpeed, accel);
                                }
                                else
                                {
                                    npc.velocity = Vector2.Lerp(npc.velocity, (pos - npc.Center).SafeNormalize(Vector2.Zero) * lowspeed, decel);
                                }
                            }

                            ref float performedAttacks = ref ai3;
                            int totalAttacks = Subphase(npc) > 1 ? 3 : 2;

                            int windupTime = 60;
                            int dashTime = 50;
                            int endlagTime = 30;
                            Vector2 windupPos = target.Center + Vector2.UnitX * target.HorizontalDirectionTo(npc.Center) * 400 - Vector2.UnitY * 240;

                            if (timer < windupTime)
                            {
                                Movement(windupPos, 0.1f, 40, 10, 0.1f, 50f);
                            }
                            else if (timer == windupTime)
                            {
                                SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound, npc.Center);
                                npc.velocity = npc.DirectionTo(target.Center) * 20f;
                                int spread = 8;
                                float totalSpread = MathF.PI * 0.7f;
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    for (int i = 0; i < spread; i++)
                                    {
                                        float rot = totalSpread * ((float)i / spread - 0.5f);
                                        Vector2 dir = npc.DirectionTo(target.Center).RotatedBy(rot);
                                        int type = ModContent.ProjectileType<GravityVileClot>();
                                        int damage = FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage);
                                        Vector2 spawnPos = npc.Center + dir * npc.height / 2;
                                        Vector2 vel = dir * 14f;
                                        Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPos, vel, type, damage, 0, Main.myPlayer);
                                    }
                                }
                            }
                            else if (timer <= windupTime + dashTime)
                            {
                                npc.velocity.Y *= 0.97f;
                            }
                            else
                            {
                                performedAttacks++;
                                if (performedAttacks >= totalAttacks)
                                {
                                    npc.velocity *= 0.96f;
                                    if (timer >= windupTime + dashTime + endlagTime)
                                    {
                                        currentAttack = (float)P2States.Reset; // Go to idle
                                        npc.netUpdate = true;
                                        npc.netSpam = 0;
                                    }
                                }
                                else
                                {
                                    timer = 0;
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
                                currentAttack = (float)P2States.Reset; // Go to idle
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
            if (npc.Distance(target.Center) > 900 && burrowTimer > 5)
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
