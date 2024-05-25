using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.DukeFishron
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathDuke : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.DukeFishron;

        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;
            if (NPC.ai[0] == 10 && NPC.ai[3] == 8 && NPC.ai[2] >= 20 && NPC.GetLifePercent() <= 0.1f)
            {
                NPC.Center = Main.player[NPC.target].Center + Main.player[NPC.target].velocity * 100;
                if (NPC.ai[2] == 25)
                    NPC.ai[3] = 5;
            }
            if (NPC.ai[3] == 1 && NPC.GetLifePercent() <= 0.1f)
            {

                NPC.ai[3] = 5;
            }
            return true;
        }
    }
}
