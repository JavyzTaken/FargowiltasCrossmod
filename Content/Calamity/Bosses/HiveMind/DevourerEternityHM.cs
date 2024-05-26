using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
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
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DevourerEternityHM : CalDLCEmodeExtraGlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange([NPCID.DevourerHead, NPCID.EaterofSouls]);
        public bool FromHM = false;
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }
        public int timer = 0;
        public override bool SafePreAI(NPC npc)
        {
            if (CalDLCWorldSavingSystem.E_EternityRev && CalamityGlobalNPC.hiveMind.IsWithinBounds(Main.maxNPCs) && Main.npc[CalamityGlobalNPC.hiveMind] is NPC hiveMind && hiveMind.TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>() && hiveMind.GetDLCBehavior<HMEternity>().Phase < 2)
                npc.active = false;
            if (FromHM && npc.type == NPCID.DevourerHead)
            {
                timer++;
                if (timer > 60 * 4)
                {
                    npc.Opacity -= 1f / 60;
                    if (npc.Opacity < 0.05f)
                        npc.active = false;
                }
            }
            return true;
        }

    }
}
