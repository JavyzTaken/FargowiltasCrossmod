using CalamityMod.Graphics.Renderers;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Systems;
using CalamityMod;
using FargowiltasSouls;
using Luminance.Core.Graphics;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BrimstoneArenaRenderer : BaseRenderer
    {
        #region Fields/Properties

        public override DrawLayer Layer => DrawLayer.AfterEverything;

        public static CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental Brimmy
        {
            get
            {
                if (!Main.npc.IndexInRange(CalamityGlobalNPC.brimstoneElemental))
                    return null;
                if (Main.npc[CalamityGlobalNPC.brimstoneElemental].type != ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>())
                    return null;

                if (Main.npc[CalamityGlobalNPC.brimstoneElemental].ModNPC is not null &&
                    Main.npc[CalamityGlobalNPC.brimstoneElemental].ModNPC is CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental brimmy && Main.npc[CalamityGlobalNPC.brimstoneElemental].TryGetDLCBehavior(out BrimstoneEternity _))
                    return brimmy;

                return null;
            }
        }

        //Should only draw if not in the main menu, provi is active and the boolean for drawing the border is true.
        public override bool ShouldDraw => !Main.gameMenu && CalamityGlobalNPC.brimstoneElemental != -1 &&
            Main.npc[CalamityGlobalNPC.brimstoneElemental].TypeAlive<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>() && Main.npc[CalamityGlobalNPC.brimstoneElemental].TryGetDLCBehavior(out BrimstoneEternity _);
        #endregion

        #region Methods
        public override void DrawToTarget(SpriteBatch spriteBatch)
        {
            var brimmy = Brimmy;
            if (brimmy == null)
                return;
            NPC npc = brimmy.NPC;
            BrimstoneEternity eternity = npc.GetDLCBehavior<BrimstoneEternity>();
            var borderDistance = 220 * 4;
            if (!npc.HasValidTarget)
                return;

            var target = Main.LocalPlayer;
            var burnIntensity = target.GetModPlayer<BrimstoneBurnPlayer>().BurnFadeIntensity;


            //Begin drawing the inferno
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/HarshNoise");
            var upwardPerlinNoise = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Perlin");
            var upwardNoise = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/MeltyNoise");

            var maxOpacity = eternity.auraOpacity;


            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.BrimstoneArenaShader");
            shader.TrySetParameter("colorMult", 7.35f); 
            shader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            shader.TrySetParameter("radius", borderDistance);
            shader.TrySetParameter("anchorPoint", eternity.auraPos);
            shader.TrySetParameter("screenPosition", Main.screenPosition);
            shader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            shader.TrySetParameter("burnIntensity", burnIntensity);
            shader.TrySetParameter("playerPosition", target.Center);
            shader.TrySetParameter("maxOpacity", maxOpacity);

            spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;
            spriteBatch.GraphicsDevice.Textures[2] = upwardNoise.Value;
            spriteBatch.GraphicsDevice.Textures[3] = upwardPerlinNoise.Value;

            //Manual end begin for the sampler state
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, shader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            //Inferno drawing complete
            CalamityUtils.ExitShaderRegion(spriteBatch);
        }
        #endregion
    }
}
