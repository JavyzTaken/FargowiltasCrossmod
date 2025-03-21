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
using FargowiltasCrossmod.Content.Calamity.Toggles;
using FargowiltasCrossmod.Core.Calamity;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Armor.SnowRuffian;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("SnowRuffianEnchantment")]
    public class SnowRuffianEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Color nameColor => new Color(160, 185, 213);
        
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SnowRuffianEffect>(Item);
            
        }
        
        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<SnowRuffianMask>();
            recipe.AddIngredient<SnowRuffianChestplate>();
            recipe.AddIngredient<SnowRuffianGreaves>();
            recipe.AddIngredient<IcicleStaff>();
            recipe.AddIngredient<FrostBlossomStaff>();
            recipe.AddIngredient(ItemID.Leather, 5);
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
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Header ToggleHeader => Header.GetHeader<GaleHeader>();
        public override int ToggleItemType => ModContent.ItemType<SnowRuffianEnchant>();
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
            float upwardAccel = 1f; //speed increase on Y while holding up

            if (player.wingTimeMax == 0)
            {
                player.Calamity().snowRuffianSet = true;
                if (player.ForceEffect<SnowRuffianEffect>())
                {
                    player.Calamity().snowRuffianSet = false;
                    player.wings = EquipLoader.GetEquipSlot(ModCompatibility.Calamity.Mod, "SnowRuffianMask", EquipType.Wings);
                    //player.wingsLogic = 46;
                    //player.wingTimeMax = player.GetWingStats(46).FlyTime;
                    int starlightWings = EquipLoader.GetEquipSlot(ModCompatibility.Calamity.Mod, "StarlightWings", EquipType.Wings);
                    player.wingsLogic = starlightWings;
                    player.wingTimeMax = 150;
                    player.wingAccRunSpeed = player.GetWingStats(starlightWings).AccRunAccelerationMult;
                }
                
                
            }
            
            if (player.jump == 0 && player.wingTime <= 0 && player.controlJump && player.velocity.X != 0)
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
                player.direction = player.velocity.X.DirectionalSign();
                player.CalamityAddon().RuffianModifiedRotation = true;
            }
            else
            {
                player.fullRotation = 0;
            }
        }
    }
}
