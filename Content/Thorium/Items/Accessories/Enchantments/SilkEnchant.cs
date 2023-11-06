using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System.Collections.Generic;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SilkEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.BlueViolet;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.SilkEnch = true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);

            FargoSoulsPlayer soulsPlayer = Main.LocalPlayer.FargoSouls();
            CrossplayerThorium DLCPlayer = Main.LocalPlayer.ThoriumDLC();

            foreach (BaseEnchant enchant in soulsPlayer.EquippedEnchants)
            {
                if (enchant.Type == Type || enchant is not BaseSynergyEnchant synEnchant)
                {
                    continue;
                }

                if (DLCPlayer.SynergyEffect(enchant.Type))
                {
                    tooltips.Add(new TooltipLine(Mod, "silk", $"[i:{enchant.Item.type}] [i:{synEnchant.SynergyEnch}]"));
                }
            }

            tooltips.RemoveAll(line => line.Name == "wizard");
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThoriumMod.Items.EarlyMagic.SilkHat>()
                .AddIngredient<ThoriumMod.Items.EarlyMagic.SilkTabard>()
                .AddIngredient<ThoriumMod.Items.EarlyMagic.SilkLeggings>()
                .Register();
        }
    }
}
