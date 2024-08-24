using CalamityMod.NPCs;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public sealed class ExoTwinsRenderTargetSystem : ModSystem
    {
        /// <summary>
        /// The render target that holds all of the Exo Twins' rendering data.
        /// </summary>
        public static ManagedRenderTarget ExoTwinsTarget
        {
            get;
            private set;
        }

        /// <summary>
        /// The render target that holds all dust.
        /// </summary>
        public static ManagedRenderTarget DustTarget
        {
            get;
            private set;
        }

        public override void OnModLoad()
        {
            ExoTwinsTarget = new(true, ManagedRenderTarget.CreateScreenSizedTarget);
            DustTarget = new(true, ManagedRenderTarget.CreateScreenSizedTarget);
            RenderTargetManager.RenderTargetUpdateLoopEvent += RenderToTargetWrapper;
        }

        /// <summary>
        /// Handles standard graphics device preparations for the rendering of the Exo Twins to the <see cref="ExoTwinsTarget"/>.
        /// </summary>
        private static void RenderToTargetWrapper()
        {
            if (!CalDLCWorldSavingSystem.E_EternityRev || CalamityGlobalNPC.draedonExoMechTwinGreen == -1 || CalamityGlobalNPC.draedonExoMechTwinRed == -1)
                return;

            // Be efficient with computational resources and only render to the render target if the Exo Twins specifically need it.
            if (!ExoTwinsStates.DeathAnimation_SuccessfullyCollided)
                return;

            var gd = Main.instance.GraphicsDevice;
            gd.SetRenderTarget(ExoTwinsTarget);
            gd.Clear(Color.Transparent);

            Main.spriteBatch.Begin();
            Main.instance.DrawNPC(CalamityGlobalNPC.draedonExoMechTwinGreen, false);
            Main.instance.DrawNPC(CalamityGlobalNPC.draedonExoMechTwinRed, false);
            Main.spriteBatch.End();

            gd.SetRenderTarget(DustTarget);
            gd.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < Main.maxDustToDraw; i++)
            {
                Dust dust = Main.dust[i];
                if (!dust.active)
                    continue;

                Main.spriteBatch.Draw(TextureAssets.Dust.Value, dust.position - Main.screenPosition, dust.frame, Color.White, dust.GetVisualRotation(), Vector2.One * 4f, dust.GetVisualScale(), SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
        }
    }
}
