using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using CalamityMod.Items.Accessories;
using System.Collections.Generic;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod;
using CalamityMod.Items.Armor.Auric;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Rarities;
using Terraria.Localization;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AuricEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(227, 174, 0);
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<Violet>();
        }


        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);
            string tarragonEffect = "";
            string bloodflareEffect = "";
            string silvaEffect = "";
            string godslayerEffect = "";
            string key = "Mods.FargowiltasCrossmod.Items.AuricEnchantment.";
            CrossplayerCalamity player = Main.player[Main.myPlayer].GetModPlayer<CrossplayerCalamity>();
            if (player.Tarragon) tarragonEffect = Language.GetTextValue(key + "TarragonTooltip") + "\n";
            if (player.BFCrazierRegen) bloodflareEffect = Language.GetTextValue(key + "BloodflareTooltip") + "\n";
            if (player.Silva) silvaEffect = Language.GetTextValue(key + "SilvaTooltip") + "\n";
            if (player.GodSlayerMeltdown) godslayerEffect = Language.GetTextValue(key + "GodSlayerTooltip") + "\n";
            TooltipLine tooltip = new TooltipLine(Mod, "FargowiltasCrossmod: AuricEnch",
                Language.GetTextValue(key + "AuricTooltip") + "\n" +
                tarragonEffect + bloodflareEffect + silvaEffect + godslayerEffect +
                "\"" + Language.GetTextValue(key + "Quote") + "\"");
            tooltips.Add(tooltip);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().Auric = true;

        }



        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyAuricHelms");
            recipe.AddIngredient(ModContent.ItemType<AuricTeslaBodyArmor>());
            recipe.AddIngredient(ModContent.ItemType<AuricTeslaCuisses>());
            recipe.AddIngredient(ModContent.ItemType<YharimsGift>());
            recipe.AddIngredient(ModContent.ItemType<Ataraxia>());
            recipe.AddIngredient(ModContent.ItemType<AuricToilet>());
            recipe.AddTile(ModContent.TileType<CalamityMod.Tiles.Furniture.CraftingStations.DraedonsForge>());
            recipe.Register();
        }
    }
}
