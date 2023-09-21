using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using CalamityMod;
using Terraria.Audio;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class HydrothermicEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(248, 182, 89);

        public override void SetStaticDefaults()
        {
            //name and description
            base.SetStaticDefaults();

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.AtaxiaEruption = true;
        }

        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyHydrothermHelms");
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Hydrothermic.HydrothermicArmor>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Hydrothermic.HydrothermicSubligar>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Magic.ForbiddenSun>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.HavocsBreath>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Hellborn>(), 1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void AtaxiaEffects()
        {
            if (AtaxiaDR <= 5) Player.endurance += 0.05f * AtaxiaDR;
            if (AtaxiaCountdown == 0 && AtaxiaDR < 5) AtaxiaCountdown = 30 * 60;
            if (AtaxiaDR == 5 && AtaxiaCountdown == 0)
            {
                SoundEngine.PlaySound(SoundID.Item74, Player.Center);
                AtaxiaDR = 0;
                Player.AddBuff(ModContent.BuffType<AtaxiaOverheat>(), 15 * 60);
            }
        }
        public void HydrothermicHitEffect(NPC target, int damage)
        {
            if (AtaxiaCooldown <= 0 && Player.ownedProjectileCounts[ModContent.ProjectileType<CalamityMod.Projectiles.Typeless.ChaoticGeyser>()] < 3)
            {
                int ataxiaDamage = CalamityUtils.DamageSoftCap(damage, 60);
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.position, Vector2.Zero, ModContent.ProjectileType<CalamityMod.Projectiles.Typeless.ChaoticGeyser>(), ataxiaDamage, 2f, Player.whoAmI);
                if (AtaxiaDR < 5) AtaxiaDR++;
                if (!DevastEffects) AtaxiaCooldown = 180; else AtaxiaCooldown = 60;
            }
        }
        public void HydrothermicProjHitEffect(NPC target, int damage)
        {
            if (AtaxiaCooldown <= 0 && Player.ownedProjectileCounts[ModContent.ProjectileType<CalamityMod.Projectiles.Typeless.ChaoticGeyser>()] < 3)
            {
                int ataxiaDamage = CalamityUtils.DamageSoftCap(damage, 60);
                Projectile.NewProjectile(Player.GetSource_FromThis(), target.position, Vector2.Zero, ModContent.ProjectileType<CalamityMod.Projectiles.Typeless.ChaoticGeyser>(), ataxiaDamage, 2f, Player.whoAmI);
                if (AtaxiaDR < 5) AtaxiaDR++;
                if (!DevastEffects) AtaxiaCooldown = 180; else AtaxiaCooldown = 60;
            }
        }
        public void AtaxiaHurt()
        {
            AtaxiaDR = 0;
        }
    }
}
