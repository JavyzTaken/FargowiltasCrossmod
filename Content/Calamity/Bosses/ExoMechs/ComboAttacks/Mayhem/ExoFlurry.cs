using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using Luminance.Common.Easings;
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
        /// The diameter of the explosion from Ares' nukes.
        /// </summary>
        public static float AresNukeExplosionDiameter => Variables.GetAIFloat("ExoFlurry_AresNukeExplosionDiameter", ExoMechAIVariableType.Combo);

        /// <summary>
        /// Ares' acceleration when flying towards the player.
        /// </summary>
        public static float AresAcceleration => Variables.GetAIFloat("ExoFlurry_AresAcceleration", ExoMechAIVariableType.Combo);

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

            Vector2 hoverDestination = Target.Center - Vector2.UnitY * 350f;
            Vector2 flyDirection = npc.SafeDirectionTo(hoverDestination);
            if (!npc.WithinRange(hoverDestination, 300f))
                npc.velocity += flyDirection * AresAcceleration;
            if (npc.velocity.AngleBetween(flyDirection) >= 1.4f)
                npc.velocity *= 0.9f;

            ares.InstructionsForHands[0] = new(h => Perform_Ares_GaussNuke(npc, h, new Vector2(-400f, 40f), 0));
            ares.InstructionsForHands[1] = new(h => ares.BasicHandUpdate(h, new Vector2(-280f, 224f), 1));
            ares.InstructionsForHands[2] = new(h => ares.BasicHandUpdate(h, new Vector2(280f, 224f), 2));
            ares.InstructionsForHands[3] = new(h => Perform_Ares_GaussNuke(npc, h, new Vector2(400f, 40f), 3));
        }

        /// <summary>
        /// Handles the updating of Ares' gauss nuke(s) in the ExoFlurry attack.
        /// </summary>
        /// <param name="ares">Ares' NPC instance.</param>
        /// <param name="hand">The hand's ModNPC instance.</param>
        /// <param name="hoverOffset">The hover offset of the hand relative to Ares.</param>
        /// <param name="armIndex">The index of the arm.</param>
        public static void Perform_Ares_GaussNuke(NPC ares, AresHand hand, Vector2 hoverOffset, int armIndex)
        {
            int wrappedTimer = (AITimer + armIndex * NukeShotDelay / 3) % (NukeChargeUpTime + NukeShotDelay);
            NPC handNPC = hand.NPC;
            handNPC.SmoothFlyNear(ares.Center + hoverOffset * ares.scale, 0.3f, 0.8f);
            handNPC.Opacity = Utilities.Saturate(handNPC.Opacity + 0.2f);
            hand.UsesBackArm = armIndex == 0 || armIndex == AresBodyEternity.ArmCount - 1;
            hand.ArmSide = (armIndex >= AresBodyEternity.ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.GaussNuke;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.EnergyDrawer.chargeProgress = Utilities.InverseLerp(0f, NukeChargeUpTime, wrappedTimer);
            if (hand.EnergyDrawer.chargeProgress >= 1f)
                hand.EnergyDrawer.chargeProgress = 0f;
            hand.GlowmaskDisabilityInterpolant = 0f;
            hand.RotateToLookAt(Target.Center);

            if (wrappedTimer % 20 == 19 && hand.EnergyDrawer.chargeProgress >= 0.4f)
            {
                int pulseCounter = (int)MathF.Round(hand.EnergyDrawer.chargeProgress * 5f);
                hand.EnergyDrawer.AddPulse(pulseCounter);
            }

            if (wrappedTimer == 1)
                SoundEngine.PlaySound(AresGaussNuke.TelSound with { Volume = 3f });

            AresBodyEternity.HandleGaussNukeShots(hand, handNPC, wrappedTimer, NukeChargeUpTime, AresNukeExplosionDiameter);
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
                if (twinInfo is not null)
                {
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
            ref float localAITimer = ref npc.ai[0];
            if (AITimer == 1)
            {
                localAITimer = 0f;
                npc.netUpdate = true;
            }

            if (!npc.TryGetDLCBehavior(out HadesHeadEternity hades))
            {
                npc.active = false;
                return;
            }

            hades.SegmentReorientationStrength = 0.1f;

            int returnOnScreenTime = 420;
            int beamWindUpTime = 180;
            int beamShootDelay = 60;
            int beamFireTime = ExoelectricBlast.Lifetime;
            int postBeamFireLeaveTime = 240;
            int attackCycleTime = returnOnScreenTime + beamWindUpTime + beamFireTime + postBeamFireLeaveTime;
            int wrappedAttackTimer = (int)localAITimer % attackCycleTime;
            float jawExtendInterpolant = 0f;

            if (wrappedAttackTimer <= returnOnScreenTime)
            {
                Vector2 idealDirection = npc.SafeDirectionTo(Target.Center);
                npc.velocity = idealDirection * MathHelper.Lerp(npc.velocity.Length(), 130f, 0.07f);
                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;

                if (npc.WithinRange(Target.Center, 500f))
                {
                    localAITimer += returnOnScreenTime - localAITimer + 1f;
                    npc.velocity *= 0.05f;
                    npc.netUpdate = true;
                }
            }

            else if (wrappedAttackTimer <= returnOnScreenTime + beamWindUpTime)
            {
                float angularVelocity = HadesTurnSpeedCoefficient / npc.Distance(Target.Center);
                npc.velocity = npc.velocity.RotateTowards(npc.AngleTo(Target.Center), angularVelocity);
                if (npc.velocity.Length() > 6f)
                    npc.velocity *= 0.9f;

                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;

                float chargeUpCompletion = Utilities.InverseLerp(0f, beamWindUpTime, wrappedAttackTimer - returnOnScreenTime);

                jawExtendInterpolant = MathF.Sqrt(chargeUpCompletion);
                hades.ReticleOpacity = MathF.Pow(Utilities.InverseLerp(0f, 0.25f, chargeUpCompletion), 0.6f);
                hades.SegmentReorientationStrength = 0f;

                int particleCount = (int)MathHelper.Lerp(1f, 3f, chargeUpCompletion);
                Vector2 mouthPosition = npc.Center + npc.velocity.SafeNormalize(Vector2.Zero) * 40f;
                for (int i = 0; i < particleCount; i++)
                {
                    float particleScale = (chargeUpCompletion + 1.1f) * Main.rand.NextFloat(0.6f, 1f);
                    Vector2 energySpawnPosition = mouthPosition + Main.rand.NextVector2Unit() * Main.rand.NextFloat(200f, 250f);
                    BloomPixelParticle energy = new(energySpawnPosition, (mouthPosition - energySpawnPosition).RotatedBy(MathHelper.PiOver4) * 0.05f, Color.Wheat, Color.DeepSkyBlue * 0.4f, 30, Vector2.One * particleScale, mouthPosition);
                    energy.Spawn();
                }

                if (wrappedAttackTimer % 4 == 3)
                {
                    float scale = MathHelper.Lerp(0.5f, 3f, chargeUpCompletion);

                    StrongBloom bloom = new(mouthPosition, Vector2.Zero, Color.DeepSkyBlue, scale, 13);
                    GeneralParticleHandler.SpawnParticle(bloom);

                    bloom = new(mouthPosition, Vector2.Zero, Color.Wheat, scale * 0.45f, 13);
                    GeneralParticleHandler.SpawnParticle(bloom);
                }

                if (wrappedAttackTimer == returnOnScreenTime + 2)
                    SoundEngine.PlaySound(HadesHeadEternity.DeathrayChargeUpSound);

                if (wrappedAttackTimer == returnOnScreenTime + beamWindUpTime)
                {
                    SoundEngine.PlaySound(HadesHeadEternity.DeathrayFireSound);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Utilities.NewProjectileBetter(npc.GetSource_FromAI(), mouthPosition, npc.velocity.SafeNormalize(Vector2.Zero), ModContent.ProjectileType<ExoelectricBlast>(), HadesHeadEternity.ExoEnergyBlastDamage, 0f);
                }
            }

            else if (wrappedAttackTimer <= returnOnScreenTime + beamWindUpTime + beamShootDelay)
            {
                jawExtendInterpolant = 1f;
                hades.ReticleOpacity = Utilities.Saturate(hades.ReticleOpacity - 0.08f);
                hades.SegmentReorientationStrength = 0f;
                if (npc.velocity.Length() >= 3f)
                    npc.velocity *= 0.9485f;
            }

            else if (wrappedAttackTimer <= returnOnScreenTime + beamWindUpTime + beamShootDelay + beamFireTime)
            {
                jawExtendInterpolant = 1f;
                if (npc.velocity.Length() >= 3f)
                    npc.velocity *= 0.9485f;
                npc.velocity = npc.velocity.RotateTowards(npc.AngleTo(Target.Center), 0.0056f);
                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
                hades.SegmentReorientationStrength = 0f;
            }
            else
            {
                npc.velocity = (npc.velocity * 1.02f).ClampLength(0f, 32f);
                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
                hades.SegmentReorientationStrength = 0f;
            }

            hades.BodyBehaviorAction = new(HadesHeadEternity.EveryNthSegment(3), HadesHeadEternity.OpenSegment());
            hades.JawRotation = MathHelper.Lerp(hades.JawRotation, jawExtendInterpolant * 0.93f, 0.15f);
            npc.damage = 0;

            localAITimer++;
        }
    }
}
