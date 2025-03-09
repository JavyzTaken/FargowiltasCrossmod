using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls.Core.Systems;
using Luminance.Assets;
using Luminance.Common.Easings;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExoFlurry : ExoMechComboHandler
    {
        /// <summary>
        /// How long Ares spends charging up before firing his nuke.
        /// </summary>
        public static int NukeChargeUpTime => Variables.GetAIInt("ExoFlurry_NukeChargeUpTime", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How long Ares waits after firing his nuke before being able to begin attempting to fire a new one.
        /// </summary>
        public static int NukeShotDelay => Variables.GetAIInt("ExoFlurry_NukeShotDelay", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How long the Exo Twins spend dashing.
        /// </summary>
        public static int ExoTwinsDashTime => Variables.GetAIInt("ExoFlurry_ExoTwinsDashTime", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How long the Exo Twins spend slowing down after a spin.
        /// </summary>
        public static int ExoTwinsSpinSlowdownTime => Variables.GetAIInt("ExoFlurry_ExoTwinsSpinSlowdownTime", ExoMechAIVariableType.Combo);

        /// <summary>
        /// How long the Exo Twins spend spinning.
        /// </summary>
        public static int ExoTwinsSpinTime => Variables.GetAIInt("ExoFlurry_ExoTwinsSpinTime", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The minimum Y offset factor on the unit circle the Exo Twins can have when spinning.
        /// </summary>
        public static float MinExoTwinYOrientation => Variables.GetAIFloat("ExoFlurry_MinExoTwinYOrientation", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The amount by which Hades turns his head when firing his laser blast.
        /// </summary>
        public static float HadesTurnSpeedCoefficient => Variables.GetAIFloat("ExoFlurry_HadesTurnSpeedCoefficient", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The amount by which Ares accelerates downwards.
        /// </summary>
        public static float AresFlyDownAcceleration => Variables.GetAIFloat("ExoFlurry_AresFlyDownAcceleration", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The terminal velocity of Ares' downward acceleration.
        /// </summary>
        public static float AresMaxFlyDownSpeed => Variables.GetAIFloat("ExoFlurry_AresMaxFlyDownSpeed", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The starting speed of the Exo Twins when dashing.
        /// </summary>
        public static float ExoTwinsStartingDashSpeed => Variables.GetAIFloat("ExoFlurry_ExoTwinsStartingDashSpeed", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The maximum speed of the Exo Twins when dashing.
        /// </summary>
        public static float ExoTwinsMaxDashSpeed => Variables.GetAIFloat("ExoFlurry_ExoTwinsMaxDashSpeed", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The standard spin radius of the Exo Twins.
        /// </summary>
        public static float ExoTwinsStandardSpinRadius => Variables.GetAIFloat("ExoFlurry_ExoTwinsStandardSpinRadius", ExoMechAIVariableType.Combo);

        /// <summary>
        /// The maximum spin radius of the Exo Twins.
        /// </summary>
        public static float ExoTwinsMaxSpinRadius => Variables.GetAIFloat("ExoFlurry_ExoTwinsMaxSpinRadius", ExoMechAIVariableType.Combo);

        public override int[] ExpectedManagingExoMechs => [ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<AresBody>(), ModContent.NPCType<Apollo>()];

        public override bool Perform(NPC npc)
        {
            if (npc.type == ExoMechNPCIDs.ArtemisID || npc.type == ExoMechNPCIDs.ApolloID)
            {
                Perform_ExoTwins(npc);
                return false;
            }

            if (npc.type == ExoMechNPCIDs.AresBodyID)
            {
                Perform_Ares(npc);
                return false;
            }

            if (npc.type == ExoMechNPCIDs.HadesHeadID)
            {
                Perform_Hades(npc);
                return false;
            }

            return false;
        }

        /// <summary>
        /// Performs Ares' part in the ExoFlurry attack.
        /// </summary>
        /// <param name="npc">Ares' NPC instance.</param>
        public static void Perform_Ares(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out AresBodyEternity ares))
                return;

            // The attack cycle of the slash is the same as that of Artemis and Apollo, to ensure that they don't drift and cause unavoidable hits.
            float modifier = WorldSavingSystem.MasochistModeReal ? 1f : 1.5f;
            int attackCycleTime = (int)((ExoTwinsSpinTime + ExoTwinsSpinSlowdownTime + ExoTwinsDashTime) * modifier);
            bool armsAreDetaching = AITimer <= AresBodyEternity.DetachHands_DetachmentDelay;
            float animationCompletion = (AITimer - AresBodyEternity.DetachHands_DetachmentDelay) / (float)attackCycleTime % 1f;
            float riseUpwardInterpolant = LumUtils.InverseLerp(0f, 0.55f, animationCompletion);
            float flyDownwardInterpolant = LumUtils.InverseLerp(0.55f, 1f, animationCompletion);
            float lightActivationInterpolant = LumUtils.InverseLerp(0.5f, 0.65f, riseUpwardInterpolant) * LumUtils.InverseLerp(1f, 0.6f, flyDownwardInterpolant);

            npc.damage = 0;

            Color lightColorA = new(255, 120, 129);
            Color lightColorB = new(255, 0, 4);
            ares.ShiftLightColors(lightActivationInterpolant, [lightColorA, lightColorA, lightColorB, lightColorA, lightColorA, lightColorB, lightColorA, lightColorB]);

            if (armsAreDetaching || riseUpwardInterpolant < 0.7f)
            {
                ares.MotionBlurInterpolant = 0f;

                float riseOffset = MathHelper.SmoothStep(250f, 480f, riseUpwardInterpolant.Squared());
                if (armsAreDetaching)
                    riseOffset = 250f;
                if (Target.velocity.Y < 0f)
                    riseOffset -= Target.velocity.Y * 24f;
                float horizontalOffset = Target.velocity.X * 18f;

                Vector2 predictedCenter = Target.Center + new Vector2(horizontalOffset, -riseOffset);
                npc.SmoothFlyNear(predictedCenter, 0.1f, 0.9f);
                npc.velocity.X += npc.HorizontalDirectionTo(predictedCenter) * 1f;
            }
            else if (riseUpwardInterpolant < 1f)
                npc.velocity.Y *= 0.8f;
            else
            {
                if (flyDownwardInterpolant <= 0.03f)
                {
                    SoundEngine.PlaySound(AresBodyEternity.SlashSound with { MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew });
                    ScreenShakeSystem.StartShakeAtPoint(npc.Center, 20f, MathHelper.TwoPi, null, 1.15f);
                }

                npc.damage = AresBodyEternity.KatanaDamage;
                npc.velocity.X *= 0.55f;
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + AresFlyDownAcceleration, -25f, AresMaxFlyDownSpeed);

                // Teleport above the player once the downward fly has concluded.
                if (flyDownwardInterpolant >= 0.9f)
                {
                    Vector2 oldPosition = npc.Center;
                    npc.Center = Target.Center - Vector2.UnitY * 1900f;
                    Vector2 teleportOffset = npc.Center - oldPosition;

                    int handID = ModContent.NPCType<AresHand>();
                    foreach (NPC hand in Main.ActiveNPCs)
                    {
                        if (hand.type == handID)
                        {
                            hand.Center += teleportOffset;
                            hand.velocity = Vector2.Zero;
                            hand.netUpdate = true;
                        }
                    }

                    npc.velocity *= 0.05f;
                    npc.netUpdate = true;
                }
            }

            if (armsAreDetaching)
            {
                for (int i = 0; i < ares.InstructionsForHands.Length; i++)
                {
                    int copyForDelegate = i;
                    ares.InstructionsForHands[i] = new(h => ares.DetachHandsUpdate(h, copyForDelegate));
                }
            }
            else
            {
                ares.MotionBlurInterpolant = LumUtils.InverseLerp(0f, 0.1f, flyDownwardInterpolant);
                ares.InstructionsForHands[0] = new(h => Perform_Ares_HandUpdate(npc, h, riseUpwardInterpolant, flyDownwardInterpolant, new Vector2(-400f, 40f), 0));
                ares.InstructionsForHands[1] = new(h => Perform_Ares_HandUpdate(npc, h, riseUpwardInterpolant, flyDownwardInterpolant, new Vector2(-280f, 224f), 1));
                ares.InstructionsForHands[2] = new(h => Perform_Ares_HandUpdate(npc, h, riseUpwardInterpolant, flyDownwardInterpolant, new Vector2(280f, 224f), 2));
                ares.InstructionsForHands[3] = new(h => Perform_Ares_HandUpdate(npc, h, riseUpwardInterpolant, flyDownwardInterpolant, new Vector2(400f, 40f), 3));
            }

            ares.UseStandardRotation = false;
            npc.rotation = npc.velocity.X * 0.0049f;
        }

        /// <summary>
        /// Handles the updating of Ares' hands in the ExoFlurry attack.
        /// </summary>
        /// <param name="ares">Ares' NPC instance.</param>
        /// <param name="hand">The hand's ModNPC instance.</param>
        /// <param name="hoverOffset">The hover offset of the hand relative to Ares.</param>
        /// <param name="armIndex">The index of the arm.</param>
        public static void Perform_Ares_HandUpdate(NPC ares, AresHand hand, float riseUpwardInterpolant, float flyDownwardInterpolant, Vector2 hoverOffset, int armIndex)
        {
            NPC handNPC = hand.NPC;
            hand.UsesBackArm = armIndex == 0 || armIndex == AresBodyEternity.ArmCount - 1;
            hand.ArmSide = (armIndex >= AresBodyEternity.ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.EnergyKatana;
            hand.Frame = 0;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.EnergyDrawer.chargeProgress = 0f;
            hand.GlowmaskDisabilityInterpolant = 0f;
            hand.KatanaInUse = riseUpwardInterpolant >= 0.4f;
            hand.AttachedToArm = flyDownwardInterpolant < 0.8f;
            if (hand.KatanaAppearanceInterpolant >= 0.1f)
                hand.KatanaAppearanceInterpolant = MathHelper.Lerp(hand.KatanaAppearanceInterpolant, hand.KatanaInUse.ToInt(), 0.15f);

            hoverOffset.X *= MathHelper.Lerp(1f, 0.19f, riseUpwardInterpolant.Cubed()) + LumUtils.Convert01To010(riseUpwardInterpolant.Cubed()).Squared() * 0.8f;
            hoverOffset.Y -= LumUtils.InverseLerpBump(0f, 0.75f, 0.91f, 1f, riseUpwardInterpolant).Cubed() * 410f;
            hoverOffset.Y += LumUtils.InverseLerp(0f, 0.05f, flyDownwardInterpolant) * 200f + flyDownwardInterpolant * 300f;

            float fastMovementInterpolant = LumUtils.InverseLerp(0f, 60f, AITimer - AresBodyEternity.DetachHands_DetachmentDelay);
            float movementSharpness = MathHelper.Lerp(0.2f, 0.9f, fastMovementInterpolant);
            float movementSmoothness = MathHelper.Lerp(0.85f, 0.32f, fastMovementInterpolant);

            Vector2 hoverDestination = ares.Center + hoverOffset.RotatedBy(ares.rotation) * ares.scale;
            handNPC.SmoothFlyNear(hoverDestination, movementSharpness, movementSmoothness);
            if (!handNPC.WithinRange(hoverDestination, 400f))
            {
                handNPC.Center = hoverDestination;
                handNPC.velocity = Vector2.Zero;
                handNPC.netUpdate = true;
            }

            handNPC.Opacity = LumUtils.Saturate(handNPC.Opacity + 0.2f);
            handNPC.spriteDirection = 1;
            handNPC.rotation = handNPC.AngleFrom(ares.Center).AngleLerp(hand.ShoulderToHandDirection, 0.8f).AngleLerp(MathHelper.PiOver2, MathF.Pow(riseUpwardInterpolant, 2.5f));
            handNPC.damage = flyDownwardInterpolant >= 0.01f ? AresBodyEternity.KatanaDamage : 0;

            // Disable incoming damage to nullify ram dash cheese.
            if (handNPC.damage >= 1)
                handNPC.dontTakeDamage = true;
        }

        /// <summary>
        /// Performs Artemis and Apollo's part in the ExoFlurry attack. This method processes behaviors for both mechs.
        /// </summary>
        /// <param name="npc">The Exo Twins' NPC instance.</param>
        public static void Perform_ExoTwins(NPC npc)
        {
            int spinTime = ExoTwinsSpinTime;
            int spinSlowdownAndRepositionTime = ExoTwinsSpinSlowdownTime;
            int dashTime = ExoTwinsDashTime;
            int spinCycleTime = spinTime + spinSlowdownAndRepositionTime + dashTime;
            int spinCycleTimer = AITimer % spinCycleTime;
            bool isApollo = npc.type == ExoMechNPCIDs.ApolloID;
            ExoTwinAnimation animation = ExoTwinAnimation.ChargingUp;

            // Initialize the random spin orientation of the twins at the very beginning of the attack.
            if (AITimer == 1 && isApollo)
            {
                do
                {
                    ExoTwinsStateManager.SharedState.Values[0] = Main.rand.NextFloat(MathHelper.TwoPi);
                }
                while (MathF.Abs(MathF.Sin(ExoTwinsStateManager.SharedState.Values[0] + MathHelper.Pi)) <= MinExoTwinYOrientation);
                npc.netUpdate = true;
            }

            float spinCompletion = Utilities.InverseLerp(0f, spinTime + spinSlowdownAndRepositionTime, spinCycleTimer);
            if (spinCompletion < 1f)
            {
                // Determine the radius of the spin for the two Exo Twins.
                float radiusExtendInterpolant = Utilities.InverseLerp(0f, spinSlowdownAndRepositionTime, spinCycleTimer - spinTime);
                float radiusExtension = MathF.Pow(radiusExtendInterpolant, 3.13f) * ExoTwinsMaxSpinRadius;
                float currentRadius = ExoTwinsStandardSpinRadius + radiusExtension;

                float spinOffsetAngle = EasingCurves.Quintic.Evaluate(EasingType.Out, 0f, MathHelper.Pi * 3f, spinCompletion) + ExoTwinsStateManager.SharedState.Values[0];
                Vector2 hoverOffset = spinOffsetAngle.ToRotationVector2() * currentRadius;

                // Invert the hover offset if Apollo is spinning, such that it is opposite to Artemis.
                if (isApollo)
                    hoverOffset *= -1f;

                Vector2 hoverDestination = Target.Center + hoverOffset;

                // Fly around in accordance with the radius offset.
                npc.SmoothFlyNear(hoverDestination, MathF.Cbrt(spinCompletion) * 0.9f, 0.01f);
                npc.rotation = npc.AngleTo(Target.Center);
            }

            // Perform the dash.
            if (spinCycleTimer == spinTime + spinSlowdownAndRepositionTime)
            {
                SoundEngine.PlaySound(Artemis.ChargeSound, npc.Center);

                ScreenShakeSystem.StartShakeAtPoint(npc.Center, 5.3f);

                npc.velocity = npc.rotation.ToRotationVector2() * ExoTwinsStartingDashSpeed;
                npc.rotation = npc.velocity.ToRotation();
                npc.netUpdate = true;
            }

            IExoTwin? twinInfo = null;
            float motionBlurInterpolant = Utilities.InverseLerp(90f, 150f, npc.velocity.Length());
            float thrusterBoost = Utilities.InverseLerp(ExoTwinsMaxDashSpeed * 0.85f, ExoTwinsMaxDashSpeed, npc.velocity.Length()) * 1.3f;
            if (npc.TryGetDLCBehavior(out ArtemisEternity artemisBehavior))
            {
                artemisBehavior.MotionBlurInterpolant = motionBlurInterpolant;
                artemisBehavior.ThrusterBoost = MathHelper.Max(artemisBehavior.ThrusterBoost, thrusterBoost);
                twinInfo = artemisBehavior;
            }
            else if (npc.TryGetDLCBehavior(out ApolloEternity apolloBehavior))
            {
                apolloBehavior.MotionBlurInterpolant = motionBlurInterpolant;
                apolloBehavior.ThrusterBoost = MathHelper.Max(apolloBehavior.ThrusterBoost, thrusterBoost);
                twinInfo = apolloBehavior;
            }

            // Handle post-dash behaviors.
            if (spinCycleTimer >= spinTime + spinSlowdownAndRepositionTime && spinCycleTimer <= spinTime + spinSlowdownAndRepositionTime + dashTime)
            {
                // Apply hit forces when Artemis and Apollo collide, in accordance with Newton's third law.
                NPC artemis = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed];
                if (isApollo && npc.Hitbox.Intersects(artemis.Hitbox))
                {
                    Vector2 impactForce = npc.velocity.RotatedBy(-MathHelper.PiOver2) * 0.15f;
                    npc.velocity -= impactForce;
                    npc.netUpdate = true;
                    artemis.velocity += impactForce;
                    artemis.netUpdate = true;

                    SoundEngine.PlaySound(CommonCalamitySounds.ExoHitSound, npc.Center);

                    for (int i = 0; i < 35; i++)
                    {
                        npc.HitEffect();
                        artemis.HitEffect();
                    }
                }

                npc.velocity = (npc.velocity + npc.velocity.SafeNormalize(Vector2.UnitY) * 15f).ClampLength(0f, ExoTwinsMaxDashSpeed);
                npc.damage = npc.defDamage;
                npc.rotation = npc.velocity.ToRotation();

                // Disable incoming damage to nullify ram dash cheese.
                npc.dontTakeDamage = true;

                if (twinInfo is not null)
                {
                    float FlameEngulfWidthFunction(float completionRatio)
                    {
                        float baseWidth = MathHelper.Lerp(114f, 50f, completionRatio);
                        float tipSmoothenFactor = MathF.Sqrt(1f - Utilities.InverseLerp(0.3f, 0.015f, completionRatio).Cubed());
                        return npc.scale * baseWidth * tipSmoothenFactor;
                    }

                    Color FlameEngulfColorFunction(float completionRatio)
                    {
                        Color flameTipColor = new(255, 255, 208);
                        Color limeFlameColor = new(173, 255, 36);
                        Color greenFlameColor = new(52, 156, 17);
                        Color trailColor = Utilities.MulticolorLerp(MathF.Pow(completionRatio, 0.75f) * 0.7f, flameTipColor, limeFlameColor, greenFlameColor);
                        return npc.GetAlpha(trailColor) * (1 - completionRatio);
                    }

                    twinInfo.SpecificDrawAction = () =>
                    {
                        Vector2[] flamePositions = new Vector2[8];
                        for (int i = 0; i < flamePositions.Length; i++)
                            flamePositions[i] = npc.Center - npc.oldRot[i].ToRotationVector2() * (i * 90f - 100f);

                        ManagedShader flameShader = ShaderManager.GetShader("FargowiltasCrossmod.FlameEngulfShader");
                        flameShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 1, SamplerState.LinearWrap);
                        flameShader.SetTexture(MiscTexturesRegistry.TurbulentNoise.Value, 2, SamplerState.LinearWrap);

                        PrimitiveSettings flameSettings = new(FlameEngulfWidthFunction, FlameEngulfColorFunction, Shader: flameShader);
                        PrimitiveRenderer.RenderTrail(flamePositions, flameSettings, 60);
                    };
                    twinInfo.MotionBlurInterpolant = 1f;
                    twinInfo.ThrusterBoost = 1.35f;
                }

                animation = ExoTwinAnimation.Attacking;

                if (spinCycleTimer == spinCycleTime - 1 && isApollo)
                {
                    ExoTwinsStateManager.SharedState.Values[0] = npc.AngleTo(Target.Center) - MathHelper.PiOver2;
                    while (MathF.Abs(MathF.Sin(ExoTwinsStateManager.SharedState.Values[0] + MathHelper.Pi)) <= MinExoTwinYOrientation)
                        ExoTwinsStateManager.SharedState.Values[0] += 0.15f;

                    npc.netUpdate = true;
                }
            }

            // Update animations.
            if (twinInfo is not null)
            {
                twinInfo.Animation = animation;
                twinInfo.Frame = twinInfo.Animation.CalculateFrame(AITimer / 40f % 1f, twinInfo.InPhase2);
            }
        }

        /// <summary>
        /// Performs Hades' part in the ExoFlurry attack.
        /// </summary>
        /// <param name="npc">Hades' NPC instance.</param>
        public static void Perform_Hades(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out HadesHeadEternity hades))
            {
                npc.active = false;
                return;
            }

            hades.SegmentReorientationStrength = 0.1f;

            int wrappedTimer = AITimer % HadesHeadEternity.MineBarrages_AttackCycleTime;
            if (wrappedTimer < HadesHeadEternity.MineBarrages_RedirectTime)
            {
                if (!npc.WithinRange(Target.Center, 600f))
                {
                    float newSpeed = MathHelper.Lerp(npc.velocity.Length(), 26f, 0.09f);
                    Vector2 newDirection = npc.velocity.RotateTowards(npc.AngleTo(Target.Center), 0.03f).SafeNormalize(Vector2.UnitY);
                    npc.velocity = newDirection * newSpeed;
                }

                hades.BodyBehaviorAction = new(HadesHeadEternity.EveryNthSegment(4), HadesHeadEternity.OpenSegment());
            }
            else
            {
                if (!npc.WithinRange(Target.Center, 400f))
                    npc.velocity = Vector2.Lerp(npc.velocity, npc.SafeDirectionTo(Target.Center) * 25f, 0.03f);
                else
                    npc.velocity *= 1.07f;

                hades.BodyBehaviorAction = new(HadesHeadEternity.EveryNthSegment(4), hades.DoBehavior_MineBarrages_FireMine);
            }

            // GENIUS tech: Make Hades try to interpolate ahead of the player, cutting them off and eliminating horizontal fly strats.
            npc.Center = Vector2.Lerp(npc.Center, Target.Center + Target.velocity * 50f, 0.00485f);

            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
            hades.SegmentReorientationStrength = 0.1f;
        }
    }
}
