using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
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
    public class GaussNuke : ModProjectile, IExoMechProjectile, IProjOwnedByBoss<AresBody>
    {
        public bool SetActiveFalseInsteadOfKill => true;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.BluntForceTrauma;

        /// <summary>
        /// The diameter of the explosion that will result from this nuke.
        /// </summary>
        public ref float ExplosionDiameter => ref Projectile.ai[0];

        /// <summary>
        /// The ideal fly speed of this nuke.
        /// </summary>
        public static float IdealFlySpeed => 7f;

        /// <summary>
        /// The acceleration interpolant of this nuke.
        /// </summary>
        public static float FlyAccelerationInterpolant => 0.13f;

        /// <summary>
        /// The sound this nuke plays when it explodes.
        /// </summary>
        public static readonly SoundStyle ExplodeSound = new("FargowiltasCrossmod/Assets/Sounds/ExoMechs/Ares/LiveNuclearReaction");

        /// <summary>
        /// How long this nuke should exist before exploding, in frames.
        /// </summary>
        public static int Lifetime => AresBodyEternity.NukeAoEAndPlasmaBlasts_NukeExplosionDelay;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 20000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.Calamity().DealsDefenseDamage = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 6 % Main.projFrames[Projectile.type];
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
            float flyAccelerationInterpolant = LumUtils.InverseLerp(100f, 210f, Projectile.Distance(target.Center));
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(target.Center) * 7f, flyAccelerationInterpolant * FlyAccelerationInterpolant);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion();
            DrawAreaOfEffectTelegraph();
            Main.spriteBatch.ExitShaderRegion();

            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/AresGaussNukeProjectileGlow").Value;
            Utilities.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            Utilities.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], Color.White, 1, texture: glowmask);

            return false;
        }

        public void DrawAreaOfEffectTelegraph()
        {
            float lifetimeRatio = 1f - Projectile.timeLeft / (float)Lifetime;
            float opacity = Utilities.Saturate(lifetimeRatio * 8f) * 0.36f;
            float maxFlashIntensity = Utilities.InverseLerp(0.25f, 0.75f, lifetimeRatio);
            float flashColorInterpolant = Utilities.Cos01(Main.GlobalTimeWrappedHourly * 10f).Squared() * maxFlashIntensity;
            Color innerColor = Color.Lerp(Color.Goldenrod, Color.Gold, MathF.Pow(Utilities.Sin01(Main.GlobalTimeWrappedHourly), 3f) * 0.85f);
            Color edgeColor = Color.Lerp(Color.Yellow, Color.Wheat, 0.6f);

            innerColor = Color.Lerp(innerColor, Color.Crimson, MathF.Pow(flashColorInterpolant, 0.7f));
            edgeColor = Color.Lerp(edgeColor, Color.Red, flashColorInterpolant);

            var aoeShader = GameShaders.Misc["CalamityMod:CircularAoETelegraph"];
            aoeShader.UseOpacity(opacity);
            aoeShader.UseColor(innerColor);
            aoeShader.UseSecondaryColor(edgeColor);
            aoeShader.UseSaturation(lifetimeRatio);
            aoeShader.Apply();

            float explosionDiameter = ExplosionDiameter * MathF.Pow(Utilities.InverseLerp(0f, 0.25f, lifetimeRatio), 1.6f) * 0.94f;
            Texture2D pixel = MiscTexturesRegistry.InvisiblePixel.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(pixel, drawPosition, null, Color.White, 0, pixel.Size() * 0.5f, Vector2.One * explosionDiameter / pixel.Size(), 0, 0);
        }

        // This IS a heavy chunk of metal, and as such it should do damage as it's flying forward, but otherwise it should just fly without causing harm.
        // It'd be rather silly for a nuke that's just sitting in place to do damage.
        public override bool? CanDamage() => Projectile.velocity.Length() >= 25f;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Utilities.CircularHitboxCollision(Projectile.Center, 45f, targetHitbox);

        public override void OnKill(int timeLeft)
        {
            ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 15f, intensityTaperStartDistance: 3000f, intensityTaperEndDistance: 6000f);
            SoundEngine.PlaySound(ExplodeSound, Vector2.Lerp(Projectile.Center, Main.LocalPlayer.Center, 0.9f)).WithVolumeBoost(2.75f);

            // NOTE -- There used to be gores spawned here as well, but that effect was removed on account of the fact that realistically the outer shell of the
            // nuke would be obliterated instantly upon its detonation.
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Utilities.NewProjectileBetter(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GaussNukeBoom>(), AresBodyEternity.NukeExplosionDamage, 0f, -1, ExplosionDiameter);
        }
    }
}
