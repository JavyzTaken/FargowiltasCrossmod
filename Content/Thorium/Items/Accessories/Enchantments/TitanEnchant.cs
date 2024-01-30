using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class TitanEnchant : BaseEnchant
    {
        public override Color nameColor => Color.SteelBlue;
        const float EffectMult = 0.1f;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.ThoriumDLC().TitanEnch = true;
            player.GetArmorPenetration(DamageClass.Generic) += (player.statDefense * EffectMult);
            player.statDefense *= 0.8f;
        }
    }
}