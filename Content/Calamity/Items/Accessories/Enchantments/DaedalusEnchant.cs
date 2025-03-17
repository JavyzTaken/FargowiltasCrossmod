using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using CalamityMod;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using Terraria.Audio;
using FargowiltasCrossmod.Core.Calamity;
using rail;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasSouls;
using Terraria.GameContent.ItemDropRules;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Projectiles.Ranged;
using Mono.Cecil;
using static System.Net.Mime.MediaTypeNames;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("DaedalusEnchantment")]
    public class DaedalusEnchant : BaseEnchant
    {
        public static readonly Color NameColor = new(132, 212, 246);
        public override Color nameColor => NameColor;
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.FargoSouls().WingTimeModifier += 0.25f;
            player.AddEffect<DaedalusEffect>(item);
        }
        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyDaedalusHelms", 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusBreastplate>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusLeggings>(), 1);
            recipe.AddIngredient(ItemID.IceRod, 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.Wings.SoulofCryogen>(), 1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DaedalusEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ElementsHeader>();
        public override int ToggleItemType => ModContent.ItemType<DaedalusEnchant>();
        public const float WindupTime = 60f;
        public override void PostUpdateEquips(Player player)
        {
            var addonPlayer = player.CalamityAddon();
            if (player.HasEffect<ElementsForceEffect>() && !addonPlayer.ReaverToggle)
                return;
            if (player.velocity.Y != 0)
            {
                addonPlayer.DaedalusTimer++;
                if (addonPlayer.DaedalusTimer == WindupTime)
                    addonPlayer.DaedalusTimer += 60;
                //if (player.whoAmI == Main.myPlayer)
                //    CooldownBarManager.Activate("DaedalusEnchantWings", ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Items/Accessories/Enchantments/DaedalusEnchant").Value, DaedalusEnchant.NameColor,
                //        () => LumUtils.Saturate(Main.LocalPlayer.CalamityAddon().DaedalusTimer / WindupTime), true, activeFunction: player.HasEffect<DaedalusEffect>);
            }
            else
            {
                addonPlayer.DaedalusTimer = 0;
            }
        }
        public override void TryAdditionalAttacks(Player player, int damage, DamageClass damageType)
        {
            var addonPlayer = player.CalamityAddon();
            if (addonPlayer.DaedalusTimer > WindupTime + 40)
            {
                addonPlayer.DaedalusTimer = (int)WindupTime;
                bool forceEffect = player.ForceEffect<DaedalusEffect>();
                float arrowSpeed = forceEffect ? 16f : 12f;
                int projDamage = forceEffect ? 100 : 65;
                if (player.HasEffect<ElementsForceEffect>())
                {
                    projDamage = 120;
                    arrowSpeed = 22;
                }
                projDamage = FargoSoulsUtil.HighestDamageTypeScaling(player, projDamage);

                int amt = forceEffect ? 6 : 4;
                float knockback = 1f;
                // ignore all this shit
                for (int i = 0; i < amt; i++)
                {
                    Vector2 realPlayerPos = new Vector2(player.position.X + player.width * 0.5f + (Main.rand.Next(300) * -(float)player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                    realPlayerPos.X = (realPlayerPos.X + player.Center.X) / 2f + Main.rand.Next(-300, 300);
                    realPlayerPos.Y -= 100f;
                    float mouseXDist = Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
                    float mouseYDist = Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
                    if (mouseYDist < 0f)
                        mouseYDist *= -1f;
                    if (mouseYDist < 20f)
                        mouseYDist = 20f;
                    float mouseDistance = (float)Math.Sqrt(mouseXDist * mouseXDist + mouseYDist * mouseYDist);
                    mouseDistance = arrowSpeed / mouseDistance;
                    mouseXDist *= mouseDistance;
                    mouseYDist *= mouseDistance;


                    float speedX4 = mouseXDist + Main.rand.Next(-40, 41) * 0.02f;
                    float speedY5 = mouseYDist + Main.rand.Next(-40, 41) * 0.02f;
                    Projectile.NewProjectile(this.GetSource_EffectItem(player), realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, ModContent.ProjectileType<DaedalusArrow>(), projDamage, knockback, player.whoAmI);
                }
            }
        }
        public override void OnHitByEither(Player player, NPC npc, Projectile proj)
        {
            var addonPlayer = player.CalamityAddon();
            if (addonPlayer.DaedalusTimer > WindupTime)
            {
                addonPlayer.DaedalusTimer = 0;
                if (!player.HasEffect<ElementsForceEffect>())
                    player.wingTime /= 2;
            }
        }
    }
}
