using CalamityMod;
using CalamityMod.NPCs;
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
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HadesExoEnergyOrb : ModProjectile, IProjOwnedByBoss<ThanatosHead>, IExoMechProjectile
    {
        public bool SetActiveFalseInsteadOfKill => true;

        /// <summary>
        /// The lifetime ratio of this orb.
        /// </summary>
        public float LifetimeRatio => LumUtils.Saturate(Time / Lifetime);

        /// <summary>
        /// How long this orb should exist for, in frames.
        /// </summary>
        public ref float Lifetime => ref Projectile.ai[0];

        /// <summary>
        /// How long this orb has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[1];

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Electricity;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.hide = true;
            Projectile.timeLeft = 240000;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.draedonExoMechWorm == -1)
            {
                Projectile.Kill();
                return;
            }

            NPC hades = Main.npc[CalamityGlobalNPC.draedonExoMechWorm];
            float reachInterpolant = LumUtils.InverseLerp(0.92f, 0.75f, LifetimeRatio);
            Projectile.rotation = hades.velocity.ToRotation();
            Projectile.scale = LumUtils.InverseLerpBump(0f, 0.4f, 0.7f, 0.92f, LifetimeRatio);
            Projectile.Center = hades.Center + Projectile.rotation.ToRotationVector2() * Projectile.width * MathHelper.Lerp(0.06f, 0.7f, reachInterpolant);

            CreateElectricParticles();

            if (Time % 2f == 1f && LifetimeRatio <= 0.8f)
                CreateElectricSpark();

            if (Time % 20f == 19f && LifetimeRatio <= 0.85f)
                CustomExoMechsSky.CreateLightning(Projectile.Center.ToScreenPosition());

            Time++;
            if (Time >= Lifetime)
                Projectile.Kill();
        }

        /// <summary>
        /// Creates electric particles and dust around the orb.
        /// </summary>
        public void CreateElectricParticles()
        {
            if (Time % 2f == 0f && LifetimeRatio <= 0.7f)
            {
                Vector2 pixelSpawnOffset = Main.rand.NextVector2Unit() * Projectile.width * Projectile.scale;
                float pixelSpawnDirectionAngle = MathHelper.Pi / Main.rand.NextFloat(2f, 5f) * pixelSpawnOffset.X.NonZeroSign();
                Vector2 pixelSpawnPosition = Projectile.Center + pixelSpawnOffset;
                Vector2 pixelVelocity = (Projectile.Center - pixelSpawnPosition).RotatedBy(pixelSpawnDirectionAngle) * 0.06f + Main.rand.NextVector2Circular(2f, 2f);
                BloomPixelParticle pixel = new(pixelSpawnPosition, pixelVelocity, Color.Wheat, Color.Cyan * 0.5f, 60, Vector2.One * 1.32f, Projectile.Center);
                pixel.Spawn();
            }

            if (LifetimeRatio <= 0.87f)
            {
                Dust electricity = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(Projectile.width, Projectile.height) * Projectile.scale * 0.44f, 267);
                electricity.color = Main.hslToRgb(Main.rand.NextFloat(0.524f, 0.61f), 1f, 0.6f);
                electricity.scale = Main.rand.NextFloat(0.5f, 1f);
                electricity.velocity = Projectile.SafeDirectionTo(electricity.position).RotatedBy(Main.rand.NextFromList(-1f, 1f) * MathHelper.PiOver4) / electricity.scale * 2f + Projectile.velocity;
                electricity.noGravity = true;
            }
        }

        /// <summary>
        /// Creates a single electric spark around the sphere's edge.
        /// </summary>
        public void CreateElectricSpark()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int arcLifetime = Main.rand.Next(9, 16);
            Vector2 arcSpawnPosition = Projectile.Center + Main.rand.NextVector2Unit() * (Projectile.width * 0.41f - Main.rand.NextFloat(40f, 95f));
            Vector2 arcLength = Main.rand.NextVector2Unit() * Main.rand.NextFloat(40f, Projectile.width * 0.034f + 60f);

            if (Vector2.Dot(arcLength, Projectile.Center - arcSpawnPosition) > 0f)
                arcLength *= -1f;

            if (Main.rand.NextBool(3))
                arcLength *= 1.35f;
            if (Main.rand.NextBool(3))
                arcLength *= 1.35f;
            if (Main.rand.NextBool(5))
                arcLength *= 1.6f;
            if (Main.rand.NextBool(5))
                arcLength *= 1.6f;

            LumUtils.NewProjectileBetter(Projectile.GetSource_FromThis(), arcSpawnPosition, arcLength, ModContent.ProjectileType<SmallTeslaArc>(), 0, 0f, -1, arcLifetime, 0f);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) =>
            behindNPCs.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Texture2D bloom = MiscTexturesRegistry.BloomCircleSmall.Value;
            Main.spriteBatch.Draw(bloom, drawPosition, null, Projectile.GetAlpha(new(1f, 1f, 1f, 0f)) * 0.75f, 0f, bloom.Size() * 0.5f, Projectile.Size * Projectile.scale / bloom.Size() * 1.8f, 0, 0f);
            Main.spriteBatch.Draw(bloom, drawPosition, null, Projectile.GetAlpha(new(0.34f, 0.5f, 1f, 0f)) * 0.4f, 0f, bloom.Size() * 0.5f, Projectile.Size * Projectile.scale / bloom.Size() * 3f, 0, 0f);

            Main.spriteBatch.PrepareForShaders();
            DrawOrb(drawPosition);
            Main.spriteBatch.ResetToDefault();

            return false;
        }

        public void DrawOrb(Vector2 drawPosition)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.HadesExoEnergyOrbShader");
            shader.SetTexture(MiscTexturesRegistry.DendriticNoiseZoomedOut.Value, 1, SamplerState.PointWrap);
            shader.TrySetParameter("textureSize0", Projectile.Size);
            shader.TrySetParameter("pulseIntensity", 0.09f);
            shader.TrySetParameter("glowIntensity", 0.5f);
            shader.TrySetParameter("glowPower", 2.74f);
            shader.Apply();

            Texture2D pixel = MiscTexturesRegistry.Pixel.Value;
            Main.spriteBatch.Draw(pixel, drawPosition, null, Projectile.GetAlpha(new Color(0.05f, 0.3f, 0.9f)), 0f, pixel.Size() * 0.5f, Projectile.Size * Projectile.scale / pixel.Size() * 1.2f, 0, 0f);
        }
    }
}
