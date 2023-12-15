using Terraria;
using Terraria.ModLoader;
using Fargowiltas.NPCs;
//using FargowiltasCrossmod.Content.Thorium.Items.Summons; 
using System.Collections.Generic;
using System.Linq;

namespace FargowiltasCrossmod.Core.Globals
{
    public class DevianttGlobalNPC : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == ModContent.NPCType<Deviantt>();
        public override bool InstancePerEntity => true;

        internal static int currentShop;
        internal static readonly List<NPCShop> ModShops = new();
        public static void CycleShop()
        {
            currentShop++;
            currentShop %= (ModShops.Count + 1);
        }

        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            if (currentShop == 0) return;

            if (ModShops.Count >= currentShop && ModShops[currentShop - 1] != null)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = null;
                    if (ModShops[currentShop - 1].ActiveEntries.Count() > i)
                    {
                        items[i] = ModShops[currentShop - 1].ActiveEntries.ElementAt(i).Item;
                    }
                } 
                return;
            }

            Main.NewText("You shouldn't be seeing this. Tall Ghoose");
        }

        public static void AddCalamityShop()
        {
            NPCShop shop = new(ModContent.NPCType<Deviantt>(), "Calamity");
            //shop.Add(new Item(Terraria.ID.ItemID.DirtBlock) { shopCustomPrice = Item.buyPrice(platinum: 3) }, new Condition("ech", () => !Main.hardMode));
            ModShops.Add(shop);
            shop.Register();
        }

        /* public static void AddThoriumShop()
        {
            NPCShop shop = new(ModContent.NPCType<Deviantt>(), "Thorium");
            shop.Add(new Item(ModContent.ItemType<MynaSummon>()) { shopCustomPrice = Item.buyPrice(gold: 3) }, new Condition("After Myna has been defeated", () => Systems.DownedEnemiesSystem.DLCDownedBools["Myna"]));
            shop.Add(new Item(ModContent.ItemType<GildedSummon>()) { shopCustomPrice = Item.buyPrice(gold: 3) }, new Condition("After the gilded enemies have been defeated", () => 
            Systems.DownedEnemiesSystem.DLCDownedBools["GildedLycan"] && Systems.DownedEnemiesSystem.DLCDownedBools["GildedBat"] && Systems.DownedEnemiesSystem.DLCDownedBools["GildedSlime"]));

            ModShops.Add(shop);
            shop.Register();
        } */
    }
}