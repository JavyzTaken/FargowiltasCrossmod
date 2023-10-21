using CalamityMod;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Toggler;
using System;
using System.Collections.Generic;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using System.IO;
using Terraria.Graphics.Effects;
using FargowiltasCrossmod.Content.Common.Sky;
using FargowiltasCrossmod.Content.Common.Bosses.Mutant;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;

namespace FargowiltasCrossmod;

public class FargowiltasCrossmod : Mod
{
    internal static FargowiltasCrossmod Instance;
    public override void Load()
    {
        Instance = this;
        /*
        #region Boss Checklist edits
        if (ModCompatibility.SoulsMod.Mod.Version >= Version.Parse("1.6.1") && ModCompatibility.Calamity.Loaded && ModCompatibility.BossChecklist.Loaded)
        {
            ref Dictionary<string, float> soulsChecklistValues = ref (ModCompatibility.SoulsMod.Mod as FargowiltasSouls.FargowiltasSouls).BossChecklistValues;
            //cal values are internal. thanks cal!
            soulsChecklistValues["AbomBoss"] = 22.6f;
            soulsChecklistValues["MutantBoss"] = 25.9f;
            foreach (string entry in soulsChecklistValues.Keys)
            {
                if (entry.Contains("Champion"))
                {
                    soulsChecklistValues[entry] += 1; //ends up being 19.x, aka post provi
                }
            }
        }
        #endregion
        */
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
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public override void PostSetupContent()
    {
        if (ModCompatibility.Calamity.Loaded)
        {
            pierceResistExceptionList.Add(ProjectileID.FinalFractal);
        }
        if (MutantDLC.ShouldDoDLC)
        {
            SkyManager.Instance["FargowiltasSouls:MutantBoss"] = new MutantDLCSky();
        }

    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) => PacketManager.ReceivePacket(reader);
}