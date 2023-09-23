using CalamityMod;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Toggler;
using System;
using System.Collections.Generic;

namespace FargowiltasCrossmod;

public class FargowiltasCrossmod : Mod
{
    public override void Load()
    {
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
    public static ref List<int> pierceResistExceptionList => ref CalamityLists.pierceResistExceptionList;

    [JITWhenModsEnabled("CalamityMod")]
    public override void PostSetupContent()
    {
        if (ModLoader.HasMod("CalamityMod"))
         pierceResistExceptionList.Add(ProjectileID.FinalFractal);
    }
}