using CalamityMod.NPCs.NormalNPCs;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.KingSlime
{
    public class EDeathKS : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.KingSlime);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write(summonedJewel);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            summonedJewel = binaryReader.ReadBoolean();
        }
        public bool summonedJewel = false;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                return true;
            }
            if (npc.GetLifePercent() <= 0.5 && !summonedJewel)
            {
                NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y - 50, ModContent.NPCType<KingSlimeJewel>());
                summonedJewel = true;
                SoundEngine.PlaySound(SoundID.Item38, npc.Center);
                for (int i = 0; i < 100; i++)
                {
                    Dust.NewDustDirect(npc.Center + new Vector2(0, -50), 0, 0, DustID.GemRuby).noGravity = true;
                }
            }
            
            return base.SafePreAI(npc);
        }
    }
}
