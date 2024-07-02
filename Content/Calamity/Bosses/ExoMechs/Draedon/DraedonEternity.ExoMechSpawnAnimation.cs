﻿using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.World;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    public sealed partial class DraedonEternity : CalDLCEmodeBehavior
    {
        public LoopedSoundInstance SirenSoundInstance
        {
            get;
            set;
        }

        /// <summary>
        /// How long it takes for the siren blare to appear.
        /// </summary>
        public static int SirenDelay => Utilities.SecondsToFrames(1.5f);

        /// <summary>
        /// How long the siren blare spends fading in.
        /// </summary>
        public static int SirenFadeInTime => Utilities.SecondsToFrames(0.9f);

        /// <summary>
        /// How long Draedon waits before summoning the first Exo Mech.
        /// </summary>
        public static int ExoMechSummonDelay => Utilities.SecondsToFrames(5f);

        /// <summary>
        /// How long the cargo plane spends flying overhead.
        /// </summary>
        public static int ExoMechPlaneFlyTime => Utilities.SecondsToFrames(1.2f);

        /// <summary>
        /// The siren sound played as the Exo Mechs wait to be summoned.
        /// </summary>
        public static readonly SoundStyle SirenSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/GeneralExoMechs/ExoMechSiren");

        /// <summary>
        /// The AI method that makes Draedon handle the Exo Mech spawning.
        /// </summary>
        public void DoBehavior_ExoMechSpawnAnimation()
        {
            if (AITimer == 1f)
                CalamityUtils.DisplayLocalizedText("Mods.FargowiltasCrossmod.NPCs.Draedon.ExoMechChoiceResponse1", CalamityMod.NPCs.ExoMechs.Draedon.TextColor);
            if (AITimer == SirenDelay)
                CalamityUtils.DisplayLocalizedText("Mods.FargowiltasCrossmod.NPCs.Draedon.ExoMechChoiceResponse2", CalamityMod.NPCs.ExoMechs.Draedon.TextColorEdgy);

            PerformStandardFraming();

            if (AITimer >= SirenDelay)
            {
                Main.numCloudsTemp = (int)Utils.Remap(MaxSkyOpacity, 0f, 1f, Main.numCloudsTemp, Main.maxClouds);

                Vector2 hoverDestination = PlayerToFollow.Center + PlayerToFollow.SafeDirectionTo(NPC.Center) * new Vector2(820f, 560f);
                NPC.SmoothFlyNearWithSlowdownRadius(hoverDestination, 0.06f, 0.9f, 60f);

                Main.windSpeedTarget = 1.1f;
            }

            SirenSoundInstance ??= LoopedSoundManager.CreateNew(SirenSound, () =>
            {
                return !NPC.active || AIState != DraedonAIState.ExoMechSpawnAnimation || AITimer >= ExoMechPlaneFlyTime + ExoMechSummonDelay + 30f;
            });
            SirenSoundInstance.Update(Main.LocalPlayer.Center, sound =>
            {
                sound.Volume = Utilities.InverseLerp(0f, SirenFadeInTime, AITimer) * Utilities.InverseLerp(30f, 0f, AITimer - ExoMechPlaneFlyTime - ExoMechSummonDelay);
            });

            MaxSkyOpacity = Utilities.Saturate(MaxSkyOpacity + 0.05f);
            PlaneFlyForwardInterpolant = Utilities.InverseLerp(0f, ExoMechPlaneFlyTime, AITimer - ExoMechSummonDelay);
            CustomExoMechsSky.RedSirensIntensity = MathF.Pow(Utilities.Sin01(MathHelper.TwoPi * (AITimer - SirenDelay) / 240f), 0.7f) * (1f - PlaneFlyForwardInterpolant) * 0.7f;

            if (PlaneFlyForwardInterpolant >= 1f)
            {
                ChangeAIState(DraedonAIState.MoveAroundDuringBattle);
                PlaneFlyForwardInterpolant = 0f;
            }

            if (AITimer == ExoMechPlaneFlyTime + ExoMechSummonDelay - 2f - (CalamityWorld.DraedonMechToSummon == ExoMech.Prime ? 12f : 0f))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 exoMechSpawnPosition = PlayerToFollow.Center - Vector2.UnitY * 1900f;
                    switch (CalamityWorld.DraedonMechToSummon)
                    {
                        case ExoMech.Destroyer:
                            CalamityUtils.SpawnBossBetter(exoMechSpawnPosition, ModContent.NPCType<ThanatosHead>());
                            break;
                        case ExoMech.Prime:
                            CalamityUtils.SpawnBossBetter(exoMechSpawnPosition + Vector2.UnitY * 1600f, ModContent.NPCType<AresBody>());
                            break;
                        case ExoMech.Twins:
                            CalamityUtils.SpawnBossBetter(exoMechSpawnPosition - Vector2.UnitX * 350f, ModContent.NPCType<Artemis>());
                            CalamityUtils.SpawnBossBetter(exoMechSpawnPosition + Vector2.UnitX * 350f, ModContent.NPCType<Apollo>());
                            break;
                    }
                }

                SoundEngine.PlaySound(Artemis.ChargeSound);
                ScreenShakeSystem.StartShake(20f);
            }
        }
    }
}
