using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Buffs
{
    // disclaimer: doesn't actually do anything yet
    [ExtendsFromMod("ThoriumMod")]
    public class MynaDB : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderDebuff";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            
        }
    }
}
