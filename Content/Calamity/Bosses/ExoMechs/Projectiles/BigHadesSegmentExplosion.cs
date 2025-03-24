using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Thanatos;
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
    public class BigHadesSegmentExplosion : ModProjectile, IExoMechProjectile, IProjOwnedByBoss<ThanatosHead>
    {
        public ExoMechDamageSource DamageType => ExoMechDamageSource.Electricity;

        /// <summary>
        /// The general purpose frame timer of this sphere.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        /// <summary>
        /// How long this sphere should exist for, in frames.
        /// </summary>
        public static int Lifetime => Utilities.SecondsToFrames(2.34f);

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public override void SetStaticDefaults() => Main.projFrames[Type] = 7;

        public override void SetDefaults()
        {
            Projectile.width = 650;
            Projectile.height = 650;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.scale = 2.5f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.Opacity = LumUtils.InverseLerp(1f, 0.75f, Time / Lifetime);
            Time++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.PrepareForShaders(BlendState.Additive);
            Vector2 size = Projectile.Size * Projectile.scale;

            float lifetimeRatio = MathF.Pow(Time / Lifetime, 0.67f);
            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.HadesElectricBoomShader");
            shader.SetTexture(MiscTexturesRegistry.DendriticNoise.Value, 1, SamplerState.LinearWrap);
            shader.SetTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise"), 2, SamplerState.LinearWrap);
            shader.TrySetParameter("lifetimeRatio", lifetimeRatio);
            shader.TrySetParameter("textureSize0", size);
            shader.Apply();

            Color color = Projectile.GetAlpha(new Color(0.9f, 0.24f, 0.23f));
            Texture2D pixel = MiscTexturesRegistry.Pixel.Value;

            Main.spriteBatch.Draw(pixel, drawPosition, null, color, 0f, pixel.Size() * 0.5f, size / pixel.Size() * 1.2f, 0, 0f);

            Main.spriteBatch.ResetToDefault();

            return false;
        }
    }
}
