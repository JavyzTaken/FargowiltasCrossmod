using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.ModPlayers;
using Terraria;

namespace FargowiltasCrossmod.Core.Calamity
{
    public static partial class CalDLCUtils
    {
        public static CalDLCPlayer CalamityDLC(this Player player)
            => player.GetModPlayer<CalDLCPlayer>();
        public static CalDLCAddonPlayer CalamityAddon(this Player player)
            => player.GetModPlayer<CalDLCAddonPlayer>();

        public static CalDLCNPCChanges CalamityDLC(this NPC npc)
            => npc.GetGlobalNPC<CalDLCNPCChanges>();
        /// <summary>
        /// Gets the NPC'S CalDLCEmodeBehavior override if it has one. Returns null if override is missing.
        /// </summary>
        public static T GetDLCBehavior<T>(this NPC npc) where T : CalDLCEmodeBehavior => npc.GetGlobalNPC<CalDLCEmodeBehaviorManager>().DLCBehaviour is T behavior && behavior != null ? behavior : null;

        /// <summary>
        /// Gets the NPC'S CalDLCEmodeBehavior override if it has one via an out parameter, returning null if the override is missing.
        /// </summary>
        public static bool TryGetDLCBehavior<T>(this NPC npc, out T behavior) where T : CalDLCEmodeBehavior
        {
            behavior = npc.GetDLCBehavior<T>();
            return behavior is not null;
        }
    }
}
