using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AresSwingingKatanas : ModProjectile, IProjOwnedByBoss<AresBody>, IExoMechProjectile
    {
        /// <summary>
        /// How long this slash has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[1];

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Electricity;

        public override void SetStaticDefaults() => On_Main.DrawNPCs += DrawBackBehindNPCs;

        public override void SetDefaults()
        {
            Projectile.width = 1120;
            Projectile.height = 232;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 72000;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.draedonExoMechPrime == -1)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Opacity = LumUtils.InverseLerp(0f, 10f, Time);

            NPC ares = Main.npc[CalamityGlobalNPC.draedonExoMechPrime];
            Projectile.Center = ares.Center;
            Projectile.rotation = ares.rotation * 0.04f;
            Projectile.velocity = ares.velocity.SafeNormalize(Vector2.Zero);
            Projectile.Opacity = ares.Opacity;

            DelegateMethods.v3_1 = Color.Crimson.ToVector3();
            Utils.PlotTileLine(Projectile.Center - Vector2.UnitY * 50f, Projectile.Center + Vector2.UnitY * 50f, Projectile.width, DelegateMethods.CastLight);

            for (int i = 0; i < 4; i++)
            {
                int sparkLifetime = Main.rand.Next(10, 48);
                Color sparkColor = Color.Lerp(Color.Crimson, Color.Wheat, Main.rand.NextFloat());
                Vector2 sparkSpawnPosition = Projectile.Center + Utils.Vector2FromElipse(Main.rand.NextVector2Unit(), Projectile.Size * Main.rand.NextFloat(0.9f, 0.92f));
                Vector2 sparkVelocity = Projectile.SafeDirectionTo(sparkSpawnPosition).RotatedBy(MathHelper.PiOver2 * Projectile.velocity.X.NonZeroSign()) * 20f + ares.velocity;

                BloomPixelParticle spark = new(sparkSpawnPosition, sparkVelocity, Color.White, sparkColor * 0.67f, sparkLifetime, Vector2.One * Main.rand.NextFloat(1f, 2.1f));
                spark.Spawn();
            }

            Player target = Main.player[ares.target];
            if (Main.netMode != NetmodeID.MultiplayerClient && !Projectile.WithinRange(target.Center, 500f) && Time % 15f == 0f)
            {
                Vector2 energySpawnPosition = Projectile.Center + Main.rand.NextVector2CircularEdge(100f, 100f);
                Vector2 energyVelocity = energySpawnPosition.SafeDirectionTo(target.Center).RotatedByRandom(0.32f) * 7f;
                LumUtils.NewProjectileBetter(Projectile.GetSource_FromThis(), energySpawnPosition, energyVelocity, ModContent.ProjectileType<AresCoreLaserSmall>(), AresBodyEternity.SmallLaserDamage, 0f);
            }

            Time++;
        }

        public void DrawSelf(bool clipTopHalf)
        {
            float swingDirection = Projectile.velocity.X.NonZeroSign();
            float animationPrecision = 6f;
            float animationSpeed = 8f;
            float animatedTime = (int)(Time * swingDirection / 60f * animationSpeed * animationPrecision) / animationPrecision;
            Vector2 size = Projectile.Size * 1.19f;
            Color lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());

            ManagedShader slashShader = ShaderManager.GetShader("FargowiltasCrossmod.EnergyKatanaSlashShader");
            slashShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 1, SamplerState.LinearWrap);
            slashShader.SetTexture(MiscTexturesRegistry.DendriticNoiseZoomedOut.Value, 2, SamplerState.LinearWrap);
            slashShader.TrySetParameter("clipTopHalf", clipTopHalf);
            slashShader.TrySetParameter("textureSize", size);
            slashShader.TrySetParameter("swingDirection", swingDirection);
            slashShader.TrySetParameter("metalColor", new Color(80, 89, 104).MultiplyRGBA(lightColor).ToVector4());
            slashShader.TrySetParameter("slashSourceDirection", (MathHelper.TwoPi * animatedTime).ToRotationVector2());
            slashShader.Apply();

            Texture2D pixel = MiscTexturesRegistry.Pixel.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(pixel, drawPosition, null, Projectile.GetAlpha(Color.Crimson), Projectile.rotation, pixel.Size() * 0.5f, size / pixel.Size(), 0, 0f);
        }

        private static void DrawBackBehindNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            if (!behindTiles)
            {
                var slashes = LumUtils.AllProjectilesByID(ModContent.ProjectileType<AresSwingingKatanas>());
                if (slashes.Any())
                {
                    Main.spriteBatch.PrepareForShaders();
                    foreach (Projectile slash in slashes)
                        slash.As<AresSwingingKatanas>().DrawSelf(false);

                    Main.spriteBatch.ResetToDefault();
                }
            }

            orig(self, behindTiles);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.PrepareForShaders();
            DrawSelf(true);
            Main.spriteBatch.ResetToDefault();

            return false;
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
