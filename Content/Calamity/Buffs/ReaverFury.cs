

using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core; 

namespace FargowiltasCrossmod.Content.Calamity.Buffs
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ReaverFury : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CrossplayerCalamity>().ReaverHageBuff = true;
        }
    }
}
