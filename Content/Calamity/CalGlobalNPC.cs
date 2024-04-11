using CalamityMod;
using CalamityMod.Buffs;
using CalamityMod.NPCs;
using FargowiltasCrossmod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool WulfrumScanned = false;
        public override void ResetEffects(NPC npc)
        {
            
        }
        public override bool PreAI(NPC npc)
        {
            
            return base.PreAI(npc);
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
        }
    }
}
