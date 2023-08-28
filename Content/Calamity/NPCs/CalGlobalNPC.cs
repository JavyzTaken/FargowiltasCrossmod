
using CalamityMod;
using CalamityMod.NPCs.NormalNPCs;

using FargowiltasCrossmod.Core.Calamity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.NPCs
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalGlobalNPC : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            //make golem not fly
            if (npc.type == NPCID.Golem && ModContent.GetInstance<Core.Calamity.CalamityConfig>().RevVanillaAIDisabled)
            {
                npc.noGravity = false;
            }
            if (npc.type == NPCID.GolemHeadFree && ModContent.GetInstance<Core.Calamity.CalamityConfig>().RevVanillaAIDisabled)
            {
                npc.dontTakeDamage = true;
            }
            //make destroyer not invincible
            if (npc.type >= NPCID.TheDestroyer && npc.type <= NPCID.TheDestroyerTail && ModContent.GetInstance<Core.Calamity.CalamityConfig>().RevVanillaAIDisabled)
            {
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.Call("SetCalamityAI", npc, 1, 600f);
                calamity.Call("SetCalamityAI", npc, 2, 0f);
            }
            //make plantera not summon free tentacles
            if (npc.type == ModContent.NPCType<PlanterasFreeTentacle>() && ModContent.GetInstance<Core.Calamity.CalamityConfig>().RevVanillaAIDisabled)
            {
                npc.StrikeInstantKill();
            }
            return base.PreAI(npc);
        }
    }
}
