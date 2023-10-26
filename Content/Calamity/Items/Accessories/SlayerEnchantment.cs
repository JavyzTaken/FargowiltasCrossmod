using CalamityMod.CalPlayer;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using CalamityMod;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class SlayerEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new(89, 170, 204);
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.GodSlayerMeltdown = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnySlayerHelms");
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.GodSlayer.GodSlayerChestplate>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.GodSlayer.GodSlayerLeggings>());
            recipe.AddIngredient(ModContent.ItemType<StatigelEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.CleansingBlaze>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.NebulousCore>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void SlayerTrigger()
        {
            if (CalamityKeybinds.ArmorSetBonusHotKey.JustPressed && Main.myPlayer == Player.whoAmI && SlayerHeadCD == 0)
            {
                SlayerHeadCD = 930;
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero) * 15, ModContent.ProjectileType<DogSoul>(), 1500, 0, Main.myPlayer);
            }
        }
    }
}