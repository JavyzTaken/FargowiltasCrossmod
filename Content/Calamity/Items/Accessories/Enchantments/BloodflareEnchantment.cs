using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Rarities;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{

    [ExtendsFromMod("CalamityMod")]
    public class BloodflareEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(204, 42, 60);
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.BFCrazierRegen = true;
            SBDPlayer.UmbraCrazyRegen = false;
        }

        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyBloodflareHelms", 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Bloodflare.BloodflareBodyArmor>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Bloodflare.BloodflareCuisses>(), 1);
            recipe.AddIngredient(ModContent.ItemType<UmbraphileEnchantment>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.BloodBoiler>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Summon.DragonbloodDisgorger>(), 1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void BloodflareCalc(int damage)
        {
            if (damage / 4 <= 180)
            {

                BloodBuffTimer = damage / 4;
            }
            else if (damage / 4 > 180 && damage / 4 <= 300)
            {
                BloodBuffTimer = damage / 4;
            }
            else if (damage / 4 > 300)
                BloodBuffTimer = 300;
        }
        public void BloodflareHitEffect(NPC target, int damage)
        {
            if (Player.GetToggleValue("BloodflareBuffs"))
                Player.AddBuff(ModContent.BuffType<BloodflareRegeneration>(), BloodBuffTimer);
            if (LifestealCD <= 0 && Player.GetToggleValue("BloodflareLifesteal"))
            {
                Item.NewItem(target.GetSource_Loot(), target.Hitbox, 58);
                if (damage / 2 < Player.statLifeMax2 / 5)
                    Player.Heal(damage / 2);
                else
                {
                    Player.Heal(Player.statLifeMax2 / 5);

                }
                LifestealCD = 300;
                if (Auric && Player.GetToggleValue("AuricLightning"))
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center + new Vector2(0, -600), Vector2.Zero, ModContent.ProjectileType<AuricLightning>(), 2000, 0, Main.myPlayer, MathHelper.PiOver2, 4);
                }
            }
        }
        public void BloodflareProjHitEffect(NPC target, int damage)
        {
            if (Player.GetToggleValue("BloodflareBuffs"))
                Player.AddBuff(ModContent.BuffType<BloodflareRegeneration>(), BloodBuffTimer);
            if (LifestealCD <= 0 && Player.GetToggleValue("BloodflareLifesteal"))
            {
                Item.NewItem(target.GetSource_Loot(), target.Hitbox, 58);
                if (damage / 2 < Player.statLifeMax2 / 5)
                    Player.Heal(damage / 2);
                else Player.Heal(Player.statLifeMax2 / 5);
                LifestealCD = 300;
                if (Auric && Player.GetToggleValue("AuricLightning"))
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center + new Vector2(0, -600), Vector2.Zero, ModContent.ProjectileType<AuricLightning>(), 2000, 0, Main.myPlayer, MathHelper.PiOver2, 3.5f);
                }
            }
        }
    }
}
