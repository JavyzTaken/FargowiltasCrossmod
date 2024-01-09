using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Globals;
using ThoriumMod.NPCs.BossTheGrandThunderBird;
using FargowiltasSouls.Core.NPCMatching;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace FargowiltasCrossmod.Content.Thorium.EternityMode.Enemy
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GFBHatling : EModeNPCBehaviour
    {
        //public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(-1);
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<StormHatchling>());

        public override void SetDefaults(NPC npc)
        {
            npc.damage = 10;
            npc.lifeMax = 80;
            npc.life = 80;
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);
        }

        public override bool SafePreAI(NPC npc)
        {
            // too tired
            return false;
        }
    }
}
