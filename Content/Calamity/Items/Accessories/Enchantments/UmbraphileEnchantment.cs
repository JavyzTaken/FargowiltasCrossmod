using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;
using Terraria.Audio;
using CalamityMod;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{

    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class UmbraphileEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(117, 69, 87);
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            //size, state and rarity
            Item.width = 30;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.UmbraCrazyRegen = true;
        }

        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Umbraphile.UmbraphileHood>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Umbraphile.UmbraphileRegalia>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Umbraphile.UmbraphileBoots>(), 1);
            recipe.AddIngredient(ItemID.VampireKnives, 1);
            recipe.AddIngredient(ItemID.SanguineStaff, 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.VampiricTalisman>(), 1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void UmbraphileEffect()
        {

            
        }
        public void UmbraphileTrigger()
        {
            if (!Player.HasCooldown("UmbraBatCD") && CalamityKeybinds.SetBonusHotKey.JustPressed && Main.myPlayer == Player.whoAmI)
            {
                Player.AddCooldown("UmbraBatCD", 900);
                SoundEngine.PlaySound(SoundID.NPCDeath6, Player.Center);
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, new Vector2(0, Main.rand.Next(3, 8)).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<UmbraBat>(), 75, 0);
                }
            }
        }
    }
}