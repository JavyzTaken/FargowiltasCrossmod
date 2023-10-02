using FargowiltasCrossmod.Core.Calamity;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Core.Systems
{
    public class DLCWorldSavingSystem : ModSystem
    {
        public static bool EternityRev { get; set; }
        public static bool EternityDeath { get; set; }
        public static bool E_EternityRev => EternityRev && DLCCalamityConfig.Instance.EternityPriorityOverRev;
        public static bool R_EternityRev = EternityRev && !DLCCalamityConfig.Instance.EternityPriorityOverRev; 
        
        public override void SaveWorldData(TagCompound tag)
        {
            if (WorldGen.generatingWorld)
                return;

            var downed = new List<string>();
            if (EternityRev)
                downed.Add("EternityRevActive");
            if (EternityDeath)
                downed.Add("EternityDeathActive");

            tag["downed"] = downed;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");
            EternityRev = downed.Contains("EternityRevActive");
            EternityDeath = downed.Contains("EternityDeathActive");
            
            if (ModLoader.HasMod("InfernumMode"))
            {
                DLCCalamityConfig.Instance.EternityPriorityOverRev = false;
            }
        }
    }
}
