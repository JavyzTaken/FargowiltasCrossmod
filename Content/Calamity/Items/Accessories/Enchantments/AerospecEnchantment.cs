using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core; 
using Microsoft.Xna.Framework;
using Terraria.ID;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AerospecEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(153, 200, 193);
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.RideOfTheValkyrie = true;
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
            if (cplayer.ForceEffect(ModContent.ItemType<AerospecEnchantment>()))
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
            for (int i = 0; i <100; i++)
            {
                Dust.NewDustDirect(player.BottomLeft - new Vector2(20, 0), player.width + 40, 25, DustID.UnusedWhiteBluePurple);
            }
            //feather projectiles
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
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {

        public void AerospecEffects()
        {
            Player.GetJumpState<FeatherJump>().Enable();
            if (Player.GetJumpState<FeatherJump>().Active)
            {
                Dust.NewDustDirect(Player.BottomLeft, Player.width, 0, DustID.UnusedWhiteBluePurple);
            }
            Player.GetCritChance(DamageClass.Generic) += AeroCritBoost;
            
            for (int i = 0; i < AeroCritBoost/5; i++)
            {
                if (Main.rand.NextBool())
                Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.UnusedWhiteBluePurple, Player.velocity.X, Player.velocity.Y);
            }
           if (Collision.SolidCollision(Player.BottomLeft, Player.width, 6, true) && Player.velocity.Y == 0)
            {
                AeroCritBoost = 0;
            }
        }
    }
}