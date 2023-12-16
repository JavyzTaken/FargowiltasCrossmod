using CalamityMod.Events;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.QueenBee
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathQB : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.QueenBee);

        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            if (!BossRushEvent.BossRushActive)
                npc.position += npc.velocity * 0.75f;
            return base.SafePreAI(npc);
        }
    }
}
