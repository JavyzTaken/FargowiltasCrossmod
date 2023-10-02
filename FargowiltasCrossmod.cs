using CalamityMod;
using Terraria.ID;
using Terraria.ModLoader; 
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
//using FargowiltasCrossmod.Content.Common;
using System;
using FargowiltasSouls.Core.Toggler;
using System.Collections.Generic;
using FargowiltasCrossmod.Core;
using System.Reflection;
using System.Linq;

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
        [JITWhenModsEnabled("CalamityMod")]
        public static void LoadTogglesFromType(Type type)
        {

            ToggleCollection toggles = (ToggleCollection)Activator.CreateInstance(type);

            if (toggles.Active)
            {
                ModContent.GetInstance<FargowiltasCrossmod>().Logger.Info($"ToggleCollection found: {nameof(type)}");
                List<Toggle> toggleCollectionChildren = toggles.Load();
                foreach (Toggle toggle in toggleCollectionChildren)
                {
                    ToggleLoader.RegisterToggle(toggle);
                }
            }
        }
        [JITWhenModsEnabled("CalamityMod")]
        public static ref List<int> pierceResistExceptionList
        {
            get { return ref CalamityLists.pierceResistExceptionList; }
        }
        [JITWhenModsEnabled("CalamityMod")]
        public override void PostSetupContent()
        {
            if (ModLoader.HasMod("CalamityMod"))
                pierceResistExceptionList.Add(ProjectileID.FinalFractal);
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


                //DeviantHooks.SetChatButtons = new Hook(SetChatButtons_DETOUR, DevianttPatches.SetChatButtons);
                //DeviantHooks.OnChatButtonClicked = new Hook(OnChatButtonClicked_DETOUR, DevianttPatches.OnChatButtonClicked);
                //DeviantHooks.AddShops = new Hook(AddShops_DETOUR, DevianttPatches.AddShops);

                //DeviantHooks.SetChatButtons.Apply();
                //DeviantHooks.OnChatButtonClicked.Apply();
                //DeviantHooks.AddShops.Apply();
            }

            Type lumberDetourClass = ModContent.Find<ModNPC>("Fargowiltas/LumberJack").GetType();

            if (lumberDetourClass != null)
            {
                MethodInfo OnChatButtonClicked_DETOUR = lumberDetourClass.GetMethod("OnChatButtonClicked", BindingFlags.Public | BindingFlags.Instance);
                MethodInfo AddShops_DETOUR = lumberDetourClass.GetMethod("AddShops", BindingFlags.Public | BindingFlags.Instance);

                //LumberHooks.OnChatButtonClicked = new Hook(OnChatButtonClicked_DETOUR, LumberBoyPatches.OnChatButtonClicked);
                //LumberHooks.AddShops = new Hook(AddShops_DETOUR, LumberBoyPatches.AddShops);

                //LumberHooks.OnChatButtonClicked.Apply();
                //LumberHooks.AddShops.Apply();
            }
        }

        public void Thorium_Load()
        {
            Assembly ThoriumAssembly = ModLoader.GetMod("ThoriumMod").GetType().Assembly;
            Type thoriumProjExtensions = ThoriumAssembly.GetType("ThoriumMod.Utilities.ProjectileHelper", true);

            Content.Thorium.Projectiles.DLCHealing.HealMethod = thoriumProjExtensions.GetMethod("ThoriumHeal", BindingFlags.Static | BindingFlags.NonPublic);
            Content.Thorium.Projectiles.DLCHealing.CustomHealingType = thoriumProjExtensions.GetNestedType("CustomHealing", BindingFlags.NonPublic);

            //DevianttPatches.AddThoriumDeviShop();
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