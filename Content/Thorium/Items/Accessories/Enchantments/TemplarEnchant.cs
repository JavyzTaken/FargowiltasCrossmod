using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using ThoriumMod;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using FargowiltasSouls.Common.Utilities;
using ThoriumMod.Items.Donate;
using ThoriumMod.Items.HealerItems;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class TemplarEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override Color nameColor => Color.PaleVioletRed;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.ThoriumDLC();
            modPlayer.TemplarEnch = true;
            modPlayer.TemplarEnchItem = Item;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<TemplarsCirclet>(), 1);
            recipe.AddIngredient(ModContent.ItemType<TemplarsTabard>(), 1);
            recipe.AddIngredient(ModContent.ItemType<TemplarsLeggings>(), 1);
            recipe.AddIngredient(ModContent.ItemType<TemplarJudgment>(), 1);
            recipe.AddIngredient(ModContent.ItemType<Recuperate>(), 1);
            recipe.AddIngredient(ModContent.ItemType<RichLeaf>(), 1);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}
