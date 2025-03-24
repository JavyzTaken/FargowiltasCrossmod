using CalamityMod;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.Easings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.GraphicsReplacements
{
    /// <summary>
    /// A representation of an Exo Mech selection icon.
    /// </summary>
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExoMechSelectionIcon
    {
        /// <summary>
        /// The type of Exo Mech this icon should summon.
        /// </summary>
        public ExoMech TypeToSummon
        {
            get;
            set;
        }

        /// <summary>
        /// The localization key to display upon hovering over this icon.
        /// </summary>
        public string LocalizationKey
        {
            get;
            set;
        }

        /// <summary>
        /// Whether the mouse was hovering over this icon on the previous frame.
        /// </summary>
        public bool WasMouseHoveringOverIcon
        {
            get;
            set;
        }

        /// <summary>
        /// The scale of this icon.
        /// </summary>
        public float Scale
        {
            get;
            set;
        } = 1f;

        /// <summary>
        /// The unscaled draw offset of this icon.
        /// </summary>
        public Vector2 UnscaledOffset
        {
            get;
            set;
        }

        /// <summary>
        /// The texture associated with this icon.
        /// </summary>
        public LazyAsset<Texture2D> Texture
        {
            get;
            set;
        }

        /// <summary>
        /// The hover sound that should be played when 
        /// </summary>
        public SoundStyle HoverSound
        {
            get;
            set;
        }

        /// <summary>
        /// The draw position of this icon.
        /// </summary>
        public Vector2 DrawPosition => Main.LocalPlayer.Center - Main.screenPosition + Vector2.UnitY * Main.LocalPlayer.gfxOffY + UnscaledOffset * Main.UIScale;

        public ExoMechSelectionIcon(ExoMech typeToSummon, string localizationKey, Vector2 unscaledOffset, LazyAsset<Texture2D> texture, SoundStyle hoverSound)
        {
            TypeToSummon = typeToSummon;
            LocalizationKey = localizationKey;
            UnscaledOffset = unscaledOffset;
            Texture = texture;
            HoverSound = hoverSound;
        }

        /// <summary>
        /// Updates this icon, evaluating its state based on inputs.
        /// </summary>
        public void Update()
        {
            Rectangle mouseRectangle = Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);
            Rectangle hoverRectangle = Utils.CenteredRectangle(DrawPosition, Texture.Value.Size() * Main.UIScale * Scale);
            bool currentlyHoveringOverIcon = mouseRectangle.Intersects(hoverRectangle);

            if (currentlyHoveringOverIcon != WasMouseHoveringOverIcon)
            {
                if (currentlyHoveringOverIcon)
                    SoundEngine.PlaySound(HoverSound);

                WasMouseHoveringOverIcon = currentlyHoveringOverIcon;
            }

            if (currentlyHoveringOverIcon)
            {
                Main.blockMouse = Main.LocalPlayer.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    SoundEngine.PlaySound(CalamityMod.NPCs.ExoMechs.Draedon.SelectionSound);
                    ExoMechSelectionUIReplacer.SummonExoMech(TypeToSummon);
                }
            }

            Scale = MathHelper.Clamp(Scale + (currentlyHoveringOverIcon ? 0.0375f : -0.05f), 1f, 1.2f);
        }

        /// <summary>
        /// Renders this icon.
        /// </summary>
        public void Render(float timeOffset)
        {
            float animationInterpolant = LumUtils.Saturate(ExoMechSelectionUIReplacer.GeneralScaleInterpolant + timeOffset);
            if (MathHelper.Distance(animationInterpolant, ExoMechSelectionUIReplacer.ScaleIncrement) <= 0.001f)
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/Codebreaker/DialogOptionHover") with { MaxInstances = 0 });

            float scale = Scale * MathF.Pow(EasingCurves.Elastic.Evaluate(EasingType.Out, animationInterpolant), 1.4f);
            Texture2D iconTexture = Texture.Value;
            Main.spriteBatch.Draw(iconTexture, DrawPosition, null, Color.White, 0f, iconTexture.Size() * 0.5f, scale * Main.UIScale, 0, 0f);

            if (WasMouseHoveringOverIcon)
            {
                string description = Language.GetTextValue(LocalizationKey);
                Color descriptionColor = new(155, 255, 255);
                Vector2 descriptionDrawPosition = DrawPosition + new Vector2(FontAssets.MouseText.Value.MeasureString(description).X * -0.5f, 36f) * Main.UIScale;
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, description, descriptionDrawPosition.X, descriptionDrawPosition.Y, descriptionColor, Color.Black, Vector2.Zero, Main.UIScale);
            }
        }
    }
}
