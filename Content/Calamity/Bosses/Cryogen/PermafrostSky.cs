using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PermafrostBackgroundScene : ModSceneEffect
    {
        public static bool AnyP2Permafrost => Main.npc.Any(p => p.TypeAlive<PermafrostBoss>() && p.As<PermafrostBoss>().Phase >= 2);
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player)
        {
            PermafrostSky.UpdateDrawEligibility();
            bool result = AnyP2Permafrost || PermafrostSky.ShouldDrawRegularly;
            PermafrostSky.UpdateDrawEligibility();
            return result;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (SkyManager.Instance["FargowiltasCrossmod:Permafrost"] != null && isActive != SkyManager.Instance["FargowiltasCrossmod:Permafrost"].IsActive())
            {
                if (isActive)
                    SkyManager.Instance.Activate("FargowiltasCrossmod:Permafrost", player.Center);
                else
                    SkyManager.Instance.Deactivate("FargowiltasCrossmod:Permafrost");
            }
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PermafrostSky : CustomSky
    {
        public struct PermafrostAurora
        {
            public float Depth;
            public float ColorHueOffset;
            public float CenterOffsetRatio;
            public Vector2 Center;
            public SpriteEffects DirectionEffect;
        }

        public int PermafrostIndex = -1;
        public float FadeInCountdown = 0f;
        public float FadeoutTimer = 0f;
        public PermafrostAurora[] Auroras;

        public static bool ShouldDrawRegularly;

        public static void UpdateDrawEligibility()
        {
            bool useEffect = (PermafrostBackgroundScene.AnyP2Permafrost || ShouldDrawRegularly) && !Main.gameMenu;

            if (SkyManager.Instance["FargowiltasCrossmod:Permafrost"] != null && useEffect != SkyManager.Instance["FargowiltasCrossmod:Permafrost"].IsActive())
            {
                if (useEffect)
                    SkyManager.Instance.Activate("FargowiltasCrossmod:Permafrost");
                else
                    SkyManager.Instance.Deactivate("FargowiltasCrossmod:Permafrost");
            }

            if (ShouldDrawRegularly)
                ShouldDrawRegularly = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (FadeInCountdown > 0)
                FadeInCountdown--;

            if (PermafrostIndex == -1 && !ShouldDrawRegularly)
            {
                UpdatePermafrostIndex();
                if (FadeoutTimer == 0)
                    FadeoutTimer = 45f;
                FadeoutTimer--;
                if (FadeoutTimer <= 0f)
                    Deactivate();
            }

            float auroraStrength = CalculateAuroraStrength();
            float width = Main.screenWidth * 0.5f;
            float height = Main.screenHeight * 0.1f;
            Vector2 centerOffset = Vector2.UnitX * Main.screenWidth * 0.5f;
            float time = Main.GlobalTimeWrappedHourly * 1.2f;
            float centerOffsetRatioIncrement = 1f / 1800f * MathHelper.Lerp(0.2f, 1f, auroraStrength) * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.9f);
            for (int i = 0; i < Auroras.Length; i++)
            {
                Vector2 centerRange = new Vector2(width, height / Auroras[i].Depth);
                Auroras[i].Center = centerRange * (Auroras[i].CenterOffsetRatio * MathHelper.TwoPi).ToRotationVector2() + centerOffset;
                Auroras[i].Center.Y -= 180f + (float)Math.Cos(time + Auroras[i].CenterOffsetRatio * MathHelper.Pi) * 50f;
                Auroras[i].CenterOffsetRatio += centerOffsetRatioIncrement;
            }
        }

        public float GetPermafrostLifeRatio()
        {
            if (UpdatePermafrostIndex())
            {
                float lifeRatio = 0f;
                if (PermafrostIndex != -1)
                    lifeRatio = Main.npc[PermafrostIndex].life / (float)Main.npc[PermafrostIndex].lifeMax;

                return lifeRatio;
            }
            return 0f;
        }

        public float CalculateAuroraStrength()
        {
            if (ShouldDrawRegularly)
                return 1f;

            return 1f - GetPermafrostLifeRatio();
        }

        public bool UpdatePermafrostIndex()
        {
            int PermafrostType = ModContent.NPCType<PermafrostBoss>();
            if (PermafrostIndex >= 0 && Main.npc[PermafrostIndex].active && Main.npc[PermafrostIndex].type == PermafrostType)
                return true;

            PermafrostIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == PermafrostType)
                {
                    PermafrostIndex = i;
                    break;
                }
            }

            return PermafrostIndex != -1;
        }
        internal static Texture2D AuroraTexture
        {
            get
            {
                Main.instance.LoadProjectile(ProjectileID.HallowBossDeathAurora);
                return TextureAssets.Projectile[ProjectileID.HallowBossDeathAurora].Value;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                var auroraTexture = AuroraTexture;
                float auroraStrength = CalculateAuroraStrength();
                float hueOffset = 0.5f * MathHelper.Lerp(0.15f, 0.95f, auroraStrength);
                float time = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 1.2f) * 0.25f;
                float time2 = Main.GlobalTimeWrappedHourly * 0.7f;
                float auroraColorLerp = MathHelper.Lerp(0.3f, 1f, auroraStrength);
                float fadeInLerp = Utils.GetLerpValue(45f, 0f, FadeInCountdown);
                float fadeOutLerp = Utils.GetLerpValue(0f, 45f, FadeoutTimer);
                float brightnessLerp = MathHelper.Lerp(0.6f, 1.1f, (float)Math.Sin(Main.GlobalTimeWrappedHourly / 1.8f) * 0.5f + 0.5f);
                Vector2 origin = auroraTexture.Size() * 0.5f;
                for (int i = 0; i < Auroras.Length; i++)
                {
                    float hue = 0.5f + Auroras[i].ColorHueOffset * hueOffset;
                    hue += time;
                    hue %= 1f;
                    float scale = 1.4f / Auroras[i].Depth;
                    scale += (float)Math.Cos(time2 + Auroras[i].CenterOffsetRatio * MathHelper.TwoPi) * 0.2f;

                    Color auroraColor = Main.hslToRgb(hue, 1f, 0.825f) * 0.85f;
                    auroraColor *= auroraColorLerp;

                    if (FadeInCountdown > 0f)
                        auroraColor *= fadeInLerp;
                    if (FadeoutTimer > 0f)
                        auroraColor *= fadeOutLerp;
                    if (Main.dayTime)
                        auroraColor *= 0.4f;

                    float yBrightness = MathHelper.Lerp(1.5f, 0.5f, 1f - MathHelper.Clamp((Auroras[i].Center.Y + 300f) / 200f, 0f, 1f)) * 1.3f;
                    yBrightness *= brightnessLerp;
                    if (yBrightness > 1.3f)
                        yBrightness = 1.3f;
                    auroraColor *= yBrightness;

                    spriteBatch.Draw(auroraTexture, Auroras[i].Center, null, auroraColor * 1.1f, MathHelper.PiOver2, origin, scale, Auroras[i].DirectionEffect, 0f);
                }
            }
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            FadeInCountdown = 45f;
            Auroras = new PermafrostAurora[150];
            float randomOffsetMax = 3f / Auroras.Length;
            for (int i = 0; i < Auroras.Length; i++)
            {
                Auroras[i].Depth = Main.rand.NextFloat(1f, 2.2f);
                Auroras[i].ColorHueOffset = Main.rand.NextFloat(-1f, 1f);
                Auroras[i].DirectionEffect = Utils.SelectRandom(Main.rand, SpriteEffects.None, SpriteEffects.FlipHorizontally);
                Auroras[i].CenterOffsetRatio = i / (float)Auroras.Length + Main.rand.NextFloat(randomOffsetMax);
            }
        }

        public override void Reset() { }

        public override void Deactivate(params object[] args) { }

        public override bool IsActive() => (PermafrostIndex != -1 || FadeoutTimer > 0 || FadeInCountdown > 0) && !Main.gameMenu;
    }
}