using System.IO;
using CalamityMod.NPCs.NormalNPCs;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.KingSlime
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathKS : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.KingSlime;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(summonedJewel);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            summonedJewel = binaryReader.ReadBoolean();
        }
        public bool summonedJewel = false;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget)
            {
                return true;
            }
            if (NPC.GetLifePercent() <= 0.5 && !summonedJewel)
            {
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 50, ModContent.NPCType<KingSlimeJewelRuby>());
                summonedJewel = true;
                SoundEngine.PlaySound(SoundID.Item38, NPC.Center);
                for (int i = 0; i < 100; i++)
                {
                    Dust.NewDustDirect(NPC.Center + new Vector2(0, -50), 0, 0, DustID.GemRuby).noGravity = true;
                }
            }

            return true;
        }
    }
}
