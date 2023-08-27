using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items.Armor.Prismatic;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;
using CalamityMod.Items.Armor.GemTech;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Rarities;
using CalamityMod.CalPlayer;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{

    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class GemtechEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(200, 150, 0);

        public override void SetStaticDefaults()
        {


        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.Gemtech = true;
        }

        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<GemTechHeadgear>(), 1);
            recipe.AddIngredient(ModContent.ItemType<GemTechBodyArmor>(), 1);
            recipe.AddIngredient(ModContent.ItemType<GemTechSchynbaulds>(), 1);
            recipe.AddIngredient(ModContent.ItemType<Karasawa>(), 1);
            recipe.AddIngredient(ModContent.ItemType<ArtAttack>(), 1);
            recipe.AddIngredient(ModContent.ItemType<WarloksMoonFist>(), 1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        Item HeldItem = null;
        public int GemTechTimer;
        public void GemTechEffects()
        {
            
            Item newHeld = Player.HeldItem;
            if (HeldItem == null || newHeld == null || HeldItem != newHeld || newHeld.damage == -1)
            {
                GemTechTimer = 0;
            }
            else if (GemTechTimer < 200)
            {
                GemTechTimer += 1;
            }
            if (GemTechTimer == 199)
            {
                for (int i = 0; i < 30; i++)
                {
                    int dustID = DustID.YellowStarDust;
                    if (newHeld.DamageType == DamageClass.Ranged)
                        dustID = DustID.GreenFairy;
                    if (newHeld.DamageType == DamageClass.Magic)
                        dustID = DustID.PinkFairy;
                    if (newHeld.DamageType == DamageClass.Throwing)
                        dustID = DustID.RedTorch;
                    if (newHeld.DamageType == DamageClass.Summon)
                        dustID = DustID.BlueFairy;
                    Dust.NewDustDirect(Player.position, Player.width, Player.height, dustID);
                }
                SoundEngine.PlaySound(SoundID.Item4 with { Pitch = -0.5f}, Player.Center);
            }
            
            HeldItem = newHeld;
            
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity.Items
{
    public partial class CalamityGlobalItem : GlobalItem
    {
        public void GemTechUseEffect(Item item, Player player)
        {
            CrossplayerCalamity mp = player.GetModPlayer<CrossplayerCalamity>();
            Main.NewText(item.DamageType);
            if ((item.DamageType == DamageClass.Melee || item.DamageType == DamageClass.MeleeNoSpeed || item.DamageType == ModContent.GetInstance<TrueMeleeDamageClass>() || item.DamageType == ModContent.GetInstance<TrueMeleeNoSpeedDamageClass>()) && mp.GemTechTimer >= 200)
            {
                
                for (int i = -2; i < 3; i++) {
                    Projectile.NewProjectile(player.GetSource_ItemUse(item), player.Center,
                        (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(i * 5)) * 12,
                        ModContent.ProjectileType<GemTechMelee>(), player.CalcIntDamage<MeleeDamageClass>(3000), 0, Main.myPlayer);
                    }
                Projectile.NewProjectile(player.GetSource_ItemUse(item), player.Center,
                        (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * 22,
                        ModContent.ProjectileType<GemTechMeleeWave>(), player.CalcIntDamage<MeleeDamageClass>(15000), 0, Main.myPlayer);
                SoundEngine.PlaySound(SoundID.Item60, player.Center);
            }
            if (item.DamageType == DamageClass.Ranged && mp.GemTechTimer >= 200)
            {
                for (int i = 0; i < 30; i++)
                {
                    Projectile.NewProjectile(player.GetSource_ItemUse(item), player.Center + new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10)),
                        (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(7f, 15f),
                        ModContent.ProjectileType<GemTechRanged>(), player.CalcIntDamage<RangedDamageClass>(1000), 0, Main.myPlayer);
                }
                SoundEngine.PlaySound(SoundID.Item60, player.Center);
            }
            mp.GemTechTimer = 0;
        }
    }
}