using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades
{
    public sealed partial class HadesHeadEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The amount of damage missiles from Hades do.
        /// </summary>
        public static int MissileDamage => Variables.GetAIInt("MissileDamage", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The maximum amount of time Hades can spend redirecting during his Missile Lunges attack.
        /// </summary>
        public static int MissileLunges_RedirectMaxTime => Variables.GetAIInt("MissileLunges_RedirectMaxTime", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The maximum amount of time Hades spends lunging during his Missile Lunges attack.
        /// </summary>
        public static int MissileLunges_LungeDuration => Variables.GetAIInt("MissileLunges_LungeDuration", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How many lunges Hades should perform during his Missile Lunges attack.
        /// </summary>
        public static int MissileLunges_LungeCount => Variables.GetAIInt("MissileLunges_LungeCount", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The horizontal direction in which Hades should lunge during his Missile Lunges attack.
        /// </summary>
        public ref float MissileLunges_LungeDirection => ref NPC.ai[0];

        /// <summary>
        /// How many lunges Hades has performed so far in his Missile Lunges attack.
        /// </summary>
        public ref float MissileLunges_LungeCounter => ref NPC.ai[1];

        /// <summary>
        /// Whether Hades is within ground during his Missile Lunges attack.
        /// </summary>
        public bool MissileLunges_InGround
        {
            get => NPC.ai[2] == 1f;
            set => NPC.ai[2] = value.ToInt();
        }

        /// <summary>
        /// The sound played when Hades charges up for a dash.
        /// </summary>
        public static readonly SoundStyle DashChargeUpSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/DashChargeUp") with { Volume = 1.7f };

        /// <summary>
        /// The sound played when Hades impacts the ground.
        /// </summary>
        public static readonly SoundStyle GroundImpactSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/GroundImpact") with { Volume = 1.4f };

        /// <summary>
        /// AI update loop method for the MissileLunges attack.
        /// </summary>
        public void DoBehavior_MissileLunges()
        {
            BodyBehaviorAction = new(AllSegments(), DoBehavior_MissileLunges_ReleaseMissile);

            SegmentReorientationStrength = 0.4f;

            if (AITimer == 1)
            {
                MissileLunges_LungeDirection = NPC.HorizontalDirectionTo(Target.Center);
                NPC.netUpdate = true;
            }

            if (AITimer < MissileLunges_RedirectMaxTime)
            {
                Vector2 hoverDestination = Target.Center + new Vector2(MissileLunges_LungeDirection * 1700f, 3600f + Target.velocity.Y * 75f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(hoverDestination) * 120f, 0.2f);

                if (NPC.WithinRange(hoverDestination, 240f))
                {
                    AITimer = MissileLunges_RedirectMaxTime;
                    NPC.netUpdate = true;
                }
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }

            else if (AITimer <= MissileLunges_RedirectMaxTime + MissileLunges_LungeDuration)
            {
                if (AITimer == MissileLunges_RedirectMaxTime + 4)
                    SoundEngine.PlaySound(DashChargeUpSound).WithVolumeBoost(2.3f);

                // Open Hades' jaws after he does the dash charge-up roar sound.
                if (AITimer >= MissileLunges_RedirectMaxTime + 13)
                    JawRotation = MathHelper.Lerp(JawRotation, 0.93f, 0.1f);

                float animationCompletion = LumUtils.InverseLerp(0f, MissileLunges_LungeDuration, AITimer - MissileLunges_RedirectMaxTime);

                // Bias the animation completion towards 0.5, effectively making the midpoint (aka the apex of the lunge) more pronounced.
                animationCompletion = MathHelper.Lerp(animationCompletion, 0.5f, 0.25f);

                float height = MathHelper.Lerp(3900f, -600f, MathF.Pow(LumUtils.Convert01To010(animationCompletion) + 0.001f, 0.7f));
                Vector2 idealPosition = Target.Center + new Vector2(MathHelper.Lerp(1700f, -1700f, animationCompletion) * MissileLunges_LungeDirection, height);
                idealPosition.X += MathF.Cos(MathHelper.TwoPi * AITimer / 45f) * LumUtils.InverseLerp(50f, 500f, height) * 350f;

                Vector2 oldPosition = NPC.Center;
                NPC.Center = Vector2.Lerp(NPC.Center, idealPosition, 0.45f);
                NPC.velocity *= 0.5f;
                NPC.rotation = oldPosition.AngleTo(NPC.Center) + MathHelper.PiOver2;

                SegmentOpenInterpolant = LumUtils.Saturate(SegmentOpenInterpolant + 0.1f);
            }
            else
            {
                AITimer = 0;
                MissileLunges_LungeCounter++;
                if (MissileLunges_LungeCounter >= MissileLunges_LungeCount)
                    SelectNewState();

                NPC.netUpdate = true;
            }

            DoBehavior_MissileLunges_HandleGroundCollision();

            NPC.damage = 0;
        }

        /// <summary>
        /// Handles ground collision effects during Hades' Missile Lunges attack.
        /// </summary>
        public void DoBehavior_MissileLunges_HandleGroundCollision()
        {
            bool inGround = Collision.SolidCollision(NPC.TopLeft, NPC.width, NPC.height);
            if (MissileLunges_InGround != inGround)
            {
                MissileLunges_InGround = inGround;
                NPC.netUpdate = true;

                CreateGroundImpactVisuals();
            }
        }

        /// <summary>
        /// Creates sounds and visuals for when Hades interacts with the ground.
        /// </summary>
        public void CreateGroundImpactVisuals()
        {
            if (NPC.soundDelay <= 0)
            {
                SoundEngine.PlaySound(GroundImpactSound);
                SoundEngine.PlaySound(AresTeslaCannon.TeslaOrbShootSound);
                ScreenShakeSystem.StartShake(17.5f, shakeStrengthDissipationIncrement: 0.5f);
                NPC.soundDelay = 45;
            }

            // Create a centralized burst of dust that flies to the sides.
            for (int i = 0; i < 40; i++)
            {
                Color dustColor = Color.Lerp(Color.SaddleBrown, Color.DarkGray, Main.rand.NextFloat(0.5f));
                Vector2 dustSpawnPosition = NPC.Center + Vector2.UnitY * 20f + Main.rand.NextVector2Circular(30f, 4f);
                dustSpawnPosition = LumUtils.FindGround(dustSpawnPosition.ToTileCoordinates(), Vector2.UnitY).ToWorldCoordinates();

                Vector2 dustVelocity = Vector2.UnitX.RotatedByRandom(0.2f) * Main.rand.NextFloat(5f, 67f) * Main.rand.NextFromList(-1f, 1f);
                float dustScale = dustVelocity.Length() / 32f;
                SmallSmokeParticle impactDust = new(dustSpawnPosition, dustVelocity, dustColor, Color.Transparent, dustScale, 200f);
                GeneralParticleHandler.SpawnParticle(impactDust);
            }

            // Create a wide spread of dust that flies upward.
            for (float dx = -400f; dx < 400f; dx += Main.rand.NextFloat(12f, 31f))
            {
                float dustSizeInterpolant = Main.rand.NextFloat();
                float dustScale = MathHelper.Lerp(1f, 2f, dustSizeInterpolant);
                Vector2 dustVelocity = (-Vector2.UnitY * Main.rand.NextFloat(30f, 44f) + Main.rand.NextVector2Circular(10f, 20f)) / dustScale;
                Color dustColor = Color.Lerp(Color.SaddleBrown, Color.DarkGray, Main.rand.NextFloat(0.5f));

                Point groundSearchPoint = (NPC.Center + Vector2.UnitX * dx).ToTileCoordinates();
                Point groundTilePosition = LumUtils.FindGroundVertical(groundSearchPoint);
                Vector2 groundPosition = groundTilePosition.ToWorldCoordinates();
                SmallSmokeParticle dust = new(groundPosition, dustVelocity, dustColor, Color.Transparent, dustScale, 200f);
                GeneralParticleHandler.SpawnParticle(dust);

                for (int j = 0; j < 2; j++)
                {
                    int groundDustIndex = WorldGen.KillTile_MakeTileDust(groundTilePosition.X, groundTilePosition.Y, Main.tile[groundTilePosition]);
                    Dust groundDust = Main.dust[groundDustIndex];
                    groundDust.velocity.Y -= Main.rand.NextFloat(6f, 13f);
                    groundDust.scale *= 1.6f;
                }
            }

            // Create lightning that flies upward.
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 arcSpawnPosition = NPC.Center + Vector2.UnitX * Main.rand.NextFloatDirection() * 400f;
                    arcSpawnPosition = LumUtils.FindGround(arcSpawnPosition.ToTileCoordinates(), Vector2.UnitY).ToWorldCoordinates();

                    Vector2 arcDestination = arcSpawnPosition - Vector2.UnitY.RotatedByRandom(MathHelper.Pi * 0.27f) * Main.rand.NextFloat(150f, 720f);
                    Vector2 arcLength = (arcDestination - arcSpawnPosition).RotatedByRandom(0.12f) * Main.rand.NextFloat(0.9f, 1f);
                    LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), arcSpawnPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(9, 29), 1f);
                }
            }
        }

        public void DoBehavior_MissileLunges_ReleaseMissile(HadesBodyEternity behaviorOverride)
        {
            NPC segment = behaviorOverride.NPC;
            int missileReleaseCycle = 10;
            bool properTimeForMissileRelease = AITimer >= MissileLunges_RedirectMaxTime && AITimer <= MissileLunges_RedirectMaxTime + MissileLunges_LungeDuration;
            bool canReleaseMissiles = properTimeForMissileRelease && segment.Center.Y <= Target.Center.Y + 1100f && behaviorOverride.RelativeIndex % 2 == 0;
            if (canReleaseMissiles && behaviorOverride.GenericCountdown <= 0 && AITimer % missileReleaseCycle == behaviorOverride.RelativeIndex % missileReleaseCycle && behaviorOverride.SegmentOpenInterpolant >= 0.8f)
            {
                SoundEngine.PlaySound(Apollo.MissileLaunchSound with { Volume = 0.4f, MaxInstances = 0 }, segment.Center);
                ScreenShakeSystem.StartShakeAtPoint(behaviorOverride.TurretPosition, 4f);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 missileVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * 90f;
                    LumUtils.NewProjectileBetter(segment.GetSource_FromAI(), behaviorOverride.TurretPosition, missileVelocity, ModContent.ProjectileType<HadesMissile>(), MissileDamage, 0f, -1, 0.013f, 7f);

                    behaviorOverride.GenericCountdown = 120;
                    segment.netUpdate = true;
                }
            }

            if (!canReleaseMissiles)
                CloseSegment().Invoke(behaviorOverride);
            else
                OpenSegment().Invoke(behaviorOverride);
        }
    }
}
