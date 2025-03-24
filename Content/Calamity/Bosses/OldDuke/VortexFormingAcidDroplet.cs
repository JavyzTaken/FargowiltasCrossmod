using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

public class VortexFormingAcidDroplet : ModProjectile
{
    /// <summary>
    /// How long this droplet has existed for, in frames.
    /// </summary>
    public ref float Time => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
    }

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
        Projectile.scale = Main.rand?.NextFloat(0.5f, 2f) ?? 1f;
        Projectile.hide = true;
        Projectile.MaxUpdates = 2;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        int vortexID = ModContent.ProjectileType<NuclearVortex>();
        Projectile? vortex = null;
        foreach (Projectile projectile in Main.ActiveProjectiles)
        {
            if (projectile.type == vortexID)
            {
                vortex = projectile;
                break;
            }
        }

        if (vortex is null)
        {
            Projectile.Kill();
            return;
        }

        Vector2 flyDestination = vortex.Center;
        float flyInterpolant = Utils.Remap(Time, 60f, 240f, 0.01f, 0.045f);
        float acceleration = Utils.Remap(Time, 0f, 120f, 0.5f, 1.5f);
        float bearing = MathF.Cos(MathHelper.TwoPi * Time / 30f + Projectile.identity) * LumUtils.InverseLerp(90f, 30f, Time) * 0.9f;
        Projectile.Center = Vector2.Lerp(Projectile.Center, flyDestination, flyInterpolant);
        Projectile.velocity += Projectile.SafeDirectionTo(flyDestination).RotatedBy(bearing) * acceleration;
        if (Projectile.velocity.Length() > 40f)
            Projectile.velocity *= 0.7f;

        Projectile.rotation = (Projectile.position - Projectile.oldPosition).ToRotation() + MathHelper.PiOver2;
        Projectile.Opacity = LumUtils.InverseLerp(0f, 45f, Projectile.timeLeft);

        if (Projectile.WithinRange(flyDestination, 60f))
            Projectile.Kill();

        Time++;
    }

    public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindProjectiles.Add(index);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            float opacity = 1f - i / (float)Projectile.oldPos.Length;
            Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(Color.Wheat) * opacity, Projectile.oldRot[i], texture.Size() * 0.5f, Projectile.scale, 0, 0f);
        }

        return false;
    }
}
