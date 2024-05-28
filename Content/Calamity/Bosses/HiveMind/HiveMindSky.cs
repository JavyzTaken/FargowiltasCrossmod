using FargowiltasCrossmod.Content.Common.Bosses.Mutant;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.NPCs;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core;
using Luminance.Core.Graphics;
using CalamityMod.Particles;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HiveMindSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private float lifeIntensity = 0f;
        private float specialColorLerp = 0f;
        private Color? specialColor = null;
        public override void Update(GameTime gameTime)
        {
            const float increment = 0.003f;

            bool useSpecialColor = false;
            if (CalamityGlobalNPC.hiveMind.IsWithinBounds(Main.maxNPCs) && Main.npc[CalamityGlobalNPC.hiveMind] is NPC hiveMind && hiveMind.TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>() && HMEternity.Subphase(hiveMind) > 2)
            {
                intensity += increment;
                lifeIntensity = hiveMind.GetLifePercent() / HMEternity.Subphase3HP;

                void ChangeColorIfDefault(Color color) //waits for bg to return to default first
                {
                    if (specialColor == null)
                        specialColor = color;
                    if (specialColor != null && specialColor == color)
                        useSpecialColor = true;
                }

                if (intensity > 1f)
                    intensity = 1f;
            }
            else
            {
                lifeIntensity -= increment;
                if (lifeIntensity < 0f)
                    lifeIntensity = 0f;

                specialColorLerp -= increment * 2;
                if (specialColorLerp < 0)
                    specialColorLerp = 0;

                intensity -= increment;
                if (intensity < 0f)
                {
                    intensity = 0f;
                    lifeIntensity = 0f;
                    specialColorLerp = 0f;
                    specialColor = null;
                    Deactivate();
                    return;
                }
            }

            if (useSpecialColor)
            {
                specialColorLerp += increment * 2;
                if (specialColorLerp > 1)
                    specialColorLerp = 1;
            }
            else
            {
                specialColorLerp -= increment * 2;
                if (specialColorLerp < 0)
                {
                    specialColorLerp = 0;
                    specialColor = null;
                }
            }
        }

        private Color ColorToUse(ref float opacity)
        {
            opacity = intensity * 0.75f;
            return HiveMindPulse.GlowColor;
        }

        public struct DarkSpot
        {
            public DarkSpot(Vector2 screenPosition, int timeLeft)
            {
                ScreenPosition = screenPosition;
                TimeLeft = timeLeft;
                Variant = Main.rand.Next(3);
                Rotation = Main.rand.NextFloat(MathF.Tau);
                RotationSpeed = Main.rand.NextFloat(0.05f, 0.1f) * (Main.rand.NextBool() ? 1 : -1);
            }
            public Vector2 ScreenPosition;
            public int TimeLeft;
            public int Variant;
            public float Rotation;
            public float RotationSpeed;
            public const int MaxTime = 110;
        }
        public List<DarkSpot> DarkSpots = [];
        int delay = 0;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                float opacity = 0f;
                Color color = ColorToUse(ref opacity);

                spriteBatch.Draw(ModContent.Request<Texture2D>("FargowiltasSouls/Content/Sky/MutantSky", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), color * opacity);

                Texture2D vignette = ModContent.Request<Texture2D>("FargowiltasCrossmod/Assets/Vignette").Value;
                Vector2 vignetteScale = Vector2.UnitX *  Main.screenWidth / vignette.Width + Vector2.UnitY * Main.screenHeight / vignette.Height;
                Vector2 screenCenter = Vector2.UnitX * Main.screenWidth / 2 + Vector2.UnitY * Main.screenHeight / 2;

                int delayTime = (int)(lifeIntensity * 25) + 25;
                if (++delay > delayTime)
                {
                    int maxOffset = 50;
                    bool side = Main.rand.NextBool();
                    float xPos;
                    float yPos;
                    float xOffset = Main.rand.NextFloat(-20, maxOffset);
                    float yOffset = Main.rand.NextFloat(-20, maxOffset);
                    xOffset = MathF.Pow(xOffset / maxOffset, 2) * maxOffset;
                    yOffset = MathF.Pow(yOffset / maxOffset, 2) * maxOffset;
                    if (side)
                    {
                        xPos = Main.rand.NextBool() ? 0 + xOffset : Main.screenWidth - xOffset;
                        yPos = Main.rand.NextFloat(Main.screenHeight);
                    }
                    else
                    {
                        xPos = Main.rand.NextFloat(Main.screenWidth);
                        yPos = Main.rand.NextBool() ? 0 + yOffset : Main.screenHeight - yOffset;
                    }

                    DarkSpots.Add(new(Vector2.UnitX * xPos + Vector2.UnitY * yPos, DarkSpot.MaxTime));
                    delay = 0;
                }

                for (int i = 0; i < DarkSpots.Count; i++)
                {
                    var DarkSpot = DarkSpots[i];
                    DarkSpot.TimeLeft--;
                    Vector2 dir = Vector2.Normalize(DarkSpot.ScreenPosition - screenCenter);
                    DarkSpot.ScreenPosition -= dir * 1.25f;
                    DarkSpot.Rotation += DarkSpot.RotationSpeed;


                    DarkSpots[i] = DarkSpot;
                    if (DarkSpot.TimeLeft < 0)
                    {
                        DarkSpots.RemoveAt(i);
                    }
                    float progress = (float)DarkSpot.TimeLeft / DarkSpot.MaxTime;
                    float spotOpacity = MathF.Sin(progress * MathF.PI);
                    float spotScale = 4 * (progress);
                    float opacityMult = 1f;
                    Color drawColor = ColorToUse(ref opacityMult);
                    spotOpacity *= opacityMult;
                    Color bloomColor = Color.Black;
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Particles/MediumMist").Value;
                    int yHeight = texture.Height / 3;
                    Rectangle frame = new(0, yHeight * DarkSpot.Variant, texture.Width, yHeight);

                    spriteBatch.Draw(texture, DarkSpot.ScreenPosition, frame, drawColor with { A = 0 } * spotOpacity, DarkSpot.Rotation, frame.Size() / 2, spotScale, SpriteEffects.None, 0);

                    spriteBatch.Draw(texture, DarkSpot.ScreenPosition, frame, bloomColor with { A = 0 } * 0.7f * spotOpacity, DarkSpot.Rotation, frame.Size() / 2, spotScale * 0.66f, SpriteEffects.None, 0);
                }
                spriteBatch.Draw(vignette, screenCenter, null, Color.Black * opacity, 0f, vignette.Size() / 2, vignetteScale, SpriteEffects.None, 0);
            }
        }

        public override float GetCloudAlpha()
        {
            return 1f - intensity;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive;
        }

        public override Color OnTileColor(Color inColor)
        {
            float dummy = 0f;
            Color skyColor = Color.Lerp(Color.White, ColorToUse(ref dummy), 0.5f);
            return Color.Lerp(skyColor, inColor, 1f - intensity);
        }
    }
}
