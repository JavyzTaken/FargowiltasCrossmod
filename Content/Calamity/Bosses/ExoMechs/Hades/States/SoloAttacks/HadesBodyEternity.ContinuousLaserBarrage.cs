using CalamityMod.Sounds;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
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
        /// How far along Hades is with his firing animation during his ContinuousLaserBarrage attack.
        /// </summary>
        public float ContinuousLaserBarrage_FireCompletion => Utilities.InverseLerp(0f, ContinuousLaserBarrage_ShootTime, AITimer - ContinuousLaserBarrage_TelegraphTime);

        /// <summary>
        /// How many barrages Hades has done so far for his ContinuousLaserBarrage attack.
        /// </summary>
        public ref float ContinuousLaserBarrage_BarrageCounter => ref NPC.ai[0];

        /// <summary>
        /// How many barrages Hades should perform during his ContinuousLaserBarrage attack.
        /// </summary>
        public static int ContinuousLaserBarrage_BarrageCount => Variables.GetAIInt("ContinuousLaserBarrage_BarrageCount", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How long Hades spends telegraphing and moving around during his ContinuousLaserBarrage attack.
        /// </summary>
        public static int ContinuousLaserBarrage_TelegraphTime => Variables.GetAIInt("ContinuousLaserBarrage_TelegraphTime", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How long Hades spends shooting lasers during his ContinuousLaserBarrage attack.
        /// </summary>
        public static int ContinuousLaserBarrage_ShootTime => Variables.GetAIInt("ContinuousLaserBarrage_ShootTime", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The standard fly speed at which Hades moves during his ContinuousLaserBarrage attack.
        /// </summary>
        public static float ContinuousLaserBarrage_StandardFlySpeed => Variables.GetAIFloat("ContinuousLaserBarrage_StandardFlySpeed", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How fast lasers shot by Hades should be during his ContinuousLaserBarrage attack.
        /// </summary>
        public static float ContinuousLaserBarrage_LaserShootSpeed => Variables.GetAIFloat("ContinuousLaserBarrage_LaserShootSpeed", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How close one of Hades' segments has to be to a target in order to fire.
        /// </summary>
        public static float ContinuousLaserBarrage_ShootProximityRequirement => Variables.GetAIFloat("ContinuousLaserBarrage_ShootProximityRequirement", ExoMechAIVariableType.Hades);

        /// <summary>
        /// AI update loop method for the ContinuousLaserBarrage attack.
        /// </summary>
        public void DoBehavior_ContinuousLaserBarrage()
        {
            if (AITimer == 1)
                SoundEngine.PlaySound(LaserChargeUpSound);

            SegmentReorientationStrength = 0.125f;

            DoBehavior_ContinuousLaserBarrage_PerformTelegraphing();
            DoBehavior_ContinuousLaserBarrage_FlyAroundTarget();
            DoBehavior_ContinuousLaserBarrage_FireProjectiles();

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public void DoBehavior_ContinuousLaserBarrage_PerformTelegraphing()
        {
            float telegraphCompletion = Utilities.InverseLerp(0f, ContinuousLaserBarrage_TelegraphTime, AITimer);
            BodyRenderAction = new(AllSegments(), new(behaviorOverride =>
            {
                float fireCompletion = ContinuousLaserBarrage_FireCompletion;
                float indexRatioAlongHades = behaviorOverride.RelativeIndex / (float)BodySegmentCount;
                float closenessToFiring = Utilities.InverseLerp(-0.1f, 0.01f, fireCompletion - indexRatioAlongHades);
                float fadeOutDueToOncomingFiring = (1f - closenessToFiring).Squared();
                PrimitivePixelationSystem.RenderToPrimsNextFrame(() =>
                {
                    ContinuousLaserBarrage_CreateTelegraphsOnSegments(behaviorOverride, telegraphCompletion, MathF.Sqrt(telegraphCompletion) * fadeOutDueToOncomingFiring * 2485f);
                }, PixelationPrimitiveLayer.AfterNPCs);
            }));
        }

        public void DoBehavior_ContinuousLaserBarrage_FlyAroundTarget()
        {
            float slowdownFactor = Utils.Remap(AITimer - ContinuousLaserBarrage_TelegraphTime, 0f, 60f, 1f, 0.25f);
            float idealFlySpeed = slowdownFactor * ContinuousLaserBarrage_StandardFlySpeed;
            float newSpeed = MathHelper.Lerp(NPC.velocity.Length(), idealFlySpeed, 0.005f);

            Vector2 flyDestination = Target.Center + Vector2.UnitY * 450f;
            float idealDirection = NPC.AngleTo(flyDestination);
            float currentDirection = NPC.velocity.ToRotation();
            if (NPC.WithinRange(flyDestination, 800f))
                idealDirection = currentDirection;

            NPC.velocity = currentDirection.AngleTowards(idealDirection, 0.1f).ToRotationVector2() * newSpeed;
        }

        public void DoBehavior_ContinuousLaserBarrage_FireProjectiles()
        {
            if (ContinuousLaserBarrage_FireCompletion >= 1f)
            {
                AITimer = 0;
                ContinuousLaserBarrage_BarrageCounter++;
                if (ContinuousLaserBarrage_BarrageCounter >= ContinuousLaserBarrage_BarrageCount)
                    SelectNewState();
                NPC.netUpdate = true;
            }

            BodyBehaviorAction = new(AllSegments(), new(behaviorOverride =>
            {
                NPC segment = behaviorOverride.NPC;
                float fireCompletion = ContinuousLaserBarrage_FireCompletion;
                float indexRatioAlongHades = behaviorOverride.RelativeIndex / (float)BodySegmentCount;

                bool readyToFire = fireCompletion > 0f && MathHelper.Distance(indexRatioAlongHades, fireCompletion) <= 0.01f && behaviorOverride.GenericCountdown <= 0f;
                if (readyToFire && ContinuousLaserBarrage_SegmentCanFire(segment, NPC))
                {
                    Vector2 laserSpawnPosition = behaviorOverride.TurretPosition;
                    SoundEngine.PlaySound(CommonCalamitySounds.ExoLaserShootSound with { MaxInstances = 0 }, laserSpawnPosition);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float slowdownFactor = Utils.Remap(Target.Distance(laserSpawnPosition), 775f, 1300f, 0.5f, 1f);
                        Vector2 laserVelocity = (Target.Center - laserSpawnPosition).SafeNormalize(Vector2.UnitY) * slowdownFactor * ContinuousLaserBarrage_LaserShootSpeed;
                        Utilities.NewProjectileBetter(segment.GetSource_FromAI(), laserSpawnPosition, laserVelocity, ModContent.ProjectileType<HadesLaserBurst>(), BasicLaserDamage, 0f, -1, 60f, -1f);

                        behaviorOverride.GenericCountdown = 20f;
                        segment.netUpdate = true;
                    }
                }

                bool hasFired = fireCompletion - indexRatioAlongHades >= 0f;
                if (hasFired)
                    CloseSegment().Invoke(behaviorOverride);
                else
                    OpenSegment(smokeQuantityInterpolant: 0.45f).Invoke(behaviorOverride);
            }));
        }

        /// <summary>
        /// Renders a laser telegraph for a given <see cref="HadesBodyEternity"/> in a given direction.
        /// </summary>
        /// <param name="behaviorOverride">The behavior override responsible for the segment.</param>
        public void ContinuousLaserBarrage_CreateTelegraphsOnSegments(HadesBodyEternity behaviorOverride, float telegraphCompletion, float telegraphSize)
        {
            if (!ContinuousLaserBarrage_SegmentCanFire(behaviorOverride.NPC, NPC))
                return;

            Vector2 telegraphDirection = behaviorOverride.NPC.SafeDirectionTo(Target.Center);
            RenderLaserTelegraph(behaviorOverride, telegraphCompletion, telegraphSize, telegraphDirection);
        }

        /// <summary>
        /// Determines whether a body segment on Hades can fire during the ContinuousLaserBarrage attack.
        /// </summary>
        /// <param name="segment">The segment NPC instance.</param>
        /// <param name="head">The head NPC instance.</param>
        public static bool ContinuousLaserBarrage_SegmentCanFire(NPC segment, NPC head) =>
            segment.WithinRange(Target.Center, ContinuousLaserBarrage_ShootProximityRequirement) && !segment.WithinRange(Target.Center, 300f);
    }
}
