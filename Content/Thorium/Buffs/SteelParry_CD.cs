using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Buffs
{
    [ExtendsFromMod("ThoriumMod")]
    public class SteelParry_CD : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderDebuff";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;

        }
    }
}
