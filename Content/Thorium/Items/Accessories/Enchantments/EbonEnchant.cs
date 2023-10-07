using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod("ThoriumMod")]
    public class EbonEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Purple;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.EbonEnch = true;

            EbonEffect(player);
        }

        public static void EbonEffect(Player player)
        {
            var thoriumPlayer = player.GetModPlayer<ThoriumMod.ThoriumPlayer>();
            thoriumPlayer.darkAura = true;
            player.GetDamage(DamageClass.Generic) += 0.05f * thoriumPlayer.healBonus;
        }
    }
}