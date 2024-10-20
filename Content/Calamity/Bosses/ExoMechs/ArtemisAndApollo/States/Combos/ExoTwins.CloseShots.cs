using CalamityMod.NPCs.ExoMechs.Artemis;
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
        /// How long Artemis and Apollo spend redirecting in anticipation of the dash during the CloseShots attack.
        /// </summary>
        public static int CloseShots_RedirectTime => Variables.GetAIInt("CloseShots_RedirectTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Artemis and Apollo spend slowing down in anticipation of the dash during the CloseShots attack.
        /// </summary>
        public static int CloseShots_DashSlowdownTime => Variables.GetAIInt("CloseShots_DashSlowdownTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Artemis and Apollo spend dashing at the player at maximum during the CloseShots attack.
        /// </summary>
        public static int CloseShots_MaxDashTime => Variables.GetAIInt("CloseShots_MaxDashTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How long Artemis and Apollo spend shooting at the player during the CloseShots attack.
        /// </summary>
        public static int CloseShots_ShootAtPlayerTime => Variables.GetAIInt("CloseShots_ShootAtPlayerTime", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The rate at which Artemis releases lasers during the CloseShots attack.
        /// </summary>
        public static int CloseShots_ArtemisShootRate => Variables.GetAIInt("CloseShots_ArtemisShootRate", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The rate at which Apollo releases fireballs during the CloseShots attack.
        /// </summary>
        public static int CloseShots_ApolloShootRate => Variables.GetAIInt("CloseShots_ApolloShootRate", ExoMechAIVariableType.Twins);

        /// <summary>
        /// The amount of attack cycles performed during the CloseShots attack.
        /// </summary>
        public static int CloseShots_AttackCycleCount => Variables.GetAIInt("CloseShots_AttackCycleCount", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How fast Artemis and Apollo dash during the CloseShots attack.
        /// </summary>
        public static float CloseShots_DashSpeed => Variables.GetAIFloat("CloseShots_DashSpeed", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How fast Artemis' shot lasers are during the CloseShots attack.
        /// </summary>
        public static float CloseShots_ArtemisLaserShootSpeed => Variables.GetAIFloat("CloseShots_ArtemisLaserShootSpeed", ExoMechAIVariableType.Twins);

        /// <summary>
        /// How fast Apollo's shot fireballs are during the CloseShots attack.
        /// </summary>
        public static float CloseShots_ApolloFireballShootSpeed => Variables.GetAIFloat("CloseShots_ApolloFireballShootSpeed", ExoMechAIVariableType.Twins);

        /// <summary>
        /// AI update loop method for the CloseShots attack.
        /// </summary>
        /// <param name="npc">The Exo Twin's NPC instance.</param>
        /// <param name="twinAttributes">The Exo Twin's designated generic attributes.</param>
        public static void DoBehavior_CloseShots(NPC npc, IExoTwin twinAttributes)
        {
            bool isApollo = npc.type == ExoMechNPCIDs.ApolloID;

            int redirectTime = CloseShots_RedirectTime;
            int dashSlowdownTime = CloseShots_DashSlowdownTime;
            int maxDashTime = CloseShots_MaxDashTime;
            int attackCycleDuration = redirectTime + dashSlowdownTime + maxDashTime + CloseShots_ShootAtPlayerTime;
            int wrappedTimer = AITimer % attackCycleDuration;
            ref float idealRotation = ref npc.ai[2];
            ref float spinDirection = ref npc.ai[3];

            if (AITimer >= attackCycleDuration * CloseShots_AttackCycleCount)
            {
                ExoTwinsStateManager.TransitionToNextState();
                return;
            }

            if (wrappedTimer <= redirectTime)
            {
                float redirectInterpolant = (wrappedTimer / (float)redirectTime).Squared() * 0.09f + 0.01f;
                npc.Center = Vector2.Lerp(npc.Center, Target.Center + new Vector2(isApollo.ToDirectionInt() * 540f, -240f), redirectInterpolant);
                npc.velocity.X *= 0.9f;
                npc.velocity.Y -= 0.5f;
                npc.velocity.Y *= 1.01f;
                npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center), 0.2f);
            }

            else if (wrappedTimer <= redirectTime + dashSlowdownTime)
            {
                npc.velocity.Y *= 0.77f;
                npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(Target.Center + Vector2.UnitY * 200f), 0.18f);
            }

            else if (wrappedTimer == redirectTime + dashSlowdownTime + 1)
            {
                ScreenShakeSystem.StartShakeAtPoint(npc.Center, 5f);
                SoundEngine.PlaySound(Artemis.ChargeSound with { MaxInstances = 2 }, npc.Center);

                npc.velocity = npc.rotation.ToRotationVector2() * CloseShots_DashSpeed;
                npc.netUpdate = true;
            }

            else if (wrappedTimer <= redirectTime + dashSlowdownTime + maxDashTime)
            {
                bool hasMovedPastTarget = Vector2.Dot(npc.velocity, npc.SafeDirectionTo(Target.Center)) < 0f;
                if (hasMovedPastTarget && !npc.WithinRange(Target.Center, 200f))
                    AITimer += redirectTime + dashSlowdownTime + maxDashTime - wrappedTimer + 1;

                npc.velocity.Y -= 5f;
                npc.rotation = npc.velocity.ToRotation();
                npc.damage = npc.defDamage;

                twinAttributes.ThrusterBoost = 1.6f;
                twinAttributes.WingtipVorticesOpacity = 1f;
            }

            else
            {
                if (wrappedTimer == redirectTime + dashSlowdownTime + maxDashTime + 1)
                {
                    idealRotation = npc.AngleTo(Target.Center);
                    spinDirection = (Target.Center.X - npc.Center.X).NonZeroSign();
                    twinAttributes.ThrusterBoost *= 0.4f;
                    twinAttributes.WingtipVorticesOpacity *= 0.6f;
                    npc.netUpdate = true;
                }

                idealRotation += MathF.Sin(MathHelper.TwoPi * wrappedTimer * spinDirection / 70f) * 0.08f;
                idealRotation = idealRotation.AngleLerp(npc.AngleTo(Target.Center + Target.velocity * 24f), 0.1f);
                npc.rotation = npc.rotation.AngleLerp(MathHelper.WrapAngle(idealRotation), 0.25f);
                npc.velocity *= 0.825f;
                npc.Center = Vector2.Lerp(npc.Center, Target.Center - Vector2.UnitX * isApollo.ToDirectionInt() * 150f, 0.02f);

                bool canShoot = wrappedTimer >= redirectTime + dashSlowdownTime + maxDashTime + 30;
                if (!npc.WithinRange(Target.Center, 150f) && canShoot)
                {
                    if (isApollo && AITimer % CloseShots_ApolloShootRate == CloseShots_ApolloShootRate - 1)
                    {
                        SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaShootSound, npc.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 aimDirection = npc.rotation.ToRotationVector2();
                            Vector2 fireballSpawnPosition = npc.Center + aimDirection * 70f;
                            Vector2 fireballShootVelocity = aimDirection * CloseShots_ApolloFireballShootSpeed;
                            Utilities.NewProjectileBetter(npc.GetSource_FromAI(), fireballSpawnPosition, fireballShootVelocity, ModContent.ProjectileType<ApolloPlasmaFireball>(), BasicShotDamage, 0f, -1, Target.Center.X, Target.Center.Y, 1f);
                        }
                    }

                    if (!isApollo && AITimer % CloseShots_ArtemisShootRate == CloseShots_ArtemisShootRate - 1)
                        ShootArtemisLaser(npc, CloseShots_ArtemisLaserShootSpeed);
                }
            }

            twinAttributes.Animation = ExoTwinAnimation.Attacking;
            twinAttributes.Frame = twinAttributes.Animation.CalculateFrame(wrappedTimer / 40f % 1f, twinAttributes.InPhase2);
        }
    }
}
