using CalamityMod.NPCs.DesertScourge;
using FargowiltasCrossmod.Core.Calamity.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.DesertScourge
{
    public abstract class NuisanceEternity : CalDLCEmodeBehavior
    {
        public override void SetDefaults()
        {
            NPC.lifeMax /= 3;
        }
    }
    public class NuisanceHeadEternity : NuisanceEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<DesertNuisanceHead>();
    }
    public class NuisanceBodyEternity : NuisanceEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<DesertNuisanceBody>();
    }
    public class NuisanceTailEternity : NuisanceEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<DesertNuisanceTail>();
    }
}
