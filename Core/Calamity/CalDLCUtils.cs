using FargowiltasCrossmod.Content.Calamity.Items;
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
    }
}
