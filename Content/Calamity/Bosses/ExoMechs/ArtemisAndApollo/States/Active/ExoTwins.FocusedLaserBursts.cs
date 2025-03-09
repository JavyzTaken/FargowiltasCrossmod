using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public static partial class ExoTwinsStates
    {
        /// <summary>
        /// How long Artemis speeds charging up energy during the FocusedLaserBursts attack.
        /// </summary>
        public static int FocusedLaserBursts_ChargeUpTime => Variables.GetAIInt("FocusedLaserBursts_ChargeUpTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Artemis spends recoiling after doing the initial spread burst during the FocusedLaserBursts attack.
        /// </summary>
        public static int FocusedLaserBursts_ChargeRecoilTime => Variables.GetAIInt("FocusedLaserBursts_ChargeRecoilTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Artemis spends firing rapid shots during the FocusedLaserBursts attack.
        /// </summary>
        public static int FocusedLaserBursts_RapidShotsTime => Variables.GetAIInt("FocusedLaserBursts_RapidShotsTime", ExoMechAIVariableType.Twins);
        /// <summary>
        /// The rate at which Artemis shoots lasers during the FocusedLaserBursts attack.
        /// </summary>
        public static int FocusedLaserBursts_RapidShotRate => Variables.GetAIInt("FocusedLaserBursts_RapidShotRate", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The amount of cycles that happen during the FocusedLaserBursts attack before the mechs choose another one.
        /// </summary>
        public static int FocusedLaserBursts_CycleCount => Variables.GetAIInt("FocusedLaserBursts_CycleCount", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The speed of rapid-fire lasers shot during the FocusedLaserBursts attack.
        /// </summary>
        public static float FocusedLaserBursts_RapidShotShootSpeed => Variables.GetAIFloat("FocusedLaserBursts_RapidShotShootSpeed", ExoMechAIVariableType.Twins);

        /// <summary>
        /// AI update loop method for the FocusedLaserBursts attack.
        /// </summary>
        /// <param name="npc">Artemis' NPC instance.</param>
        /// <param name="artemisAttributes">Artemis' designated generic attributes.</param>
        /// <param name="localAITimer">Artemis' local AI timer.</param>
        public static void DoBehavior_FocusedLaserBursts(NPC npc, IExoTwin artemisAttributes, ref int localAITimer)
        {
            int wrappedAITimer = localAITimer % (FocusedLaserBursts_ChargeUpTime + FocusedLaserBursts_ChargeRecoilTime + FocusedLaserBursts_RapidShotsTime);
            bool doneAttacking = localAITimer >= (FocusedLaserBursts_ChargeUpTime + FocusedLaserBursts_ChargeRecoilTime + FocusedLaserBursts_RapidShotsTime) * FocusedLaserBursts_CycleCount - 45;

            if (doneAttacking)
            {
                npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.04f);
                npc.velocity *= 0.9f;
            }
            else if (wrappedAITimer <= FocusedLaserBursts_ChargeUpTime)
                DoBehavior_FocusedLaserBursts_FireLaserSpread(npc, artemisAttributes, wrappedAITimer);

            else if (wrappedAITimer <= FocusedLaserBursts_ChargeUpTime + FocusedLaserBursts_ChargeRecoilTime)
            {
                npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.32f);
                npc.velocity *= 0.887f;
            }
            else
                DoBehavior_FocusedLaserBursts_FireRapidShots(npc, artemisAttributes, wrappedAITimer);

            if (localAITimer >= (FocusedLaserBursts_ChargeUpTime + FocusedLaserBursts_ChargeRecoilTime + FocusedLaserBursts_RapidShotsTime) * FocusedLaserBursts_CycleCount)
                ExoTwinsStateManager.TransitionToNextState();
        }

        public static void DoBehavior_FocusedLaserBursts_FireLaserSpread(NPC npc, IExoTwin artemisAttributes, int wrappedAITimer)
        {
            if (wrappedAITimer < FocusedLaserBursts_ChargeUpTime)
            {
                Vector2 hoverDestination = Target.Center + new Vector2((Target.Center.X - npc.Center.X).NonZeroSign() * -600f, -50f);
                npc.SmoothFlyNear(hoverDestination, 0.089f, 0.86f);
                npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.4f);

                ScreenShakeSystem.SetUniversalRumble((wrappedAITimer / (float)FocusedLaserBursts_ChargeUpTime).Cubed() * 0.5f, MathHelper.TwoPi, null, 0.2f);

                float chargeUpCompletion = wrappedAITimer / (float)FocusedLaserBursts_ChargeUpTime;
                float particleSpawnChance = Utilities.InverseLerp(0f, 0.85f, chargeUpCompletion).Squared();
                Vector2 aimDirection = npc.rotation.ToRotationVector2();
                Vector2 pupilPosition = npc.Center + aimDirection * 72f;
                for (int i = 0; i < 3; i++)
                {
                    if (Main.rand.NextBool(particleSpawnChance))
                    {
                        Color energyColor = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0.9f));
                        Vector2 energySpawnPosition = pupilPosition + Main.rand.NextVector2Unit() * Main.rand.NextFloat(46f, chargeUpCompletion.Squared() * 130f + 90f);
                        Vector2 energyVelocity = (pupilPosition - energySpawnPosition) * 0.16f + npc.velocity;
                        LineParticle energy = new(energySpawnPosition, energyVelocity, false, Main.rand.Next(8, 14), chargeUpCompletion * 0.9f, energyColor);
                        GeneralParticleHandler.SpawnParticle(energy);
                    }
                }

                if (wrappedAITimer % 12 == 0)
                {
                    StrongBloom bloom = new(pupilPosition, npc.velocity, Color.Orange * MathHelper.Lerp(0.5f, 1.5f, chargeUpCompletion), chargeUpCompletion, 16);
                    GeneralParticleHandler.SpawnParticle(bloom);
                }
            }

            if (wrappedAITimer == FocusedLaserBursts_ChargeUpTime)
            {
                SoundEngine.PlaySound(Artemis.LaserShotgunSound, npc.Center);
                FocusedLaserBursts_FireSpreadOfLasers(npc, 4, 3f, 0.51f);
                FocusedLaserBursts_FireSpreadOfLasers(npc, 7, 5f, 0.91f);

                npc.velocity -= npc.rotation.ToRotationVector2() * 30f;
                npc.netUpdate = true;

                ScreenShakeSystem.StartShakeAtPoint(npc.Center, 7.5f);
            }

            artemisAttributes.Animation = ExoTwinAnimation.ChargingUp;
            artemisAttributes.Frame = artemisAttributes.Animation.CalculateFrame(wrappedAITimer / 45f % 1f, artemisAttributes.InPhase2);
        }

        public static void DoBehavior_FocusedLaserBursts_FireRapidShots(NPC npc, IExoTwin artemisAttributes, int wrappedAITimer)
        {
            npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.156f);

            if (!npc.WithinRange(Target.Center, 700f))
                npc.SmoothFlyNear(Target.Center, 0.03f, 0.93f);
            else if (!npc.WithinRange(Target.Center, 136f))
                npc.velocity *= 0.951f;
            else
                npc.velocity -= npc.SafeDirectionTo(Target.Center) * 2f;

            if (AITimer % FocusedLaserBursts_RapidShotRate == FocusedLaserBursts_RapidShotRate - 1)
                ShootArtemisLaser(npc, FocusedLaserBursts_RapidShotShootSpeed);

            artemisAttributes.ThrusterBoost = Utilities.InverseLerp(13f, 24f, npc.velocity.Length()) * 0.8f;
            artemisAttributes.WingtipVorticesOpacity = artemisAttributes.ThrusterBoost;
            artemisAttributes.Animation = ExoTwinAnimation.Attacking;
            artemisAttributes.Frame = artemisAttributes.Animation.CalculateFrame(wrappedAITimer / 40f % 1f, artemisAttributes.InPhase2);
        }

        public static void FocusedLaserBursts_FireSpreadOfLasers(NPC npc, int laserCount, float laserShootSpeed, float spread)
        {
            for (int i = 0; i < laserCount; i++)
            {
                bool playsGrazeSound = i == laserCount / 2;
                Vector2 aimDirection = (npc.rotation + MathHelper.Lerp(-spread, spread, i / (float)(laserCount - 1f))).ToRotationVector2();
                Vector2 laserSpawnPosition = npc.Center + aimDirection * 76f;
                Vector2 laserShootVelocity = aimDirection * laserShootSpeed / ArtemisLaserImproved.TotalUpdates;

                for (int j = 0; j < 4; j++)
                {
                    int particleLifetime = Main.rand.Next(4, 11);
                    float particleSpeed = Main.rand.NextFloat(9f, 38f);
                    Color particleColor = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0.9f));
                    LineParticle line = new(laserSpawnPosition, aimDirection.RotatedByRandom(0.5f) * particleSpeed, false, particleLifetime, 1f, particleColor);
                    GeneralParticleHandler.SpawnParticle(line);
                }

                CritSpark spark = new(laserSpawnPosition, Vector2.Zero, Color.Yellow, Color.OrangeRed, 2.2f, 11, 0.1f, 3f);
                GeneralParticleHandler.SpawnParticle(spark);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Utilities.NewProjectileBetter(npc.GetSource_FromAI(), laserSpawnPosition, laserShootVelocity, ModContent.ProjectileType<ArtemisLaserImproved>(), BasicShotDamage, 0f, -1, 0f, playsGrazeSound.ToInt());
            }
        }
    }
}
