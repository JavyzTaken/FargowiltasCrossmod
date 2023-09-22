using CalamityMod.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using FargowiltasSouls.Content.Items.Materials;
using Terraria.ID;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using Fargowiltas.Items.Tiles;
using CalamityMod.Rarities;
using Terraria.Localization;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AncientsSoul : BaseSoul
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = ModContent.RarityType<Violet>();
            Item.defense = 10;
        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().AncientsSoulEffect(player, hideVisual);
            //player.GetModPlayer<CrossplayerCalamity>().AncestralCharm = true;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient<DimensionalSoulArtifact>()
                .AddIngredient<ProfanedSoulArtifact>()
                .AddIngredient<EldritchSoulArtifact>()
                .AddIngredient<GodlySoulArtifact>()
                .AddIngredient<PhantomicArtifact>()
                .AddIngredient<DarkSunRing>()
                .AddIngredient<AbomEnergy>(10)
                .AddTile<CrucibleCosmosSheet>()
                .Register();
        }
    }

}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void AncientsSoulEffect(Player player, bool hideVisual)
        {
            if (player.GetToggleValue("DimensionalSoulArtifact"))
                ModContent.GetInstance<DimensionalSoulArtifact>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("ProfanedSoulArtifact"))
                ModContent.GetInstance<ProfanedSoulArtifact>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("EldritchSoulArtifact"))
                ModContent.GetInstance<EldritchSoulArtifact>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("AuricSoulArtifact"))
                ModContent.GetInstance<GodlySoulArtifact>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("PhantomicArtifact"))
                ModContent.GetInstance<PhantomicArtifact>().UpdateAccessory(player, hideVisual);
            if (player.GetToggleValue("DarkSunRing"))
                ModContent.GetInstance<DarkSunRing>().UpdateAccessory(player, hideVisual);
        }
    }
}
