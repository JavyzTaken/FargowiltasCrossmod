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
        public override Color nameColor => new(217, 144, 67);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(ModContent.GetInstance<RogueDamageClass>()) += 0.18f;
            player.Calamity().rogueVelocity += 0.1f;
            player.GetCritChance(ModContent.GetInstance<RogueDamageClass>()) += 8f;

        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasCrossmod:AnyGildedDagger")
                .AddIngredient<BouncingEyeball>()
                .AddIngredient<InfestedClawmerang>()
                .AddIngredient<MeteorFist>()
                .AddIngredient<SludgeSplotch>(300)
                .AddIngredient<SkyStabber>()
                .AddIngredient<HardenedHoneycomb>(300)
                .AddIngredient<InfernalKris>(300)
                .AddIngredient<RogueEmblem>()
                .AddIngredient(ItemID.HallowedBar, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

        }
    }
}
