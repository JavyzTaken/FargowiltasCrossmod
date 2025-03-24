using FargowiltasCrossmod.Assets;
using FargowiltasCrossmod.Assets.Models;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon;
using FargowiltasCrossmod.Core.Calamity;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers
{
    public class CustomExoMechsSky : CustomSky
    {
        public class LightningData
        {
            public float Brightness;

            public Vector2 LightningPosition;

            public void Update()
            {
                Brightness = Utilities.Saturate(Brightness * 0.991f - 0.0011f);
            }
        }

        private bool skyActive;

        /// <summary>
        /// The exposure value of the clouds. Gradually returns to a stable value over time.
        /// </summary>
        public static float CloudExposure
        {
            get;
            set;
        }

        /// <summary>
        /// The general opacity of this sky.
        /// </summary>
        public static new float Opacity
        {
            get;
            set;
        }

        /// <summary>
        /// The intensity of the red sirens effect.
        /// </summary>
        public static float RedSirensIntensity
        {
            get;
            set;
        }

        /// <summary>
        /// How much the sky colors should be biased towards red.
        /// </summary>
        public static float RedSkyInterpolant
        {
            get;
            set;
        }

        /// <summary>
        /// The offset of clouds.
        /// </summary>
        public static Vector2 CloudOffset
        {
            get;
            set;
        }

        /// <summary>
        /// The default cloud exposure value.
        /// </summary>
        public static float DefaultCloudExposure => 0.3f;

        /// <summary>
        /// The lightning instances.
        /// </summary>
        public static readonly LightningData[] Lightning = new LightningData[5];

        /// <summary>
        /// The identifier key for this sky.
        /// </summary>
        public const string SkyKey = "FargowiltasCrossmod:ExoMechsSky";

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            // Calculate the maximum sky opacity value.
            // If Draedon is not present it is assumed that the Exo Mechs were just spawned in via cheating, and as such the sky should immediately draw at its maximum intensity, rather than not at all.
            float maxSkyOpacity = 1f;
            float planeForwardInterpolant = 0f;
            int draedonIndex = NPC.FindFirstNPC(ModContent.NPCType<CalamityMod.NPCs.ExoMechs.Draedon>());
            if (draedonIndex >= 0 && Main.npc[draedonIndex].TryGetDLCBehavior(out DraedonEternity behavior))
            {
                maxSkyOpacity = behavior.MaxSkyOpacity;
                planeForwardInterpolant = 1f - behavior.PlaneFlyForwardInterpolant;
            }

            if (!Main.gamePaused)
            {
                CloudExposure = MathHelper.Lerp(CloudExposure, DefaultCloudExposure, 0.03f);
                Opacity = MathHelper.Clamp(Opacity + skyActive.ToDirectionInt() * 0.0011f, 0f, maxSkyOpacity);
            }

            // Prevent drawing beyond the back layer.
            if (maxDepth >= float.MaxValue || minDepth < float.MaxValue)
            {
                Matrix backgroundMatrix = Main.BackgroundViewMatrix.TransformationMatrix;
                Vector3 translationDirection = new(1f, Main.BackgroundViewMatrix.Effects.HasFlag(SpriteEffects.FlipVertically) ? -1f : 1f, 1f);
                backgroundMatrix.Translation -= Main.BackgroundViewMatrix.ZoomMatrix.Translation * translationDirection;

                // Draw clouds.
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, backgroundMatrix);
                DrawGreySky();

                // Return to standard drawing.
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, backgroundMatrix);
            }

            if (maxDepth < float.MaxValue || minDepth >= float.MaxValue)
            {
                PrimitivePixelationSystem.RenderToPrimsNextFrame(() =>
                {
                    DrawPlane(planeForwardInterpolant);
                }, PixelationPrimitiveLayer.AfterProjectiles);
            }
        }

        public static void DrawGreySky()
        {
            for (int i = 0; i < Lightning.Length; i++)
            {
                Lightning[i] ??= new();

                if (!Main.gamePaused)
                    Lightning[i].Update();
            }

            int lightningSpawnChance = (int)MathHelper.Lerp(480f, 45f, RedSkyInterpolant);
            if (Main.rand.NextBool(lightningSpawnChance))
                CreateLightning();

            Vector2 screenSize = new(Main.instance.GraphicsDevice.Viewport.Width, Main.instance.GraphicsDevice.Viewport.Height);

            if (!Main.gamePaused)
                CloudOffset -= Vector2.UnitX * (float)Main.dayRate * 0.001f;

            float[] lightningIntensities = new float[Lightning.Length];
            Vector2[] lightningPositions = new Vector2[Lightning.Length];
            for (int i = 0; i < lightningIntensities.Length; i++)
            {
                lightningIntensities[i] = Lightning[i].Brightness;
                lightningPositions[i] = Lightning[i].LightningPosition;
            }

            ManagedShader cloudShader = ShaderManager.GetShader("FargowiltasCrossmod.ExoMechCloudShader");
            cloudShader.TrySetParameter("screenSize", screenSize);
            cloudShader.TrySetParameter("invertedGravity", Main.LocalPlayer.gravDir == -1f);
            cloudShader.TrySetParameter("sunPosition", new Vector3(screenSize.X * 0.5f, screenSize.Y * 0.7f, -600f));
            cloudShader.TrySetParameter("worldPosition", Main.screenPosition);
            cloudShader.TrySetParameter("parallax", new Vector2(0.3f, 0.175f) * Main.caveParallax);
            cloudShader.TrySetParameter("cloudDensity", Opacity * 0.6f);
            cloudShader.TrySetParameter("horizontalOffset", CloudOffset.X);
            cloudShader.TrySetParameter("cloudExposure", CloudExposure);
            cloudShader.TrySetParameter("pixelationFactor", 4f);
            cloudShader.TrySetParameter("lightningIntensities", lightningIntensities);
            cloudShader.TrySetParameter("lightningPositions", lightningPositions);
            cloudShader.SetTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise"), 1, SamplerState.LinearWrap);
            cloudShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 2, SamplerState.LinearWrap);
            cloudShader.Apply();

            Texture2D cloud = NoiseTexturesRegistry.CloudDensityMap.Value;
            Vector2 drawPosition = screenSize * 0.5f;
            Vector2 skyScale = screenSize / cloud.Size();
            Color redSkyColor = Color.Lerp(new(255, 66, 78), new(255, 190, 184), LumUtils.Cos01(Main.GlobalTimeWrappedHourly * 6f));
            Color cloudColor = Color.Lerp(new(48, 57, 70), redSkyColor, RedSkyInterpolant);
            Main.spriteBatch.Draw(cloud, drawPosition, null, cloudColor, 0f, cloud.Size() * 0.5f, skyScale, 0, 0f);
        }

        public static void CreateLightning(Vector2? lightningPosition = null)
        {
            if (Main.netMode == NetmodeID.Server || Main.gamePaused)
                return;

            lightningPosition ??= new Vector2(Main.rand.NextFloat(0.2f, 0.8f), Main.rand.NextFloat(-0.07f, 0.9f));

            for (int i = 0; i < Lightning.Length; i++)
            {
                if (Lightning[i].Brightness < 0.03f)
                {
                    Lightning[i].Brightness = Main.rand.NextFloat(0.6f, 0.72f);
                    Lightning[i].LightningPosition = lightningPosition.Value;
                    break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Disable ambient sky objects like wyverns and eyes appearing in front of the background.
            if (Opacity >= 0.5f)
                SkyManager.Instance["Ambience"].Deactivate();

            // Disable Calamity's vanilla Exo Mechs background.
            if (Opacity >= 0.01f)
                SkyManager.Instance["CalamityMod:ExoMechs"]?.Deactivate();

            if (!skyActive)
                ResetVariablesWhileInactive();
            if (!Main.gamePaused)
                RedSkyInterpolant = LumUtils.Saturate(RedSkyInterpolant - 0.01f);
        }

        public static void DrawPlane(float forwardInterpolant)
        {
            if (forwardInterpolant <= 0f || forwardInterpolant >= 1f)
                return;

            Vector2 screenSize = new(Main.instance.GraphicsDevice.Viewport.Width, Main.instance.GraphicsDevice.Viewport.Height);
            Vector3 planePosition = new(screenSize * new Vector2(0.5f, 0.45f), MathHelper.Lerp(100f, -0.95f, MathF.Pow(1f - forwardInterpolant, 0.67f)));
            float scale = 0.7f / (planePosition.Z + 1f);
            float opacity = Utilities.InverseLerp(100f, 54f, planePosition.Z);
            planePosition.Y -= scale * 1560f;

            Matrix rotation = Matrix.CreateRotationX((1f - forwardInterpolant) * 0.5f) * Matrix.CreateRotationZ(MathHelper.Pi);

            Matrix world = rotation * Matrix.CreateScale(scale) * Matrix.CreateWorld(planePosition, Vector3.Forward, Vector3.Up);
            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, screenSize.X, screenSize.Y, 0f, -5000f, 5000f);
            Model plane = ModelRegistry.CargoPlane;

            // Prepare shaders.
            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.ModelPrimitiveShader");
            Main.instance.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/CargoPlaneModelTexture").Value;
            Main.instance.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Main.instance.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            foreach (ModelMesh mesh in plane.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    if (part.PrimitiveCount > 0)
                    {
                        shader.TrySetParameter("opacity", opacity);
                        shader.TrySetParameter("uWorldViewProjection", mesh.ParentBone.Transform * world * projection);
                        shader.Apply();

                        Main.instance.GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        Main.instance.GraphicsDevice.Indices = part.IndexBuffer;
                        Main.instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.VertexOffset, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }

        public static void ResetVariablesWhileInactive()
        {
            RedSirensIntensity = Utilities.Saturate(RedSirensIntensity - 0.1f);
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
