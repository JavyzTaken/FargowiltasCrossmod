
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using FargowiltasCrossmod.Core;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cultist
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathCultist : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.CultistBoss);

        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            
            return base.SafePreAI(npc);
        }
    }
}
