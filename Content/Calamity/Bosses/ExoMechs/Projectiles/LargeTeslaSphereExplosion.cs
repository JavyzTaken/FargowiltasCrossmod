using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class LargeTeslaSphereExplosion : ModProjectile, IProjOwnedByBoss<AresBody>, IExoMechProjectile
    {
        /// <summary>
        /// How long this sphere has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        /// <summary>
        /// How long the explosion lasts.
        /// </summary>
        public static int Lifetime => Utilities.SecondsToFrames(0.33f);

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Electricity;

        public override void SetDefaults()
        {
            Projectile.width = 1200;
            Projectile.height = 1200;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = Lifetime;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
                CreateElectricPixel();

            Time++;
        }

        /// <summary>
        /// Creates a single swirling electric pixel around the explosion.
        /// </summary>
        public void CreateElectricPixel()
        {
            float pixelOffsetAngle = (Main.rand.NextGaussian(0.17f) + MathHelper.PiOver2) * Main.rand.NextFromList(-1f, 1f);
            Vector2 pixelSpawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height) * Time / Lifetime * 0.85f;
            Vector2 pixelVelocity = Projectile.SafeDirectionTo(pixelSpawnPosition).RotatedBy(pixelOffsetAngle) * Main.rand.NextFloat(9f, 40f);

            BloomPixelParticle pixel = new(pixelSpawnPosition, pixelVelocity, Color.White, Color.DeepSkyBlue * 0.4f, 36, Vector2.One * Main.rand.NextFloat(1.4f, 2.3f));
            pixel.Spawn();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.PrepareForShaders();

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.TeslaExplosionShader");
            shader.SetTexture(MiscTexturesRegistry.TurbulentNoise.Value, 1, SamplerState.LinearWrap);
            shader.TrySetParameter("lifetimeRatio", Time / Lifetime);
            shader.TrySetParameter("textureSize0", Projectile.Size);
            shader.Apply();

            Texture2D pixel = MiscTexturesRegistry.Pixel.Value;
            Main.spriteBatch.Draw(pixel, drawPosition, null, Projectile.GetAlpha(new Color(0.22f, 0.76f, 1f)), 0f, pixel.Size() * 0.5f, Projectile.Size * Projectile.scale / pixel.Size() * 1.2f, 0, 0f);

            Main.spriteBatch.ResetToDefault();

            return false;
        }
    }
}
