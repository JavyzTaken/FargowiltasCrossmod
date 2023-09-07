using CalamityMod;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using System.Linq;
using System.Reflection;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Toggler;
using System;
using System.Collections.Generic;
using FargowiltasCrossmod.Content.Calamity.Toggles;

namespace FargowiltasCrossmod;

public class FargowiltasCrossmod : Mod
{
    public override void Load()
    {
        if (ModLoader.TryGetMod(ModCompatibility.Calamity.Name, out Mod calamity))
        {
            _ = new EternityRevDifficulty();
        }
        if (ModLoader.HasMod("CalamityMod"))
        {
            LoadTogglesFromType(typeof(CalamityToggles));
        }
    }
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
    public override void PostSetupContent()
    {
        CalamityLists.pierceResistExceptionList.Add(ProjectileID.FinalFractal);
    }
}