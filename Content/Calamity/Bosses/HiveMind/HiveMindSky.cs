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

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                float opacity = 0f;
                Color color = ColorToUse(ref opacity);

                spriteBatch.Draw(ModContent.Request<Texture2D>("FargowiltasSouls/Content/Sky/MutantSky", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), color * opacity);
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
