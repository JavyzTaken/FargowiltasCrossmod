﻿using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
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

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HolographicRush : ExoMechComboHandler
    {
        /// <summary>
        /// The spin angle at which the Exo Twins should orient themselves in accordance with.
        /// </summary>
        public static float ExoTwinSpinAngle
        {
            get
            {
                if (!Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechWorm))
                    return 0f;

                NPC hades = Main.npc[CalamityGlobalNPC.draedonExoMechWorm];
                return hades.ai[0];
            }
            set
            {
                if (!Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechWorm))
                    return;

                NPC hades = Main.npc[CalamityGlobalNPC.draedonExoMechWorm];
                hades.ai[0] = value;
            }
        }

        /// <summary>
        /// How long the Exo Twins spend redirecting and spinning around the target in a cycle.
        /// </summary>
        public static int ExoTwinLockOnTime => LumUtils.SecondsToFrames(1f);

        /// <summary>
        /// How long the Exo Twins spend dashing in a cycle.
        /// </summary>
        public static int ExoTwinDashTime => LumUtils.SecondsToFrames(0.4f);

        /// <summary>
        /// The amount of dashes the Exo Twins should perform before transitioning to the next attack.
        /// </summary>
        public static int ExoTwinDashCount => 8;

        /// <summary>
        /// The starting radius of the Exo Twins.
        /// </summary>
        public static float ExoTwinStartingRadius => 300f;

        /// <summary>
        /// The ending radius of the Exo Twins, by the time the reel back completes.
        /// </summary>
        public static float ExoTwinEndingRadius => 780f;

        /// <summary>
        /// How long Hades spends redirecting before releasing mines.
        /// </summary>
        public static int HadesRedirectTime => Utilities.SecondsToFrames(1f);

        /// <summary>
        /// How long Hades spends releasing mines.
        /// </summary>
        public static int HadesMineReleaseTime => Utilities.SecondsToFrames(2f);

        /// <summary>
        /// How long Hades spends releasing mines.
        /// </summary>
        public static int HadesAttackCycleTime => HadesRedirectTime + HadesMineReleaseTime;

        public override int[] ExpectedManagingExoMechs => [ModContent.NPCType<ThanatosHead>(), ModContent.NPCType<Apollo>()];

        public override bool Perform(NPC npc)
        {
            if (npc.type == ExoMechNPCIDs.HadesHeadID)
            {
                Perform_Hades(npc);

                // This is executed by Hades since unless the Exo Twins there is only one instance of him, and as such he can be counted on for
                // storing and executing attack data.
                HandleAttackState(npc);
            }
            if (npc.type == ExoMechNPCIDs.ArtemisID || npc.type == ExoMechNPCIDs.ApolloID)
                Perform_ExoTwin(npc);

            return AITimer >= ExoTwinDashCount * (ExoTwinLockOnTime + ExoTwinDashTime);
        }

        /// <summary>
        /// Performs Hades' part in the ExothermalLaserDashes attack.
        /// </summary>
        /// <param name="npc">Hades' NPC instance.</param>
        public static void Perform_Hades(NPC npc)
        {
            if (!npc.TryGetDLCBehavior(out HadesHeadEternity hades))
                return;

            hades.SegmentReorientationStrength = 0.1f;

            int wrappedTimer = AITimer % HadesAttackCycleTime;
            if (wrappedTimer < HadesRedirectTime)
            {
                if (!npc.WithinRange(Target.Center, 540f))
                {
                    float newSpeed = MathHelper.Lerp(npc.velocity.Length(), 29f, 0.05f);
                    Vector2 newDirection = npc.velocity.RotateTowards(npc.AngleTo(Target.Center), 0.04f).SafeNormalize(Vector2.UnitY);
                    npc.velocity = newDirection * newSpeed;
                }

                hades.BodyBehaviorAction = new(HadesHeadEternity.EveryNthSegment(4), HadesHeadEternity.OpenSegment());
            }
            else
            {
                if (!npc.WithinRange(Target.Center, 450f))
                    npc.velocity = Vector2.Lerp(npc.velocity, npc.SafeDirectionTo(Target.Center) * 21f, 0.03f);

                hades.BodyBehaviorAction = new(HadesHeadEternity.EveryNthSegment(3), DoBehavior_FireMine);
            }

            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public static void DoBehavior_FireMine(HadesBodyEternity behaviorOverride)
        {
            NPC segment = behaviorOverride.NPC;

            if ((segment.whoAmI * 47 + AITimer) % 150 == 0)
            {
                Vector2 mineSpawnPosition = behaviorOverride.TurretPosition;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int time = AITimer % HadesAttackCycleTime - HadesRedirectTime - Main.rand.Next(60);
                    int mineLifetime = LumUtils.SecondsToFrames(Main.rand.NextFloat(4f, 10f));
                    float mineSpeed = Main.rand.NextFloat(50f, 150f);
                    float mineOffsetAngle = Main.rand.NextGaussian(0.14f);
                    Vector2 mineVelocity = (Target.Center - mineSpawnPosition).SafeNormalize(Vector2.UnitY).RotatedBy(mineOffsetAngle) * mineSpeed;
                    Utilities.NewProjectileBetter(segment.GetSource_FromAI(), mineSpawnPosition, mineVelocity, ModContent.ProjectileType<HadesMine>(), HadesHeadEternity.MineDamage, 0f, -1, mineLifetime, time);
                }

                SoundEngine.PlaySound(Apollo.MissileLaunchSound with { Volume = 0.6f, MaxInstances = 0 }, mineSpawnPosition);
                segment.netUpdate = true;
            }

            HadesHeadEternity.OpenSegment().Invoke(behaviorOverride);
        }

        /// <summary>
        /// Performs The Exo Twins' part in the ExothermalLaserDashes attack.
        /// </summary>
        /// <param name="npc">The Exo Twins' NPC instance.</param>
        public static void Perform_ExoTwin(NPC npc)
        {
            if (!Main.npc.IndexInRange(CalamityGlobalNPC.draedonExoMechWorm))
                return;

            int lockOnTime = ExoTwinLockOnTime;
            int dashTime = ExoTwinDashTime;
            int wrappedAITimer = AITimer % (lockOnTime + dashTime);
            float hoverOffsetAngle = ExoTwinSpinAngle;

            // Apply visuals and animations.
            if (npc.TryGetDLCBehavior(out CalDLCEmodeBehavior behavior) && behavior is IExoTwin twin)
            {
                twin.SpecificDrawAction = () =>
                {
                    RenderElectricGleam(npc.Center + npc.rotation.ToRotationVector2() * 68f - Main.screenPosition, (1f - npc.Opacity).Cubed(), npc.type == ModContent.NPCType<Artemis>());
                };
                twin.Frame = twin.Animation.CalculateFrame(AITimer / 40f % 1f, twin.InPhase2);
                twin.SpecialShaderAction = Perform_ExoTwin_RenderWithHologramShader;
            }

            // Make the Exo Twins SPIN!!
            if (npc.type == ModContent.NPCType<Artemis>())
                hoverOffsetAngle += MathHelper.Pi;
            else
                ExoTwinSpinAngle += LumUtils.Convert01To010(LumUtils.InverseLerp(0f, lockOnTime * 0.7f, wrappedAITimer)) * MathHelper.Pi / 30f;

            float lockOnInterpolant = LumUtils.InverseLerp(lockOnTime, lockOnTime * 0.5f, wrappedAITimer);
            float reelBackInterpolant = MathF.Pow(LumUtils.InverseLerp(0f, lockOnTime * 0.833f, wrappedAITimer), 6f);
            float hoverRadius = MathHelper.Lerp(ExoTwinStartingRadius, ExoTwinEndingRadius, reelBackInterpolant);
            Vector2 hoverDestination = Target.Center + hoverOffsetAngle.ToRotationVector2() * hoverRadius;

            // Teleport to the hover destination on the first frame.
            if (wrappedAITimer == 1)
            {
                npc.Center = hoverDestination;
                npc.velocity = Vector2.Zero;
                npc.netUpdate = true;
            }

            // Spin around and hover near target, reeling backwards as time passes.
            if (wrappedAITimer < lockOnTime)
            {
                npc.SmoothFlyNear(hoverDestination, lockOnInterpolant * 0.4f, 1f - lockOnInterpolant * 0.36f);
                npc.rotation = npc.AngleTo(Target.Center);
            }

            // Perform the dash.
            else if (wrappedAITimer == lockOnTime)
            {
                SoundEngine.PlaySound(Artemis.ChargeSound, npc.Center);
                ScreenShakeSystem.StartShakeAtPoint(npc.Center, 16f, shakeStrengthDissipationIncrement: 0.6f);

                npc.velocity = npc.rotation.ToRotationVector2() * 12f;
                npc.netUpdate = true;
            }

            // Accelerate after the dash.
            else
            {
                npc.velocity = npc.velocity * 1.075f + npc.velocity.SafeNormalize(Vector2.Zero) * 6f;
                npc.rotation = npc.velocity.ToRotation();
                npc.damage = npc.defDamage;
            }

            npc.Opacity = LumUtils.InverseLerp(0f, lockOnTime * 0.75f, wrappedAITimer);
        }

        /// <summary>
        /// Handles general purpose state variables for the ExothermalLaserDashes attack.
        /// </summary>
        /// <param name="hades">Hades' NPC instance.</param>
        public static void HandleAttackState(NPC hades)
        {
        }

        /// <summary>
        /// Draws a gleam on an Exo Twin's pupil/front weapon as a telegraph.
        /// </summary>
        /// <param name="drawPosition">The base draw position of the Exo Twin.</param>
        /// <param name="glimmerInterpolant">How strong the gleam should be.</param>
        /// <param name="orange">Whether the gleam should be colored orange.</param>
        public static void RenderElectricGleam(Vector2 drawPosition, float glimmerInterpolant, bool orange)
        {
            Texture2D flare = MiscTexturesRegistry.ShineFlareTexture.Value;
            Texture2D bloom = MiscTexturesRegistry.BloomCircleSmall.Value;

            float flareOpacity = LumUtils.InverseLerp(1f, 0.75f, glimmerInterpolant);
            float flareScale = MathF.Pow(LumUtils.Convert01To010(glimmerInterpolant), 0.9f) * 1.6f + 0.1f;
            flareScale *= LumUtils.InverseLerp(0f, 0.09f, glimmerInterpolant);

            float flareRotation = MathHelper.SmoothStep(0f, MathHelper.TwoPi, MathF.Pow(glimmerInterpolant, 0.2f)) + MathHelper.PiOver4;
            Color flareColorA = new(45, 197, 44, 0);
            Color flareColorB = new(174, 255, 159, 0);
            Color flareColorC = new(245, 255, 170, 0);
            if (orange)
            {
                flareColorA = new(255, 82, 7, 0);
                flareColorB = new(255, 165, 20, 0);
                flareColorC = new(255, 255, 163, 0);
            }

            Main.spriteBatch.Draw(bloom, drawPosition, null, flareColorA * flareOpacity * 0.32f, 0f, bloom.Size() * 0.5f, flareScale * 1.9f, 0, 0f);
            Main.spriteBatch.Draw(bloom, drawPosition, null, flareColorB * flareOpacity * 0.56f, 0f, bloom.Size() * 0.5f, flareScale, 0, 0f);
            Main.spriteBatch.Draw(flare, drawPosition, null, flareColorC * flareOpacity, flareRotation, flare.Size() * 0.5f, flareScale, 0, 0f);
        }

        public static bool Perform_ExoTwin_RenderWithHologramShader(Texture2D texture, NPC npc)
        {
            float hologramOverlayInterpolant = 1f - npc.Opacity;
            Vector4 frameArea = new(npc.frame.Left / (float)texture.Width, npc.frame.Top / (float)texture.Height, npc.frame.Right / (float)texture.Width, npc.frame.Bottom / (float)texture.Height);
            ManagedShader hologramShader = ShaderManager.GetShader("FargowiltasCrossmod.HologramShader");
            hologramShader.TrySetParameter("hologramInterpolant", hologramOverlayInterpolant);
            hologramShader.TrySetParameter("hologramSinusoidalOffset", MathF.Pow(hologramOverlayInterpolant, 7f) * 0.01f + LumUtils.InverseLerp(0.4f, 1f, hologramOverlayInterpolant) * 0.02f);
            hologramShader.TrySetParameter("textureSize0", texture.Size());
            hologramShader.TrySetParameter("frameArea", frameArea);
            hologramShader.Apply();
            return true;
        }
    }
}
