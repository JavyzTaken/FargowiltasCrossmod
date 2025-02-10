using FargowiltasCrossmod.Assets.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

public class TastyFuelContainer : ModProjectile
{
    /// <summary>
    /// The glowmask asset associated with this fuel container.
    /// </summary>
    public static Asset<Texture2D> GlowmaskAsset
    {
        get;
        private set;
    }

    /// <summary>
    /// How long this fuel container has existed for, in frames.
    /// </summary>
    public ref float Time => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 7;

        if (Main.netMode != NetmodeID.MultiplayerClient)
            GlowmaskAsset = ModContent.Request<Texture2D>($"{Texture}Glowmask");
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 900;
        Projectile.scale = Main.rand?.NextFloat(1f, 1.35f) ?? 1f;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        UpdateVelocity(ref Projectile.velocity);

        // Wait before colliding with tiles.
        Projectile.tileCollide = Time >= 60f && Projectile.velocity.Y > 0f;

        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

        Time++;
    }

    /// <summary>
    /// Performs velocity updates for a given fuel container, making it adhere to gravity and gradually accumulate horizontal drag.
    /// </summary>
    /// <param name="velocity">The velocity to modify.</param>
    public static void UpdateVelocity(ref Vector2 velocity)
    {
        velocity.X *= 0.984f;
        if (velocity.Y < 24f)
            velocity.Y += 0.56f;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);

        // Emit a radial spray of short-lived gas particles that compose a cloud.
        for (int i = 0; i < 80; i++)
        {
            float gasScale = Main.rand.NextFloat(1f, 1.4f);
            float gasOpacity = Main.rand.NextFloat(0.15f, 0.2f);
            Color gasColor = Color.Lerp(new Color(191, 199, 9), new Color(124, 239, 87), Main.rand.NextFloat()) * gasOpacity;
            gasColor.A /= 2;

            Vector2 gasVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.4f, 15f);
            NuclearGasParticle gas = new NuclearGasParticle(Projectile.Center, gasVelocity, gasColor, gasScale, Main.rand.Next(40, 90));
            gas.Spawn();
        }

        // Emit bits of slightly lime-tinted glass shards, since this is a fuel container that has been shattered.
        for (int i = 0; i < 24; i++)
        {
            Vector2 glassSpawnPosition = Projectile.Center + Main.rand.NextVector2Circular(10f, 10f);
            Vector2 glassVelocity = Main.rand.NextVector2Circular(20f, 20f);
            Dust glass = Dust.NewDustPerfect(glassSpawnPosition, DustID.Glass, glassVelocity);
            glass.color = Color.Lerp(Color.White, Color.YellowGreen, Main.rand.NextFloat(0.25f));
            glass.scale = Main.rand.NextFloat(1f, 1.6f);
            glass.noGravity = glass.velocity.Length() >= 13.5f;
        }
    }

    public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.65f) * Projectile.Opacity;

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            float opacity = 1f - i / (float)Projectile.oldPos.Length;
            Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor) * opacity, Projectile.oldRot[i], texture.Size() * 0.5f, Projectile.scale, 0, 0f);
        }

        Texture2D glowmask = GlowmaskAsset.Value;
        Main.spriteBatch.Draw(glowmask, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, glowmask.Size() * 0.5f, Projectile.scale, 0, 0f);

        return false;
    }
}
