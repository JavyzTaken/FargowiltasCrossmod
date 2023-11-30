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
            LoadDetours();
        }

        private struct LumberHooks
        {
            internal static Hook OnChatButtonClicked;
        }

        private static void LoadDetours()
        {
            // lumberjack stuff will be removed when someone makes a better tree treasures system.
            Type lumberDetourClass = ModContent.Find<ModNPC>("Fargowiltas/LumberJack").GetType();

            if (lumberDetourClass != null)
            {
                MethodInfo OnChatButtonClicked_DETOUR = lumberDetourClass.GetMethod("OnChatButtonClicked", BindingFlags.Public | BindingFlags.Instance);
                MethodInfo AddShops_DETOUR = lumberDetourClass.GetMethod("AddShops", BindingFlags.Public | BindingFlags.Instance);

                LumberHooks.OnChatButtonClicked = new Hook(OnChatButtonClicked_DETOUR, LumberBoyPatches.OnChatButtonClicked);

                LumberHooks.OnChatButtonClicked.Apply();
            }

            //Type CaughtNPCType = ModContent.Find<ModItem>("Fargowiltas/Items/CaughtNPCs/CaughtNPCItem").GetType(); Doesn't work because this is in load(), this has to be in load() to add() content
            Type CaughtNPCType = ModCompatibility.MutantMod.Mod.GetType().Assembly.GetType("Fargowiltas.Items.CaughtNPCs.CaughtNPCItem", true);

            if (CaughtNPCType != null)
            {
                CaughtTownies = (Dictionary<int, int>)CaughtNPCType.GetField("CaughtTownies", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            }
        }

        public static readonly List<int> UnlimitedPotionBlacklist = new();

        internal static Dictionary<int, int> CaughtTownies;

        public override void Unload()
        {
            if (LumberHooks.OnChatButtonClicked != null) LumberHooks.OnChatButtonClicked.Undo();
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