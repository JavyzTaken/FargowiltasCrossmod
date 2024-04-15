using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using CalamityMod;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using Terraria.Audio;
using FargowiltasCrossmod.Core.Calamity;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [AutoloadEquip]
    public class DaedalusEnchantment : BaseEnchant
    {

        public override Color nameColor => new(132, 212, 246);
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<DaedalusEffect>(Item);
            player.AddEffect<SnowRuffianEffect>(Item);
        }
        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyDaedalusHelms", 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusBreastplate>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusLeggings>(), 1);
            recipe.AddIngredient(ModContent.ItemType<SnowRuffianEnchantment>(), 1);
            recipe.AddIngredient(ItemID.IceRod, 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.Wings.StarlightWings>(), 1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    public class DaedalusEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<DaedalusEnchantment>();
        public override void PreUpdate(Player player)
        {
            
        }
        public override void PostUpdateEquips(Player player)
        {
            if (player.wingTime > 0)
            {
                Main.NewText(player.CalamityDLC().DaedalusHeight);
                player.wingTime = (int)player.Center.Y - player.CalamityDLC().DaedalusHeight;
            }
        }
    }
}
