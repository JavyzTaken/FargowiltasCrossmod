using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Particles;
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
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HadesMine : ModProjectile, IExoMechProjectile, IProjOwnedByBoss<ThanatosHead>
    {
        public bool SetActiveFalseInsteadOfKill => true;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.BluntForceTrauma;

        /// <summary>
        /// How long this mine should exist before exploding, in frames.
        /// </summary>
        public ref float Lifetime => ref Projectile.ai[0];

        /// <summary>
        /// The general purpose frame timer of this mine.
        /// </summary>
        public ref float Time => ref Projectile.ai[1];

        /// <summary>
        /// The diameter of explosions created by this mine.
        /// </summary>
        public static float ExplosionDiameter => 250f;

        /// <summary>
        /// The sound played momentarily before this mine explodes.
        /// </summary>
        public static readonly SoundStyle ExplosionWarningSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/MineWarning") with { MaxInstances = 0, Volume = 0.54f };

        /// <summary>
        /// The sound played when this mine explodes.
        /// </summary>
        public static readonly SoundStyle ExplodeSound = new SoundStyle("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Hades/MineExplode", 3) with { MaxInstances = 0, Volume = 0.8f };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 624;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 72000;
            Projectile.Calamity().DealsDefenseDamage = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 6 % Main.projFrames[Projectile.type];

            Projectile.velocity *= 0.92f;

            DelegateMethods.v3_1 = new Vector3(1f, 0.8f, 0.35f);
            Utils.PlotTileLine(Projectile.Top, Projectile.Bottom, Projectile.width, DelegateMethods.CastLight);

            if (Time == Lifetime - 75)
                SoundEngine.PlaySound(ExplosionWarningSound, Projectile.Center);

            Time++;
            if (Time >= Lifetime)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion();
            DrawAreaOfEffectTelegraph();
            Main.spriteBatch.ExitShaderRegion();

            Utilities.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            DrawGlowTelegraph();

            return false;
        }

        public void DrawAreaOfEffectTelegraph()
        {
            float lifetimeRatio = Time / Lifetime;
            float opacity = Utilities.Saturate(lifetimeRatio * 5f) * Utilities.InverseLerp(15f, 7f, Projectile.velocity.Length()) * 0.275f;
            float maxFlashIntensity = Utilities.InverseLerp(0.25f, 0.75f, lifetimeRatio);
            float flashColorInterpolant = Utilities.Cos01(Main.GlobalTimeWrappedHourly * 10f).Squared() * maxFlashIntensity;
            Color innerColor = Color.Coral;
            Color edgeColor = Color.Lerp(Color.Yellow, Color.Wheat, 0.9f);

            innerColor = Color.Lerp(innerColor, Color.Crimson, MathF.Pow(flashColorInterpolant, 0.7f));
            edgeColor = Color.Lerp(edgeColor, Color.Red, flashColorInterpolant);

            var aoeShader = GameShaders.Misc["CalamityMod:CircularAoETelegraph"];
            aoeShader.UseOpacity(opacity);
            aoeShader.UseColor(innerColor);
            aoeShader.UseSecondaryColor(edgeColor);
            aoeShader.UseSaturation(lifetimeRatio);
            aoeShader.Apply();

            Texture2D pixel = MiscTexturesRegistry.InvisiblePixel.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(pixel, drawPosition, null, Color.White, 0, pixel.Size() * 0.5f, Vector2.One * ExplosionDiameter / pixel.Size(), 0, 0);
        }

        public void DrawGlowTelegraph()
        {
            float lifetimeRatio = Time / Lifetime;
            float maxFlashIntensity = Utilities.InverseLerp(0.75f, 0.9f, lifetimeRatio);
            float flashColorInterpolant = Utilities.Cos01(Main.GlobalTimeWrappedHourly * 32f).Squared() * maxFlashIntensity;
            float glowScale = Utils.Remap(lifetimeRatio, 0.8f, 1f, 1f, 1.8f);

            Texture2D glow = MiscTexturesRegistry.BloomCircleSmall.Value;
            Color flashColor = Color.Lerp(Color.Red, Color.Yellow, flashColorInterpolant * 0.7f) with { A = 0 } * Projectile.Opacity * maxFlashIntensity;
            Color glowColor = Color.Wheat with { A = 0 } * Projectile.Opacity * flashColorInterpolant * maxFlashIntensity;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(glow, drawPosition, null, flashColor, 0f, glow.Size() * 0.5f, glowScale * 0.3f, 0, 0f);
            Main.spriteBatch.Draw(glow, drawPosition, null, glowColor, 0f, glow.Size() * 0.5f, 0.14f, 0, 0f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Utilities.CircularHitboxCollision(Projectile.Center, Projectile.width * 0.5f, targetHitbox) && Projectile.velocity.Length() <= 3.2f;

        public override void OnKill(int timeLeft)
        {
            Projectile.velocity = Vector2.Zero;

            Projectile.Resize((int)(ExplosionDiameter * 0.8f), (int)(ExplosionDiameter * 0.8f));
            Projectile.Damage();

            ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 2.5f, MathHelper.TwoPi, null, 0.2f, 1185f, 500f);
            SoundEngine.PlaySound(ExplodeSound, Projectile.Center);

            // Create the generic burst explosion.
            MagicBurstParticle burst = new(Projectile.Center, Vector2.Zero, Color.Orange, 13, 1.15f);
            burst.Spawn();

            // Create sparks.
            for (int i = 0; i < 40; i++)
            {
                Vector2 sparkVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(8f, 25.5f);
                int sparkLifetime = (int)Utils.Remap(sparkVelocity.Length(), 25f, 8f, 7, 24);
                LineParticle spark = new(Projectile.Center, sparkVelocity, sparkVelocity.Length() <= 14.72f, sparkLifetime, 0.67f, Color.Yellow);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // Create smoke.
            for (int i = 0; i < 32; i++)
            {
                Vector2 smokeVelocity = Main.rand.NextVector2Circular(15f, 7f) - Vector2.UnitY * Main.rand.NextFloat(6f, 9f);
                if (Main.rand.NextBool())
                    smokeVelocity.Y *= -0.4f;

                SmokeParticle smoke = new(Projectile.Center, smokeVelocity, Color.Lerp(Color.LightGray, Color.Red, Main.rand.NextFloat(0.75f)), Main.rand.Next(45, 60), 0.85f, 0.0875f);
                smoke.Spawn();
            }

            // Create inner lingering fire.
            for (int i = 0; i < 30; i++)
            {
                int fireLifetime = Main.rand.Next(25, 50);
                float fireRotationalVelocity = Main.rand.NextFloatDirection() * 0.08f;
                Vector2 fireVelocity = Main.rand.NextVector2Circular(5f, 3f) - Vector2.UnitY * Main.rand.NextFloat(2.6f, 3.3f);
                if (Main.rand.NextBool())
                    fireVelocity.Y *= -0.6f;
                fireVelocity *= 3f;

                HeavySmokeParticle fire = new(Projectile.Center, fireVelocity, Color.Lerp(Color.Orange, Color.Wheat, Main.rand.NextFloat(0.75f)), fireLifetime, 0.7f, 1f, fireRotationalVelocity, true);
                GeneralParticleHandler.SpawnParticle(fire);
            }
        }
    }
}
