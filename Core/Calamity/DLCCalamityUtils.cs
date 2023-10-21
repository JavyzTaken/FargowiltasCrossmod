using FargowiltasCrossmod.Content.Calamity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace FargowiltasCrossmod.Core.Calamity
{
    public static partial class DLCCalamityUtils
    {
        public static CrossplayerCalamity CalamityDLC(this Player player)
            => player.GetModPlayer<CrossplayerCalamity>();
    }
}
