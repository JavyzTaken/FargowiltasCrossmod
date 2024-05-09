using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using System.Security.Policy;
using Terraria.Graphics.Renderers;
using CalamityMod.Graphics.Renderers;
using Terraria.Graphics;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using CalamityMod.Projectiles.Turret;
using CalamityMod.Particles;
using Terraria.Audio;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasSouls;
using CalamityMod.Items.Armor.TitanHeart;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class TitanHeartEnchantment : BaseEnchant
    {

        public override Color nameColor => new Color(102, 96, 117);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Lime;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TitanHeartEffect>(Item);
            
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<TitanHeartMask>(1);
            recipe.AddIngredient<TitanHeartMantle>(1);
            recipe.AddIngredient<TitanHeartBoots>(1);
            recipe.AddIngredient<TitanArm>(1);
            recipe.AddIngredient<GacruxianMollusk>(1);
            recipe.AddIngredient<UrsaSergeant>(1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    public class TitanHeartEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DevastationHeader>();
        public override int ToggleItemType => ModContent.ItemType<UmbraphileEnchantment>();
        
        public override void PostUpdateEquips(Player player)
        {
            
        }
    }
}
