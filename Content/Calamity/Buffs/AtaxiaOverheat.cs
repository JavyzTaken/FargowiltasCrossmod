using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
namespace FargowiltasCrossmod.Content.Calamity.Buffs
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AtaxiaOverheat : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.endurance -= 0.3f;
            player.GetDamage(DamageClass.Generic) += 0.25f;
            player.GetModPlayer<CrossplayerCalamity>().AtaxiaCooldown = 60;
        }
    }
}
