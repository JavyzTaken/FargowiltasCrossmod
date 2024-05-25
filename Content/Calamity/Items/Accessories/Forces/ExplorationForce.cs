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
            player.AddEffect<MarniteStatsEffect>(Item);
            player.AddEffect<MarniteLasersEffect>(Item);
            player.AddEffect<DesertProwlerEffect>(Item);
            player.AddEffect<WulfrumEffect>(Item);
            SulphurEnchantment.AddEffects(player, Item);
            VictideEnchantment.AddEffects(player, Item);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<AerospecEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<DesertProwlerEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<MarniteEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<WulfrumEnchantment>(), 1);
            recipe.AddIngredient(ModContent.ItemType<SulphurEnchantment>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VictideEnchantment>(), 1);
            recipe.AddTile(ModContent.TileType<Fargowiltas.Items.Tiles.CrucibleCosmosSheet>());
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExplorationHeader : EnchantHeader
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override int Item => ModContent.ItemType<ExplorationForce>();
        public override float Priority => 0.15f;
    }
}
