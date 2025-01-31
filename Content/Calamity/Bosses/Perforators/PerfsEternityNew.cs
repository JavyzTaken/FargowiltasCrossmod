using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.TownNPCs;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common.InverseKinematics;
using FargowiltasSouls;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
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
        public const bool Enabled = true;
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
        public static int[] JointParts;

        public const int LegPartLength = 80;
        public const int JointLength = 28;

        public static int HeightAboveGround = 275;
        public static int HeightLeeway = 25;

        /// <summary>
        /// The acceleration of the spider's dash.
        /// </summary>
        public static float DashAcceleration => 0.14f;

        /// <summary>
        /// The maximum speed at which the spider can dash.
        /// </summary>
        public static float MaxDashSpeed => 10f;

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

            // draw legs
            if (Legs is not null)
            {
                if (NPC.IsABestiaryIconDummy)
                {
                    for (int j = 0; j < Legs.Length; j++)
                        Legs[j]?.Update(NPC);
                }

                DrawLegSet(Legs, NPC.GetAlpha(drawColor), screenPos);
            }

            // draw hive
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            float rotation = NPC.rotation / 5;
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = PerforatorHive.GlowTexture.Value;
            Color glowmaskColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);
            glowmaskColor = NPC.GetAlpha(glowmaskColor);

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, glowmaskColor, rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
            return false;
        }
        public static void DrawLeg(SpriteBatch spriteBatch, Texture2D legTexture, Vector2 start, Vector2 end, Color color, float width, SpriteEffects direction)
        {
            // Draw nothing if the start and end are equal, to prevent division by 0 problems.
            if (start == end)
                return;

            float rotation = (end - start).ToRotation();
            Vector2 scale = new(Vector2.Distance(start, end) / legTexture.Width, width);
            start.Y += 2f;

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
                    int jointParts = JointParts[j];
                    int joints = j == 0 ? 2 : 1;
                    float jointDiv = ((float)JointLength / LegPartLength);
                    float partLength = leg[j].Offset.Length() / ((jointParts - joints) + (jointDiv * joints));
                    float jointLength = partLength * jointDiv;
                    float accumulatedLength = 0;
                    for (int k = 0; k < jointParts; k++)
                    {
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
                // Ensure that legs are already grounded when the Perforator has fully spawned in.
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < Legs.Length; j++)
                        Legs[j]?.Update(NPC);
                }

                SpawnProgress += 1f / SpawnTime;
                if (SpawnProgress < 0.8f)
                    NPC.Opacity = 0f;
                else if (SpawnProgress < 1f)
                {
                    NPC.Opacity = (SpawnProgress - 0.8f) / 0.2f;
                    NPC.dontTakeDamage = false;
                }
                else
                {
                    NPC.Opacity = 1f;
                    NPC.dontTakeDamage = false;
                    // do a little "spawn animation" thing
                    NPC.netUpdate = true;
                }
            }



            NewAI();
            return false;
        }
        public void NewAI()
        {
            // Reset the gravity direction to down every frame.
            GravityDirection = Vector2.UnitY;

            WalkToPositionAI(Target.Center);

            // Look forward
            Vector2 forwardDirection = Vector2.UnitX * NPC.SafeDirectionTo(Target.Center).X.NonZeroSign();
            float idealRotation = NPC.velocity.X * 0.05f + NPC.velocity.Y * NPC.spriteDirection * 0.097f + forwardDirection.ToRotation();
            if (NPC.velocity.Length() >= 4f && Math.Sign(NPC.velocity.X) == (int)forwardDirection.X)
                NPC.spriteDirection = (int)forwardDirection.X;
            NPC.rotation = NPC.velocity.X / 20;
            //NPC.rotation = NPC.rotation.AngleTowards(idealRotation, 0.09f).AngleLerp(idealRotation, 0.03f);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < Legs.Length; j++)
                    Legs[j]?.Update(NPC);
            }
        }
        public void WalkToPositionAI(Vector2 pos)
        {
            bool canWalkToPlayer = CheckIfCanWalk(pos, out Point groundAtPlayer);
            groundAtPlayer = LumUtils.FindGround(groundAtPlayer, GravityDirection);

            if (canWalkToPlayer)
            {
                // check if player is reasonably above ground
                Vector2 groundAtPlayerV = groundAtPlayer.ToWorldCoordinates();
                Dust.NewDust(groundAtPlayerV, 1, 1, DustID.Torch);
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
                    if (Math.Abs(groundAtPlayerV.Y - pos.Y) < HeightAboveGround * 2) // position isn't too far above ground
                    {
                        // all good! we can walk
                        WalkTowards(pos);
                        return;
                    }
                }
            }
            FlyTowards(pos);
        }
        public void WalkTowards(Vector2 pos)
        {
            int dir = Math.Sign(pos.X - NPC.Center.X);
            Vector2 desiredPos = NPC.Center + dir * Vector2.UnitX * 80;
            desiredPos = LumUtils.FindGround(desiredPos.ToTileCoordinates(), GravityDirection).ToWorldCoordinates() - Vector2.UnitY * HeightAboveGround;
            Movement(desiredPos);
        }
        public void FlyTowards(Vector2 pos)
        {
            Vector2 desiredPos = pos;
            desiredPos = LumUtils.FindGround(desiredPos.ToTileCoordinates(), GravityDirection).ToWorldCoordinates() - Vector2.UnitY * HeightAboveGround;
            Movement(desiredPos);
        }
        public void Movement(Vector2 desiredPos)
        {
            float speedMultiplier = 1.6f;
            float accel = DashAcceleration * speedMultiplier;
            float decel = DashAcceleration * 2 * speedMultiplier;
            float resistance = NPC.velocity.Length() * accel / (MaxDashSpeed * speedMultiplier);
            NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, desiredPos, NPC.velocity, accel - resistance, decel + resistance);
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
        // if there's a reasonable ground path to player's X position from the spider
        // does not guarantee player to be at a reasonable spot above that ground position
        public bool CheckIfCanWalk(Vector2 pos, out Point groundAtPlayer)
        {
            int maxHeight = HeightAboveGround * 2 / 16; 

            float targetX = pos.X;
            int tiles = (int)((targetX - NPC.Center.X) / 16);
            int dir = Math.Sign(tiles);
            tiles = Math.Abs(tiles);
            Point point = LumUtils.FindGround(NPC.Center.ToTileCoordinates(), GravityDirection) - new Point(0, 1);
            for (int i = 0; i < tiles; i++)
            {
                point.X += dir;
                // make sure we are along ground
                // search for surface tile
                // (searches up if we're at solid tile, down if we're at air)
                Point ground = LumUtils.FindGround(point, GravityDirection);

                // abs is the height difference between this block and previous block
                // if it's too great, we can't simply walk to the player
                if (Math.Abs(ground.X - point.X) < maxHeight) // height difference small enough
                    continue;
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
    }
}