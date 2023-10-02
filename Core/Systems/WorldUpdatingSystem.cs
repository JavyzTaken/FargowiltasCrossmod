using Fargowiltas.NPCs;
using FargowiltasCrossmod.Core.Calamity;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Systems
{
    public class WorldUpdatingSystem : ModSystem
    {
        public bool emodeOverRev => ModContent.GetInstance<CalamityConfig>().EternityPriorityOverRev;
        public override void PreUpdateWorld()
        {
            if (ModCompatibility.Calamity.Loaded)
            {
                ModCompatibility.SoulsMod.Mod.Call("EternityVanillaBossBehaviour", ModContent.GetInstance<DLCCalamityConfig>().EternityPriorityOverRev);
            }
            
        }
        public override void PostUpdateWorld()
        {
            if (ModCompatibility.Calamity.Loaded)
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
