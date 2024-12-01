using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HadesLaserBurst : ModProjectile, IPixelatedPrimitiveRenderer, IProjOwnedByBoss<ThanatosHead>, IExoMechProjectile
    {
        /// <summary>
        /// How long this laser burst has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Electricity;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = Projectile.MaxUpdates * 240;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (Main.rand.NextBool())
                ReleaseElectricSpark();

            Time++;
        }

        /// <summary>
        /// Releases a single electric spark out near the tip of this laser burst.
        /// </summary>
        public void ReleaseElectricSpark()
        {
            int sparkLifetime = Main.rand.Next(10, 21);
            Vector2 sparkVelocity = Main.rand.NextVector2Circular(4f, 4f);
            Color sparkColor = Color.Lerp(Color.Wheat, new(1f, 0.02f, 0.22f), Main.rand.NextFloat());
            ElectricSparkParticle spark = new(Projectile.Center + Main.rand.NextVector2Circular(20f, 20f) - Projectile.velocity * 1.1f, sparkVelocity, sparkColor, Color.Transparent, sparkLifetime, Vector2.One * 0.19f);
            spark.Spawn();
        }

        /// <summary>
        /// The width function for the laser primitives.
        /// </summary>
        /// <param name="completionRatio">How far along the trail the sampled position is.</param>
        public float LaserWidthFunction(float completionRatio)
        {
            float sizeInterpolant = MathF.Pow(Utilities.InverseLerpBump(0.05f, 0.13f, 0.87f, 0.95f, completionRatio), 0.9f);
            return MathHelper.Lerp(1f, Projectile.width * 0.6f, sizeInterpolant);
        }

        /// <summary>
        /// The color function for the laser primitives.
        /// </summary>
        /// <param name="completionRatio">How far along the trail the sampled position is.</param>
        public Color LaserColorFunction(float completionRatio) => new Color(1f, 0.1f, 0.22f) * Projectile.Opacity;

        /// <summary>
        /// The width function for the laser bloom's primitives.
        /// </summary>
        /// <param name="completionRatio">How far along the trail the sampled position is.</param>
        public float LaserWidthFunctionBloom(float completionRatio) => LaserWidthFunction(completionRatio) * 2.7f;

        /// <summary>
        /// The color function for the laser bloom's primitives.
        /// </summary>
        /// <param name="completionRatio">How far along the trail the sampled position is.</param>
        public Color LaserColorFunctionBloom(float completionRatio) => LaserColorFunction(completionRatio) * 0.3f;

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.HadesLaserShader");
            shader.TrySetParameter("glowIntensity", 1f);
            shader.TrySetParameter("noiseScrollOffset", Projectile.identity * 0.3149f);
            shader.SetTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Cracks"), 1, SamplerState.LinearWrap);

            PrimitiveSettings settings = new(LaserWidthFunction, LaserColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 21);

            shader.TrySetParameter("glowIntensity", 0.76f);
            shader.SetTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Cracks"), 1, SamplerState.LinearWrap);

            PrimitiveSettings bloomSettings = new(LaserWidthFunctionBloom, LaserColorFunctionBloom, _ => Projectile.Size * 0.5f + Projectile.velocity * 3f, Pixelate: true, Shader: shader);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, bloomSettings, 21);
        }
    }
}
