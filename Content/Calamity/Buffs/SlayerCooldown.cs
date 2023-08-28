

using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Buffs
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class SlayerCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {

            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CrossplayerCalamity>().SlayerCD = true;
        }
    }
}
