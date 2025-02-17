using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.Perforator;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasCrossmod.Core.Common.InverseKinematics;
using FargowiltasSouls;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PerfsEternityNew : CalDLCEmodeBehavior
    {
        public const bool Enabled = false;
        public override bool IsLoadingEnabled(Mod mod) => Enabled;
        public override int NPCOverrideID => ModContent.NPCType<PerforatorHive>();

        #region Fields
        // Basic targeting and movement fields
        public Player Target => Main.player[NPC.target];
        public const int HeightAboveGround = 275;
        public float Acceleration => State == (int)States.MoveToPlayer ? 0.4f : 0.2f;
        public static float MaxMovementSpeed => 12f;
        #region Fight Related

        public float SpawnProgress;
        public static int SpawnTime = 60 * 3;

        public ref float State => ref NPC.ai[0];
        public ref float Timer => ref NPC.ai[1];
        public ref float AI2 => ref NPC.ai[2];
        public ref float AI3 => ref NPC.ai[3];
        public ref float AI4 => ref NPC.localAI[3];
        public ref float NextState => ref NPC.localAI[0];
        public ref float Phase => ref NPC.localAI[2];
        public int MediumWormCooldown = 0;
        public List<States> RecentAttacks = [];
        public bool PhaseTwo => Phase > 0;
        public enum States
        {
            // misc
            Opening = 0,
            MoveToPlayer,
            // attacks
            SmallWorm,
            MediumWorm,
            BigWorm,
            RubbleStomp,
            LegAssault,
            // phase 2
            GroundSpikes,
            GroundSpikesAngled,
        }
        public List<States> Attacks
        {
            get
            {
                List<States> attacks =
                    [
                    States.SmallWorm,
                    States.RubbleStomp,
                    States.LegAssault,
                    States.BigWorm,
                    ];
                if (MediumWormCooldown <= 0)
                    attacks.Add(States.MediumWorm);
                if (PhaseTwo)
                {
                    attacks.Add(States.GroundSpikes);
                    attacks.Add(States.GroundSpikesAngled);
                }
                return attacks;
            }
        }
        #endregion

        #region Legs Related

        public Vector2 GravityDirection => Vector2.UnitY;

        public PerforatorLeg[] Legs;
        public int[][][] LegSprites; // don't ask
        public Vector2[] LegBraces;

        public bool WasWalkingUpward
        {
            get => NPC.ai[2] == 1f;
            set => NPC.ai[2] = value.ToInt();
        }

        public static LazyAsset<Texture2D>[] LegTextures = new LazyAsset<Texture2D>[4];
        public static LazyAsset<Texture2D>[] LegEndTextures = new LazyAsset<Texture2D>[4];
        public static LazyAsset<Texture2D>[] LegJointTextures = new LazyAsset<Texture2D>[4];

        public static float LegSizeFactor => 3.5f;
        public static int[] JointParts;

        public const int LegPartLength = 80;
        public const int JointLength = 28;
        #endregion 

        #endregion Fields and Properties
        public override void SetStaticDefaults()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                var path = "FargowiltasCrossmod/Assets/ExtraTextures/PerfLegs/";
                for (int i = 0; i < 4; i++)
                {
                    int alt = Main.rand.Next(i) + 1;
                    LegTextures[i] = LazyAsset<Texture2D>.Request($"{path}PerfLeg{alt}");
                    LegEndTextures[i] = LazyAsset<Texture2D>.Request($"{path}PerfLegEnd{alt}");
                    LegJointTextures[i] = LazyAsset<Texture2D>.Request($"{path}PerfLegJoint{alt}");
                }
            }
        }
        public override void SetDefaults()
        {
            if (!WorldSavingSystem.EternityMode) return;
            NPC.lifeMax = (int)(NPC.lifeMax * 1.6f);
            NPC.noGravity = true;
            if (BossRushEvent.BossRushActive)
            {
                NPC.lifeMax = 5000000;
            }
            NPC.Opacity = 0;
            NPC.dontTakeDamage = true;

            // cursed 3d array ahead
            Legs = new PerforatorLeg[4];
            LegSprites = new int[Legs.Length][][];
            LegBraces = new Vector2[Legs.Length];
            JointParts = [3, 5];

            for (int i = 0; i < Legs.Length; i++)
            {
                float horizontalOffset;
                float verticalOffset;
                if (i % 2 == 0)
                {
                    horizontalOffset = 90 * (i == 0 ? 1 : -1);
                    verticalOffset = 130;
                }
                else
                {
                    horizontalOffset = 70 * (i == 1 ? 1 : -1);
                    verticalOffset = 110;
                }

                Vector2 legOffset = new(horizontalOffset, verticalOffset);
                
                Legs[i] = new(LegSizeFactor * legOffset, LegSizeFactor, legOffset.Length() * 0.685f * 0.45f, legOffset.Length() * 0.685f, i);
                Legs[i].Leg[0].Rotation = legOffset.ToRotation();
                Legs[i].Leg[1].Rotation = Vector2.UnitY.ToRotation();

                LegSprites[i] = new int[2][];
                for (int j = 0; j < 2; j++)
                {
                    LegSprites[i][j] = new int[JointParts[j]];
                    for (int k = 0; k < JointParts[j]; k++)
                    {
                        LegSprites[i][j][k] = Main.rand.Next(4);
                    }
                }

                float spriteLength = 80 + 22;
                float angle = -Math.Sign(horizontalOffset);
                float angleMult = i % 2 == 0 ? 2.4f : 0.7f;
                angle *= MathHelper.PiOver2 * 0.22f * angleMult;

                LegBraces[i] = Vector2.UnitY.RotatedBy(angle) * spriteLength * 1;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            //NPC.Center -= Vector2.UnitY * 1000;
        }

        #region Draw
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode)
                return true;

            // draw leg braces
            if (Legs is not null)
            {
                if (NPC.IsABestiaryIconDummy)
                {
                    for (int j = 0; j < Legs.Length; j++)
                        Legs[j]?.Update(NPC);
                }

                DrawLegBraces(Legs, NPC.GetAlpha(drawColor), screenPos);
            }

            // draw hive
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            float rotation = NPC.rotation / 5;
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = PerforatorHive.GlowTexture.Value;
            Color glowmaskColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);
            glowmaskColor = NPC.GetAlpha(glowmaskColor);

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, glowmaskColor, rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            // draw legs proper
            if (Legs is not null)
            {
                if (NPC.IsABestiaryIconDummy)
                {
                    for (int j = 0; j < Legs.Length; j++)
                        Legs[j]?.Update(NPC);
                }

                DrawLegSet(Legs, NPC.GetAlpha(drawColor), screenPos);
            }
            return false;
        }
        public static void DrawLeg(SpriteBatch spriteBatch, Texture2D legTexture, Vector2 start, Vector2 end, Color color, float width, SpriteEffects direction, bool glow)
        {
            // Draw nothing if the start and end are equal, to prevent division by 0 problems.
            if (start == end)
                return;

            float rotation = (end - start).ToRotation();
            Vector2 scale = new(Vector2.Distance(start, end) / legTexture.Width, width);
            start.Y += 2f;

            if (glow)
            {
                Main.spriteBatch.UseBlendState(BlendState.Additive);
                for (int j = 0; j < 8; j++)
                {
                    Vector2 afterimageOffset = (rotation + (MathHelper.Pi * (j - 4) / 8f)).ToRotationVector2() * 4f;
                    spriteBatch.Draw(legTexture, start + afterimageOffset, null, color, rotation, legTexture.Size() * Vector2.UnitY * 0.5f, scale, direction, 0f);
                }
                Main.spriteBatch.ResetToDefault();
            }

            spriteBatch.Draw(legTexture, start, null, color, rotation, legTexture.Size() * Vector2.UnitY * 0.5f, scale, direction, 0f);
        }
        public void DrawLegBraces(PerforatorLeg[] legs, Color lightColor, Vector2 screenPos)
        {
            for (int i = 0; i < legs.Length; i++)
            {
                if (legs[i] is null)
                    continue;

                KinematicChain leg = legs[i].Leg;
                if (leg.JointCount <= 0)
                    continue;

                bool glow = legs[i].DamageTime > 0;

                // draw leg brace
                Vector2 dir = LegBraces[i].SafeNormalize(Vector2.Zero);
                Vector2 start = NPC.Center - screenPos;
                Vector2 end = start + dir * 80;
                SpriteEffects direction = (LegBraces[i].X).NonZeroSign() == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
                DrawLeg(Main.spriteBatch, LegTextures[0].Value, start, end, lightColor, 1f, direction, glow: false);
                // leg end
                start = end + dir * 22;
                DrawLeg(Main.spriteBatch, LegJointTextures[0].Value, start, end, lightColor, 1f, direction, glow: false);
            }
        }
        public void DrawLegSet(PerforatorLeg[] legs, Color lightColor, Vector2 screenPos)
        {
            for (int i = 0; i < legs.Length; i++)
            {
                if (legs[i] is null)
                    continue;

                KinematicChain leg = legs[i].Leg;
                if (leg.JointCount <= 0)
                    continue;

                bool glow = legs[i].DamageTime > 0;

                // draw leg
                Vector2 previousPosition = leg.StartingPoint;
                for (int j = 0; j < leg.JointCount; j++)
                {
                    int jointParts = JointParts[j];
                    int joints = j == 0 ? 2 : 1;
                    float jointDiv = ((float)JointLength / LegPartLength);
                    float partLength = leg[j].Offset.Length() / ((jointParts - joints) + (jointDiv * joints));
                    float jointLength = partLength * jointDiv;
                    float accumulatedLength = 0;
                    for (int k = 0; k < jointParts; k++)
                    {
                        bool legEnd = false;
                        bool joint = false;
                        int spriteIndex = LegSprites[i][j][k];
                        Texture2D legTexture;
                        bool flip = false;
                        if (j == 0)
                            flip = true;
                        if (k == jointParts - 1)
                        {
                            if (j == 0)
                            {
                                joint = true;
                                legTexture = LegJointTextures[spriteIndex].Value;
                            }
                            else
                            {
                                legTexture = LegEndTextures[spriteIndex].Value;
                                legEnd = true;
                            }
                        }
                        else
                        {
                            if (k == 0)
                            {
                                joint = true;
                                legTexture = LegJointTextures[spriteIndex].Value;
                                if (flip)
                                    flip = false;
                            }
                            else
                                legTexture = LegTextures[spriteIndex].Value;
                        }
                        Vector2 partOffset = leg[j].Offset.SafeNormalize(Vector2.Zero) * (joint ? jointLength : partLength);

                        int spriteDir = (leg.EndEffectorPosition.X - LegBraces[i].X).NonZeroSign();
                        if (k == 0 && j == 0)
                            spriteDir *= -1;
                        SpriteEffects direction = spriteDir == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
                        Vector2 start = previousPosition - screenPos;
                        Vector2 end = previousPosition + partOffset - screenPos;

                        if (flip)
                            (start, end) = (end, start);

                        DrawLeg(Main.spriteBatch, legTexture, start, end, lightColor, 1f, direction, glow: glow && legEnd);
                        previousPosition += partOffset;
                        accumulatedLength += partOffset.Length();
                    }
                }
            }
        }
        #endregion
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }


        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            for (int i = 0; i < NPC.localAI.Length; i++)
                binaryWriter.Write(NPC.localAI[i]);
            binaryWriter.Write7BitEncodedInt(MediumWormCooldown);
            for (int i = 0; i < RecentAttacks.Count; i++)
                binaryWriter.Write7BitEncodedInt((int)RecentAttacks[i]);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            for (int i = 0; i < NPC.localAI.Length; i++)
                NPC.localAI[i] = binaryReader.ReadSingle();
            MediumWormCooldown = binaryReader.Read7BitEncodedInt();
            for (int i = 0; i < RecentAttacks.Count; i++)
                RecentAttacks[i] = (States)binaryReader.Read7BitEncodedInt();
        }
        #region AI
        public override bool PreAI()
        {
            if (!WorldSavingSystem.EternityMode) return true;

            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
                NetSync(NPC);
            }
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.velocity.Y += 1;
                ManageLegs();
                return false;
            }

            //low ground
            if (Main.LocalPlayer.active && !Main.LocalPlayer.ghost && !Main.LocalPlayer.dead && NPC.Distance(Main.LocalPlayer.Center) < 2000)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<LowGroundBuff>(), 2);

            // manage global timers
            if (MediumWormCooldown > 0)
                MediumWormCooldown--;

            switch ((States)State)
            {
                case States.Opening:
                    Opening();
                    break;
                case States.MoveToPlayer:
                    MoveToPlayerForAttack();
                    break;
                case States.SmallWorm:
                    SmallWorm();
                    break;
                case States.MediumWorm:
                    MediumWorm();
                    break;
                case States.BigWorm:
                    BigWorm();
                    break;
                case States.RubbleStomp:
                    RubbleStomp();
                    break;
                case States.LegAssault:
                    LegAssault();
                    break;
                case States.GroundSpikes:
                    GroundSpikes();
                    break;
                case States.GroundSpikesAngled:
                    GroundSpikesAngled();
                    break;
            }
            ManageLegs();

            return false;
        }
        public void ManageLegs()
        {
            // Look forward
            Vector2 forwardDirection = Vector2.UnitX * NPC.SafeDirectionTo(Target.Center).X.NonZeroSign();
            //float idealRotation = NPC.velocity.X * 0.05f + NPC.velocity.Y * NPC.spriteDirection * 0.097f + forwardDirection.ToRotation();
            if (NPC.velocity.Length() >= 4f && Math.Sign(NPC.velocity.X) == (int)forwardDirection.X)
                NPC.spriteDirection = (int)forwardDirection.X;
            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X / 10, 0.05f);
            //NPC.rotation = NPC.rotation.AngleTowards(idealRotation, 0.09f).AngleLerp(idealRotation, 0.03f);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < Legs.Length; j++)
                    Legs[j]?.Update(NPC);
            }
        }
        #region State Methods
        public void Opening()
        {
            if (Timer < 1)
            {
                // Ensure that legs are already grounded when the Perforator has fully spawned in.
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < Legs.Length; j++)
                        Legs[j]?.Update(NPC);
                }

                Timer += 1f / SpawnTime;
                if (Timer < 0.8f)
                    NPC.Opacity = 0f;
                else if (Timer < 1f)
                {
                    if (NPC.Opacity == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath23 with { Pitch = -0.3f }, NPC.Center);
                        for (int i = 0; i < 160; i++)
                        {
                            Vector2 offset = Main.rand.NextVector2CircularEdge(NPC.width / 2.5f, NPC.height / 2.5f) * Main.rand.NextFloat(0.1f, 1f);
                            Color color = Main.rand.NextBool(0.1f) ? Color.Gold : Color.Red;
                            Particle p = new BloodParticle(NPC.Center + offset, offset.SafeNormalize(-Vector2.UnitY) * Main.rand.NextFloat(5, 30), 50, 1, color);
                            GeneralParticleHandler.SpawnParticle(p);
                        }
                    }
                    NPC.Opacity = (Timer - 0.8f) / 0.2f;
                    NPC.dontTakeDamage = false;
                }
                else
                {
                    NPC.Opacity = 1f;
                    NPC.dontTakeDamage = false;
                    NPC.netUpdate = true;
                }
                WalkToPositionAI(Target.Center);
                SpawnProgress = Timer;
            }
            else
            {
                SpawnProgress = 1;
                GoToNeutral();
            }
        }
        public void MoveToPlayerForAttack()
        {
            float speed = 0.5f;
            if (Timer > 60)
                speed += (Timer - 60) / 60f;
            if (PhaseTwo)
                speed *= 1.5f;
            if (!NPC.HasPlayerTarget)
                return;
            Vector2 targetPos = AttackStartOffset(Target.Center, (int)NextState, out int leniency);
            if (targetPos.HasNaNs())
                return;
            WalkToPositionAI(targetPos, speed);
            Timer++;
            int minimumWait = PhaseTwo ? 35 : 60;
            if (Timer < minimumWait)
                return;
            if (Math.Abs(targetPos.X - NPC.Center.X) < leniency)
            {
                Reset();
                State = NextState;
                return;
            }
        }
        public Vector2 AttackStartOffset(Vector2 target, int State, out int leniency)
        {
            leniency = 100;
            switch ((States)State)
            {
                // far attacks
                case States.RubbleStomp:
                case States.GroundSpikesAngled:
                    target += Vector2.UnitX * target.SafeDirectionTo(NPC.Center).X * 770;
                    leniency = 30;
                    break;

                case States.LegAssault:
                    target += Vector2.UnitX * target.SafeDirectionTo(NPC.Center).X * 650;
                    leniency = 50;
                    break;

                default:
                    leniency = 200;
                    break;
            }
            return target;
        }
        public void SmallWorm()
        {
            ref float ChosenLeg = ref AI2;

            float speed = 0.2f;
            float dist = Target.Distance(NPC.Center);
            if (dist > 300)
                speed += (dist - 300) / 500;
            WalkToPositionAI(Target.Center, speed, 315);
            int interval = 80;
            if (Timer % interval == 0)
            {
                SoundEngine.PlaySound(SoundID.NPCHit20, NPC.Center);
                if (DLCUtils.HostCheck)
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorHeadSmall>());
            }

            /*
            if (Timer == 0)
            {
                ChosenLeg = ClosestLegIndex(Target.Center);
                NPC.netUpdate = true;
                var leg = Legs[(int)ChosenLeg];
                int sign = Math.Sign(leg.GetEndPoint().X - Target.Center.X);
                Vector2 pos = Target.Center + new Vector2(sign * 100, -200);
                leg.StartCustomAnimation(NPC, pos, 1f / stabTelegraphTime);
            }
            if (Timer == stabTelegraphTime)
            {
                var leg = Legs[(int)ChosenLeg];
                Vector2 offset = Vector2.Normalize(Target.Center - leg.GetEndPoint()) * 60;
                leg.StartCustomAnimation(NPC, Target.Center + offset, 1f / stabTime);
            }
            */
            float time = WorldSavingSystem.MasochistModeReal ? 4.9f : 3.9f;
            if (++Timer >= (int)(interval * time))
            {
                GoToNeutral();
            }
                
        }
        public void MediumWorm()
        {
            /*
            ref float WormCount = ref AI2;
            int wormSegments = 10;
            int wormFrequency = 15;
            WalkToPositionAI(Target.Center, 0.6f);
            if (Timer % wormFrequency == 0 && WormCount < wormSegments)
            {
                int segmentType = ModContent.NPCType<MediumWormBodySegment>();
                if (WormCount == 0)
                    segmentType = ModContent.NPCType<MediumWormHeadSegment>();
                if (WormCount == wormSegments - 1)
                    segmentType = ModContent.NPCType<MediumWormTailSegment>();

                NPC minion = NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center - Vector2.UnitY * NPC.height / 3, segmentType);
                minion.velocity = -Vector2.UnitY * 10 + Vector2.UnitX * Main.rand.NextFloat(-7, 7);
                NetSync(minion);
                WormCount++;
            }
            if (++Timer > wormFrequency * wormSegments + 90)
            {
                GoToNeutral();
            }
            */
            WalkToPositionAI(Target.Center, 0.6f);
            if (Timer == 0)
            {
                if (DLCUtils.HostCheck)
                {
                    var minion = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - NPC.height / 3, ModContent.NPCType<PerforatorHeadMedium>());
                    minion.GetGlobalNPC<MediumPerforator>().VelocityReal = -Vector2.UnitY * 24;
                }
                SoundEngine.PlaySound(SoundID.NPCDeath23, NPC.Center);
            }
            if (++Timer > 150)
            {
                GoToNeutral();
                //MediumWormCooldown = 60 * 25;
            }
        }
        public void BigWorm()
        {
            int expTelegraph = WorldSavingSystem.MasochistModeReal ? 100 : 85;
            int endTime = 150;
            if (NPC.velocity.X.NonZeroSign() == NPC.HorizontalDirectionTo(Target.Center))
                NPC.velocity *= 0.99f;
            else
                NPC.velocity *= 0.92f;
            if (Timer < expTelegraph) 
            {
                // shake
                float max = MathHelper.PiOver2 * 1.2f * Timer / expTelegraph;
                NPC.rotation = max * MathF.Sin(Timer * MathHelper.Pi / (expTelegraph / 11));
                if (Timer % 3 == 0)
                {
                    Vector2 offset = Main.rand.NextVector2CircularEdge(NPC.width / 2.5f, NPC.height / 2.5f);
                    Color color = Main.rand.NextBool(0.1f) ? Color.Gold : Color.Red;
                    Particle p = new BloodParticle(NPC.Center + offset, offset.SafeNormalize(-Vector2.UnitY) * Main.rand.NextFloat(10, 20), 50, 1, color);
                    GeneralParticleHandler.SpawnParticle(p);
                }
            }
            if (Timer == 10) // explosion telegraph
            {
                if (DLCUtils.HostCheck)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PerfExplosion>(), 0, 0);
                }
            }
            if (Timer == expTelegraph)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath23 with { Pitch = -0.3f }, NPC.Center);
                for (int i = 0; i < 80; i++)
                {
                    Vector2 offset = Main.rand.NextVector2CircularEdge(NPC.width / 2.5f, NPC.height / 2.5f) * Main.rand.NextFloat(0.1f, 1f);
                    Color color = Main.rand.NextBool(0.1f) ? Color.Gold : Color.Red;
                    Particle p = new BloodParticle(NPC.Center + offset, offset.SafeNormalize(-Vector2.UnitY) * Main.rand.NextFloat(5, 30), 50, 1, color);
                    GeneralParticleHandler.SpawnParticle(p);
                }
                if (DLCUtils.HostCheck)
                {
                    Projectile p = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PerfExplosion>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: NPC.whoAmI);

                    var minion = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - NPC.height / 3, ModContent.NPCType<PerforatorHeadLarge>());
                    minion.GetGlobalNPC<LargePerforator>().VelocityReal = -Vector2.UnitY * 18 + Vector2.UnitX * NPC.HorizontalDirectionTo(Target.Center) * 3;

                    for (int i = 0; i < 60; i++)
                    {
                        float shotSpeed = Main.rand.NextFloat(4f, 20f);
                        Vector2 shotDir = -Vector2.UnitY.RotatedByRandom(MathHelper.Pi / 2f);
                        Vector2 vel = shotDir * shotSpeed;
                        if (vel.Y < -6)
                            vel.Y *= 0.6f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextFloat() * shotDir * NPC.width / 2f, vel, ModContent.ProjectileType<IchorShot>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: NPC.whoAmI);
                    }
                }
            }
            if (++Timer > expTelegraph + endTime)
            {
                GoToNeutral();
            }
        }
        public void RubbleStomp()
        {
            NPC.velocity *= 0.8f;
            int startTime = 5;
            int stabTelegraphTime = 30;
            int stabTime = 10;

            float cycle = (stabTelegraphTime + stabTime);
            float cyclicalTimer = (Timer - startTime) % cycle;
            if ((Timer - startTime) < cycle * 2)
            {
                if (cyclicalTimer < stabTelegraphTime)
                    NPC.velocity.Y -= 0.2f;
                if (cyclicalTimer > stabTelegraphTime && cyclicalTimer < stabTelegraphTime + stabTime)
                    NPC.velocity.Y += 0.3f;
            }
            
            void TelegraphStab(int numStab)
            {
                ref float ChosenLeg = ref AI2;
                if (numStab == 0)
                    ChosenLeg = ClosestLegIndex(Target.Center);
                else
                    ChosenLeg = ClosestLegIndex(Legs[(int)ChosenLeg].GetEndPoint(), [(int)ChosenLeg]);
                NPC.netUpdate = true;
                var leg = Legs[(int)ChosenLeg];
                Vector2 defaultPos = leg.DefaultPosition(this);
                defaultPos.Y = NPC.Center.Y;
                defaultPos = FindGround(defaultPos.ToTileCoordinates(), GravityDirection, "C").ToWorldCoordinates();
                Vector2 pos = defaultPos + new Vector2(0, -200);
                leg.StartCustomAnimation(NPC, pos, 0.5f / stabTelegraphTime);
            }
            void Stab()
            {
                ref float ChosenLeg = ref AI2;
                NPC.netUpdate = true;
                var leg = Legs[(int)ChosenLeg];
                Vector2 endPoint = leg.GetEndPoint();
                int dir = Math.Sign(endPoint.X - NPC.Center.X);
                Vector2 pos = endPoint + new Vector2(dir * 90, 0);
                pos = FindGround(pos.ToTileCoordinates(), GravityDirection, "D").ToWorldCoordinates();
                leg.StartCustomAnimation(NPC, pos, 0.5f / stabTime, animationMode: PerforatorLeg.Accel, stepSound: true);
                leg.SetAnimationEndAction((PerforatorLeg leg, NPC npc) =>
                {
                    Vector2 endPoint = leg.GetEndPoint();
                    if (DLCUtils.HostCheck)
                    {
                        int dir = Math.Sign(endPoint.X - npc.Center.X);
                        int rubbleCount = 9;
                        for (int i = 0; i < rubbleCount; i++)
                        {
                            Vector2 vel = new Vector2(dir * 7, -12).RotatedByRandom(MathHelper.PiOver2 * 0.25f) * Main.rand.NextFloat(0.2f, 1f);
                            Projectile p = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), endPoint, vel, ModContent.ProjectileType<PerforatorRubble>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0);
                            if (p != null)
                            {
                                p.extraUpdates = 1;
                            }
                        }
                    }
                });
            }
            if (Timer == startTime)
                TelegraphStab(0);
            if (Timer == startTime + stabTelegraphTime)
                Stab();
            if (Timer == startTime + stabTelegraphTime + stabTime)
                TelegraphStab(1);
            if (Timer == startTime + stabTelegraphTime + stabTime + stabTelegraphTime)
                Stab();
            if (++Timer > 160)
            {
                GoToNeutral();
            }
        }
        public void LegAssault()
        {
            float walkbackTime = 25;
            float windupTime = 40;
            float stabTime = 15;
            void TelegraphStab()
            {
                ref float ChosenLeg1 = ref AI2;
                ref float ChosenLeg2 = ref AI3;
                // inner left and right leg
                if (Target.Center.X > NPC.Center.X) // right two
                {
                    ChosenLeg1 = 0;
                    ChosenLeg2 = 1;
                }
                else
                {
                    ChosenLeg1 = 2;
                    ChosenLeg2 = 3;
                }

                NPC.netUpdate = true;
                for (int i = 0; i < 2; i++)
                {
                    int index = (int)(i == 0 ? ChosenLeg1 : ChosenLeg2);
                    var leg = Legs[index];
                    Vector2 defaultPos = leg.DefaultPosition(this);
                    defaultPos.Y = NPC.Center.Y;
                    defaultPos = FindGround(defaultPos.ToTileCoordinates(), GravityDirection, "C").ToWorldCoordinates();
                    Vector2 pos = defaultPos + new Vector2(0, -250);
                    pos.X += MathF.Sign(NPC.Center.X - pos.X) * -40;
                    // remember that each leg updates 2 times per second
                    leg.StartCustomAnimation(NPC, pos, 0.5f / windupTime);
                    leg.DamageTime = 2 * (int)(windupTime + stabTime);

                    leg.SetAnimationEndAction((PerforatorLeg leg, NPC npc) =>
                    {
                        Vector2 endPoint = leg.GetEndPoint();
                        Vector2 pos = Target.Center + Target.velocity * stabTime / 2;
                        pos += endPoint.DirectionTo(pos) * 80;
                        //pos = endPoint + endPoint.DirectionTo(pos) * 380;
                        leg.StartCustomAnimation(NPC, pos, 0.5f / stabTime, animationMode: PerforatorLeg.Accel, stepSound: true);
                        leg.DamageTime = 2 * (int)(stabTime);

                        if (DLCUtils.HostCheck)
                        {
                            Projectile.NewProjectileDirect(npc.GetSource_FromAI(), endPoint, Vector2.Zero, ModContent.ProjectileType<PerforatorLegHitbox>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0, ai0: npc.whoAmI, ai1: index, ai2: stabTime);
                        }
                    });
                }
            }

            if (Timer < windupTime)
            {

            }

            if (Timer == 0)
            {
                AI4 = NPC.HorizontalDirectionTo(Target.Center);
                TelegraphStab();
            }
            else if (Timer == windupTime)
            {
                //Stab();
            }
            if (Timer < walkbackTime)
            {
                NPC.velocity.Y -= 0.35f;
                WalkToPositionAI(Target.Center - AI4 * 500 * Vector2.UnitX, 1.2f, 450);
                NPC.velocity *= 0.94f;

                NPC.rotation = MathHelper.PiOver2 * 2f * ((float)Timer / walkbackTime) * -AI4;
            }
            else if (Timer - walkbackTime < 15)
            {
                NPC.velocity.Y *= 0.95f;
                NPC.velocity.Y += 0.5f;
                NPC.velocity.X += AI4 * 0.6f;
            }
            else
            {
                NPC.velocity *= 0.94f;
            }
            if (++Timer > windupTime + stabTime + 10)
                GoToNeutral();
        }
        public void GroundSpikes()
        {
            ClearGore();

            NPC.velocity *= 0.8f;
            int stabTelegraphTime = WorldSavingSystem.MasochistModeReal ? 45 : 35;
            int stabTime = 10;
            if (Timer < 25)
                NPC.velocity.Y -= 0.5f;
            if (Timer > stabTelegraphTime && Timer < stabTelegraphTime + stabTime)
                NPC.velocity.Y += 0.7f;

            void TelegraphStab()
            {
                ref float ChosenLeg1 = ref AI2;
                ref float ChosenLeg2 = ref AI3;
                // inner left and right leg
                ChosenLeg1 = 1; 
                ChosenLeg2 = 3;

                NPC.netUpdate = true;
                for (int i = 0; i < 2; i++)
                {
                    var leg = i == 0 ? Legs[(int)ChosenLeg1] : Legs[(int)ChosenLeg2];
                    Vector2 defaultPos = leg.DefaultPosition(this);
                    defaultPos.Y = NPC.Center.Y;
                    defaultPos = FindGround(defaultPos.ToTileCoordinates(), GravityDirection, "C").ToWorldCoordinates();
                    Vector2 pos = defaultPos + new Vector2(0, -200);
                    pos.X += MathF.Sign(NPC.Center.X - pos.X) * 40;
                    leg.StartCustomAnimation(NPC, pos, 0.5f / stabTelegraphTime);
                }
            }
            void Stab()
            {
                ref float ChosenLeg1 = ref AI2;
                ref float ChosenLeg2 = ref AI3;
                NPC.netUpdate = true;
                for (int i = 0; i < 2; i++)
                {
                    var leg = i == 0 ? Legs[(int)ChosenLeg1] : Legs[(int)ChosenLeg2];
                    Vector2 endPoint = leg.GetEndPoint();
                    int dir = Math.Sign(endPoint.X - NPC.Center.X);
                    Vector2 pos = endPoint + new Vector2(dir * -80);
                    pos = FindGround(pos.ToTileCoordinates(), GravityDirection, "D").ToWorldCoordinates();
                    pos.Y += 40;
                    leg.StartCustomAnimation(NPC, pos, 0.5f / stabTime, animationMode: PerforatorLeg.Accel, stepSound: true);
                    if (i == 0)
                    {
                        leg.SetAnimationEndAction((PerforatorLeg leg, NPC npc) =>
                        {
                            if (!npc.HasPlayerTarget)
                                return;
                            int spacing = WorldSavingSystem.MasochistModeReal ? 115 : 140;
                            int random = 20;
                            if (DLCUtils.HostCheck)
                            {
                                for (int i = -5; i < 5; i++)
                                {
                                    Vector2 pos = Main.player[npc.target].Center + Vector2.UnitX * i * spacing;
                                    pos.X += Main.rand.NextFloat(-random, random);
                                    pos = FindGround(pos.ToTileCoordinates(), GravityDirection, "E").ToWorldCoordinates();

                                    Vector2 vel = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2 * 0.23f);

                                    Projectile.NewProjectile(npc.GetSource_FromAI(), pos, vel, ModContent.ProjectileType<PerforatorSpike>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0);
                                }
                            }
                        });
                    }
                }
            }

            if (Timer == 0)
                TelegraphStab();
            if (Timer == stabTelegraphTime)
                Stab();
            if (++Timer > 120)
                GoToNeutral();
        }
        public void GroundSpikesAngled()
        {
            ClearGore();

            NPC.velocity *= 0.8f;
            if (Timer < 25)
                NPC.velocity.Y -= 0.5f;
            int stabTelegraphTime = WorldSavingSystem.MasochistModeReal ? 45 : 35;
            int stabTime = 10;

            void TelegraphStab()
            {
                ref float ChosenLeg = ref AI2;
                ChosenLeg = ClosestLegIndex(Target.Center);
                NPC.netUpdate = true;
                var leg = Legs[(int)ChosenLeg];
                Vector2 defaultPos = leg.DefaultPosition(this);
                defaultPos.Y = NPC.Center.Y;
                defaultPos = FindGround(defaultPos.ToTileCoordinates(), GravityDirection, "C").ToWorldCoordinates();
                Vector2 pos = defaultPos + new Vector2(0, -200);
                leg.StartCustomAnimation(NPC, pos, 0.5f / stabTelegraphTime);
            }
            void Stab()
            {
                ref float ChosenLeg = ref AI2;
                NPC.netUpdate = true;
                var leg = Legs[(int)ChosenLeg];
                Vector2 endPoint = leg.GetEndPoint();
                int dir = Math.Sign(endPoint.X - NPC.Center.X);
                Vector2 pos = endPoint + new Vector2(dir * 90, 0);
                pos = FindGround(pos.ToTileCoordinates(), GravityDirection, "D").ToWorldCoordinates();
                pos.Y += 40;
                leg.StartCustomAnimation(NPC, pos, 0.5f / stabTime, animationMode: PerforatorLeg.Accel, stepSound: true);
                leg.SetAnimationEndAction((PerforatorLeg leg, NPC npc) =>
                {
                    if (!npc.HasPlayerTarget)
                        return;

                    int spacing = WorldSavingSystem.MasochistModeReal ? 125 : 150;
                    int random = 5;
                    if (DLCUtils.HostCheck)
                    {
                        for (int i = -3; i < 3; i++)
                        {
                            Vector2 target = Main.player[npc.target].Center;
                            int dir = Math.Sign(target.X - npc.Center.X);

                            Vector2 pos = target + Vector2.UnitX * i * spacing * dir;
                            pos.X += Main.rand.NextFloat(-random, random);
                            pos = FindGround(pos.ToTileCoordinates(), GravityDirection, "E").ToWorldCoordinates();

                            float rot = MathHelper.PiOver2 * (0.17f * i + 0.4f);
                               
                            Vector2 vel = -Vector2.UnitY.RotatedBy(rot).RotatedByRandom(MathHelper.PiOver2 * 0.07f);

                            if (dir < 0)
                                vel.X *= -1;

                            Projectile.NewProjectile(npc.GetSource_FromAI(), pos, vel, ModContent.ProjectileType<PerforatorSpike>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0);
                        }
                    }
                });
            }

            if (Timer == 0)
                TelegraphStab();
            if (Timer == stabTelegraphTime)
                Stab();
            if (++Timer > 120)
                GoToNeutral();
        }
        #endregion
        #region Help Methods
        public void GoToNeutral()
        {
            Reset();
            var attacks = Attacks;
            int memoryMax = PhaseTwo ? 3 : 2;
            if (RecentAttacks.Count < memoryMax)
                RecentAttacks.Add((States)State);
            else
            {
                RecentAttacks[0] = RecentAttacks[1];
                RecentAttacks[1] = (States)State;
            }
            if (!PhaseTwo && NPC.GetLifePercent() < 0.65f) // enter p2
            {
                NextState = (int)States.GroundSpikes;
                Phase = 2;
            }
            else
            {
                attacks = attacks.Except(RecentAttacks).ToList();
                NextState = (int)Main.rand.NextFromCollection(attacks);
            }
            //Main.NewText(Enum.GetName(typeof(States), (States)NextState));
            State = (int)States.MoveToPlayer;

            // debug
            //NextState = (int)States.LegAssault;
        }
        public void Reset()
        {
            Timer = 0;
            AI2 = 0;
            AI3 = 0;
        }
        public static void ClearGore()
        {
            for (int i = 0; i < Main.gore.Length; i++)
            {
                if (Main.gore[i].timeLeft > 5)
                    Main.gore[i].timeLeft = 5;
            }
                
        }
        public int ClosestLegIndex(Vector2 pos, List<int> except = null)
        {
            int min = -1;
            for (int i = 0; i < Legs.Length; i++)
            {
                if (except != null && except.Contains(i))
                    continue;
                if (min < 0 || Legs[i].GetEndPoint().Distance(pos) < Legs[min].GetEndPoint().Distance(pos))
                    min = i;
            }
            return min;
        }
        public static Point FindGround(Point p, Vector2 direction, string num)
        {
            //Main.NewText($"illegal position is {p} and dir is {direction}");
            if (p.X > 0 && p.Y > 0 && WorldGen.InWorld(p.X, p.Y, 2))
            {
                return LumUtils.FindGround(p, direction);
            }
            else
            {
                //Main.NewText("what? how? " + num);
                //Main.NewText($"illegal position is {p} and dir is {direction}");
                return p;
            }
        }
        #endregion
        #region Walking Methods
        public void WalkToPositionAI(Vector2 pos, float speedMod = 1f, int heightAboveGround = HeightAboveGround)
        {
            bool canWalkToPlayer = CheckIfCanWalk(pos, out Point groundAtPlayer);
            groundAtPlayer = FindGround(groundAtPlayer, GravityDirection, "F");

            if (canWalkToPlayer)
            {
                // check if player is reasonably above ground
                Vector2 groundAtPlayerV = groundAtPlayer.ToWorldCoordinates();
                bool validAboveGround = true;
                int playerPointY = pos.ToTileCoordinates().Y;
                int dir = Math.Sign(playerPointY - groundAtPlayer.Y); // should be negative
                if (dir < 0)
                {
                    while (groundAtPlayer.Y != playerPointY)
                    {
                        groundAtPlayer.Y += dir;
                        if (Main.tile[groundAtPlayer.X, groundAtPlayer.Y].IsTileSolid())
                        {
                            validAboveGround = false;
                            break;
                        }
                    }
                }
                else if (dir > 0)
                    validAboveGround = false;

                if (validAboveGround) // position has line of sight to the ground below it
                {
                    if (Math.Abs(groundAtPlayerV.Y - pos.Y) < heightAboveGround * 2) // position isn't too far above ground
                    {
                        // all good! we can walk
                        WalkTowards(pos, speedMod, heightAboveGround);
                        return;
                    }
                }
            }
            FlyTowards(pos, speedMod, heightAboveGround);
        }
        public void WalkTowards(Vector2 pos, float speedMod, int heightAboveGround)
        {
            int dir = Math.Sign(pos.X - NPC.Center.X);
            Vector2 desiredPos = NPC.Center + dir * Vector2.UnitX * 80;
            desiredPos = FindGround(desiredPos.ToTileCoordinates(), GravityDirection, "G").ToWorldCoordinates() - Vector2.UnitY * heightAboveGround;
            Movement(desiredPos, speedMod);
        }
        public void FlyTowards(Vector2 pos, float speedMod, int heightAboveGround)
        {
            Vector2 desiredPos = pos;
            desiredPos = FindGround(desiredPos.ToTileCoordinates(), GravityDirection, "H").ToWorldCoordinates() - Vector2.UnitY * heightAboveGround;
            Movement(desiredPos, speedMod);
        }
        public void Movement(Vector2 desiredPos, float speedMod)
        {
            speedMod *= 1.6f;
            float accel = Acceleration * speedMod;
            float decel = Acceleration * 2 * speedMod;
            float max = MaxMovementSpeed * speedMod;
            if (max > Target.velocity.Length() + MaxMovementSpeed)
                max = Target.velocity.Length() + MaxMovementSpeed;
            float resistance = NPC.velocity.Length() * accel / max;
            NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, desiredPos, NPC.velocity, accel - resistance, decel + resistance);
        }
        // if there's a reasonable ground path to player's X position from the spider
        // does not guarantee player to be at a reasonable spot above that ground position
        public bool CheckIfCanWalk(Vector2 pos, out Point groundAtPlayer)
        {
            if (pos.HasNaNs() || pos.X < 0 || pos.Y < 0)
            {
                groundAtPlayer = new();
                return false;
            }
                
            int maxHeight = HeightAboveGround * 2 / 16; 

            float targetX = pos.X;
            int tiles = (int)((targetX - NPC.Center.X) / 16);
            int dir = Math.Sign(tiles);
            if (dir == 0)
                dir = 1;
            tiles = Math.Abs(tiles);
            Point point = FindGround(NPC.Center.ToTileCoordinates(), GravityDirection, "I") - new Point(0, 1);
            for (int i = 0; i < tiles; i++)
            {
                point.X += dir;
                // make sure we are along ground
                // search for surface tile
                // (searches up if we're at solid tile, down if we're at air)
                Point ground = FindGround(point, GravityDirection, "J");

                // abs is the height difference between this block and previous block
                // if it's too great, we can't simply walk to the player
                //Main.NewText(ground.Y + " " + point.Y);
                if (Math.Abs(ground.Y - point.Y) < maxHeight) // height difference small enough
                {
                    continue;
                }
                else // height difference too big
                {
                    groundAtPlayer = new(); // irrelevant
                    return false;
                }
            }
            groundAtPlayer = point;
            // we got through iteration, each step passed the height check
            return true;
        }
        #endregion
        #endregion
    }
}