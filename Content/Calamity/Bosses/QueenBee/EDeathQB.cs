
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using FargowiltasCrossmod.Core;
using Terraria.ModLoader;
using CalamityMod.Events;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.QueenBee
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathQB : EternideathNPC
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
