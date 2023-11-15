using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Common.Bosses.Mutant
{
    public class MutantDLC : GlobalNPC
    {
        private static bool AnyCompatibleModsLoaded()
        {
            return ModCompatibility.Calamity.Loaded;
        }
        public override bool AppliesToEntity(NPC npc, bool lateInstantiation) => npc.type == ModContent.NPCType<MutantBoss>() && DLCCalamityConfig.Instance.MutantDLC && AnyCompatibleModsLoaded();

        public static void ManageMusic(NPC npc)
        {
            
            if (npc.localAI[3] >= 3)
            {

            }
            else
            {
                npc.ModNPC.Music = MusicLoader.GetMusicSlot("FargowiltasCrossmod/Assets/Music/Irondust");
            }
        }
        public override bool PreAI(NPC npc)
        {
            if (!AnyCompatibleModsLoaded())
            {
                return true;
            }
            ManageMusic(npc);
            //switch ()
            return base.PreAI(npc);

            void CalamityFishron()
            {
                const int fishronDelay = 3;
                int maxFishronSets = WorldSavingSystem.MasochistModeReal ? 3 : 2;
                if (npc.ai[1] == fishronDelay * maxFishronSets)
                {
                    if (DLCUtils.HostCheck)
                    {

                    }
                }
            }
        }

    }
}
