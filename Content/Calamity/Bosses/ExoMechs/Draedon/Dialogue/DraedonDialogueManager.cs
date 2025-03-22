using FargowiltasCrossmod.Core.Calamity;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue
{
    public class DraedonDialogueManager : ModSystem
    {
        /// <summary>
        /// The set of all dialogue registered.
        /// </summary>
        public static readonly Dictionary<string, DraedonDialogue> Dialogue = [];

        /// <summary>
        /// Whether subtitles should be used over chat-based text.
        /// </summary>
        public static bool UseSubtitles => false; // CalDLCConfig.Instance.VoiceActingEnabled;

        public override void PostSetupContent()
        {
            Color edgyTextColor = CalamityMod.NPCs.ExoMechs.Draedon.TextColorEdgy;

            // TODO -- Autoload, then add timings and stuff afterwards on a case by case basis, rather than doing this manually.
            GenerateNew("IntroductionMonologue1");
            GenerateNew("IntroductionMonologue2");
            GenerateNew("IntroductionMonologue3");
            GenerateNew("IntroductionMonologue4");
            GenerateNew("IntroductionMonologue5", edgyTextColor, 1);
            GenerateNew("IntroductionMonologueBrief", edgyTextColor, 1);

            GenerateNew("ExoMechChoiceResponse1");
            GenerateNew("ExoMechChoiceResponse2", edgyTextColor, 1);

            GenerateNew("Interjection1");
            GenerateNew("Interjection2_BluntForceTrauma_Minor");
            GenerateNew("Interjection2_BluntForceTrauma_Major");
            GenerateNew("Interjection2_BluntForceTrauma_NearLethal");
            GenerateNew("Interjection2_Electricity_Minor");
            GenerateNew("Interjection2_Electricity_Major");
            GenerateNew("Interjection2_Electricity_NearLethal");
            GenerateNew("Interjection2_Internal_Minor");
            GenerateNew("Interjection2_Internal_Major");
            GenerateNew("Interjection2_Internal_NearLethal");
            GenerateNew("Interjection2_Plasma_Minor");
            GenerateNew("Interjection2_Plasma_Major");
            GenerateNew("Interjection2_Plasma_NearLethal");
            GenerateNew("Interjection2_Thermal_Minor");
            GenerateNew("Interjection2_Thermal_Major");
            GenerateNew("Interjection2_Thermal_NearLethal");
            GenerateNew("Interjection2_Undamaged");
            GenerateNew("Interjection3");
            GenerateNew("Interjection4");
            GenerateNew("Interjection5");
            GenerateNew("Interjection6");

            GenerateNew("Interjection7");
            GenerateNew("Interjection8");
            GenerateNew("Interjection9");
            GenerateNew("Interjection10");
            GenerateNew("Interjection10_Plural");
            GenerateNew("Interjection11", edgyTextColor, 1);

            GenerateNew("EndOfBattle_FirstDefeat1");
            GenerateNew("EndOfBattle_FirstDefeat2");
            GenerateNew("EndOfBattle_FirstDefeat3");
            GenerateNew("EndOfBattle_FirstDefeat4");
            GenerateNew("EndOfBattle_FirstDefeat5");
            GenerateNew("EndOfBattle_FirstDefeat6");
            GenerateNew("EndOfBattle_SuccessiveDefeat1");
            GenerateNew("EndOfBattle_SuccessiveDefeat2");
            GenerateNew("EndOfBattle_SuccessiveDefeat3_Perfect");
            GenerateNew("EndOfBattle_SuccessiveDefeat3_Excellent");
            GenerateNew("EndOfBattle_SuccessiveDefeat3_Good");
            GenerateNew("EndOfBattle_SuccessiveDefeat3_Acceptable");
            GenerateNew("EndOfBattle_SuccessiveDefeat3_Bad");
            GenerateNew("EndOfBattle_SuccessiveDefeat3_WhyDidYouMeltTheBoss");
            GenerateNew("EndOfBattle_SuccessiveDefeat4_Perfect");
            GenerateNew("EndOfBattle_SuccessiveDefeat4_Excellent");
            GenerateNew("EndOfBattle_SuccessiveDefeat4_ImproveFightTime");
            GenerateNew("EndOfBattle_SuccessiveDefeat4_ImproveAggression");
            GenerateNew("EndOfBattle_SuccessiveDefeat4_ImproveBuffs");
            GenerateNew("EndOfBattle_SuccessiveDefeat4_ImproveHitCounter");
            GenerateNew("EndOfBattle_SuccessiveDefeat4_Bad");
            GenerateNew("EndOfBattle_SuccessiveDefeat4_WhyDidYouMeltTheBoss");
            GenerateNew("EndOfBattle_SuccessiveDefeat5");

            GenerateNew("EndOfBattle_FirstDefeatReconBodyKill1");
            GenerateNew("EndOfBattle_FirstDefeatReconBodyKill2");
            GenerateNew("EndOfBattle_FirstDefeatReconBodyKill3");

            GenerateNew("Death", null, LumUtils.SecondsToFrames(1.85f));

            GenerateNew("PlayerDeathAtAmusingTime", null, LumUtils.SecondsToFrames(1.2f));

            GenerateNew("Error");
        }

        internal static DraedonDialogue GenerateNew(string key, Color? chatTextColorOverride = null, int? chatTextSpeakTimeOverride = null)
        {
            int chatTextSpeakTime = chatTextSpeakTimeOverride ?? DraedonEternity.StandardSpeakTime;
            Color chatTextColor = chatTextColorOverride ?? CalamityMod.NPCs.ExoMechs.Draedon.TextColor;

            string localizationKey = $"Mods.FargowiltasCrossmod.NPCs.Draedon.{key}";
            DraedonDialogue dialogue = new(localizationKey, new DraedonSubtitle(SoundPath(key)), new DraedonChatTextData(chatTextColor, chatTextSpeakTime));
            Dialogue.Add(key, dialogue);

            return dialogue;
        }

        internal static string SoundPath(string relativePath) =>
            $"Assets/Sounds/ExoMechs/VoiceActing/Drae_{relativePath}.wav";
    }
}
