using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core; 
using Microsoft.Xna.Framework;
using Terraria.ID;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Armor.TitanHeart;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using CalamityMod;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Dusts;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [AutoloadEquip(EquipType.Shield)]
    public class TitanHeartEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(102, 96, 117);
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
            SBDPlayer.TitanHeart = true;
            
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<TitanHeartMask>(1);
            recipe.AddIngredient<TitanHeartMantle>(1);
            recipe.AddIngredient<TitanHeartBoots>(1);
            recipe.AddIngredient<TitanArm>(1);
            recipe.AddIngredient<GacruxianMollusk>(1);
            recipe.AddIngredient<UrsaSergeant>(1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void TitanHeartEffects()
        {
            Player.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = true;
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
                if (Player.wingTime > 30)
                {
                    Player.wingTime = 30;
                }
                Player.endurance += 0.2f;
                Player.lifeRegen += 10;
                
            }
            for (int i = 3; i < 8 + Player.extraAccessorySlots; i++)
            {
                
                if (Player.shield == -1 && Player.armor[i].shieldSlot != -1)
                    Player.shield = Player.armor[i].shieldSlot;
            }
        }
        public void TitanHeartPostUpdate()
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
        public void TitanHeartHurtEffects()
        {
            Player.shieldRaised = TitanGuardCooldown == 0 && Player.selectedItem != 58 && Player.controlUseTile && !Player.tileInteractionHappened && Player.releaseUseItem
               && !Player.controlUseItem && !Player.mouseInterface && !CaptureManager.Instance.Active && !Main.HoveringOverAnNPC
               && !Main.SmartInteractShowingGenuine && !Player.mount.Active &&
               Player.itemAnimation == 0 && Player.itemTime == 0 && Player.reuseDelay == 0 && PlayerInput.Triggers.Current.MouseRight;
            if (Player.shieldRaised)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<SabatonBoom>(), 500, 5, ai0: 20);
                for(int i = 0; i < 360; i++)
                {
                    Vector2 speed = new Vector2(0, Main.rand.Next(10, 20)).RotatedBy(MathHelper.ToRadians(i));
                    Dust.NewDust(Player.Center, 0, 0, ModContent.DustType<AstralBlue>(), speed.X, speed.Y);
                    Vector2 speed2 = new Vector2(0, Main.rand.Next(10, 20)).RotatedBy(MathHelper.ToRadians(i));
                    Dust.NewDust(Player.Center, 0, 0, ModContent.DustType<AstralOrange>(), speed2.X, speed2.Y);

                }
                SoundEngine.PlaySound(SoundID.Item14, Player.Center);
                TitanGuardCooldown = 300;
                if (ForceEffect(ModContent.ItemType<TitanHeartEnchantment>())) TitanGuardCooldown /= 2;
            }

        }
    }
}