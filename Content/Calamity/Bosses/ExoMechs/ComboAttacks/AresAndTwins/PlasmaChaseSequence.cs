using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
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
    public class PlasmaChaseSequence : ExoMechComboHandler
    {
        /// <summary>
        /// How long Ares spends charging up energy for his plasma cannons.
        /// </summary>
        public static int PlasmaCannonChargeUpTime => LumUtils.SecondsToFrames(2.62f);

        /// <summary>
        /// How much damage plasma flame jets from Ares' plasma cannons do.
        /// </summary>
        public static int PlasmaJetDamage => Main.expertMode ? 420 : 300;

        /// <summary>
        /// How long Exo Twins spend idly hovering during the dash sequence.
        /// </summary>
        public static int ExoTwinIdleHoverTime => LumUtils.SecondsToFrames(0.75f);

        /// <summary>
        /// How long Exo Twins spend reeling back during the dash sequence.
        /// </summary>
        public static int ExoTwinReelBackTime => LumUtils.SecondsToFrames(0.72f);

        /// <summary>
        /// How long Exo Twins spend dashing during the dash sequence.
        /// </summary>
        public static int ExoTwinDashSpinTime => LumUtils.SecondsToFrames(1.05f);

        /// <summary>
        /// How long the attack goes on for after the charge up.
        /// </summary>
        public static int AttackDuration => LumUtils.SecondsToFrames(9.5f);

        /// <summary>
        /// The speed at which Ares chases his target.
        /// </summary>
        public static float AresChaseSpeedInterpolant => 0.0284f;

        /// <summary>
        /// The speed at which the Exo Twins dash at their target.
        /// </summary>
        public static float ExoTwinDashSpeed => 90f;

        /// <summary>
        /// The sound the Exo Twins make when warping through their portals.
        /// </summary>
        public static readonly SoundStyle PortalWarpSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/ExoTwins/PortalWarp", 2);

        public override int[] ExpectedManagingExoMechs => [ModContent.NPCType<AresBody>(), ModContent.NPCType<Apollo>()];

        public override bool Perform(NPC npc)
        {
            if (npc.type == ExoMechNPCIDs.AresBodyID)
                Perform_Ares(npc);
            if (npc.type == ExoMechNPCIDs.ArtemisID)
                Perform_Artemis(npc);
            if (npc.type == ExoMechNPCIDs.ApolloID)
                Perform_Apollo(npc);

            return AITimer >= PlasmaCannonChargeUpTime + AttackDuration;
        }

        /// <summary>
        /// Performs Ares' part in the PlasmaChaseSequence attack.
        /// </summary>
        /// <param name="npc">Ares' NPC instance.</param>
        public static void Perform_Ares(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out AresBodyEternity ares))
            {
                npc.active = false;
                return;
            }

            if (AITimer == Math.Max(1, PlasmaCannonChargeUpTime - LumUtils.SecondsToFrames(2.6f)))
                SoundEngine.PlaySound(AresPlasmaFlamethrower.TelSound);

            if (AITimer <= PlasmaCannonChargeUpTime)
            {
                Vector2 hoverOffset = new(npc.HorizontalDirectionTo(Target.Center) * -350f, -300f);
                npc.Center = Vector2.Lerp(npc.Center, Target.Center + hoverOffset, 0.016f);
                npc.velocity *= 0.95f;
                ares.AnimationState = AresBodyEternity.AresFrameAnimationState.Default;
            }
            else
            {
                float moveSpeedInterpolant = LumUtils.InverseLerp(0f, 45f, AITimer - PlasmaCannonChargeUpTime);
                Vector2 hoverDestination = Target.Center + new Vector2(npc.OnRightSideOf(Target).ToDirectionInt() * 150f, -450f);
                npc.SmoothFlyNear(hoverDestination, moveSpeedInterpolant * AresChaseSpeedInterpolant, 0.92f);

                float moveAbovePlayerInterpolant = LumUtils.InverseLerp(400f, 150f, Target.Center.Y - npc.Center.Y) + 0.2f;
                npc.Center = Vector2.Lerp(npc.Center, new(npc.Center.X, hoverDestination.Y), moveAbovePlayerInterpolant * 0.2f);
                ares.AnimationState = AresBodyEternity.AresFrameAnimationState.Laugh;
            }

            if (AITimer == PlasmaCannonChargeUpTime)
            {
                SoundEngine.PlaySound(AresBodyEternity.LaughSound);
                SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaShootSound);
                SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaExplosionSound);
                ScreenShakeSystem.StartShake(10f);

                npc.velocity.Y -= 54f;
                npc.netUpdate = true;
            }

            npc.rotation = npc.velocity.X * 0.015f;

            float chargeUpInterpolant = LumUtils.InverseLerp(0f, PlasmaCannonChargeUpTime, AITimer);
            float handRepositionInterpolant = chargeUpInterpolant.Squared();
            Vector2 farLeftHandOffset = Vector2.SmoothStep(new Vector2(-448f, 90f), new Vector2(-400f, 50f), handRepositionInterpolant);
            Vector2 leftHandOffset = Vector2.SmoothStep(new Vector2(-250f, 220f), new Vector2(-140f, 274f), handRepositionInterpolant);
            Vector2 rightHandOffset = Vector2.SmoothStep(new Vector2(250f, 220f), new Vector2(140f, 274f), handRepositionInterpolant);
            Vector2 farRightHandOffset = Vector2.SmoothStep(new Vector2(448f, 90f), new Vector2(400f, 50f), handRepositionInterpolant);

            ares.InstructionsForHands[0] = new(h => AresHandUpdate(npc, h, farLeftHandOffset, 0));
            ares.InstructionsForHands[1] = new(h => AresHandUpdate(npc, h, leftHandOffset, 1));
            ares.InstructionsForHands[2] = new(h => AresHandUpdate(npc, h, rightHandOffset, 2));
            ares.InstructionsForHands[3] = new(h => AresHandUpdate(npc, h, farRightHandOffset, 3));
        }

        /// <summary>
        /// Handles updating for Ares' hands during the PlasmaChaseSequence attack.
        /// </summary>
        /// <param name="aresBody">Ares' NPC instance.</param>
        /// <param name="hand">The hand ModNPC instance.</param>
        /// <param name="hoverOffset">The offset for the hand relative to the body's position.</param>
        /// <param name="armIndex">The arm's index in the overall set.</param>
        public static void AresHandUpdate(NPC aresBody, AresHand hand, Vector2 hoverOffset, int armIndex)
        {
            NPC handNPC = hand.NPC;
            Vector2 hoverDestination = aresBody.Center + hoverOffset * aresBody.scale;

            hand.KatanaInUse = false;
            hand.UsesBackArm = armIndex == 0 || armIndex == AresBodyEternity.ArmCount - 1;
            hand.ArmSide = (armIndex >= AresBodyEternity.ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.PlasmaCannon;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.GlowmaskDisabilityInterpolant = 0f;
            hand.Frame = AITimer / 4 % 12;
            handNPC.spriteDirection = -hand.ArmSide;
            handNPC.Opacity = LumUtils.Saturate(handNPC.Opacity + 0.3f);

            hand.EnergyDrawer.chargeProgress = LumUtils.InverseLerp(0f, PlasmaCannonChargeUpTime * 0.85f, AITimer);

            float idealRotation = handNPC.AngleTo(aresBody.Center + Vector2.UnitY * 400f).AngleLerp(MathHelper.PiOver2, hand.EnergyDrawer.chargeProgress);
            if (handNPC.spriteDirection == -1)
                idealRotation += MathHelper.Pi;

            handNPC.rotation = handNPC.rotation.AngleLerp(idealRotation, 0.1f);

            if (AITimer >= PlasmaCannonChargeUpTime)
            {
                hand.Frame += 24;
                hand.EnergyDrawer.chargeProgress = 0f;
            }

            handNPC.SmoothFlyNear(hoverDestination, 0.35f, 0.65f);

            Vector2 cannonEnd = handNPC.Center + new Vector2(handNPC.spriteDirection * 54f, 8f).RotatedBy(handNPC.rotation);
            if (Main.netMode != NetmodeID.MultiplayerClient && AITimer == PlasmaCannonChargeUpTime)
                LumUtils.NewProjectileBetter(handNPC.GetSource_FromAI(), cannonEnd, Vector2.UnitY, ModContent.ProjectileType<PlasmaFlameJet>(), PlasmaJetDamage, 0f, -1, handNPC.whoAmI);
        }

        /// <summary>
        /// Performs Artemis' part in the PlasmaChaseSequence attack.
        /// </summary>
        /// <param name="npc">Artemis' NPC instance.</param>
        public static void Perform_Artemis(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out ArtemisEternity artemis))
            {
                npc.active = false;
                return;
            }

            PerformDashSequence_ExoTwin(npc, artemis, 0);
            artemis.Frame = artemis.Animation.CalculateFrame(AITimer / 36f % 1f, artemis.InPhase2);
        }

        /// <summary>
        /// Performs Apollo's part in the PlasmaChaseSequence attack.
        /// </summary>
        /// <param name="npc">Apollo's NPC instance.</param>
        public static void Perform_Apollo(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out ApolloEternity apollo))
            {
                npc.active = false;
                return;
            }

            PerformDashSequence_ExoTwin(npc, apollo, (ExoTwinIdleHoverTime + ExoTwinReelBackTime + ExoTwinDashSpinTime) / 2);
        }

        /// <summary>
        /// Handles default hover movement for one of the Exo Twins.
        /// </summary>
        /// <param name="npc">The Exo Twins' NPC instance.</param>
        /// <param name="verticalHoverDirection">The vertical hover direction.</param>
        /// <param name="reelBackDistance">How far back the Exo Twin should hover, relative to the base offset.</param>
        /// <param name="hoverSpeedInterpolant">How fast the Exo Twin should be when hovering.</param>
        public static void HoverNearTarget_ExoTwin(NPC npc, int verticalHoverDirection, float reelBackDistance, float hoverSpeedInterpolant)
        {
            if (!Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechPrime))
                return;

            NPC ares = Main.npc[CalamityGlobalNPC.draedonExoMechPrime];
            Vector2 hoverDestination = Target.Center + new Vector2(ares.OnRightSideOf(Target).ToDirectionInt() * -(reelBackDistance + 700f), verticalHoverDirection * 120f);

            npc.SmoothFlyNear(hoverDestination, hoverSpeedInterpolant * 0.07f, 1f - hoverSpeedInterpolant * 0.112f);
            if (hoverSpeedInterpolant > 1f)
                npc.Center = Vector2.Lerp(npc.Center, hoverDestination, (hoverSpeedInterpolant - 1f) * 0.03f);
        }

        /// <summary>
        /// Handles the dash sequence for the Exo Twins.
        /// </summary>
        /// <param name="npc">The Exo Twins' NPC instance.</param>
        /// <param name="exoTwin">The Exo Twin's interface.</param>
        /// <param name="timeOffset">The time offset of the sequence for the given Exo Twin.</param>
        public static void PerformDashSequence_ExoTwin(NPC npc, IExoTwin exoTwin, int timeOffset)
        {
            if (!Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechPrime))
                return;

            exoTwin.Animation = ExoTwinAnimation.Idle;

            int wrappedTimer = (AITimer - PlasmaCannonChargeUpTime + timeOffset) % (ExoTwinIdleHoverTime + ExoTwinReelBackTime + ExoTwinDashSpinTime);
            int verticalHoverDirection = (npc.type == ExoMechNPCIDs.ArtemisID).ToDirectionInt();

            if (AITimer <= PlasmaCannonChargeUpTime)
                wrappedTimer = ExoTwinIdleHoverTime + ExoTwinReelBackTime / 2;

            // Hover near the target and reel back.
            if (wrappedTimer < ExoTwinIdleHoverTime + ExoTwinReelBackTime)
            {
                if (wrappedTimer == ExoTwinIdleHoverTime + (int)(ExoTwinReelBackTime * 0.375f))
                    SoundEngine.PlaySound(Artemis.ChargeTelegraphSound, npc.Center);

                float hoverSpeedInterpolant = MathHelper.Lerp(0.4f, 1f, LumUtils.InverseLerp(0f, ExoTwinIdleHoverTime + ExoTwinReelBackTime, wrappedTimer).Squared());
                float reelBackDistanceInterpolant = LumUtils.InverseLerp(0f, ExoTwinReelBackTime, wrappedTimer - ExoTwinIdleHoverTime).Cubed();
                float reelBackDistance = reelBackDistanceInterpolant * 920f;
                HoverNearTarget_ExoTwin(npc, verticalHoverDirection, reelBackDistance, hoverSpeedInterpolant + reelBackDistanceInterpolant * 1.3f);

                if (reelBackDistanceInterpolant >= 0.05f)
                    exoTwin.Animation = ExoTwinAnimation.ChargingUp;
                if (reelBackDistanceInterpolant <= 0.85f)
                    npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.2f);
            }

            // Dash.
            else if (wrappedTimer == ExoTwinIdleHoverTime + ExoTwinReelBackTime && AITimer >= PlasmaCannonChargeUpTime + 60)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.velocity = npc.rotation.ToRotationVector2() * ExoTwinDashSpeed;
                    npc.netUpdate = true;

                    Vector2 portalDirection = -npc.velocity.SafeNormalize(Vector2.Zero);
                    LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), npc.Center + npc.velocity * 12f, portalDirection, ModContent.ProjectileType<ExoTwinHyperfuturisticPortal>(), 0, 0f);
                }

                SoundEngine.PlaySound(Artemis.ChargeSound with { Volume = 0.74f }, npc.Center);
            }

            // Spin after the dash.
            else
            {
                if (wrappedTimer >= ExoTwinIdleHoverTime + ExoTwinReelBackTime + 11)
                {
                    npc.velocity.Y -= 2.2f;
                    npc.velocity.X *= 0.875f;
                }
                else
                    npc.velocity *= 1.02f;

                npc.Opacity = LumUtils.InverseLerp(12f, 10f, wrappedTimer - ExoTwinIdleHoverTime - ExoTwinReelBackTime);
                if (npc.Opacity >= 0.5f)
                    npc.damage = npc.defDamage;
                else
                    npc.dontTakeDamage = true;

                if (wrappedTimer == ExoTwinIdleHoverTime + ExoTwinReelBackTime + 2)
                    SoundEngine.PlaySound(PortalWarpSound with { Volume = 2f }).WithVolumeBoost(1.84f);

                if (wrappedTimer == ExoTwinIdleHoverTime + ExoTwinReelBackTime + 10)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 14; i++)
                        {
                            Vector2 arcSpawnPosition = npc.Center + Main.rand.NextVector2Circular(100f, 100f);
                            Vector2 arcDestination = arcSpawnPosition - npc.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(MathHelper.Pi / 3f) * Main.rand.NextFloat(84f, 900f);
                            Vector2 arcLength = (arcDestination - arcSpawnPosition).RotatedByRandom(0.12f) * Main.rand.NextFloat(0.9f, 1f);
                            LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), arcSpawnPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(10, 20));
                        }
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        Color streakColor = Color.Lerp(Color.Green, Color.DeepSkyBlue, Main.rand.NextFloat(0.75f));
                        streakColor = Color.Lerp(streakColor, Color.White, 0.5f);

                        Vector2 streakVelocity = -npc.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.1f) * Main.rand.NextFloat(17f, 47f);
                        Vector2 streakScale = new(2f, 0.02f);
                        Vector2 endingStreakScale = new(0.45f, 0.065f);
                        Vector2 streakSpawnPosition = npc.Center + Main.rand.NextVector2Circular(192f, 192f);
                        LineStreakParticle streak = new(streakSpawnPosition, streakVelocity, streakColor, Main.rand.Next(12, 28), streakVelocity.ToRotation(), streakScale, endingStreakScale);
                        streak.Spawn();
                    }
                    CustomExoMechsSky.CreateLightning((npc.Center - Main.screenPosition) / Main.ScreenSize.ToVector2());
                }

                // Loop around to the other side of the player.
                if (wrappedTimer >= ExoTwinIdleHoverTime + ExoTwinReelBackTime + ExoTwinDashSpinTime - 2)
                {
                    NPC ares = Main.npc[CalamityGlobalNPC.draedonExoMechPrime];
                    npc.Center = Target.Center + new Vector2(ares.OnRightSideOf(Target).ToDirectionInt() * -1650f, -400f);
                    npc.velocity *= 0.1f;
                    npc.oldPos = new Vector2[npc.oldPos.Length];
                }

                npc.rotation = npc.velocity.ToRotation();
                exoTwin.Animation = ExoTwinAnimation.Attacking;
                exoTwin.MotionBlurInterpolant = 1f;
                exoTwin.ThrusterBoost = 1f;
            }

            exoTwin.Frame = exoTwin.Animation.CalculateFrame(AITimer / 36f % 1f, exoTwin.InPhase2);
        }
    }
}
