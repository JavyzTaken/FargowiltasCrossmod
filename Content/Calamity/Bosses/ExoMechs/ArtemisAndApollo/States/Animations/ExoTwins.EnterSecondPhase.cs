using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasSouls;
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

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public static partial class ExoTwinsStates
    {
        /// <summary>
        /// The opacity of Apollo's forcefield while protecting Artemis.
        /// </summary>
        public static ref float EnterSecondPhase_ProtectiveForcefieldOpacity => ref SharedState.Values[1];

        /// <summary>
        /// How long the Exo Twins spend slowing down in place before beginning their phase transition.
        /// </summary>
        public static int EnterSecondPhase_SlowDownTime => Variables.GetAIInt("EnterSecondPhase_SlowDownTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long the phase 2 transition animation lasts.
        /// </summary>
        public static int EnterSecondPhase_SecondPhaseAnimationTime => Variables.GetAIInt("EnterSecondPhase_SecondPhaseAnimationTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Artemis waits before she enters her second phase. This should be at least <see cref="EnterSecondPhase_SecondPhaseAnimationTime"/>, since she's meant to transition after Apollo does.
        /// </summary>
        public static int EnterSecondPhase_ArtemisPhaseTransitionDelay => EnterSecondPhase_SecondPhaseAnimationTime + Utilities.SecondsToFrames(1f);

        /// <summary>
        /// How long Apollo spends lowering the forcefield during the second phase transition.
        /// </summary>
        public static int EnterSecondPhase_LowerForcefieldTime => Variables.GetAIInt("EnterSecondPhase_LowerForcefieldTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The speed at which the Exo Twins shoot their lens upon releasing it.
        /// </summary>
        public static float EnterSecondPhase_LensPopOffSpeed => Variables.GetAIFloat("EnterSecondPhase_LensPopOffSpeed", ExoMechAIVariableType.Twins);

        /// <summary>
        /// What damage is multiplied by if Apollo is hit by a projectile while protecting Artemis.
        /// </summary>
        public static float EnterSecondPhase_ApolloDamageProtectionFactor => Variables.GetAIFloat("EnterSecondPhase_ApolloDamageProtectionFactor", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The sound the Exo Twins make when ejecting their lens.
        /// </summary>
        public static readonly SoundStyle LensEjectSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/ExoTwins/LensEject");

        /// <summary>
        /// The sound the Exo Twins make when entering their second phase.
        /// </summary>
        public static readonly SoundStyle Phase2TransitionSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/ExoTwins/Phase2Transition");

        public static void ReleaseLens(NPC npc)
        {
            SoundEngine.PlaySound(LensEjectSound, npc.Center);
            ScreenShakeSystem.StartShakeAtPoint(npc.Center, 5f);

            if (npc.type == ExoMechNPCIDs.ApolloID)
                SoundEngine.PlaySound(Phase2TransitionSound);

            Vector2 lensDirection = npc.rotation.ToRotationVector2();
            Vector2 lensOffset = lensDirection * 62f;
            Vector2 lensPosition = npc.Center + lensOffset;
            for (int i = 0; i < 45; i++)
            {
                Color smokeColor = Color.Lerp(Color.White, Color.Gray, Main.rand.NextFloat(0.85f));
                Vector2 smokeVelocity = lensDirection.RotatedByRandom(0.5f) * Main.rand.NextFloat(4f, 60f);
                int smokeLifetime = (int)Utils.Remap(smokeVelocity.Length(), 5f, 60f, 50f, 18f) + Main.rand.Next(-5, 10);
                SmokeParticle smoke = new(lensPosition, smokeVelocity, smokeColor, smokeLifetime, Main.rand.NextFloat(0.7f, 1.3f), 0.15f);
                smoke.Spawn();
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int lensProjectileID = ModContent.ProjectileType<BrokenArtemisLens>();
                if (npc.type == ModContent.NPCType<Apollo>())
                    lensProjectileID = ModContent.ProjectileType<BrokenApolloLens>();

                Vector2 lensPopOffVelocity = lensDirection * EnterSecondPhase_LensPopOffSpeed;
                Utilities.NewProjectileBetter(npc.GetSource_FromAI(), lensPosition, lensPopOffVelocity, lensProjectileID, 0, 0f);

                npc.netUpdate = true;
            }
        }

        /// <summary>
        /// Determines whether Apollo is currently protecting Artemis or not.
        /// </summary>
        /// <param name="apolloAttributes">Apollo's designated generic attributes.</param>
        public static bool DoBehavior_EnterSecondPhase_ApolloIsProtectingArtemis(IExoTwin apolloAttributes) => SharedState.AIState == ExoTwinsAIState.EnterSecondPhase;

        /// <summary>
        /// AI update loop method for the EnterSecondPhase state.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        public static void DoBehavior_EnterSecondPhase(NPC npc, IExoTwin twinAttributes)
        {
            bool isArtemis = npc.type == ExoMechNPCIDs.ArtemisID;
            bool isApollo = !isArtemis;

            // Get near the target.
            if (AITimer < EnterSecondPhase_SlowDownTime)
            {
                if (AITimer <= 2)
                    npc.velocity *= 0.3f;

                if (isApollo)
                    npc.Center = Vector2.Lerp(npc.Center, Target.Center + Target.SafeDirectionTo(npc.Center) * 640f, AITimer / (float)EnterSecondPhase_SlowDownTime * 0.07f);

                npc.velocity *= 0.84f;
                npc.rotation = npc.AngleTo(Target.Center);
                return;
            }

            bool shouldProtectArtemis = isApollo && twinAttributes.InPhase2;

            if (isApollo)
            {
                if (shouldProtectArtemis)
                    ProtectArtemis(npc, Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed], twinAttributes);
                else if (!twinAttributes.InPhase2)
                {
                    float animationCompletion = Utilities.InverseLerp(0f, EnterSecondPhase_SecondPhaseAnimationTime, AITimer - EnterSecondPhase_SlowDownTime);
                    PerformPhase2TransitionAnimations(npc, twinAttributes, animationCompletion);
                    npc.rotation = npc.AngleTo(Target.Center);
                }
            }

            else
            {
                npc.chaseable = false;

                float animationCompletion = Utilities.InverseLerp(0f, EnterSecondPhase_SecondPhaseAnimationTime, AITimer - EnterSecondPhase_SlowDownTime - EnterSecondPhase_ArtemisPhaseTransitionDelay);
                PerformPhase2TransitionAnimations(npc, twinAttributes, animationCompletion);

                // Look to the side if Artemis' animation completion is ongoing.
                NPC apollo = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen];
                if (animationCompletion > 0f)
                {
                    npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(apollo.Center) + 0.5f, 0.28f);
                    npc.velocity *= 0.9f;
                }

                // Otherwise, stick behind Apollo, waiting for his transition animation to finish.
                // The intent with this is that Artemis is trying to hide behind him.
                else
                {
                    Vector2 behindApollo = apollo.Center + Target.SafeDirectionTo(apollo.Center) * 360f;
                    npc.SmoothFlyNear(behindApollo, 0.11f, 0.8f);
                    npc.rotation = npc.AngleTo(Target.Center);
                }

                if (twinAttributes.InPhase2)
                {
                    ExoTwinsStateManager.TransitionToNextState(ExoTwinsAIState.DashesAndLasers);
                    ExoTwinsStateManager.SharedState.TotalFinishedAttacks = 0;
                }
            }
        }

        /// <summary>
        /// Performs phase 2 transition animations, popping off the lens when ready and setting the <see cref="IExoTwin.InPhase2"/> variable.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        /// <param name="animationCompletion">The animation completion.</param>
        public static void PerformPhase2TransitionAnimations(NPC npc, IExoTwin twinAttributes, float animationCompletion)
        {
            if (Collision.SolidCollision(npc.TopLeft - Vector2.One * 150f, npc.width + 300, npc.height + 300))
                npc.velocity.Y -= 2.54f;
            else
                npc.velocity *= 0.81f;

            int previousFrame = twinAttributes.Frame;
            twinAttributes.Animation = ExoTwinAnimation.EnteringSecondPhase;
            twinAttributes.Frame = twinAttributes.Animation.CalculateFrame(animationCompletion, false);

            if (previousFrame != twinAttributes.Frame && twinAttributes.Frame == ExoTwinAnimation.LensPopOffFrame)
                ReleaseLens(npc);

            if (animationCompletion >= 1f)
            {
                twinAttributes.InPhase2 = true;
                npc.netUpdate = true;
            }
        }

        /// <summary>
        /// Updates Apollo's movement in order to protect Artemis as she transitions to her second phase.
        /// </summary>
        /// <param name="apollo">Apollo's NPC instance.</param>
        /// <param name="artemis">Artemis' NPC instance.</param>
        /// <param name="artemis">Apollo's designated generic attributes.</param>
        public static void ProtectArtemis(NPC apollo, NPC artemis, IExoTwin apolloAttributes)
        {
            float guardOffset = MathF.Min(artemis.Distance(Target.Center), 285f);
            Vector2 guardDestination = artemis.Center + artemis.SafeDirectionTo(Target.Center) * guardOffset;
            apollo.SmoothFlyNear(guardDestination, 0.15f, 0.45f);
            apollo.rotation = apollo.AngleTo(Target.Center);

            apolloAttributes.Animation = ExoTwinAnimation.Idle;
            float frameAnimationCompletion = AITimer / 50f % 1f;

            bool playerIsUncomfortablyCloseToArtemis = Target.WithinRange(artemis.Center, 600f);
            if (playerIsUncomfortablyCloseToArtemis)
            {
                apollo.damage = apollo.defDamage;
                apolloAttributes.Animation = ExoTwinAnimation.Attacking;
                frameAnimationCompletion = SharedState.Values[0] / 50f % 1f;
                SharedState.Values[0]++;
            }
            else
                SharedState.Values[0] = 0f;

            float artemisAnimationCompletion = Utilities.InverseLerp(0f, EnterSecondPhase_SecondPhaseAnimationTime, AITimer - EnterSecondPhase_SlowDownTime - EnterSecondPhase_ArtemisPhaseTransitionDelay);
            bool lowerForcefield = artemisAnimationCompletion >= 0.78f;
            UpdateForcefieldOpacity(lowerForcefield);

            apollo.position -= apollo.SafeDirectionTo(Target.Center) * EnterSecondPhase_ProtectiveForcefieldOpacity * 6f;

            apolloAttributes.Frame = apolloAttributes.Animation.CalculateFrame(frameAnimationCompletion, apolloAttributes.InPhase2);
            apolloAttributes.SpecificDrawAction = () =>
            {
                PrimitivePixelationSystem.RenderToPrimsNextFrame(() => ProjectLensShield(apollo, true), PixelationPrimitiveLayer.AfterNPCs);
            };

            // Reflect projectiles that intersect with the forcefield.
            if (EnterSecondPhase_ProtectiveForcefieldOpacity >= 0.75f)
            {
                Vector2 perpendicular = (apollo.rotation + MathHelper.PiOver2).ToRotationVector2();
                Vector2 forcefieldStart = apollo.Center + apollo.rotation.ToRotationVector2() * 220f;
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    bool canBeReflected = projectile.CanBeReflected() || (projectile.aiStyle == 0 && projectile.penetrate != -1 && !projectile.reflected);
                    if (projectile.hostile || !canBeReflected || !FargoSoulsUtil.CanDeleteProjectile(projectile))
                        continue;

                    bool movingTowardsForcefield = Vector2.Dot(projectile.velocity, apollo.rotation.ToRotationVector2()) < 0f;
                    bool collidingWithForcefield =
                        projectile.Colliding(projectile.Hitbox, Utils.CenteredRectangle(forcefieldStart - perpendicular * 110f, Vector2.One * 110f)) ||
                        projectile.Colliding(projectile.Hitbox, Utils.CenteredRectangle(forcefieldStart + perpendicular * 110f, Vector2.One * 110f)) ||
                        projectile.Colliding(projectile.Hitbox, Utils.CenteredRectangle(forcefieldStart, Vector2.One * 140f));

                    if (collidingWithForcefield && movingTowardsForcefield)
                    {
                        Vector2 impactPoint = apollo.Center + apollo.SafeDirectionTo(projectile.Center) * 256f;
                        ScreenShakeSystem.StartShakeAtPoint(impactPoint, 2f);

                        SoundEngine.PlaySound(AresTeslaCannon.TeslaOrbShootSound, impactPoint);

                        for (int i = 0; i < 14; i++)
                        {
                            int pixelLifetime = Main.rand.Next(11, 32);
                            float pixelSize = Main.rand.NextFloat(0.85f, 1.7f);
                            BloomPixelParticle pixel = new(impactPoint, perpendicular * Main.rand.NextFloatDirection() * 32f + Main.rand.NextVector2Circular(9f, 9f), Color.Wheat, Color.Lime * 0.65f, pixelLifetime, Vector2.One * pixelSize);
                            pixel.Spawn();
                        }

                        float bloomScaleFactor = Main.rand.NextFloat(0.6f, 0.95f) + Main.rand.NextGaussian() * 0.4f;
                        for (int i = 0; i < 3; i++)
                        {
                            StrongBloom bloom = new(impactPoint, Vector2.Zero, Color.Wheat, bloomScaleFactor * 0.56f, 9);
                            GeneralParticleHandler.SpawnParticle(bloom);

                            StrongBloom glow = new(impactPoint, Vector2.Zero, Color.Lime * 0.6f, bloomScaleFactor * 0.95f, 12);
                            GeneralParticleHandler.SpawnParticle(glow);

                            StrongBloom outerGlow = new(impactPoint, Vector2.Zero, Color.Turquoise * 0.35f, bloomScaleFactor * 1.5f, 14);
                            GeneralParticleHandler.SpawnParticle(outerGlow);
                        }

                        projectile.velocity *= -0.7f;
                        projectile.velocity += Main.rand.NextVector2Circular(2f, 2f);
                        projectile.damage = 0;
                        projectile.netUpdate = true;
                    }
                }
            }

            if (Main.rand.NextBool(EnterSecondPhase_ProtectiveForcefieldOpacity * 0.8f))
                CreateForcefieldHologramDust(apollo);
        }

        /// <summary>
        /// Updates Apollo's protective forcefield opacity in a given direction.
        /// </summary>
        /// <param name="lowerForcefield">Whether the forcefield should be lowered or not.</param>
        public static void UpdateForcefieldOpacity(bool lowerForcefield)
        {
            float opacityUpdateRate = 0.05f;
            EnterSecondPhase_ProtectiveForcefieldOpacity = Utilities.Saturate(EnterSecondPhase_ProtectiveForcefieldOpacity - lowerForcefield.ToDirectionInt() * opacityUpdateRate);
        }

        /// <summary>
        /// Creates a single dust instance as a result of Apollo's forcefield.
        /// </summary>
        /// <param name="apollo">Apollo's NPC instance.</param>
        public static void CreateForcefieldHologramDust(NPC apollo)
        {
            Vector2 hologramDustPosition = apollo.Center + (apollo.rotation + Main.rand.NextGaussian(0.42f)).ToRotationVector2() * Main.rand.NextFloat(60f, 185f);
            Dust hologramDust = Dust.NewDustPerfect(hologramDustPosition, 261);
            hologramDust.velocity = apollo.SafeDirectionTo(hologramDustPosition).RotatedBy(Main.rand.NextFromList(-1f, 1f) * MathHelper.PiOver2) * Main.rand.NextFloatDirection() * 5f;
            hologramDust.color = Color.Lerp(Color.Lime, Color.Teal, Main.rand.NextFloat());
            hologramDust.scale *= 0.85f;
            hologramDust.noGravity = true;
        }

        public static void ProjectLensShield(NPC apollo, bool pixelated)
        {
            if (!pixelated)
                Main.spriteBatch.PrepareForShaders();

            Texture2D invisible = MiscTexturesRegistry.InvisiblePixel.Value;
            Texture2D forcefield = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/ArtemisAndApollo/Forcefield").Value;

            float spreadScale = 425f;
            float opacity = EnterSecondPhase_ProtectiveForcefieldOpacity;
            Vector2 forcefieldScale = Vector2.One * EnterSecondPhase_ProtectiveForcefieldOpacity * 0.8f;
            Vector2 spreadDrawPosition = apollo.Center - Main.screenPosition + apollo.rotation.ToRotationVector2() * 22f;
            Vector2 forcefieldDrawPosition = apollo.Center - Main.screenPosition + apollo.rotation.ToRotationVector2() * 220f;

            // This is necessary since the pixelation target is downscaled by 2x.
            if (pixelated)
            {
                spreadScale *= 0.5f;
                forcefieldScale *= 0.5f;
                spreadDrawPosition *= 0.5f;
                forcefieldDrawPosition *= 0.5f;
            }

            Effect hologramSpread = Filters.Scene["CalamityMod:SpreadTelegraph"].GetShader().Shader;
            hologramSpread.Parameters["centerOpacity"].SetValue(1f);
            hologramSpread.Parameters["mainOpacity"].SetValue(0.3f);
            hologramSpread.Parameters["halfSpreadAngle"].SetValue(opacity * 1.09f);
            hologramSpread.Parameters["edgeColor"].SetValue(new Vector3(0f, 5f, 2.25f));
            hologramSpread.Parameters["centerColor"].SetValue(new Vector3(0f, 0.9f, 0.6f));
            hologramSpread.Parameters["edgeBlendLength"].SetValue(0.04f);
            hologramSpread.Parameters["edgeBlendStrength"].SetValue(10f);
            hologramSpread.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(invisible, spreadDrawPosition, null, Color.White, apollo.rotation, invisible.Size() * 0.5f, Vector2.One * opacity * spreadScale * 1.6f, 0, 0f);

            hologramSpread.Parameters["mainOpacity"].SetValue(0.2f);
            hologramSpread.Parameters["halfSpreadAngle"].SetValue(opacity * 0.56f);
            hologramSpread.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(invisible, spreadDrawPosition, null, Color.White, apollo.rotation + 0.09f, invisible.Size() * 0.5f, Vector2.One * opacity * spreadScale, 0, 0f);

            hologramSpread.Parameters["mainOpacity"].SetValue(0.1f);
            hologramSpread.Parameters["halfSpreadAngle"].SetValue(opacity * 0.46f);
            hologramSpread.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(invisible, spreadDrawPosition, null, Color.White, apollo.rotation - 0.07f, invisible.Size() * 0.5f, Vector2.One * opacity * spreadScale, 0, 0f);
            Main.spriteBatch.Draw(invisible, spreadDrawPosition, null, Color.White, apollo.rotation - 0.1967f, invisible.Size() * 0.5f, Vector2.One * opacity * spreadScale, 0, 0f);

            if (!pixelated)
                Main.spriteBatch.PrepareForShaders(BlendState.NonPremultiplied);
            else
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            }

            ManagedShader forcefieldShader = ShaderManager.GetShader("FargowiltasCrossmod.LensShieldShader");
            forcefieldShader.SetTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise"), 1, SamplerState.LinearWrap);
            forcefieldShader.Apply();

            Main.instance.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            Color forcefieldColor = new(42, 209, 128);
            Main.spriteBatch.Draw(forcefield, forcefieldDrawPosition, null, forcefieldColor * opacity * 0.6f, apollo.rotation + MathHelper.PiOver2, forcefield.Size() * 0.5f, forcefieldScale, 0, 0f);

            Main.spriteBatch.ResetToDefault();
        }
    }
}
