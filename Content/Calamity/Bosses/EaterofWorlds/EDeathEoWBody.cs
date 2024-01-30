﻿using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.EaterofWorlds
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathEoWBody : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(timer);
            binaryWriter.Write7BitEncodedInt(time);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            timer = binaryReader.Read7BitEncodedInt();
            time = binaryReader.Read7BitEncodedInt();
        }
        int timer = 800;
        int time = 0;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            Player target = Main.player[npc.target];
            if (time == 0) time = Main.rand.Next(1000, 2000);
            if (NPC.CountNPCS(NPCID.EaterofWorldsBody) + NPC.CountNPCS(NPCID.EaterofWorldsHead) + NPC.CountNPCS(NPCID.EaterofWorldsTail) <= 60)
            {
                timer++;
                if (timer >= time)
                {

                    timer = 0;
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 10, ProjectileID.CursedFlameHostile, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                }
            }
            return base.SafePreAI(npc);
        }
    }
}
