using CalamityMod;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    public sealed partial class DraedonEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The monologue that Draedon uses upon the Exo Mechs battle concluding the first time you defeat them.
        /// </summary>
        public static readonly DraedonDialogueChain PostBattleInterjection = new DraedonDialogueChain("Mods.FargowiltasCrossmod.NPCs.Draedon.").
            Add("EndOfBattle_FirstDefeat1").
            Add("EndOfBattle_FirstDefeat2").
            Add("EndOfBattle_FirstDefeat3").
            Add("EndOfBattle_FirstDefeat4").
            Add("EndOfBattle_FirstDefeat5").
            Add("EndOfBattle_FirstDefeat6");

        /// <summary>
        /// The monologue that Draedon uses upon the Exo Mechs battle concluding the first time you defeat them.
        /// </summary>
        public static readonly DraedonDialogueChain PostBattleAnalysisInterjection = new DraedonDialogueChain("Mods.FargowiltasCrossmod.NPCs.Draedon.").
            Add("EndOfBattle_SuccessiveDefeat1").
            Add("EndOfBattle_SuccessiveDefeat2").
            Add(EndOfBattle_SuccessiveDefeat3Selection).
            Add(EndOfBattle_SuccessiveDefeat4Selection).
            Add("EndOfBattle_SuccessiveDefeat5");

        /// <summary>
        /// How many style points the player must have in order for Draedon to recognize a battle as perfect.
        /// </summary>
        public static float StylePoints_Perfection => 0.99f;

        /// <summary>
        /// How many style points the player must have in order for Draedon to recognize a battle as excellent.
        /// </summary>
        public static float StylePoints_Excellent => 0.93f;

        /// <summary>
        /// How many style points the player must have in order for Draedon to recognize a battle as good.
        /// </summary>
        public static float StylePoints_Good => 0.77f;

        /// <summary>
        /// How many style points the player must have in order for Draedon to recognize a battle as acceptable.
        /// </summary>
        public static float StylePoints_Acceptable => 0.56f;

        /// <summary>
        /// The AI method that makes Draedon speak to the player after an Exo Mech has been defeated.
        /// </summary>
        public void DoBehavior_PostBattleInterjection()
        {
            int speakTimer = (int)AITimer - 90;
            var monologue = DownedBossSystem.downedExoMechs ? PostBattleAnalysisInterjection : PostBattleInterjection;
            for (int i = 0; i < monologue.Count; i++)
            {
                if (speakTimer == monologue[i].SpeakDelay)
                    monologue[i].SayInChat();
            }

            Vector2 hoverDestination = PlayerToFollow.Center + new Vector2((PlayerToFollow.Center.X - NPC.Center.X).NonZeroSign() * -450f, -5f);
            NPC.SmoothFlyNear(hoverDestination, 0.05f, 0.94f);
            NPC.dontTakeDamage = WasKilled;

            bool monologueIsFinished = speakTimer >= monologue.OverallDuration;

            // Reset the variables to their controls by healing the player.
            if (Main.netMode != NetmodeID.MultiplayerClient && speakTimer == monologue[3].SpeakDelay - 60)
            {
                foreach (Player player in Main.ActivePlayers)
                {
                    if (player.WithinRange(NPC.Center, 6700f))
                        Utilities.NewProjectileBetter(NPC.GetSource_FromAI(), player.Center - Vector2.UnitY * 800f, Vector2.Zero, ModContent.ProjectileType<DraedonLootCrate>(), 0, 0f, player.whoAmI);
                }
            }

            if (monologueIsFinished)
            {
                HologramInterpolant = Utilities.Saturate(HologramInterpolant + 0.04f);
                MaxSkyOpacity = 1f - HologramInterpolant;
                if (HologramInterpolant >= 1f)
                    NPC.active = false;
            }
            else
                HologramInterpolant = 0f;

            PerformStandardFraming();
        }

        /// <summary>
        /// Selects interjection text based on the player's performance for the third post-battle dialogue line.
        /// </summary>
        public static string EndOfBattle_SuccessiveDefeat3Selection()
        {
            Player closest = Main.player[Player.FindClosest(new Vector2(Main.maxTilesX * 0.5f, (float)Main.worldSurface) * 16f, 1, 1)];

            if (!closest.TryGetModPlayer(out ExoMechStylePlayer stylePlayer))
                return "Error";

            float style = stylePlayer.Style;
            if (stylePlayer.PlayerIsMeltingBoss)
                return "EndOfBattle_SuccessiveDefeat3_WhyDidYouMeltTheBoss";
            if (style >= StylePoints_Perfection)
                return "EndOfBattle_SuccessiveDefeat3_Perfect";
            if (style >= StylePoints_Excellent)
                return "EndOfBattle_SuccessiveDefeat3_Excellent";
            if (style >= StylePoints_Good)
                return "EndOfBattle_SuccessiveDefeat3_Good";
            if (style >= StylePoints_Acceptable)
                return "EndOfBattle_SuccessiveDefeat3_Acceptable";

            return "EndOfBattle_SuccessiveDefeat3_Bad";
        }

        /// <summary>
        /// Selects interjection text based on the player's performance for the fourth post-battle dialogue line.
        /// </summary>
        public static string EndOfBattle_SuccessiveDefeat4Selection()
        {
            Player closest = Main.player[Player.FindClosest(new Vector2(Main.maxTilesX * 0.5f, (float)Main.worldSurface) * 16f, 1, 1)];

            if (!closest.TryGetModPlayer(out ExoMechStylePlayer stylePlayer))
                return "Error";

            float style = stylePlayer.Style;
            if (stylePlayer.PlayerIsMeltingBoss)
                return "EndOfBattle_SuccessiveDefeat4_WhyDidYouMeltTheBoss";
            if (style >= StylePoints_Perfection)
                return "EndOfBattle_SuccessiveDefeat4_Perfect";
            if (style >= StylePoints_Excellent)
                return "EndOfBattle_SuccessiveDefeat4_Excellent";
            if (style >= StylePoints_Acceptable)
            {
                float[] weights = [stylePlayer.HitsWeight, stylePlayer.BuffsWeight, stylePlayer.FightTimeWeight, stylePlayer.AggressivenessWeight];
                float weakestWeight = weights.Min();
                int weakestWeightIndex = Array.IndexOf(weights, weakestWeight);

                return weakestWeightIndex switch
                {
                    0 => "EndOfBattle_SuccessiveDefeat4_ImproveHitCounter",
                    1 => "EndOfBattle_SuccessiveDefeat4_ImproveBuffs",
                    2 => "EndOfBattle_SuccessiveDefeat4_ImproveFightTime",
                    3 => "EndOfBattle_SuccessiveDefeat4_ImproveAggression",
                    _ => "Error",
                };
            }

            return "EndOfBattle_SuccessiveDefeat4_Bad";
        }
    }
}
