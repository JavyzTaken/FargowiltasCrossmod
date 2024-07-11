using CalamityMod.NPCs.ExoMechs.Apollo;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades
{
    public sealed partial class HadesHeadEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The amount of damage missiles from Hades do.
        /// </summary>
        public static int MissileDamage => Main.expertMode ? 300 : 200;

        /// <summary>
        /// The maximum amount of time Hades can spend redirecting during his Missile Lunges attack.
        /// </summary>
        public static int MissileLunges_RedirectMaxTime => LumUtils.SecondsToFrames(4f);

        /// <summary>
        /// The maximum amount of time Hades spends lunging during his Missile Lunges attack.
        /// </summary>
        public static int MissileLunges_LungeDuration => LumUtils.SecondsToFrames(1.75f);

        /// <summary>
        /// How many lunges Hades should perform during his Missile Lunges attack.
        /// </summary>
        public static int MissileLunges_LungeCount => 3;

        /// <summary>
        /// The horizontal direction in which Hades should lunge during his Missile Lunges attack.
        /// </summary>
        public ref float MissileLunges_LungeDirection => ref NPC.ai[0];

        /// <summary>
        /// How many lunges Hades has performed so far in his Missile Lunges attack.
        /// </summary>
        public ref float MissileLunges_LungeCounter => ref NPC.ai[1];

        /// <summary>
        /// The sound played when Hades charges up for a dash.
        /// </summary>
        public static readonly SoundStyle DashChargeUpSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/DashChargeUp") with { Volume = 1.7f };

        /// AI update loop method for the MineBarrages attack.
        /// </summary>
        public void DoBehavior_MissileLunges()
        {
            BodyBehaviorAction = new(AllSegments(), DoBehavior_MissileLunges_ReleaseMissile);

            if (AITimer == 1)
            {
                MissileLunges_LungeDirection = NPC.HorizontalDirectionTo(Target.Center);
                NPC.netUpdate = true;
            }

            if (AITimer < MissileLunges_RedirectMaxTime)
            {
                Vector2 hoverDestination = Target.Center + new Vector2(MissileLunges_LungeDirection * 1700f, 3600f + Target.velocity.Y * 75f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(hoverDestination) * 120f, 0.2f);

                if (NPC.WithinRange(hoverDestination, 240f))
                {
                    AITimer = MissileLunges_RedirectMaxTime;
                    NPC.netUpdate = true;
                }
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }

            else if (AITimer <= MissileLunges_RedirectMaxTime + MissileLunges_LungeDuration)
            {
                if (AITimer == MissileLunges_RedirectMaxTime + 4)
                {
                    SoundEngine.PlaySound(DashChargeUpSound);
                    ScreenShakeSystem.StartShake(13f);
                }

                NPC.velocity *= 0.5f;

                if (AITimer >= MissileLunges_RedirectMaxTime + 13)
                    JawRotation = MathHelper.Lerp(JawRotation, 0.93f, 0.1f);

                float animationCompletion = LumUtils.InverseLerp(0f, MissileLunges_LungeDuration, AITimer - MissileLunges_RedirectMaxTime);
                animationCompletion = MathHelper.Lerp(animationCompletion, 0.5f, 0.25f);

                float height = MathHelper.Lerp(3900f, -600f, MathF.Pow(LumUtils.Convert01To010(animationCompletion) + 0.001f, 0.7f));
                Vector2 idealPosition = Target.Center + new Vector2(MathHelper.Lerp(1700f, -1700f, animationCompletion) * MissileLunges_LungeDirection, height);

                Vector2 oldPosition = NPC.Center;
                NPC.Center = Vector2.Lerp(NPC.Center, idealPosition, 0.9f);

                NPC.rotation = oldPosition.AngleTo(NPC.Center) + MathHelper.PiOver2;

                SegmentOpenInterpolant = LumUtils.Saturate(SegmentOpenInterpolant + 0.1f);
            }
            else
            {
                AITimer = 0;
                MissileLunges_LungeCounter++;
                if (MissileLunges_LungeCounter >= MissileLunges_LungeCount)
                    SelectNewState();

                NPC.netUpdate = true;
            }

            NPC.damage = 0;
        }

        public void DoBehavior_MissileLunges_ReleaseMissile(HadesBodyEternity behaviorOverride)
        {
            NPC segment = behaviorOverride.NPC;
            segment.damage = 0;

            bool properTimeForMissileRelease = AITimer >= MissileLunges_RedirectMaxTime && AITimer <= MissileLunges_RedirectMaxTime + MissileLunges_LungeDuration;
            bool canReleaseMissiles = properTimeForMissileRelease && segment.Center.Y <= Target.Center.Y + 1100f && behaviorOverride.RelativeIndex % 2 == 0;
            if (canReleaseMissiles && behaviorOverride.GenericCountdown <= 0 && AITimer % 24 == behaviorOverride.RelativeIndex % 24 && behaviorOverride.SegmentOpenInterpolant >= 0.8f)
            {
                SoundEngine.PlaySound(Apollo.MissileLaunchSound with { Volume = 0.4f, MaxInstances = 0 }, segment.Center);
                ScreenShakeSystem.StartShakeAtPoint(behaviorOverride.TurretPosition, 4f);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 missileVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * 90f;
                    LumUtils.NewProjectileBetter(segment.GetSource_FromAI(), behaviorOverride.TurretPosition, missileVelocity, ModContent.ProjectileType<HadesMissile>(), MissileDamage, 0f);

                    behaviorOverride.GenericCountdown = 120;
                    segment.netUpdate = true;
                }
            }

            if (!canReleaseMissiles)
                CloseSegment().Invoke(behaviorOverride);
            else
                OpenSegment().Invoke(behaviorOverride);
        }
    }
}
