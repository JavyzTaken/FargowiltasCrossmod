
using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Buffs
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class WulfrumEmpowerment : ModBuff
    {
        public override void SetStaticDefaults()
        {

        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.endurance += 0.15f;
        }
    }
}
