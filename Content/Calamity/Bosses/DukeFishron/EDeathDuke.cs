
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using FargowiltasCrossmod.Core;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.DukeFishron
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathDuke : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DukeFishron);
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
            if (npc.ai[0] == 10 && npc.ai[3] == 8 && npc.ai[2] >= 20 && npc.GetLifePercent() <= 0.1f)
            {
                npc.Center = Main.player[npc.target].Center + Main.player[npc.target].velocity * 100;
                if (npc.ai[2] == 25)
                npc.ai[3] = 5;
            }
            if (npc.ai[3] == 1 && npc.GetLifePercent() <= 0.1f)
            {
                
                npc.ai[3] = 5;
            }
            return base.SafePreAI(npc);
        }
    }
}
