using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;

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
            float endur = 0.3f;
            float dmg = 0.25f;
            if (player.GetModPlayer<CrossplayerCalamity>().ForceEffect(ModContent.ItemType<HydrothermicEnchantment>()))
            {
                endur += 0.05f;
                dmg += 0.1f;
            }
            player.endurance -= endur;
            player.GetDamage(DamageClass.Generic) += dmg;
            player.GetModPlayer<CrossplayerCalamity>().AtaxiaCooldown = 60;
        }
    }
}
