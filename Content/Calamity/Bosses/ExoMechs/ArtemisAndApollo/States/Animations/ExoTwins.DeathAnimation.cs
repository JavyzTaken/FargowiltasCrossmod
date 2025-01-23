using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public static partial class ExoTwinsStates
    {
        /// <summary>
        /// The index of the current beep delay during their death animation.
        /// </summary>
        public static ref float DeathAnimation_BeepDelayIndex => ref SharedState.Values[0];

        /// <summary>
        /// Whether the Exo Twins have successfully collided with the player during their death animation.
        /// </summary>
        public static bool DeathAnimation_SuccessfullyCollided
        {
            get => SharedState.Values[1] == 1f && SharedState.AIState == ExoTwinsAIState.DeathAnimation;
            set => SharedState.Values[1] = value.ToInt();
        }

        /// <summary>
        /// How long the Exo Twins spend hovering near the player during their death animation.
        /// </summary>
        public static int DeathAnimation_HoverNearPlayerTime => Variables.GetAIInt("DeathAnimation_HoverNearPlayerTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long the Exo Twins should spend reorienting above the player during their death animation.
        /// </summary>
        public static int DeathAnimation_ReorientTime => Variables.GetAIInt("DeathAnimation_ReorientTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long the Exo Twins should spend charging up in anticipation of the dash for their death animation.
        /// </summary>
        public static int DeathAnimation_DashChargeUpTime => Variables.GetAIInt("DeathAnimation_DashChargeUpTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long the Exo Twins spend waiting before exploding after colliding.
        /// </summary>
        public static int DeathAnimation_ExplodeDelay => Variables.GetAIInt("DeathAnimation_ExplodeDelay", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The set of beep delays that should be used in a sequential order.
        /// </summary>
        public static readonly int[] BeepDelays =
        [
            LumUtils.SecondsToFrames(0.667f),
            LumUtils.SecondsToFrames(0.526f),
            LumUtils.SecondsToFrames(0.428f),
            LumUtils.SecondsToFrames(0.371f),
            LumUtils.SecondsToFrames(0.332f),
            LumUtils.SecondsToFrames(0.271f),
            LumUtils.SecondsToFrames(0.266f),
            LumUtils.SecondsToFrames(0.223f),
            LumUtils.SecondsToFrames(0.18f),
            LumUtils.SecondsToFrames(0.165f),
            LumUtils.SecondsToFrames(0.15f),
            LumUtils.SecondsToFrames(0.145f),
            LumUtils.SecondsToFrames(0.13f),
            LumUtils.SecondsToFrames(0.113f),
            LumUtils.SecondsToFrames(0.105f),
            LumUtils.SecondsToFrames(0.079f),
        ];

        /// <summary>
        /// The sound Artemis and Apollo make upon colliding with each other during their death animation.
        /// </summary>
        public static readonly SoundStyle CollisionSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/ExoTwins/DeathAnimationCollision") with { Volume = 1.6f };

        /// <summary>
        /// The beep sound Artemis and Apollo play as a warning.
        /// </summary>
        public static readonly SoundStyle WarningBeepSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/ExoTwins/WarningBeep", 2) with { MaxInstances = 0 };

        /// <summary>
        /// AI update loop method for the death animation of the Exo Twins.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        public static void DoBehavior_DeathAnimation(NPC npc, IExoTwin twinAttributes)
        {
            npc.dontTakeDamage = true;
            npc.Calamity().ShouldCloseHPBar = true;

            if (!DeathAnimation_SuccessfullyCollided)
            {
                if (AITimer <= DeathAnimation_HoverNearPlayerTime)
                    DoBehavior_DeathAnimation_MoveNearPlayer(npc, twinAttributes);

                int relativeTimer = AITimer - DeathAnimation_HoverNearPlayerTime;
                if (relativeTimer >= 1 && relativeTimer <= DeathAnimation_ReorientTime)
                    DoBehavior_DeathAnimation_Reorient(npc, twinAttributes, relativeTimer);

                relativeTimer -= DeathAnimation_ReorientTime;
                if (relativeTimer >= 1 && relativeTimer <= DeathAnimation_DashChargeUpTime)
                    DoBehavior_DeathAnimation_DashChargeUp(npc, twinAttributes, relativeTimer);

                relativeTimer -= DeathAnimation_DashChargeUpTime;
                if (relativeTimer >= 1)
                    DoBehavior_DeathAnimation_PerformKamikazeDash(npc, twinAttributes, relativeTimer);

                if (!DeathAnimation_SuccessfullyCollided)
                {
                    ReleaseDeathAnimationParticles(npc);
                    ScreenShakeSystem.SetUniversalRumble(CustomExoMechsSky.RedSkyInterpolant * 8f, MathHelper.TwoPi, null, 0.2f);
                }
                twinAttributes.Frame = twinAttributes.Animation.CalculateFrame(AITimer / 40f % 1f, twinAttributes.InPhase2);
            }
            else
                DoBehavior_DeathAnimation_PerformKamikazeImpactEffects(npc, twinAttributes);
        }

        /// <summary>
        /// Makes the Exo Twins redirect towards the player before anything else for their death animation.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        public static void DoBehavior_DeathAnimation_MoveNearPlayer(NPC npc, IExoTwin twinAttributes)
        {
            Vector2 hoverDestination = Target.Center + Target.SafeDirectionTo(npc.Center, -Vector2.UnitY) * 350f;

            // Have Artemis stay behind Apollo, the shy little thing she is.
            if (npc.type == ExoMechNPCIDs.ArtemisID)
            {
                NPC apollo = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen];
                hoverDestination = apollo.Center + Target.SafeDirectionTo(apollo.Center).RotatedBy(0f) * 360f;
            }

            npc.SmoothFlyNear(hoverDestination, 0.17f, 0.85f);
            npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.16f);

            twinAttributes.Animation = ExoTwinAnimation.Idle;
        }

        /// <summary>
        /// Makes the Exo Twins reorient near the player, playing warning beep sounds.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        /// <param name="relativeTimer">The AI timer relative to this AI substate.</param>
        public static void DoBehavior_DeathAnimation_Reorient(NPC npc, IExoTwin twinAttributes, int relativeTimer)
        {
            float reorientInterpolant = LumUtils.InverseLerp(0f, DeathAnimation_ReorientTime, relativeTimer);
            float flySpeedInterpolant = MathF.Pow(reorientInterpolant, 0.6f);
            float hoverOffsetAngle = MathHelper.SmoothStep(MathHelper.PiOver2, MathHelper.PiOver4, reorientInterpolant);
            if (npc.type == ExoMechNPCIDs.ArtemisID)
                hoverOffsetAngle += MathHelper.Pi;

            float hoverOffset = MathHelper.Lerp(350f, 560f, reorientInterpolant);

            Vector2 hoverDestination = Target.Center - Vector2.UnitY.RotatedBy(hoverOffsetAngle) * hoverOffset;
            npc.SmoothFlyNear(hoverDestination, flySpeedInterpolant * 0.25f, 1f - flySpeedInterpolant * 0.24f);
            npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.16f);

            twinAttributes.Animation = ExoTwinAnimation.ChargingUp;

            // Handle beep sounds.
            if (reorientInterpolant >= 0.1f && npc.soundDelay <= 0 && npc.type == ExoMechNPCIDs.ApolloID)
            {
                float beepInterpolant = LumUtils.InverseLerp(0.1f, 0.5f, reorientInterpolant);
                float beepPitch = LumUtils.InverseLerp(0.1f, 0.9f, reorientInterpolant).Squared() * 0.12f;

                npc.soundDelay = BeepDelays[(int)DeathAnimation_BeepDelayIndex];
                DeathAnimation_BeepDelayIndex = (int)MathHelper.Clamp(DeathAnimation_BeepDelayIndex + 1f, 0f, BeepDelays.Length - 1f);

                SoundEngine.PlaySound(WarningBeepSound with { Pitch = beepPitch });
            }

            CustomExoMechsSky.RedSkyInterpolant = MathF.Pow(reorientInterpolant, 0.75f);
        }

        /// <summary>
        /// Makes the Exo Twins hover near the player, jittering and preparing the dash.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        /// <param name="relativeTimer">The AI timer relative to this AI substate.</param>
        public static void DoBehavior_DeathAnimation_DashChargeUp(NPC npc, IExoTwin twinAttributes, int relativeTimer)
        {
            float chargeUpInterpolant = LumUtils.InverseLerp(0f, DeathAnimation_DashChargeUpTime, relativeTimer);

            float hoverOffset = MathHelper.Lerp(560f, 1000f, chargeUpInterpolant.Squared());
            Vector2 hoverDestination = Target.Center + Target.SafeDirectionTo(npc.Center) * hoverOffset;
            if (npc.type == ExoMechNPCIDs.ArtemisID)
                hoverDestination = Target.Center + Target.SafeDirectionTo(Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].Center) * -hoverOffset;

            npc.SmoothFlyNear(hoverDestination, 0.27f, 0.8f);
            npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.16f);

            twinAttributes.ThrusterBoost = chargeUpInterpolant * 2.95f;
            twinAttributes.Animation = ExoTwinAnimation.ChargingUp;

            CustomExoMechsSky.RedSkyInterpolant = 1f;
        }

        /// <summary>
        /// Makes the Exo Twins crash into the player.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        /// <param name="relativeTimer">The AI timer relative to this AI substate.</param>
        public static void DoBehavior_DeathAnimation_PerformKamikazeDash(NPC npc, IExoTwin twinAttributes, int relativeTimer)
        {
            if (relativeTimer == 2)
            {
                ScreenShakeSystem.StartShake(30f, shakeStrengthDissipationIncrement: 0.6f);
                SoundEngine.PlaySound(Artemis.ChargeSound);
            }

            float dashSpeed = relativeTimer * 4f + 75f;
            npc.velocity = Vector2.Lerp(npc.velocity, npc.SafeDirectionTo(Target.Center) * dashSpeed, 0.5f);
            if (!npc.WithinRange(Target.Center, 180f))
                npc.rotation = npc.velocity.ToRotation();

            twinAttributes.Animation = ExoTwinAnimation.Attacking;
            twinAttributes.ThrusterBoost = 3f;
            twinAttributes.WingtipVorticesOpacity = 1f;
            twinAttributes.MotionBlurInterpolant = 1f;
            CustomExoMechsSky.RedSkyInterpolant = 1f;

            if (npc.Hitbox.Intersects(Target.Hitbox))
            {
                for (int i = 0; i < 75; i++)
                {
                    float speedInterpolant = Main.rand.NextFloat();
                    Vector2 impactDustSpawnPosition = npc.Center + Main.rand.NextVector2Circular(38f, 38f);
                    Vector2 impactDustVelocity = npc.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.6f) * MathHelper.Lerp(8f, 46f, speedInterpolant);
                    Dust impactDust = Dust.NewDustPerfect(impactDustSpawnPosition, 264, impactDustVelocity);
                    impactDust.noLight = true;
                    impactDust.noGravity = speedInterpolant >= 0.5f && Main.rand.NextBool();
                    impactDust.color = Color.Orange;
                    impactDust.scale *= MathHelper.Lerp(1.45f, 0.4f, speedInterpolant);
                }

                SoundEngine.PlaySound(CollisionSound);
                AITimer = 0;
                DeathAnimation_SuccessfullyCollided = true;
                npc.netUpdate = true;
            }
        }

        /// <summary>
        /// Performs impact effects for the kami
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="twinAttributes"></param>
        public static void DoBehavior_DeathAnimation_PerformKamikazeImpactEffects(NPC npc, IExoTwin twinAttributes)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ManagedScreenFilter impactFrameShader = ShaderManager.GetFilter("FargowiltasCrossmod.ImpactFrameShader");
                Color darkFrameColor = Color.Black;
                Color lightFrameColor = Color.White;
                impactFrameShader.TrySetParameter("darkFrameColor", darkFrameColor);
                impactFrameShader.TrySetParameter("lightFrameColor", lightFrameColor);
                impactFrameShader.TrySetParameter("contrastMatrix", CalculateContrastMatrix(1f));
                impactFrameShader.TrySetParameter("impactFrameInterpolant", LumUtils.InverseLerp(50f, 35f, AITimer));
                impactFrameShader.SetTexture(ExoTwinsRenderTargetSystem.ExoTwinsTarget, 1);
                impactFrameShader.SetTexture(ExoTwinsRenderTargetSystem.DustTarget, 2);
                impactFrameShader.Activate();
            }

            if (AITimer >= DeathAnimation_ExplodeDelay)
            {
                if (npc.DeathSound.HasValue)
                    SoundEngine.PlaySound(npc.DeathSound.Value with { Volume = 2.4f });

                npc.life = 0;
                npc.HitEffect();
                if (npc.realLife == -1 && AresBody.CanDropLoot())
                    npc.checkDead();

                npc.active = false;

                ScreenShakeSystem.StartShake(19f);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.type == ExoMechNPCIDs.ApolloID)
                {
                    Utilities.NewProjectileBetter(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<GaussNukeBoom>(), 0, 0f, -1, 2000f, 0f, 2f);
                    Utilities.NewProjectileBetter(npc.GetSource_FromAI(), Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Center, Vector2.Zero, ModContent.ProjectileType<GaussNukeBoom>(), 0, 0f, -1, 2000f, 0f, 2f);
                }
            }

            twinAttributes.HasBeenDestroyed = true;
            npc.velocity *= 0.85f;
        }

        /// <summary>
        /// Constructs a contrast matrix for the impact frame shader.
        /// </summary>
        /// <param name="contrast">How strong the contrast should be.</param>
        public static Matrix CalculateContrastMatrix(float contrast)
        {
            // The way matrices work is as a means of creating linear transformations, such as squishes, rotations, scaling effects, etc.
            // Strictly speaking, however, they act as a sort of encoding for functions. The exact specifics of how this works is a bit too dense
            // to stuff into a massive code comment, but 3blue1brown's linear algebra series does an excellent job of explaining how they work:
            // https://www.youtube.com/watch?v=fNk_zzaMoSs&list=PLZHQObOWTQDPD3MizzM2xVFitgF8hE_ab

            // For this matrix, the "axes" are the RGBA channels, in that order.
            // Given that the matrix is somewhat sparse, it can be easy to represent the output equations for each color one-by-one.
            // For the purpose of avoiding verbose expressions, I will represent "oneOffsetContrast" as "c", and "inverseForce" as "f":

            // R = c * R + f * A
            // G = c * G + f * A
            // B = c * B + f * A
            // For the purposes of the screen shaders, A is always 1, so it's possible to rewrite things explicitly like so:
            // R = c * R + (1 - c) * 0.5
            // G = c * G + (1 - c) * 0.5
            // B = c * B + (1 - c) * 0.5

            // These are all linear equations with slopes that become increasingly sharp the greater c is. At a certain point (which can be trivially computed from c) the output
            // will be zero, and everything above or below that will race towards a large absolute value. The result of this is that color channels that are already strong are emphasized to their maximum
            // extent while color channels that are weak vanish into nothing, effectively increasing the contrast by a significant margin.
            // The reason the contrast needs to be offset by 1 is because inputs from 0-1 have the inverse effect, making the resulting colors more homogenous by bringing them closer to a neutral grey.
            float oneOffsetContrast = contrast + 1f;
            float inverseForce = (1f - oneOffsetContrast) * 0.5f;
            return new(
                oneOffsetContrast, 0f, 0f, 0f,
                0f, oneOffsetContrast, 0f, 0f,
                0f, 0f, oneOffsetContrast, 0f,
                inverseForce, inverseForce, inverseForce, 1f);
        }

        /// <summary>
        /// Releases death animation particles for a given Exo Twin.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        public static void ReleaseDeathAnimationParticles(NPC npc)
        {
            float smokeReleaseChance = LumUtils.InverseLerp(0f, DeathAnimation_HoverNearPlayerTime + DeathAnimation_ReorientTime, AITimer) * 0.55f;
            if (Main.rand.NextBool(smokeReleaseChance))
                ReleaseDeathAnimationSmoke(npc.Center + Main.rand.NextVector2Circular(42f, 42f));

            float electricityReleaseChance = LumUtils.InverseLerp(0f, DeathAnimation_ReorientTime, AITimer - DeathAnimation_HoverNearPlayerTime).Squared() * 0.54f;
            for (int i = 0; i < 2; i++)
            {
                if (Main.rand.NextBool(electricityReleaseChance))
                    ReleaseDeathAnimationElectricity(npc, npc.Center + Main.rand.NextVector2Circular(42f, 42f) + npc.rotation.ToRotationVector2() * 46f);
            }
        }

        /// <summary>
        /// Releases smoke at a given point for the Exo Twins' death animation.
        /// </summary>
        /// <param name="smokeBaseSpawnPosition">The position where the smoke should spawn.</param>
        public static void ReleaseDeathAnimationSmoke(Vector2 smokeBaseSpawnPosition)
        {
            Color smokeColor = Color.Lerp(Color.SlateGray, Color.Red, Main.rand.NextFloat(0.65f));
            smokeColor = Color.Lerp(smokeColor, Color.Black, Main.rand.NextFloat().Cubed() * 0.9f);

            Vector2 smokeVelocity = -Vector2.UnitY.RotatedByRandom(0.43f) * Main.rand.NextFloat(5f, 25f);
            SmokeParticle smoke = new(smokeBaseSpawnPosition + Main.rand.NextVector2Circular(50f, 50f), smokeVelocity, smokeColor, Main.rand.Next(30, 75), Main.rand.NextFloat(0.7f, 1.4f), 0.045f);
            smoke.Spawn();
        }

        /// <summary>
        /// Releases electricity at a given point for the Exo Twins' death animation.
        /// </summary>
        /// <param name="npc">The Exo Twin responsible for creating the electricity.</param>
        /// <param name="electricityBaseSpawnPosition">The position where the electricity should spawn.</param>
        public static void ReleaseDeathAnimationElectricity(NPC npc, Vector2 electricityBaseSpawnPosition)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 arcDestination = electricityBaseSpawnPosition + Main.rand.NextVector2Unit() * Main.rand.NextFloat(84f, 250f);
            Vector2 arcLength = (arcDestination - electricityBaseSpawnPosition).RotatedByRandom(0.16f);
            LumUtils.NewProjectileBetter(npc.GetSource_FromAI(), electricityBaseSpawnPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, Main.rand.Next(6, 9));
        }
    }
}
