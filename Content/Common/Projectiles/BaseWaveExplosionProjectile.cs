using FargowiltasSouls;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using static Microsoft.Xna.Framework.MathHelper;
using static System.MathF;

namespace FargowiltasCrossmod.Content.Common.Projectiles
{
    public abstract class BaseWaveExplosionProjectile : ModProjectile
    {
        public float Radius
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public virtual float Opacity { get; } = 1f;

        public virtual float MinScale { get; } = 1.2f;

        public virtual float MaxScale { get; } = 5f;

        public virtual Texture2D ExplosionNoiseTexture => ModContent.Request<Texture2D>("Terraria/Images/Misc/Perlin").Value;

        public abstract int Lifetime { get; }

        public abstract float MaxRadius { get; }

        public abstract float RadiusExpandRateInterpolant { get; }

        public abstract float DetermineScreenShakePower(float lifetimeCompletionRatio, float distanceFromPlayer);

        public abstract Color DetermineExplosionColor(float lifetimeCompletionRatio);

        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Lifetime;
            Projectile.scale = 0.001f;
        }

        public override void AI()
        {
            // Cause the wave to expand outward, along with its hitbox.
            Radius = Lerp(Radius, MaxRadius, RadiusExpandRateInterpolant);
            Projectile.scale = Lerp(MinScale, MaxScale, Utils.GetLerpValue(Lifetime, 0f, Projectile.timeLeft, true));
            ExpandHitboxBy((int)(Radius * Projectile.scale), (int)(Radius * Projectile.scale));
        }
        public void ExpandHitboxBy(int width, int height)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = width;
            Projectile.height = height;
            Projectile.position -= Projectile.Size * 0.5f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return LumUtils.CircularHitboxCollision(Projectile.Center, Radius * 0.4f, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 scale = new(1.5f, 1f);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + Projectile.Size * scale * 0.5f;
            DrawData explosionDrawData = new(
                ExplosionNoiseTexture,
                drawPosition,
                new Rectangle(0, 0, Projectile.width, Projectile.height),
                new Color(new Vector4(Sqrt(Projectile.timeLeft / (float)Lifetime))) * 0.7f * Opacity,
                Projectile.rotation,
                Projectile.Size,
                scale,
                SpriteEffects.None,
                0);

            GameShaders.Misc["ForceField"].UseColor(DetermineExplosionColor(1f - Projectile.timeLeft / (float)Lifetime));
            GameShaders.Misc["ForceField"].Apply(explosionDrawData);
            explosionDrawData.Draw(Main.spriteBatch);

            Main.spriteBatch.ResetToDefault();
            return false;
        }
    }
}
