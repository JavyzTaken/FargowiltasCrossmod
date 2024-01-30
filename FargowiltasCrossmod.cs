using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using FargowiltasCrossmod.Content.Common;
using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Skies;
using FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen;
using FargowiltasCrossmod.Content.Common.Bosses.Mutant;
using FargowiltasCrossmod.Content.Common.Sky;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common.Globals;
using FargowiltasSouls;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Toggler;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod;

public class FargowiltasCrossmod : Mod
{
    internal static FargowiltasCrossmod Instance;
    public override void Load()
    {
        Instance = this;
        LoadDetours();

        ModCompatibility.BossChecklist.AdjustValues();
    }
    public override void Unload()
    {
        Instance = null;
    }

    /* no need for this anymore
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public static void LoadTogglesFromType(Type type)
    {

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
            LumberHooks.OnChatButtonClicked?.Undo();
        }

        internal enum PacketID : byte
        {
            RequestFallenPaladinUsed
        }
    }
    */
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public static ref List<int> pierceResistExceptionList => ref CalamityLists.pierceResistExceptionList;
    public override void PostSetupContent()
    {
        if (ModCompatibility.Calamity.Loaded)
        {
            PostSetupContent_Calamity();
            SkyManager.Instance["FargowiltasCrossmod:Permafrost"] = new PermafrostSky();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketID type = (PacketID)reader.ReadByte();

            if (!Enum.IsDefined(type))
            {
                return;
            }

        if (MutantDLC.ShouldDoDLC)
        {
            SkyManager.Instance["FargowiltasSouls:MutantBoss"] = new MutantDLCSky();
        }
    }
    //[JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    //public DamageClass rogueDamageClass => ModContent.GetInstance<RogueDamageClass>();
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public void PostSetupContent_Calamity()
    {
        pierceResistExceptionList.Add(ProjectileID.FinalFractal);
        
        /* doesn't seem to be working, may investigate later
        List<int> CalamityReworkedSpears = new List<int>
        {
            ModContent.ItemType<AstralPike>()
        };
        SpearRework.ReworkedSpears.AddRange(CalamityReworkedSpears);
        */
        
        /*
         * PR'd to Calamity
        #region Stat Sheet
        double Damage(DamageClass damageClass) => Math.Round(Main.LocalPlayer.GetTotalDamage(damageClass).Additive * Main.LocalPlayer.GetTotalDamage(damageClass).Multiplicative * 100 - 100);
        int Crit(DamageClass damageClass) => (int)Main.LocalPlayer.GetTotalCritChance(damageClass);

        int rogueItem = ModContent.ItemType<WulfrumKnife>();
        Func<string> rogueDamage = () => $"Rogue Damage: {Damage(rogueDamageClass)}%";
        Func<string> rogueCrit = () => $"Rogue Critical: {Crit(rogueDamageClass)}%";
        ModCompatibility.MutantMod.Mod.Call("AddStat", rogueItem, rogueDamage);
        ModCompatibility.MutantMod.Mod.Call("AddStat", rogueItem, rogueCrit);
        #endregion
        */
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
    public override void HandlePacket(BinaryReader reader, int whoAmI) => PacketManager.ReceivePacket(reader);
}