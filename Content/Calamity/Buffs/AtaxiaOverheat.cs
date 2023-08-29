

using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Buffs
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AtaxiaOverheat : ModBuff
    {
        public override void SetStaticDefaults()
        {

            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.endurance -= 0.3f;
            player.GetDamage(DamageClass.Generic) += 0.25f;
            player.GetModPlayer<CrossplayerCalamity>().AtaxiaCooldown = 60;
        }
    }
}
