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
using Luminance.Core.Graphics;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasSouls;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.SolarEclipse;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Projectiles.Typeless;
using FargowiltasSouls.Content.Items.Accessories.Masomode;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("ReaverEnchantment")]
    public class ReaverEnchant : BaseEnchant
    {
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<ReaverEffect>()];
        public override Color nameColor => new(145, 203, 102);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Lime;
            Item.value = 250000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ReaverEffect>(Item);
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyReaverHelms", 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Reaver.ReaverScaleMail>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Reaver.ReaverCuisses>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Tools.BeastialPickaxe>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.NecklaceofVexation>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.SpelunkersAmulet>(), 1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ReaverEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            var addon = player.CalamityAddon();
            addon.ReaverToggle = !addon.ReaverToggle;
            SoundEngine.PlaySound(SoundID.Item4, player.Center);
            Color color = addon.ReaverToggle ? Color.Red : Color.Green;
            for (int i = 0; i < 14; i++)
            {
                ReaverSpark spark = new(new Vector2(player.Center.X + Main.rand.NextFloat(-10, 10), player.Center.Y + Main.rand.NextFloat(-10, 10)), Main.rand.NextVector2Circular(4, 4),
                    color, 0.3f, 20, 10, player.whoAmI);
                spark.Spawn();
            }
        }
        public override void PostUpdateEquips(Player player)
        {
            bool force = player.ForceEffect<ReaverEffect>();
            var addon = player.CalamityAddon();
            if (addon.ReaverToggle) // swift mode
            {
                player.lifeRegen += force ? 15 : 8;
                player.moveSpeed += force ? 0.3f : 0.15f;
                if (player.miscCounter % 3 == 2 && player.dashDelay > 0)
                    player.dashDelay--;
                player.Calamity().reaverSpeed = true;
            }
            else // plated mode
            {
                player.statDefense += force ? 30 : 15;
                player.endurance += force ? 0.3f : 0.15f;
            }
            Color color = addon.ReaverToggle ? Color.Red : Color.Green;
            if (Main.rand.NextBool(6))
            {
                ReaverSpark spark = new(new Vector2(player.Center.X + Main.rand.NextFloat(-10, 10), player.Center.Y + Main.rand.NextFloat(-10, 10)), Main.rand.NextVector2Circular(4, 4),
                    color, 0.3f, 20, 10, player.whoAmI);
                spark.Spawn();
            }
        }
        public override void OnHitByProjectile(Player player, Projectile proj, Player.HurtInfo hurtInfo)
        {
            ReaverOnHit(player, hurtInfo);
        }
        public override void OnHitByNPC(Player player, NPC npc, Player.HurtInfo hurtInfo)
        {
            ReaverOnHit(player, hurtInfo);
        }
        public static void ReaverOnHit(Player player, Player.HurtInfo hurtInfo)
        {
            bool force = player.ForceEffect<ReaverEffect>();
            if (player.CalamityAddon().ReaverToggle)
            {

            }
            else
            {
                player.AddBuff(ModContent.BuffType<ReaverRage>(), 60 * (force ? 5 : 10));
                var source = player.GetSource_Misc("23");
                if (hurtInfo.Damage > 0)
                {
                    int rDamage = FargoSoulsUtil.HighestDamageTypeScaling(player, force ? 700 : 400);

                    if (player.whoAmI == Main.myPlayer)
                        Projectile.NewProjectile(source, player.Center.X, player.position.Y + 36f, 0f, -18f, ModContent.ProjectileType<ReaverThornBase>(), rDamage, 0f, player.whoAmI, 0f, 0f);
                }
            }
        }
    }
}
