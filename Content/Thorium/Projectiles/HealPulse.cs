using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class HealPulse : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.penetrate = -2;
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.scale = 0.1f;
        }

        public override void AI()
        {
            Projectile.scale *= 1.15f;
            Projectile.Center -= Projectile.Size * 0.075f; 
            Projectile.height = (int)(100 * Projectile.scale);
            Projectile.width = (int)(100 * Projectile.scale);
            Projectile.DLCHeal((int)Projectile.ai[0], Projectile.width);// Healing i-frames when?
            float x = 30 - Projectile.timeLeft;
            Projectile.alpha = (int)((x - 15f) * (x - 15f) * 100f / 225f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(lightColor.R - Projectile.alpha, lightColor.G - Projectile.alpha, lightColor.B - Projectile.alpha, 63 - Projectile.alpha / 4);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() / 2f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Bounds, GetAlpha(lightColor).Value, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}