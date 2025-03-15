using System;
using System.IO;
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
    public class CalcloneWave : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override Color? GetAlpha(Color lightColor) => lightColor * Projectile.Opacity;

        ref float Timer => ref Projectile.ai[0];

        ref float Decrement => ref Projectile.ai[1];

        ref float speed => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Timer = 0;
        }

        public override bool PreAI()
        {
            if (Timer % 14 == 0)
            {
                Decrement++;
                Projectile.Opacity = 1 - (Decrement / speed);
            }
            //Projectile gets bigger (at a decreasing rate) over time
            Projectile.position = Projectile.Center;
            Projectile.width += (int)(speed - Decrement);
            Projectile.height += (int)(speed - Decrement);
            Projectile.Center = Projectile.position;
            if (Projectile.Opacity == 0)
            {
                Projectile.Kill();
                return false;
            }
            return true;
        }

        public override void AI()
        {
            Timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 auraPos = Projectile.Center;
            float radius = Projectile.width / 2;
            var target = Main.LocalPlayer;
            var blackTile = TextureAssets.MagicPixel;
            var diagonalNoise = FargosTextureRegistry.WavyNoise;
            var maxOpacity = Projectile.Opacity * 0.2f;

            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.CalcloneWaveShader");
            shader.TrySetParameter("colorMult", 7.35f);
            shader.TrySetParameter("time", Main.GlobalTimeWrappedHourly);
            shader.TrySetParameter("radius", radius * Projectile.scale);
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
