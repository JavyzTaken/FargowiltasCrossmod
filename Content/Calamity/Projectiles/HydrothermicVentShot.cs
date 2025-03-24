using System;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    public class HydrothermicVentShot : ModProjectile, IPixelatedPrimitiveRenderer
    {
        public override string Texture => "Terraria/Images/Projectile_258";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 14;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 105 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.25f, 0f);

            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20 with { Volume = 10, Pitch = -0.5f }, Projectile.position);
                Projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;
            }

            Projectile.spriteDirection = Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation += 0.3f * Projectile.direction;

            if (Main.rand.NextBool(4))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Flare, 0f, 0f, 100, default, 1.2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
                Main.dust[d].velocity += Projectile.velocity * 0.1f;
            }

            if (Projectile.timeLeft < 105)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 450f, 12f, 20f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.OnFire3, 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
            target.AddBuff(BuffID.OnFire3, 180);
        }

        // visuals
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 25);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.OrangeRed, Color.Red, 0.3f);
            return Color.Lerp(color, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.BlobTrail");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.FadedStreak.Value);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 44);
        }
    }
}
