using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace FargowiltasCrossmod.Core.Utils
{
    public static partial class DLCUtils
    {
        public static bool HostCheck => Main.netMode != NetmodeID.MultiplayerClient;
    }
}
