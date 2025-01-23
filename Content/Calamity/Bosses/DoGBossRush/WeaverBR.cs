using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ProvidenceBossRush
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class WeaverBR : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<CalamityMod.NPCs.StormWeaver.StormWeaverHead>();
        }
        public int attackTimer = 0;
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(attackTimer);
            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            attackTimer = binaryReader.Read7BitEncodedInt();
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }
        
        public override bool PreAI(NPC npc)
        {
            if (!BossRushEvent.BossRushActive)
                return base.PreAI(npc);
            CalamityGlobalNPC calnpc = npc.Calamity();
            StormWeaverHead weav = (StormWeaverHead)npc.ModNPC;
            //Main.NewText("ai: " + npc.ai[0] + ", " + npc.ai[1] + ", " + npc.ai[2] + ", " + npc.ai[3]);
            //Main.NewText("calai: " + calnpc.newAI[0] + ", " + calnpc.newAI[1] + ", " + calnpc.newAI[2] + ", " + calnpc.newAI[3]);
            if (npc.GetLifePercent() > 0.8f)
            {
                npc.life = (int)(npc.lifeMax * 0.79f);
            }
            return base.PreAI(npc);
        }
    }
}
