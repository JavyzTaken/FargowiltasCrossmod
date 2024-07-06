using CalamityMod.NPCs.ExoMechs.Apollo;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Hades
{
    public sealed partial class HadesHeadEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The amount of damage mines from Hades do.
        /// </summary>
        public static int MineDamage => Main.expertMode ? 350 : 250;

        /// <summary>
        /// How many mine barrages Hades should do during his MineBarrages attack.
        /// </summary>
        public static int MineBarrages_BarrageCount => 2;

        /// <summary>
        /// How long Hades spends redirecting before releasing mines during the MineBarrages attack.
        /// </summary>
        public static int MineBarrages_RedirectTime => Utilities.SecondsToFrames(2.3f);

        /// <summary>
        /// How long Hades spends releasing mines during the MineBarrages attack.
        /// </summary>
        public static int MineBarrages_MineReleaseTime => Utilities.SecondsToFrames(2.7f);

        /// <summary>
        /// How long Hades spends releasing mines during the MineBarrages attack.
        /// </summary>
        public static int MineBarrages_AttackCycleTime => MineBarrages_RedirectTime + MineBarrages_MineReleaseTime;

        /// <summary>
        /// AI update loop method for the MineBarrages attack.
        /// </summary>
        public void DoBehavior_MineBarrages()
        {
            int wrappedTimer = AITimer % MineBarrages_AttackCycleTime;
            if (wrappedTimer < MineBarrages_RedirectTime)
            {
                if (!NPC.WithinRange(Target.Center, 600f))
                {
                    float newSpeed = MathHelper.Lerp(NPC.velocity.Length(), 34f, 0.09f);
                    Vector2 newDirection = NPC.velocity.RotateTowards(NPC.AngleTo(Target.Center), 0.03f).SafeNormalize(Vector2.UnitY);
                    NPC.velocity = newDirection * newSpeed;
                }

                BodyBehaviorAction = new(EveryNthSegment(4), OpenSegment());
            }
            else
            {
                if (!NPC.WithinRange(Target.Center, 400f))
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.SafeDirectionTo(Target.Center) * 32f, 0.03f);
                else
                    NPC.velocity *= 1.07f;

                BodyBehaviorAction = new(EveryNthSegment(2), DoBehavior_MineBarrages_FireMine);
            }

            if (AITimer >= MineBarrages_BarrageCount * MineBarrages_AttackCycleTime)
                SelectNewState();

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public void DoBehavior_MineBarrages_FireMine(HadesBodyEternity behaviorOverride)
        {
            NPC segment = behaviorOverride.NPC;

            if ((segment.whoAmI * 53 + AITimer) % 110 == 0)
            {
                Vector2 mineSpawnPosition = behaviorOverride.TurretPosition;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int time = AITimer % MineBarrages_AttackCycleTime - MineBarrages_RedirectTime - Main.rand.Next(60);
                    int mineLifetime = LumUtils.SecondsToFrames(Main.rand.NextFloat(4f, 10f));
                    float mineSpeed = Main.rand.NextFloat(30f, 175f);
                    float mineOffsetAngle = Main.rand.NextGaussian(0.14f);
                    Vector2 mineVelocity = (Target.Center - mineSpawnPosition).SafeNormalize(Vector2.UnitY).RotatedBy(mineOffsetAngle) * mineSpeed;
                    Utilities.NewProjectileBetter(segment.GetSource_FromAI(), mineSpawnPosition, mineVelocity, ModContent.ProjectileType<HadesMine>(), MineDamage, 0f, -1, mineLifetime, time);
                }

                SoundEngine.PlaySound(Apollo.MissileLaunchSound with { Volume = 0.6f, MaxInstances = 0 }, mineSpawnPosition);
                segment.netUpdate = true;
            }

            OpenSegment().Invoke(behaviorOverride);
        }
    }
}
