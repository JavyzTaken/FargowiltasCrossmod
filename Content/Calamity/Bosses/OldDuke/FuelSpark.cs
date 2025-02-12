using FargowiltasCrossmod.Core;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

[JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
[ExtendsFromMod(ModCompatibility.Calamity.Name)]
public class FuelSpark : ModProjectile, IPixelatedPrimitiveRenderer
{
    /// <summary>
    /// The width factor to be used in the <see cref="ArcWidthFunction(float)"/>.
    /// </summary>
    public float WidthFactor;

    /// <summary>
    /// The color to be used in the <see cref="ArcColorFunction(float)"/>.
    /// </summary>
    public Color ArcColor;

    /// <summary>
    /// The set of all points used to draw the composite arc.
    /// </summary>
    /// 
    /// <remarks>
    /// This array is not synced, but since it's a purely visual effect it shouldn't need to be.
    /// </remarks>
    public Vector2[] ArcPoints;

    /// <summary>
    /// How long this spark should exist, in frames.
    /// </summary>
    public ref float Lifetime => ref Projectile.ai[0];

    /// <summary>
    /// How long this spark has existed, in frames.
    /// </summary>
    public ref float Time => ref Projectile.ai[2];

    public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 12;
    }

    public override void SetDefaults()
    {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.hostile = true;
        Projectile.timeLeft = 300;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void SendExtraAI(BinaryWriter writer) => writer.Write(WidthFactor);

    public override void ReceiveExtraAI(BinaryReader reader) => WidthFactor = reader.ReadSingle();

    public void GenerateArcPoints()
    {
        ArcPoints = new Vector2[25];

        Vector2 start = Projectile.Center;
        Vector2 lengthForPerpendicular = Projectile.velocity.ClampLength(0f, 640f);
        Vector2 end = start + Projectile.velocity * Main.rand.NextFloat(0.67f, 1.2f) + Main.rand.NextVector2Circular(30f, 30f);
        Vector2 farFront = start - lengthForPerpendicular.RotatedByRandom(3.1f) * Main.rand.NextFloat(0.26f, 0.8f);
        Vector2 farEnd = end + lengthForPerpendicular.RotatedByRandom(3.1f) * 4f;
        for (int i = 0; i < ArcPoints.Length; i++)
        {
            ArcPoints[i] = Vector2.CatmullRom(farFront, start, end, farEnd, i / (float)(ArcPoints.Length - 1f));

            if (Main.rand.NextBool(9))
                ArcPoints[i] += Main.rand.NextVector2CircularEdge(10f, 10f);
        }
    }

    public override void AI()
    {
        if (WidthFactor <= 0f)
            WidthFactor = 1f;

        if (ArcPoints is null)
            GenerateArcPoints();
        else
        {
            for (int i = 0; i < ArcPoints.Length; i += 2)
            {
                float trailCompletionRatio = i / (float)(ArcPoints.Length - 1f);
                float arcProtrudeAngleOffset = Main.rand.NextGaussian(0.63f) + MathHelper.PiOver2;
                float arcProtrudeDistance = Main.rand.NextGaussian(4.6f);
                if (Main.rand.NextBool(100))
                    arcProtrudeDistance *= 7f;
                Vector2 arcOffset = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(arcProtrudeAngleOffset) * arcProtrudeDistance;

                ArcPoints[i] += arcOffset * Utilities.Convert01To010(trailCompletionRatio);
            }
        }

        Time++;
        if (Time >= Lifetime)
            Projectile.Kill();
    }

    public float ArcWidthFunction(float completionRatio)
    {
        float lifetimeRatio = Time / Lifetime;
        float lifetimeSquish = Utilities.InverseLerpBump(0.1f, 0.35f, 0.75f, 1f, lifetimeRatio);
        return MathHelper.Lerp(1f, 3f, Utilities.Convert01To010(completionRatio)) * lifetimeSquish * WidthFactor;
    }

    public Color ArcColorFunction(float completionRatio) => Projectile.GetAlpha(ArcColor);

    public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
    {
        if (ArcPoints is null)
            return;

        float lifetimeRatio = Time / Lifetime;
        ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.FuelSparkShader");
        shader.TrySetParameter("lifetimeRatio", lifetimeRatio);
        shader.TrySetParameter("erasureThreshold", 0.7f);
        shader.SetTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons2"), 1, SamplerState.LinearWrap);
        shader.Apply();

        PrimitiveSettings settings = new(ArcWidthFunction, ArcColorFunction, Smoothen: false, Pixelate: true, Shader: shader);

        float colorInterpolant = Projectile.identity / 19f % 1f;
        ArcColor = Color.Lerp(new Color(0.98f, 1f, 0.3f), new Color(0.35f, 1f, 0.3f), colorInterpolant);

        PrimitiveRenderer.RenderTrail(ArcPoints, settings, 22);
    }

    public override bool ShouldUpdatePosition() => false;
}