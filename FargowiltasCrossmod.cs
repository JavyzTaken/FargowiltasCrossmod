using CalamityMod;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Toggler;
using System;
using System.Collections.Generic;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Systems;
using FargowiltasCrossmod.Core.Calamity;

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

    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public override void PostSetupContent()
    {
        if (ModCompatibility.Calamity.Loaded)
         pierceResistExceptionList.Add(ProjectileID.FinalFractal);
        
    }
}