using System;
using System.IO;
using CalamityMod.NPCs.CalClone;
using Fargowiltas.Common.Configs;
using FargowiltasCrossmod.Content.Common.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Luminance.Assets;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalcloneFireBackground : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override Color? GetAlpha(Color lightColor) => lightColor * Projectile.Opacity;

        ref float NPCID => ref Projectile.ai[0];


        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Terraria.ID.ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 5000;
        }

        public override void SetDefaults()
        {
            Terraria.ID.ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 5000;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.Opacity = 0f;
        }

        public override void AI()
        {
            int npcID = (int)NPCID;
            if (!npcID.IsWithinBounds(Main.maxNPCs))
            {
                Deplete();
                return;
            }
            NPC npc = Main.npc[npcID];
            if (!npc.TypeAlive<CalamityMod.NPCs.CalClone.CalamitasClone>())
            {
                Deplete();
                return;
            }
            Projectile.timeLeft = 60 * 5;
            Projectile.Center = Main.LocalPlayer.Center;
            float opacity = 1f - 0.3f * npc.GetLifePercent();
            if (NPC.AnyNPCs(ModContent.NPCType<Cataclysm>()) || NPC.AnyNPCs(ModContent.NPCType<Catastrophe>()))
            {
                opacity = 0.5f;
            }
            Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, opacity, 0.001f);
        }
        public void Deplete()
        {
            Projectile.Opacity -= 0.05f;
            if (Projectile.Opacity < 0f)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 auraPos = Main.LocalPlayer.Center;
            float radius = Main.screenWidth * 1.2f / 2;
            var target = Main.LocalPlayer;
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargosTextureRegistry.WavyNoise;
            var maxOpacity = Projectile.Opacity * 0.15f;

            if (!blackTile.IsLoaded || !diagonalNoise.IsLoaded)
                return false;

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.CalcloneBackgroundShader");
            shader.TrySetParameter("colorMult", 7.35f);
            shader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            shader.TrySetParameter("radius", radius);
            shader.TrySetParameter("anchorPoint", auraPos);
            shader.TrySetParameter("screenPosition", Main.screenPosition);
            shader.TrySetParameter("screenSize", Main.ScreenSize.ToVector2());
            shader.TrySetParameter("playerPosition", target.Center);
            shader.TrySetParameter("maxOpacity", maxOpacity);


            Main.spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, shader.WrappedEffect, Main.GameViewMatrix.TransformationMatrix);
            Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
            Main.spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
