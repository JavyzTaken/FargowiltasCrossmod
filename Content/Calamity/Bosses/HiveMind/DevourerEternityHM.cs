using CalamityMod.Events;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DevourerEternityHM : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DevourerHead);
        private bool FromHM = false;
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (CalDLCWorldSavingSystem.E_EternityRev)
            {
                if (source is EntitySource_Parent parent && parent.Entity is NPC parentNPC && parentNPC.type == ModContent.NPCType<CalamityMod.NPCs.HiveMind.HiveMind>() && npc.TryGetGlobalNPC(out HMEternity _))
                {
                    FromHM = true;
                }
            }
            base.OnSpawn(npc, source);
        }
        public override bool SafePreAI(NPC npc)
        {
            if (FromHM)
            {
                Main.NewText("e");
                npc.velocity.Y += 0.3f;
                int closestPlayer = Player.FindClosest(npc.Center, 0, 0);
                if (closestPlayer.IsWithinBounds(Main.maxPlayers))
                {
                    Player player = Main.player[closestPlayer];
                    if (player.Alive())
                    {
                        if (player.Distance(npc.Center) > 1500)
                            npc.active = false;
                    }
                }
                return false;
            }
            return base.SafePreAI(npc);
        }

    }
}
