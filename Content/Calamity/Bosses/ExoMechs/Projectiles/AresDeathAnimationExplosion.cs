using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
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
    public class AresDeathAnimationExplosion : ModProjectile, IProjOwnedByBoss<AresBody>, IExoMechProjectile
    {
        /// <summary>
        /// How long this explosion has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[1];

        /// <summary>
        /// How long the explosion lasts.
        /// </summary>
        public static int Lifetime => AresBodyEternity.DeathAnimation_SilhouetteAppearDelay + AresBodyEternity.DeathAnimation_SilhouetteFadeInTime + AresBodyEternity.DeathAnimation_SilhouetteDissolveDelay + AresBodyEternity.DeathAnimation_SilhouetteDissolveTime + AresBodyEternity.DeathAnimation_DeathDelay;

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Thermal;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 32000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 900;
            Projectile.height = 900;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Lifetime;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Time++;
            Projectile.Opacity = Utilities.InverseLerp(1f, 0.73f, Time / Lifetime).Cubed();
            Projectile.scale = MathHelper.Clamp(Projectile.scale + 2f, 1f, 22f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.PrepareForShaders();

            float lifetimeRatio = MathF.Pow(Time / Lifetime, 0.32f);

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.GaussNukeExplosionShader");
            shader.SetTexture(MiscTexturesRegistry.DendriticNoiseZoomedOut.Value, 1, SamplerState.LinearWrap);
            shader.TrySetParameter("lifetimeRatio", lifetimeRatio);
            shader.TrySetParameter("textureSize0", Projectile.Size * Projectile.scale);
            shader.Apply();

            Texture2D pixel = MiscTexturesRegistry.Pixel.Value;
            Main.spriteBatch.Draw(pixel, drawPosition, null, Projectile.GetAlpha(Color.White), 0f, pixel.Size() * 0.5f, Projectile.Size * Projectile.scale / pixel.Size() * 1.2f, 0, 0f);

            Main.spriteBatch.ResetToDefault();

            return false;
        }
    }
}
