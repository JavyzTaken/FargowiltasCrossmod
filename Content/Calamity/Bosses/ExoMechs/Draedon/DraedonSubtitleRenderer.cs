using CalamityMod.UI.DraedonSummoning;
using Luminance.Common.Easings;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.DraedonSubtitleManager;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon
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

            SubtitleSection currentSection = CurrentSequence.Sections.OrderBy(s => s.Start).LastOrDefault(s => SequenceTimer >= s.Start);

            // This shouldn't happen, but just in case there is no valid section, don't try to draw anything.
            if (currentSection is null)
                return;

            int currentSectionIndex = Array.IndexOf(CurrentSequence.Sections, currentSection);
            int startTime = currentSection.Start;
            int endTime = CurrentSequence.Duration;
            if (currentSectionIndex < CurrentSequence.Sections.Length - 1)
                endTime = CurrentSequence.Sections[currentSectionIndex + 1].Start;

            EasingCurves.Curve offsetAnimationCurve = EasingCurves.Quartic;

            int animationTime = 30;
            float maxHorizontalOffset = Main.ScreenSize.X * 1.15f;
            float startInterpolant = LumUtils.InverseLerp(startTime + animationTime, startTime, SequenceTimer);
            float endInterpolant = LumUtils.InverseLerp(endTime - animationTime, endTime, SequenceTimer);
            float horizontalDrawOffsetStart = offsetAnimationCurve.Evaluate(EasingType.InOut, startInterpolant) * -maxHorizontalOffset;
            float horizontalDrawOffsetEnd = offsetAnimationCurve.Evaluate(EasingType.InOut, endInterpolant) * maxHorizontalOffset;
            float horizontalDrawOffset = horizontalDrawOffsetStart + horizontalDrawOffsetEnd;

            TextOffsetInterpolant = MathF.Max(startInterpolant, endInterpolant);

            string text = currentSection.Text;
            DynamicSpriteFont font = CodebreakerUI.DialogFont;
            Vector2 textSize = font.MeasureString(text);
            Vector2 drawPosition = Main.ScreenSize.ToVector2() * new Vector2(0.5f, 0.85f) + Vector2.UnitX * horizontalDrawOffset;
            Vector2 origin = textSize * 0.5f;

            for (int i = 0; i < 3; i++)
                ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, font, text, drawPosition, Color.Black, 0f, origin, Vector2.One * 1.5f, -1, i + 1f);

            ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, drawPosition, SubtitleColor, 0f, origin, Vector2.One * 1.5f);
        }

        internal static void RenderSubtitlesWithPostProcessing()
        {
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
