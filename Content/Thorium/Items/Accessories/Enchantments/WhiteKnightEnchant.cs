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
    public class WhiteKnightEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Silver;
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        
        public override void SetStaticDefaults()
        {

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerThorium>().WhiteKnightEnch = true;
            player.GetDamage(DamageClass.Generic) += 0.075f * player.townNPCs;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThoriumMod.Items.MagicItems.WhiteKnightMask>()
                .AddIngredient<ThoriumMod.Items.MagicItems.WhiteKnightTabard>()
                .AddIngredient<ThoriumMod.Items.MagicItems.WhiteKnightLeggings>()
                .Register();
        }
    }
}
