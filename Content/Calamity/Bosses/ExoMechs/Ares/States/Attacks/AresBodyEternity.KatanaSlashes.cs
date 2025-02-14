using CalamityMod.Items.Weapons.Melee;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Easings;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    public sealed partial class AresBodyEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// How long Ares waits before slashing during his Katana Slashes attack.
        /// </summary>
        public static int KatanaSlashes_AttackDelay => Variables.GetAIInt("KatanaSlashes_AttackDelay", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long a single swipe cycle lasts during Ares' Katana Slashes attack.
        /// </summary>
        public static int KatanaSlashes_AttackCycleTime => Variables.GetAIInt("KatanaSlashes_AttackCycleTime", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How much damage katanas from Ares' core do.
        /// </summary>
        public static int KatanaDamage => Variables.GetAIInt("KatanaDamage", ExoMechAIVariableType.Ares);

        /// <summary>
        /// The animation easing curve used when Ares' back arms are slashing.
        /// </summary>
        public static readonly PiecewiseCurve SlashAnimationCurve_BackArm = new PiecewiseCurve().
            Add(EasingCurves.Quadratic, EasingType.InOut, -0.91f, AnticipationCurveEnd).
            Add(EasingCurves.MakePoly(20f), EasingType.Out, 1.68f, SlashCurveEnd).
            Add(EasingCurves.Quintic, EasingType.InOut, 0f, 1f);

        /// <summary>
        /// The animation easing curve used when Ares' front arms are slashing.
        /// </summary>
        public static readonly PiecewiseCurve SlashAnimationCurve_FrontArm = new PiecewiseCurve().
            Add(EasingCurves.Quadratic, EasingType.InOut, -1.21f, AnticipationCurveEnd).
            Add(EasingCurves.MakePoly(20f), EasingType.Out, 1.1f, SlashCurveEnd).
            Add(EasingCurves.Quintic, EasingType.InOut, 0f, 1f);

        /// <summary>
        /// The 0-1 interpolant value at which the anticipation ends for Ares' slash animation.
        /// </summary>
        public const float AnticipationCurveEnd = 0.45f;

        /// <summary>
        /// The 0-1 interpolant value at which the slash ends for Ares' slash animation.
        /// </summary>
        public const float SlashCurveEnd = 0.59f;

        /// <summary>
        /// Calculates the hover destination for one of Ares' hands. Meant to be used for energy katanas.
        /// </summary>
        /// <param name="hand">The hand's ModNPC instance.</param>
        /// <param name="slashAnimationCompletion">How far along the slash animation currently is, as a 0-1 interpolant.</param>
        /// <param name="defaultHoverOffset">The default hover offset of the hand.</param>
        public Vector2 CalculateSlashHandDestination(AresHand hand, float slashAnimationCompletion, Vector2 defaultHoverOffset)
        {
            PiecewiseCurve animationCurve = hand.UsesBackArm ? SlashAnimationCurve_BackArm : SlashAnimationCurve_FrontArm;

            // This serves two main functions. It makes slashes more densely compacted as the attack goes on, as well as
            // giving a jerky rebound effect when the cycle goes from the end to the start again, serving as an indirect animation state
            // before Ares slashes again.
            Vector2 hoverOffsetSquishFactor = new(MathHelper.Lerp(hand.UsesBackArm ? 1.4f : 1.6f, 0.46f, slashAnimationCompletion), 0.8f);
            float handOffsetAngle = animationCurve.Evaluate(slashAnimationCompletion) * hand.ArmSide;
            Vector2 hoverOffset = defaultHoverOffset.RotatedBy(handOffsetAngle) * hoverOffsetSquishFactor * NPC.scale;
            return NPC.Center + hoverOffset;
        }

        /// <summary>
        /// AI update loop method for the KatanaSlashes attack.
        /// </summary>
        public void DoBehavior_KatanaSlashes()
        {
            if (AITimer == 1)
            {
                ScreenShakeSystem.StartShake(10f);
                SoundEngine.PlaySound(LaughSound with { Volume = 10f });
            }

            AnimationState = AresFrameAnimationState.Laugh;

            NPC.SmoothFlyNear(Target.Center - Vector2.UnitY * 305f, 0.07f, 0.95f);

            if (AITimer >= 600)
                SelectNewState();

            InstructionsForHands[0] = new(h => KatanaSlashesHandUpdate(h, new Vector2(-400f, 40f), KatanaSlashes_AttackDelay, KatanaSlashes_AttackCycleTime, 0, true));
            InstructionsForHands[1] = new(h => KatanaSlashesHandUpdate(h, new Vector2(-280f, 224f), KatanaSlashes_AttackDelay, KatanaSlashes_AttackCycleTime, 1, true));
            InstructionsForHands[2] = new(h => KatanaSlashesHandUpdate(h, new Vector2(280f, 224f), KatanaSlashes_AttackDelay, KatanaSlashes_AttackCycleTime, 2, true));
            InstructionsForHands[3] = new(h => KatanaSlashesHandUpdate(h, new Vector2(400f, 40f), KatanaSlashes_AttackDelay, KatanaSlashes_AttackCycleTime, 3, true));
        }

        /// <summary>
        /// Updates one of Ares' hands for the Katana Slashes attack.
        /// </summary>
        /// <param name="hand">The hand's ModNPC instance.</param>
        /// <param name="hoverOffset">The hover offset of the hand.</param>
        /// <param name="attackDelay">How long the hand should wait before attacking.</param>
        /// <param name="attackCycleTime">The attack cycle time for the slash.</param>
        /// <param name="armIndex">The index of the hand.</param>
        /// <param name="katanasDoDamage">Whether the katanas do damage or not.</param>
        public void KatanaSlashesHandUpdate(AresHand hand, Vector2 hoverOffset, int attackDelay, int attackCycleTime, int armIndex, bool katanasDoDamage)
        {
            NPC handNPC = hand.NPC;

            hand.KatanaInUse = true;
            hand.UsesBackArm = armIndex == 0 || armIndex == ArmCount - 1;
            hand.ArmSide = (armIndex >= ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.EnergyKatana;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.EnergyDrawer.chargeProgress = Utilities.InverseLerp(0f, 30f, AITimer);
            hand.GlowmaskDisabilityInterpolant = 0f;
            hand.Frame = 0;
            handNPC.damage = katanasDoDamage ? KatanaDamage : 0;
            handNPC.spriteDirection = 1;
            handNPC.Opacity = Utilities.Saturate(handNPC.Opacity + 0.3f);

            int animationTimer = (int)(AITimer + handNPC.whoAmI * attackCycleTime / (float)ArmCount - attackDelay) % attackCycleTime;
            KatanaSlashesHandUpdate_HandleSlashMotion(hand, handNPC, hoverOffset, attackDelay, animationTimer, attackCycleTime);
            KatanaSlashesHandUpdate_CreateParticles(hand, handNPC);
        }

        /// <summary>
        /// Handles the motion of one of Ares' katanas during the Katana Slashes attack.
        /// </summary>
        /// <param name="hand">The hand's ModNPC instance.</param>
        /// <param name="handNPC">The hand's NPC instance.</param>
        /// <param name="hoverOffset">The hover offset of the hand.</param>
        /// <param name="attackDelay">How long it takes for Ares to begin slashing.</param>
        /// <param name="animationTimer">The timer for the overall slash animation.</param>
        /// <param name="attackCycleTime">The attack cycle time for the slash.</param>
        public void KatanaSlashesHandUpdate_HandleSlashMotion(AresHand hand, NPC handNPC, Vector2 hoverOffset, int attackDelay, int animationTimer, int attackCycleTime)
        {
            float animationCompletion = animationTimer / (float)attackCycleTime;
            Vector2 hoverDestination = NPC.Center + hoverOffset * NPC.scale;

            // Bear in mind that the motion resulting from the easing curve in this function is not followed exactly, it's only closely
            // followed via the SmoothFlyNear function below. This gives the motion a slightly jerky, mechanical feel to it, which is well in
            // line with Ares.
            float rotateForwardInterpolant = Utilities.InverseLerpBump(0.1f, AnticipationCurveEnd * 1.1f, 0.9f, 1f, animationCompletion).Squared();
            if (AITimer >= attackDelay)
            {
                if (animationTimer == (int)(attackCycleTime * AnticipationCurveEnd) + 3)
                    KatanaSlashesHandUpdate_DoSlashEffects(handNPC);

                hoverDestination = CalculateSlashHandDestination(hand, animationCompletion, hoverOffset);
                if (animationCompletion >= AnticipationCurveEnd && animationCompletion <= SlashCurveEnd)
                    hand.KatanaAfterimageOpacity = 1f;

                if (animationCompletion <= AnticipationCurveEnd + 0.045f || animationCompletion >= SlashCurveEnd + 0.1f)
                    NPC.damage = 0;
            }
            else
            {
                rotateForwardInterpolant = 0f;
                handNPC.damage = 0;
            }

            handNPC.Center = Vector2.Lerp(handNPC.Center, hoverDestination, 0.075f);
            handNPC.SmoothFlyNear(hoverDestination, 0.6f, 0.5f);
            handNPC.rotation = handNPC.AngleFrom(NPC.Center).AngleLerp(hand.ShoulderToHandDirection, rotateForwardInterpolant);
        }

        /// <summary>
        /// Performs slash effects for one of Ares' katanas during the Katana Slashes attack.
        /// </summary>
        /// <param name="handNPC">The hand's NPC instance.</param>
        public void KatanaSlashesHandUpdate_DoSlashEffects(NPC handNPC)
        {
            NPC.velocity += (handNPC.position - handNPC.oldPosition).SafeNormalize(Vector2.Zero) * new Vector2(7f, 1.5f);
            ScreenShakeSystem.StartShakeAtPoint(NPC.Center, 4.1f);
            SoundEngine.PlaySound(Exoblade.BigSwingSound with { Volume = 0.5f, MaxInstances = 0 }, handNPC.Center);
        }

        /// <summary>
        /// Creates idle particles for one of Ares' katanas during the Katana Slashes attack.
        /// </summary>
        /// <param name="hand">The hand's ModNPC instance.</param>
        /// <param name="handNPC">The hand's NPC instance.</param>
        public void KatanaSlashesHandUpdate_CreateParticles(AresHand hand, NPC handNPC)
        {
            if (AITimer % 20 == 19 && hand.EnergyDrawer.chargeProgress >= 0.4f)
            {
                int pulseCounter = (int)MathF.Round(hand.EnergyDrawer.chargeProgress * 5f);
                hand.EnergyDrawer.AddPulse(pulseCounter);
            }

            float handSpeed = handNPC.position.Distance(handNPC.oldPosition);
            int particleCount = (int)Utils.Remap(handSpeed, 7f, 30f, 1f, 3f);

            for (int i = 0; i < particleCount; i++)
            {
                Vector2 forward = handNPC.rotation.ToRotationVector2() * handNPC.spriteDirection;
                Vector2 perpendicular = forward.RotatedBy(MathHelper.PiOver2);
                Vector2 pixelVelocity = Main.rand.NextVector2Circular(3f, 3f) + perpendicular * Main.rand.NextFromList(-1f, 1f) * 5f;
                pixelVelocity *= 1f + handSpeed * 0.03f;

                Vector2 pixelSpawnPosition = handNPC.Center + forward * Main.rand.NextFloat(30f, 280f) + pixelVelocity.SafeNormalize(Vector2.Zero) * 26f;
                float pixelScale = Main.rand.NextFloat(0.6f, 1.1f);
                BloomPixelParticle pixel = new(pixelSpawnPosition, pixelVelocity, Color.Wheat, Color.Red * 0.5f, Main.rand.Next(7, 15), Vector2.One * pixelScale);
                pixel.Spawn();
            }
        }
    }
}
