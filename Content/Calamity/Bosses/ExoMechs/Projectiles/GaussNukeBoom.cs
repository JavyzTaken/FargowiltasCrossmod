using CalamityMod.NPCs.ExoMechs.Ares;
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
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class GaussNukeBoom : ModProjectile, IProjOwnedByBoss<AresBody>, IExoMechProjectile
    {
        /// <summary>
        /// How long this explosion has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[1];

        // This exists because using ai[0] for super big radii doesn't work because of stupid "Oh? A projectile it outside of the world? Kill it!" mechanic.
        /// <summary>
        /// The scale of this explosion.
        /// </summary>
        public ref float Scale => ref Projectile.ai[2];

        /// <summary>
        /// How long the explosion lasts.
        /// </summary>
        public static int Lifetime => Utilities.SecondsToFrames(1.2f);

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Thermal;

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 24000;

        public override void SetDefaults()
        {
            Projectile.width = 360;
            Projectile.height = 360;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = Lifetime;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Resize((int)Projectile.ai[0] + 360, (int)Projectile.ai[0] + 360);
            if (Scale >= 0.01f)
                Scale += 0.6f;
        }

        public override void AI()
        {
            Time++;
            Projectile.Opacity = Utilities.InverseLerp(1f, 0.82f, Time / Lifetime);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.PrepareForShaders(BlendState.Additive);
            Vector2 size = Projectile.Size * Projectile.scale;
            if (Scale > 0f)
                size *= Scale;

            float lifetimeRatio = Time / Lifetime;
            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.GaussNukeExplosionShader");
            shader.SetTexture(MiscTexturesRegistry.DendriticNoiseZoomedOut.Value, 1, SamplerState.LinearWrap);
            shader.TrySetParameter("lifetimeRatio", lifetimeRatio);
            shader.TrySetParameter("textureSize0", size);
            shader.Apply();

            Color color = Projectile.GetAlpha(new Color(1f, 0.54f, 0.09f));
            Texture2D pixel = MiscTexturesRegistry.Pixel.Value;

            Main.spriteBatch.Draw(pixel, drawPosition, null, color, 0f, pixel.Size() * 0.5f, size / pixel.Size() * 1.2f, 0, 0f);

            Main.spriteBatch.ResetToDefault();

            return false;
        }

        public override bool? CanDamage() => Projectile.Opacity >= 0.6f;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) =>
            Utilities.CircularHitboxCollision(Projectile.Center, MathF.Sqrt(Time / Lifetime) * Projectile.width * 0.52f, targetHitbox);
    }
}
