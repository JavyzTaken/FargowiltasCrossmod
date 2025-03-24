using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using Luminance.Assets;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExoOverloadDeathray : ModProjectile, IProjOwnedByBoss<AresBody>, IExoMechProjectile
    {
        /// <summary>
        /// The rotation of this deathray.
        /// </summary>
        public Quaternion Rotation
        {
            get;
            set;
        }

        /// <summary>
        /// How long this sphere has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[1];

        /// <summary>
        /// How long this laserbeam currently is.
        /// </summary>
        public ref float LaserbeamLength => ref Projectile.ai[2];

        /// <summary>
        /// How long the explosion lasts.
        /// </summary>
        public static int Lifetime => Utilities.SecondsToFrames(9f);

        /// <summary>
        /// The maximum length of this laserbeam.
        /// </summary>
        public static float MaxLaserbeamLength => 8000f;

        /// <summary>
        /// How many segments should be generated for the cylinder when subdiving its radial part.
        /// </summary>
        public const int CylinderWidthSegments = 12;

        /// <summary>
        /// How many segments should be generated for the cylinder when subdiving its height part.
        /// </summary>
        public const int CylinderHeightSegments = 2;

        /// <summary>
        /// The amount of cylinders used when subdividing the vertices for use by the bloom.
        /// </summary>
        public const int BloomSubdivisions = 40;

        public override string Texture => MiscTexturesRegistry.InvisiblePixelPath;

        public ExoMechDamageSource DamageType => ExoMechDamageSource.Plasma;

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 6000;

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;

            // This is done for more precision in the collision checks, due to the fact that the laser moves rather quickly.
            // Wouldn't want it to skip over the player's hitbox in a single update and do nothing.
            Projectile.MaxUpdates = 2;

            Projectile.timeLeft = Lifetime * Projectile.MaxUpdates;
            CooldownSlot = ImmunityCooldownID.Bosses;
            LaserbeamLength = 800f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Rotation.X);
            writer.Write(Rotation.Y);
            writer.Write(Rotation.Z);
            writer.Write(Rotation.W);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            float rotationX = reader.ReadSingle();
            float rotationY = reader.ReadSingle();
            float rotationZ = reader.ReadSingle();
            float rotationW = reader.ReadSingle();
            Rotation = new(rotationX, rotationY, rotationZ, rotationW);
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.draedonExoMechPrime == -1 || !Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active || !Main.npc[CalamityGlobalNPC.draedonExoMechPrime].TryGetDLCBehavior(out AresBodyEternity ares))
            {
                Projectile.Kill();
                return;
            }

            float rotationTime = Time / Projectile.MaxUpdates / 167f;
            float sine = MathF.Sin(MathHelper.TwoPi * rotationTime);
            float cosine = MathF.Cos(MathHelper.TwoPi * rotationTime);
            float upwardsInterpolant = Utilities.InverseLerp(30f, -30f, Time / Projectile.MaxUpdates - AresBodyEternity.BackgroundCoreLaserBeams_MissileShootDelay);
            float zRotation = MathHelper.SmoothStep(cosine * 0.1f, -MathHelper.PiOver2, upwardsInterpolant);
            var quaternionRotation = Matrix.CreateRotationZ(zRotation) * Matrix.CreateRotationY(sine * (1f - upwardsInterpolant) * 1.6f + MathHelper.PiOver2);
            Rotation = Quaternion.CreateFromRotationMatrix(quaternionRotation);

            Projectile.Center = ares.CorePosition;
            LaserbeamLength = MathHelper.Clamp(LaserbeamLength + 98f, 0f, MaxLaserbeamLength);

            Projectile.Opacity = MathF.Pow(Utilities.InverseLerp(0f, 11f, Time), 0.63f);

            if (Projectile.timeLeft <= 60)
                Projectile.scale *= 0.9f;

            Time++;
        }

        public void GetBloomVerticesAndIndices(Color baseColor, Vector3 start, Vector3 end, out VertexPosition2DColorTexture[] leftVertices, out VertexPosition2DColorTexture[] rightVertices, out int[] indices)
        {
            int numVertices = (CylinderWidthSegments + 1) * (CylinderHeightSegments + 1);
            int numIndices = CylinderWidthSegments * CylinderHeightSegments * 6;

            leftVertices = new VertexPosition2DColorTexture[numVertices * BloomSubdivisions];
            rightVertices = new VertexPosition2DColorTexture[leftVertices.Length];
            indices = new int[numIndices * BloomSubdivisions * 6];

            for (int i = 0; i < BloomSubdivisions; i++)
            {
                float subdivisionInterpolant = i / (float)(BloomSubdivisions - 1f);
                float bloomWidthFactor = subdivisionInterpolant * 1.3f + 1f;
                Color bloomColor = baseColor * MathHelper.SmoothStep(0.05f, 0.005f, MathF.Sqrt(subdivisionInterpolant));
                GetVerticesAndIndices(bloomWidthFactor, bloomColor, start, end, MathHelper.Pi, out VertexPosition2DColorTexture[] localRightVertices, out int[] localIndices);
                GetVerticesAndIndices(bloomWidthFactor, bloomColor, start, end, 0f, out VertexPosition2DColorTexture[] localLeftVertices, out _);

                int startingIndex = indices.Max();
                for (int j = 0; j < localIndices.Length; j++)
                    indices[j + i * numIndices] = localIndices[j] + startingIndex;
                for (int j = 0; j < localLeftVertices.Length; j++)
                {
                    leftVertices[j + i * numVertices] = localLeftVertices[j];
                    rightVertices[j + i * numVertices] = localRightVertices[j];
                }
            }
        }

        /// <summary>
        /// Collects vertices and indices for rendering the laser cylinder.
        /// </summary>
        /// <param name="widthFactor">The width factor of the cylinder.</param>
        /// <param name="baseColor">The color of the cylinder.</param>
        /// <param name="start">The starting point of the deathray, in 3D space.</param>
        /// <param name="end">The ending point of the deathray, in 3D space.</param>
        /// <param name="cylinderOffsetAngle">The offset angle of the vertices on the cylinder.</param>
        /// <param name="vertices">The resulting vertices.</param>
        /// <param name="indices">The resulting indices.</param>
        public void GetVerticesAndIndices(float widthFactor, Color baseColor, Vector3 start, Vector3 end, float cylinderOffsetAngle, out VertexPosition2DColorTexture[] vertices, out int[] indices)
        {
            int numVertices = (CylinderWidthSegments + 1) * (CylinderHeightSegments + 1);
            int numIndices = CylinderWidthSegments * CylinderHeightSegments * 6;

            vertices = new VertexPosition2DColorTexture[numVertices];
            indices = new int[numIndices];

            float widthStep = 1f / CylinderWidthSegments;
            float heightStep = 1f / CylinderHeightSegments;

            // Create vertices.
            for (int i = 0; i <= CylinderHeightSegments; i++)
            {
                for (int j = 0; j <= CylinderWidthSegments; j++)
                {
                    float width = Utils.Remap(i * heightStep, 0f, 0.5f, 3f, Projectile.width * Projectile.scale) * widthFactor;
                    float angle = MathHelper.Pi * j * widthStep + cylinderOffsetAngle;
                    Vector3 orthogonalOffset = Vector3.Transform(new Vector3(0f, MathF.Sin(angle), MathF.Cos(angle)), Rotation) * width;
                    Vector3 cylinderPoint = Vector3.Lerp(start, end, i * heightStep) + orthogonalOffset;
                    vertices[i * (CylinderWidthSegments + 1) + j] = new(new(cylinderPoint.X, cylinderPoint.Y), baseColor, new Vector2(i * heightStep, j * widthStep), 1f);
                }
            }

            // Create indices.
            for (int i = 0; i < CylinderHeightSegments; i++)
            {
                for (int j = 0; j < CylinderWidthSegments; j++)
                {
                    int upperLeft = i * (CylinderWidthSegments + 1) + j;
                    int upperRight = upperLeft + 1;
                    int lowerLeft = upperLeft + (CylinderWidthSegments + 1);
                    int lowerRight = lowerLeft + 1;

                    indices[(i * CylinderWidthSegments + j) * 6] = upperLeft;
                    indices[(i * CylinderWidthSegments + j) * 6 + 1] = lowerRight;
                    indices[(i * CylinderWidthSegments + j) * 6 + 2] = lowerLeft;

                    indices[(i * CylinderWidthSegments + j) * 6 + 3] = upperLeft;
                    indices[(i * CylinderWidthSegments + j) * 6 + 4] = upperRight;
                    indices[(i * CylinderWidthSegments + j) * 6 + 5] = lowerRight;
                }
            }
        }

        /// <summary>
        /// Renders the deathray.
        /// </summary>
        /// <param name="start">The starting point of the deathray, in 3D space.</param>
        /// <param name="end">The ending point of the deathray, in 3D space.</param>
        /// <param name="baseColor">The color of the cylinder.</param>
        /// <param name="widthFactor">The width factor of the cylinder.</param>
        public void RenderLaser(Vector3 start, Vector3 end, Color baseColor, float widthFactor)
        {
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            GetVerticesAndIndices(widthFactor, baseColor, start, end, MathHelper.Pi, out VertexPosition2DColorTexture[] rightVertices, out int[] indices);
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, rightVertices, 0, rightVertices.Length, indices, 0, indices.Length / 3);

            GetVerticesAndIndices(widthFactor, baseColor, start, end, 0f, out VertexPosition2DColorTexture[] leftVertices, out _);
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, leftVertices, 0, leftVertices.Length, indices, 0, indices.Length / 3);
        }

        /// <summary>
        /// Renders bloom for the deathray.
        /// </summary>
        /// <param name="start">The starting point of the deathray, in 3D space.</param>
        /// <param name="end">The ending point of the deathray, in 3D space.</param>
        /// <param name="projection">The matrix responsible for manipulating primitive vertices.</param>
        public void RenderBloom(Vector3 start, Vector3 end, Matrix projection)
        {
            Color bloomColor = Color.White with { A = 0 };

            ManagedShader bloomShader = ShaderManager.GetShader("FargowiltasCrossmod.CylinderPrimitiveBloomShader");
            bloomShader.TrySetParameter("innerGlowIntensity", 0.45f);
            bloomShader.TrySetParameter("uWorldViewProjection", projection);
            bloomShader.Apply();

            GetBloomVerticesAndIndices(bloomColor, start, end, out VertexPosition2DColorTexture[] leftVertices, out VertexPosition2DColorTexture[] rightVertices, out int[] indices);

            GraphicsDevice gd = Main.instance.GraphicsDevice;
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, rightVertices, 0, rightVertices.Length, indices, 0, indices.Length / 3);
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, leftVertices, 0, leftVertices.Length, indices, 0, indices.Length / 3);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float hueShift = Utilities.Cos01(Main.GlobalTimeWrappedHourly * 9f) * -0.09f;
            float bloomScaleFactor = MathHelper.Lerp(0.9f, 1.1f, Utilities.Cos01(Main.GlobalTimeWrappedHourly * 22f)) * Projectile.Opacity;
            Texture2D bloom = MiscTexturesRegistry.BloomCircleSmall.Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(bloom, drawPosition, null, (Color.White with { A = 0 }) * Projectile.Opacity * 0.5f, 0f, bloom.Size() * 0.5f, bloomScaleFactor * 0.24f, 0, 0f);
            Main.spriteBatch.Draw(bloom, drawPosition, null, (Color.Wheat with { A = 0 }) * Projectile.Opacity * 0.5f, 0f, bloom.Size() * 0.5f, bloomScaleFactor * 0.5f, 0, 0f);
            Main.spriteBatch.Draw(bloom, drawPosition, null, (Color.HotPink.HueShift(hueShift) with { A = 0 }) * Projectile.Opacity * 0.25f, 0f, bloom.Size() * 0.5f, bloomScaleFactor * 1.1f, 0, 0f);
            Main.spriteBatch.Draw(bloom, drawPosition, null, (Color.Orange.HueShift(hueShift) with { A = 0 }) * Projectile.Opacity * 0.2f, 0f, bloom.Size() * 0.5f, bloomScaleFactor * 1.97f, 0, 0f);

            Vector3 start = new(Projectile.Center - Main.screenPosition, 0f);
            Vector3 end = start + Vector3.Transform(Vector3.UnitX, Rotation) * LaserbeamLength;
            end.Z /= LaserbeamLength;

            GraphicsDevice gd = Main.instance.GraphicsDevice;
            gd.RasterizerState = RasterizerState.CullNone;

            int width = Main.screenWidth;
            int height = Main.screenHeight;
            Utilities.CalculatePrimitiveMatrices(width, height, out Matrix view, out Matrix projection);
            Matrix overallProjection = view * projection;

            RenderBloom(start, end, overallProjection);

            ManagedShader overloadShader = ShaderManager.GetShader("FargowiltasCrossmod.ExoOverloadDeathrayShader");
            overloadShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 1, SamplerState.LinearWrap);
            overloadShader.SetTexture(MiscTexturesRegistry.TurbulentNoise.Value, 2, SamplerState.LinearWrap);
            overloadShader.TrySetParameter("uWorldViewProjection", overallProjection);
            overloadShader.TrySetParameter("scrollColorA", Color.White);
            overloadShader.TrySetParameter("scrollColorB", Color.White);
            overloadShader.TrySetParameter("baseColor", Color.White);
            overloadShader.Apply();

            RenderLaser(start, end, Color.White, 1f);
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector3 start = new(Projectile.Center, 0f);
            Vector3 end = start + Vector3.Transform(Vector3.UnitX, Rotation) * LaserbeamLength;
            end.Z /= LaserbeamLength;

            Vector3 rayDirection = Vector3.Normalize(end - start);
            Vector3 boxMin = new(targetHitbox.TopLeft(), -1f);
            Vector3 boxMax = new(targetHitbox.BottomRight(), 1f);

            Vector3 tMin = (boxMin - start) / rayDirection;
            Vector3 tMax = (boxMax - start) / rayDirection;
            Vector3 t1 = Vector3.Min(tMin, tMax);

            float tNear = MathF.Max(MathF.Max(t1.X, t1.Y), t1.Z);
            Vector3 targetCenter = new(targetHitbox.Center(), 0f);
            Vector3 endPoint = start + rayDirection * tNear;
            Vector3 directionToTarget = Vector3.Normalize(targetCenter - start);
            float distanceToProjection = Vector3.Distance(endPoint, targetCenter);
            return distanceToProjection <= Projectile.width * Projectile.scale && Vector3.Dot(directionToTarget, rayDirection) >= 0.96f;
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
