using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Forces;
namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class DesolationForce : BaseForce
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
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.DesolEffects = true;
            SBDPlayer.OmegaBlue = true;
            SBDPlayer.Astral = true;
            SBDPlayer.Mollusk = true;
            SBDPlayer.DoctorBeeKill = true;
            SBDPlayer.FathomBubble = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<OmegaBlueEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<AstralEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<MolluskEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<PlagueReaperEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<FathomEnchantment>());
            recipe.AddTile(ModContent.TileType<Fargowiltas.Items.Tiles.CrucibleCosmosSheet>());
            recipe.Register();
        }
    }
}