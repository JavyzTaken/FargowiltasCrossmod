using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    public sealed partial class AresBodyEternity : CalDLCEmodeBehavior
    {
        public static int KatanaSwingDashes_RedirectTime => LumUtils.SecondsToFrames(1.4f);

        public static int KatanaSwingDashes_FlyAwayTime => LumUtils.SecondsToFrames(3f);

        /// <summary>
        /// AI update loop method for the KatanaSlashes attack.
        /// </summary>
        public void DoBehavior_KatanaSwingDashes()
        {
            if (AITimer == 1)
            {
                ScreenShakeSystem.StartShake(10f);
                SoundEngine.PlaySound(LaughSound with { Volume = 10f });
            }

            AnimationState = AresFrameAnimationState.Laugh;

            bool drawBlurSlash = false;

            if (AITimer <= KatanaSwingDashes_RedirectTime)
            {
                float redirectSpeed = MathHelper.Lerp(0.05f, 0.2f, LumUtils.Convert01To010(LumUtils.InverseLerp(0f, 30f, AITimer).Squared()));
                redirectSpeed *= LumUtils.InverseLerp(KatanaSwingDashes_RedirectTime, KatanaSwingDashes_RedirectTime - 45f, AITimer);

                Vector2 hoverDestination = Target.Center + new Vector2(NPC.HorizontalDirectionTo(Target.Center) * -400f, -150f);
                NPC.SmoothFlyNearWithSlowdownRadius(hoverDestination, redirectSpeed, 1f - redirectSpeed, 50f);
            }
            else if (AITimer <= KatanaSwingDashes_RedirectTime + KatanaSwingDashes_FlyAwayTime)
            {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.velocity.X.NonZeroSign() * 67f, 0.1f);
                NPC.velocity.Y *= 1.01f;
            }
            else
            {
                // Teleport to the other side of the player.
                if (Main.netMode != NetmodeID.MultiplayerClient && AITimer == KatanaSwingDashes_RedirectTime + KatanaSwingDashes_FlyAwayTime + 1)
                {
                    if (LumUtils.CountProjectiles(ModContent.ProjectileType<AresSwingingKatanas>()) <= 0)
                        LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AresSwingingKatanas>(), KatanaDamage, 0f);

                    Vector2 teleportOffset = (NPC.SafeDirectionTo(Target.Center) * new Vector2(1f, 0.5f)).SafeNormalize(Vector2.UnitX);
                    NPC.Center = Target.Center + teleportOffset * 1750f;
                    NPC.velocity = NPC.SafeDirectionTo(Target.Center) * 37f;
                    NPC.netUpdate = true;
                }

                NPC.velocity *= 1.025f;

                if (AITimer >= KatanaSwingDashes_RedirectTime + KatanaSwingDashes_FlyAwayTime + 24 && !NPC.WithinRange(Target.Center, 1900f))
                {
                    AITimer = KatanaSwingDashes_RedirectTime + KatanaSwingDashes_FlyAwayTime;
                    NPC.netUpdate = true;
                }

                drawBlurSlash = true;

                ScreenShakeSystem.StartShakeAtPoint(NPC.Center, 6f, MathHelper.PiOver2, NPC.velocity.SafeNormalize(Vector2.Zero), 0.3f, 700f, 950f);
            }

            NPC.rotation = NPC.velocity.X * 0.002f;

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

            handNPC.SmoothFlyNear(hoverDestination, 0.6f, 0.5f);
            handNPC.rotation = handNPC.AngleFrom(NPC.Center).AngleLerp(hand.ShoulderToHandDirection, 0.3f);

            if (canRender)
                KatanaSlashesHandUpdate_CreateParticles(hand, handNPC);
        }
    }
}
