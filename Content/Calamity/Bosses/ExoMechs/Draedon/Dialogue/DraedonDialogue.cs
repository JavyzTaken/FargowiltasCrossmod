namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue
{
    public class DraedonDialogue(string localizationKey, DraedonSubtitle subtitle, DraedonChatTextData text)
    {
        /// <summary>
        /// The localization key of this dialogue.
        /// </summary>
        public string LocalizationKey
        {
            get;
            private set;
        } = localizationKey;

        /// <summary>
        /// The subtitle option of this dialogue.
        /// </summary>
        public DraedonSubtitle Subtitle
        {
            get;
            set;
        } = subtitle;

        /// <summary>
        /// The chat text option of this dialogue.
        /// </summary>
        public DraedonChatTextData Text
        {
            get;
            set;
        } = text;

        /// <summary>
        /// How much time, in frames, this dialogue instance needs to fully play.
        /// </summary>
        public int Duration => DraedonDialogueManager.UseSubtitles ? Subtitle.Duration : Text.Duration;
    }
}
