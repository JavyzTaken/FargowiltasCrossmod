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
    public class AresCoreLaserSmall : ModProjectile, IPixelatedPrimitiveRenderer, IProjOwnedByBoss<Artemis>, IExoMechProjectile
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterProjectiles;

        /// <summary>
        /// How long this laser has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Electricity;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
            Projectile.MaxUpdates = 2;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
            float arcAngularVelocity = LumUtils.InverseLerp(90f, 0f, Time).Squared() * 0.03f;

            Vector2 targetDirection = target.velocity.SafeNormalize(Vector2.Zero);
            float verticalBiasInterpolant = MathF.Abs(targetDirection.Y) - MathF.Abs(targetDirection.X);
            arcAngularVelocity += verticalBiasInterpolant * 0.019f;

            Projectile.velocity = Projectile.velocity.RotateTowards(Projectile.AngleTo(target.Center), arcAngularVelocity);
            Projectile.velocity = (Projectile.velocity * 1.01f).ClampLength(0f, 11f);

            Time++;

            if (Main.rand.NextBool(12))
            {
                Dust spark = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), 261);
                spark.velocity = Main.rand.NextVector2Circular(1.5f, 1.5f);
                spark.scale = 1.05f;
                spark.color = Color.Lerp(Color.Crimson, Color.OrangeRed, Main.rand.NextFloat());
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
            Color startingColor = new(255, 16, 47);
            Color endingColor = startingColor.HueShift(-0.1f);
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
