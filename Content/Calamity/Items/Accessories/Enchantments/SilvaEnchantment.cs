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
using CalamityMod.Rarities;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class SilvaEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(161, 255, 107);
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<DarkBlue>();
        }


        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine tooltip in tooltips)
            {
                int index = tooltip.Text.IndexOf("[button]");
                if (index != -1 && tooltip.Text.Length > 0)
                {
                    tooltip.Text = tooltip.Text.Remove(index, 8);
                    tooltip.Text = tooltip.Text.Insert(index, CalamityKeybinds.SetBonusHotKey.TooltipHotkeyString());
                }
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().Silva = true;

        }



        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnySilvaHelms");
            recipe.AddIngredient(ModContent.ItemType<SilvaArmor>());
            recipe.AddIngredient(ModContent.ItemType<SilvaLeggings>());
            recipe.AddIngredient(ModContent.ItemType<SarosPossession>());
            recipe.AddIngredient(ModContent.ItemType<YharimsCrystal>());
            recipe.AddIngredient(ModContent.ItemType<CrownJewel>());
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void SilvaEffects()
        {
            if (Main.myPlayer == Player.whoAmI)
            {

                if (Player.ownedProjectileCounts[ModContent.ProjectileType<LargeSilvaCrystal>()] < 1)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<LargeSilvaCrystal>(), 10, 0f, Main.myPlayer);
                    Player.AddBuff(ModContent.BuffType<LifeShell>(), 18000);
                }
            }
        }
        public void SilvaTrigger()
        {
            if (CalamityKeybinds.SetBonusHotKey.JustPressed && SilvaTimer == 0)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<LargeSilvaCrystal>() && Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer)
                    {
                        Main.projectile[i].ai[0] = 1;
                        SilvaTimer = 1800;
                    }
                }
            }
        }

    }
}
