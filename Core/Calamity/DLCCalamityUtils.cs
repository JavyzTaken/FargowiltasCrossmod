using FargowiltasCrossmod.Content.Calamity;
using Terraria;

namespace FargowiltasCrossmod.Core.Calamity
{
    public static partial class DLCCalamityUtils
    {
        public static CrossplayerCalamity CalamityDLC(this Player player)
            => player.GetModPlayer<CrossplayerCalamity>();
    }
}
