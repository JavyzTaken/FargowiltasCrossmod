using FargowiltasCrossmod.Core.Common.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Localization;

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

            if (Main.LocalPlayer.talkNPC != -1 && !Main.playerInventory && Main.npc[Main.LocalPlayer.talkNPC].type == ModContent.NPCType<Fargowiltas.NPCs.Deviantt>())
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
                    "FargowiltasCrossmod: ModSwapperInterface",
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
    public class SwitchModButton : UITextPanel<string>
    {
        public SwitchModButton(string text, float textScale = 1, bool large = false) : base(text, textScale, large)
        {
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
    public class ModSwapperUIState : UIState
    {
        public override void OnInitialize()
        {
            SwitchModButton textBox = new("Switch Mod");
            textBox.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) =>
            {
                DevianttGlobalNPC.CycleShop();
                int currentShop = DevianttGlobalNPC.currentShop;
                if (currentShop == 0)
                {
                    (listeningElement as UITextPanel<string>).SetText(Language.GetTextValue("Mods.FargowiltasCrossmod.NPCs.ShopModSwapper.Vanilla"));
                }
                else
                {
                    (listeningElement as UITextPanel<string>).SetText(DevianttGlobalNPC.ModShops[currentShop - 1].Name);
                }
            };
            textBox.Left.Set(550, 0);
            textBox.Top.Set(200, 0);
            textBox.IgnoresMouseInteraction = false;
            Append(textBox);
        }

        public override void OnActivate()
        {
            RecalculateChildren();
        }
    }
}
