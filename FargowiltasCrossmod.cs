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
using ThoriumMod.NPCs;

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

        private struct LumberHooks
        {
            internal static Hook OnChatButtonClicked;
            internal static Hook AddShops;
        }

        private void LoadDetours()
        {
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

            //Type CaughtNPCType = ModContent.Find<ModItem>("Fargowiltas/Items/CaughtNPCs/CaughtNPCItem").GetType(); Doesn't work because this is in load
            Type CaughtNPCType = ModCompatibility.MutantMod.Mod.GetType().Assembly.GetType("Fargowiltas.Items.CaughtNPCs.CaughtNPCItem", true);

            if (CaughtNPCType != null)
            {
                CaughtTownies = (Dictionary<int, int>)CaughtNPCType.GetField("CaughtTownies", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            }
        }

        public static readonly List<int> UnlimitedPotionBlacklist = new();
        public void Thorium_Load()
        {
            Assembly ThoriumAssembly = ModLoader.GetMod("ThoriumMod").GetType().Assembly;
            Type thoriumProjExtensions = ThoriumAssembly.GetType("ThoriumMod.Utilities.ProjectileHelper", true);

            Content.Thorium.Projectiles.DLCHealing.HealMethod = thoriumProjExtensions.GetMethod("ThoriumHeal", BindingFlags.Static | BindingFlags.NonPublic);
            Content.Thorium.Projectiles.DLCHealing.CustomHealingType = thoriumProjExtensions.GetNestedType("CustomHealing", BindingFlags.NonPublic);
            MonoModHooks.Modify(thoriumProjExtensions.GetMethod("ThoriumHealTarget", BindingFlags.Static | BindingFlags.NonPublic), Content.Thorium.Projectiles.DLCHealing.DLCOnHealEffects_ILEdit);

            Content.Thorium.Items.Accessories.Enchantments.YewWoodEnchant.LoadModdedAmmo();

            RegisterThoriumCaughtNPCs();

        }

        internal Dictionary<int, int> CaughtTownies;
        public void RegisterThoriumCaughtNPCs()
        {
            void Add(string internalName, int id, string quote)
            {
                Fargowiltas.Items.CaughtNPCs.CaughtNPCItem item = new(internalName, id, quote);
                AddContent(item);
                CaughtTownies.Add(id, item.Type);
            }

            Add("Cobbler", ModContent.NPCType<Cobbler>(), "''");
            Add("DesertAcolyte", ModContent.NPCType<DesertAcolyte>(), "''");
            Add("Cook", ModContent.NPCType<Cook>(), "'I am the danger'");
            Add("ConfusedZombie", ModContent.NPCType<ConfusedZombie>(), "'Guh?!'");
            Add("Blacksmith", ModContent.NPCType<Blacksmith>(), "''");
            Add("Tracker", ModContent.NPCType<Tracker>(), "''");
            Add("Diverman", ModContent.NPCType<Diverman>(), "'Sam?'");
            Add("Druid", ModContent.NPCType<Druid>(), "''");
            Add("Spiritualist", ModContent.NPCType<Spiritualist>(), "''");
            Add("WeaponMaster", ModContent.NPCType<WeaponMaster>(), "''");
        }

        public override void PostSetupContent()
        {
            if (ThoriumLoaded) 
            {
                AddThoriumStats();
                Core.Globals.DevianttGlobalNPC.AddThoriumShop();
            }
        }

        public static void AddThoriumStats()
        {
            double Damage(DamageClass damageClass) => Math.Round(Main.LocalPlayer.GetTotalDamage(damageClass).Additive * Main.LocalPlayer.GetTotalDamage(damageClass).Multiplicative * 100 - 100);
            int Crit(DamageClass damageClass) => (int)Main.LocalPlayer.GetTotalCritChance(damageClass);

            void Add<T>(Func<string> func) where T : ModItem => ModCompatibility.MutantMod.Mod.Call("AddStat", ModContent.ItemType<T>(), func);

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
            if (LumberHooks.OnChatButtonClicked != null) LumberHooks.OnChatButtonClicked.Undo();
            if (LumberHooks.AddShops != null) LumberHooks.AddShops.Undo();
        }

        internal enum PacketID : byte
        {
            RequestFallenPaladinUsed
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketID type = (PacketID)reader.ReadByte();

            if (!Enum.IsDefined(type))
            {
                return;
            }

            switch (type)
            {
                case PacketID.RequestFallenPaladinUsed:
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        int healer = reader.ReadInt32();
                        if (healer != Main.myPlayer)
                        {
                            Player healerPlayer = Main.player[healer];
                            for (int i = 0; i < Main.LocalPlayer.buffType.Length; i++)
                            {
                                if (Content.Thorium.Items.Accessories.Enchantments.FallenPaladinEnchant.WhiteList.Contains(Main.LocalPlayer.buffType[i]))
                                {
                                    healerPlayer.AddBuff(Main.LocalPlayer.buffType[i], Main.LocalPlayer.buffTime[i], false);
                                }
                            }
                            Main.LocalPlayer.AddBuff(ModContent.BuffType<Content.Thorium.Buffs.FallenPaladinBuff>(), 2); 
                        }
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = GetPacket();
                        packet.Write((byte)PacketID.RequestFallenPaladinUsed);
                        packet.Write(whoAmI);
                        packet.Send();
                    }
                    break;
            }
        }
    }
}