using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        /// The opacity of Ares' flare glow during his death animation.
        /// </summary>
        public ref float DeathAnimation_FlareOpacity => ref NPC.ai[0];

        /// <summary>
        /// The scale of Ares' flare glow during his death animation.
        /// </summary>
        public ref float DeathAnimation_FlareScale => ref NPC.ai[1];

        /// <summary>
        /// The intensity of Ares' flare pulse during his death animation.
        /// </summary>
        public ref float DeathAnimation_FlarePulseIntensity => ref NPC.ai[2];

        /// <summary>
        /// How long Ares spends sitting in place, releasing smoke, during his death animation.
        /// </summary>
        public static int DeathAnimation_SmokeReleaseBuildupTime => Variables.GetAIInt("DeathAnimation_SmokeReleaseBuildupTime", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How frequently Ares' core pulses during his death animation.
        /// </summary>
        public static int DeathAnimation_PulseRate => Variables.GetAIInt("DeathAnimation_PulseRate", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long Ares spends performing pulses before exploding.
        /// </summary>
        public static int DeathAnimation_PulseTime => Variables.GetAIInt("DeathAnimation_PulseTime", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long it takes for Ares' silhouette to appear.
        /// </summary>
        public static int DeathAnimation_SilhouetteAppearDelay => Variables.GetAIInt("DeathAnimation_SilhouetteAppearDelay", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long it takes for Ares' silhouette to fade in.
        /// </summary>
        public static int DeathAnimation_SilhouetteFadeInTime => Variables.GetAIInt("DeathAnimation_SilhouetteFadeInTime", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long it takes for Ares' silhouette to start dissolving.
        /// </summary>
        public static int DeathAnimation_SilhouetteDissolveDelay => Variables.GetAIInt("DeathAnimation_SilhouetteDissolveDelay", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long it takes for Ares' silhouette to go from being fully stable to dissolving.
        /// </summary>
        public static int DeathAnimation_SilhouetteDissolveTime => Variables.GetAIInt("DeathAnimation_SilhouetteDissolveTime", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long it takes for Ares to officially die after his silhouette dissolves.
        /// </summary>
        public static int DeathAnimation_DeathDelay => Variables.GetAIInt("DeathAnimation_DeathDelay", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How far along Ares is with his jittering during his death animation.
        /// </summary>
        public float DeathAnimation_JitterInterpolant =>
            LumUtils.InverseLerp(0f, 0.6f, AITimer / (float)DeathAnimation_SmokeReleaseBuildupTime) *
            Utils.Remap(AITimer / (float)DeathAnimation_SmokeReleaseBuildupTime, 0.75f, 1f, 1f, 0.25f) *
            LumUtils.InverseLerp(DeathAnimation_PulseTime * 0.85f, DeathAnimation_PulseTime * 0.4f, AITimer - DeathAnimation_SmokeReleaseBuildupTime);

        /// <summary>
        /// The probability that Ares will create smoke on a given frame during his death animation.
        /// </summary>
        public float DeathAnimation_SmokeReleaseChance => LumUtils.InverseLerp(DeathAnimation_SmokeReleaseBuildupTime * 0.3f, DeathAnimation_SmokeReleaseBuildupTime, AITimer).Squared() * 0.56f;

        /// <summary>
        /// Whether visual effects, such as smoke, can appear during Ares' death animation.
        /// </summary>
        public bool DeathAnimation_DoneDoingVisualEffects => AITimer <= DeathAnimation_SmokeReleaseBuildupTime + DeathAnimation_PulseTime - LumUtils.SecondsToFrames(1.5f);

        /// <summary>
        /// The sound played as Ares' death animation progresses.
        /// </summary>
        public static readonly SoundStyle DeathBuildupSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/DeathBuildup");

        /// <summary>
        /// The AI update loop method for Ares' death animation.
        /// </summary>
        public void DoBehavior_DeathAnimation()
        {
            ZPosition *= 0.85f;
            NPC.Center = Vector2.Lerp(NPC.Center, Target.Center - Vector2.UnitY * 400f, LumUtils.InverseLerp(30f, 0f, AITimer) * 0.1f);
            NPC.velocity *= 0.95f;
            NPC.dontTakeDamage = true;
            NPC.damage = 0;
            NPC.velocity += Main.rand.NextVector2Circular(4.4f, 2.4f) * DeathAnimation_JitterInterpolant;
            NPC.rotation = NPC.velocity.X * 0.023f;
            if (Collision.SolidCollision(NPC.TopLeft, NPC.width, NPC.height + 540))
                NPC.velocity.Y -= 0.5f;

            // Make the sky go red as Ares' core becomes increasingly unstable.
            CustomExoMechsSky.RedSkyInterpolant = LumUtils.InverseLerp(0f, DeathAnimation_PulseTime, AITimer - DeathAnimation_SmokeReleaseBuildupTime);

            // Make Ares' glowmask lights malfunction.
            float colorInterpolant = LumUtils.Cos01(MathHelper.TwoPi * AITimer / 24f);
            Color[] malfunctionColors =
            [
                Color.Lerp(Color.Red, Color.Cyan, colorInterpolant),
                Color.Lerp(Color.Black, Color.White, colorInterpolant),
                Color.Lerp(Color.Wheat, Color.Yellow, colorInterpolant),
            ];
            ShiftLightColors(LumUtils.InverseLerp(0f, 16f, AITimer), malfunctionColors);

            // Make the screen rumble in accoradance with how much Ares is jittering.
            ScreenShakeSystem.SetUniversalRumble(DeathAnimation_JitterInterpolant * 8f, MathHelper.TwoPi, null, 0.2f);

            HandleDeathAnimationCoreVisualEffects();
            DeathAnimationHandUpdateWrapper();

            if (Main.rand.NextBool(DeathAnimation_SmokeReleaseChance))
                ReleaseDeathAnimationSmoke(NPC.Center - Vector2.UnitY.RotatedBy(NPC.rotation) * 110f);

            if (DeathAnimation_FlarePulseIntensity >= 0.4f && AITimer % DeathAnimation_PulseRate == (int)(DeathAnimation_PulseRate * 0.3f))
                PushPlayersAndHandsAway();

            if (AITimer == 1)
                SoundEngine.PlaySound(DeathBuildupSound).WithVolumeBoost(1.7f);

            // Explode.
            if (AITimer == DeathAnimation_SmokeReleaseBuildupTime + DeathAnimation_PulseTime)
            {
                ScreenShakeSystem.StartShake(27f);
                if (NPC.DeathSound.HasValue)
                    SoundEngine.PlaySound(NPC.DeathSound.Value with { Volume = 2.4f }).WithVolumeBoost(1.5f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), CorePosition, Vector2.Zero, ModContent.ProjectileType<AresDeathAnimationExplosion>(), 0, 0f);
            }

            HandleDeathAnimationSilhouetteEffects();

            if (AITimer >= DeathAnimation_SmokeReleaseBuildupTime + DeathAnimation_PulseTime + DeathAnimation_SilhouetteAppearDelay + DeathAnimation_SilhouetteFadeInTime + DeathAnimation_SilhouetteDissolveDelay + DeathAnimation_DeathDelay)
            {
                NPC.life = 0;
                if (AresBody.CanDropLoot())
                    NPC.NPCLoot();

                NPC.active = false;

                if (Main.netMode != NetmodeID.Server)
                {
                    Mod calamity = ModCompatibility.Calamity.Mod;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center - Vector2.UnitY.RotatedBy(NPC.rotation) * 44f, Vector2.Zero, calamity.Find<ModGore>("AresBody3").Type, NPC.scale);
                }
            }
        }

        /// <summary>
        /// Handles core flare related effects during Ares' death animation.
        /// </summary>
        public void HandleDeathAnimationCoreVisualEffects()
        {
            DeathAnimation_FlareOpacity = LumUtils.InverseLerp(0f, 90f, AITimer - DeathAnimation_SmokeReleaseBuildupTime);
            DeathAnimation_FlarePulseIntensity = LumUtils.InverseLerpBump(0f, 90f, DeathAnimation_PulseTime - 45f, DeathAnimation_PulseTime, AITimer - DeathAnimation_SmokeReleaseBuildupTime);
            DeathAnimation_FlareScale = LumUtils.InverseLerp(0f, 30f, AITimer - DeathAnimation_SmokeReleaseBuildupTime) * 0.25f;
            DeathAnimation_FlareScale += LumUtils.InverseLerp(0f, DeathAnimation_PulseTime, AITimer - DeathAnimation_SmokeReleaseBuildupTime).Squared() * 0.2f;

            float scalePulsationFactor = LumUtils.Cos01(MathHelper.TwoPi * AITimer / 6.7f);
            DeathAnimation_FlareScale += LumUtils.InverseLerp(DeathAnimation_PulseTime * 0.51f, DeathAnimation_PulseTime * 0.9f, AITimer - DeathAnimation_SmokeReleaseBuildupTime) * scalePulsationFactor * 0.24f;

            DelegateMethods.v3_1 = Color.Wheat.ToVector3() * DeathAnimation_FlareOpacity;
            Utils.PlotTileLine(NPC.Top - Vector2.UnitY * 250f, NPC.Bottom + Vector2.UnitY * 250f, (int)((NPC.width + 400) * DeathAnimation_FlareScale * 6f), DelegateMethods.CastLight);

            // Ensure that Ares draws visuals over his core.
            OptionalDrawAction = RenderDeathAnimationVisuals;
        }

        /// <summary>
        /// Pushes players and Ares' hands away from his center, to indicate energy pulses.
        /// </summary>
        public void PushPlayersAndHandsAway()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                float pushForce = Utils.Remap(player.Distance(NPC.Center), 100f, 2000f, 13f, 5f);
                player.velocity -= player.SafeDirectionTo(NPC.Center) * DeathAnimation_FlarePulseIntensity * pushForce;
            }

            int handNPC = ModContent.NPCType<AresHand>();
            foreach (NPC otherNPC in Main.ActiveNPCs)
            {
                if (otherNPC.type == handNPC && Main.rand.NextBool())
                {
                    otherNPC.velocity -= otherNPC.SafeDirectionTo(NPC.Center).RotatedByRandom(MathHelper.Pi) * 32f;
                    otherNPC.netUpdate = true;
                }
            }
            ScreenShakeSystem.StartShake(24f, MathHelper.Pi / 9f, NPC.SafeDirectionTo(Main.LocalPlayer.Center), 0.51f);
        }

        /// <summary>
        /// Handles silhouette related effects for Ares' death animation, making him dissolve after the explosion.
        /// </summary>
        public void HandleDeathAnimationSilhouetteEffects()
        {
            SilhouetteOpacity = LumUtils.InverseLerp(0f, DeathAnimation_SilhouetteFadeInTime, AITimer - DeathAnimation_SmokeReleaseBuildupTime - DeathAnimation_PulseTime - DeathAnimation_SilhouetteAppearDelay);
            SilhouetteDissolveInterpolant = LumUtils.InverseLerp(0f, DeathAnimation_SilhouetteDissolveTime, AITimer - DeathAnimation_SmokeReleaseBuildupTime - DeathAnimation_PulseTime - DeathAnimation_SilhouetteAppearDelay - DeathAnimation_SilhouetteFadeInTime - DeathAnimation_SilhouetteDissolveDelay);

            if (SilhouetteOpacity > 0f)
            {
                DeathAnimation_FlareOpacity = 0f;
                NPC.velocity *= 0.9f;
            }
        }

        /// <summary>
        /// Prepares Ares' hands for updating during his death animation.
        /// </summary>
        public void DeathAnimationHandUpdateWrapper()
        {
            Vector2 backArmOffset = new(430f, 50f);
            Vector2 frontArmOffset = new(280f, 224f);
            InstructionsForHands[0] = new(h => DeathAnimationHandUpdate(h, backArmOffset * new Vector2(-1f, 1f), 0));
            InstructionsForHands[1] = new(h => DeathAnimationHandUpdate(h, frontArmOffset * new Vector2(-1f, 1f), 1));
            InstructionsForHands[2] = new(h => DeathAnimationHandUpdate(h, frontArmOffset, 2));
            InstructionsForHands[3] = new(h => DeathAnimationHandUpdate(h, backArmOffset, 3));
        }

        /// <summary>
        /// Releases smoke at a given point for Ares' death animation.
        /// </summary>
        /// <param name="smokeBaseSpawnPosition">The position where the smoke should spawn.</param>
        public void ReleaseDeathAnimationSmoke(Vector2 smokeBaseSpawnPosition)
        {
            if (!DeathAnimation_DoneDoingVisualEffects)
                return;

            Color smokeColor = Color.Lerp(Color.SlateGray, Color.Red, Main.rand.NextFloat(0.65f));
            smokeColor = Color.Lerp(smokeColor, Color.Black, Main.rand.NextFloat().Cubed() * 0.9f);

            Vector2 smokeVelocity = -Vector2.UnitY.RotatedByRandom(0.43f) * Main.rand.NextFloat(5f, 25f);
            SmokeParticle smoke = new(smokeBaseSpawnPosition + Main.rand.NextVector2Circular(50f, 50f), smokeVelocity, smokeColor, Main.rand.Next(30, 75), Main.rand.NextFloat(0.7f, 1.4f), 0.045f);
            smoke.Spawn();

            if (Main.rand.NextBool() && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 arcDestination = smokeBaseSpawnPosition + Main.rand.NextVector2Unit() * Main.rand.NextFloat(84f, 225f);
                Vector2 arcLength = (arcDestination - smokeBaseSpawnPosition).RotatedByRandom(0.12f) * Main.rand.NextFloat(0.9f, 1f);
                LumUtils.NewProjectileBetter(NPC.GetSource_FromAI(), smokeBaseSpawnPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(6, 9));
            }
        }

        /// <summary>
        /// Performs secondary rendering effects for Ares' death animation.
        /// </summary>
        public void RenderDeathAnimationVisuals()
        {
            Texture2D flare = MiscTexturesRegistry.BloomFlare.Value;
            Texture2D glow = MiscTexturesRegistry.BloomCircleSmall.Value;

            float flareScale = DeathAnimation_FlareScale;
            Color flareColor = Color.Wheat * DeathAnimation_FlareOpacity;
            Vector2 glowDrawPosition = CorePosition - Main.screenPosition;
            Main.spriteBatch.Draw(flare, glowDrawPosition, null, NPC.GetAlpha(flareColor) with { A = 0 }, Main.GlobalTimeWrappedHourly * -0.51f, flare.Size() * 0.5f, flareScale, 0, 0f);
            Main.spriteBatch.Draw(flare, glowDrawPosition, null, NPC.GetAlpha(flareColor) with { A = 0 }, Main.GlobalTimeWrappedHourly * 0.4f, flare.Size() * 0.5f, flareScale, 0, 0f);

            float pulse = AITimer / (float)DeathAnimation_PulseRate % 1f * DeathAnimation_FlarePulseIntensity;
            Color glowColor = Color.White * DeathAnimation_FlareOpacity;
            Main.spriteBatch.Draw(glow, glowDrawPosition, null, NPC.GetAlpha(glowColor) with { A = 0 }, 0f, glow.Size() * 0.5f, flareScale * 4f, 0, 0f);

            Main.spriteBatch.Draw(glow, glowDrawPosition, null, NPC.GetAlpha(glowColor) with { A = 0 } * MathF.Pow(1f - pulse, 0.27f), 0f, glow.Size() * 0.5f, flareScale * MathHelper.Lerp(1f, 50f, pulse), 0, 0f);
        }

        /// <summary>
        /// Updates one of Ares' hands during his death animation.
        /// </summary>
        /// <param name="hand">The hand.</param>
        /// <param name="hoverOffset">How far the hand should hover, relative to Ares' position.</param>
        /// <param name="armIndex">The hand's index in the <see cref="InstructionsForHands"/> array.</param>
        public void DeathAnimationHandUpdate(AresHand hand, Vector2 hoverOffset, int armIndex)
        {
            NPC handNPC = hand.NPC;
            handNPC.Opacity = Utilities.Saturate(handNPC.Opacity + 0.025f);
            handNPC.SmoothFlyNear(NPC.Center + hoverOffset * NPC.scale, 0.25f, 0.75f);
            handNPC.rotation = handNPC.rotation.AngleLerp(handNPC.spriteDirection * MathHelper.PiOver2, 0.12f);
            handNPC.damage = 0;

            handNPC.velocity += Main.rand.NextVector2Circular(6f, 4f) * DeathAnimation_JitterInterpolant;

            hand.UsesBackArm = armIndex == 0 || armIndex == ArmCount - 1;
            hand.ArmSide = (armIndex >= ArmCount / 2).ToDirectionInt();
            hand.Frame = AITimer / 4 % Math.Min(hand.HandType.TotalHorizontalFrames * hand.HandType.TotalVerticalFrames, 12);

            hand.ArmEndpoint = Vector2.Lerp(hand.ArmEndpoint, handNPC.Center + handNPC.velocity, handNPC.Opacity);
            hand.EnergyDrawer.chargeProgress *= 0.7f;

            if (handNPC.Opacity <= 0f)
                hand.GlowmaskDisabilityInterpolant = 0f;

            if (Main.rand.NextBool(DeathAnimation_SmokeReleaseChance))
                ReleaseDeathAnimationSmoke(handNPC.Center - Vector2.UnitY.RotatedByRandom(0.6f) * Main.rand.NextFloat(30f, 120f));
        }

        /// <summary>
        /// Handles the rendering of things after Ares' silhouette.
        /// </summary>
        public void RenderAfterSilhouette()
        {
            Texture2D skullTexture = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Ares/AresSkull").Value;
            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            Main.spriteBatch.Draw(skullTexture, drawPosition, null, Color.Black * MathF.Pow(SilhouetteOpacity, 10f), NPC.rotation, skullTexture.Size() * 0.5f, NPC.scale, 0, 0f);
        }
    }
}
