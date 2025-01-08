using CalamityMod;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
{
    public sealed partial class DraedonEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// The monologue that Draedon uses upon an Exo Mech being defeated.
        /// </summary>
        public static readonly DraedonDialogueChain FirstInterjection = new DraedonDialogueChain().
            Add("Interjection1").
            Add(() => DraedonDialogueManager.Dialogue[SelectDamageInterjectionText()]).
            Add("Interjection3").
            Add("Interjection4").
            Add("Interjection5").
            Add("Interjection6");

        /// <summary>
        /// How much damage needs to be incurred by a given <see cref="ExoMechDamageSource"/> in order for the damage to be considered major.
        /// </summary>
        public const int MajorDamageThreshold = 1000;

        /// <summary>
        /// The life ratio players have to be in order for their wounds to be considered near-lethal for the purposes of Draedon's dialogue.
        /// </summary>
        public const float NearLethalDamageLifeRatio = 0.33f;

        /// <summary>
        /// The AI method that makes Draedon speak to the player after an Exo Mech has been defeated.
        /// </summary>
        public void DoBehavior_FirstInterjection()
        {
            int speakTimer = (int)AITimer - 150;
            var dialogue = FirstInterjection;
            dialogue.Process(speakTimer, out DraedonDialogue? currentLine, out int relativeTimer);

            HologramOverlayInterpolant = 0f;

            Vector2 hoverDestination = PlayerToFollow.Center + new Vector2((PlayerToFollow.Center.X - NPC.Center.X).NonZeroSign() * -450f, -85f);
            NPC.SmoothFlyNear(hoverDestination, 0.05f, 0.94f);

            // Reset the variables to their controls by healing the player.
            if (currentLine == DraedonDialogueManager.Dialogue["Interjection4"] && relativeTimer == currentLine.Duration - 60)
                ResetPlayerFightVariables();

            if (currentLine == DraedonDialogueManager.Dialogue["Interjection6"] && relativeTimer == currentLine.Duration - 60 && !DraedonDialogueManager.UseSubtitles)
            {
                ScreenShakeSystem.StartShake(6f);
                SoundEngine.PlaySound(CalamityMod.NPCs.ExoMechs.Draedon.LaughSound);
            }

            if (dialogue.Finished(speakTimer))
            {
                AIState = DraedonAIState.MoveAroundDuringBattle;
                AITimer = 0f;
                NPC.netUpdate = true;
            }

            PerformStandardFraming();
        }

        /// <summary>
        /// "Resets the variables to their controls", as Draedon puts it. This heals the player and resets their rage, adrenaline, and mana.
        /// </summary>
        public static void ResetPlayerFightVariables()
        {
            if (Main.LocalPlayer.statLife < Main.LocalPlayer.statLifeMax2)
                Main.LocalPlayer.Heal(Main.LocalPlayer.statLifeMax2 - Main.LocalPlayer.statLife);
            Main.LocalPlayer.statMana = Main.LocalPlayer.statManaMax2;
            Main.LocalPlayer.ClearBuff(BuffID.PotionSickness);
            Main.LocalPlayer.Calamity().rage = 0f;
            Main.LocalPlayer.Calamity().adrenaline = 0f;

            ScreenShakeSystem.StartShake(3f);
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/OrbHeal", 5) with { Volume = 0.9f });
        }

        /// <summary>
        /// Selects interjection text based on whatever the player took the most damage from.
        /// </summary>
        public static string SelectDamageInterjectionText()
        {
            Player closest = Main.player[Player.FindClosest(new Vector2(Main.maxTilesX * 0.5f, (float)Main.worldSurface) * 16f, 1, 1)];

            if (!closest.TryGetModPlayer(out ExoMechDamageRecorderPlayer recorderPlayer))
                return "Error";

            ExoMechDamageSource source = recorderPlayer.MostDamagingSource;
            int damageIncurred = recorderPlayer.GetDamageBySource(source);
            float playerLifeRatio = closest.statLife / (float)closest.statLifeMax2;

            string typePrefix = source.ToString();
            string damagePrefix = "Minor";

            bool majorDamage = damageIncurred >= MajorDamageThreshold;
            bool nearLethalDamage = majorDamage && playerLifeRatio <= NearLethalDamageLifeRatio;

            if (majorDamage)
                damagePrefix = "Major";
            if (nearLethalDamage)
                damagePrefix = "NearLethal";

            if (damageIncurred <= 0)
                return "Interjection2_Undamaged";

            return $"Interjection2_{typePrefix}_{damagePrefix}";
        }
    }
}
