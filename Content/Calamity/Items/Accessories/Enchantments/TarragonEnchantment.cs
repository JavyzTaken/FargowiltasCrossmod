using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System.Collections.Generic;
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Accessories;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class TarragonEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(200, 100, 20);
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
        }


        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().Tarragon = true;

        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyTarragonHelms");
            recipe.AddIngredient(ModContent.ItemType<TarragonBreastplate>());
            recipe.AddIngredient(ModContent.ItemType<TarragonLeggings>());
            recipe.AddIngredient(ModContent.ItemType<LifefruitScythe>());
            recipe.AddIngredient(ModContent.ItemType<BadgeofBravery>());
            recipe.AddIngredient(ModContent.ItemType<SandCloak>());
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity.Items
{

    public partial class CalamityGlobalItem : GlobalItem
    {
        public void TarragonPickupEffect(Item item, Player player)
        {
            CrossplayerCalamity calplayer = player.GetModPlayer<CrossplayerCalamity>();
            if (calplayer != null && item.type == ItemID.Heart)
            {
                if (calplayer.Auric && player.GetToggleValue("AuricLightning") && player.GetToggleValue("TarragonCloak"))
                {
                    Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<AuricLightning>(), 1000, 0, Main.myPlayer, MathHelper.ToRadians(90), 0.6f);
                    Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<AuricLightning>(), 1000, 0, Main.myPlayer, MathHelper.ToRadians(120 + 90), 0.6f);
                    Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<AuricLightning>(), 1000, 0, Main.myPlayer, MathHelper.ToRadians(240 + 90), 0.6f);
                }
                if (calplayer.TarragonTimer == 0 && player.GetToggleValue("TarragonCloak"))
                {
                    player.AddBuff(ModContent.BuffType<TarragonCloak>(), 600);
                    calplayer.TarragonTimer = 1800;

                }
                if (player.GetToggleValue("TarragonAura"))
                {
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<TarragonAura>()] == 0 && player.statLife >= player.statLifeMax2)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<TarragonAura>(), player.statDefense * 2, 0, Main.myPlayer);
                        if (calplayer.Auric && player.GetToggleValue("AuricExplosions"))
                        {
                            Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<AuricExplosion>(), 1000, 0, Main.myPlayer, 22);
                            Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<AuricExplosion>(), 500, 0, Main.myPlayer, 11, 1);
                        }
                    }
                    else if (player.statLife >= player.statLifeMax2)
                    {
                        for (int i = 0; i < Main.projectile.Length; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<TarragonAura>() && Main.projectile[i].owner == player.whoAmI)
                            {
                                Main.projectile[i].timeLeft = 600;
                                break;
                            }
                        }
                        if (calplayer.Auric && player.GetToggleValue("AuricExplosions"))
                        {
                            Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<AuricExplosion>(), 1000, 0, Main.myPlayer, 15);
                        }
                    }
                }
            }
        }

    }
}
