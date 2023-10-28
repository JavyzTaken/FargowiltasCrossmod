using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class EbonEnchant : BaseSynergyEnchant
    {
        protected override Color nameColor => Color.Purple;
        internal override bool SynergyActive(CrossplayerThorium DLCPlayer) => DLCPlayer.EbonEnch && DLCPlayer.NoviceClericEnch;
        
        protected override Color SynergyColor1 => Color.White with { A = 0 };
        protected override Color SynergyColor2 => Color.Purple with { A = 0 };
        internal override int SynergyEnch => ModContent.ItemType<NoviceClericEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.EbonEnch = true;
            DLCPlayer.EbonEnchItem = Item;

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