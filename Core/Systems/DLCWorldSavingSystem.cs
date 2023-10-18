using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls.Core.Systems;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Core.Systems
{
    public class DLCWorldSavingSystem : ModSystem
    {
        private static bool eternityRev;
        private static bool eternityDeath;
        public static bool EternityRev {
            get => eternityRev; 
            set => eternityRev = value; 
        }
        public static bool EternityDeath { 
            get => eternityDeath; 
            set => eternityDeath = value; 
        }
        public static bool E_EternityRev => EternityRev && WorldSavingSystem.EternityMode && DLCCalamityConfig.Instance.EternityPriorityOverRev;
        public static bool R_EternityRev = EternityRev && !DLCCalamityConfig.Instance.EternityPriorityOverRev;

        public override void OnWorldLoad()
        {
            EternityRev = false;
            EternityDeath = false;
        }
        public override void OnWorldUnload()
        {
            EternityRev = false;
            EternityDeath = false;
        }
        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new();
            flags[0] = EternityRev;
            flags[1] = EternityDeath;
            writer.Write(flags);
        }
        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            EternityRev = flags[0];
            EternityDeath = flags[1];

        }
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
