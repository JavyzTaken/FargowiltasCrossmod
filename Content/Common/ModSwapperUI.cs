using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace FargowiltasCrossmod.Content.Common
{
    public class ModSwapperUISystem : ModSystem
    {
        internal UserInterface SwapperInterface;
        internal ModSwapperUIState SwapperState;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                SwapperInterface = new UserInterface();

                SwapperState = new();
                SwapperState.Activate(); 
            }
        }

        public override void Unload()
        {
            SwapperState = null;
        }

        private GameTime _lastUpdateUiGameTime;

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            // Update this when we add more shop patches
            if (Main.LocalPlayer.talkNPC > 0 && !Main.playerInventory/* && Main.npc[Main.LocalPlayer.talkNPC].ModNPC.Type == ModContent.NPCType<Fargowiltas.NPCs.Deviantt>()*/)
            {
                if (SwapperInterface?.CurrentState != null)
                {
                    SwapperInterface.Update(gameTime);
                }
                SwapperInterface.SetState(SwapperState);
            }
            else SwapperInterface.SetState(null);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int npcChatMenuIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
            if (npcChatMenuIndex != -1)
            {
                layers.Insert(npcChatMenuIndex, new LegacyGameInterfaceLayer(
                    "SoulsBetterDLC: ModSwapperInterface",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && SwapperInterface?.CurrentState != null)
                        {
                            SwapperInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }

    public class ModSwapperUIState : UIState
    {
        public override void OnInitialize()
        {
            UITextPanel<string> textBox = new("Switch Mod");
            textBox.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => DevianttPatches.CycleShop();
            textBox.Left.Set(600, 0);
            textBox.Top.Set(200, 0);
            textBox.IgnoresMouseInteraction = false;
            Append(textBox);
        }
    }
}
