using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{

    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class WulfrumEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(206, 201, 170);

        public override void SetStaticDefaults()
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
            SBDPlayer.WulfrumOverpower = true;
        }

        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Wulfrum.WulfrumHat>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Wulfrum.WulfrumJacket>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Wulfrum.WulfrumOveralls>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Summon.WulfrumController>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.TrinketofChi>(), 1);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void WulfrumEffects()
        {
            if (Player.statLife <= Player.statLifeMax2 * 0.3f)
                Player.AddBuff(ModContent.BuffType<WulfrumEmpowerment>(), 2);

        }
    }
}