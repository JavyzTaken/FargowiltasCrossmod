using CalamityMod.NPCs;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
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

        public override void OnModLoad()
        {
            ExoTwinsTarget = new(true, ManagedRenderTarget.CreateScreenSizedTarget);
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

            gd.SetRenderTarget(null);
        }
    }
}
