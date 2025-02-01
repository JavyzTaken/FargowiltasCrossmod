using CalamityMod.Events;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalamityAIs.CalamityBossAIs;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
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
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ProvidenceBossRush
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class YharonBR : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<CalamityMod.NPCs.Yharon.Yharon>();
        }
        public int FuckerSpawnCounter;
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(FuckerSpawnCounter);
            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            FuckerSpawnCounter = binaryReader.Read7BitEncodedInt();
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }
        
        public override bool PreAI(NPC npc)
        {
            if (!BossRushEvent.BossRushActive)
                return base.PreAI(npc);
            if (npc.GetLifePercent() <= 0.53f && FuckerSpawnCounter == 0)
            {
                FuckerSpawnCounter = 1;
                NPC.NewNPCDirect(npc.GetSource_FromAI(), npc.Center, ModContent.NPCType<Bumblefuck>());
                SoundEngine.PlaySound(SoundID.DD2_BetsyScream with { Pitch = 0.25f }, npc.Center);
            } 
            return base.PreAI(npc);
        }
    }
}
