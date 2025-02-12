using FargowiltasCrossmod.Assets;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class OldDukeSky : CustomSky
    {
        private bool skyActive;

        private static readonly float[] lightningFlashLifetimeRatios = new float[4];

        private static readonly float[] lightningFlashIntensities = new float[4];

        private static readonly Vector2[] lightningFlashPositions = new Vector2[4];

        /// <summary>
        /// The general opacity of this sky.
        /// </summary>
        public static new float Opacity
        {
            get;
            set;
        }

        /// <summary>
        /// The angle of the rain in the sky.
        /// </summary>
        public static float RainAngle
        {
            get;
            set;
        }

        /// <summary>
        /// The timer for the rain shader visual.
        /// </summary>
        public static float RainTimer
        {
            get;
            set;
        }

        /// <summary>
        /// The brightness factor for the rain shader visual.
        /// </summary>
        public static float RainBrightnessFactor
        {
            get;
            set;
        } = 1f;

        /// <summary>
        /// The speed at which the rain beats down.
        /// </summary>
        public static float RainSpeed
        {
            get;
            set;
        } = 1f;

        /// <summary>
        /// The identifier key for this sky.
        /// </summary>
        public const string SkyKey = "FargowiltasCrossmod:OldDukeSky";

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            Rectangle screenArea = new Rectangle(0, 0, Main.instance.GraphicsDevice.Viewport.Width, Main.instance.GraphicsDevice.Viewport.Height);
            bool lightningEnabled = maxDepth < float.MaxValue || minDepth >= float.MaxValue;
            float opacity = lightningEnabled ? 0.95f : 0.2f;
            ManagedShader skyShader = ShaderManager.GetShader("FargowiltasCrossmod.OldDukeSkyShader");
            skyShader.TrySetParameter("lightningFlashLifetimeRatios", lightningFlashLifetimeRatios.ToArray());
            skyShader.TrySetParameter("lightningFlashIntensities", lightningFlashIntensities.ToArray());
            skyShader.TrySetParameter("lightningFlashPositions", lightningFlashPositions.ToArray());
            skyShader.TrySetParameter("lightningEnabled", lightningEnabled);
            skyShader.TrySetParameter("lightningColor", new Vector3(1.5f, 1.8f, 1.75f));
            skyShader.TrySetParameter("pixelationFactor", Vector2.One * 3f / screenArea.Size());
            skyShader.SetTexture(NoiseTexturesRegistry.CrackedNoiseA.Value, 1, SamplerState.LinearWrap);
            skyShader.SetTexture(MiscTexturesRegistry.DendriticNoiseZoomedOut.Value, 2, SamplerState.LinearWrap);
            skyShader.Apply();

            Main.spriteBatch.Draw(MiscTexturesRegistry.Pixel.Value, screenArea, Main.ColorOfTheSkies * Opacity * opacity);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
        }

        /// <summary>
        /// Creates a new lightning flash with a given UV position in the sky.
        /// </summary>
        public static void CreateLightningFlash(Vector2 lightningPosition)
        {
            for (int i = 0; i < lightningFlashIntensities.Length; i++)
            {
                if (lightningFlashLifetimeRatios[i] <= 0f)
                {
                    lightningFlashPositions[i] = lightningPosition;
                    lightningFlashLifetimeRatios[i] = 0.001f;
                    lightningFlashIntensities[i] = Main.rand.NextFloat(0.4f, 0.8f);
                    break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            Opacity = LumUtils.Saturate(Opacity + skyActive.ToDirectionInt() * 0.04f);
            RainAngle = RainAngle.AngleLerp(1.34f, 0.2f);

            // Disable ambient sky objects like wyverns and eyes appearing in front of the background.
            if (Opacity >= 0.5f)
                SkyManager.Instance["Ambience"].Deactivate();

            if (!skyActive)
                ResetVariablesWhileInactive();

            if (Main.gamePaused)
                return;

            RainSpeed = MathHelper.Lerp(RainSpeed, 1f, 0.0167f);
            RainBrightnessFactor = MathHelper.Lerp(RainBrightnessFactor, 1f, 0.0167f);
            RainTimer = (RainTimer + RainSpeed / 30.75f) % 10000f;

            for (int i = 0; i < lightningFlashIntensities.Length; i++)
            {
                ref float lifetimeRatio = ref lightningFlashLifetimeRatios[i];
                if (lifetimeRatio <= 0f)
                    continue;

                lifetimeRatio += MathF.Pow(LumUtils.Saturate(lightningFlashIntensities[i]), 1.44f) * 0.064f;
                if (lifetimeRatio >= 1f)
                    lifetimeRatio = 0f;
            }
        }

        public static void ResetVariablesWhileInactive()
        {
        }

        public override Color OnTileColor(Color inColor)
        {
            inColor = Color.Lerp(inColor, new Color(216, 255, 230) * 0.75f, Opacity * 0.9f);
            Main.ColorOfTheSkies = new Color(46, 51, 60);
            return inColor;
        }

        #region Boilerplate
        public override void Deactivate(params object[] args) => skyActive = false;

        public override void Reset() => skyActive = false;

        public override bool IsActive() => skyActive || Opacity > 0f;

        public override void Activate(Vector2 position, params object[] args) => skyActive = true;

        public override float GetCloudAlpha() => 1f - Opacity;
        #endregion Boilerplate
    }
}
