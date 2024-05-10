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
using CalamityMod.Items.Weapons.Melee;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasSouls;
using CalamityMod.Items.Accessories.Wings;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class SnowRuffianEnchantment : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Color nameColor => new Color(160, 185, 213);
        
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SnowRuffianEffect>(Item);
            
        }
        
        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.SnowRuffian.SnowRuffianMask>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.SnowRuffian.SnowRuffianChestplate>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.SnowRuffian.SnowRuffianGreaves>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Magic.IcicleStaff>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Summon.FrostBlossomStaff>(), 1);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SnowRuffianEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Header ToggleHeader => Header.GetHeader<DevastationHeader>();
        public override int ToggleItemType => ModContent.ItemType<SnowRuffianEnchantment>();
        public override void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (player.wings >= 0 && player.wingTime <= 0 && player.controlUp)
            {
                player.wingFrame = 2;
                player.armorEffectDrawShadow = true;
            }
        }
        public override void PostUpdateEquips(Player player)
        {
            float neutralAcceleration = 0.15f; //speed X increase while gliding and not holding up or down
            float boostAcceleration = 0.25f; //speed X increase while holding down
            float risingDecel = 0.99f; //speed X is multiplied by this when holding up
            float upwardAccel = 0.8f; //speed increase on Y while holding up

            if (player.wingTimeMax == 0)
            {
                player.Calamity().snowRuffianSet = true;
                if (player.ForceEffect<SnowRuffianEffect>())
                {
                    player.Calamity().snowRuffianSet = false;
                    player.wings = EquipLoader.GetEquipSlot(ModCompatibility.Calamity.Mod, "SnowRuffianMask", EquipType.Wings);
                    player.wingsLogic = 46;
                    player.wingTimeMax = player.GetWingStats(46).FlyTime;
                }
                
                
            }
            
            if (player.jump == 0 && player.wingTime <= 0 && player.controlJump)
            {
                if (((player.direction == 1 && player.velocity.X < 15) || (player.direction == -1 && player.velocity.X > -15)) && !player.controlUp && player.velocity.Y > 0)
                {
                    float accel = neutralAcceleration;
                    if (player.controlDown) accel = boostAcceleration;

                    player.velocity.X += accel * player.direction;
                }
                
                if (player.controlUp && (player.velocity.ToRotation() > MathHelper.ToRadians(-120) || player.velocity.ToRotation() < MathHelper.ToRadians(-60)))
                {
                    player.velocity.X *= risingDecel;
                    if (Math.Abs(player.velocity.X) > 6 && player.dashDelay != -1 && player.velocity.Y > -10)
                    {
                        player.velocity.Y -= upwardAccel;
                        player.controlLeft = false;
                        player.controlRight = false;
                        player.releaseLeft = false;
                        player.releaseRight = false;
                        player.dashDelay = 30;
                        player.armorEffectDrawShadow = true;
                    }
                }
                player.fullRotation = player.velocity.ToRotation() + (player.velocity.X > 0 ? 0 : MathHelper.Pi);
                player.fullRotationOrigin = player.Size / 2;
            }
            else
            {
                player.fullRotation = 0;
            }
        }
    }
}
