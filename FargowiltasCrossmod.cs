using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using FargowiltasCrossmod.Content.Common;
using System;
using System.Linq;
using System.Reflection;
using FargowiltasCrossmod.Core;
using System.Collections.Generic;
using FargowiltasSouls.Core.Toggler;
using System.IO;
using FargowiltasCrossmod.Content.Thorium;
using ThoriumMod.Items.Consumable;
using ThoriumMod.Items.Donate;
using ThoriumMod.Items.NPCItems;
using ThoriumMod.NPCs;

namespace FargowiltasCrossmod
{
	public class FargowiltasCrossmod : Mod
	{
		internal static bool CalamityLoaded;
		internal static bool ThoriumLoaded;

        internal static FargowiltasCrossmod Instance;


        private class ClassPreJitFilter : PreJITFilter
        {
            public override bool ShouldJIT(MemberInfo member)
            {
                return base.ShouldJIT(member) &&
                       // also check attributes on declaring class
                       member.DeclaringType?.GetCustomAttributes<MemberJitAttribute>().All(a => a.ShouldJIT(member)) != false;
            }
        }

        public FargowiltasCrossmod()
        {
            PreJITFilter = new ClassPreJitFilter();
        }

        public override void Load()
        {
            Instance = this;

            DLCCaughtNPCItem.RegisterItems();

            ThoriumLoaded = ModLoader.HasMod("ThoriumMod");
            CalamityLoaded = ModLoader.HasMod("CalamityMod");
            LoadDetours();

            if (ThoriumLoaded) ThoriumLoad();
        }

        private struct DeviantHooks
        {
            internal static Hook SetChatButtons;
            internal static Hook OnChatButtonClicked;
            //internal static Hook AddShops;
        }
        private struct LumberHooks
        {
            internal static Hook OnChatButtonClicked;
            internal static Hook AddShops;
        }

        private static Hook TryUnlimBuff;

        private static void LoadDetours()
        {
            Type deviDetourClass = ModContent.Find<ModNPC>("Fargowiltas/Deviantt").GetType();

            if (deviDetourClass != null)
            {
                MethodInfo SetChatButtons_DETOUR = deviDetourClass.GetMethod("SetChatButtons", BindingFlags.Public | BindingFlags.Instance);
                MethodInfo OnChatButtonClicked_DETOUR = deviDetourClass.GetMethod("OnChatButtonClicked", BindingFlags.Public | BindingFlags.Instance);
                MethodInfo AddShops_DETOUR = deviDetourClass.GetMethod("AddShops", BindingFlags.Public | BindingFlags.Instance);


                DeviantHooks.SetChatButtons = new Hook(SetChatButtons_DETOUR, DevianttPatches.SetChatButtons);
                DeviantHooks.OnChatButtonClicked = new Hook(OnChatButtonClicked_DETOUR, DevianttPatches.OnChatButtonClicked);

                DeviantHooks.SetChatButtons.Apply();
                DeviantHooks.OnChatButtonClicked.Apply();
            }

            Type lumberDetourClass = ModContent.Find<ModNPC>("Fargowiltas/LumberJack").GetType();

            if (lumberDetourClass != null)
            {
                MethodInfo OnChatButtonClicked_DETOUR = lumberDetourClass.GetMethod("OnChatButtonClicked", BindingFlags.Public | BindingFlags.Instance);
                MethodInfo AddShops_DETOUR = lumberDetourClass.GetMethod("AddShops", BindingFlags.Public | BindingFlags.Instance);

                LumberHooks.OnChatButtonClicked = new Hook(OnChatButtonClicked_DETOUR, LumberBoyPatches.OnChatButtonClicked);
                LumberHooks.AddShops = new Hook(AddShops_DETOUR, LumberBoyPatches.AddShops);

                LumberHooks.OnChatButtonClicked.Apply();
                LumberHooks.AddShops.Apply();
            }
        }

