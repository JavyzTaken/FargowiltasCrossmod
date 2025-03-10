using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using CalamityMod.NPCs.NormalNPCs;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Calamity.EnemyBehavior
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CnidironEternity : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<Cnidrion>();
        public override void OnKill()
        {
            if (FargoSoulsUtil.HostCheck)
            {
                List<int> jellyfish = [NPCID.BlueJellyfish, NPCID.GreenJellyfish, NPCID.PinkJellyfish];
                for (int i = -1; i <= 1; i++)
                {
                    Vector2 vel = -Vector2.UnitY * 8 + Vector2.UnitX * i * 3;
                    Vector2 dir = vel.SafeNormalize(-Vector2.UnitY);
                    Vector2 pos = NPC.Center + dir * 20;
                    int type = jellyfish[i + 1];
                    int n = FargoSoulsUtil.NewNPCEasy(NPC.GetSource_FromThis(), pos, type, velocity: vel);
                    if (n.IsWithinBounds(Main.maxNPCs))
                    {
                        Main.npc[n].FargoSouls().CanHordeSplit = false;
                        Main.npc[n].knockBackResist = 0;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    }
                }
            }
        }
    }
}
