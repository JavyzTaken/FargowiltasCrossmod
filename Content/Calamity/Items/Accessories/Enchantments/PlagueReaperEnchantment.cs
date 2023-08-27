using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;
namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{

    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class PlagueReaperEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(118, 146, 147);
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.Turquoise>();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.DoctorBeeKill = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<CalamityMod.Items.Armor.PlagueReaper.PlagueReaperMask>(1);
            recipe.AddIngredient<CalamityMod.Items.Armor.PlagueReaper.PlagueReaperVest>(1);
            recipe.AddIngredient<CalamityMod.Items.Armor.PlagueReaper.PlagueReaperStriders>(1);
            recipe.AddIngredient<CalamityMod.Items.Weapons.Rogue.AlphaVirus>(1);
            recipe.AddIngredient<CalamityMod.Items.Weapons.Melee.AnarchyBlade>(1);
            recipe.AddIngredient<CalamityMod.Items.Weapons.Melee.SoulHarvester>(1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void PlagueReaperHitEffect(NPC target)
        {
            if (target.lifeMax <= 60000 && target.life == target.lifeMax)
            {
                if (Main.rand.NextBool(2))
                {

                    target.life = 0;
                    target.HitEffect();
                    target.active = false;
                    target.NPCLoot();
                }
            }
        }
        public void PlagueReaperProjHitEffect(NPC target)
        {
            if (target.lifeMax <= 60000 && target.life == target.lifeMax)
            {
                if (Main.rand.NextBool(2))
                {
                    target.life = 0;
                    target.HitEffect();
                    target.active = false;
                    target.NPCLoot();
                }
            }
        }
    }
}