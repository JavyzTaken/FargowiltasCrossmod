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
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HomingTeslaBurst : ModProjectile, IPixelatedPrimitiveRenderer, IProjOwnedByBoss<AresBody>, IExoMechProjectile
    {
        /// <summary>
        /// Whether this burst has played a shooting sound yet.
        /// </summary>
        public bool HasPlayedShootSound
        {
            get;
            set;
        }

        /// <summary>
        /// How long this tesla burst has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        /// <summary>
        /// How long this burst spends homing in on players before accelerating.
        /// </summary>
        public static int HomeInTime => Utilities.SecondsToFrames(0.45f);

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Electricity;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
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
            if (!HasPlayedShootSound)
            {
                SoundEngine.PlaySound(AresTeslaCannon.TeslaOrbShootSound with { MaxInstances = 2, PitchVariance = 0.2f }, Projectile.Center);
                HasPlayedShootSound = true;
            }

            if (Main.rand.NextBool())
                ReleaseElectricSpark();

            if (Projectile.IsFinalExtraUpdate())
                Time++;

            float homeInInterpolant = MathF.Sqrt(Utilities.InverseLerp(HomeInTime, 0f, Time));
            Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
            if (!Projectile.WithinRange(target.Center, 270f))
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(target.Center) / Projectile.MaxUpdates * 14f, homeInInterpolant * 0.1f);

            if (Projectile.velocity.Length() < 24f)
            {
                float acceleration = Utils.Remap(homeInInterpolant, 0f, 0.4f, 1.0128f, 1f);
                Projectile.velocity *= acceleration;
            }
        }

        /// <summary>
        /// Releases a single electric spark out near the tip of this tesla burst.
        /// </summary>
        public void ReleaseElectricSpark()
        {
            int sparkLifetime = Main.rand.Next(7, 16);
            Vector2 sparkVelocity = Main.rand.NextVector2Circular(4f, 4f);
            Color sparkColor = Color.Lerp(Color.Wheat, new(0.15f, 0.7f, 1f), Main.rand.NextFloat());
            ElectricSparkParticle spark = new(Projectile.Center + Main.rand.NextVector2Circular(20f, 20f) - Projectile.velocity * 1.1f, sparkVelocity, sparkColor, Color.Transparent, sparkLifetime, Vector2.One * 0.19f);
            spark.Spawn();
        }

        /// <summary>
        /// The width function for the electricity primitives.
        /// </summary>
        /// <param name="completionRatio">How far along the trail the sampled position is.</param>
        public float ElectricityWidthFunction(float completionRatio)
        {
            float pulseInterpolant = MathF.Cos(Main.GlobalTimeWrappedHourly * 10f) * Utilities.InverseLerpBump(0f, 0.1f, 0.45f, 1f, completionRatio) * 0.2f;
            float sizeInterpolant = MathF.Pow(Utilities.InverseLerpBump(0.05f, 0.13f, 0.87f, 0.95f, completionRatio), 0.45f) + pulseInterpolant;
            return MathHelper.Lerp(1f, Projectile.width * 0.85f, sizeInterpolant);
        }

        /// <summary>
        /// The color function for the electricity primitives.
        /// </summary>
        /// <param name="completionRatio">How far along the trail the sampled position is.</param>
        public Color ElectricityColorFunction(float completionRatio) => new Color(0.19f, 0.78f, 1f) * Projectile.Opacity;

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.TeslaBurstShader");

            PrimitiveSettings settings = new(ElectricityWidthFunction, ElectricityColorFunction, _ => Projectile.Size * 0.5f + Projectile.velocity * 2f, Pixelate: true, Shader: shader);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 24);
        }
    }
}
