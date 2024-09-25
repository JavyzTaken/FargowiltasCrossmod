using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Artemis;
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
    public class ArtemisLaserSmall : ModProjectile, IPixelatedPrimitiveRenderer, IProjOwnedByBoss<Artemis>, IExoMechProjectile
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterProjectiles;

        /// <summary>
        /// How long this laser has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        /// <summary>
        /// The amount of max updates this laser has.
        /// </summary>
        public static int TotalUpdates => 4;

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Thermal;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 18;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.MaxUpdates = TotalUpdates;
            Projectile.timeLeft = Projectile.MaxUpdates * 300;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (Time >= 12f)
                Projectile.velocity *= 0.93f;
            if (Projectile.velocity.Length() <= 0.45f)
                Projectile.Kill();

            if (Projectile.FinalExtraUpdate())
                Time++;

            if (Main.rand.NextBool(30))
            {
                Dust spark = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), 261);
                spark.velocity = Main.rand.NextVector2Circular(1.5f, 1.5f);
                spark.scale = 0.9f;
                spark.color = Color.Lerp(Color.Yellow, Color.OrangeRed, Main.rand.NextFloat());
                spark.noGravity = true;
            }
        }

        public float LaserWidthFunction(float completionRatio)
        {
            float tipSqueeze = MathF.Pow(Utilities.InverseLerp(0f, 0.15f, completionRatio), 1.5f);
            return (1f - completionRatio) * tipSqueeze * Projectile.width * 0.67f + 0.5f;
        }

        public Color LaserColorFunction(float completionRatio)
        {
            Color startingColor = new(241, 168, 50);
            Color endingColor = new(203, 62, 24);
            return Color.Lerp(startingColor, endingColor, completionRatio * 0.81f) * Projectile.Opacity;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader trailShader = ShaderManager.GetShader("FargowiltasCrossmod.ArtemisLaserShotShader");
            trailShader.Apply();

            PrimitiveSettings settings = new(LaserWidthFunction, LaserColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: trailShader);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 32);
        }
    }
}
