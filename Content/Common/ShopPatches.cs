using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Fargowiltas.NPCs;
using FargowiltasCrossmod.Content.Thorium.Items.Summons;
using System.Reflection;
// using static FargowiltasCrossmod.Core.Systems.DownedEnemiesSystem;

namespace FargowiltasCrossmod.Content.Common
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public static class MiscThoriumMethods
    {
        internal static void ThoriumBiomeBugs(Player player, ref string quote)
        {
            if (player.InModBiome<ThoriumMod.Biomes.Depths.DepthsBiome>())
            {
                quote = "Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn.";
                int itemType = ModContent.ItemType<ThoriumMod.Items.Depths.WaterChestnut>();
                player.QuickSpawnItem(player.GetSource_OpenItem(itemType), itemType, 5);
                itemType = ModContent.ItemType<ThoriumMod.Items.Depths.MarineBlock>();
                player.QuickSpawnItem(player.GetSource_OpenItem(itemType), itemType, 50);
            }
        }
    }

    public static class LumberBoyPatches
    {
        internal delegate void orig_OnChatButtonClicked(LumberJack self, bool firstButton, ref string shopName);
        internal static void OnChatButtonClicked(orig_OnChatButtonClicked orig, LumberJack self, bool firstButton, ref string shopName)
        {
            bool nightOver = (bool)self.GetType().GetField("nightOver", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
            bool dayOver = (bool)self.GetType().GetField("dayOver", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
            if (!firstButton && nightOver && dayOver)
            {
                Player player = Main.LocalPlayer;
                string quote = "";
                if (Core.ModCompatibility.ThoriumMod.Loaded)
                {
                    MiscThoriumMethods.ThoriumBiomeBugs(player, ref quote);
                }

                if (quote != "")
                {
                    Main.npcChatText = quote;
                    return;
                }
            }
            orig(self, firstButton, ref shopName);
        }
    }
}