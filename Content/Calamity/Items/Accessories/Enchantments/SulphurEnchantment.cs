using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{

    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class SulphurEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(181, 139, 161);
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.SulphurBubble = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<CalamityMod.Items.Armor.Sulphurous.SulphurousHelmet>();
            recipe.AddIngredient<CalamityMod.Items.Armor.Sulphurous.SulphurousBreastplate>();
            recipe.AddIngredient<CalamityMod.Items.Armor.Sulphurous.SulphurousLeggings>();
            recipe.AddIngredient<CalamityMod.Items.Weapons.Rogue.ContaminatedBile>();
            recipe.AddIngredient<CalamityMod.Items.Weapons.Summon.CausticCroakerStaff>();
            recipe.AddIngredient<CalamityMod.Items.Accessories.RustyMedallion>();
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void SulphurEffects()
        {
            if (!DirtyPop && !NastyPop)
            {
                bubbleOffset.X = -400 + Main.rand.Next(800);
                bubbleOffset.Y = -300 + Main.rand.Next(320);
                NPC.NewNPC(Player.GetSource_FromThis(), (int)Player.Center.X, (int)Player.Center.Y, ModContent.NPCType<SulphurBubble>(), ai0: Player.whoAmI);
                DirtyPop = true;
            }
        }
    }
}
