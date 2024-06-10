
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CryogenShieldEternity : CalDLCEmodeBehavior
    {
        public override bool IsLoadingEnabled(Mod mod) => CryogenEternity.Enabled;
        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.Cryogen.CryogenShield>();
        public override bool PreAI()
        {
            if (!CalDLCConfig.Instance.EternityPriorityOverRev || !CalDLCWorldSavingSystem.EternityRev) return true;
            NPC owner = Main.npc[(int)NPC.ai[0]];
            if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.Cryogen.Cryogen>())
            {
                return true;
            }
            NPC.active = false;
            
            return false;
        }
    }
}
