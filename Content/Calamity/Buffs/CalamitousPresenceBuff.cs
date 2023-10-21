using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Buffs
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalamitousPresenceBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mutant Presence");
            // Description.SetDefault("Defense, damage reduction, and life regen reduced; almost all soul toggles disabled; reduced graze radius");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变驾到");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "减少防御、伤害减免和生命恢复速度; 关闭近乎所有魂的效果; 附带混沌状态减益");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //also halves defense, DR, and cripples life regen
            player.FargoSouls().noDodge = true;
            player.FargoSouls().noSupersonic = true;
            player.CalamityDLC().CalamitousPresence = true;
            player.FargoSouls().GrazeRadius *= 0.5f;
            player.moonLeech = true;
        }
    }
}
