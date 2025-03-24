using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Particles;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    public sealed partial class AresBodyEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// How much forward Ares should aim his lasers on the second sweep during the Aimed Laser Bursts attack.
        /// </summary>
        public Vector2 AimedLaserBursts_AimOffset
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of sweeps that have happened so far during the Aimed Laser Bursts attack.
        /// </summary>
        public ref float AimedLaserBursts_SweepCounter => ref NPC.ai[1];

        /// <summary>
        /// How long it takes for cannons to charge up energy during the Aimed Laser Bursts attack.
        /// </summary>
        public int AimedLaserBursts_CannonChargeUpTime => LumUtils.SecondsToFrames(AimedLaserBursts_SweepCounter >= 1f ? 1.36f : 2.05f);

        /// <summary>
        /// The rate at which Ares releases small lasers during the Aimed Laser Bursts attack.
        /// </summary>
        public static int AimedLaserBursts_SmallLaserShootRate => Variables.GetAIInt("AimedLaserBursts_SmallLaserShootRate", ExoMechAIVariableType.Ares);

        /// <summary>
        /// The amount of small lasers Ares releases per burst during the Aimed Laser Bursts attack.
        /// </summary>
        public static int AimedLaserBursts_SmallLaserCount => Variables.GetAIInt("AimedLaserBursts_SmallLaserCount", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How much damage small lasers from Ares' core do.
        /// </summary>
        public static int SmallLaserDamage => Variables.GetAIInt("SmallLaserDamage", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How much damage laserbeams from Ares' laser cannons do.
        /// </summary>
        public static int CannonLaserbeamDamage => Variables.GetAIInt("CannonLaserbeamDamage", ExoMechAIVariableType.Ares);

        /// <summary>
        /// The maximum speed at which Ares hovers during the Aimed Laser Bursts attack.
        /// </summary>
        public static float AimedLaserBursts_MaxHoverSpeed => Variables.GetAIFloat("AimedLaserBursts_MaxHoverSpeed", ExoMechAIVariableType.Ares);

        /// <summary>
        /// The acceleration at which Ares hovers during the Aimed Laser Bursts attack.
        /// </summary>
        public static float AimedLaserBursts_MaxHoverAcceleration => Variables.GetAIFloat("AimedLaserBursts_MaxHoverAcceleration", ExoMechAIVariableType.Ares);

        /// <summary>
        /// The shoot speed of small lasers from Ares core during the Aimed Laser Bursts attack.
        /// </summary>
        public static float AimedLaserBursts_SmallLaserShootSpeed => Variables.GetAIFloat("AimedLaserBursts_SmallLaserShootSpeed", ExoMechAIVariableType.Ares);

        /// <summary>
        /// AI update loop method for the Aimed Laser Bursts attack.
        /// </summary>
        public void DoBehavior_AimedLaserBursts()
        {
            if (AITimer == 1)
                SoundEngine.PlaySound(AresLaserCannon.TelSound);

            Vector2 hoverDestination = Target.Center + new Vector2(NPC.OnRightSideOf(Target).ToDirectionInt() * 300f, -350f);
            if (MathHelper.Distance(Target.Center.X, NPC.Center.X) <= 120f)
                hoverDestination.X = Target.Center.X;

            NPC.Center = Vector2.Lerp(NPC.Center, hoverDestination, 0.0051f);
            if (NPC.WithinRange(hoverDestination, 85f))
                NPC.velocity *= 0.93f;
            else
                NPC.SimpleFlyMovement(NPC.SafeDirectionTo(hoverDestination) * AimedLaserBursts_MaxHoverSpeed, AimedLaserBursts_MaxHoverAcceleration);

            ShiftLightColors(LumUtils.InverseLerp(0f, 30f, AITimer), new(239, 62, 62), new(242, 112, 72));

            InstructionsForHands[0] = new(h => AimedLaserBurstsHandUpdate(h, new Vector2(-430f, 50f), 0, AimedLaserBursts_CannonChargeUpTime));
            InstructionsForHands[1] = new(h => AimedLaserBurstsHandUpdate(h, new Vector2(-280f, 224f), 1, AimedLaserBursts_CannonChargeUpTime));
            InstructionsForHands[2] = new(h => AimedLaserBurstsHandUpdate(h, new Vector2(280f, 224f), 2, AimedLaserBursts_CannonChargeUpTime));
            InstructionsForHands[3] = new(h => AimedLaserBurstsHandUpdate(h, new Vector2(430f, 50f), 3, AimedLaserBursts_CannonChargeUpTime));

            if (AITimer >= AimedLaserBursts_CannonChargeUpTime + CannonLaserbeam.Lifetime + 45)
            {
                AITimer = 0;
                AimedLaserBursts_SweepCounter++;
                NPC.netUpdate = true;
            }

            if (AimedLaserBursts_SweepCounter >= 1f)
            {
                AimedLaserBursts_SweepCounter = 0f;
                AimedLaserBursts_AimOffset = Vector2.Zero;
                SelectNewState();
            }
        }

        public void AimedLaserBurstsHandUpdate(AresHand hand, Vector2 hoverOffset, int armIndex, int chargeUpTime, float sweepTurnSpeedFactor = 1f)
        {
            NPC handNPC = hand.NPC;
            handNPC.SmoothFlyNear(NPC.Center + hoverOffset * NPC.scale, 0.3f, 0.8f);
            handNPC.Opacity = LumUtils.Saturate(handNPC.Opacity + 0.2f);
            hand.UsesBackArm = armIndex == 0 || armIndex == ArmCount - 1;
            hand.ArmSide = (armIndex >= ArmCount / 2).ToDirectionInt();
            hand.HandType = AresHandType.LaserCannon;
            hand.ArmEndpoint = handNPC.Center + handNPC.velocity;
            hand.GlowmaskDisabilityInterpolant = 0f;
            hand.Frame = AITimer / 5 % 12;
            hand.OptionalDrawAction = () =>
            {
                float sizeFadeout = LumUtils.InverseLerp(1f, 0.83f, hand.EnergyDrawer.chargeProgress).Cubed();
                float opacity = LumUtils.InverseLerp(0f, 60f, AITimer);
                float telegraphSize = (MathF.Cos(hand.NPC.position.X / 390f + AITimer / 23f) * 132f + 500f) * MathF.Sqrt(hand.EnergyDrawer.chargeProgress) * sizeFadeout;
                RenderLaserTelegraph(hand, opacity, sizeFadeout, telegraphSize, handNPC.rotation.ToRotationVector2() * handNPC.spriteDirection);
            };

            int relativeTimer = AITimer + handNPC.whoAmI * 101 % 30;
            hand.EnergyDrawer.chargeProgress = LumUtils.InverseLerp(0f, chargeUpTime, relativeTimer);
            if (hand.EnergyDrawer.chargeProgress >= 1f)
                hand.EnergyDrawer.chargeProgress = 0f;

            // Jitter in place.
            float chargeUpCompletion = hand.EnergyDrawer.chargeProgress;
            handNPC.velocity += Main.rand.NextVector2CircularEdge(0.27f, 1.6f) * chargeUpCompletion.Squared();

            if (AITimer % 15 == 14 && chargeUpCompletion >= 0.4f)
            {
                int pulseCounter = (int)MathF.Round(chargeUpCompletion * 5f);
                hand.EnergyDrawer.AddPulse(pulseCounter);
            }

            // Render pulsating energy atop the cannon.
            Vector2 cannonEnd = handNPC.Center + new Vector2(handNPC.spriteDirection * 74f, 16f).RotatedBy(handNPC.rotation);
            Vector2 aimDirection = NPC.SafeDirectionTo(cannonEnd);
            hand.OptionalDrawAction = () =>
            {
                float bloomAppearanceInterpolant = LumUtils.InverseLerpBump(0f, chargeUpTime, chargeUpTime + CannonLaserbeam.Lifetime - 25f, chargeUpTime + CannonLaserbeam.Lifetime, relativeTimer);

                Texture2D bloom = MiscTexturesRegistry.BloomCircleSmall.Value;
                float scalePulse = MathHelper.Lerp(0.75f, 1.32f, LumUtils.Cos01(Main.GlobalTimeWrappedHourly * 72f + NPC.whoAmI * 19f));
                float bloomScale = bloomAppearanceInterpolant * scalePulse * 0.5f;

                // Capturing the cannonEnd variable results in a one-frame delay due to that variable being computed in this AI function, rather than
                // during rendering. As such, it's calculated again manually in here.
                Vector2 drawPosition = handNPC.Center + new Vector2(handNPC.spriteDirection * 74f, 16f).RotatedBy(handNPC.rotation) - Main.screenPosition;

                Main.spriteBatch.Draw(bloom, drawPosition, null, new Color(255, 255, 255, 0), 0f, bloom.Size() * 0.5f, bloomScale * 0.4f, 0, 0f);
                Main.spriteBatch.Draw(bloom, drawPosition, null, new Color(255, 255, 174, 0), 0f, bloom.Size() * 0.5f, bloomScale * 0.67f, 0, 0f);
                Main.spriteBatch.Draw(bloom, drawPosition, null, new Color(255, 0, 32, 0), 0f, bloom.Size() * 0.5f, bloomScale, 0, 0f);
            };

            // Charge energy in anticipation of the laser shot.
            if (chargeUpCompletion < 0.9f)
            {
                float particleSpawnChance = LumUtils.InverseLerp(0f, 0.85f, chargeUpCompletion).Squared();
                for (int i = 0; i < 2; i++)
                {
                    if (Main.rand.NextBool(particleSpawnChance))
                    {
                        Color energyColor = Color.Lerp(Color.Red, Color.Orange, Main.rand.NextFloat(0.24f));
                        energyColor = Color.Lerp(energyColor, Color.Wheat, Main.rand.NextBool(chargeUpCompletion) ? 0.75f : 0.1f);

                        Vector2 energySpawnPosition = cannonEnd + Main.rand.NextVector2Unit() * Main.rand.NextFloat(36f, chargeUpCompletion.Squared() * 100f + 80f);
                        Vector2 energyVelocity = (cannonEnd - energySpawnPosition) * 0.16f + handNPC.velocity;
                        LineParticle energy = new(energySpawnPosition, energyVelocity, false, Main.rand.Next(6, 12), chargeUpCompletion * 0.9f, energyColor);
                        GeneralParticleHandler.SpawnParticle(energy);
                    }
                }
            }

            // Make the screen shake slightly before firing.
            ScreenShakeSystem.SetUniversalRumble(chargeUpCompletion.Cubed() * 3f, MathHelper.TwoPi, null, 0.2f);

            // Fire.
            if (relativeTimer == chargeUpTime)
            {
                ScreenShakeSystem.StartShake(7.5f);

                SoundEngine.PlaySound(AresLaserCannon.LaserbeamShootSound, handNPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), handNPC.Center, handNPC.rotation.ToRotationVector2() * handNPC.spriteDirection, ModContent.ProjectileType<CannonLaserbeam>(), CannonLaserbeamDamage, 0f, -1, handNPC.whoAmI);
            }

            Vector2 aimDestination = Target.Center;
            if (AimedLaserBursts_SweepCounter >= 1f)
            {
                if (AimedLaserBursts_AimOffset == Vector2.Zero)
                    AimedLaserBursts_AimOffset = Target.velocity * new Vector2(100f, 10f);
                aimDestination += AimedLaserBursts_AimOffset;
            }

            // Look at the player before firing.
            if (relativeTimer < chargeUpTime)
                hand.RotateToLookAt(handNPC.AngleTo(aimDestination), 0.1875f);

            // Handle post-firing particles.
            else
            {
                Vector2 energyDirection = (handNPC.rotation.ToRotationVector2() * handNPC.spriteDirection).RotatedByRandom(0.6f);
                BloomPixelParticle energy = new(cannonEnd, energyDirection * Main.rand.NextFloat(3f, 15f), Color.White, Color.Red * 0.45f, 20, Vector2.One);
                energy.Spawn();

                handNPC.velocity -= handNPC.rotation.ToRotationVector2() * handNPC.spriteDirection * 2f + Main.rand.NextVector2Circular(0.5f, 0.5f);

                // Attempt to aim at the target.
                // This doesn't use hand.RotateToLookAt because that changes the spriteDirection of the cannon.
                // For most cases this looks natural, but when a laser is being fired from the cannon it's super important that it never "jump" in terms of position.
                // By locking the spriteDirection in place and just rotating normally, this issue is avoided.
                float cannonTurnSpeed = LumUtils.InverseLerp(0f, 45f, relativeTimer - chargeUpTime) * sweepTurnSpeedFactor * 0.072f;
                float idealCannonRotation = handNPC.AngleTo(Target.Center);
                if (handNPC.spriteDirection == -1)
                    idealCannonRotation += MathHelper.Pi;

                handNPC.rotation = handNPC.rotation.AngleLerp(idealCannonRotation, cannonTurnSpeed);
            }
        }

        public static void RenderLaserTelegraph(AresHand hand, float opacity, float telegraphIntensityFactor, float telegraphSize, Vector2 telegraphDirection)
        {
            PrimitivePixelationSystem.RenderToPrimsNextFrame(() =>
            {
                RenderLaserTelegraphWrapper(hand, opacity, telegraphIntensityFactor, telegraphSize, telegraphDirection);
            }, PixelationPrimitiveLayer.AfterNPCs);
        }

        public static void RenderLaserTelegraphWrapper(AresHand hand, float opacity, float telegraphIntensityFactor, float telegraphSize, Vector2 telegraphDirection)
        {
            NPC handNPC = hand.NPC;
            Vector2 start = handNPC.Center + new Vector2(handNPC.spriteDirection * 74f, 16f).RotatedBy(handNPC.rotation);
            Texture2D invisible = MiscTexturesRegistry.InvisiblePixel.Value;

            // The multiplication by 0.5 is because this is being rendered to the pixelation target, wherein everything is downscaled by a factor of two, so that it can be upscaled later.
            Vector2 drawPosition = (start - Main.screenPosition) * 0.5f;

            Effect spread = Filters.Scene["CalamityMod:SpreadTelegraph"].GetShader().Shader;
            spread.Parameters["centerOpacity"].SetValue(0.4f);
            spread.Parameters["mainOpacity"].SetValue(opacity * 0.7f);
            spread.Parameters["halfSpreadAngle"].SetValue((1.0356f - LumUtils.Saturate(opacity) + LumUtils.Cos01(Main.GlobalTimeWrappedHourly * 2f + start.X / 99f) * 0.01f) * 1.6f);
            spread.Parameters["edgeColor"].SetValue(Vector3.Lerp(new(1.3f, 0.1f, 0.67f), new(4f, 0.6f, 0.08f), telegraphIntensityFactor));
            spread.Parameters["centerColor"].SetValue(new Vector3(1f, 0.1f, 0.1f));
            spread.Parameters["edgeBlendLength"].SetValue(0.07f);
            spread.Parameters["edgeBlendStrength"].SetValue(32f);
            spread.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(invisible, drawPosition, null, Color.White, telegraphDirection.ToRotation(), invisible.Size() * 0.5f, Vector2.One * opacity * telegraphSize, SpriteEffects.None, 0f);
        }
    }
}
