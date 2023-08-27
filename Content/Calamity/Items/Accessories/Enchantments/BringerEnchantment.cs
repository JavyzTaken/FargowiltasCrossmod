using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.CalPlayer;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;
namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{

    [ExtendsFromMod("CalamityMod")]
    public class BringerEnchantment : BaseEnchant
    {
        public int peaceTimer;
        
        protected override Color nameColor => new Color(128, 188, 67);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.ButterBeeSwarm = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Plaguebringer.PlaguebringerVisor>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Plaguebringer.PlaguebringerCarapace>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Plaguebringer.PlaguebringerPistons>());
            recipe.AddIngredient(ModContent.ItemType<FargowiltasSouls.Content.Items.Accessories.Enchantments.BeeEnchant>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.PlagueHive>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.EpidemicShredder>());
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void PlaguebringerEffects()
        {
            if (Player.GetToggleValue("PlagueBees"))
            {
                Player.GetModPlayer<CalamityPlayer>().plaguebringerPistons = true; //I mean why would I need to write all this shit down myself when it already exists in the form I need?
                Player.strongBees = true;
            }
            if (ButterBeeCD > 0) ButterBeeCD--;
        }
        public void PlaguebringerHitEffect(Item item, NPC target, int damage)
        {
            int bee;
            if (Player.GetToggleValue("PlagueDebuff"))
                target.AddBuff(ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.Plague>(), 300);
            if (ButterBeeCD <= 0 && target.realLife == -1 && Player.GetToggleValue("PlagueBees"))
            {
                if (damage > 0)
                {
                    if (!DevastEffects)
                    {
                        bee = Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center.X, target.Center.Y, Main.rand.Next(-35, 36) * 0.02f, Main.rand.Next(-35, 36) * 0.02f, ModContent.ProjectileType<CalamityMod.Projectiles.Rogue.PlaguenadeBee>(), damage, item.knockBack, Player.whoAmI);
                    }
                    else
                    {
                        if (!Main.rand.NextBool(2))
                            bee = Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center.X, target.Center.Y, Main.rand.Next(-35, 36) * 0.02f, Main.rand.Next(-35, 0) * 0.02f, ModContent.ProjectileType<CalamityMod.Projectiles.Rogue.PlaguenadeBee>(), damage, item.knockBack, Player.whoAmI);
                        else
                            bee = Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center.X, target.Center.Y, Main.rand.Next(-31, 32) * 0.2f, Main.rand.Next(-35, 0) * 0.02f, ModContent.ProjectileType<CalamityMod.Projectiles.Melee.PlagueSeeker>(), damage, item.knockBack, Player.whoAmI);
                    }
                    if (bee != 1000)
                    {
                        Main.projectile[bee].DamageType = DamageClass.Generic;
                    }
                }
                ButterBeeCD = 60;
            }
        }
        public void PlaguebringerProjHitEffect(Projectile proj, NPC target, int damage)
        {
            int bee;
            if (Player.GetToggleValue("PlagueDebuff"))
                target.AddBuff(ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.Plague>(), 300);
            if (ButterBeeCD <= 0 && target.realLife == -1 && proj.type != ModContent.ProjectileType<CalamityMod.Projectiles.Rogue.PlaguenadeBee>() && proj.type != ModContent.ProjectileType<CalamityMod.Projectiles.Melee.PlagueSeeker>() && proj.maxPenetrate != 1 && !proj.usesIDStaticNPCImmunity && proj.owner == Main.myPlayer && Player.GetToggleValue("PlagueBees"))
            {
                if (damage > 0)
                {
                    if (!DevastEffects)
                    {
                        bee = Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center.X, target.Center.Y, Main.rand.Next(-35, 36) * 0.02f, Main.rand.Next(-35, 36) * 0.02f, ModContent.ProjectileType<CalamityMod.Projectiles.Rogue.PlaguenadeBee>(), damage, proj.knockBack, Player.whoAmI);
                    }
                    else
                    {
                        if (!Main.rand.NextBool(2))
                            bee = Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center.X, target.Center.Y, Main.rand.Next(-35, 36) * 0.02f, Main.rand.Next(-35, 0) * 0.02f, ModContent.ProjectileType<CalamityMod.Projectiles.Rogue.PlaguenadeBee>(), damage, proj.knockBack, Player.whoAmI);
                        else
                            bee = Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center.X, target.Center.Y, Main.rand.Next(-31, 32) * 0.2f, Main.rand.Next(-35, 0) * 0.02f, ModContent.ProjectileType<CalamityMod.Projectiles.Melee.PlagueSeeker>(), damage, proj.knockBack, Player.whoAmI);
                    }
                    if (bee != 1000)
                    {
                        Main.projectile[bee].DamageType = DamageClass.Generic;
                    }
                }
            }
        }
    }
}
