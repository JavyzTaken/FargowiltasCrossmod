
using CalamityMod;
using CalamityMod.NPCs.NormalNPCs;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Balance
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalNPCChanges : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            if (ModContent.GetInstance<Core.Calamity.CalamityConfig>().RevVanillaAIDisabled)
            {
                return base.PreAI(npc);
            }

            //make golem not fly
            if (npc.type == NPCID.Golem)
            {
                npc.noGravity = false;
            }
            if (npc.type == NPCID.GolemHeadFree)
            {
                npc.dontTakeDamage = true;
            }
            //make destroyer not invincible
            if (npc.type >= NPCID.TheDestroyer && npc.type <= NPCID.TheDestroyerTail)
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.Call("SetCalamityAI", npc, 1, 600f);
                calamity.Call("SetCalamityAI", npc, 2, 0f);
            }
            //make plantera not summon free tentacles
            if (npc.type == ModContent.NPCType<PlanterasFreeTentacle>())
            {
                npc.StrikeInstantKill();
            }
            return base.PreAI(npc);
        }
    }
}
