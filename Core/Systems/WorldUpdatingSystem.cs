﻿using Fargowiltas.NPCs;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Common.Systems
{
    public class WorldUpdatingSystem : ModSystem
    {
        public override void PreUpdateWorld()
        {
            ModLoader.GetMod(ModCompatibility.SoulsMod.Name).Call("EternityVanillaBossBehaviour", ModContent.GetInstance<CalamityConfig>().EternityPriorityOverRev);
        }
        public override void PostUpdateWorld()
        {
            if (ModLoader.TryGetMod(ModCompatibility.Calamity.Name, out Mod cal))
            {
                if (FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode && !FargowiltasSouls.Core.Systems.WorldSavingSystem.SpawnedDevi && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int devi = NPC.NewNPC(new EntitySource_SpawnNPC(), Main.spawnTileX*16, Main.spawnTileY*16 - 400, ModContent.NPCType<Deviantt>());
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, devi);
                    FargowiltasSouls.Core.Systems.WorldSavingSystem.SpawnedDevi = true;
                }
            }
            base.PostUpdateWorld();
        }
    }
}
