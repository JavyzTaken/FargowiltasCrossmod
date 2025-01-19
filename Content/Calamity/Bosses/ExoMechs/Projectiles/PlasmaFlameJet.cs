using CalamityMod.DataStructures;
using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Luminance.Core.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PlasmaFlameJet : ModProjectile, IPixelatedPrimitiveRenderer, IProjOwnedByBoss<AresBody>, IExoMechProjectile
    {
        /// <summary>
        /// The loop sound instance for this plasma jet.
        /// </summary>
        public LoopedSoundInstance LoopSoundInstance
        {
            get;
            private set;
        }

        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.AfterProjectiles;

        /// <summary>
        /// The owner of this flame jet.
        /// </summary>
        public NPC Owner
        {
            get
            {
                int ownerIndex = (int)Projectile.ai[0];
                if (ownerIndex <= -1 || ownerIndex >= Main.maxNPCs)
                    return null;

                NPC owner = Main.npc[ownerIndex];
                if (!owner.active || owner.type != ModContent.NPCType<AresHand>())
                    return null;

                return owner;
            }
        }

        /// <summary>
        /// The set of control points that compose this flame jet.
        /// </summary>
        public Vector2[] ControlPoints
        {
            get
            {
                float curvatureInterpolant = 0.4f;
                Vector2[] controlPoints = new Vector2[Projectile.oldPos.Length];
                for (int i = 0; i < controlPoints.Length; i++)
                {
                    controlPoints[i] = Projectile.Center + Vector2.UnitY * i / (float)(controlPoints.Length - 1f) * JetLength;

                    if (Projectile.oldPos[i] != Vector2.Zero)
                        controlPoints[i].X = MathHelper.Lerp(controlPoints[i].X, Projectile.oldPos[i].X + Projectile.width * 0.5f, curvatureInterpolant);
                }
                return controlPoints;
            }
        }

        /// <summary>
        /// How long this flame jet has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[1];

        /// <summary>
        /// The length of this flame jet.
        /// </summary>
        public ref float JetLength => ref Projectile.ai[2];

        /// <summary>
        /// The maximum length of this jet.
        /// </summary>
        public static float MaxJetLength => 2400f;

        /// <summary>
        /// The base color of this fire jet, before the shader is applied.
        /// </summary>
        public static Color BaseJetColor => new(147, 206, 74);

        /// <summary>
        /// The color of this fire jet's back bloom.
        /// </summary>
        public static Color BloomColor => new(99, 204, 5);

        /// <summary>
        /// The first fire particle color. Is intended to be interpolated with <see cref="FireParticleColorB"/>.
        /// </summary>
        public static Color FireParticleColorA => new(120, 255, 0);

        /// <summary>
        /// The second fire particle color. Is intended to be interpolated with <see cref="FireParticleColorA"/>.
        /// </summary>
        public static Color FireParticleColorB => new(255, 233, 72);

        /// <summary>
        /// The glow color applied to this fire jet in the shader.
        /// </summary>
        public static Vector4 GlowColor => new(3f, 3f, 1.5f, 0f);

        /// <summary>
        /// The sound played idly by this jet.
        /// </summary>
        public static readonly SoundStyle LoopSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/PlasmaJetLoop");

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Plasma;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
        }

        public override void SetDefaults()
        {
            Projectile.width = 148;
            Projectile.height = 148;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = PlasmaChaseSequence.AttackDuration;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (Owner is null)
            {
                Projectile.Kill();
                return;
            }

            Vector2 cannonEnd = Owner.Center + new Vector2(Owner.spriteDirection * 58f, 16f).RotatedBy(Owner.rotation);
            Projectile.Center = cannonEnd;
            Projectile.velocity = Vector2.UnitY;
            JetLength = MathHelper.Clamp(JetLength + 150f, 0f, MaxJetLength);

            if (Projectile.timeLeft <= 30)
                Projectile.width = (int)(Projectile.width * 0.935f);

            LoopSoundInstance ??= LoopedSoundManager.CreateNew(LoopSound, () => !Projectile.active);
            LoopSoundInstance?.Update(new Vector2(Projectile.Center.X, Main.LocalPlayer.Center.Y), sound =>
            {
                sound.Volume = Projectile.width / 954f;
            });

            BezierCurve shapeCurve = new(ControlPoints);
            for (int i = 0; i < 17; i++)
            {
                float fireScale = Main.rand.NextFloat(0.9f, 1.4f);
                Color fireColor = Color.Lerp(FireParticleColorA, FireParticleColorB, Main.rand.NextFloat());
                fireColor = Color.Lerp(fireColor, Color.Wheat, Main.rand.NextFloat(0.7f));

                Vector2 fireSpawnPosition = shapeCurve.Evaluate(Main.rand.NextFloat(0.075f, 1f));
                Vector2 fireVelocity = Main.rand.NextVector2Circular(20f, 7f) / fireScale + Vector2.UnitY * Main.rand.NextFloat(10f, 40f) + Owner.velocity;

                FireParticle fire = new(fireSpawnPosition, fireVelocity, fireColor * (Projectile.width / 150f), Main.rand.Next(20, 42), fireScale);
                fire.Spawn();
            }

            Time++;
        }

        /// <summary>
        /// The width function for the flame jet primitives.
        /// </summary>
        /// <param name="completionRatio">How far along the trail the sampled position is.</param>
        public float FlameJetWidthFunction(float completionRatio)
        {
            float widthFactor = MathHelper.Lerp(0.75f, 1f, completionRatio) * JetLength / MaxJetLength;
            return Projectile.width * widthFactor;
        }

        /// <summary>
        /// The color function for the flame jet primitives.
        /// </summary>
        /// <param name="completionRatio">How far along the trail the sampled position is.</param>
        public Color FlameJetColorFunction(float completionRatio) => BaseJetColor * Projectile.Opacity;

        /// <summary>
        /// The width function for the flame jet bloom primitives.
        /// </summary>
        /// <param name="completionRatio">How far along the trail the sampled position is.</param>
        public float FlameJetBloomWidthFunction(float completionRatio)
        {
            float widthFactor = MathHelper.SmoothStep(0f, 1f, LumUtils.InverseLerp(0.012f, 0.115f, completionRatio)) * 1.4f;
            return FlameJetWidthFunction(completionRatio) * widthFactor;
        }

        /// <summary>
        /// The color function for the flame jet bloom primitives.
        /// </summary>
        /// <param name="completionRatio">How far along the trail the sampled position is.</param>
        public Color FlameJetBloomColorFunction(float completionRatio)
        {
            float opacity = Projectile.Opacity * LumUtils.InverseLerp(0.94f, 0.8f, completionRatio);
            return BloomColor * opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.PrimitiveBloomShader");
            shader.TrySetParameter("innerGlowIntensity", 0.29f);

            PrimitiveSettings bloomSettings = new(FlameJetBloomWidthFunction, FlameJetBloomColorFunction, Shader: shader);
            PrimitiveRenderer.RenderTrail(ControlPoints, bloomSettings, 42);
            return false;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader flameShader = ShaderManager.GetShader("FargowiltasCrossmod.PlasmaFlameJetShader");
            flameShader.TrySetParameter("localTime", Main.GlobalTimeWrappedHourly + Projectile.identity * 0.412f);
            flameShader.TrySetParameter("glowPower", 2.5f);
            flameShader.TrySetParameter("glowColor", GlowColor);
            flameShader.TrySetParameter("edgeFadeThreshold", 0.1f);
            flameShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 1, SamplerState.LinearWrap);

            PrimitiveSettings settings = new(FlameJetWidthFunction, FlameJetColorFunction, _ => Vector2.Zero, Pixelate: true, Shader: flameShader);
            PrimitiveRenderer.RenderTrail(ControlPoints, settings, 35);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * JetLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.width * 0.6f, ref _);
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
