using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using CalamityMod.Items.Armor.MarniteArchitect;
using CalamityMod.Items.Accessories;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class MarniteEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(153, 200, 193);
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().Marnite = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<MarniteArchitectHeadgear>());
            recipe.AddIngredient(ModContent.ItemType<MarniteArchitectToga>());
            recipe.AddIngredient(ModContent.ItemType<MarniteRepulsionShield>());
            recipe.AddIngredient(ModContent.ItemType<UnstableGraniteCore>());
            recipe.AddIngredient(ModContent.ItemType<GladiatorsLocket>());
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }

    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void MarniteEffects()
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                if (Player.GetToggleValue("BuildBuff"))
                {
                    Player.tileRangeX += 9;
                    Player.tileRangeY += 9;
                    Player.tileSpeed += 0.5f;
                }
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<MarniteSword>()] < 2 && Player.GetToggleValue("MarniteSwords"))
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<MarniteSword>(), 10, 0f, Main.myPlayer);
                    Player.AddBuff(ModContent.BuffType<MarniteSwordsBuff>(), 18000);
                }
            }
        }
    }
}
