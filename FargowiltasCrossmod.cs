using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod;
using FargowiltasCrossmod.Content.Common.Bosses.Mutant;
using FargowiltasCrossmod.Content.Common.Sky;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls;
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
    }
    public override void Unload()
    {
        Instance = null;
    }

    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
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
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public static ref List<int> pierceResistExceptionList => ref CalamityLists.pierceResistExceptionList;

    public override void PostSetupContent()
    {
        if (ModCompatibility.Calamity.Loaded)
        {
            PostSetupContent_Calamity();
        }

        if (MutantDLC.ShouldDoDLC)
        {
            SkyManager.Instance["FargowiltasSouls:MutantBoss"] = new MutantDLCSky();
        }
    }

    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public void PostSetupContent_Calamity()
    {
        pierceResistExceptionList.Add(ProjectileID.FinalFractal);

        #region Stat Sheet
        double Damage(DamageClass damageClass) => Math.Round(Main.LocalPlayer.GetTotalDamage(damageClass).Additive * Main.LocalPlayer.GetTotalDamage(damageClass).Multiplicative * 100 - 100);
        int Crit(DamageClass damageClass) => (int)Main.LocalPlayer.GetTotalCritChance(damageClass);

        //int rogueItem = ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.WulfrumKnife>();
        //Func<string> rogueDamage = () => $"Rogue Damage: {Damage(ModContent.GetInstance<RogueDamageClass>())}%";
        //ModCompatibility.MutantMod.Mod.Call("AddStat", rogueItem, rogueDamage);
        #endregion
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) => PacketManager.ReceivePacket(reader);
}