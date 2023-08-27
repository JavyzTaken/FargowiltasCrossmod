using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod("ThoriumMod")]
    public class SilkEnchant : BaseEnchant
    {
        public override string wizardEffect => "";
        protected override Color nameColor => Color.BlueViolet;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerThorium>().SilkEnch = true;
            if (player.statMana >= player.statManaMax * 0.95) return; // so you dont get boosts with just full mana
            player.GetDamage(DamageClass.Generic) += 0.0025f * player.statMana;
            if (player.GetModPlayer<FargowiltasSouls.Core.ModPlayers.FargoSoulsPlayer>().WizardEnchantActive) player.GetDamage(DamageClass.Generic) += 0.0025f * player.statMana;
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
