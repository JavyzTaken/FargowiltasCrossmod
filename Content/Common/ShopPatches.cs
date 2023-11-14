using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Fargowiltas.NPCs;
using FargowiltasCrossmod.Content.Thorium.Items.Summons;
using System.Reflection;
using static FargowiltasCrossmod.Core.Systems.DownedEnemiesSystem;

namespace FargowiltasCrossmod.Content.Common
{
    public static class DevianttPatches
    {
        internal static int DevianttShopCurrent = 0;
        private static readonly List<string> ModShopNames = new() { "Shop" };

        public static void CycleShop()
        {
            DevianttShopCurrent++;
            DevianttShopCurrent %= ModShopNames.Count;
        }

        internal delegate void orig_SetChatButtons(Deviantt self, ref string button, ref string button2);
        internal static void SetChatButtons(orig_SetChatButtons orig, Deviantt self, ref string button, ref string button2)
        {
            orig(self, ref button, ref button2);
            button = ModShopNames[DevianttShopCurrent % ModShopNames.Count];
        }

        internal delegate void orig_OnChatButtonClicked(Deviantt self, bool firstButton, ref string shopName);
        internal static void OnChatButtonClicked(orig_OnChatButtonClicked orig, Deviantt self, bool firstButton, ref string shopName)
        {
            orig(self, firstButton, ref shopName);

            if (firstButton)
            {
                shopName = ModShopNames[DevianttShopCurrent % ModShopNames.Count];
            }
        }

        internal static void AddThoriumDeviShop()
        {
            ModShopNames.Add("Thorium");
            var npcShop = new NPCShop(ModContent.NPCType<Deviantt>(), "Thorium");

            //npcShop.Add(ModContent.ItemType<GildedSummon>());
            //TODO: find out what is causing a bug that makes adding anything to this shop not work

            npcShop.Register();
        }
    }

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
        //internal static void SetupThoriumShop(Chest shop, ref int nextSlot)
        //{
        //    shop.item[nextSlot].SetDefaults(ModContent.ItemType<ThoriumMod.Items.ArcaneArmor.YewWood>());
        //    shop.item[nextSlot].shopCustomPrice = 10;
        //    nextSlot++;
        //}
    }

    public static class LumberBoyPatches
    {
        internal delegate void orig_AddShops(LumberJack self);
        internal static void AddShops(orig_AddShops orig, LumberJack self)
        {
            orig(self);
        }

        internal delegate void orig_OnChatButtonClicked(LumberJack self, bool firstButton, ref string shopName);
        internal static void OnChatButtonClicked(orig_OnChatButtonClicked orig, LumberJack self, bool firstButton, ref string shopName)
        {
            bool nightOver = (bool)self.GetType().GetField("nightOver", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
            bool dayOver = (bool)self.GetType().GetField("dayOver", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self);
            if (!firstButton && nightOver && dayOver)
            {
                Player player = Main.LocalPlayer;
                string quote = "";
                if (FargowiltasCrossmod.ThoriumLoaded)
                {
                    MiscThoriumMethods.ThoriumBiomeBugs(player, ref quote);
                }
                if (FargowiltasCrossmod.CalamityLoaded && quote == "")
                {

                }

                if (quote != "")
                {
                    Main.npcChatText = quote;
                    return;
                }
            }
            orig(self, firstButton, ref shopName);
        }

        internal static void SetupCalamityShop(Chest shop, ref int nextSlot)
        {

        }
    }
}