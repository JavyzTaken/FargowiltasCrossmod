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
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ArtemisLaserImproved : ModProjectile, IPixelatedPrimitiveRenderer, IProjOwnedByBoss<Artemis>, IExoMechProjectile
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterProjectiles;

        /// <summary>
        /// Whether this laser should play a graze sound or not.
        /// </summary>
        public bool UsesGrazeSound => Projectile.ai[1] == 1f;

        /// <summary>
        /// How long this laser has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        /// <summary>
        /// The amount of max updates this laser has.
        /// </summary>
        public static int TotalUpdates => 4;

        /// <summary>
        /// The sound played upon grazing the player.
        /// </summary>
        public static readonly SoundStyle GrazeSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/ExoTwins/LaserGraze", 2) with { Volume = 1.2f };

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Thermal;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 18;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.MaxUpdates = TotalUpdates;
            Projectile.timeLeft = Projectile.MaxUpdates * 300;
            Projectile.Calamity().DealsDefenseDamage = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (Time >= 24f)
                Projectile.velocity *= 1.016f;

            if (Projectile.FinalExtraUpdate())
                Time++;

            if (UsesGrazeSound && Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.Zero), Projectile.SafeDirectionTo(Main.LocalPlayer.Center)) < 0.5f && Projectile.localAI[0] == 0f)
            {
                if (Main.LocalPlayer.WithinRange(Projectile.Center + Projectile.velocity * Projectile.MaxUpdates * 3f, 450f) && !Projectile.WithinRange(Main.LocalPlayer.Center, 60f))
                    SoundEngine.PlaySound(GrazeSound with { MaxInstances = 0 });
                Projectile.localAI[0] = 1f;
            }

            if (Main.rand.NextBool(10))
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
            return Color.Lerp(Color.Orange, new Color(1f, 0.1f, 0.31f), completionRatio * 0.76f) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacityFactor = MathHelper.Clamp((Projectile.velocity.Length() - 3f) * 0.7f, 1f, 20f);
            Texture2D bloom = MiscTexturesRegistry.BloomCircleSmall.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition - Projectile.velocity * 2f;
            Main.spriteBatch.Draw(bloom, drawPosition, null, Projectile.GetAlpha(Color.Orange) with { A = 0 } * opacityFactor * 0.4f, 0f, bloom.Size() * 0.5f, Projectile.scale * 0.5f, 0, 0f);
            Main.spriteBatch.Draw(bloom, drawPosition, null, Projectile.GetAlpha(Color.Yellow) with { A = 0 } * opacityFactor * 0.7f, 0f, bloom.Size() * 0.5f, Projectile.scale * 0.24f, 0, 0f);
            Main.spriteBatch.Draw(bloom, drawPosition, null, Projectile.GetAlpha(Color.Wheat) with { A = 0 } * (opacityFactor - 1f) * 0.95f, 0f, bloom.Size() * 0.5f, Projectile.scale * 0.189f, 0, 0f);
            return false;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader trailShader = ShaderManager.GetShader("FargowiltasCrossmod.ArtemisLaserShotShader");
            trailShader.Apply();

            PrimitiveSettings settings = new(LaserWidthFunction, LaserColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: trailShader);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 36);
        }
    }
}
