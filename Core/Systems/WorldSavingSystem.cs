using FargowiltasCrossmod.Core.Calamity;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Core.Systems
{
    public class WorldSavingSystem : ModSystem
    {
        public static bool EternityRev { get; set; }
        public static bool E_EternityRev => EternityRev && CalamityConfig.Instance.EternityPriorityOverRev;
        public static bool R_EternityRev = EternityRev && !CalamityConfig.Instance.EternityPriorityOverRev; 
        
        public override void SaveWorldData(TagCompound tag)
        {
            if (WorldGen.generatingWorld)
                return;

            var downed = new List<string>();
            if (EternityRev)
                downed.Add("EternityRevActive");

            tag["downed"] = downed;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");
            EternityRev = downed.Contains("EternityRevActive");
        }
    }
}
