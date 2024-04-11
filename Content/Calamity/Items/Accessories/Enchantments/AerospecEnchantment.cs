using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasSouls;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AerospecEnchantment : BaseEnchant
    {
        public override Color nameColor => new Color(153, 200, 193);
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 10);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<AerospecJumpEffect>(Item);
            player.AddEffect<AerospecFeathersEffect>(Item);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyAerospecHelms");
            recipe.AddIngredient<CalamityMod.Items.Armor.Aerospec.AerospecBreastplate>(1);
            recipe.AddIngredient<CalamityMod.Items.Armor.Aerospec.AerospecLeggings>(1);
            recipe.AddIngredient<CalamityMod.Items.Weapons.Rogue.Turbulance>(1);
            recipe.AddIngredient<CalamityMod.Items.Weapons.Magic.SkyGlaze>(1);
            recipe.AddIngredient<CalamityMod.Items.Accessories.AeroStone>(1);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AerospecJumpEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ExplorationHeader>();
        public override int ToggleItemType => ModContent.ItemType<AerospecEnchantment>();
        
        public override void PostUpdateEquips(Player player)
        {
            CrossplayerCalamity mplayer = player.GetModPlayer<CrossplayerCalamity>();
            player.GetJumpState<FeatherJump>().Enable();
            if (player.GetJumpState<FeatherJump>().Active)
            {
                Dust.NewDustDirect(player.BottomLeft, player.width, 0, DustID.UnusedWhiteBluePurple);
            }
            player.GetCritChance(DamageClass.Generic) += mplayer.AeroCritBoost;
            for (int i = 0; i < mplayer.AeroCritBoost / 5; i++)
            {
                if (Main.rand.NextBool())
                    Dust.NewDustDirect(player.position, player.width, player.height, DustID.UnusedWhiteBluePurple, player.velocity.X, player.velocity.Y);
            }
            if (Collision.SolidCollision(player.BottomLeft, player.width, 6, true) && player.velocity.Y == 0)
            {
                mplayer.AeroCritBoost = 0;
            }
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AerospecFeathersEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ExplorationHeader>();
        public override int ToggleItemType => ModContent.ItemType<AerospecEnchantment>();

    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class FeatherJump : ExtraJump
    {
        public override Position GetDefaultPosition()
        {
            return new Before(CloudInABottle);
        }

        public override float GetDurationMultiplier(Player player)
        {
            ref int jumps = ref player.GetModPlayer<CrossplayerCalamity>().FeatherJumpsRemaining;
            if (jumps > 0)
                return 1.2f;
            else return 0f;
        }
        public override void UpdateHorizontalSpeeds(Player player)
        {
            player.runAcceleration *= 3;
            player.maxRunSpeed *= 1.5f;
        }
        public override void OnRefreshed(Player player)
        {
            CrossplayerCalamity cplayer = player.GetModPlayer<CrossplayerCalamity>();
            if (player.ForceEffect<AerospecJumpEffect>())
            {
                cplayer.FeatherJumpsRemaining = 5;
            }
            else
            {
                cplayer.FeatherJumpsRemaining = 2;
            }
        }
        public override void OnStarted(Player player, ref bool playSound)
        {
            ref int jumps = ref player.GetModPlayer<CrossplayerCalamity>().FeatherJumpsRemaining;
            jumps--;
            if (jumps > 0)
            {
                player.GetJumpState(this).Available = true;
            }
            //cloud gores
            for (int i = -2; i < 3; i++)
            {
                Gore gor = Gore.NewGoreDirect(player.GetSource_FromThis(), player.Bottom + new Vector2(10 * i - 15, 0), Vector2.Zero, Main.rand.Next(11, 14));

                gor.velocity /= 3;
            }
            //aerospec dust
            for (int i = 0; i < 100; i++)
            {
                Dust.NewDustDirect(player.BottomLeft - new Vector2(20, 0), player.width + 40, 25, DustID.UnusedWhiteBluePurple);
            }
            //feather projectiles
            if (player.HasEffect<AerospecFeathersEffect>())
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 baseVelocity = player.velocity * Main.rand.NextFloat(0.9f, 1.2f);
                    if (baseVelocity.ToRotation() > -1f) baseVelocity = new Vector2(baseVelocity.Length(), 0).RotatedBy(-1f);
                    if (baseVelocity.ToRotation() < -2f) baseVelocity = new Vector2(baseVelocity.Length(), 0).RotatedBy(-2f);
                    //Main.NewText(baseVelocity.ToRotation());
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Bottom, baseVelocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)), ModContent.ProjectileType<FeatherJumpFeather>(), 20, 0);

                }
            }
        }
    }
}
