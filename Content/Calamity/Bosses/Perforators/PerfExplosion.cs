using FargowiltasSouls;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    public class PerfExplosion : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/GlowRingHollow";
        public const int Duration = 20;
        public const int BaseRadius = 80;
        public override void SetDefaults()
        {
            Projectile.width = BaseRadius;
            Projectile.height = BaseRadius;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Duration;
            Projectile.tileCollide = false;
            Projectile.light = 0.75f;
            Projectile.ignoreWater = true;
            //Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
            Projectile.FargoSouls().CanSplit = false;

            Projectile.scale = 1f;

        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Pitch = 0.3f }, Projectile.Center);
            //Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            => Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < projHitbox.Width / 2;
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Red * Projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
        }
        public override void AI()
        {
            Projectile.position = Projectile.Center;
            float scaleModifier = 1.7f;
            Projectile.scale += scaleModifier * 5f / Duration;
            Projectile.width = Projectile.height = (int)(BaseRadius * Projectile.scale);
            Projectile.Center = Projectile.position;
            if (Projectile.timeLeft < 8)
                Projectile.Opacity -= 0.15f;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            float rotation = Projectile.rotation;
            Vector2 drawPos = Projectile.Center;
            var texture = TextureAssets.Projectile[Projectile.type].Value;

            int sizeY = texture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int frameY = Projectile.frame * sizeY;
            Rectangle rectangle = new(0, frameY, texture.Width, sizeY);
            Vector2 origin = rectangle.Size() / 2f;
            float scaleModifier = (float)BaseRadius / sizeY;
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(lightColor),
                    rotation, origin, Projectile.scale * scaleModifier, spriteEffects, 0);
            return false;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.Knockback *= 3;
        }
    }
}