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
    public class ProwlerEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(204, 42, 60);

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().ProwlinOnTheFools = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.DesertProwler.DesertProwlerHat>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.DesertProwler.DesertProwlerShirt>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.DesertProwler.DesertProwlerPants>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.CrackshotColt>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Summon.SunSpiritStaff>());
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void ProwlerHitEffect()
        {
            if (Main.rand.NextBool(10))
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - new Vector2(960f, 0), new Vector2(12f, 0), ModContent.ProjectileType<ProwlerTornado>(), 25, 0f, Player.whoAmI);

            }
        }
        public void ProwlerProjHitEffect(Projectile proj)
        {
            if (Main.rand.NextBool(10) && proj.type != ModContent.ProjectileType<ProwlerTornado>())
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center - new Vector2(960f, 0), new Vector2(36f, 0), ModContent.ProjectileType<ProwlerTornado>(), 25, 0f, Player.whoAmI);

            }
        }
    }
}
