using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ExplorationForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Purple;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.RideOfTheValkyrie = true;
            SBDPlayer.ProwlinOnTheFools = true;
            SBDPlayer.Marnite = true;
            SBDPlayer.WulfrumOverpower = true;
            SBDPlayer.ExploEffects = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<AerospecEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<ProwlerEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<MarniteEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<WulfrumEnchantment>(), 1);
            recipe.AddTile(ModContent.TileType<Fargowiltas.Items.Tiles.CrucibleCosmosSheet>());
            recipe.Register();
        }
    }
}