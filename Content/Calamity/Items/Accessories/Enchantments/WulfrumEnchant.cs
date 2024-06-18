using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using CalamityMod;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Tools;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("WulfrumEnchantment")]
    public class WulfrumEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Color nameColor => new Color(206, 201, 170);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<WulfrumEffect>(Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<CalamityMod.Items.Armor.Wulfrum.WulfrumHat>();
            recipe.AddIngredient<CalamityMod.Items.Armor.Wulfrum.WulfrumJacket>();
            recipe.AddIngredient<CalamityMod.Items.Armor.Wulfrum.WulfrumOveralls>();
            recipe.AddIngredient<CalamityMod.Items.Weapons.Summon.WulfrumController>();
            recipe.AddIngredient<CalamityMod.Items.Tools.WulfrumTreasurePinger>();
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class WulfrumEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Header ToggleHeader => Header.GetHeader<ExplorationHeader>();
        public override int ToggleItemType => ModContent.ItemType<WulfrumEnchant>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<WulfrumScanner>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_EffectItem<WulfrumEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<WulfrumScanner>(), 0, 0, player.whoAmI);
            }
        }
    }
}
