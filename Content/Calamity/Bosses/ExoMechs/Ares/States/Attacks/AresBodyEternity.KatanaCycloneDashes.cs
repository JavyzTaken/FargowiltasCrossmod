using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    public sealed partial class AresBodyEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The amount of slash dashes Ares has performed during the Katana Cyclone Dashes attack.
        /// </summary>
        public ref float KatanaCycloneDashes_SlashCounter => ref NPC.ai[0];

        /// <summary>
        /// How long Ares spends redirecting to get near the target during the Katana Cyclone Dashes attack.
        /// </summary>
        public static int KatanaCycloneDashes_RedirectTime => Variables.GetAIInt("KatanaCycloneDashes_RedirectTime", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long Ares spends flying away from the target during the Katana Cyclone Dashes attack.
        /// </summary>
        public static int KatanaCycloneDashes_FlyAwayTime => Variables.GetAIInt("KatanaCycloneDashes_FlyAwayTime", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How many slash dashes should be performed during the Katana Cyclone Dashes attack.
        /// </summary>
        public static int KatanaCycloneDashes_SlashCount => Variables.GetAIInt("KatanaCycloneDashes_SlashCount", ExoMechAIVariableType.Ares);

        /// <summary>
        /// Ares' starting speed during the Katana Cyclone Dashes attack.
        /// </summary>
        public static float KatanaCycloneDashes_StartingDashSpeed => Variables.GetAIFloat("KatanaCycloneDashes_StartingDashSpeed", ExoMechAIVariableType.Ares);

        /// <summary>
        /// Ares' acceleration during the Katana Cyclone Dashes attack.
        /// </summary>
        public static float KatanaCycloneDashes_AccelerationFactor => Variables.GetAIFloat("KatanaCycloneDashes_AccelerationFactor", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How far Ares teleports away from the player when performing a dash during the Katana Cyclone Dashes attack.
        /// </summary>
        public static float KatanaCycloneDashes_TeleportOffset => Variables.GetAIFloat("KatanaCycloneDashes_TeleportOffset", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How far away Ares must be from the player, at minimum, to teleport to the opposite side and start a new dash during the Katana Cyclone Dashes attack.
        /// </summary>
        public static float KatanaCycloneDashes_MinimumRestartDistance => Variables.GetAIFloat("KatanaCycloneDashes_MinimumRestartDistance", ExoMechAIVariableType.Ares);

        /// <summary>
        /// Ares' maximum possible rotational velocity as a result of the player's relative Y position during the Katana Cyclone Dashes attack.
        /// </summary>
        public static float KatanaCycloneDashes_MaxRotationalVelocity => MathHelper.ToRadians(Variables.GetAIFloat("KatanaCycloneDashes_MaxRotationalVelocityDegrees", ExoMechAIVariableType.Ares));

        /// <summary>
        /// The sound played when Ares performs a slash.
        /// </summary>
        public static readonly SoundStyle SlashSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/Slash") with { Volume = 1.2f, MaxInstances = 0 };

        /// <summary>
        /// The sound played when Ares unsheathes his katanas.
        /// </summary>
        public static readonly SoundStyle KatanaUnsheatheSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/EnergyKatanaUnsheathe") with { Volume = 1.2f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };

        /// <summary>
        /// AI update loop method for the Katana Cyclone Dashes attack.
        /// </summary>
        public void DoBehavior_KatanaCycloneDashes()
        {
            AnimationState = AresFrameAnimationState.Laugh;

            float lightActivationInterpolant = LumUtils.InverseLerp(0f, KatanaCycloneDashes_RedirectTime * 0.8f, AITimer);
            Color lightColorA = new(255, 120, 129);
            Color lightColorB = new(255, 0, 4);
            ShiftLightColors(lightActivationInterpolant, [lightColorA, lightColorA, lightColorB, lightColorA, lightColorA, lightColorB, lightColorA, lightColorB]);

            bool drawBlurSlash = false;

            if (AITimer <= KatanaCycloneDashes_RedirectTime)
            {
                NPC.Opacity = 1f;

                float redirectSpeed = MathHelper.Lerp(0.05f, 0.2f, LumUtils.Convert01To010(LumUtils.InverseLerp(0f, 30f, AITimer).Squared()));
                redirectSpeed *= LumUtils.InverseLerp(KatanaCycloneDashes_RedirectTime, KatanaCycloneDashes_RedirectTime - 45f, AITimer);

                Vector2 hoverDestination = Target.Center + new Vector2(NPC.HorizontalDirectionTo(Target.Center) * -400f, -150f);
                NPC.SmoothFlyNearWithSlowdownRadius(hoverDestination, redirectSpeed, 1f - redirectSpeed, 50f);
            }
            else if (AITimer <= KatanaCycloneDashes_RedirectTime + KatanaCycloneDashes_FlyAwayTime)
            {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.velocity.X.NonZeroSign() * 93f, 0.099f);
                NPC.velocity.Y *= 1.018f;

                if (AITimer == KatanaCycloneDashes_RedirectTime + KatanaCycloneDashes_FlyAwayTime)
                {
                    ScreenShakeSystem.StartShake(10f);
                    SoundEngine.PlaySound(LaughSound with { Volume = 10f });
                }
            }
            else
            {
                // Teleport to the other side of the player.
                if (Main.netMode != NetmodeID.MultiplayerClient && AITimer == KatanaCycloneDashes_RedirectTime + KatanaCycloneDashes_FlyAwayTime + 1)
                {
                    if (LumUtils.CountProjectiles(ModContent.ProjectileType<AresSwingingKatanas>()) <= 0)
                        LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AresSwingingKatanas>(), KatanaDamage, 0f);

                    Vector2 teleportOffset = (NPC.SafeDirectionTo(Target.Center) * new Vector2(1f, 0.5f)).SafeNormalize(Vector2.UnitX);
                    if (KatanaCycloneDashes_SlashCounter <= 0f)
                        teleportOffset *= -1f;

                    NPC.Center = Target.Center + teleportOffset * KatanaCycloneDashes_TeleportOffset;
                    NPC.velocity = NPC.SafeDirectionTo(Target.Center) * KatanaCycloneDashes_StartingDashSpeed;

                    LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), NPC.Center - NPC.velocity * 2f, Vector2.Zero, ModContent.ProjectileType<AresHyperfuturisticPortal>(), 0, 0f);

                    KatanaCycloneDashes_SlashCounter++;

                    // Teleport above the player and reset Ares' velocity to zero for consistency once he's done with the attack.
                    if (KatanaCycloneDashes_SlashCounter > KatanaCycloneDashes_SlashCount)
                    {
                        IProjOwnedByBoss<AresBody>.KillAll();
                        NPC.Center = Target.Center - Vector2.UnitY * 800f;
                        NPC.velocity = Vector2.Zero;
                        SelectNewState();
                    }

                    NPC.netUpdate = true;
                }

                if (Main.rand.NextBool(10))
                    CustomExoMechsSky.CreateLightning((NPC.Center - Main.screenPosition) / Main.ScreenSize.ToVector2());

                if (AITimer % 11f == 10f)
                    SoundEngine.PlaySound(SlashSound, NPC.Center);

                NPC.velocity *= KatanaCycloneDashes_AccelerationFactor;
                NPC.Opacity = LumUtils.InverseLerp(0f, 7f, AITimer - KatanaCycloneDashes_RedirectTime - KatanaCycloneDashes_FlyAwayTime);

                float rotateToPlayerInterpolant = LumUtils.InverseLerp(12f, 22f, MathF.Abs(Target.velocity.Y));
                NPC.velocity = NPC.velocity.RotateTowards(NPC.AngleTo(Target.Center), rotateToPlayerInterpolant * KatanaCycloneDashes_MaxRotationalVelocity);

                if (AITimer >= KatanaCycloneDashes_RedirectTime + KatanaCycloneDashes_FlyAwayTime + 24 && !NPC.WithinRange(Target.Center, KatanaCycloneDashes_MinimumRestartDistance))
                {
                    AITimer = KatanaCycloneDashes_RedirectTime + KatanaCycloneDashes_FlyAwayTime;
                    NPC.netUpdate = true;
                }

                drawBlurSlash = true;
                ScreenShakeSystem.StartShakeAtPoint(NPC.Center, 6f, MathHelper.PiOver2, NPC.velocity.SafeNormalize(Vector2.Zero), 0.3f, 700f, 950f);
            }

            NPC.rotation *= 0.45f;

            InstructionsForHands[0] = new(h => KatanaSwingHandUpdate(h, new Vector2(-400f, 40f), KatanaSlashes_AttackDelay, KatanaSlashes_AttackCycleTime, 0, !drawBlurSlash));
            InstructionsForHands[1] = new(h => KatanaSwingHandUpdate(h, new Vector2(-280f, 224f), KatanaSlashes_AttackDelay, KatanaSlashes_AttackCycleTime, 1, !drawBlurSlash));
            InstructionsForHands[2] = new(h => KatanaSwingHandUpdate(h, new Vector2(280f, 224f), KatanaSlashes_AttackDelay, KatanaSlashes_AttackCycleTime, 2, !drawBlurSlash));
            InstructionsForHands[3] = new(h => KatanaSwingHandUpdate(h, new Vector2(400f, 40f), KatanaSlashes_AttackDelay, KatanaSlashes_AttackCycleTime, 3, !drawBlurSlash));
        }

        /// <summary>
        /// Updates one of Ares' hands for the Katana Slashes attack.
        /// </summary>
        /// <param name="hand">The hand's ModNPC instance.</param>
        /// <param name="hoverOffset">The hover offset of the hand.</param>
        /// <param name="attackDelay">How long the hand should wait before attacking.</param>
        /// <param name="attackCycleTime">The attack cycle time for the slash.</param>
        /// <param name="armIndex">The index of the hand.</param>
        /// <param name="canRender">Whether the hand can be rendered or not.</param>
        public void KatanaSwingHandUpdate(AresHand hand, Vector2 hoverOffset, int attackDelay, int attackCycleTime, int armIndex, bool canRender)
        {
            NPC handNPC = hand.NPC;
            Vector2 hoverDestination = NPC.Center + hoverOffset * NPC.scale;

            hand.KatanaInUse = true;
            hand.UsesBackArm = armIndex == 0 || armIndex == ArmCount - 1;
            hand.ArmSide = (armIndex >= ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.EnergyKatana;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.EnergyDrawer.chargeProgress = Utilities.InverseLerp(0f, 30f, AITimer);
            hand.GlowmaskDisabilityInterpolant = 0f;
            hand.Frame = 0;
            hand.CanRender = canRender;
            handNPC.damage = 0;
            handNPC.spriteDirection = 1;
            handNPC.Opacity = Utilities.Saturate(handNPC.Opacity + 0.3f);
            handNPC.SmoothFlyNear(hoverDestination, 0.5f, 0.5f);
            handNPC.rotation = handNPC.AngleFrom(NPC.Center).AngleLerp(hand.ShoulderToHandDirection, 0.3f);

            if (canRender)
                KatanaSlashesHandUpdate_CreateParticles(hand, handNPC);
        }
    }
}
