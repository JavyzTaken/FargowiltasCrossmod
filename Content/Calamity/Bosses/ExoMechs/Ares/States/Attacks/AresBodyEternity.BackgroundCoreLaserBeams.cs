using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    public sealed partial class AresBodyEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The sound responsible for the laser sound loop.
        /// </summary>
        public LoopedSoundInstance ExoOverloadLoopedSound;

        /// <summary>
        /// How much damage missiles from Ares' core do.
        /// </summary>
        public static int MissileDamage => Variables.GetAIInt("MissileDamage", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How much damage laserbeams from Ares' core do.
        /// </summary>
        public static int CoreLaserbeamDamage => Variables.GetAIInt("CoreLaserbeamDamage", ExoMechAIVariableType.Ares);

        /// <summary>
        /// The rate at which Ares releases missiles during the Background Core Laserbeams attack.
        /// </summary>
        public static int BackgroundCoreLaserBeams_MissileReleaseRate => Variables.GetAIInt("BackgroundCoreLaserBeams_MissileReleaseRate", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long Ares waits before starting the looped blender sound during the Background Core Laserbeams attack.
        /// </summary>
        public static int BackgroundCoreLaserBeams_LoopSoundDelay => Variables.GetAIInt("BackgroundCoreLaserBeams_LoopSoundDelay", ExoMechAIVariableType.Ares);

        /// <summary>
        /// How long Ares waits before releasing missiles during the Background Core Laserbeams attack.
        /// </summary>
        public static int BackgroundCoreLaserBeams_MissileShootDelay => Variables.GetAIInt("BackgroundCoreLaserBeams_MissileShootDelay", ExoMechAIVariableType.Ares);

        /// <summary>
        /// The sound played when a distant missile is fired by Ares.
        /// </summary>
        public static readonly SoundStyle DistantMissileLaunchSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/DistantMissileLaunch") with { MaxInstances = 0, Volume = 0.6f, PitchVariance = 0.15f };

        /// <summary>
        /// AI update loop method for the Background Core Laserbeams attack.
        /// </summary>
        public void DoBehavior_BackgroundCoreLaserBeams()
        {
            float enterBackgroundInterpolant = Utilities.InverseLerp(0f, 30f, AITimer);
            float slowDownInterpolant = Utilities.InverseLerp(54f, 60f, AITimer);
            bool doneAttacking = AITimer >= ExoOverloadDeathray.Lifetime;
            NPC.Center = Vector2.Lerp(NPC.Center, Target.Center - Vector2.UnitY * enterBackgroundInterpolant * 360f, enterBackgroundInterpolant * (1f - slowDownInterpolant) * 0.08f);
            NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(Target.Center.X, NPC.Center.Y), slowDownInterpolant * 0.111f);
            NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(NPC.Center.X, Target.Center.Y - 70f), slowDownInterpolant * 0.019f);
            NPC.Center = NPC.Center.MoveTowards(new Vector2(NPC.Center.X, Target.Center.Y - 70f), slowDownInterpolant * 1.7f);
            NPC.velocity *= 0.93f;

            if (AITimer == 1)
            {
                ScreenShakeSystem.StartShake(10f);
                SoundEngine.PlaySound(AresBody.LaserStartSound);
                SoundEngine.PlaySound(LaughSound with { Volume = 10f });
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Utilities.NewProjectileBetter(NPC.GetSource_FromAI(), CorePosition, Vector2.Zero, ModContent.ProjectileType<ExoOverloadDeathray>(), CoreLaserbeamDamage, 0f, -1);
            }

            if (AITimer == BackgroundCoreLaserBeams_LoopSoundDelay)
                ExoOverloadLoopedSound = LoopedSoundManager.CreateNew(AresBody.LaserLoopSound, () => CurrentState != AresAIState.BackgroundCoreLaserBeams || !NPC.active);

            var deathrays = Utilities.AllProjectilesByID(ModContent.ProjectileType<ExoOverloadDeathray>());
            if (deathrays.Any())
            {
                Vector3 direction = Vector3.Transform(Vector3.UnitX, deathrays.First().As<ExoOverloadDeathray>().Rotation);
                ExoOverloadLoopedSound?.Update(Target.Center, sound =>
                {
                    sound.Volume = Utilities.InverseLerp(0.8f, 0.42f, MathF.Abs(direction.X)) * 1.1f + 0.56f;
                });
            }

            if (!doneAttacking && AITimer >= BackgroundCoreLaserBeams_MissileShootDelay && AITimer % BackgroundCoreLaserBeams_MissileReleaseRate == 0)
            {
                Vector2 velocity = NPC.position - NPC.oldPosition;
                Vector2 sparkSpawnPosition = NPC.Center - Vector2.UnitY.RotatedByRandom(1.1f) * NPC.scale * 70f;
                StrongBloom sparkle = new(sparkSpawnPosition, velocity, Color.Wheat, 0.15f, 10);
                GeneralParticleHandler.SpawnParticle(sparkle);

                SoundEngine.PlaySound(DistantMissileLaunchSound);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 missileSpawnPosition = Target.Center + new Vector2(Main.rand.NextFloatDirection() * 1100f, -740f);
                    if (Main.rand.NextBool(3))
                        missileSpawnPosition.X = Target.Center.X + Main.rand.NextFloatDirection() * 50f + Target.velocity.X * Main.rand.NextFloat(8f, 32f);

                    if (MathHelper.Distance(missileSpawnPosition.X, NPC.Center.X) >= 240f)
                    {
                        Utilities.NewProjectileBetter(NPC.GetSource_FromAI(), missileSpawnPosition, Vector2.UnitY * 3f, ModContent.ProjectileType<AresMissile>(), MissileDamage, 0f, -1, Target.Bottom.Y);

                        Vector2 backgroundMissileVelocity = NPC.SafeDirectionTo(sparkSpawnPosition).RotatedByRandom(0.2f) * Main.rand.NextFloat(16f, 27f);
                        backgroundMissileVelocity.X *= 0.25f;
                        Utilities.NewProjectileBetter(NPC.GetSource_FromAI(), sparkSpawnPosition, backgroundMissileVelocity, ModContent.ProjectileType<AresMissileBackground>(), 0, 0f);
                    }
                }
            }

            InstructionsForHands[0] = new(h =>
            {
                BasicHandUpdate(h, new Vector2(-430f, 50f), 0);
                h.NPC.dontTakeDamage = true;
            });
            InstructionsForHands[1] = new(h =>
            {
                BasicHandUpdate(h, new Vector2(-280f, 224f), 1);
                h.NPC.dontTakeDamage = true;
            });
            InstructionsForHands[2] = new(h =>
            {
                BasicHandUpdate(h, new Vector2(280f, 224f), 2);
                h.NPC.dontTakeDamage = true;
            });
            InstructionsForHands[3] = new(h =>
            {
                BasicHandUpdate(h, new Vector2(430f, 50f), 3);
                h.NPC.dontTakeDamage = true;
            });

            if (AITimer >= ExoOverloadDeathray.Lifetime)
                ZPosition = MathHelper.Clamp(ZPosition - 0.5f, 0f, 10f);
            else
                ZPosition = enterBackgroundInterpolant * 3.7f;

            if (AITimer >= ExoOverloadDeathray.Lifetime + 15)
                SelectNewState();
        }
    }
}