        public override void PostSetupContent()
        {
            Type fargoGlobalItemClass = ModContent.Find<GlobalItem>("Fargowiltas/FargoGlobalItem").GetType();

            if (fargoGlobalItemClass != null)
            {
                MethodInfo TryUnlimBuff_DETOUR = fargoGlobalItemClass.GetMethod("TryUnlimBuff", BindingFlags.Public | BindingFlags.Static);
                TryUnlimBuff = new Hook(TryUnlimBuff_DETOUR, TryUnlimBuffPatch);

                TryUnlimBuff.Apply();
            }
        }

        public void ThoriumLoad()
        {
            Assembly ThoriumAssembly = ModLoader.GetMod("ThoriumMod").GetType().Assembly;
            Type thoriumProjExtensions = ThoriumAssembly.GetType("ThoriumMod.Utilities.ProjectileHelper", true);

            Content.Thorium.Projectiles.DLCHealing.HealMethod = thoriumProjExtensions.GetMethod("ThoriumHeal", BindingFlags.Static | BindingFlags.NonPublic);
            Content.Thorium.Projectiles.DLCHealing.CustomHealingType = thoriumProjExtensions.GetNestedType("CustomHealing", BindingFlags.NonPublic);

            Content.Thorium.Items.Accessories.Enchantments.YewWoodEnchant.LoadModdedAmmo();

            DevianttPatches.AddThoriumDeviShop();

            unlimitedBlackList.Add(ModContent.ItemType<KineticPotion>());
            unlimitedBlackList.Add(ModContent.ItemType<InsectRepellent>());
            unlimitedBlackList.Add(ModContent.ItemType<BatRepellent>());
            unlimitedBlackList.Add(ModContent.ItemType<FishRepellent>());
            unlimitedBlackList.Add(ModContent.ItemType<SkeletonRepellent>());
            unlimitedBlackList.Add(ModContent.ItemType<ZombieRepellent>());
            unlimitedBlackList.Add(ModContent.ItemType<CactusFruit>());

            thoriumBuffNPCs.Add(ModContent.Find<ModItem>("FargowiltasCrossmod/Cobbler").Type, ModContent.BuffType<ThoriumMod.Buffs.CobblerBuff>());
        }

        private static readonly Dictionary<int, int> thoriumBuffNPCs = new();
        public static readonly List<int> unlimitedBlackList = new();
        internal delegate void orig_TryUnlimBuff(Item item, Player player);
        internal static void TryUnlimBuffPatch(orig_TryUnlimBuff orig, Item item, Player player)
        {
            if (item.IsAir || !ModContent.GetInstance<Fargowiltas.Common.Configs.FargoServerConfig>().UnlimitedPotionBuffsOn120  || unlimitedBlackList.Contains(item.type)) return;

            if (thoriumBuffNPCs.ContainsKey(item.type) && item.stack >= 5)
            {
                player.AddBuff(thoriumBuffNPCs[item.type], 2);
            }

            // calling orig causes a crash so here's this
            if (item.stack >= 30 && item.buffType != 0)
            {
                player.AddBuff(item.buffType, 2);

                //compensate to account for luck potion being weaker based on remaining duration wtf
                if (item.type == ItemID.LuckPotion)
                    player.GetModPlayer<Fargowiltas.FargoPlayer>().luckPotionBoost = Math.Max(player.GetModPlayer<Fargowiltas.FargoPlayer>().luckPotionBoost, 0.1f);
                else if (item.type == ItemID.LuckPotionGreater)
                    player.GetModPlayer<Fargowiltas.FargoPlayer>().luckPotionBoost = Math.Max(player.GetModPlayer<Fargowiltas.FargoPlayer>().luckPotionBoost, 0.2f);
            }
        }

        public override void Unload()
        {
            if (DeviantHooks.SetChatButtons != null) DeviantHooks.SetChatButtons.Undo();
            if (DeviantHooks.OnChatButtonClicked != null) DeviantHooks.OnChatButtonClicked.Undo();
            //DeviantHooks.AddShops.Undo();

            if (LumberHooks.OnChatButtonClicked != null) LumberHooks.OnChatButtonClicked.Undo();
            if (LumberHooks.AddShops != null) LumberHooks.AddShops.Undo();
            if (TryUnlimBuff != null) TryUnlimBuff.Undo();

            thoriumBuffNPCs.Clear();
            unlimitedBlackList.Clear();
        }
    }
}