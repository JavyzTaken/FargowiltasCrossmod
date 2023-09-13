using CalamityMod;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.CalPlayer;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Forces;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AnnihilationForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();


        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Purple;
        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine tooltip in tooltips)
            {
                int index = tooltip.Text.IndexOf("[button]");
                if (index != -1 && tooltip.Text.Length > 0)
                {
                    tooltip.Text = tooltip.Text.Remove(index, 8);
                    tooltip.Text = tooltip.Text.Insert(index, CalamityKeybinds.RageHotKey.TooltipHotkeyString());
                }
                int index2 = tooltip.Text.IndexOf("[button2]");
                if (index2 != -1 && tooltip.Text.Length > 0)
                {
                    tooltip.Text = tooltip.Text.Remove(index2, 9);
                    tooltip.Text = tooltip.Text.Insert(index2, CalamityKeybinds.SetBonusHotKey.TooltipHotkeyString());
                }
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.AnnihilEffects = true;
            SBDPlayer.Demonshade = true;
            SBDPlayer.Gemtech = true;
            SBDPlayer.FearOfTheValkyrie = true;
            SBDPlayer.Prismatic = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DemonshadeEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<GemtechEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<FearmongerEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<PrismaticEnchantment>());
            recipe.AddTile(ModContent.TileType<Fargowiltas.Items.Tiles.CrucibleCosmosSheet>());
            recipe.Register();
        }
    }
}