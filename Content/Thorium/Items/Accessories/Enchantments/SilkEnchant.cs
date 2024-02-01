using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System.Collections.Generic;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SilkEnchant : BaseEnchant
    {
        public override Color nameColor => Color.BlueViolet;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SilkEffect>(Item);
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);

            FargoSoulsPlayer soulsPlayer = Main.LocalPlayer.FargoSouls();
            CrossplayerThorium DLCPlayer = Main.LocalPlayer.ThoriumDLC();

            foreach (BaseEnchant enchant in soulsPlayer.EquippedEnchants)
            {
                // For someone smarter than me: I need this to iterate through the player's accessories and add a tooltip for each one that is a synergy enchant, however idk how to check the type, since BaseSynergyEnchant is templated.

                //if (enchant.Type == Type || enchant is not BaseSynergyEnchant synEnchant)
                //{
                //    continue;
                //}

                //if (DLCPlayer.SynergyEffect(enchant.Type))
                //{
                //    tooltips.Add(new TooltipLine(Mod, "silk", $"[i:{enchant.Item.type}] [i:{synEnchant.SynergyEnch}]"));
                //}
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

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SilkEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.HelheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<SilkEnchant>();
    }
}
