using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.Items;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Terraria;

namespace FargowiltasCrossmod.Core.Calamity
{
    public static partial class CalDLCUtils
    {
        public static CrossplayerCalamity CalamityDLC(this Player player)
            => player.GetModPlayer<CrossplayerCalamity>();
        public static CalamityAddonPlayer CalamityAddon(this Player player)
            => player.GetModPlayer<CalamityAddonPlayer>();
    }
}
