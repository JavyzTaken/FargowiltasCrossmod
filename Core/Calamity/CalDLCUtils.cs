using FargowiltasCrossmod.Content.Calamity.Items;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.ModPlayers;
using Terraria;

namespace FargowiltasCrossmod.Core.Calamity
{
    public static partial class CalDLCUtils
    {
        public static CrossplayerCalamity CalamityDLC(this Player player)
            => player.GetModPlayer<CrossplayerCalamity>();
        public static CalDLCAddonPlayer CalamityAddon(this Player player)
            => player.GetModPlayer<CalDLCAddonPlayer>();
        /// <summary>
        /// Gets the NPC'S CalDLCEmodeBehavior override if it has one. Returns null if override is missing.
        /// </summary>
        public static T GetDLCBehavior<T>(this NPC npc) where T : CalDLCEmodeBehavior => npc.GetGlobalNPC<CalDLCEmodeBehaviorManager>().DLCBehaviour is T behavior && behavior != null ? behavior : null;
    }
}
