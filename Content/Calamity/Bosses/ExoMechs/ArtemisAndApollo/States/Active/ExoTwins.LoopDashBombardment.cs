using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Artemis;
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
        /// How long Apollo spends hovering/reeling back before dashing during the LoopDashBombardment attack.
        /// </summary>
        public static int LoopDashBombardment_HoverTime => Variables.GetAIInt("LoopDashBombardment_HoverTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The amount of time before the dash's happening that Apollo uses to determine when he should play a telegraph beep sound during the LoopDashBombardment attack.
        /// </summary>
        public static int LoopDashBombardment_TelegraphSoundBuffer => Variables.GetAIInt("LoopDashBombardment_TelegraphSoundBuffer", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Apollo spends performing his initial straight dash during the LoopDashBombardment attack.
        /// </summary>
        public static int LoopDashBombardment_StraightDashTime => Variables.GetAIInt("LoopDashBombardment_StraightDashTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Apollo spends spinning after dashing during the LoopDashBombardment attack.
        /// </summary>
        public static int LoopDashBombardment_SpinTime => Variables.GetAIInt("LoopDashBombardment_SpinTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Apollo spends dashing after spinning during the LoopDashBombardment attack.
        /// </summary>
        public static int LoopDashBombardment_FinalDashTime => Variables.GetAIInt("LoopDashBombardment_FinalDashTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How many cycles that occur during the LoopDashBombardment attack before a new one is selected.
        /// </summary>
        public static int LoopDashBombardment_CycleCount => Variables.GetAIInt("LoopDashBombardment_CycleCount", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The rate at which Apollo releases missiles during the LoopDashBombardment attack.
        /// </summary>
        public static int LoopDashBombardment_MissileReleaseRate => Variables.GetAIInt("LoopDashBombardment_MissileReleaseRate", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The speed Apollo starts his straight dash at during the LoopDashBombardment attack.
        /// </summary>
        public static float LoopDashBombardment_InitialApolloDashSpeed => Variables.GetAIFloat("LoopDashBombardment_InitialApolloDashSpeed", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The maximum speed that Apollo flies at while spinning during the LoopDashBombardment attack. When above this speed he will slow down.
        /// </summary>
        public static float LoopDashBombardment_MaxApolloSpinSpeed => Variables.GetAIFloat("LoopDashBombardment_MaxApolloSpinSpeed", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The speed of missiles shot by Apollo during the LoopDashBombardment attack.
        /// </summary>
        public static float LoopDashBombardment_ApolloMissileShootSpeed => Variables.GetAIFloat("LoopDashBombardment_ApolloMissileShootSpeed", ExoMechAIVariableType.Twins);

        // This serves two purposes:
        // 1. Anti-telefrag prevention. Wouldn't want missiles to just immediately fly at the player.
        // 2. It encourages risky play in the attack. By getting close to Apollo you can induce him to not fire rockets, and make it easier to weave through them as they fall after the fact.
        /// <summary>
        /// The closest distance Apollo can be to his targets before he ceases to fire rockets during the LoopDashBombardment attack.
        /// </summary>
        public static float LoopDashBombardment_ApolloMissileSpawnDistanceThreshold => Variables.GetAIFloat("LoopDashBombardment_ApolloMissileSpawnDistanceThreshold", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The acceleration of Apollo when performing his final straight dash during the LoopDashBombardment attack.
        /// </summary>
        public static float LoopDashBombardment_ApolloFinalDashAcceleration => Variables.GetAIFloat("LoopDashBombardment_ApolloFinalDashAcceleration", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The max speed of Apollo when performing his final straight dash during the LoopDashBombardment attack.
        /// </summary>
        public static float LoopDashBombardment_MaxApolloFinalDashSpeed => Variables.GetAIFloat("LoopDashBombardment_MaxApolloFinalDashSpeed", ExoMechAIVariableType.Twins);

        /// <summary>
        /// AI update loop method for the LoopDashBombardment attack.
        /// </summary>
        /// <param name="npc">Apollo's NPC instance.</param>
        /// <param name="apolloAttributes">Apollo's designated generic attributes.</param>
        /// <param name="localAITimer">Apollo's local AI timer.</param>
        public static void DoBehavior_LoopDashBombardment(NPC npc, IExoTwin apolloAttributes, ref int localAITimer)
        {
            int hoverTime = LoopDashBombardment_HoverTime;
            int telegraphSoundBuffer = LoopDashBombardment_TelegraphSoundBuffer;
            int straightDashTime = LoopDashBombardment_StraightDashTime;
            int spinTime = LoopDashBombardment_SpinTime;
            bool doneHovering = localAITimer >= hoverTime;
            bool performingStraightDash = localAITimer >= hoverTime && localAITimer <= hoverTime + straightDashTime;
            bool pastSpinTime = localAITimer >= hoverTime + straightDashTime;
            bool performingSpinDash = pastSpinTime && localAITimer <= hoverTime + straightDashTime + spinTime;
            bool acceleratingAfterSpin = localAITimer >= hoverTime + straightDashTime + spinTime + 10;
            ref float spinDirection = ref npc.ai[2];
            ref float cycleCounter = ref npc.ai[3];

            if (!doneHovering)
            {
                if (localAITimer == hoverTime - telegraphSoundBuffer)
                    SoundEngine.PlaySound(Artemis.ChargeTelegraphSound, npc.Center);

                // Hover to the side of the target at first. The hover offset calculation is the unit direction from the target to Apollo.
                // By default, using this would make Apollo attempt to stay a direction from the target, merely adjusting his radius.
                // However, since the Y position is multiplied every frame, this causes him to gradually level out and hover to the side of the target as time passes.
                Vector2 reelBackOffset = Target.SafeDirectionTo(npc.Center) * MathF.Pow(localAITimer / (float)hoverTime, 8f) * 350f;
                Vector2 hoverOffset = Target.SafeDirectionTo(npc.Center) * new Vector2(1f, 0.94f);
                Vector2 hoverDestination = Target.Center + hoverOffset * new Vector2(750f, 400f) + reelBackOffset;
                npc.SmoothFlyNear(hoverDestination, localAITimer / (float)hoverTime * 0.6f, 0.71f);
                npc.rotation = npc.AngleTo(Target.Center);

                apolloAttributes.Animation = ExoTwinAnimation.Idle;
                if (localAITimer >= hoverTime - telegraphSoundBuffer)
                    apolloAttributes.Animation = ExoTwinAnimation.ChargingUp;
            }

            if (doneHovering && !acceleratingAfterSpin)
                npc.damage = npc.defDamage;

            if (localAITimer == hoverTime)
            {
                ScreenShakeSystem.StartShakeAtPoint(npc.Center, 7.5f);
                npc.velocity = npc.SafeDirectionTo(Target.Center) * LoopDashBombardment_InitialApolloDashSpeed;
                spinDirection = -npc.velocity.X.NonZeroSign();
                npc.netUpdate = true;

                SoundEngine.PlaySound(Artemis.ChargeSound, npc.Center);
            }

            if (performingStraightDash)
            {
                npc.velocity *= 1.014f;
                apolloAttributes.ThrusterBoost = MathHelper.Lerp(apolloAttributes.ThrusterBoost, 1.1f, 0.2f);
            }

            if (pastSpinTime)
            {
                if (performingSpinDash)
                {
                    npc.velocity = npc.velocity.RotatedBy(MathHelper.TwoPi / spinTime * spinDirection);
                    if (localAITimer >= hoverTime + straightDashTime + 8 && npc.velocity.AngleBetween(npc.SafeDirectionTo(Target.Center + Target.velocity * 17f)) < 0.16f)
                        localAITimer = hoverTime + straightDashTime + spinTime;
                }

                if (npc.velocity.Length() > LoopDashBombardment_MaxApolloSpinSpeed && !acceleratingAfterSpin)
                    npc.velocity *= 0.94f;

                apolloAttributes.Animation = ExoTwinAnimation.Attacking;
                npc.rotation = npc.velocity.ToRotation();
            }

            // Release missiles.
            bool canFireMissiles = localAITimer >= hoverTime + straightDashTime && npc.velocity.Length() <= 150f;
            bool tooCloseToFireMissiles = npc.WithinRange(Target.Center, LoopDashBombardment_ApolloMissileSpawnDistanceThreshold);
            if (localAITimer % LoopDashBombardment_MissileReleaseRate == 0 && canFireMissiles && !tooCloseToFireMissiles)
                DoBehavior_LoopDashBombardment_ReleasePlasmaMissile(npc);

            if (acceleratingAfterSpin && npc.velocity.Length() <= LoopDashBombardment_MaxApolloFinalDashSpeed)
            {
                npc.velocity += npc.velocity.SafeNormalize(Vector2.UnitY) * LoopDashBombardment_ApolloFinalDashAcceleration;
                apolloAttributes.ThrusterBoost = MathHelper.Clamp(apolloAttributes.ThrusterBoost + 0.25f, 0f, 2f);
            }

            apolloAttributes.Frame = apolloAttributes.Animation.CalculateFrame(localAITimer / 40f % 1f, apolloAttributes.InPhase2);
            apolloAttributes.WingtipVorticesOpacity = LumUtils.InverseLerp(30f, 45f, npc.velocity.Length());

            if (localAITimer >= hoverTime + straightDashTime + spinTime + LoopDashBombardment_FinalDashTime)
            {
                localAITimer = 0;
                cycleCounter++;
                if (cycleCounter >= LoopDashBombardment_CycleCount)
                    ExoTwinsStateManager.TransitionToNextState();

                npc.netUpdate = true;
            }
        }

        /// <summary>
        /// Releases a single missing during the LoopDashBombardment attack.
        /// </summary>
        /// <param name="apollo">Apollo's NPC instance.</param>
        public static void DoBehavior_LoopDashBombardment_ReleasePlasmaMissile(NPC apollo)
        {
            SoundEngine.PlaySound(Apollo.MissileLaunchSound with { Volume = 0.4f, MaxInstances = 0 }, apollo.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 missileVelocity = Vector2.Lerp(apollo.rotation.ToRotationVector2(), apollo.SafeDirectionTo(Target.Center), 0.5f) * LoopDashBombardment_ApolloMissileShootSpeed;
                Vector2 missileSpawnPosition = apollo.Center + apollo.rotation.ToRotationVector2() * 70f;
                LumUtils.NewProjectileBetter(apollo.GetSource_FromAI(), missileSpawnPosition, missileVelocity, ModContent.ProjectileType<ApolloMissile>(), BasicShotDamage, 0f, Main.myPlayer, Target.Center.Y);
            }
        }
    }
}
