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
using rail;
using Terraria.Localization;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{

    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class GemtechEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(200, 150, 0);

        public override void SetStaticDefaults()
        {


        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!Item.social)
            {
                string tooltip = "";
                Player player = Main.player[Main.myPlayer];
                if (player.HeldItem.DamageType == DamageClass.Melee || player.HeldItem.DamageType == DamageClass.MeleeNoSpeed || player.HeldItem.DamageType == ModContent.GetInstance<TrueMeleeDamageClass>() || player.HeldItem.DamageType == ModContent.GetInstance<TrueMeleeNoSpeedDamageClass>())
                    tooltip = Language.GetTextValue("Mods.FargowiltasCrossmod.Items.GemtechEnchantment.MeleeEffect");
                if (player.HeldItem.DamageType == DamageClass.Magic)
                    tooltip = Language.GetTextValue("Mods.FargowiltasCrossmod.Items.GemtechEnchantment.MagicEffect");
                if (player.HeldItem.DamageType == DamageClass.Summon || player.HeldItem.DamageType == DamageClass.SummonMeleeSpeed)
                    tooltip = Language.GetTextValue("Mods.FargowiltasCrossmod.Items.GemtechEnchantment.SummonEffect");
                if (player.HeldItem.DamageType == DamageClass.Ranged)
                    tooltip = Language.GetTextValue("Mods.FargowiltasCrossmod.Items.GemtechEnchantment.RangedEffect");
                if (player.HeldItem.DamageType == ModContent.GetInstance<RogueDamageClass>())
                    tooltip = Language.GetTextValue("Mods.FargowiltasCrossmod.Items.GemtechEnchantment.RogueEffect");
                tooltips.Insert(4, new TooltipLine(Mod, "GemtechAbility", tooltip));
            }
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
                    if (newHeld.DamageType == ModContent.GetInstance<RogueDamageClass>())
                        dustID = DustID.Firework_Red;
                    if (newHeld.DamageType == DamageClass.Summon)
                        dustID = DustID.BlueFairy;
                    Dust.NewDustDirect(Player.position, Player.width, Player.height, dustID).noGravity = true;
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
            //Main.NewText(item.DamageType);
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
            if (item.DamageType == DamageClass.Magic && mp.GemTechTimer >= 200)
            {
                for (int i = 0; i < 40; i++)
                {
                    int type = Main.rand.NextBool(7) ? 1 : 0;
                    Projectile.NewProjectile(player.GetSource_ItemUse(item),
                        player.Center + new Vector2(Main.rand.Next(-900, 900), 600),
                        new Vector2(0, Main.rand.Next(-22, -2)),
                        ModContent.ProjectileType<GemTechMagic>(),
                        player.CalcIntDamage<MagicDamageClass>(800), 0, Main.myPlayer, ai1: type);
                }
            }
            if (item.DamageType == ModContent.GetInstance<RogueDamageClass>() &&  mp.GemTechTimer >= 200)
            {
                player.SetImmuneTimeForAllTypes(90);
                for (int i = -1; i < 2; i++)
                {
                    Vector2 targetVel = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * 20;
                    Projectile.NewProjectile(player.GetSource_ItemUse(item), player.Center,
                        (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(i * 20)) * 6,
                        ModContent.ProjectileType<GemTechRogue>(), player.CalcIntDamage<RogueDamageClass>(3000), 0, Main.myPlayer, targetVel.X, targetVel.Y);
                }
            }
            mp.GemTechTimer = 0;
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    public partial class CalamityGlobalProjectile : GlobalProjectile
    {
        public int GemTechTimer;
        public void GemTechMinionEffect(Projectile proj)
        {
            
            if (proj.minion && Main.player[proj.owner].GetModPlayer<CrossplayerCalamity>().Gemtech && proj.active)
            {
                Player owner = Main.player[proj.owner];

                GemTechTimer++;
                
                if (owner.GetModPlayer<CrossplayerCalamity>().GemTechTimer >= 200 && GemTechTimer >= 200 && proj.active)
                {
                    proj.Kill();
                    for (int i = 0; i < 7; i++)
                    {
                        Projectile.NewProjectile(proj.GetSource_Death(), proj.Center,
                            new Vector2(Main.rand.Next(10, 20), 0).RotatedByRandom(MathHelper.TwoPi),
                            ModContent.ProjectileType<GemTechSummon>(), owner.CalcIntDamage<SummonDamageClass>(3000), 0, owner.whoAmI);
                    }
                    Projectile.NewProjectile(proj.GetSource_Death(), proj.Center, Vector2.Zero, ModContent.ProjectileType<GemTechSummonBoom>(), owner.CalcIntDamage<SummonDamageClass>(15000), 0, owner.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item14, proj.Center);
                }
            }

        }
    }
}