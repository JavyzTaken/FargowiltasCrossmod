using System.Collections.Generic;
using System.IO;
using System.Linq;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls.Core.Systems;
using log4net.Core;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCWorldSavingSystem : ModSystem
    {
        private static bool eternityRev;
        private static bool eternityDeath;

        public static bool downedMiniPlaguebringer = false;
        public static bool downedReaperShark = false;
        public static bool downedColossalSquid = false;
        public static bool downedEidolonWyrm = false;
        public static bool downedCloudElemental = false;
        public static bool downedEarthElemental = false;
        public static bool downedArmoredDigger = false;
        public static bool EternityRev
        {
            get => eternityRev;
            set => eternityRev = value;
        }
        public static bool EternityDeath
        {
            get => eternityDeath;
            set => eternityDeath = value;
        }

        public static bool E_EternityRev => EternityRev && WorldSavingSystem.EternityMode && CalDLCConfig.Instance.EternityPriorityOverRev;
        public static bool R_EternityRev => EternityRev && !CalDLCConfig.Instance.EternityPriorityOverRev;

        internal static bool permafrostPhaseSeen;
        public static bool PermafrostPhaseSeen
        {
            get => permafrostPhaseSeen;
            set => permafrostPhaseSeen = value;
        }

        public static List<int> DroppedSummon = [];

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
        public override void ClearWorld()
        {
            downedMiniPlaguebringer = false;
            downedReaperShark = false;
            downedColossalSquid = false;
            downedEidolonWyrm = false;
            downedCloudElemental = false;
            downedEarthElemental = false;
            downedArmoredDigger = false;
            base.ClearWorld();
        }
        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new();
            flags[0] = EternityRev;
            flags[1] = EternityDeath;
            flags[2] = PermafrostPhaseSeen;

            BitsByte downedFlags = new();
            downedFlags[0] = downedMiniPlaguebringer;
            downedFlags[1] = downedReaperShark;
            downedFlags[2] = downedColossalSquid;
            downedFlags[3] = downedEidolonWyrm;
            downedFlags[4] = downedCloudElemental;
            downedFlags[5] = downedEarthElemental;
            downedFlags[6] = downedArmoredDigger;
            writer.Write(flags);
            writer.Write(downedFlags);
        }
        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            BitsByte downedFlags = reader.ReadByte();
            EternityRev = flags[0];
            EternityDeath = flags[1];
            PermafrostPhaseSeen = flags[2];

            downedMiniPlaguebringer = downedFlags[0];
            downedReaperShark = downedFlags[1];
            downedColossalSquid = downedFlags[2];
            downedEidolonWyrm = downedFlags[3];
            downedCloudElemental = downedFlags[4];
            downedEarthElemental = downedFlags[5];
            downedArmoredDigger = downedFlags[6];
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
            if (PermafrostPhaseSeen)
                downed.Add("PermafrostPhaseSeen");
            if (downedMiniPlaguebringer)
                downed.Add("downedMiniPlaguebringer");
            if (downedReaperShark)
                downed.Add("downedReaperShark");
            if (downedColossalSquid)
                downed.Add("downedColossalSquid");
            if (downedEidolonWyrm)
                downed.Add("downedEidolonWyrm");
            if (downedCloudElemental)
                downed.Add("downedCloudElemental");
            if (downedEarthElemental)
                downed.Add("downedEarthElemental");
            if (downedArmoredDigger)
                downed.Add("downedArmoredDigger");
            tag["downed"] = downed;

            tag["droppedSummon"] = DroppedSummon;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");
            EternityRev = downed.Contains("EternityRevActive");
            EternityDeath = downed.Contains("EternityDeathActive");
            PermafrostPhaseSeen = downed.Contains("PermafrostPhaseSeen");
            downedMiniPlaguebringer = downed.Contains("downedMiniPlaguebringer");
            downedReaperShark = downed.Contains("downedReaperShark");
            downedColossalSquid = downed.Contains("downedColossalSquid");
            downedEidolonWyrm = downed.Contains("downedEidolonWyrm");
            downedCloudElemental = downed.Contains("downedCloudElemental");
            downedEarthElemental = downed.Contains("downedEarthElemental");
            downedArmoredDigger = downed.Contains("downedArmoredDigger");

            DroppedSummon = tag.GetList<int>("droppedSummon").ToList();
            /*
            if (ModCompatibility.InfernumMode.Loaded)
            {
                DLCCalamityConfig.Instance.EternityPriorityOverRev = false;
            }
            */
        }
    }
}
