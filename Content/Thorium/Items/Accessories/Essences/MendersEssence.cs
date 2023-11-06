using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Essences;
using ThoriumMod.Items.HealerItems;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Essences
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class MendersEssence : BaseEssence
    {
        protected override Color nameColor => new(255, 0, 255);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var thoriumPlayer = player.Thorium();
            player.GetDamage<ThoriumMod.HealerDamage>() += 0.18f;
            player.GetCritChance<ThoriumMod.HealerDamage>() += 0.05f;
            thoriumPlayer.healBonus += 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HereticBreaker>()
                .AddIngredient<TemplarsGrace>()
                .AddIngredient<GraniteIonStaff>()
                .AddIngredient<AquaiteScythe>()
                .AddIngredient<TheStalker>()
                .AddIngredient<EaterOfPain>()
                .AddIngredient<DeepStaff>()
                .AddIngredient<HeartWand>()
                .AddIngredient(ItemID.HallowedBar, 5)
                .AddIngredient<ClericEmblem>()
                .AddTile(TileID.TinkerersWorkbench);
        }
    }
}