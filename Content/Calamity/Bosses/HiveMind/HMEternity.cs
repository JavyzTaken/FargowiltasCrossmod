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
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static FargowiltasSouls.Content.Bosses.Magmaw.Magmaw;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HMEternity : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.HiveMind.HiveMind>();

        public override void SetDefaults()
        {
            if (!WorldSavingSystem.EternityMode) return;
            NPC.scale = 0.01f;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 5000000;
            }
            else
            {
                NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 1.175f);
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.buffImmune[BuffID.Oiled] = true;
            NPC.buffImmune[ModContent.BuffType<OiledBuff>()] = true;
        }

        public Vector2 sprite = new Vector2(0, 0);
        public int frameCounter = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            Asset<Texture2D> ground = ModContent.Request<Texture2D>("CalamityMod/NPCs/HiveMind/HiveMind");
            Asset<Texture2D> fly = ModContent.Request<Texture2D>("CalamityMod/NPCs/HiveMind/HiveMindP2");

            Color glowColor = HiveMindPulse.GlowColor;
            if (sprite.X == 0)
            {
                Main.EntitySpriteDraw(ground.Value, NPC.Center - Main.screenPosition - new Vector2(0, 10), new Rectangle(0, 122 * (int)sprite.Y, 178, 122), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(178, 122) / 2, NPC.scale, SpriteEffects.None);
            }
            else
            {
                bool desperation = (Subphase(NPC) >= 3);
                Color afterimageColor = desperation ? glowColor : drawColor;
                for (int i = 0; i < (int)currentAfterimages; i++)
                {
                    
                    Main.EntitySpriteDraw(fly.Value, NPC.oldPos[i] + new Vector2(NPC.width / 2, NPC.height / 2) - Main.screenPosition + new Vector2(0, 10), new Rectangle(178 * ((int)sprite.X - 1), 142 * (int)sprite.Y, 178, 142), NPC.GetAlpha(afterimageColor) * (1 - i / 10f) * 0.4f, NPC.rotation, new Vector2(178, 142) / 2, NPC.scale, SpriteEffects.None);
                }
                if (desperation && NPC.Opacity > 0.5f)
                    DLCUtils.DrawBackglow(fly, NPC.GetAlpha(glowColor) * NPC.Opacity, NPC.Center + new Vector2(0, 10), new Vector2(178, 142) / 2, NPC.rotation, NPC.scale, offsetMult: 2, sourceRectangle: new Rectangle(178 * ((int)sprite.X - 1), 142 * (int)sprite.Y, 178, 142));
                
                if (NPC.ai[1] == (float)P2States.I_OffscreenDash1 && NPC.ai[0] < 50)
                {
                    Player target = Main.player[NPC.target];
                    Asset<Texture2D> line = TextureAssets.Extra[178];

                    float opacity = MathHelper.Lerp(0, 1, NPC.ai[0] / 90);
                    Main.EntitySpriteDraw(line.Value, NPC.Center - Main.screenPosition, null, Color.Lime * opacity, NPC.DirectionTo(target.Center).ToRotation(), new Vector2(0, line.Height() * 0.5f), new Vector2(0.5f, NPC.scale * 8), SpriteEffects.None);
                }
                Main.EntitySpriteDraw(fly.Value, NPC.Center - Main.screenPosition + new Vector2(0, 10), new Rectangle(178 * ((int)sprite.X - 1), 142 * (int)sprite.Y, 178, 142), NPC.GetAlpha(drawColor), NPC.rotation, new Vector2(178, 142) / 2, NPC.scale, SpriteEffects.None);
            }

            return false;
        }
        public override void DrawBehind(int index)
        {
            if (!WorldSavingSystem.EternityMode) return;
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }
        public override void FindFrame(int frameHeight)
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
        public bool DidRainDash = false;

        public static float Subphase2HP => WorldSavingSystem.MasochistModeReal ? 0.65f : 0.5f;
        public static float Subphase3HP => WorldSavingSystem.MasochistModeReal ? 0.25f : 0.175f;
        public static int Subphase(NPC NPC)
        {
            float life = NPC.GetLifePercent();
            return life < Subphase2HP ? life < Subphase3HP ? 3 : 2 : 1;
        }

        public float targetAfterimages = 0;
        public float currentAfterimages = 0;

        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(Phase);
            binaryWriter.Write7BitEncodedInt(LastAttack);
            binaryWriter.Write(DidRainDash);
            binaryWriter.WriteVector2(sprite);
            binaryWriter.WriteVector2(LockVector1);
            binaryWriter.Write(NPC.localAI[2]);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            Phase = binaryReader.Read7BitEncodedInt();
            LastAttack = binaryReader.Read7BitEncodedInt();
            DidRainDash = binaryReader.ReadBoolean();
            sprite = binaryReader.ReadVector2();
            LockVector1 = binaryReader.ReadVector2();
            NPC.localAI[2] = binaryReader.ReadSingle();
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
        public override bool PreAI()
        {
            if (!WorldSavingSystem.EternityMode) return true;

            Main.windSpeedTarget = Main.windSpeedCurrent = 0.4f;
            //sets rain time to 4 hours
            int day = 86400;
            int hour = day / 24;
            if (Main.rainTime < hour * 0.5f)
                Main.rainTime = hour * 0.5f;
            Main.raining = true;
            if (Main.maxRaining < 0.6f)
                Main.maxRaining += 0.01f;
            Main.cloudAlpha = Main.maxRaining;

            CalamityGlobalNPC.hiveMind = NPC.whoAmI;

            ref float timer = ref NPC.ai[0];
            ref float currentAttack = ref NPC.ai[1];
            ref float attackCounter = ref NPC.ai[2];
            ref float ai3 = ref NPC.ai[3];
            
            if (!Targeting())
            {
                return false;
            }
            Player target = Main.player[NPC.target];
            Vector2 toTarget = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
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
            targetAfterimages = Math.Min(5, NPC.velocity.Length());
            Vector2 bottom = NPC.Bottom;
            NPC.width = (int)(150 * NPC.scale);
            NPC.height = (int)(100 * NPC.scale);
            NPC.Bottom = bottom;

            Main.LocalPlayer.ZoneCorrupt = true;

            //ai :real:
            #region Phase 0: Spawn animation
            if (Phase == 0)
            {
                NPC.defense = 200;
                NPC.damage = 0;
                NPC.alpha = 0;
                for (int i = 0; i < timer / 100f; i++)
                {
                    Dust.NewDustDirect(NPC.Center - new Vector2(100, 0), 200, 0, DustID.Corruption);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Player player = Main.player[Main.myPlayer];
                    PunchCameraModifier modifier = new(player.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), (timer / 60f), 1f, 60, 100f, FullName);
                    Main.instance.CameraModifiers.Add(modifier);
                }
                if (timer < 300)
                {
                    NPC.Center += NPC.DirectionTo(target.Center) * 5;
                    NPC.dontTakeDamage = true;
                }
                timer++;
                if (timer >= 300 && timer < 320)
                {
                    NPC.scale += 0.05f;

                }
                if (timer == 300)
                {
                    SoundEngine.PlaySound(roar with { Pitch = -0.5f }, NPC.Center);
                    ScreenShakeSystem.StartShake(15);
                    if (DLCUtils.HostCheck)
                    {
                        int maxBlobs = BlobEternity.P1Blobs;

                        for (int i = 0; i < maxBlobs; i++)
                        {
                            Vector2 offset = Main.rand.NextVector2Circular(150f / 2f, 100f / 2f);
                            int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + offset.X), (int)(NPC.Center.Y + offset.Y), ModContent.NPCType<HiveBlob>(), NPC.whoAmI);
                            if (n.IsWithinBounds(Main.maxNPCs))
                            {
                                NPC blob = Main.npc[n];
                                if (blob.TypeAlive<HiveBlob>())
                                {
                                    blob.velocity = offset / 2;
                                }
                            }
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HiveMindPulse>(), 0, 0, ai1: 10);

                    }
                    NetSync(NPC);
                }
                if (timer >= 320)
                {
                    Phase = 1;
                    NPC.dontTakeDamage = false;
                    timer = 0;
                    ai3 = 60 * 7;
                    for (int i = 0; i < 100f; i++)
                    {
                        Dust.NewDustDirect(NPC.Center - new Vector2(100, 0), 200, 0, DustID.Corruption, 0, -5);
                    }
                    NPC.scale = 1;
                    NetSync(NPC);
                }

            }
            else
            {

                if (Phase == 1 && ai3 > 0) // not during burrow
                {
                    NPC.scale = 1; //to avoid mp issues
                    NPC.alpha = 0;
                }
                    
            }
            #endregion
            #region Phase 1: Stationary
            if (Phase == 1)
            {
                ref float burrowTimer = ref NPC.ai[3];

                timer++;

                NPC.noGravity = false;
                NPC.noTileCollide = false;
                NPC.chaseable = false;
                NPC.damage = 0;
                NPC.defense = 200;
                NPC.life = NPC.lifeMax;

                //Aura();
                void Aura()
                {
                    float distance = NPC.Distance(Main.LocalPlayer.Center);
                    float threshold = 600f;
                    Player player = Main.LocalPlayer;
                    for (int l = 0; l < 16; l++)
                    {
                        double rad2 = Main.rand.NextFloat(MathF.Tau);
                        double dustdist2 = Main.rand.NextFloat(threshold, threshold + 300);
                        int DustX2 = (int)NPC.Center.X - (int)(Math.Cos(rad2) * dustdist2);
                        int DustY2 = (int)NPC.Center.Y - (int)(Math.Sin(rad2) * dustdist2);
                        int DustType = Main.rand.NextFromList(DustID.Corruption);
                        int i = Dust.NewDust(new Vector2(DustX2, DustY2), 1, 1, DustType, Scale: Main.rand.NextFloat(1f, 1.5f));
                        Main.dust[i].noGravity = true;
                        Main.dust[i].velocity = Vector2.Normalize(NPC.Center - Main.dust[i].position) * Main.rand.Next(2, 5);
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

                            Vector2 movement = NPC.Center - player.Center;
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
              

                CalamityBurrow(NPC, target);

                if (((!NPC.AnyNPCs(ModContent.NPCType<HiveBlob>()) && !NPC.AnyNPCs(ModContent.NPCType<HiveBlob2>())) || timer > 60 * 21) && burrowTimer > 0)
                {
                    SoundEngine.PlaySound(roar with { Pitch = 0.5f }, NPC.Center);
                    Phase = 2;
                    sprite.X = 1;
                    NPC.velocity = -NPC.DirectionTo(target.Center) * 10;
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.defense = NPC.defDefense;
                    NPC.frame.Y = 0;
                    NPC.scale = 1f;
                    NPC.alpha = 0;
                    NPC.dontTakeDamage = false;
                    NPC.damage = 0;
                    NPC.netSpam = 0;
                    NPC.netUpdate = true;
                    NPC.chaseable = true;

                    if (NPC.life > NPC.lifeMax * 0.75f)
                    {
                        int aliveBlobs = NPC.CountNPCS(ModContent.NPCType<HiveBlob>()) + NPC.CountNPCS(ModContent.NPCType<HiveBlob2>());
                        float damageMult = 1 - LumUtils.Saturate(aliveBlobs / BlobEternity.P1Blobs);
                        NPC.SimpleStrikeNPC((int)Math.Round(damageMult * (NPC.life - (NPC.lifeMax * 0.75f))), 1);
                    }
                    foreach (NPC n in Main.npc)
                    {
                        if ((n.type == ModContent.NPCType<HiveBlob>() || n.type == ModContent.NPCType<HiveBlob2>()) && n.ai[0] == NPC.whoAmI)
                        {
                            n.StrikeInstantKill();
                        }
                    }
                    SoundEngine.PlaySound(SoundID.NPCDeath23, NPC.Center);
                    NetSync(NPC);
                    for (int i = 0; i < 3; i++)
                    {
                        NPC.ai[i] = 0;
                        NPC.localAI[i] = 0;
                    }
                        
                }
            }
            #endregion
            #region Phase 2: Mobile
            const int MidwayIdleStart = 60 * 4 + 10;
            const int IdleEnd = 60 * 9 - 20;
            if (Phase >= 2)
            {
                NPC.damage = NPC.defDamage;
                Main.NewText(currentAttack);
                Main.NewText(timer);
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
                                NPC.localAI[i] = 0;
                            }
                            if (Subphase(NPC) >= 2 && Main.rand.NextBool(2) && !DidRainDash && LastAttack != (float)P2States.I_SpindashStart)
                            {
                                currentAttack = (float)P2States.I_RainDashStart;
                                DidRainDash = true;
                                timer = 0;
                                if (attackCounter == 3) // if this replaced this replaced spin dashes
                                    attackCounter = 2;
                                NPC.netUpdate = true;
                            }
                            else
                                DidRainDash = false;
                            if (Subphase(NPC) > 2)
                            {
                                currentAttack = (float)P2States.I_SpindashStart;
                                timer = 0;
                            }
                        }
                        break;
                    case P2States.Idle: // idle float, spawn some shit as a shield
                        {
                            if (WorldSavingSystem.MasochistModeReal && timer < MidwayIdleStart)
                                timer = MidwayIdleStart;

                            targetAfterimages = 0;
                            NPC.damage = 0;

                            if (NPC.alpha > 0)
                                NPC.alpha -= 3;
                            else
                                NPC.alpha = 0;

                            int creeperCount = attackCounter == 0 ? 4 : 3;
                            var creepers = Main.npc.Where(n => n.TypeAlive<DankCreeper>());
                            if (timer % 40 == 0 && creepers.Count() < creeperCount)
                                SpawnCreepers(1);
                            void SpawnCreepers(int count)
                            {
                                if (DLCUtils.HostCheck)
                                {
                                    for (int i = 0; i < count; i++)
                                    {
                                        int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-NPC.width / 2, NPC.width / 2), (int)NPC.Center.Y + Main.rand.Next(-NPC.height / 2, NPC.height / 2), ModContent.NPCType<DankCreeper>(), ai0: NPC.whoAmI);
                                        if (n.IsWithinBounds(Main.maxNPCs))
                                        {
                                            Main.npc[n].velocity = Main.rand.NextVector2Circular(3, 3);
                                        }
                                    }
                                }
                            }
                            float speedMod = MathF.Min(1f, timer / 60f);
                            float speed = 16 * speedMod;
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(target.Center - NPC.Center) * speed, 0.02f);

                            if (timer == MidwayIdleStart + 10 || timer == MidwayIdleStart + 100 && WorldSavingSystem.MasochistModeReal && Subphase(NPC) > 1)
                            {
                                if (DLCUtils.HostCheck)
                                {
                                    if ((attackCounter > 0 && Subphase(NPC) > 1) || WorldSavingSystem.MasochistModeReal)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<ShadeLightningCloud>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                                    
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
                                NPC.netUpdate = true;
                                NPC.netSpam = 0;

                                if (attackCounter == 3 || Subphase(NPC) >= 3)
                                {
                                    currentAttack = (float)P2States.I_SpindashStart;
                                }
                                else
                                {
                                    List<P2States> attacks = [P2States.I_OffscreenDash1, P2States.I_WormDrop, P2States.I_DiagonalDashes];
                                    attacks.Remove((P2States)LastAttack);

                                    currentAttack = (float)Main.rand.NextFromCollection(attacks);
                                }
                                LastAttack = (int)currentAttack;
                                
                                timer = 0;
                                ai3 = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        break;
                    case P2States.I_OffscreenDash1: // back off, dash,  go offscreen and dash in from offscreen at 90 degree angle
                        {
                            ref float dashRotation = ref NPC.localAI[1];

                            targetAfterimages = 10;
                            if (timer == 1) // set random direction, wait 5 frames to sync for mp
                            {
                                dashRotation = (Main.rand.NextBool() ? 1 : -1) * Main.rand.NextFloat(0.35f, 0.5f);
                                NPC.netUpdate = true;
                            }
                            if (timer < 6)
                                break;
                            if (timer == 6) // start of attack
                            {
                                NPC.velocity = -NPC.DirectionTo(target.Center).RotatedBy(MathF.PI * dashRotation) * 25;
                                SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound, NPC.Center);
                            }
                            float attackTime = 120f + 5f;
                            float fadeTime = 100f + 5f;

                            bool fade = timer > fadeTime;

                            float telegraphTime = 50 + 5;
                            bool accelerate = timer > telegraphTime;
                            float accelStraightTime = telegraphTime + 0f;

                            if (timer < telegraphTime)
                            {
                                NPC.velocity *= 0.965f;

                                if (timer > telegraphTime - 10)
                                {
                                    float lerp = (float)(timer - (telegraphTime - 10)) / 10f;
                                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(target.Center) * 15f, lerp);
                                }
                            }
                            else if (timer == telegraphTime)
                            {
                                NPC.velocity = NPC.DirectionTo(target.Center) * 15f;
                                SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.FastRoarSound, NPC.Center);
                            }

                            //NPC.velocity = Vector2.Lerp(NPC.velocity, LockVector1, 0.02f);
                            if (fade)
                            {
                                float fadeDuration = (attackTime - fadeTime);
                                //NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, (timer - fadeTime) / (attackTime - fadeTime));
                                NPC.Opacity -= 1f / fadeDuration;
                            }

                            if (accelerate) //accelerate during dash towards player
                            {
                                const float accel = 1.12f;
                                const float maxSpeed = 30;
                                Vector2 accelDir = NPC.SafeDirectionTo(Main.player[NPC.target].Center);
                                if (timer > accelStraightTime)
                                {
                                    float lerp = Math.Min(((float)timer - accelStraightTime) / (accelStraightTime + 5f), 1);
                                    accelDir = Vector2.Lerp(accelDir, Vector2.Normalize(NPC.velocity), lerp).SafeNormalize(accelDir);
                                }
                                    
                                NPC.velocity += accelDir * accel;
                                if (NPC.velocity.LengthSquared() > maxSpeed * maxSpeed)
                                {
                                    NPC.velocity = Vector2.Normalize(NPC.velocity) * maxSpeed;
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
                            const float maxSpeed = 25f;
                            targetAfterimages = 10;
                            if (timer == 1)
                            {
                                Vector2 currentVelocity = NPC.velocity;
                                Vector2 newVelocity = currentVelocity.RotatedBy(MathF.PI * 0.5f * (Main.rand.NextBool() ? 1 : -1));
                                Vector2 newPosition = target.Center - Vector2.Normalize(newVelocity) * 1200;
                                NPC.Center = newPosition;
                                NPC.velocity = newVelocity;
                                NPC.netUpdate = true;
                                NPC.velocity = NPC.velocity.ClampMagnitude(0f, maxSpeed);

                                SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound with { Pitch = -0.5f }, NPC.Center);
                            }
                            if (timer % 9 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                                if (FargoSoulsUtil.HostCheck)
                                {

                                }
                                for (int i = -1; i < 2; i += 2)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.rand.NextVector2FromRectangle(NPC.Hitbox), -NPC.velocity.RotatedBy(MathF.PI / 3f * i) * 0.8f, ModContent.ProjectileType<BrainMassProjectile>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai1: 1);
                                }
                            }
                            float dashTime = 80;
                            float decelTime = 25;
                            if (Math.Abs(FargoSoulsUtil.RotationDifference(NPC.velocity, NPC.DirectionTo(target.Center))) > MathHelper.PiOver2)
                                if (timer < dashTime)
                                    timer = dashTime;
                            bool decel = timer >= dashTime;
                            bool accelerate = timer < dashTime;

                            float fadeinTime = 15f;
                            if (NPC.Opacity < 1)
                                NPC.Opacity += 1f / fadeinTime;

                            if (decel)
                            {
                                NPC.velocity *= 0.96f;
                            }
                            if (accelerate) //accelerate during dash towards player
                            {
                                const float accel = 0.85f;
                                Vector2 accelDir = NPC.DirectionTo(Main.player[NPC.target].Center);
                                NPC.velocity += accelDir * accel;
                                if (NPC.velocity.LengthSquared() > maxSpeed * maxSpeed)
                                {
                                    NPC.velocity = Vector2.Normalize(NPC.velocity) * maxSpeed;
                                }
                            }
                            if (timer > dashTime + decelTime)
                            {
                                NPC.velocity *= 0.9f;
                                NPC.Opacity = 1f;
                                if (timer > dashTime + decelTime + 10)
                                    currentAttack = (float)P2States.Reset;
                            }
                        }
                        break;
                    case P2States.I_SpindashStart: // start spin dash, by dashing away and fading
                        {
                            ref float rotation = ref NPC.localAI[0];
                            ref float rotationDirection = ref NPC.localAI[1];

                            float startTime = 30;

                            if (timer == 5) // before end to give time for netupdate
                            {
                                rotation = Main.rand.NextFloat(MathF.Tau);
                                rotationDirection = Main.rand.NextFromList(1, -1);
                                int delayBase = WorldSavingSystem.MasochistModeReal ? 40 : 60;
                                NPC.localAI[2] = delayBase + Main.rand.Next(15);
                                NPC.netUpdate = true;

                                if (Subphase(NPC) > 2)
                                {
                                    SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.FastRoarSound, NPC.Center);
                                    NPC.velocity = -NPC.DirectionTo(target.Center) * 10;
                                }
                            }
                            if (NPC.Opacity > 0)
                                NPC.Opacity -= 1f / startTime;
                            if (timer == startTime)
                            {
                                timer = 0;
                                currentAttack = (float)P2States.Spindash;
                            }
                            if (timer > startTime)
                                timer = 0;
                        }
                        break;
                    case P2States.Spindash: // multi-cal spin
                        {
                            ref float rotation = ref NPC.localAI[0];
                            ref float rotationDirection = ref NPC.localAI[1];
                            ref float dashes = ref NPC.localAI[3];
                            int totalDashes = Subphase(NPC) switch
                            {
                                3 => 9999,
                                2 => 4,
                                _ => 2
                            };
                            if (WorldSavingSystem.MasochistModeReal)
                                totalDashes += 2;

                            int lungeFade = WorldSavingSystem.MasochistModeReal ? 11 : 15; // Divide 255 by this for duration of hive mind spin before slowing for lunge
                            double lungeRots = 0.4;
                            double rotationIncrement = 0.0246399424 * lungeRots * lungeFade;

                            ref float lungeDelay = ref NPC.localAI[2]; // # of ticks long hive mind spends sliding to a stop before lunging

                            int teleportRadius = 300;
                            float lungeTime = 23;

                            if (Subphase(NPC) > 2)
                            {
                                SkyManager.Instance.Activate("FargowiltasCrossmod:HiveMind", target.Center);
                            }

                            NPC.netUpdate = true;
                            NPC.netSpam = 0;
                            if (NPC.alpha > 0)
                            {
                                NPC.alpha -= lungeFade;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float distance = teleportRadius;
                                    if (dashes > 0)
                                        distance = MathHelper.Lerp(NPC.Distance(target.Center), teleportRadius, 0.1f);
                                    Vector2 desiredVel = (target.Center + new Vector2(distance, 0).RotatedBy(rotation)) - NPC.Center;
                                    NPC.velocity = desiredVel;

                                }

                                rotation += (float)(rotationIncrement * rotationDirection);
                                timer = lungeDelay;
                            }
                            else
                            {
                                NPC.alpha = 0;
                                timer -= 2;
                                if (ai3 != 1)
                                {
                                    if (timer <= 0)
                                    {

                                        timer = lungeTime;
                                        NPC.velocity = target.Center  - NPC.Center;
                                        NPC.velocity.Normalize();
                                        NPC.velocity *= teleportRadius / (lungeTime);
                                        NPC.velocity *= 1.2f;
                                        ai3 = 1;
                                        int delayBase = WorldSavingSystem.MasochistModeReal ? 35 : 60;
                                        int randomBase = WorldSavingSystem.MasochistModeReal ? 35 : 15;
                                        NPC.localAI[2] = delayBase + Main.rand.Next(randomBase);
                                        SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.FastRoarSound, NPC.Center);
                                        NPC.netUpdate = true;
                                    }
                                    else
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            float distance = teleportRadius;
                                            if (dashes > 0)
                                                distance = MathHelper.Lerp(NPC.Distance(target.Center), teleportRadius, MathHelper.Lerp(1, 0.025f, timer / lungeDelay));
                                            Vector2 desiredPos = (target.Center + new Vector2(distance, 0).RotatedBy(rotation));
                                            NPC.velocity = desiredPos - NPC.Center;
                                        }

                                        if (dashes > 0)
                                            rotationIncrement *= 0.6f;

                                        rotation += (float)(rotationIncrement * rotationDirection * timer / lungeDelay);
                                    }
                                }
                                else
                                {

                                    NPC.velocity *= 1.02f;
                                    targetAfterimages = 10;

                                    float dif = FargoSoulsUtil.RotationDifference(NPC.velocity, NPC.DirectionTo(target.Center));
                                    if (Math.Abs(dif) >= MathHelper.PiOver2 && timer <= lungeTime / 2)
                                    {
  
                                        if (dashes < totalDashes - 1)
                                        {
                                            dashes++;
                                            ai3 = 0;
                                            rotation = target.DirectionTo(NPC.Center).ToRotation();
                                            rotationDirection = Math.Sign(FargoSoulsUtil.RotationDifference(NPC.DirectionTo(target.Center), (NPC.Center + NPC.velocity).DirectionTo(target.Center)));
                                            timer = lungeDelay;

                                            if (Subphase(NPC) > 2 && dashes > 3 && Main.rand.NextBool(2))
                                            {
                                                dashes = 0;
                                                currentAttack = (float)P2States.I_RainDashStart;
                                                timer = 0;
                                            }
                                        }
                                        else if (timer <= 0)
                                        {
                                            currentAttack = (float)P2States.FinalSpindash; // Final spin
                                            LockVector1 = target.Center;
                                            dashes = 0;
                                            
                                            timer = 0;
                                            ai3 = 0;
                                            rotation = target.DirectionTo(NPC.Center).ToRotation();
                                            rotationDirection = Math.Sign(FargoSoulsUtil.RotationDifference(NPC.DirectionTo(target.Center), (NPC.Center + NPC.velocity).DirectionTo(target.Center)));
                                        }
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                        }
                        break;
                    case P2States.FinalSpindash: // Spin out, lock spin center, then spin in to center
                        {
                            targetAfterimages = 10;
                            ref float rotation = ref NPC.localAI[0];
                            ref float rotationDirection = ref NPC.localAI[1];
                            ref float dashes = ref NPC.localAI[3];

                            int lungeFade = 15; // Divide 255 by this for duration of hive mind spin before slowing for lunge
                            double lungeRots = 0.4;
                            double rotationIncrement = 0.0246399424 * lungeRots * lungeFade;
                            int teleportRadius = 400;

                            NPC.netUpdate = true;
                            NPC.netSpam = 0;

                            int totalTime = 160;
                            int spinoutTime = 40;

                            if (timer == spinoutTime)
                            {
                                SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound, NPC.Center);
                            }

                            if (timer < totalTime)
                            {
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    float progress = timer / totalTime;
                                    if (timer <= spinoutTime)
                                    {
                                        LockVector1 = target.Center;
                                        teleportRadius = (int)MathHelper.Lerp(0, teleportRadius, timer / spinoutTime);
                                        if (teleportRadius < NPC.Distance(LockVector1))
                                            teleportRadius = (int)NPC.Distance(LockVector1);
                                    }
                                    else
                                    {
                                        float spininProg = (timer - spinoutTime) / (totalTime - spinoutTime);
                                        teleportRadius = (int)MathHelper.Lerp(teleportRadius, 0, spininProg);

                                        if (Subphase(NPC) >= 2 && timer % 25 == 0 && timer < totalTime - 40 && NPC.CountNPCS(ModContent.NPCType<DarkHeart>()) < 1)
                                        {
                                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + Main.rand.Next(NPC.width), (int)NPC.position.Y + Main.rand.Next(NPC.height), ModContent.NPCType<DarkHeart>());
                                        }
                                    }
                                    
                                    float distance = MathHelper.Lerp(NPC.Distance(LockVector1), teleportRadius, 0.1f);

                                    Vector2 desiredPos = (LockVector1 + new Vector2(distance, 0).RotatedBy(rotation));
                                    NPC.velocity = desiredPos - NPC.Center;
                                }

                                rotationIncrement *= 0.8f;

                                rotation += (float)(rotationIncrement * rotationDirection * (totalTime - timer) / totalTime);
                            }
                            else if (timer < totalTime + 7)
                            {
                                NPC.velocity *= 0.96f;
                            }
                            else 
                            {
                                currentAttack = (float)P2States.Reset; // Go to idle
                                NPC.netUpdate = true;
                                NPC.netSpam = 0;

                            }
                        }
                        break;
                    case P2States.I_WormDrop:
                        {
                            ref float attackPhase = ref ai3;
                            ref float initialRotation = ref NPC.localAI[0];

                            int aboveDistance = 450;
                            Vector2 abovePlayer = target.Center - Vector2.UnitY * aboveDistance;

                            if (attackPhase == 0) // get in position
                            {
                                int repositionTime = 80;
                                float lerp = Math.Min(MathF.Pow(timer / repositionTime, 3), 1);
                                float distance = MathHelper.Lerp(NPC.Distance(target.Center), aboveDistance, 1);

                                Vector2 currentDirection = target.DirectionTo(NPC.Center);

                                float rotation = currentDirection.ToRotation() + FargoSoulsUtil.RotationDifference(currentDirection, -Vector2.UnitY) * 0.2f;
                                Vector2 desiredPos = target.Center + rotation.ToRotationVector2() * distance;

                                if (NPC.Distance(abovePlayer) > 40 && timer < 60)
                                    Movement(desiredPos, 0.4f, 40, 40, 0.4f, 0);
                                else
                                {
                                    //NPC.velocity = Vector2.Lerp(NPC.velocity, abovePlayer - NPC.Center, 0.05f);
                                    timer = 0;
                                    attackPhase = 1;
                                    NPC.netUpdate = true;
                                }

                                
                            }
                            else if (attackPhase == 1)
                            {
                                int wormTime = 120;

                                Vector2 toNeutral = abovePlayer - NPC.Center;
                                Movement(abovePlayer, 0.1f, 10, 6, 0.1f, 50f);
                                //NPC.velocity.Y = toNeutral.Y;
                                //NPC.velocity.X = toNeutral.X * 0.1f;

                                if (timer % 10 == 0)
                                {
                                    int side = timer % 20 == 0 ? 1 : -1;
                                    if (side == 1)
                                        SoundEngine.PlaySound(SoundID.NPCDeath13 with { Volume = 0.4f }, NPC.Center);
                                    if (FargoSoulsUtil.HostCheck)
                                    {
                                        
                                        float progress = timer / wormTime;

                                        int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-NPC.width / 6, NPC.width / 6), (int)NPC.Center.Y + Main.rand.Next(-NPC.height / 6, NPC.height / 6), NPCID.DevourerHead);
                                        if (n.IsWithinBounds(Main.maxNPCs))
                                        {
                                            NPC worm = Main.npc[n];
                                            worm.velocity = Vector2.UnitX * side * MathHelper.Lerp(16, 2, progress) + Vector2.UnitY * MathHelper.Lerp(0, 4, progress);
                                            worm.GetGlobalNPC<DevourerEternityHM>().FromHM = true;
                                            worm.life = worm.lifeMax *= 7;
                                            worm.damage = worm.defDamage = NPC.defDamage;
                                        }
                                    }
                                }

                                if (timer >= wormTime)
                                {
                                    SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound, NPC.Center);
                                    timer = 0;
                                    attackPhase = 2;
                                    NPC.velocity.Y = -7;
                                    NPC.netUpdate = true;
                                }
                            }
                            else if (attackPhase == 2)
                            {
                                targetAfterimages = 10;
                                int dashTime = 75;
                                Vector2 toNeutral = abovePlayer - NPC.Center;
                                NPC.velocity.Y += 0.4f;
                                NPC.velocity.X *= 0.99f;
                                //NPC.velocity.X += Math.Sign(toNeutral.X) * 0.4f;
                                NPC.velocity.Y = Math.Clamp(NPC.velocity.Y, -15f, 15f);

                                if (timer > dashTime)
                                {
                                    timer = 0;
                                    attackPhase = 3;
                                    NPC.netUpdate = true;
                                }
                            }

                            if (attackPhase == 3)
                            {
                                int endlagTime = 20;
                                NPC.velocity *= 0.97f;
                                if (timer > endlagTime)
                                {
                                    for (int i = 0; i < Main.maxNPCs; i++)
                                    {
                                        NPC otherNPC = Main.npc[i];
                                        if (otherNPC.TypeAlive(NPCID.DevourerHead))
                                        {
                                            //otherNPC.StrikeInstantKill();
                                        }
                                    }
                                    NPC.velocity *= 0;
                                    currentAttack = (float)P2States.Reset; // Go to idle
                                    NPC.netUpdate = true;
                                    NPC.netSpam = 0;
                                }
                                
                            }
                        }
                        break;
                    case P2States.I_RainDashStart:
                        {
                            ref float rotation = ref NPC.localAI[0];
                            ref float rotationDirection = ref NPC.localAI[1];

                            float startTime = 30;

                            if (timer == 1)
                            {
                                SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.FastRoarSound, NPC.Center);
                                NPC.velocity = -NPC.DirectionTo(target.Center) * 10;
                            }
                            NPC.velocity -= NPC.velocity.SafeNormalize(Vector2.Zero) * 10f / startTime;

                            if (timer == 5) // before end to give time for netupdate
                            {
                                rotation = Main.rand.NextFloat(MathF.Tau);
                                rotationDirection = Main.rand.NextFromList(1, -1);
                                NPC.netUpdate = true;
                            }
                            if (NPC.Opacity > 0)
                                NPC.Opacity -= 1f / startTime;
                            else
                            {
                                timer = 0;
                                ai3 = 0;
                                currentAttack = (float)P2States.RainDash;
                                NPC.velocity = Vector2.Zero;
                            }
                        }
                        break;
                    case P2States.RainDash:
                        {

                            int teleportRadius = 300;
                            float arcTime = 45f; // Ticks needed to complete movement for spawn and rain attacks

                            ref float rotationDirection = ref NPC.localAI[1];

                            if (NPC.alpha > 0)
                            {
                                NPC.alpha -= 5;
                                if (Subphase(NPC) > 2)
                                    NPC.alpha -= 3;
                                if (WorldSavingSystem.MasochistModeReal)
                                    NPC.alpha -= 4;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    NPC.Center = target.Center;
                                    NPC.position.Y -= teleportRadius;
                                    NPC.position.X += teleportRadius * rotationDirection;

                                    float x = NPC.alpha / 255f;
                                    float factor = (x - x * x) * 4;
                                    NPC.position.X += (260 + factor * 100) * rotationDirection;
                                }
                                NPC.netUpdate = true;
                                NPC.netSpam = 0;
                            }
                            else
                            {
                                if (ai3 == 0)
                                {

                                    ai3 = 1;
                                    SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound, NPC.Center);
                                    NPC.velocity.X = teleportRadius / arcTime * 3;
                                    NPC.velocity.X *= 1.2f;
                                    NPC.velocity *= -rotationDirection;
                                    NPC.netUpdate = true;
                                    NPC.netSpam = 0;
                                    timer = 0;
                                }
                                else
                                {
                                    targetAfterimages = 10;

                                    if (timer == 4)
                                    {
                                        timer = 0;
                                        ai3++;
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            int type = ModContent.ProjectileType<ShadeNimbusHostile>();
                                            int damage = NPC.GetProjectileDamage(type);
                                            Vector2 cloudSpawnPos = new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height));
                                            Vector2 randomVelocity = Vector2.Zero;
                                            int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), cloudSpawnPos, randomVelocity, type, damage, 0, Main.myPlayer, 11f);
                                            if (p.IsWithinBounds(Main.maxProjectiles))
                                            {
                                                Main.projectile[p].extraUpdates += 1;
                                                Main.projectile[p].timeLeft *= 2;
                                            }
                                        }

                                        if (ai3 == 13)
                                        {
                                            if (Subphase(NPC) < 3)
                                            {
                                                currentAttack = (float)P2States.Decelerate;
                                                ai3 = NPC.velocity.Length() / 30f;
                                            }
                                            else
                                            {
                                                currentAttack = (float)P2States.I_SpindashStart;
                                                ai3 = 0;
                                                timer = 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case P2States.I_DiagonalDashes:
                        {

                            ref float performedAttacks = ref ai3;
                            int totalAttacks = Subphase(NPC) > 1 ? 3 : 2;

                            int windupTime = 60;
                            int dashTime = 50;
                            int endlagTime = 30;
                            Vector2 windupPos = target.Center + Vector2.UnitX * target.HorizontalDirectionTo(NPC.Center) * 400 - Vector2.UnitY * 240;

                            if (timer < windupTime)
                            {
                                Movement(windupPos, 0.1f, 40, 10, 0.1f, 50f);
                            }
                            else if (timer == windupTime)
                            {
                                SoundEngine.PlaySound(CalamityMod.NPCs.HiveMind.HiveMind.RoarSound, NPC.Center);
                                NPC.velocity = NPC.DirectionTo(target.Center) * 20f;
                                int spread = 10;
                                float totalSpread = MathF.PI * 0.875f;
                                if (WorldSavingSystem.MasochistModeReal)
                                    totalSpread *= 0.8f;
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    for (int i = 0; i < spread; i++)
                                    {
                                        float rot = totalSpread * ((float)i / spread - 0.5f);
                                        Vector2 dir = NPC.DirectionTo(target.Center).RotatedBy(rot);
                                        int type = ModContent.ProjectileType<GravityVileClot>();
                                        int damage = FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage);
                                        Vector2 spawnPos = NPC.Center + dir * NPC.height / 2;
                                        Vector2 vel = dir * 12.5f;
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, vel, type, damage, 0, Main.myPlayer);
                                    }
                                }
                            }
                            else if (timer <= windupTime + dashTime)
                            {
                                NPC.velocity.Y *= 0.97f;
                            }
                            else
                            {
                                performedAttacks++;
                                if (performedAttacks >= totalAttacks)
                                {
                                    NPC.velocity *= 0.96f;
                                    if (timer >= windupTime + dashTime + endlagTime)
                                    {
                                        currentAttack = (float)P2States.Reset; // Go to idle
                                        NPC.netUpdate = true;
                                        NPC.netSpam = 0;
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
                            NPC.velocity -= NPC.velocity.SafeNormalize(Vector2.Zero) * ai3;
                            float rotDif = FargoSoulsUtil.RotationDifference(NPC.velocity, toTarget);
                            NPC.velocity = NPC.velocity.RotatedBy(rotDif / 50f);
                            if (timer > 30)
                            {
                                NPC.velocity *= 0;
                                currentAttack = (float)P2States.Reset; // Go to idle
                                NPC.netUpdate = true;
                                NPC.netSpam = 0;
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

            void Movement(Vector2 pos, float accel = 0.03f, float maxSpeed = 20, float lowspeed = 5, float decel = 0.03f, float slowdown = 30)
            {
                accel *= 16;
                decel *= 16;

                float resistance = NPC.velocity.Length() * accel / (maxSpeed);
                NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, pos, NPC.velocity, accel - resistance, decel + resistance);
                /*
                if (NPC.Distance(pos) > slowdown)
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * maxSpeed, accel);
                }
                else
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (pos - NPC.Center).SafeNormalize(Vector2.Zero) * lowspeed, decel);
                }
                */
            }

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
                        if (NPC.timeLeft == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                FargoSoulsUtil.ClearHostileProjectiles(2, NPC.whoAmI);
                            }
                        }
                        return false;
                    }
                }
                return true;
            }
        }
        public void CalamityBurrow(NPC NPC, Player target)
        {
            ref float burrowTimer = ref NPC.ai[3];
            burrowTimer--;
            if (NPC.Distance(target.Center) > 900 && burrowTimer > 5)
            {
                burrowTimer = 5;
            }
            if (burrowTimer < -120)
            {
                burrowTimer = 60 * 7;
                if (burrowTimer < 30)
                    burrowTimer = 30;

                NPC.scale = 1f;
                NPC.alpha = 0;
                NPC.dontTakeDamage = false;
            }
            else if (burrowTimer < -60)
            {
                NPC.scale += 0.0165f;
                NPC.alpha -= 4;

                int burrowedDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, DustID.Demonite, 0f, -3f, 100, default, 2.5f * NPC.scale);
                Main.dust[burrowedDust].velocity *= 2f;
                if (Main.rand.NextBool())
                {
                    Main.dust[burrowedDust].scale = 0.5f;
                    Main.dust[burrowedDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }

                for (int i = 0; i < 2; i++)
                {
                    int burrowedDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, DustID.Demonite, 0f, -3f, 100, default, 3.5f * NPC.scale);
                    Main.dust[burrowedDust2].noGravity = true;
                    Main.dust[burrowedDust2].velocity *= 3.5f;
                    burrowedDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, DustID.Demonite, 0f, -3f, 100, default, 2.5f * NPC.scale);
                    Main.dust[burrowedDust2].velocity *= 1f;
                }
            }
            else if (burrowTimer == -60)
            {
                NPC.scale = 0.01f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.Center = target.Center;
                    NPC.position.Y = target.position.Y - NPC.height;
                    int tilePosX = (int)NPC.Center.X / 16;
                    int tilePosY = (int)(NPC.position.Y + NPC.height) / 16 + 1;

                    while (!(Main.tile[tilePosX, tilePosY].HasUnactuatedTile && Main.tileSolid[Main.tile[tilePosX, tilePosY].TileType]))
                    {
                        tilePosY++;
                        NPC.position.Y += 16;
                    }
                }
                NPC.netUpdate = true;
                NPC.netSpam = 0;
            }
            else if (burrowTimer < 0)
            {
                NPC.scale -= 0.0165f;
                NPC.alpha += 4;

                int burrowedDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, DustID.Demonite, 0f, -3f, 100, default, 2.5f * NPC.scale);
                Main.dust[burrowedDust].velocity *= 2f;
                if (Main.rand.NextBool())
                {
                    Main.dust[burrowedDust].scale = 0.5f;
                    Main.dust[burrowedDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }

                for (int i = 0; i < 2; i++)
                {
                    int burrowedDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, DustID.Demonite, 0f, -3f, 100, default, 3.5f * NPC.scale);
                    Main.dust[burrowedDust2].noGravity = true;
                    Main.dust[burrowedDust2].velocity *= 3.5f;
                    burrowedDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, DustID.Demonite, 0f, -3f, 100, default, 2.5f * NPC.scale);
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
                    NPC.TargetClosest();
                    NPC.dontTakeDamage = true;
                }
            }
        }
    }
}
