using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using FargowiltasCrossmod.Content.Thorium;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class AstroEnchant : BaseEnchant
    {
        public override Color nameColor => Color.LightBlue;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<FargowiltasSouls.Content.Items.Accessories.Masomode.MasoGravEffect>(Item);
            player.AddEffect<AstroEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThoriumMod.Items.SummonItems.AstroHelmet>()
                .AddIngredient<ThoriumMod.Items.SummonItems.AstroSuit>()
                .AddIngredient<ThoriumMod.Items.SummonItems.AstroBoots>()
                .Register();
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class AstroEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.SvartalfheimHeader>();
        public override bool ExtraAttackEffect => true;
        public override int ToggleItemType => ModContent.ItemType<AstroEnchant>();

        public override void PostUpdateEquips(Player player)
        {
            var DLCPlayer = player.ThoriumDLC();

            if (DLCPlayer.AstroLaserCD > 0) DLCPlayer.AstroLaserCD--;
        }

        public void SpawnAstroLaser(Player player, NPC target)
        {
            int Damage = 100;
            if (player.ForceEffect<AstroEffect>()) Damage += 50;
            if (player.position.Y < Main.worldSurface * 0.35 * 16) Damage += 50; // in space
            Vector2 pos = new(target.Center.X, MathHelper.Max(player.Center.Y - Main.screenHeight, 10f));

            Projectile.NewProjectile(GetSource_EffectItem(player), pos,
                Vector2.UnitY, ModContent.ProjectileType<Projectiles.SaucerDeathrayProj>(),
                Damage, 2f, player.whoAmI);

            player.ThoriumDLC().AstroLaserCD = 60;
        }

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (hitInfo.Crit && player.ThoriumDLC().AstroLaserCD <= 0)
            {
                SpawnAstroLaser(player, target);
            }
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium : ModPlayer
    {
    }
}

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    public class SaucerDeathrayProj : FargowiltasSouls.Content.Projectiles.Minions.SaucerDeathray
    {
        // code from SoulsMod SaucerDeathray
        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            float num801 = 0.4f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = num801;
            float num804 = Projectile.velocity.ToRotation();
            Projectile.rotation = num804 - 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            const int yOffset = 250;
            samplingPoint.Y += yOffset;
            float[] array3 = new float[(int)num805];
            Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 2400f, array3);
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807 + yOffset, amount);
            Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = Projectile.velocity.ToRotation() + (Main.rand.NextBool(2) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, DustID.CopperCoin, vector80.X, vector80.Y, 0, default, 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, DustID.CopperCoin, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }

            Projectile.position -= Projectile.velocity;
        }
    }
}
