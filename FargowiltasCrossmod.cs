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

namespace FargowiltasCrossmod
{
	public class FargowiltasCrossmod : Mod
	{
		internal static bool CalamityLoaded;
		internal static bool ThoriumLoaded;


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
            ThoriumLoaded = ModLoader.HasMod("ThoriumMod");
            CalamityLoaded = ModLoader.HasMod("CalamityMod");
            LoadDetours();

            if (ThoriumLoaded) Thorium_Load();
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

        public static readonly List<int> UnlimitedPotionBlacklist = new();
        public static bool PotionTypeInBlacklist(int type) => UnlimitedPotionBlacklist.Contains(type);
        public void Thorium_Load()
        {
            Assembly ThoriumAssembly = ModLoader.GetMod("ThoriumMod").GetType().Assembly;
            Type thoriumProjExtensions = ThoriumAssembly.GetType("ThoriumMod.Utilities.ProjectileHelper", true);

            Content.Thorium.Projectiles.DLCHealing.HealMethod = thoriumProjExtensions.GetMethod("ThoriumHeal", BindingFlags.Static | BindingFlags.NonPublic);
            Content.Thorium.Projectiles.DLCHealing.CustomHealingType = thoriumProjExtensions.GetNestedType("CustomHealing", BindingFlags.NonPublic);

            Content.Thorium.Items.Accessories.Enchantments.YewWoodEnchant.LoadModdedAmmo();

            MonoModHooks.Modify(thoriumProjExtensions.GetMethod("ThoriumHealTarget", BindingFlags.Static | BindingFlags.NonPublic), Content.Thorium.Projectiles.DLCHealing.DLCOnHealEffects_ILEdit);

            DevianttPatches.AddThoriumDeviShop();
        }

        public override void PostSetupContent()
        {
            Mod mutantMod = ModCompatibility.MutantMod.Mod;
            double Damage(DamageClass damageClass) => Math.Round(Main.LocalPlayer.GetTotalDamage(damageClass).Additive * Main.LocalPlayer.GetTotalDamage(damageClass).Multiplicative * 100 - 100);
            int Crit(DamageClass damageClass) => (int)Main.LocalPlayer.GetTotalCritChance(damageClass);

            void Add<T>(Func<string> func) where T : ModItem => mutantMod.Call("AddStat", ModContent.ItemType<T>(), func);

            Add<ThoriumMod.Items.HealerItems.WoodenBaton>(() => $"Radiant Damage: {Damage(ModContent.GetInstance<ThoriumMod.HealerDamage>())}%");
            Add<ThoriumMod.Items.HealerItems.WoodenBaton>(() => $"Radiant Critical: {Crit(ModContent.GetInstance<ThoriumMod.HealerDamage>())}%");

            Add<ThoriumMod.Items.BardItems.WoodenWhistle>(() => $"Synphonic Damage: {Damage(ModContent.GetInstance<ThoriumMod.BardDamage>())}%");
            Add<ThoriumMod.Items.BardItems.WoodenWhistle>(() => $"Synphonic Critical: {Crit(ModContent.GetInstance<ThoriumMod.BardDamage>())}%");

            Add<ThoriumMod.Items.ThrownItems.StoneThrowingSpear>(() => $"Throwing Damage: {Damage(DamageClass.Throwing)}%");
            Add<ThoriumMod.Items.ThrownItems.StoneThrowingSpear>(() => $"Throwing Critical: {Crit(DamageClass.Throwing)}%");

            Add<ThoriumMod.Items.BossMini.TheGoodBook>(() => $"Bonus Healing: {Main.LocalPlayer.Thorium().healBonus}");
            Add<ThoriumMod.Items.HealerItems.HoneyHeart>(() => $"Life recovery: {Main.LocalPlayer.Thorium().lifeRecovery}/ sec");

            Add<ThoriumMod.Items.BardItems.InspirationFragment>(() => $"Inspiration: {Main.LocalPlayer.Thorium().bardResourceMax2}");
            Add<ThoriumMod.Items.BardItems.InspirationNote>(() => $"Inspiration Regen: {Main.LocalPlayer.Thorium().inspirationRegenBonus}/ sec");
        }

        public override void Unload()
        {
            if (DeviantHooks.SetChatButtons != null) DeviantHooks.SetChatButtons.Undo();
            if (DeviantHooks.OnChatButtonClicked != null) DeviantHooks.OnChatButtonClicked.Undo();
            //DeviantHooks.AddShops.Undo();

            if (LumberHooks.OnChatButtonClicked != null) LumberHooks.OnChatButtonClicked.Undo();
            if (LumberHooks.AddShops != null) LumberHooks.AddShops.Undo();
        }
    }
}