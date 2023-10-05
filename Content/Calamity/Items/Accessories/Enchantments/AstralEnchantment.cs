using Terraria;
using Terraria.ModLoader; 
using FargowiltasCrossmod.Core; 
using Microsoft.Xna.Framework;
using Terraria.ID;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using CalamityMod.Items.Weapons.Rogue;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using CalamityMod.Items.Armor.Astral;
using CalamityMod.Items.Accessories;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Typeless;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [AutoloadEquip(EquipType.Shield)]
    public class AstralEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(52, 237, 212);
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
            SBDPlayer.Astral = true;
            
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<AstralHelm>(1);
            recipe.AddIngredient<AstralBreastplate>(1);
            recipe.AddIngredient<AstralLeggings>(1);
            recipe.AddIngredient<TitanHeartEnchantment>(1);
            recipe.AddIngredient<RadiantStar>(1);
            recipe.AddIngredient<HideofAstrumDeus>(1);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void AstralEffects()
        {
            Player.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = true;
            if (Player.GetToggleValue("AstralShield"))
            {
                if (TitanGuardCooldown == 1)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit41, Player.Center);
                    for (int i = 0; i < 10; i++)
                        Dust.NewDust(Player.position, Player.width, Player.height, ModContent.DustType<CalamityMod.Dusts.AstralBasic>());
                }
                Player.hasRaisableShield = true;
                Player.shieldRaised = TitanGuardCooldown == 0 && Player.selectedItem != 58 && Player.controlUseTile && !Player.tileInteractionHappened && Player.releaseUseItem
                    && !Player.controlUseItem && !Player.mouseInterface && !CaptureManager.Instance.Active && !Main.HoveringOverAnNPC
                    && !Main.SmartInteractShowingGenuine && !Player.mount.Active &&
                    Player.itemAnimation == 0 && Player.itemTime == 0 && Player.reuseDelay == 0 && PlayerInput.Triggers.Current.MouseRight;
                if (Player.shieldRaised)
                {
                    if (Player.wingTime > 90)
                    {
                        Player.wingTime = 90;
                    }
                    Player.endurance += 0.3f;
                    Player.lifeRegen += 20;

                }
                for (int i = 3; i < 8 + Player.extraAccessorySlots; i++)
                {

                    if (Player.shield == -1 && Player.armor[i].shieldSlot != -1)
                        Player.shield = Player.armor[i].shieldSlot;
                }
            }
        }
        public void AstralPostUpdate()
        {
            Player.shieldRaised = TitanGuardCooldown == 0 && Player.selectedItem != 58 && Player.controlUseTile && !Player.tileInteractionHappened && Player.releaseUseItem
                && !Player.controlUseItem && !Player.mouseInterface && !CaptureManager.Instance.Active && !Main.HoveringOverAnNPC
                && !Main.SmartInteractShowingGenuine && !Player.mount.Active &&
                Player.itemAnimation == 0 && Player.itemTime == 0 && Player.reuseDelay == 0 && PlayerInput.Triggers.Current.MouseRight;
            if (Player.shieldRaised)
            {
                Player.bodyFrame.Y = Player.bodyFrame.Height * 10;

            }
        }
        public void AstralHurtEffects()
        {
            
            Player.shieldRaised = TitanGuardCooldown == 0 && Player.selectedItem != 58 && Player.controlUseTile && !Player.tileInteractionHappened && Player.releaseUseItem
               && !Player.controlUseItem && !Player.mouseInterface && !CaptureManager.Instance.Active && !Main.HoveringOverAnNPC
               && !Main.SmartInteractShowingGenuine && !Player.mount.Active &&
               Player.itemAnimation == 0 && Player.itemTime == 0 && Player.reuseDelay == 0 && PlayerInput.Triggers.Current.MouseRight;
            if (Player.shieldRaised)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<SabatonBoom>(), 2000, 5, ai0: 20);
                for (int i = 0; i < 360; i++)
                {
                    Vector2 speed = new Vector2(0, Main.rand.Next(10, 20)).RotatedBy(MathHelper.ToRadians(i));
                    Dust.NewDust(Player.Center, 0, 0, ModContent.DustType<AstralBlue>(), speed.X, speed.Y);
                    Vector2 speed2 = new Vector2(0, Main.rand.Next(10, 20)).RotatedBy(MathHelper.ToRadians(i));
                    Dust.NewDust(Player.Center, 0, 0, ModContent.DustType<AstralOrange>(), speed2.X, speed2.Y);

                }
                for (int j = 0; j < 2; j++)
                for (int i = -3; i < 4; i++)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center + new Vector2(100 * i, Main.rand.Next(-650, -600)), new Vector2(0, 25 * (j+1)).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)), ModContent.ProjectileType<AstralStar>(), 800, 2);
                }
                SoundEngine.PlaySound(SoundID.Item14, Player.Center);
                TitanGuardCooldown = 300;
                if (ForceEffect(ModContent.ItemType<AstralEnchantment>())) TitanGuardCooldown /= 2;
            }
        }
    }
}