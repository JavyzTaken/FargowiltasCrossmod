using CalamityMod;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

public class FallingVomitGore : ModProjectile
{
    /// <summary>
    /// How long this gore has existed for, in frames.
    /// </summary>
    public ref float Time => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 360;
        Projectile.scale = Main.rand?.NextFloat(0.85f, 1.4f) ?? 1f;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        Projectile.velocity.X *= 0.99f;

        // Wait before permitting tile collisions.
        Projectile.tileCollide = Time >= 75f;

        if (Main.rand.NextBool(3) && Projectile.velocity.Length() >= 9f)
        {
            Vector2 dustSpawnPosition = Projectile.Center + Main.rand.NextVector2CircularEdge(Projectile.width, Projectile.height) * 0.5f;
            Dust nuclear = Dust.NewDustPerfect(dustSpawnPosition, 261, Main.rand.NextVector2Circular(3f, 3f));
            nuclear.color = new Color(141, 255, 9);
            nuclear.scale = Main.rand.NextFloat(0.6f, 1f);
            nuclear.noGravity = true;
        }

        // Become bouyant in water. Otherwise fall down.
        if (Collision.WetCollision(Projectile.TopLeft, Projectile.width, Projectile.height))
        {
            Projectile.velocity *= 0.98f;
            Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, -2.6f, 0.1f);
        }
        else
            Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y + 0.4f, -36f, 9.5f);

        Projectile.rotation += MathF.Abs(Projectile.velocity.Y) * Projectile.velocity.X.NonZeroSign() * 0.03f;

        if (Projectile.timeLeft < 60)
        {
            Projectile.velocity.X *= 0.9f;
            Projectile.scale *= 0.971f;

            int newSize = (int)(Projectile.scale * 24f);
            Projectile.Resize(newSize, newSize);
        }

        Time++;
    }

    public override bool? CanDamage() => Time >= 75f;

    public override bool PreDraw(ref Color lightColor)
    {
        Asset<Texture2D> t = TextureAssets.Projectile[Type];
        Main.spriteBatch.SetBlendState(BlendState.Additive);
        for (int j = 0; j < 12; j++)
        {
            Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 3f * Projectile.scale;
            Color glowColor = Color.YellowGreen * 0.9f;


            Main.EntitySpriteDraw(t.Value, Projectile.Center + afterimageOffset - Main.screenPosition, null, Projectile.GetAlpha(glowColor), Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
        }
        Main.spriteBatch.ResetToDefault();
        Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
        return false;
    }
}
