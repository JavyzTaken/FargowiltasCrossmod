using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Rogue;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Essences;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; 

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Essences
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class OutlawsEssence : BaseEssence
    {
        protected override Color nameColor => new(217, 144, 67);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(ModContent.GetInstance<RogueDamageClass>()) += 0.18f;
            player.Calamity().rogueVelocity += 0.05f;
            player.GetCritChance(ModContent.GetInstance<RogueDamageClass>()) += 0.05f;

        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GildedDagger>())
                .AddIngredient(ModContent.ItemType<BouncingEyeball>())
                .AddIngredient(ModContent.ItemType<InfestedClawmerang>())
                .AddIngredient(ModContent.ItemType<MeteorFist>())
                .AddIngredient(ModContent.ItemType<SludgeSplotch>(), 300)
                .AddIngredient(ModContent.ItemType<SkyStabber>())
                .AddIngredient(ModContent.ItemType<HardenedHoneycomb>(), 300)
                .AddIngredient(ModContent.ItemType<InfernalKris>(), 300)
                .AddIngredient(ItemID.HallowedBar, 5)
                .AddIngredient(ModContent.ItemType<RogueEmblem>())
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

        }
    }
}
