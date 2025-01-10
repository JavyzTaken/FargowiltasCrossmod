﻿using CalamityMod.NPCs;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Systems;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HadesPostProcessingSystem : ModSystem
    {
        /// <summary>
        /// The render target that holds all render information of Hades, for the purposes of allowing all of him to render with post-processing.
        /// </summary>
        public static ManagedRenderTarget HadesTarget
        {
            get;
            private set;
        }

        /// <summary>
        /// An optional post-processing action to perform on all of Hades.
        /// </summary>
        public static Action? PostProcessingAction
        {
            get;
            set;
        }

        public override void OnModLoad()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            HadesTarget = new(true, ManagedRenderTarget.CreateScreenSizedTarget);
            RenderTargetManager.RenderTargetUpdateLoopEvent += RenderHadesToTarget;

            On_Main.DrawNPCs += DrawHadesTarget;
        }

        private static void RenderHadesToTarget()
        {
            if (CalamityGlobalNPC.draedonExoMechWorm == -1 || !CalDLCWorldSavingSystem.E_EternityRev)
                return;

            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            graphicsDevice.SetRenderTarget(HadesTarget);
            graphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin();

            for (int i = Main.maxNPCs - 1; i >= 0; i--)
            {
                NPC npc = Main.npc[i];
                if (npc.realLife == CalamityGlobalNPC.draedonExoMechWorm || npc.whoAmI == CalamityGlobalNPC.draedonExoMechWorm)
                    NPCLoader.PreDraw(npc, Main.spriteBatch, Main.screenPosition, Lighting.GetColor(npc.Center.ToTileCoordinates()));
            }

            Main.spriteBatch.End();
        }

        private static void DrawHadesTarget(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);
            if (CalamityGlobalNPC.draedonExoMechWorm != -1 && !behindTiles && CalDLCWorldSavingSystem.E_EternityRev)
            {
                Main.spriteBatch.PrepareForShaders();

                PostProcessingAction?.Invoke();
                Main.spriteBatch.Draw(HadesTarget, Main.screenLastPosition - Main.screenPosition, Color.White);

                Main.spriteBatch.ResetToDefault();
            }
        }
    }
}
