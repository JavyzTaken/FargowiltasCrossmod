using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Utils
{
    public static partial class DLCUtils
    {
        public static bool HostCheck => Main.netMode != NetmodeID.MultiplayerClient;
        //copy psated from fargo but changed so it can be from any mod
        public static void DropSummon(NPC npc, string mod, string itemName, bool downed, ref bool droppedSummonFlag, bool prerequisite = true)
        {
            if (WorldSavingSystem.EternityMode && prerequisite && !downed && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummonFlag)
            {
                Player player = Main.player[npc.target];

                if (ModContent.TryFind(mod, itemName, out ModItem modItem))
                    Item.NewItem(npc.GetSource_Loot(), player.Hitbox, modItem.Type);
                droppedSummonFlag = true;
            }
        }
    }
}
