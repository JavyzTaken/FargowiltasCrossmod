using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod("CalamityMod")]
    public class StatigelEnchantment : BaseEnchant
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
        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return !player.GetModPlayer<CalamityPlayer>().dodgeScarf;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.StatigelNinjaStyle = true;
            player.GetModPlayer<CalamityPlayer>().dodgeScarf = true;
            player.GetModPlayer<CalamityPlayer>().DashID = CounterScarfDash.ID;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyStatisHelms");
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Statigel.StatigelArmor>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Statigel.StatigelGreaves>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.EvasionScarf>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.OverloadedBlaster>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.GelDart>(), 200);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void StatigelHitEffect(NPC target, int damage)
        {
            int kunaiDmg;
            if (damage > 100 && kunaiKuldown <= 0)
            {
                if (damage < 150) kunaiDmg = damage;
                else kunaiDmg = 150;
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<GelKunai>(), kunaiDmg, 0f, Player.whoAmI);
                kunaiKuldown = 120;
            }
        }
        public void StatigelProjHitEffect(Projectile proj, NPC target, int damage, bool crit)
        {
            int kunaiDmg;
            if ((damage > 100 || crit && damage > 50) && proj.type != ModContent.ProjectileType<GelKunai>() && kunaiKuldown <= 0)
            {
                if (damage < 150 || crit && damage < 75)
                {
                    kunaiDmg = damage;
                    if (crit) kunaiDmg = damage * 2;
                }
                else kunaiDmg = 150;
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<GelKunai>(), kunaiDmg, 0f, Player.whoAmI);
                kunaiKuldown = 120;
            }
        }
    }
}