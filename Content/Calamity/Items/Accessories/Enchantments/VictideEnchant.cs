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
using FargowiltasCrossmod.Content.Calamity.Toggles;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("VictideEnchantment")]
    public class VictideEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Color nameColor => new(255, 233, 197);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
            Item.value = 15000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
            
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<VictideEffect>(item);
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyVictideHelms", 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Victide.VictideBreastplate>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Victide.VictideGreaves>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.SnapClam>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Melee.UrchinMace>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.UrchinStinger>(), 200);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class VictideEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Header ToggleHeader => Header.GetHeader<ExplorationHeader>();
        public override int ToggleItemType => ModContent.ItemType<VictideEnchant>();
        public override bool ExtraAttackEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            int damage;
            if (player.ForceEffect<VictideEffect>())
            {
                damage = 250;
            }
            else
            {
                damage = 38;
            }


            if (player.ownedProjectileCounts[ModContent.ProjectileType<VictideSpike>()] <= 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectileDirect(player.GetSource_EffectItem<VictideEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<VictideSpike>(), damage, 1, player.whoAmI, MathHelper.Lerp(0.6f, 3f, i/4f));
                    Projectile.NewProjectileDirect(player.GetSource_EffectItem<VictideEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<VictideSpike>(), damage, 1, player.whoAmI, -MathHelper.Lerp(0.6f, 3f, i / 4f));
                }
                Projectile.NewProjectileDirect(player.GetSource_EffectItem<VictideEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<VictideSpike>(), damage, 1, player.whoAmI, MathHelper.Pi);
                SoundEngine.PlaySound(SoundID.Item17 with { Pitch = -0.4f }, player.Center);
            }
        }
    }
}
