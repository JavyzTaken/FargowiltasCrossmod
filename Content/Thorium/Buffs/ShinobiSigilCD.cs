using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Buffs
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class ShinobiSigilCD : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderDebuff";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }
    }
}
