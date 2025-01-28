using CalamityMod.Sounds;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades
{
    public sealed partial class HadesHeadEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The side offset direction of Hades during his Missile Tail Snaps attack.
        /// </summary>
        public ref float MissileTailSnaps_SideOffsetDirection => ref NPC.ai[2];

        /// <summary>
        /// How long Hades spends reorientation to get his segments into position during his Missile Tail Snaps attack.
        /// </summary>
        public static int MissileTailSnaps_ReorientTime => Variables.GetAIInt("MissileTailSnaps_ReorientTime", ExoMechAIVariableType.Hades);

        /// <summary>
        /// AI update loop method for the MissileTailSnaps attack.
        /// </summary>
        public void DoBehavior_MissileTailSnaps()
        {
            BodyBehaviorAction = new(EveryNthSegment(2), OpenSegment());

            SegmentReorientationStrength = 0.28f;

            if (AITimer < MissileTailSnaps_ReorientTime)
            {
                if (AITimer == 1)
                {
                    MissileTailSnaps_SideOffsetDirection = NPC.AngleTo(Target.Center) + MathHelper.PiOver2;
                    NPC.netUpdate = true;
                }

                Vector2 sideOffset = MissileTailSnaps_SideOffsetDirection.ToRotationVector2() * 400f;
                Vector2 forwardOffset = (MissileTailSnaps_SideOffsetDirection - MathHelper.PiOver2).ToRotationVector2() * BodySegmentCount * NPC.scale * 50f;
                Vector2 hoverDestination = Target.Center + forwardOffset + sideOffset;
                NPC.SmoothFlyNearWithSlowdownRadius(hoverDestination, 0.06f, 0.92f, 250f);
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                NPC.velocity *= 0.84f;
                BodyBehaviorAction = new(OnlyTailSegment(), DoBehavior_MissileTailSnaps_ReleaseMissile());
            }

            if (AITimer == MissileTailSnaps_ReorientTime)
            {
                BodyBehaviorAction = new(OnlyTailSegment(), new(tail =>
                {
                    Vector2 directionToPlayer = tail.NPC.Center.SafeDirectionTo(Target.Center);
                    Vector2 tailDirection = (tail.NPC.rotation - MathHelper.PiOver2).ToRotationVector2();
                    float snapDirection = Vector2.Dot(tailDirection, directionToPlayer).NonZeroSign();
                    tail.SpringForce = snapDirection * 0.5f;
                }));
            }

            if (AITimer >= MissileTailSnaps_ReorientTime + 120)
                SelectNewState();

            NPC.damage = 0;
        }

        /// <summary>
        /// An action that causes a given segment to release a significant quantity of missiles.
        /// </summary>
        /// <param name="segmentOpenRate">The amount by which the segment open interpolant changes every frame.</param>
        /// <param name="smokeQuantityInterpolant">A multiplier for how much smoke should be released.</param>
        public static BodySegmentAction DoBehavior_MissileTailSnaps_ReleaseMissile()
        {
            return new(behaviorOverride =>
            {
                NPC segment = behaviorOverride.NPC;
                bool segmentIsMovingQuickly = !segment.oldPosition.WithinRange(segment.position, 54f);
                bool segmentWithinDistanceRange = !segment.WithinRange(Target.Center, 270f) && segment.WithinRange(Target.Center, 960f);

                if (segmentIsMovingQuickly && segmentWithinDistanceRange)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 arcSpawnPosition = segment.Center + Main.rand.NextVector2Circular(15f, 15f);
                            Vector2 arcDestination = arcSpawnPosition + Main.rand.NextVector2Unit() * Main.rand.NextFloat(150f, 1100f);
                            Vector2 arcLength = (arcDestination - arcSpawnPosition).RotatedByRandom(0.12f) * Main.rand.NextFloat(0.9f, 1f);
                            LumUtils.NewProjectileBetter(segment.GetSource_FromAI(), arcSpawnPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(9, 18), 1f);
                        }
                    }

                    if (Main.rand.NextBool(7))
                        CustomExoMechsSky.CreateLightning((segment.Center - Main.screenPosition) / Main.ScreenSize.ToVector2());

                    for (int i = 0; i < 3; i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(3))
                            LumUtils.NewProjectileBetter(segment.GetSource_FromAI(), behaviorOverride.TurretPosition + Main.rand.NextVector2Circular(800f, 800f), Vector2.Zero, ModContent.ProjectileType<SmallHadesSegmentExplosion>(), 0, 0f);
                    }

                    if (LumUtils.CountProjectiles(ModContent.ProjectileType<HadesMissile>()) < 30)
                    {
                        SoundEngine.PlaySound(CommonCalamitySounds.LargeWeaponFireSound with { MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew, Volume = 3f });
                        ScreenShakeSystem.StartShake(6f);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 missileVelocity = segment.SafeDirectionTo(Target.Center).RotatedByRandom(MathHelper.Pi / 5f) * -59f;
                                LumUtils.NewProjectileBetter(segment.GetSource_FromAI(), behaviorOverride.TurretPosition, missileVelocity, ModContent.ProjectileType<HadesMissile>(), MissileDamage, 0f);
                            }
                        }
                    }
                }
            });
        }
    }
}
