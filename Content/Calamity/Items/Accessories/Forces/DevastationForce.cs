using FargowiltasSouls.Content.Items.Accessories.Forces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class DevastationForce : BaseForce
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Purple;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<HydrothermicEffect>(Item);
            player.AddEffect<PlaguebringerEffect>(Item);
            player.AddEffect<ReaverEffect>(Item);
            player.AddEffect<DaedalusEffect>(Item);
            player.AddEffect<SnowRuffianEffect>(Item);
            player.CalamityAddon().HydrothermicHide = hideVisual;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DaedalusEnchant>());
            recipe.AddIngredient(ModContent.ItemType<ReaverEnchant>());
            recipe.AddIngredient(ModContent.ItemType<HydrothermicEnchant>());
            recipe.AddIngredient(ModContent.ItemType<PlaguebringerEnchant>(), 1);
            recipe.AddTile(ModContent.TileType<Fargowiltas.Items.Tiles.CrucibleCosmosSheet>());
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DevastationHeader : EnchantHeader
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override int Item => ModContent.ItemType<DevastationForce>();
        public override float Priority => 0.15f;
    }
}
