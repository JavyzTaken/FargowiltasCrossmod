using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
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
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SuperchargedTurbodashes : ExoMechComboHandler
    {
        /// <summary>
        /// How long Ares spends electrifying Hades.
        /// </summary>
        public static int ElectrifyTime => Variables.GetAIInt("SuperchargedTurbodashes_ElectrifyTime", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How many dashes Hades should perform.
        /// </summary>
        public static int HadesDashCount => Variables.GetAIInt("SuperchargedTurbodashes_HadesDashCount", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How long each one of Hades' dashes lasts.
        /// </summary>
        public static int HadesDashCycleTime => Variables.GetAIInt("SuperchargedTurbodashes_HadesDashCycleTime", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The rate at which Ares' laser cannons fire at the player target after electrifying Hades.
        /// </summary>
        public static int AresCannonShootRate => Variables.GetAIInt("SuperchargedTurbodashes_AresCannonShootRate", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The general shoot rate at which Hades releases mines. Higher values equate to less mines.
        /// </summary>
        public static int HadesMineReleaseCycleTime => Variables.GetAIInt("SuperchargedTurbodashes_HadesMineReleaseCycleTime", ExoMechAIVariableType.Combo);

        /// <summary>
        /// Ares' max fly speed while electrifying Hades.
        /// </summary>
        public static float AresMaxFlySpeed_Electrify => Variables.GetAIFloat("SuperchargedTurbodashes_AresMaxFlySpeed_Electrify", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How far away Hades teleports when preparing to dash at the player target.
        /// </summary>
        public static float HadesTeleportOffset => Variables.GetAIFloat("SuperchargedTurbodashes_HadesTeleportOffset", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The maximum speed that Hades can go at while dashing at the player target.
        /// </summary>
        public static float HadesMaxDashSpeed => Variables.GetAIFloat("SuperchargedTurbodashes_HadesMaxDashSpeed", ExoMechAIVariableType.Combo);

        /// <summary>
        /// Ares' acceleration while electrifying Hades.
        /// </summary>
        public static float AresAcceleration_Electrify => Variables.GetAIFloat("AresAcceleration_Electrify", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The sound Ares plays when electrifying Hades.
        /// </summary>
        public static readonly SoundStyle HadesElectrifySound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/HadesElectrify");

        public override int[] ExpectedManagingExoMechs => [ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<AresBody>()];

        public override bool Perform(NPC npc)
        {
            if (npc.type == ExoMechNPCIDs.AresBodyID)
                return Perform_Ares(npc);

            Perform_Hades(npc);
            return AITimer >= ElectrifyTime + HadesDashCycleTime * HadesDashCount - 5;
        }

        /// <summary>
        /// Performs Ares' part in the SuperchargedTurbodashes attack.
        /// </summary>
        /// <param name="npc">Ares' NPC instance.</param>
        public static bool Perform_Ares(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out AresBodyEternity ares))
            {
                npc.active = false;
                return false;
            }

            // Bias light colors to red.
            ares.ShiftLightColors(LumUtils.InverseLerp(0f, 30f, AITimer), new(239, 62, 62), new(242, 112, 72));

            if (AITimer <= AresBodyEternity.DetachHands_DetachmentDelay)
            {
                for (int i = 0; i < ares.InstructionsForHands.Length; i++)
                {
                    int copyForDelegate = i;
                    ares.InstructionsForHands[i] = new(h => ares.DetachHandsUpdate(h, copyForDelegate));
                }
            }
            else
            {
                ares.InstructionsForHands[0] = new(h => AresHandUpdate(npc, h, new Vector2(-400f, 40f), 0));
                ares.InstructionsForHands[1] = new(h => AresHandUpdate(npc, h, new Vector2(-280f, 224f), 1));
                ares.InstructionsForHands[2] = new(h => AresHandUpdate(npc, h, new Vector2(280f, 224f), 2));
                ares.InstructionsForHands[3] = new(h => AresHandUpdate(npc, h, new Vector2(400f, 40f), 3));
            }

            int electrifyTimeStart = ElectrifyTime - LumUtils.SecondsToFrames(3.36f);
            if (electrifyTimeStart < 1)
                electrifyTimeStart = 1;

            int cannonChargeUpStart = ElectrifyTime - LumUtils.SecondsToFrames(2.54f);
            if (cannonChargeUpStart < 1)
                cannonChargeUpStart = 1;

            if (AITimer == electrifyTimeStart)
                SoundEngine.PlaySound(HadesElectrifySound).WithVolumeBoost(1.5f);
            if (AITimer == cannonChargeUpStart)
                SoundEngine.PlaySound(AresLaserCannon.TelSound).WithVolumeBoost(1.8f);

            // Laugh while charging Hades up.
            if (AITimer == (int)(ElectrifyTime * 0.4f))
                SoundEngine.PlaySound(AresBodyEternity.LaughSound).WithVolumeBoost(1.6f);
            if (AITimer >= ElectrifyTime * 0.4f)
                ares.AnimationState = AresBodyEternity.AresFrameAnimationState.Laugh;

            // Hover above Hades while charging him up.
            if (AITimer <= ElectrifyTime)
            {
                NPC hades = Main.npc[CalamityGlobalNPC.draedonExoMechWorm];

                float movementWindUpInterpolant = LumUtils.InverseLerp(0f, 40f, AITimer).Squared();
                Vector2 hoverDestination = hades.Center - Vector2.UnitY * 250f;
                Vector2 idealDirection = npc.SafeDirectionTo(hoverDestination);
                Vector2 acceleration = Vector2.One * AresAcceleration_Electrify * movementWindUpInterpolant;

                npc.Center = Vector2.Lerp(npc.Center, hoverDestination, 0.03f);
                npc.velocity = (npc.velocity + idealDirection * acceleration).ClampLength(0f, AresMaxFlySpeed_Electrify);
                if (npc.velocity.AngleBetween(idealDirection) >= 1.37f)
                    npc.velocity *= 0.94f;

                float electrifyInterpolant = LumUtils.InverseLerp(0f, ElectrifyTime, AITimer);
                float shakeIntensity = LumUtils.InverseLerpBump(0f, 0.75f, 0.95f, 1f, electrifyInterpolant).Squared() * 5f;
                ScreenShakeSystem.SetUniversalRumble(shakeIntensity, MathHelper.TwoPi, null, 0.2f);
            }

            // Hover side to side above the target player after Hades is charged up and active.
            else
            {
                float horizontalOffset = MathF.Cos(MathHelper.TwoPi * AITimer / 180f) * 600f;
                npc.SmoothFlyNear(Target.Center + new Vector2(horizontalOffset, -550f), 0.074f, 0.93f);
            }

            return false;
        }

        /// <summary>
        /// Handles updating for Ares' hands during the SuperchargedTurbodashes attack.
        /// </summary>
        /// <param name="aresBody">Ares' NPC instance.</param>
        /// <param name="hand">The hand ModNPC instance.</param>
        /// <param name="hoverOffset">The offset for the hand relative to the body's position.</param>
        /// <param name="aimDestination">Where the hand should aim.</param>
        /// <param name="armIndex">The arm's index in the overall set.</param>
        public static void AresHandUpdate(NPC aresBody, AresHand hand, Vector2 hoverOffset, int armIndex)
        {
            NPC handNPC = hand.NPC;
            Vector2 hoverDestination = aresBody.Center + hoverOffset * aresBody.scale;

            hand.UsesBackArm = armIndex == 0 || armIndex == AresBodyEternity.ArmCount - 1;
            hand.ArmSide = (armIndex >= AresBodyEternity.ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.LaserCannon;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.GlowmaskDisabilityInterpolant = 0f;
            handNPC.spriteDirection = 1;
            handNPC.Opacity = LumUtils.Saturate(handNPC.Opacity + 0.3f);

            Vector2 aimDestination = handNPC.Center + Vector2.UnitY * 300f;

            // Firstly, emit electricity and dust from the cannon that electrifies Hades.
            Vector2 cannonEnd = handNPC.Center + new Vector2(handNPC.spriteDirection * 76f, 8f).RotatedBy(handNPC.rotation);
            if (AITimer <= ElectrifyTime)
            {
                // Pick the nearest Hades segment to electrify.
                float minDistance = 999999f;
                foreach (NPC hadesSegment in Main.ActiveNPCs)
                {
                    if (hadesSegment.type != ExoMechNPCIDs.HadesHeadID && hadesSegment.realLife != CalamityGlobalNPC.draedonExoMechWorm)
                        continue;

                    if (minDistance > handNPC.Distance(hadesSegment.Center))
                    {
                        minDistance = handNPC.Distance(hadesSegment.Center);
                        aimDestination = hadesSegment.Center;
                    }
                }

                // Emit arcs.
                float arcCreationChance = MathF.Pow(LumUtils.InverseLerp(0f, ElectrifyTime, AITimer), 0.75f) * 0.6f;
                for (int i = 0; i < 3; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(arcCreationChance))
                    {
                        Vector2 arcLength = (aimDestination - cannonEnd).RotatedByRandom(0.02f) * Main.rand.NextFloat(0.97f, 1.03f);
                        int arc = LumUtils.NewProjectileBetter(handNPC.GetSource_FromAI(), cannonEnd, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(6, 9), 1f);
                        if (Main.projectile.IndexInRange(arc))
                            Main.projectile[arc].As<SmallTeslaArc>().WidthFactor = Main.rand.NextFloat(1f, 2f);
                    }
                }

                // Create dust.
                Dust energy = Dust.NewDustPerfect(cannonEnd, 182);
                energy.velocity = handNPC.SafeDirectionTo(cannonEnd).RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloatDirection() * 6f + handNPC.velocity;
                energy.noGravity = true;

                hand.EnergyDrawer.chargeProgress = MathF.Sqrt(LumUtils.InverseLerpBump(0f, ElectrifyTime * 0.32f, ElectrifyTime * 0.81f, ElectrifyTime, AITimer));
            }

            // After Hades is charged up and active, cease attacking.
            else
            {
                aimDestination = Target.Center;
                hand.EnergyDrawer.chargeProgress = 0f;
            }

            handNPC.SmoothFlyNear(hoverDestination, 0.25f, 0.75f);
            handNPC.rotation = handNPC.rotation.AngleLerp(handNPC.AngleTo(aimDestination), 0.12f);
        }

        /// <summary>
        /// Performs Hades' part in the SuperchargedTurbodashes attack.
        /// </summary>
        /// <param name="npc">Hades' NPC instance.</param>
        public static void Perform_Hades(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out HadesHeadEternity hades))
            {
                npc.active = false;
                return;
            }

            bool shootMines = false;
            float electrifyInterpolant =
                LumUtils.InverseLerp(0f, ElectrifyTime, AITimer) *
                LumUtils.InverseLerp(ElectrifyTime + HadesDashCycleTime * HadesDashCount - 10f, ElectrifyTime + HadesDashCycleTime * HadesDashCount - 30f, AITimer);

            // Stay near the player while being electrified.
            if (electrifyInterpolant < 1f)
            {
                Vector2 hoverDestination = Target.Center;
                npc.Center = Vector2.Lerp(npc.Center, hoverDestination, 0.045f);
                npc.velocity = Vector2.Lerp(npc.velocity, npc.SafeDirectionTo(hoverDestination) * 15f, 0.032f);
                npc.damage = 0;
            }
            else
            {
                if (AITimer == ElectrifyTime + 1)
                    SoundEngine.PlaySound(HadesHeadEternity.DashChargeUpSound).WithVolumeBoost(2f);

                // Handle the dash cycle, teleporting behind the player and dashing, in a cycle.
                int cycleTimer = (AITimer - ElectrifyTime) % HadesDashCycleTime;
                bool hasDashed = AITimer >= ElectrifyTime + HadesDashCycleTime - 1;
                if (cycleTimer == HadesDashCycleTime - 1)
                {
                    Vector2 teleportOffsetDirection = -Target.velocity.SafeNormalize(Vector2.UnitX * Target.direction);
                    npc.Center = Target.Center + teleportOffsetDirection * HadesTeleportOffset;
                    npc.velocity = npc.SafeDirectionTo(Target.Center) * 10f;
                    npc.netUpdate = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), npc.Center, npc.velocity, ModContent.ProjectileType<HadesLineTelegraph>(), 0, 0f, -1, 32f);
                        foreach (NPC segment in Main.ActiveNPCs)
                        {
                            if (segment.realLife == npc.whoAmI)
                            {
                                segment.Center = npc.Center;
                                segment.netUpdate = true;
                            }
                        }
                    }
                    SoundEngine.PlaySound(HadesHeadEternity.DashChargeUpSound with { MaxInstances = 0 }).WithVolumeBoost(2f);
                }
                else
                {
                    npc.velocity = (npc.velocity * 1.04f + npc.velocity.SafeNormalize(Vector2.UnitY)).ClampLength(0f, HadesMaxDashSpeed);
                    shootMines = true;
                }

                bool doDamage = AITimer >= ElectrifyTime + 45;
                npc.damage = doDamage ? npc.defDamage : 0;
            }

            hades.SegmentReorientationStrength = 0f;
            hades.BodyBehaviorAction = new(HadesHeadEternity.EveryNthSegment(2), new(segment =>
            {
                HadesHeadEternity.OpenSegment().Invoke(segment);

                // Do damage if Hades is doing damage.
                if (npc.damage >= 1)
                    segment.NPC.damage = segment.NPC.defDamage;

                if (Main.rand.NextBool(electrifyInterpolant.Squared()))
                {
                    float opacity = 0.7f;
                    Vector2 spawnPosition = segment.NPC.Center + Main.rand.NextVector2Circular(40f, 40f);
                    ElectricSparkParticle spark = new(spawnPosition, Main.rand.NextVector2CircularEdge(14f, 14f), Color.White * opacity, Color.Red * opacity * 0.7f, Main.rand.Next(10, 15), Vector2.One * Main.rand.NextFloat(0.3f, 0.65f));
                    spark.Spawn();
                }

                // Create electricity off Hades' body.
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(electrifyInterpolant * 0.1f))
                {
                    Vector2 arcLength = Main.rand.NextVector2Unit() * Main.rand.NextFloat(50f, 185f);
                    LumUtils.NewProjectileBetter(segment.NPC.GetSource_FromAI(), segment.TurretPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(6, 9), 1f);
                }

                // Release mines outward.
                if (shootMines && (AITimer * 3 + segment.RelativeIndex * 9) % HadesMineReleaseCycleTime == 0)
                {
                    Vector2 laserSpawnPosition = segment.TurretPosition;

                    SoundEngine.PlaySound(CommonCalamitySounds.ExoLaserShootSound with { MaxInstances = 0, Volume = 0.3f, PitchVariance = 0.27f }, laserSpawnPosition);

                    Vector2 perpendicularDirection = npc.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.PiOver2);
                    HadesHeadEternity.PerpendicularBodyLaserBlasts_CreateLaserBurstParticles(laserSpawnPosition, -perpendicularDirection);
                    HadesHeadEternity.PerpendicularBodyLaserBlasts_CreateLaserBurstParticles(laserSpawnPosition, perpendicularDirection);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        // Some mines have a super short lifetime, to create neat explosion effects atop Hades.
                        int mineLifetime = Main.rand.NextBool(4) ? 3 : 120;

                        float mineShootSpeed = Main.rand.NextFloat(120f);
                        Utilities.NewProjectileBetter(segment.NPC.GetSource_FromAI(), laserSpawnPosition, perpendicularDirection * mineShootSpeed, ModContent.ProjectileType<HadesMine>(), HadesHeadEternity.MineDamage, 0f, -1, mineLifetime);
                        Utilities.NewProjectileBetter(segment.NPC.GetSource_FromAI(), laserSpawnPosition, perpendicularDirection * -mineShootSpeed, ModContent.ProjectileType<HadesMine>(), HadesHeadEternity.MineDamage, 0f, -1, mineLifetime);
                    }
                }
            }));

            // Apply post-processing shaders to Hades to indicate him being electrified.
            HadesPostProcessingSystem.PostProcessingAction = () =>
            {
                float[] blurWeights = new float[12];
                for (int i = 0; i < blurWeights.Length; i++)
                    blurWeights[i] = Utilities.GaussianDistribution(i / (float)(blurWeights.Length - 1f) * 1.5f, 0.6f);

                float flash = LumUtils.InverseLerpBump(0.6f, 0.7f, 0.7f, 0.8f, electrifyInterpolant);
                for (int i = 0; i < 2; i++)
                {
                    ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.MotionBlurShader");
                    shader.TrySetParameter("blurInterpolant", electrifyInterpolant * 0.5f);
                    shader.TrySetParameter("blurWeights", blurWeights);
                    shader.TrySetParameter("blurDirection", (MathHelper.TwoPi * i / 2f).ToRotationVector2());
                    shader.Apply();
                    Main.spriteBatch.Draw(HadesPostProcessingSystem.HadesTarget, Main.screenLastPosition - Main.screenPosition, null, new Color(255, 34, 1, 0) * electrifyInterpolant, 0f, Vector2.Zero, 2f, 0, 0f);
                }

                ManagedShader electricShader = ShaderManager.GetShader("FargowiltasCrossmod.HadesSuperchargeShader");
                electricShader.TrySetParameter("electricityColor", new Color(255, 14, 20).ToVector4() * electrifyInterpolant * 2f);
                electricShader.TrySetParameter("electrifyInterpolant", electrifyInterpolant + flash);
                electricShader.Apply();
            };
            if (electrifyInterpolant <= 0f)
                HadesPostProcessingSystem.PostProcessingAction = null;

            // Rotate forward.
            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
