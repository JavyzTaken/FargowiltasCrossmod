using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using FargowiltasCrossmod.Core.Common.InverseKinematics;

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
        #region Fight Related
        public float SpawnProgress;
        public static int SpawnTime = 60 * 3;
        #endregion

        #region Legs Related

        public Vector2 GravityDirection = Vector2.UnitY;

        public PerforatorLeg[] Legs;
        public int[][][] LegSprites; // don't ask
        public Vector2[] LegBraces;

        public Player Target => Main.player[NPC.target];


        public bool WasWalkingUpward
        {
            get => NPC.ai[2] == 1f;
            set => NPC.ai[2] = value.ToInt();
        }

        public static LazyAsset<Texture2D>[] LegTextures = new LazyAsset<Texture2D>[4];
        public static LazyAsset<Texture2D>[] LegEndTextures = new LazyAsset<Texture2D>[4];
        public static LazyAsset<Texture2D>[] LegJointTextures = new LazyAsset<Texture2D>[4];

        public static float LegSizeFactor => 3.5f;
        public static int JointParts => 5;

        /// <summary>
        /// The acceleration of the spider's dash.
        /// </summary>
        public const float DashAcceleration = 0.31f;

        /// <summary>
        /// The maximum speed at which the spider can dash.
        /// </summary>
        public const float MaxDashSpeed = 7.2f;

        /// <summary>
        /// The default quantity of gravity imposed upon the spider.
        /// </summary>
        public const float DefaultGravity = 0.2f;

        /// <summary>
        /// The amount of deceleration imposed upon forward motion when the spider is undergoing spring motion due to being too far from/near to the ground.
        /// </summary>
        public const float ForwardDecelerationDuringSpringMotion = 0.04f;

        /// <summary>
        /// The amount of acceleration used when the spider begins walking up walls.
        /// </summary>
        public const float WallClimbAcceleration = 0.1f;

        /// <summary>
        /// The maximum speed that the spider can travel at when climbing up walls.
        /// </summary>
        public const float MaxWallClimbSpeed = 4.5f;

        /// <summary>
        /// How long, in frames, dashes last.
        /// </summary>
        public static readonly int DashDuration = LumUtils.SecondsToFrames(1.5f);

        /// <summary>
        /// The minimum amount of time a dash delay can last.
        /// </summary>
        public static readonly int MinDashDelayDuration = LumUtils.SecondsToFrames(1.5f);

        /// <summary>
        /// The maximum amount of time a dash delay can last.
        /// </summary>
        public static readonly int MaxDashDelayDuration = LumUtils.SecondsToFrames(3.5f);
        #endregion 

        #endregion Fields and Properties
        public override void SetStaticDefaults()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                var path = "FargowiltasCrossmod/Assets/ExtraTextures/PerfLegs/";
                for (int i = 0; i < 4; i++)
                {
                    int alt = i + 1;
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

            // cursed 3d array ahead
            Legs = new PerforatorLeg[4];
            LegSprites = new int[Legs.Length][][];
            LegBraces = new Vector2[Legs.Length];

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
                Legs[i] = new(LegSizeFactor * legOffset, LegSizeFactor, legOffset.Length() * 0.685f, i);
                Legs[i].Leg[0].Rotation = legOffset.ToRotation();
                Legs[i].Leg[1].Rotation = Vector2.UnitY.ToRotation();

                LegSprites[i] = new int[2][];
                for (int j = 0; j < 2; j++)
                {
                    LegSprites[i][j] = new int[JointParts];
                    for (int k = 0; k < JointParts; k++)
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
            NPC.Center -= Vector2.UnitY * 1000;
        }

        #region Draw
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode)
                return true;

            if (Legs is not null)
            {
                if (NPC.IsABestiaryIconDummy)
                {
                    for (int j = 0; j < Legs.Length; j++)
                        Legs[j]?.Update(NPC);
                }

                DrawLegSet(Legs, NPC.GetAlpha(drawColor), screenPos);
            }

            return true;
        }
        public static void DrawLeg(SpriteBatch spriteBatch, Texture2D legTexture, Vector2 start, Vector2 end, Color color, float width, SpriteEffects direction)
        {
            // Draw nothing if the start and end are equal, to prevent division by 0 problems.
            if (start == end)
                return;

            float rotation = (end - start).ToRotation();
            Vector2 scale = new(Vector2.Distance(start, end) / legTexture.Width, width);
            start.Y += 2f;
            //Main.NewText(scale.X);

            spriteBatch.Draw(legTexture, start, null, color, rotation, legTexture.Size() * Vector2.UnitY * 0.5f, scale, direction, 0f);
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

                // draw leg brace
                Vector2 dir = LegBraces[i].SafeNormalize(Vector2.Zero);
                Vector2 start = NPC.Center - screenPos;
                Vector2 end = start + dir * 80;
                SpriteEffects direction = (LegBraces[i].X).NonZeroSign() == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
                DrawLeg(Main.spriteBatch, LegTextures[0].Value, start, end, lightColor, 1f, direction);
                // leg end
                start = end + dir * 22;
                DrawLeg(Main.spriteBatch, LegJointTextures[0].Value, start, end, lightColor, 1f, direction);

                // draw leg
                Vector2 previousPosition = leg.StartingPoint;
                for (int j = 0; j < leg.JointCount; j++)
                {
                    int partLength = (int)(leg[j].Offset.Length() / (JointParts - 0.7f));
                    int jointLength = (int)(partLength * 0.3f);
                    int jointParts = JointParts;
                    if (j == 0)
                        jointParts += 1;
                    for (int k = 0; k < JointParts; k++)
                    {
                        bool joint = false;
                        int spriteIndex = LegSprites[i][j][k];
                        Texture2D legTexture;
                        bool flip = false;
                        if (j == 0)
                            flip = true;
                        if (k == JointParts - 1)
                        {
                            if (j == 0)
                            {
                                joint = true;
                                legTexture = LegJointTextures[spriteIndex].Value;
                            }
                            else
                                legTexture = LegEndTextures[spriteIndex].Value;
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

                        direction = (leg.EndEffectorPosition.X - LegBraces[i].X).NonZeroSign() == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
                        start = previousPosition - screenPos;
                        end = previousPosition + partOffset - screenPos;
                        if (flip)
                            (start, end) = (end, start);
                        DrawLeg(Main.spriteBatch, legTexture, start, end, lightColor, 1f, direction);
                        previousPosition += partOffset;
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
            
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            
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
                return false;
            }
            Player target = Main.player[NPC.target];
            Vector2 toTarget = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);

            //low ground
            if (Main.LocalPlayer.active && !Main.LocalPlayer.ghost && !Main.LocalPlayer.dead && NPC.Distance(Main.LocalPlayer.Center) < 2000)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<LowGroundBuff>(), 2);

            if (SpawnProgress < 1)
            {
                SpawnProgress += 1f / SpawnTime;
                if (SpawnProgress < 0.8f)
                    NPC.Opacity = 0f;
                else if (SpawnProgress < 1f)
                    NPC.Opacity = (SpawnProgress - 0.8f) / 0.2f;
                else
                {
                    NPC.Opacity = 1f;
                    // do a little "spawn animation" thing
                    NPC.netUpdate = true;
                }
                //NPC.Opacity = 1f;
            }
                


            NewAI();
            return false;
        }
        public void NewAI()
        {
            // Reset the gravity direction to down every frame.
            GravityDirection = Vector2.UnitY;

            bool walkTowardsPlayer = true;
            Vector2 forwardDirectionToPlayer = Vector2.UnitX * NPC.SafeDirectionTo(Target.Center).X.NonZeroSign();

            // Check if the spider should walk up walls or not and act accordingly.
            bool walkingUpward = CheckIfShouldWalkUpWalls(forwardDirectionToPlayer);
            if (walkingUpward)
            {
                GravityDirection = forwardDirectionToPlayer;
                walkTowardsPlayer = false;

                if (NPC.velocity.Y >= -MaxWallClimbSpeed)
                    NPC.velocity.Y -= WallClimbAcceleration;
            }

            if (walkingUpward != WasWalkingUpward)
            {
                WasWalkingUpward = walkingUpward;
                if (walkingUpward)
                {
                    NPC.velocity.Y -= 7f;
                    NPC.position.X += NPC.SafeDirectionTo(Target.Center).X * 16f;
                }

                NPC.netUpdate = true;
            }

            Vector2 forwardDirection = new(GravityDirection.Y, GravityDirection.X);
            Vector2 absoluteForwardDirection = new(Math.Abs(GravityDirection.Y), Math.Abs(GravityDirection.X));
            Vector2 absoluteGravityDirection = new(Math.Abs(GravityDirection.X), Math.Abs(GravityDirection.Y));
            Vector2 groundPosition = LumUtils.FindGround(NPC.Center.ToTileCoordinates(), GravityDirection).ToWorldCoordinates();
            float distanceFromGround = Vector2.Distance(NPC.Center, groundPosition);

            if (distanceFromGround >= 300f)
            {
                NPC.velocity += GravityDirection * DefaultGravity;
                NPC.velocity -= NPC.velocity * absoluteForwardDirection * ForwardDecelerationDuringSpringMotion;
            }
            else if (distanceFromGround <= 250f)
            {
                NPC.velocity -= GravityDirection * DefaultGravity;
                NPC.velocity -= NPC.velocity * absoluteForwardDirection * ForwardDecelerationDuringSpringMotion;
            }
            else
            {
                NPC.velocity -= NPC.velocity * absoluteGravityDirection * 0.16f;

                if (walkTowardsPlayer)
                {
                    float perpendicularDistanceFromPlayer = Math.Abs(LumUtils.SignedDistanceToLine(NPC.Center, Target.Center, forwardDirection));

                    // Slow down near the target.
                    if (perpendicularDistanceFromPlayer <= 120f)
                        NPC.velocity -= NPC.velocity * forwardDirection * 0.06f;

                    // Move forward.
                    else if (Math.Abs(Vector2.Dot(NPC.velocity, forwardDirection)) < MaxDashSpeed)
                        NPC.velocity += NPC.SafeDirectionTo(Target.Center) * forwardDirection * DashAcceleration;

                    // Slow down if the speed limit has been exceeded.
                    else
                        NPC.velocity -= NPC.velocity * forwardDirection * 0.04f;
                }
                else if (!walkingUpward)
                    NPC.velocity -= NPC.velocity * forwardDirection * 0.05f;
            }

            // Disable gravity if minimal movement is happening.
            NPC.noTileCollide = NPC.velocity.Length() <= 1.9f && !walkingUpward;

            // Look forward.
            float idealRotation = NPC.velocity.X * 0.05f + NPC.velocity.Y * NPC.spriteDirection * 0.097f + forwardDirection.ToRotation();
            if (NPC.velocity.Length() >= 0.5f)
                NPC.spriteDirection = Vector2.Dot(NPC.velocity, forwardDirection).NonZeroSign();
            if (Math.Abs(forwardDirection.Y) >= 0.9f)
            {
                idealRotation += MathF.PI;
                NPC.spriteDirection *= -1;
            }
            NPC.rotation = NPC.rotation.AngleTowards(idealRotation, 0.09f).AngleLerp(idealRotation, 0.03f);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < Legs.Length; j++)
                    Legs[j]?.Update(NPC);
            }
        }
        public bool CheckIfShouldWalkUpWalls(Vector2 forwardDirectionToPlayer)
        {
            // Check if the target can be detected. If they can, there's no reason to walk on walls, as adhering to natural gravity is sufficient to reach them.
            if (Collision.CanHitLine(NPC.Center, 1, 1, Target.Center, 1, 1))
                return false;

            // Check up to 90 pixels forward and determine if there's an obstacle within that distance.
            bool obstacleAhead = false;
            float forwardCheckDistance = 90f;
            Vector2 obstaclePosition = NPC.Center + forwardDirectionToPlayer * forwardCheckDistance;
            while (!Collision.CanHit(NPC.Center, 1, 1, obstaclePosition, 1, 1))
            {
                // If there was an obstacle, make note of that and step back up in front of the obstacle.
                obstaclePosition -= forwardDirectionToPlayer * 16f;
                obstacleAhead = true;
            }
            obstaclePosition += forwardDirectionToPlayer * 16f;
            // If there is not obstacle, terminate this method immediately- There would be no wall to walk on in the first place.
            if (!obstacleAhead)
                return false;
            
            // Lastly, check how far up the height of the found obstacle is.
            // If it's too short, ignore it.
            float minObstacleHeight = 200f;
            float obstacleHeight = LumUtils.FindGroundVertical(obstaclePosition.ToTileCoordinates()).ToWorldCoordinates().Distance(obstaclePosition);
            if (obstacleHeight < minObstacleHeight)
                return !Collision.SolidCollision(NPC.Center, 1, 64);

            return true;
        }
        #endregion
    }
}