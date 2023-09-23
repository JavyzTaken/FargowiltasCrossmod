using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Rogue;
using Fargowiltas.Items.Tiles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Essences;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class VagabondsSoul : BaseSoul
    {
        protected override Color? nameColor => new Color(217, 144, 67);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<RogueDamageClass>() += 0.3f;
            player.Calamity().rogueVelocity += 0.15f;
            player.GetCritChance<RogueDamageClass>() += 0.15f;
            if (player.GetToggleValue("Nanotech"))
            {
                ModContent.GetInstance<Nanotech>().UpdateAccessory(player, hideVisual);
            }
            if (player.GetToggleValue("EclipseMirror"))
            {
                ModContent.GetInstance<EclipseMirror>().UpdateAccessory(player, hideVisual);
            }
            if (player.GetToggleValue("DragonScales"))
            {
                ModContent.GetInstance<DragonScales>().UpdateAccessory(player, hideVisual);
            }
            if (player.GetToggleValue("VeneratedLocket"))
            {
                ModContent.GetInstance<VeneratedLocket>().UpdateAccessory(player, hideVisual);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OutlawsEssence>()
                .AddIngredient<EclipseMirror>()
                .AddIngredient<Nanotech>()
                .AddIngredient<DragonScales>()
                .AddIngredient<VeneratedLocket>()

                .AddIngredient<WaveSkipper>()
                .AddIngredient<GraveGrimreaver>()
                .AddIngredient<SpentFuelContainer>()
                .AddIngredient<TheSyringe>()
                .AddIngredient<RegulusRiot>()
                .AddIngredient<UtensilPoker>()
                .AddIngredient<Valediction>()
                .AddIngredient<ExecutionersBlade>()
                .AddIngredient<EclipsesFall>()
                
                .AddTile<CrucibleCosmosSheet>()
                .Register();
        }
    }
}
