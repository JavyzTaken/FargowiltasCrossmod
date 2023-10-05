using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Buffs
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimflameBuff : ModBuff
    {
        public override string Texture => "CalamityMod/Buffs/StatBuffs/BrimflameFrenzyBuff";
        public override void SetStaticDefaults()
        {


            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.25f;
            player.endurance -= 0.25f;

        }
    }
}
