using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    public static partial class ExoTwinsStates
    {
        internal static LoopedSoundInstance GatlingLaserSoundLoop;

        /// <summary>
        /// The set of Artemis' laser cannon offsets, for usage during the MachineGunLasers attack.
        /// </summary>
        public static Vector2[] LaserCannonOffsets => [new(-72f, 34f), new(72f, 34f), new(-88f, 44f), new(88f, 44f), new(0f, 80f)];

        /// <summary>
        /// The rate at which Artemis shoots lasers during the MachineGunLasers attack.
        /// </summary>
        public static int MachineGunLasers_LaserShootRate => Variables.GetAIInt("MachineGunLasers_LaserShootRate", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Artemis waits before firing during the MachineGunLasers attack.
        /// </summary>
        public static int MachineGunLasers_AttackDelay => Variables.GetAIInt("MachineGunLasers_AttackDelay", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Apollo spends redirecting during the MachineGunLasers attack.
        /// </summary>
        public static int MachineGunLasers_ApolloRedirectTime => Variables.GetAIInt("MachineGunLasers_ApolloRedirectTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Apollo spends telegraphing in anticipation of his dash during the MachineGunLasers attack.
        /// </summary>
        public static int MachineGunLasers_ApolloTelegraphTime => Variables.GetAIInt("MachineGunLasers_ApolloTelegraphTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Apollo spends dashing during the MachineGunLasers attack.
        /// </summary>
        public static int MachineGunLasers_ApolloDashTime => Variables.GetAIInt("MachineGunLasers_ApolloDashTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Apollo spends slow down after a dash during the MachineGunLasers attack.
        /// </summary>
        public static int MachineGunLasers_ApolloDashSlowdownTime => Variables.GetAIInt("MachineGunLasers_ApolloDashSlowdownTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The rate at which Apollo releases lingering plasma during the MachineGunLasers attack.
        /// </summary>
        public static int MachineGunLasers_ApolloPlasmaReleaseRate => Variables.GetAIInt("MachineGunLasers_ApolloPlasmaReleaseRate", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long the MachineGunLasers attack goes on for.
        /// </summary>
        public static int MachineGunLasers_AttackDuration => Variables.GetAIInt("MachineGunLasers_AttackDuration", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The speed at which lasers fired by Artemis during the MachineGunLasers attack are shot.
        /// </summary>
        public static float MachineGunLasers_LaserShootSpeed => Variables.GetAIFloat("MachineGunLasers_LaserShootSpeed", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The standard random spread of lasers fired by Artemis during the MachineGunLasers attack.
        /// </summary>
        public static float MachineGunLasers_LaserShootSpread => MathHelper.ToRadians(Variables.GetAIFloat("MachineGunLasers_LaserShootSpreadDegrees", ExoMechAIVariableType.Twins));

        /// <summary>
        /// AI update loop method for the MachineGunLasers attack.
        /// </summary>
        /// <param name="npc">The Twins' NPC instance.</param>
        /// <param name="twinAttributes">The Twins' designated generic attributes.</param>
        public static void DoBehavior_MachineGunLasers(NPC npc, IExoTwin twinAttributes)
        {
            if (npc.type == ExoMechNPCIDs.ArtemisID)
                DoBehavior_MachineGunLasers_ArtemisLasers(npc, twinAttributes);
            else
                DoBehavior_MachineGunLasers_ApolloPlasmaDashes(npc, twinAttributes);
        }

        /// <summary>
        /// AI update loop method for the MachineGunLasers attack for Artemis specifically.
        /// </summary>
        /// <param name="npc">Artemis' NPC instance.</param>
        /// <param name="artemisAttributes">Artemis' designated generic attributes.</param>
        public static void DoBehavior_MachineGunLasers_ArtemisLasers(NPC npc, IExoTwin artemisAttributes)
        {
            if (AITimer <= MachineGunLasers_AttackDelay && !npc.WithinRange(Target.Center, 150f))
                npc.velocity += npc.SafeDirectionTo(Target.Center) * AITimer / MachineGunLasers_AttackDelay * 2.5f;

            // Slowly attempt to fly towards the target.
            npc.SmoothFlyNearWithSlowdownRadius(Target.Center, 0.03f, 0.95f, 350f);

            // Look at the target after the attack begins.
            // Before it begins, Artemis looks to the side instead, to give the player the opportunity to react properly to the lasers.
            float idealAngle = npc.AngleTo(Target.Center);
            if (AITimer < MachineGunLasers_AttackDelay)
                idealAngle += MathHelper.PiOver2;

            npc.rotation = npc.rotation.AngleTowards(idealAngle, 0.0256f).AngleLerp(idealAngle, 0.001f);

            DoBehavior_MachineGunLasers_ManageSounds(npc);

            if (AITimer % MachineGunLasers_LaserShootRate == MachineGunLasers_LaserShootRate - 1 && AITimer < MachineGunLasers_AttackDuration - 45 && AITimer >= MachineGunLasers_AttackDelay)
            {
                int offsetIndex = Main.rand.Next(LaserCannonOffsets.Length - 1);
                if (Main.rand.NextBool(4))
                    offsetIndex = LaserCannonOffsets.Length - 1;

                Vector2 unrotatedOffset = LaserCannonOffsets[offsetIndex];
                Vector2 laserShootOffset = unrotatedOffset.RotatedBy(npc.rotation - MathHelper.PiOver2) * npc.scale;
                Vector2 laserShootDirection = (npc.rotation + Main.rand.NextGaussian(MachineGunLasers_LaserShootSpread)).ToRotationVector2();
                Vector2 laserShootVelocity = laserShootDirection * Utilities.InverseLerp(60f, 120f, AITimer) * MachineGunLasers_LaserShootSpeed * Main.rand.NextFloat(1f, 1.15f);
                DoBehavior_MachineGunLasers_ShootLaser(npc, npc.Center + laserShootOffset, laserShootVelocity, offsetIndex == LaserCannonOffsets.Length - 1);
            }

            artemisAttributes.Animation = AITimer >= MachineGunLasers_AttackDelay ? ExoTwinAnimation.Attacking : ExoTwinAnimation.ChargingUp;
            artemisAttributes.Frame = artemisAttributes.Animation.CalculateFrame(AITimer / 30f % 1f, artemisAttributes.InPhase2);

            if (AITimer >= MachineGunLasers_AttackDelay + MachineGunLasers_AttackDuration)
                ExoTwinsStateManager.TransitionToNextState();
        }

        /// <summary>
        /// AI update loop method for the MachineGunLasers attack for Apollo specifically.
        /// </summary>
        /// <param name="npc">Apollo's NPC instance.</param>
        /// <param name="apolloAttributes">Apollo's designated generic attributes.</param>
        public static void DoBehavior_MachineGunLasers_ApolloPlasmaDashes(NPC npc, IExoTwin apolloAttributes)
        {
            int wrappedTimer = AITimer % (MachineGunLasers_ApolloRedirectTime + MachineGunLasers_ApolloTelegraphTime + MachineGunLasers_ApolloDashTime + MachineGunLasers_ApolloDashSlowdownTime);
            float dashSpeed = 150f;
            bool doneAttacking = AITimer >= MachineGunLasers_AttackDelay + MachineGunLasers_AttackDuration - 84;

            if (wrappedTimer <= MachineGunLasers_ApolloRedirectTime || doneAttacking)
            {
                float hoverFlySpeedInterpolant = Utilities.InverseLerpBump(0f, 0.6f, 0.8f, 1f, wrappedTimer / (float)MachineGunLasers_ApolloRedirectTime) * 0.09f;
                Vector2 artemisPerpendicularOffset = Target.SafeDirectionTo(Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].Center).RotatedBy(MathHelper.PiOver2) * 1250f;
                Vector2 left = Target.Center - artemisPerpendicularOffset;
                Vector2 right = Target.Center + artemisPerpendicularOffset;
                Vector2 hoverDestination = npc.Distance(left) < npc.Distance(right) ? left : right;

                if (hoverFlySpeedInterpolant < 0.025f)
                    npc.velocity *= 0.9f;
                else
                    npc.SmoothFlyNear(hoverDestination, hoverFlySpeedInterpolant, 0.81f);
                npc.Center = Vector2.Lerp(npc.Center, hoverDestination, 0.03f);
                npc.rotation = npc.AngleTo(Target.Center + Target.velocity * 25f);

                apolloAttributes.Animation = ExoTwinAnimation.ChargingUp;
                apolloAttributes.ThrusterBoost *= 0.75f;
            }

            else if (wrappedTimer <= MachineGunLasers_ApolloRedirectTime + MachineGunLasers_ApolloTelegraphTime)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && wrappedTimer == MachineGunLasers_ApolloRedirectTime + 1)
                {
                    npc.velocity = npc.rotation.ToRotationVector2() * -0.5f;
                    Utilities.NewProjectileBetter(npc.GetSource_FromAI(), npc.Center + npc.velocity * 30f, npc.rotation.ToRotationVector2() * 0.01f, ModContent.ProjectileType<ApolloLineTelegraph>(), 0, 0f, -1, MachineGunLasers_ApolloTelegraphTime);
                    npc.netUpdate = true;
                }

                npc.velocity *= 1.031f;
                apolloAttributes.Animation = ExoTwinAnimation.ChargingUp;
            }
            else
            {
                if (wrappedTimer == MachineGunLasers_ApolloRedirectTime + MachineGunLasers_ApolloTelegraphTime + 1)
                {
                    ScreenShakeSystem.StartShake(8.5f);

                    SoundEngine.PlaySound(Artemis.ChargeSound);
                    npc.velocity = npc.rotation.ToRotationVector2() * dashSpeed;
                    npc.netUpdate = true;
                }

                if (wrappedTimer >= MachineGunLasers_ApolloRedirectTime + MachineGunLasers_ApolloTelegraphTime + MachineGunLasers_ApolloDashTime)
                {
                    npc.velocity *= 0.7f;
                    npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.16f);
                }

                else if (wrappedTimer % MachineGunLasers_ApolloPlasmaReleaseRate == MachineGunLasers_ApolloPlasmaReleaseRate - 1)
                {
                    SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaShootSound, npc.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 plasmaFireballVelocity = npc.SafeDirectionTo(Target.Center).RotatedBy(MathHelper.Pi / 3.5f) * 35f;
                        Utilities.NewProjectileBetter(npc.GetSource_FromAI(), npc.Center, plasmaFireballVelocity, ModContent.ProjectileType<LingeringPlasmaFireball>(), BasicShotDamage, 0f);
                    }
                }

                // Do damage during the dash.
                npc.damage = npc.defDamage;

                apolloAttributes.Animation = ExoTwinAnimation.Attacking;
                apolloAttributes.ThrusterBoost = 2.3f;
                apolloAttributes.MotionBlurInterpolant = Utilities.InverseLerp(27.5f, 50f, npc.velocity.Length());
            }

            apolloAttributes.Frame = apolloAttributes.Animation.CalculateFrame(AITimer / 30f % 1f, apolloAttributes.InPhase2);
        }

        /// <summary>
        /// Instructs Artemis to shoot a single laser.
        /// </summary>
        /// <param name="npc">Artemis' NPC instance.</param>
        /// <param name="laserSpawnPosition">The spawn position of the laser.</param>
        /// <param name="laserShootVelocity">The shoot velocity of the laser.</param>
        /// <param name="big">Whether the shot laser should be big.</param>
        public static void DoBehavior_MachineGunLasers_ShootLaser(NPC npc, Vector2 laserSpawnPosition, Vector2 laserShootVelocity, bool big = false)
        {
            SoundEngine.PlaySound(CommonCalamitySounds.ExoLaserShootSound with { Volume = 0.4f, MaxInstances = 0 }, laserSpawnPosition);

            Color lightBloomColor = Color.Lerp(Color.Orange, Color.Wheat, Main.rand.NextFloat(0.75f));
            StrongBloom lightBloom = new(laserSpawnPosition, npc.velocity, lightBloomColor, 0.25f, 8);
            GeneralParticleHandler.SpawnParticle(lightBloom);

            LineParticle energy = new(laserSpawnPosition, laserShootVelocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.9f, 1.6f) + npc.velocity, false, 10, 0.6f, Color.Yellow);
            GeneralParticleHandler.SpawnParticle(energy);

            // Shake the screen just a tiny bit.
            ScreenShakeSystem.StartShakeAtPoint(laserSpawnPosition, 1.2f);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int laserID = big ? ModContent.ProjectileType<ArtemisLaserImproved>() : ModContent.ProjectileType<ArtemisLaserSmall>();
            Utilities.NewProjectileBetter(npc.GetSource_FromAI(), laserSpawnPosition, laserShootVelocity, laserID, BasicShotDamage, 0f);
        }

        /// <summary>
        /// Handles the management of sounds by Artemis during the MachineGunLasers attack.
        /// </summary>
        /// <param name="npc"></param>
        public static void DoBehavior_MachineGunLasers_ManageSounds(NPC npc)
        {
            if (AITimer == MachineGunLasers_AttackDelay - 20)
                SoundEngine.PlaySound(GatlingLaser.FireSound with { Volume = 2f });

            if (AITimer == MachineGunLasers_AttackDelay + 33)
                GatlingLaserSoundLoop = LoopedSoundManager.CreateNew(GatlingLaser.FireLoopSound, () => !npc.active || SharedState.AIState != ExoTwinsAIState.MachineGunLasers);
            if (AITimer >= MachineGunLasers_AttackDuration - 45 || AITimer <= MachineGunLasers_AttackDelay)
                GatlingLaserSoundLoop?.Stop();

            GatlingLaserSoundLoop?.Update(npc.Center);
        }
    }
}
