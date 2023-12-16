using Terraria;
using Terraria.ModLoader;
using Fargowiltas.NPCs;
//using FargowiltasCrossmod.Content.Thorium.Items.Summons; 
using System.Collections.Generic;
using System.Linq;
using FargowiltasCrossmod.Content.Calamity.Items.Summons;
using FargowiltasCrossmod.Core.Common.Systems;
using FargowiltasCrossmod.Core.Calamity.Systems;
using CalamityMod.NPCs;
using static FargowiltasCrossmod.Core.ModCompatibility;
using MonoMod.RuntimeDetour;
using System.Reflection;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Core.Common.Globals
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
            currentShop %= ModShops.Count + 1;
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
        public delegate void Orig_DevianttAddShops(Deviantt self);

        private static readonly MethodInfo DevianttAddShopsMethod = typeof(Deviantt).GetMethod("AddShops", FargoSoulsUtil.UniversalBindingFlags);
        Hook DeviShopHook;
        public override void Load()
        {
            DeviShopHook = new(DevianttAddShopsMethod, DevianttAddShopsDetour);
            DeviShopHook.Apply();
        }
        public override void Unload()
        {
            DeviShopHook.Undo();
        }

        internal static void DevianttAddShopsDetour(Orig_DevianttAddShops orig, Deviantt self)
        {
            AddCalamityShop();

            orig(self);
        }

        public static void AddCalamityShop()
        {
            NPCShop shop = new(ModContent.NPCType<Deviantt>(), "Calamity");
            //shop.Add(new Item(Terraria.ID.ItemID.DirtBlock) { shopCustomPrice = Item.buyPrice(platinum: 3) }, new Condition("ech", () => !Main.hardMode));

            Condition killedClam = new Condition("After killing a Giant Clam", () => CalamityAIOverride.DownedClam);
            Condition killedPlaguebringerMini = new Condition("After killing a Plaguebringer", () => DLCWorldSavingSystem.downedMiniPlaguebringer);
            Condition killedReaperShark = new Condition("After killing a Reaper Shark", () => DLCWorldSavingSystem.downedReaperShark);
            Condition killedColossalSquid = new Condition("After killing a Colossal Squid", () => DLCWorldSavingSystem.downedColossalSquid);
            Condition killedEidolonWyrm = new Condition("After killing an Eidolon Wyrm", () => DLCWorldSavingSystem.downedEidolonWyrm);
            Condition killedCloudElemental = new Condition("After killing a Cloud Elemental", () => DLCWorldSavingSystem.downedCloudElemental);
            Condition killedEarthElemental = new Condition("After killing an Earth Elemental", () => DLCWorldSavingSystem.downedEarthElemental);
            Condition killedArmoredDigger = new Condition("After killing an Armored Digger", () => DLCWorldSavingSystem.downedArmoredDigger);

            shop.Add(new Item(ModContent.ItemType<ClamPearl>()) { shopCustomPrice = Item.buyPrice(gold: 5) }, killedClam);
            shop.Add(new Item(ModContent.ItemType<AbandonedRemote>()) { shopCustomPrice = Item.buyPrice(gold: 10) }, killedArmoredDigger);
            shop.Add(new Item(ModContent.ItemType<PlaguedWalkieTalkie>()) { shopCustomPrice = Item.buyPrice(gold: 10) }, killedPlaguebringerMini);
            shop.Add(new Item(ModContent.ItemType<DeepseaProteinShake>()) { shopCustomPrice = Item.buyPrice(gold: 30) }, killedReaperShark);
            shop.Add(new Item(ModContent.ItemType<ColossalTentacle>()) { shopCustomPrice = Item.buyPrice(gold: 30) }, killedColossalSquid);
            shop.Add(new Item(ModContent.ItemType<WyrmTablet>()) { shopCustomPrice = Item.buyPrice(gold: 30) }, killedEidolonWyrm);
            shop.Add(new Item(ModContent.ItemType<StormIdol>()) { shopCustomPrice = Item.buyPrice(gold: 7) }, killedCloudElemental);
            shop.Add(new Item(ModContent.ItemType<QuakeIdol>()) { shopCustomPrice = Item.buyPrice(gold: 7) }, killedEarthElemental);
            
            shop.Register();

            ModShops.Add(shop);
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