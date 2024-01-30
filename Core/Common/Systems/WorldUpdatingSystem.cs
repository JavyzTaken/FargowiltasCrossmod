﻿using Fargowiltas.NPCs;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasSouls.Core.Systems;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Common.Systems
{
    public class WorldUpdatingSystem : ModSystem
    {
        public static bool InfernumStateLastFrame = false;
        public override void PreUpdateWorld()
        {
            if (ModCompatibility.Calamity.Loaded)
            {
                ModCompatibility.SoulsMod.Mod.Call("EternityVanillaBossBehaviour", DLCCalamityConfig.Instance.EternityPriorityOverRev);
            }

        }
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override void PostUpdateWorld()
        {
            if (ModCompatibility.Calamity.Loaded)
            {
                if (WorldSavingSystem.EternityMode && !WorldSavingSystem.SpawnedDevi && DLCUtils.HostCheck)
                {
                    int devi = NPC.NewNPC(new EntitySource_SpawnNPC(), Main.spawnTileX * 16, Main.spawnTileY * 16 - 400, ModContent.NPCType<Deviantt>());
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, devi);
                    WorldSavingSystem.SpawnedDevi = true;
                }
            }
            if (ModCompatibility.InfernumMode.Loaded)
            {
                if (ModCompatibility.InfernumMode.InfernumDifficulty && !InfernumStateLastFrame)
                {
                    DLCCalamityConfig.Instance.EternityPriorityOverRev = false;


                    if (DLCCalamityConfig.Instance.InfernumDisablesEternity)
                    {
                        WorldSavingSystem.EternityMode = false;
                        WorldSavingSystem.ShouldBeEternityMode = false;
                        Main.NewText("[c/00ffee:Eternity Mode] disabled by [c/9c0000:Infernum Mode].");
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            PacketManager.SendPacket<EternityRevPacket>();
                    }
                    else
                    {
                        Main.NewText("[c/9c0000:Infernum Mode] detected. [c/00ffee:Eternity Priority over Calamity Bosses] has been disabled to prevent bugs.\n" +
                            "[c/00ffee:Eternity Priority over Calamity Bosses] can be re-enabled in the config, but things will break.");
                    }
                }
                if (ModCompatibility.InfernumMode.InfernumDifficulty) InfernumStateLastFrame = true;
                else InfernumStateLastFrame = false;
            }
            base.PostUpdateWorld();
        }
    }
}
