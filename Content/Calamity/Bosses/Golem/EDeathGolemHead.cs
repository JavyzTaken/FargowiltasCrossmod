
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using FargowiltasCrossmod.Core;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Golem
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathGolemHead : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Golem);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            
            return base.SafePreAI(npc);
        }
    }
}
