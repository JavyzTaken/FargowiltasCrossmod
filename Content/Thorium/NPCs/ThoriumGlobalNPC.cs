using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.NPCs;

namespace FargowiltasCrossmod.Content.Thorium.NPCs
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class ThoriumGlobalNPC : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => true;
        //public override bool CheckDead(NPC npc)
        //{
            // can't use switches cus non-constant values
            //if (npc.type == ModContent.NPCType<GildedLycan>()) DLCSystem.TryDowned(npc, "Deviantt", Color.Yellow, "GildedLycan");
            //else if (npc.type == ModContent.NPCType<GildedBat>()) DLCSystem.TryDowned(npc, "Deviantt", Color.Yellow, "GildedBat");
            //else if (npc.type == ModContent.NPCType<GildedSlime>()) DLCSystem.TryDowned(npc, "Deviantt", Color.Yellow, "GildedSlime");
            //else if (npc.type == ModContent.NPCType<Myna>()) DLCSystem.TryDowned(npc, "Deviantt", Color.Yellow, "Myna");
            //return true;
        //}
    }
}
