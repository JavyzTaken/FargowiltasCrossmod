using CalamityMod;
using CalamityMod.UI.VanillaBossBars;
using FargowiltasCrossmod.Assets;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers;
using FargowiltasCrossmod.Core;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExoMechBossBar : ModBossBar
    {
        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {
            (Texture2D barTexture, Vector2 barCenter, _, _, Color iconColor, float life, float lifeMax, float shield, float shieldMax, float iconScale, bool showText, Vector2 textOffset) = drawParams;

            NPC fakeExoMech = ModContent.GetInstance<ExoMechsBossBar>().FalseNPCSegment;
            fakeExoMech ??= new();

            life = 0f;
            lifeMax = 0f;
            foreach (int exoMechID in ExoMechNPCIDs.ManagingExoMechIDs)
            {
                fakeExoMech.SetDefaults(exoMechID);
                lifeMax += fakeExoMech.lifeMax;

                int exoMechIndex = NPC.FindFirstNPC(exoMechID);
                NPC? exoMech = exoMechIndex >= 0 ? Main.npc[exoMechIndex] : null;

                bool exoMechWasKilled = exoMech is null && ExoMechFightStateManager.PreviouslySummonedMechIDs.Contains(exoMechID);
                bool exoMechYetToBeSummoned = exoMech is null && !ExoMechFightStateManager.PreviouslySummonedMechIDs.Contains(exoMechID);

                if (exoMechYetToBeSummoned)
                    life += fakeExoMech.lifeMax;
                else if (exoMech is not null)
                    life += exoMech.life;
            }

            float lifeRatio = LumUtils.Saturate(life / lifeMax);

            int headTextureIndex = NPCID.Sets.BossHeadTextures[npc.type];
            if (headTextureIndex == -1)
            {
                NPCLoader.BossHeadSlot(npc, ref headTextureIndex);
                if (headTextureIndex == -1)
                    return false;
            }

            Texture2D iconTexture = TextureAssets.NpcHeadBoss[headTextureIndex].Value;
            Rectangle iconFrame = iconTexture.Frame();

            Point barSize = new(456, 22);
            Point topLeftOffset = new(32, 24);
            int frameCount = 6;

            Rectangle bgFrame = barTexture.Frame(verticalFrames: frameCount, frameY: 3);
            Color bgColor = Color.White * 0.2f;

            int scale = (int)(barSize.X * lifeRatio);
            scale -= scale % 2;
            Rectangle barFrame = barTexture.Frame(verticalFrames: frameCount, frameY: 2);
            barFrame.X += topLeftOffset.X;
            barFrame.Y += topLeftOffset.Y;
            barFrame.Width = 2;
            barFrame.Height = barSize.Y;

            int shieldScale = (int)(barSize.X * shield / shieldMax);
            shieldScale -= shieldScale % 2;

            Rectangle barShieldFrame = barTexture.Frame(verticalFrames: frameCount, frameY: 5);
            barShieldFrame.X += topLeftOffset.X;
            barShieldFrame.Y += topLeftOffset.Y;
            barShieldFrame.Width = 2;
            barShieldFrame.Height = barSize.Y;

            Rectangle tipShieldFrame = barTexture.Frame(verticalFrames: frameCount, frameY: 4);
            tipShieldFrame.X += topLeftOffset.X;
            tipShieldFrame.Y += topLeftOffset.Y;
            tipShieldFrame.Width = 2;
            tipShieldFrame.Height = barSize.Y;

            Rectangle barPosition = Utils.CenteredRectangle(barCenter, barSize.ToVector2());
            Vector2 barTopLeft = barPosition.TopLeft();
            Vector2 topLeft = barTopLeft - topLeftOffset.ToVector2();

            // Background.
            spriteBatch.Draw(barTexture, topLeft, bgFrame, bgColor, 0f, Vector2.Zero, 1f, 0, 0f);

            Main.spriteBatch.PrepareForShaders(null, true);
            DrawBar(barTexture, barTopLeft, barFrame, scale, lifeRatio);
            Main.spriteBatch.ResetToDefaultUI();

            // Bar itself (shield).
            if (shield > 0f)
            {
                Vector2 stretchScale = new(shieldScale / barFrame.Width, 1f);
                spriteBatch.Draw(barTexture, barTopLeft, barShieldFrame, Color.White, 0f, Vector2.Zero, stretchScale, 0, 0f);
                spriteBatch.Draw(barTexture, barTopLeft + new Vector2(shieldScale - 2, 0f), tipShieldFrame, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);
            }

            // Frame.
            Rectangle frameFrame = barTexture.Frame(verticalFrames: frameCount, frameY: 0);
            spriteBatch.Draw(barTexture, topLeft, frameFrame, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);

            // Icon.
            Vector2 iconOffset = new(4f, 20f);
            Vector2 iconSize = new(26f, 28f);
            Vector2 iconPosition = iconOffset + iconSize * 0.5f;
            spriteBatch.Draw(iconTexture, topLeft + iconPosition, iconFrame, iconColor, 0f, iconFrame.Size() / 2f, iconScale, 0, 0f);

            // Health text.
            if (BigProgressBarSystem.ShowText && showText)
            {
                if (shield > 0f)
                    BigProgressBarHelper.DrawHealthText(spriteBatch, barPosition, textOffset, shield, shieldMax);
                else
                    BigProgressBarHelper.DrawHealthText(spriteBatch, barPosition, textOffset, life, lifeMax);
            }
            return false;
        }

        private static void DrawBar(Texture2D barTexture, Vector2 barTopLeft, Rectangle barFrame, float scale, float lifeRatio)
        {
            Vector3[] palette = new Vector3[CalamityUtils.ExoPalette.Length];
            for (int i = 0; i < palette.Length; i++)
                palette[i] = Color.Lerp(CalamityUtils.ExoPalette[i], Color.White, 0.25f).ToVector3();

            ManagedShader healthBarShader = ShaderManager.GetShader("FargowiltasCrossmod.ExoMechHealthBarShader");
            healthBarShader.TrySetParameter("imageSize", barTexture.Size());
            healthBarShader.TrySetParameter("horizontalSquish", lifeRatio);
            healthBarShader.TrySetParameter("sourceRectangle", new Vector4(barFrame.X, barFrame.Y, barFrame.Width, barFrame.Height));
            healthBarShader.TrySetParameter("gradient", palette);
            healthBarShader.TrySetParameter("gradientCount", palette.Length);
            healthBarShader.SetTexture(NoiseTexturesRegistry.ElectricNoise.Value, 1, SamplerState.LinearWrap);
            healthBarShader.Apply();

            // Bar.
            Vector2 stretchScale = new(scale / barFrame.Width, 1f);
            Main.spriteBatch.Draw(barTexture, barTopLeft, barFrame, Color.White, 0f, Vector2.Zero, stretchScale, 0, 0f);
        }
    }
}
