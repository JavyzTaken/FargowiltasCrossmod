using CalamityMod.UI.DraedonSummoning;
using Luminance.Common.Easings;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue.DraedonSubtitleManager;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue
{
    [Autoload(Side = ModSide.Client)]
    public class DraedonSubtitleRenderer : ModSystem
    {
        /// <summary>
        /// The 0-1 interpolant by which text is offset due to the (dis)appearance animation.
        /// </summary>
        public static float TextOffsetInterpolant
        {
            get;
            private set;
        }

        /// <summary>
        /// The render target responsible for holding the subtitle draw information.
        /// </summary>
        public static ManagedRenderTarget SubtitleRenderTarget
        {
            get;
            private set;
        }

        /// <summary>
        /// The font used for the subtitles.
        /// </summary>
        public static DynamicSpriteFont SubtitleFont
        {
            get
            {
                /*
                 * >Hey Dom uuh, could you make Dradong use standard font with Ru localization for now? Else I'm afraid all that will be displayed is asterisks once we translate his lines
                 * 
                 * >Didn't you say Cal's gonna make the font use a Cyrillic-friendly font instead for Draedon?
                 * >This references Cal, so
                 * 
                 * >Yeah but
                 * >That's gonna be added in SSO I'm afraid
                 * 
                 * 
                 * 
                 * 
                 * 
                 * 
                 * TODO -- Yeah I dunno when that's releasing at this point. They make a decent point. Remove this whenever that happens. -Lucille
                 */
                if (GameCulture.FromCultureName(GameCulture.CultureName.Russian).IsActive)
                    return FontAssets.MouseText.Value;

                return CodebreakerUI.DialogFont;
            }
        }

        public override void OnModLoad()
        {
            SubtitleRenderTarget = new(true, ManagedRenderTarget.CreateScreenSizedTarget);
            RenderTargetManager.RenderTargetUpdateLoopEvent += UpdateSubtitleRenderTarget;
        }

        private static void UpdateSubtitleRenderTarget()
        {
            if (CurrentSequence is null)
                return;

            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;

            graphicsDevice.SetRenderTarget(SubtitleRenderTarget);
            graphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            RenderSubtitlesWithoutPostProcessing();
            Main.spriteBatch.End();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Add(new LegacyGameInterfaceLayer("FargowiltasCrossmod: Draedon Subtitles", () =>
            {
                RenderSubtitlesWithPostProcessing();
                return true;
            }, InterfaceScaleType.None));
        }

        internal static void RenderSubtitlesWithoutPostProcessing()
        {
            if (CurrentSequence is null)
                return;

            EasingCurves.Curve offsetAnimationCurve = EasingCurves.Quartic;

            int animationTime = Math.Min(CurrentSequence.Duration / 4, 33);
            int endTime = CurrentSequence.Duration;
            float maxHorizontalOffset = Main.ScreenSize.X * 1.15f;
            float startInterpolant = LumUtils.InverseLerp(animationTime, 0f, SequenceTimer);
            float endInterpolant = LumUtils.InverseLerp(endTime - animationTime, endTime, SequenceTimer);
            float horizontalDrawOffsetStart = offsetAnimationCurve.Evaluate(EasingType.InOut, startInterpolant) * -maxHorizontalOffset;
            float horizontalDrawOffsetEnd = offsetAnimationCurve.Evaluate(EasingType.InOut, endInterpolant) * maxHorizontalOffset;
            float horizontalDrawOffset = horizontalDrawOffsetStart + horizontalDrawOffsetEnd;

            TextOffsetInterpolant = MathF.Max(startInterpolant, endInterpolant);

            string text = Language.GetTextValue(CurrentSequence.LocalizationKey);
            Vector2 textSize = SubtitleFont.MeasureString(text);
            Vector2 drawPosition = Main.ScreenSize.ToVector2() * new Vector2(0.5f, 0.85f) + Vector2.UnitX * horizontalDrawOffset;
            Vector2 origin = textSize * 0.5f;

            for (int i = 0; i < 3; i++)
                ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, SubtitleFont, text, drawPosition, Color.Black, 0f, origin, Vector2.One * 1.5f, -1, i + 1f);

            ChatManager.DrawColorCodedString(Main.spriteBatch, SubtitleFont, text, drawPosition, CurrentSequence.Text.TextColor, 0f, origin, Vector2.One * 1.5f);
        }

        internal static void RenderSubtitlesWithPostProcessing()
        {
            if (CurrentSequence is null)
                return;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            float pixelationInterpolant = LumUtils.InverseLerp(0f, 0.5f, TextOffsetInterpolant);
            float pixelation = MathHelper.SmoothStep(1f, 6f, pixelationInterpolant) + LumUtils.InverseLerp(0.5f, 1f, TextOffsetInterpolant) * 10f;
            float opacity = LumUtils.InverseLerp(0.95f, 0.1f, TextOffsetInterpolant);

            ManagedShader overlayShader = ShaderManager.GetShader("FargowiltasCrossmod.DraedonSubtitleShader");
            overlayShader.TrySetParameter("pixelation", pixelation);
            overlayShader.TrySetParameter("textureSize", SubtitleRenderTarget.Size());
            overlayShader.Apply();

            Main.spriteBatch.Draw(SubtitleRenderTarget, Vector2.Zero, Color.White * opacity);

            Main.spriteBatch.ResetToDefaultUI();
        }
    }
}
