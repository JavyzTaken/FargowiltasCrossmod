using CalamityMod.NPCs;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public sealed class AresRenderTargetSystem : ModSystem
    {
        /// <summary>
        /// The render target that holds all of Ares' rendering data.
        /// </summary>
        public static ManagedRenderTarget AresTarget
        {
            get;
            private set;
        }

        public override void OnModLoad()
        {
            AresTarget = new(true, ManagedRenderTarget.CreateScreenSizedTarget);
            RenderTargetManager.RenderTargetUpdateLoopEvent += RenderToTargetWrapper;
        }

        /// <summary>
        /// Handles standard graphics device preparations for the rendering of Ares and his body parts to the <see cref="AresTarget"/>.
        /// </summary>
        private static void RenderToTargetWrapper()
        {
            if (!CalDLCWorldSavingSystem.E_EternityRev || CalamityGlobalNPC.draedonExoMechPrime == -1)
                return;

            NPC ares = Main.npc[CalamityGlobalNPC.draedonExoMechPrime];
            if (!ares.TryGetDLCBehavior(out AresBodyEternity aresBehavior))
                return;

            // Be efficient with computational resources and only render to the render target if Ares specifically needs it.
            if (!aresBehavior.NeedsToBeDrawnToRenderTarget)
                return;

            var gd = Main.instance.GraphicsDevice;
            gd.SetRenderTarget(AresTarget);
            gd.Clear(Color.Transparent);

            Main.spriteBatch.Begin();
            RenderAresToTarget(aresBehavior);
            Main.spriteBatch.End();

            gd.SetRenderTarget(null);
        }

        /// <summary>
        /// Renders Ares and his body parts.
        /// </summary>
        private static void RenderAresToTarget(AresBodyEternity ares)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.realLife == ares.NPC.whoAmI || npc.whoAmI == ares.NPC.whoAmI)
                    Main.instance.DrawNPC(npc.whoAmI, false);
            }
        }
    }
}
