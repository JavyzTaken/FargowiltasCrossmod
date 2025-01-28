using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Common;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExoelectricPortalDashes : ExoMechComboHandler
    {
        public override bool ValidStartingAttack => false;

        public override int[] ExpectedManagingExoMechs => [ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<Apollo>()];

        /// <summary>
        /// Whether Artemis, Apollo, and Hades have emerged from the portal and are ready to dash.
        /// </summary>
        public static bool DashOngoing => AITimer >= ExoTwinHyperfuturisticPortal.Lifetime / 2 + PortalEnterWaitDelay + 10;

        /// <summary>
        /// The amount of portal dashes to perform.
        /// </summary>
        public static int DashCount => Variables.GetAIInt("ExothermalLaserDashes_DashCount", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The amount of lasers to release from the portal when a dash happens.
        /// </summary>
        public static int LasersPerBurst => Variables.GetAIInt("ExothermalLaserDashes_LasersPerBurst", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How long Apollo waits between entering one portal and emerging from another one.
        /// </summary>
        public static int PortalEnterWaitDelay => Variables.GetAIInt("ExothermalLaserDashes_PortalEnterWaitDelay", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The initial firing speed of laser bursts created from a portal when a dash happens.
        /// </summary>
        public static float LaserBurstStartingSpeed => Variables.GetAIFloat("ExothermalLaserDashes_LaserBurstStartingSpeed", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The initial dash speed of Artemis, Apollo, and Hades when a dash happens.
        /// </summary>
        public static float PortalEmergeStartingDashSpeed => Variables.GetAIFloat("ExothermalLaserDashes_PortalEmergeStartingDashSpeed", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The dash offset angle of Artemis and Apollo when a dash happens.
        /// </summary>
        public static float PortalEmergeExoTwinOffsetAngle => Variables.GetAIFloat("ExothermalLaserDashes_PortalEmergeExoTwinOffsetAngle", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The sound Hades make when warping through the Exo Twins' big portal.
        /// </summary>
        public static readonly SoundStyle HeavyPortalWarpSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/ExoTwins/PortalWarpHeavy");

        public override bool Perform(NPC npc)
        {
            if (npc.type == ExoMechNPCIDs.ArtemisID || npc.type == ExoMechNPCIDs.ApolloID)
            {
                Perform_ExoTwin(npc);
                return false;
            }

            return Perform_Hades(npc);
        }

        /// <summary>
        /// Performs Hades' part in the ExoelectricPortalDashes attack.
        /// </summary>
        /// <param name="npc">Hades' NPC instance.</param>
        public static bool Perform_Hades(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out HadesHeadEternity hades))
                return true;

            bool endAttack = false;
            hades.SegmentReorientationStrength = 0.09f;
            npc.velocity *= 1.025f;
            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
            npc.dontTakeDamage = true;

            ref float wentThroughPortal = ref npc.ai[1];

            Projectile? portal = TryToFindHadesPortal();
            bool allSegmentsHaveEnteredPortal = true;
            if (portal is not null && allSegmentsHaveEnteredPortal)
            {
                foreach (NPC segment in Main.ActiveNPCs)
                {
                    if (segment.realLife == npc.whoAmI || segment.whoAmI == npc.whoAmI)
                    {
                        bool segmentHasEnteredPortal = SegmentHasEnteredPortal(portal, segment);
                        if (!segmentHasEnteredPortal)
                        {
                            allSegmentsHaveEnteredPortal = false;
                            break;
                        }
                    }
                }
            }

            // If the portal isn't present, reset the "have the segments entered a portal?" check to true.
            if (portal is null)
                allSegmentsHaveEnteredPortal = true;

            // Otherwise, if a portal is present, do various things.
            else
            {
                // Apply a shader that makes Hades segments disappear as they enter the portal
                HadesPostProcessingSystem.PostProcessingAction = () =>
                {
                    float[] blurWeights = new float[12];
                    for (int i = 0; i < blurWeights.Length; i++)
                        blurWeights[i] = Utilities.GaussianDistribution(i / (float)(blurWeights.Length - 1f) * 1.5f, 0.6f);

                    ManagedShader cutoffShader = ShaderManager.GetShader("FargowiltasCrossmod.HadesSegmentCutoffShader");
                    cutoffShader.TrySetParameter("blurWeights", blurWeights);
                    cutoffShader.TrySetParameter("blurInterpolant", LumUtils.InverseLerp(65f, 175f, npc.velocity.Length()));
                    cutoffShader.TrySetParameter("blurDirection", npc.velocity.SafeNormalize(Vector2.Zero));
                    cutoffShader.TrySetParameter("originDirection", portal.velocity);
                    cutoffShader.TrySetParameter("disappearPoint", Vector2.Transform(portal.Center - Main.screenPosition, Main.GameViewMatrix.TransformationMatrix));
                    cutoffShader.Apply();
                };

                // Ensure that the portal stays alive as long as there are segments entering it.
                ExoTwinHyperfuturisticPortal portalModProjectile = portal.As<ExoTwinHyperfuturisticPortal>();
                if (!allSegmentsHaveEnteredPortal && portalModProjectile.Time > 11f && !DashOngoing)
                    portalModProjectile.Time = 11f;
            }

            if ((AITimer <= 10 || !allSegmentsHaveEnteredPortal) && wentThroughPortal == 0f)
                Perform_Hades_EnterPortal(npc);
            else
            {
                endAttack = Perform_Hades_PerformPortalDashes(npc, ref wentThroughPortal);
                hades.SegmentReorientationStrength = 1000f;
            }

            hades.BodyBehaviorAction = new HadesHeadEternity.BodySegmentInstructions(HadesHeadEternity.AllSegments(), segment =>
            {
                NPC segmentNPC = segment.NPC;
                bool inPortal = SegmentHasEnteredPortal(portal, segmentNPC);
                segment.DisableMapIcon = SegmentHasEnteredPortal(portal, segmentNPC);
                segmentNPC.dontTakeDamage = segment.DisableMapIcon;
                segmentNPC.ShowNameOnHover = !segment.DisableMapIcon;
                segmentNPC.damage = DashOngoing ? segmentNPC.defDamage : 0;
                HadesHeadEternity.CloseSegment().Invoke(segment);

                Vector2 nudgeBack = (segmentNPC.rotation - MathHelper.PiOver2).ToRotationVector2() * -40f;
                segmentNPC.Center += nudgeBack;
                bool wasInPortal = SegmentHasEnteredPortal(portal, segmentNPC);
                segmentNPC.Center -= nudgeBack;

                if (!DashOngoing && inPortal && !wasInPortal)
                {
                    if (Main.rand.NextBool(3))
                        CreatePortalEnterVisuals(portal, false, 1.4f);
                    if (segment.RelativeIndex == 1)
                        SoundEngine.PlaySound(HeavyPortalWarpSound).WithVolumeBoost(1.9f);
                }
            });
            hades.DisableMapIconLocally = SegmentHasEnteredPortal(portal, npc);

            return endAttack;
        }

        /// <summary>
        /// Handles Hades' portal entering behavior.
        /// </summary>
        /// <param name="npc">Hades' NPC instance.</param>
        public static void Perform_Hades_EnterPortal(NPC npc)
        {
            // Summon the portal ahead of Hades at first.
            if (Main.netMode != NetmodeID.MultiplayerClient && AITimer == 9)
            {
                npc.ai[0] = 0f;

                Vector2 portalDirection = -npc.velocity.SafeNormalize(Vector2.UnitY);
                Vector2 portalSpawnPosition = npc.Center - portalDirection * 1024f;
                Vector2 stayInWorldFluff = Vector2.One * 900f;

                // Ensure that the portal doesn't spawn outside of the portal and immediately despawn by clamping the position into the world.
                portalSpawnPosition = Vector2.Clamp(portalSpawnPosition, stayInWorldFluff, new Vector2(Main.maxTilesX, Main.maxTilesY) * 16f - stayInWorldFluff);

                LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), portalSpawnPosition, portalDirection, ModContent.ProjectileType<ExoTwinHyperfuturisticPortal>(), 0, 0f, -1, 0f, 0f, 1f);

                if (npc.velocity.Length() < 20f)
                    npc.velocity = npc.velocity.SafeNormalize(Vector2.UnitY) * 20f;
                npc.netUpdate = true;
            }

            if (AITimer > 10)
                AITimer = 10;
        }

        /// <summary>
        /// Handles Hades' portal dashing behavior.
        /// </summary>
        /// <param name="npc">Hades' NPC instance.</param>
        public static bool Perform_Hades_PerformPortalDashes(NPC npc, ref float wentThroughPortal)
        {
            wentThroughPortal = 1f;

            // Summon the portal near the target.
            if (Main.netMode != NetmodeID.MultiplayerClient && AITimer == PortalEnterWaitDelay + 10)
            {
                Vector2 portalSpawnPosition = Target.Center - Target.velocity.SafeNormalize(Main.rand.NextVector2Unit()).RotatedByRandom(MathHelper.PiOver2) * new Vector2(700f, 420f);
                Vector2 portalDirection = portalSpawnPosition.SafeDirectionTo(Target.Center);
                LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), portalSpawnPosition, portalDirection, ModContent.ProjectileType<ExoTwinHyperfuturisticPortal>(), 0, 0f, -1, 0f, 0f, 1f);

                npc.netUpdate = true;
            }

            // Make Hades, Artemis, and Apollo emerge from the portal.
            if (AITimer == ExoTwinHyperfuturisticPortal.Lifetime / 2 + PortalEnterWaitDelay + 10)
            {
                ScreenShakeSystem.StartShake(25f, MathHelper.TwoPi, null, 0.8f);
                SoundEngine.PlaySound(HadesHeadEternity.SideLaserBurstSound with { MaxInstances = 0 }).WithVolumeBoost(2f);
                SoundEngine.PlaySound(PlasmaChaseSequence.PortalWarpSound with { MaxInstances = 0 });

                bool doneAttacking = npc.ai[2] >= DashCount;

                Projectile? portal = TryToFindHadesPortal();
                if (Main.netMode != NetmodeID.MultiplayerClient && portal is not null)
                {
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        bool isExoTwin = n.type == ExoMechNPCIDs.ArtemisID || n.type == ExoMechNPCIDs.ApolloID;
                        if (n.whoAmI == npc.whoAmI || n.realLife == npc.whoAmI || isExoTwin)
                        {
                            float offsetAngle = 0f;
                            if (n.type == ExoMechNPCIDs.ArtemisID)
                                offsetAngle = -PortalEmergeExoTwinOffsetAngle;
                            if (n.type == ExoMechNPCIDs.ApolloID)
                                offsetAngle = PortalEmergeExoTwinOffsetAngle;

                            n.Center = portal.Center;
                            if (isExoTwin)
                                n.ai[2] = 0f;
                            n.velocity = portal.velocity.RotatedBy(offsetAngle) * PortalEmergeStartingDashSpeed;
                            if (n.realLife == npc.whoAmI && n.TryGetDLCBehavior(out HadesBodyEternity body))
                            {
                                n.Center -= portal.velocity * body.RelativeIndex;
                                n.velocity = Vector2.Zero;
                            }
                            n.oldPos = new Vector2[n.oldPos.Length];

                            n.netUpdate = true;
                        }
                    }

                    if (!doneAttacking)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 plasmaVelocity = portal.velocity.RotatedBy(MathHelper.Lerp(-0.81f, 0.81f, i / 7f)) * 30f + Main.rand.NextVector2Circular(5f, 5f);
                            LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), portal.Center, plasmaVelocity, ModContent.ProjectileType<ApolloPlasmaFireball>(), ExoTwinsStates.BasicShotDamage, 0f, -1, Target.Center.X, Target.Center.Y, 1f);
                        }
                        for (int i = 0; i < 24; i++)
                        {
                            Vector2 missileVelocity = portal.SafeDirectionTo(Target.Center).RotatedBy(MathHelper.Lerp(-2.49f, 2.49f, i / 23f) + MathHelper.Pi) * 4f + Main.rand.NextVector2Circular(0.4f, 0.4f);
                            LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), portal.Center, missileVelocity, ModContent.ProjectileType<ApolloMissile2>(), ExoTwinsStates.BasicShotDamage, 0f);
                        }
                    }
                    else
                        npc.velocity *= 5.6f;
                }

                if (doneAttacking)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                }

                return doneAttacking;
            }

            if (DashOngoing)
                npc.velocity = (npc.velocity * 1.07f + npc.velocity.SafeNormalize(Vector2.Zero) * 18f).ClampLength(0f, 396f);

            if (AITimer >= ExoTwinHyperfuturisticPortal.Lifetime / 2 + PortalEnterWaitDelay + 10 && !npc.WithinRange(Target.Center, 2800f))
            {
                npc.ai[2]++;
                int portalID = ModContent.ProjectileType<ExoTwinHyperfuturisticPortal>();
                int fireballID = ModContent.ProjectileType<ApolloPlasmaFireball>();
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    if (projectile.type == portalID || projectile.type == fireballID)
                        projectile.Kill();
                }

                AITimer = 0;
                wentThroughPortal = 0f;
                npc.netUpdate = true;
            }

            return false;
        }

        /// <summary>
        /// Finds and returns the portal that Hades should enter.
        /// </summary>
        public static Projectile? TryToFindHadesPortal()
        {
            int portalID = ModContent.ProjectileType<ExoTwinHyperfuturisticPortal>();
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.type == portalID && projectile.As<ExoTwinHyperfuturisticPortal>().ForHades)
                    return projectile;
            }

            return null;
        }

        /// <summary>
        /// Finds and returns the portal that a given Exo Twin should enter.
        /// </summary>
        public static Projectile? TryToFindExoTwinPortal(int identifier)
        {
            int portalID = ModContent.ProjectileType<ExoTwinHyperfuturisticPortal>();
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.type == portalID && projectile.As<ExoTwinHyperfuturisticPortal>().Identifier == identifier)
                    return projectile;
            }

            return null;
        }

        /// <summary>
        /// Determines whether a given Hades segments is invisible due to being ahead of the portal created for him.
        /// </summary>
        /// <param name="portal">The portal that the segment is expected to enter.</param>
        /// <param name="segment">The segment.</param>
        public static bool SegmentHasEnteredPortal(Projectile portal, NPC segment)
        {
            if (portal is null)
                return false;

            Vector2 portalDirection = portal.velocity.SafeNormalize(Vector2.UnitY);
            Vector2 directionToPortal = segment.SafeDirectionTo(portal.Center - portalDirection);
            bool aheadOfPortal = Vector2.Dot(portalDirection, directionToPortal) > 0f;
            return aheadOfPortal || !segment.WithinRange(Target.Center, 10000f);
        }

        /// <summary>
        /// Creates various portal entering visuals, such as screen shake.
        /// </summary>
        /// <param name="portal"></param>
        public static void CreatePortalEnterVisuals(Projectile portal, bool playSound, float visualsScale)
        {
            if (playSound && portal.WithinRange(Main.LocalPlayer.Center, 2600f))
            {
                SoundEngine.PlaySound(PlasmaChaseSequence.PortalWarpSound);
                ScreenShakeSystem.StartShakeAtPoint(portal.Center, 16f, MathHelper.TwoPi, null, 1.1f);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < visualsScale * 11f; i++)
                {
                    Vector2 arcSpawnPosition = portal.Center + Main.rand.NextVector2Circular(100f, 100f) * visualsScale;
                    Vector2 arcDestination = arcSpawnPosition + portal.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(MathHelper.Pi / 5f) * Main.rand.NextFloat(84f, 900f) * visualsScale;
                    Vector2 arcLength = (arcDestination - arcSpawnPosition).RotatedByRandom(0.12f) * Main.rand.NextFloat(0.9f, 1f);
                    LumUtils.NewProjectileBetter(portal.GetSource_FromAI(), arcSpawnPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(10, 20));
                }
            }
            CustomExoMechsSky.CreateLightning((portal.Center - Main.screenPosition) / Main.ScreenSize.ToVector2());
        }

        /// <summary>
        /// Performs The Exo Twins' part in the ExoelectricPortalDashes attack.
        /// </summary>
        /// <param name="npc">The Exo Twins' NPC instance.</param>
        public static void Perform_ExoTwin(NPC npc)
        {
            if (!Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechWorm))
                return;

            IExoTwin twinInstance = null;
            if (npc.TryGetDLCBehavior(out ArtemisEternity artemis))
                twinInstance = artemis;
            else if (npc.TryGetDLCBehavior(out ApolloEternity apollo))
                twinInstance = apollo;
            if (twinInstance is null)
                return;

            NPC hades = Main.npc[CalamityGlobalNPC.draedonExoMechWorm];
            bool isApollo = npc.type == ExoMechNPCIDs.ApolloID;
            Vector2 hadesDirection = hades.velocity.SafeNormalize(Vector2.UnitY);
            Vector2 hoverDestination = hades.Center + hadesDirection.RotatedBy(MathHelper.PiOver2 * isApollo.ToDirectionInt()) * 400f - hadesDirection * 285f;
            Projectile? hadesPortal = TryToFindHadesPortal();

            ref float localTimer = ref npc.ai[1];
            ref float invisible = ref npc.ai[2];
            ref float hasCreatedPortal = ref npc.ai[3];

            bool stayNearHades = AITimer <= 10 && hadesPortal is not null && !SegmentHasEnteredPortal(hadesPortal, hades);

            twinInstance.Animation = ExoTwinAnimation.ChargingUp;
            twinInstance.MotionBlurInterpolant = LumUtils.InverseLerp(40f, 100f, npc.velocity.Length());

            if (DashOngoing)
            {
                twinInstance.Animation = ExoTwinAnimation.Attacking;
                npc.velocity = (npc.velocity * 1.1f).ClampLength(0f, 150f);
                npc.rotation = npc.velocity.ToRotation();
            }
            else if (stayNearHades)
            {
                npc.rotation = npc.rotation.AngleLerp(hadesDirection.ToRotation(), 0.5f);
                npc.SmoothFlyNear(hoverDestination, 0.2f, 0.55f);
                hasCreatedPortal = 0f;
                invisible = 0f;
            }
            else
            {
                npc.velocity = (npc.velocity + npc.velocity.SafeNormalize(Vector2.UnitY) * 5.6f).ClampLength(0f, 165f);

                int portalIdentifier = isApollo.ToDirectionInt();
                if (Main.netMode != NetmodeID.MultiplayerClient && hasCreatedPortal == 0f)
                {
                    Vector2 portalDirection = -npc.velocity.SafeNormalize(Vector2.UnitY);
                    LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), npc.Center + npc.velocity * 24f, portalDirection, ModContent.ProjectileType<ExoTwinHyperfuturisticPortal>(), 0, 0f, -1, portalIdentifier);
                    hasCreatedPortal = 1f;
                }

                Projectile? myPortal = TryToFindExoTwinPortal(portalIdentifier);
                if (myPortal is not null && npc.WithinRange(myPortal.Center, 225f))
                {
                    CreatePortalEnterVisuals(myPortal, true, 0.67f);
                    invisible = 1f;
                    npc.netUpdate = true;
                }
            }

            localTimer++;

            npc.Opacity = 1f - invisible;
            npc.dontTakeDamage = invisible == 1f;
            twinInstance.ThrusterBoost = LumUtils.InverseLerp(40f, 100f, npc.velocity.Length()) * 2f;
            twinInstance.Frame = twinInstance.Animation.CalculateFrame((int)localTimer / 35f % 1f, twinInstance.InPhase2);
        }
    }
}
