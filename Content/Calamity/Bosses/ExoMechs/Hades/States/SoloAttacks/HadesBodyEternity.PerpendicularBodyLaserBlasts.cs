using CalamityMod;
using CalamityMod.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades
{
    public sealed partial class HadesHeadEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// Whether Hades has successfully reached his destination or not during his PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public bool PerpendicularBodyLaserBlasts_HasReachedDestination
        {
            get => NPC.ai[0] == 1f;
            set => NPC.ai[0] = value.ToInt();
        }

        /// <summary>
        /// The fly direction Hades attempts to move in during his PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public Vector2 PerpendicularBodyLaserBlasts_StartingDirection
        {
            get => NPC.ai[1].ToRotationVector2();
            set => NPC.ai[1] = value.ToRotation();
        }

        /// <summary>
        /// How many blasts Hades has performed so far during his PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public ref float PerpendicularBodyLaserBlasts_BlastCounter => ref NPC.ai[2];

        /// <summary>
        /// How long Hades spends snaking around into position in anticipation of attacking during his PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public static int PerpendicularBodyLaserBlasts_RedirectTime => Variables.GetAIInt("PerpendicularBodyLaserBlasts_RedirectTime", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How long Hades spends telegraphing prior to firing lasers during his PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public static int PerpendicularBodyLaserBlasts_BlastTelegraphTime => Variables.GetAIInt("PerpendicularBodyLaserBlasts_BlastTelegraphTime", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The 'n' in 'every Nth segment should fire' for Hades' PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public static int PerpendicularBodyLaserBlasts_SegmentUsageCycle => Variables.GetAIInt("PerpendicularBodyLaserBlasts_SegmentUsageCycle", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How many blasts Hades should perform before transitioning to the next attack during the PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public static int PerpendicularBodyLaserBlasts_BlastCount => Variables.GetAIInt("PerpendicularBodyLaserBlasts_BlastCount", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How far Hades tries to go ahead of the player before firing during his PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public static float PerpendicularBodyLaserBlasts_ForwardDestinationOffset => Variables.GetAIFloat("PerpendicularBodyLaserBlasts_ForwardDestinationOffset", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How far Hades tries to go to the side of the player before firing during his PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public static float PerpendicularBodyLaserBlasts_SideDestinationOffset => Variables.GetAIFloat("PerpendicularBodyLaserBlasts_SideDestinationOffset", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How far along, as a 0-1 completion ratio, Hades needs to be during his PerpendicularBodyLaserBlasts slow down animation before lasers are fired all at once.
        /// </summary>
        public static float PerpendicularBodyLaserBlasts_BurstShootCompletionRatio => Variables.GetAIFloat("PerpendicularBodyLaserBlasts_BurstShootCompletionRatio", ExoMechAIVariableType.Hades);

        /// <summary>
        /// How fast lasers shot by Hades should be during his PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public static float PerpendicularBodyLaserBlasts_LaserShootSpeed => Variables.GetAIFloat("PerpendicularBodyLaserBlasts_LaserShootSpeed", ExoMechAIVariableType.Hades);

        /// <summary>
        /// The amount of damage basic lasers from Hades do.
        /// </summary>
        public static int BasicLaserDamage => Variables.GetAIInt("BasicLaserDamage", ExoMechAIVariableType.Hades);

        public static readonly SoundStyle LaserChargeUpSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/LaserChargeUp");

        /// <summary>
        /// The sound Hades plays when firing his perpendicular laserbeams.
        /// </summary>
        public static readonly SoundStyle SideLaserBurstSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/SideLaserBurst");

        /// <summary>
        /// AI update loop method for the PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public void DoBehavior_PerpendicularBodyLaserBlasts()
        {
            BodyBehaviorAction = new(AllSegments(), CloseSegment());

            if (AITimer < PerpendicularBodyLaserBlasts_RedirectTime)
                DoBehavior_PerpendicularBodyLaserBlasts_MoveNearPlayer();
            else if (AITimer < PerpendicularBodyLaserBlasts_RedirectTime + PerpendicularBodyLaserBlasts_BlastTelegraphTime)
            {
                DoBehavior_PerpendicularBodyLaserBlasts_CreateBlastTelegraphs();
                DoBehavior_PerpendicularBodyLaserBlasts_UpdateSegments();

                if (AITimer == PerpendicularBodyLaserBlasts_RedirectTime + 1)
                    SoundEngine.PlaySound(LaserChargeUpSound);

                // Slow down.
                NPC.velocity *= 0.97f;
            }
            else
            {
                PerpendicularBodyLaserBlasts_StartingDirection = Vector2.Zero;
                PerpendicularBodyLaserBlasts_HasReachedDestination = false;
                PerpendicularBodyLaserBlasts_BlastCounter++;
                AITimer = 0;

                if (PerpendicularBodyLaserBlasts_BlastCounter >= PerpendicularBodyLaserBlasts_BlastCount)
                    SelectNewState();
            }

            SegmentReorientationStrength = LumUtils.InverseLerp(3f, 9.5f, NPC.velocity.Length()) * 0.1f;

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }

        /// <summary>
        /// Makes Hades move around during his PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public void DoBehavior_PerpendicularBodyLaserBlasts_MoveNearPlayer()
        {
            if (AITimer <= 10)
            {
                PerpendicularBodyLaserBlasts_StartingDirection = NPC.SafeDirectionTo(Target.Center);
                if (AITimer == 10)
                    NPC.netUpdate = true;
            }

            float forwardOffset = PerpendicularBodyLaserBlasts_ForwardDestinationOffset;
            float sideOffset = PerpendicularBodyLaserBlasts_SideDestinationOffset;
            Vector2 idealOffsetDirection = PerpendicularBodyLaserBlasts_StartingDirection;
            Vector2 flyDestination = Target.Center + idealOffsetDirection * forwardOffset + idealOffsetDirection.RotatedBy(MathHelper.PiOver2) * sideOffset;

            float moveCompletion = AITimer / (float)PerpendicularBodyLaserBlasts_RedirectTime;
            float flySpeedInterpolant = Utilities.InverseLerp(0f, 0.35f, moveCompletion);
            float idealFlySpeed = MathHelper.SmoothStep(4f, 42f, flySpeedInterpolant);
            if (PerpendicularBodyLaserBlasts_HasReachedDestination)
                idealFlySpeed *= 0.1f;

            Vector2 idealFlyDirection = PerpendicularBodyLaserBlasts_HasReachedDestination ? NPC.velocity.SafeNormalize(Vector2.UnitY) : NPC.SafeDirectionTo(flyDestination);
            Vector2 idealVelocity = idealFlyDirection * idealFlySpeed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, idealVelocity, flySpeedInterpolant * 0.195f);

            if (!PerpendicularBodyLaserBlasts_HasReachedDestination && NPC.WithinRange(flyDestination, 250f))
            {
                PerpendicularBodyLaserBlasts_HasReachedDestination = true;
                if (AITimer < PerpendicularBodyLaserBlasts_RedirectTime - 10)
                    AITimer = PerpendicularBodyLaserBlasts_RedirectTime - 10;

                NPC.netUpdate = true;
            }
        }

        /// <summary>
        /// Makes Hades slow down and cast telegraphs during his PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public void DoBehavior_PerpendicularBodyLaserBlasts_CreateBlastTelegraphs()
        {
            int localAITimer = AITimer - PerpendicularBodyLaserBlasts_RedirectTime;
            float telegraphCompletion = localAITimer / (float)PerpendicularBodyLaserBlasts_BlastTelegraphTime;
            BodyRenderAction = new(EveryNthSegment(PerpendicularBodyLaserBlasts_SegmentUsageCycle), new(behaviorOverride =>
            {
                if (!PerpendicularBodyLaserBlasts_SegmentCanFire(behaviorOverride.NPC, NPC))
                    return;

                // Render laser telegraphs.
                PrimitivePixelationSystem.RenderToPrimsNextFrame(() =>
                {
                    RenderLaserTelegraph(behaviorOverride, telegraphCompletion, 1000f, -behaviorOverride.NPC.rotation.ToRotationVector2());
                    RenderLaserTelegraph(behaviorOverride, telegraphCompletion, 1000f, behaviorOverride.NPC.rotation.ToRotationVector2());
                }, PixelationPrimitiveLayer.AfterNPCs);
            }));

            float shootCompletionRatio = PerpendicularBodyLaserBlasts_BurstShootCompletionRatio;
            float rumblePower = Utilities.InverseLerpBump(0f, shootCompletionRatio, shootCompletionRatio, shootCompletionRatio + 0.04f, telegraphCompletion);
            ScreenShakeSystem.SetUniversalRumble(rumblePower * 1.3f, MathHelper.TwoPi, null, 0.2f);
        }

        /// <summary>
        /// Makes Hades update all of his segments in preparation of his PerpendicularBodyLaserBlasts attack.
        /// </summary>
        public void DoBehavior_PerpendicularBodyLaserBlasts_UpdateSegments()
        {
            int localAITimer = AITimer - PerpendicularBodyLaserBlasts_RedirectTime;
            bool timeToFire = localAITimer == (int)(PerpendicularBodyLaserBlasts_BlastTelegraphTime * PerpendicularBodyLaserBlasts_BurstShootCompletionRatio);

            if (timeToFire)
                SoundEngine.PlaySound(SideLaserBurstSound).WithVolumeBoost(2f);

            BodyBehaviorAction = new(EveryNthSegment(PerpendicularBodyLaserBlasts_SegmentUsageCycle), new(behaviorOverride =>
            {
                if (timeToFire && PerpendicularBodyLaserBlasts_SegmentCanFire(behaviorOverride.NPC, NPC))
                    PerpendicularBodyLaserBlasts_FireLaser(behaviorOverride);

                // Disable natural reorientation of segments, to ensure that they stay in the same place before they fire.
                behaviorOverride.ShouldReorientDirection = false;

                OpenSegment(enableContactDamage: true).Invoke(behaviorOverride);
            }));
        }

        /// <summary>
        /// Creates a burst of particles at a given point in a given direction. Meant to be used in conjunction with laser bursts spawns on one of Hades' segments.
        /// </summary>
        /// <param name="laserSpawnPosition">The spawn position of the laser particles.</param>
        /// <param name="laserShootDirection">The shoot direction of the laser particles.</param>
        public static void PerpendicularBodyLaserBlasts_CreateLaserBurstParticles(Vector2 laserSpawnPosition, Vector2 laserShootDirection)
        {
            for (int i = 0; i < 10; i++)
            {
                int laserLineLifetime = Main.rand.Next(10, 30);
                float laserLineSpeed = Main.rand.NextFloat(9f, 20f);
                LineParticle line = new(laserSpawnPosition, laserShootDirection.RotatedByRandom(0.5f) * laserLineSpeed, false, laserLineLifetime, 1f, Color.Red);
                GeneralParticleHandler.SpawnParticle(line);
            }
        }

        /// <summary>
        /// Fires two lasers perpendicular to a given segment of Hades'.
        /// </summary>
        /// <param name="bodyAI">The behavior override of the body segment that should fire.</param>
        public static void PerpendicularBodyLaserBlasts_FireLaser(HadesBodyEternity bodyAI)
        {
            NPC segment = bodyAI.NPC;
            Vector2 laserSpawnPosition = bodyAI.TurretPosition;
            Vector2 perpendicularDirection = segment.rotation.ToRotationVector2();
            PerpendicularBodyLaserBlasts_CreateLaserBurstParticles(laserSpawnPosition, -perpendicularDirection);
            PerpendicularBodyLaserBlasts_CreateLaserBurstParticles(laserSpawnPosition, perpendicularDirection);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Utilities.NewProjectileBetter(segment.GetSource_FromAI(), laserSpawnPosition, perpendicularDirection * PerpendicularBodyLaserBlasts_LaserShootSpeed, ModContent.ProjectileType<HadesLaserBurst>(), BasicLaserDamage, 0f, -1);
                Utilities.NewProjectileBetter(segment.GetSource_FromAI(), laserSpawnPosition, perpendicularDirection * -PerpendicularBodyLaserBlasts_LaserShootSpeed, ModContent.ProjectileType<HadesLaserBurst>(), BasicLaserDamage, 0f, -1);
            }

            ScreenShakeSystem.StartShake(3.2f);
        }

        /// <summary>
        /// Determines whether a body segment on Hades can fire during the PerpendicularBodyLaserBlasts attack.
        /// </summary>
        /// <param name="segment">The segment NPC instance.</param>
        /// <param name="head">The head NPC instance.</param>
        public static bool PerpendicularBodyLaserBlasts_SegmentCanFire(NPC segment, NPC head)
        {
            bool roughlySameDirectionAsHead = Vector2.Dot(segment.rotation.ToRotationVector2(), head.rotation.ToRotationVector2()) >= 0.2f;
            bool closeEnoughToTarget = segment.WithinRange(Target.Center, 1120f);
            return roughlySameDirectionAsHead && closeEnoughToTarget;
        }

        /// <summary>
        /// Renders a laser telegraph for a given <see cref="HadesBodyEternity"/> in a given direction.
        /// </summary>
        /// <param name="behaviorOverride">The behavior override responsible for the segment.</param>
        /// <param name="telegraphIntensityFactor">The intensity factor of the telegraph.</param>
        /// <param name="telegraphSize">How big the telegraph should be by default.</param>
        /// <param name="telegraphDirection">The direction of the telegraph</param>
        public static void RenderLaserTelegraph(HadesBodyEternity behaviorOverride, float telegraphIntensityFactor, float telegraphSize, Vector2 telegraphDirection)
        {
            float opacity = behaviorOverride.SegmentOpenInterpolant.Cubed();
            Vector2 start = behaviorOverride.TurretPosition;
            Texture2D invisible = MiscTexturesRegistry.InvisiblePixel.Value;

            // The multiplication by 0.5 is because this is being rendered to the pixelation target, wherein everything is downscaled by a factor of two, so that it can be upscaled later.
            Vector2 drawPosition = (start - Main.screenPosition) * 0.5f;

            float fadeOut = Utilities.InverseLerp(1f, PerpendicularBodyLaserBlasts_BurstShootCompletionRatio, telegraphIntensityFactor).Squared();
            Effect spread = Filters.Scene["CalamityMod:SpreadTelegraph"].GetShader().Shader;
            spread.Parameters["centerOpacity"].SetValue(0.4f);
            spread.Parameters["mainOpacity"].SetValue(opacity * 0.3f);
            spread.Parameters["halfSpreadAngle"].SetValue((1.1f - opacity) * fadeOut * 0.89f);
            spread.Parameters["edgeColor"].SetValue(Vector3.Lerp(new(1.3f, 0.1f, 0.67f), new(4f, 0f, 0f), telegraphIntensityFactor));
            spread.Parameters["centerColor"].SetValue(new Vector3(1f, 0.1f, 0.1f));
            spread.Parameters["edgeBlendLength"].SetValue(0.07f);
            spread.Parameters["edgeBlendStrength"].SetValue(32f);
            spread.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(invisible, drawPosition, null, Color.White, telegraphDirection.ToRotation(), invisible.Size() * 0.5f, Vector2.One * fadeOut * telegraphSize, SpriteEffects.None, 0f);
        }
    }
}
