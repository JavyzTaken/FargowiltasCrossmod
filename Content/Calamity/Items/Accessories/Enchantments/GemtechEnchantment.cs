using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items.Armor.Prismatic;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{

    [ExtendsFromMod("CalamityMod")]
    public class GemtechEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(206, 201, 170);

        public override void SetStaticDefaults()
        {


        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.Prismatic = true;
        }

        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<PrismaticHelmet>(), 1);
            recipe.AddIngredient(ModContent.ItemType<PrismaticRegalia>(), 1);
            recipe.AddIngredient(ModContent.ItemType<PrismaticGreaves>(), 1);
            recipe.AddIngredient(ModContent.ItemType<DarkSpark>(), 1);
            recipe.AddIngredient(ModContent.ItemType<HandheldTank>(), 1);
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyRailguns");
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {

    }
}