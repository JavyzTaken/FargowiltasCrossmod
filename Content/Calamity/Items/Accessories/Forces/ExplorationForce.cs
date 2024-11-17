using FargowiltasSouls.Content.Items.Accessories.Forces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ExplorationForce : BaseForce
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Purple;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<AerospecJumpEffect>(Item);
            MarniteEnchant.AddEffects(player, Item);
            player.AddEffect<DesertProwlerEffect>(Item);
            player.AddEffect<WulfrumEffect>(Item);
            SulphurEnchant.AddEffects(player, Item);
            VictideEnchant.AddEffects(player, Item);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<AerospecEnchant>());
            recipe.AddIngredient(ModContent.ItemType<DesertProwlerEnchant>());
            recipe.AddIngredient(ModContent.ItemType<MarniteEnchant>());
            recipe.AddIngredient(ModContent.ItemType<WulfrumEnchant>(), 1);
            recipe.AddIngredient(ModContent.ItemType<SulphurEnchant>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VictideEnchant>(), 1);
            recipe.AddTile(ModContent.TileType<Fargowiltas.Items.Tiles.CrucibleCosmosSheet>());
            recipe.Register();
        }
    }
}
