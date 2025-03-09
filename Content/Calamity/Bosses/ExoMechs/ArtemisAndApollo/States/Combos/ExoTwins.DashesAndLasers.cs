using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
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
        /// The amount of attack cycles performed so far for the DashesAndLasers attack.
        /// </summary>
        public static ref float DashesAndLasers_AttackCycleCounter => ref SharedState.Values[0];

        /// <summary>
        /// The amount of damage basic shots from the Exo Twins do.
        /// </summary>
        public static int BasicShotDamage => Variables.GetAIInt("BasicShotDamage", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Apollo spends hovering during the DashesAndLasers attack.
        /// </summary>
        public static int DashesAndLasers_ApolloHoverTime => Variables.GetAIInt("DashesAndLasers_ApolloHoverTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Apollo spends reeling back during the DashesAndLasers attack.
        /// </summary>
        public static int DashesAndLasers_ApolloReelBackTime => Variables.GetAIInt("DashesAndLasers_ApolloReelBackTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Apollo spends dashing during the DashesAndLasers attack.
        /// </summary>
        public static int DashesAndLasers_ApolloDashTime => Variables.GetAIInt("DashesAndLasers_ApolloDashTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Apollo spends slowing down after dashing in the DashesAndLasers attack.
        /// </summary>
        public static int DashesAndLasers_ApolloSlowDownTime => Variables.GetAIInt("DashesAndLasers_ApolloSlowDownTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long attack cycles go on for during the DashesAndLasers attack.
        /// </summary>
        public static int DashesAndLasers_AttackCycleTime => DashesAndLasers_ApolloHoverTime + DashesAndLasers_ApolloReelBackTime + DashesAndLasers_ApolloDashTime + DashesAndLasers_ApolloSlowDownTime;

        /// <summary>
        /// The rate at which Artemis shoots lasers during the DashesAndLasers attack.
        /// </summary>
        public static int DashesAndLasers_ArtemisShootRate => Variables.GetAIInt("DashesAndLasers_ArtemisShootRate", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The amount of cycles Artemis and Apollo do before transitioning to the next attack during the DashesAndLasers attack.
        /// </summary>
        public static int DashesAndLasers_TotalAttackCycles => Variables.GetAIInt("DashesAndLasers_TotalAttackCycles", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Artemis spends shoots lasers during the DashesAndLasers attack.
        /// </summary>
        public static int DashesAndLasers_ArtemisShootTime => DashesAndLasers_ApolloHoverTime;

        /// <summary>
        /// How much Artemis arcs around when firing lasers during the DashesAndLasers attack.
        /// </summary>
        public static float DashesAndLasers_ArtemisShootArc => MathHelper.ToRadians(Variables.GetAIFloat("DashesAndLasers_ArtemisShootArcDegrees", ExoMechAIVariableType.Twins));

        /// <summary>
        /// The speed at which Apollo dashes during the DashesAndLasers attack.
        /// </summary>
        public static float DashesAndLasers_ApolloDashSpeed => Variables.GetAIFloat("DashesAndLasers_ApolloDashSpeed", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The speed at which lasers are shot by Artemis during the DashesAndLasers attack.
        /// </summary>
        public static float DashesAndLasers_ArtemisLaserShootSpeed => Variables.GetAIFloat("DashesAndLasers_ArtemisLaserShootSpeed", ExoMechAIVariableType.Twins);

        /// <summary>
        /// AI update loop method for the DashesAndLasers attack.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        public static void DoBehavior_DashesAndLasers(NPC npc, IExoTwin twinAttributes)
        {
            bool isApollo = npc.type == ExoMechNPCIDs.ApolloID;
            if (isApollo)
                DoBehavior_DashesAndLasers_ApolloDashes(npc, twinAttributes);
            else
                DoBehavior_DashesAndLasers_ArtemisLasers(npc, twinAttributes);

            if (AITimer % DashesAndLasers_AttackCycleTime == DashesAndLasers_AttackCycleTime - 1f)
            {
                DashesAndLasers_AttackCycleCounter++;
                if (DashesAndLasers_AttackCycleCounter >= DashesAndLasers_TotalAttackCycles)
                    ExoTwinsStateManager.TransitionToNextState();
            }
        }

        /// <summary>
        /// AI update loop method for Apollo during the DashesAndLasers.
        /// </summary>
        /// <param name="npc">Apollo's NPC instance.</param>
        /// <param name="apolloAttributes">Apollo's designated generic attributes.</param>
        public static void DoBehavior_DashesAndLasers_ApolloDashes(NPC npc, IExoTwin apolloAttributes)
        {
            int hoverTime = DashesAndLasers_ApolloHoverTime;
            int reelBackTime = DashesAndLasers_ApolloReelBackTime;
            int dashTime = DashesAndLasers_ApolloDashTime;
            int slowDownTime = DashesAndLasers_ApolloSlowDownTime;
            int wrappedTime = AITimer % DashesAndLasers_AttackCycleTime;
            Vector2 hoverDestination = Target.Center + Target.SafeDirectionTo(npc.Center) * new Vector2(650f, 450f);

            if (wrappedTime <= hoverTime)
            {
                float flySpeed = Utilities.InverseLerp(0f, hoverTime, AITimer).Cubed() * 0.15f + 0.01f;
                npc.SmoothFlyNear(hoverDestination, flySpeed, 1f - flySpeed);
                npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), wrappedTime / (float)hoverTime);
                apolloAttributes.Animation = ExoTwinAnimation.Idle;
                apolloAttributes.Frame = apolloAttributes.Animation.CalculateFrame(wrappedTime / (float)hoverTime, apolloAttributes.InPhase2);

                return;
            }

            if (wrappedTime <= hoverTime + reelBackTime)
            {
                float reelBackSpeed = Utilities.InverseLerp(0f, reelBackTime, wrappedTime - hoverTime).Squared() * 40f;
                float lookAngularVelocity = Utils.Remap(wrappedTime - hoverTime, 0f, reelBackTime, 0.4f, 0.029f);
                npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), lookAngularVelocity);
                npc.velocity = npc.rotation.ToRotationVector2() * -reelBackSpeed;
                npc.velocity *= 0.9f;

                apolloAttributes.Animation = ExoTwinAnimation.ChargingUp;
                apolloAttributes.Frame = apolloAttributes.Animation.CalculateFrame(Utilities.InverseLerp(0f, reelBackTime, wrappedTime - hoverTime), apolloAttributes.InPhase2);

                return;
            }

            if (wrappedTime <= hoverTime + reelBackTime + dashTime)
            {
                if (wrappedTime == hoverTime + reelBackTime + 1)
                {
                    ScreenShakeSystem.StartShake(10f, shakeStrengthDissipationIncrement: 0.4f);
                    SoundEngine.PlaySound(Artemis.ChargeSound);
                    npc.velocity = npc.rotation.ToRotationVector2() * 172f;
                    npc.netUpdate = true;
                }

                npc.damage = npc.defDamage;

                apolloAttributes.Animation = ExoTwinAnimation.Attacking;
                apolloAttributes.Frame = apolloAttributes.Animation.CalculateFrame(Utilities.InverseLerp(0f, dashTime, wrappedTime - hoverTime - reelBackTime), apolloAttributes.InPhase2);
                apolloAttributes.ThrusterBoost = MathHelper.Lerp(apolloAttributes.ThrusterBoost, 1.3f, 0.2f);

                return;
            }

            if (wrappedTime <= hoverTime + reelBackTime + dashTime + slowDownTime)
            {
                npc.velocity *= 0.65f;
                npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.1f);

                if (npc.velocity.Length() >= 20f)
                    npc.damage = npc.defDamage;

                apolloAttributes.Animation = ExoTwinAnimation.Idle;
                apolloAttributes.Frame = apolloAttributes.Animation.CalculateFrame(Utilities.InverseLerp(0f, slowDownTime, wrappedTime - hoverTime - reelBackTime - dashTime), apolloAttributes.InPhase2);
            }
        }

        /// <summary>
        /// AI update loop method for Artemis during the DashesAndLasers.
        /// </summary>
        /// <param name="npc">Artemis' NPC instance.</param>
        /// <param name="artemisAttributes">Artemis' designated generic attributes.</param>
        public static void DoBehavior_DashesAndLasers_ArtemisLasers(NPC npc, IExoTwin artemisAttributes)
        {
            int wrappedTime = AITimer % DashesAndLasers_AttackCycleTime;
            float lookOffsetAngle = 0f;

            if (wrappedTime <= DashesAndLasers_ArtemisShootTime)
            {
                if (wrappedTime % DashesAndLasers_ArtemisShootRate == DashesAndLasers_ArtemisShootRate - 1)
                {
                    // This ensures that the attack winds up a bit when it starts, so that the player has time to adequately prepare themselves for the lasers.
                    // Not having this resulted in telefrags due to the fact that the attack immediately started shooting after a previous attack.
                    float shootSpeedFactor = Utils.Remap(wrappedTime, 0f, 60f, 0.2f, 1f);

                    ShootArtemisLaser(npc, DashesAndLasers_ArtemisLaserShootSpeed * shootSpeedFactor);
                }

                lookOffsetAngle = MathF.Sin(MathHelper.TwoPi * wrappedTime / DashesAndLasers_ArtemisShootTime) * DashesAndLasers_ArtemisShootArc * 0.5f;
                artemisAttributes.Animation = ExoTwinAnimation.Attacking;
            }
            else
                artemisAttributes.Animation = ExoTwinAnimation.ChargingUp;

            Vector2 hoverDestination = Target.Center + new Vector2(npc.OnRightSideOf(Target).ToDirectionInt() * 600f, -300f) - npc.velocity * 3f;
            npc.SmoothFlyNearWithSlowdownRadius(hoverDestination, 0.038f, 0.74f, 74f);

            // Get really close to the desired destination on the first few frames, to ensure that Artemis doesn't fire from offscreen.
            float superFastRedirectInterpolant = LumUtils.Convert01To010(LumUtils.InverseLerp(24f, 0f, AITimer));
            npc.Center = Vector2.Lerp(npc.Center, hoverDestination, superFastRedirectInterpolant * 0.48f);

            npc.rotation = npc.AngleTo(Target.Center + Target.velocity * 10f) + lookOffsetAngle;

            artemisAttributes.Frame = artemisAttributes.Animation.CalculateFrame(wrappedTime / 50f % 1f, artemisAttributes.InPhase2);
        }

        /// <summary>
        /// Shoots a simple laser from Artemis' main shooting source, be it her pupil or orange exo crystal in phase 2.
        /// </summary>
        /// <param name="artemis">Artemis' NPC instance.</param>
        /// <param name="laserShootSpeed">How fast the lasers should be shot.</param>
        public static void ShootArtemisLaser(NPC artemis, float laserShootSpeed)
        {
            Vector2 aimDirection = artemis.rotation.ToRotationVector2();
            Vector2 laserShootVelocity = aimDirection * laserShootSpeed / ArtemisLaserImproved.TotalUpdates;
            Vector2 laserSpawnPosition = artemis.Center + aimDirection * 76f;

            for (int i = 0; i < 5; i++)
            {
                int particleLifetime = Main.rand.Next(4, 11);
                float particleSpeed = Main.rand.NextFloat(9f, 38f);
                Color particleColor = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0.9f));
                LineParticle line = new(laserSpawnPosition, aimDirection.RotatedByRandom(0.5f) * particleSpeed, false, particleLifetime, 1f, particleColor);
                GeneralParticleHandler.SpawnParticle(line);
            }

            CritSpark spark = new(laserSpawnPosition, Vector2.Zero, Color.Yellow, Color.OrangeRed, 2.2f, 11, 0.1f, 3f);
            GeneralParticleHandler.SpawnParticle(spark);

            SoundEngine.PlaySound(CommonCalamitySounds.ExoLaserShootSound with { Volume = 0.8f }, laserSpawnPosition);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                Utilities.NewProjectileBetter(artemis.GetSource_FromAI(), laserSpawnPosition, laserShootVelocity, ModContent.ProjectileType<ArtemisLaserImproved>(), BasicShotDamage, 0f);
        }
    }
}
