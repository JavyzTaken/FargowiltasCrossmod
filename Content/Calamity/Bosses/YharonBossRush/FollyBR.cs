using CalamityMod.Events;
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
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ProvidenceBossRush
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class FollyBR : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<CalamityMod.NPCs.Bumblebirb.Bumblefuck>();
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
            NPC yhar = Main.npc[CalamityMod.NPCs.CalamityGlobalNPC.yharon];
            if (yhar != null && yhar.active)
            {
                //attackTimer = 3;
                if (yhar.ai[0] == 7 && yhar.ai[2] == 0)
                {
                    attackTimer = 1;
                }
                if ((yhar.ai[0] == 3 || yhar.ai[0] == 4 || yhar.ai[0] == 6) && attackTimer < 4)
                {
                    attackTimer = 3;
                }
            }
            if (attackTimer == 0)
            {
                npc.ai[0] = 2;
                npc.ai[2] = 0;
                
            }
            else if (attackTimer == 1)
            {
                npc.ai[0] = 3;
                attackTimer = 2;
            }
            else if (attackTimer == 2)
            {
                if (npc.ai[0] == 0f)
                {
                    attackTimer = 0;
                }
            }
            else if (attackTimer == 3)
            {
                npc.ai[3] = 0;
                npc.ai[0] = 0;
                npc.ai[1] = 30;
                attackTimer = 4;
            }else if (attackTimer >= 4)
            {
                npc.ai[0] = 2;
                npc.ai[2] = 0;
                attackTimer++;
                if (attackTimer >= 120)
                {
                    attackTimer = 0;
                }
            }

            if (attackTimer == 0 || attackTimer > 3)
            {
                if (npc.HasValidTarget && npc.Distance(Main.player[npc.target].Center) < 300)
                {
                    npc.velocity *= 0.9f;
                }
            }
            return base.PreAI(npc);
        }
    }
}
