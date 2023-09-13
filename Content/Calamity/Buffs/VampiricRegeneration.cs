
using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Buffs
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class VampiricRegeneration : ModBuff
    {
        public override void SetStaticDefaults()
        {

        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 4;
        }
    }
}
