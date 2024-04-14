using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using FargowiltasCrossmod.Core.Common;
using Terraria.Audio;
using CalamityMod.Projectiles.Boss;
using Terraria.DataStructures;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimstoneDebris : ModProjectile
    {
        
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.scale = 2;
            Projectile.localAI[0] = Main.rand.Next(0, 3);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> rock = TextureAssets.Projectile[Type];
            Asset<Texture2D> line = TextureAssets.Extra[178];

            float opacity = 0;
            if (Projectile.timeLeft > 270)
            {
                opacity = MathHelper.Lerp(0, 1, (Projectile.timeLeft - 270) / 30f);
            }
            Main.EntitySpriteDraw(line.Value, Projectile.Center - Main.screenPosition, null, lightColor * opacity, new Vector2(Projectile.ai[0], Projectile.ai[1]).ToRotation(), new Vector2(0, line.Height() * 0.5f), new Vector2(1, Projectile.scale*2), SpriteEffects.None);

            Rectangle source = new Rectangle(0, 0, 32, 32);
            if (Projectile.localAI[0] == 1) source = new Rectangle(36, 6, 30, 26);
            if (Projectile.ai[0] == 2) source = new Rectangle(70, 0, 26, 32);
            DLCUtils.DrawBackglow(rock, Color.Red * Projectile.Opacity, Projectile.Center, new Vector2(16, 16), Projectile.rotation + MathHelper.PiOver2, Projectile.scale, offsetMult: 3, sourceRectangle: source);
            Main.EntitySpriteDraw(rock.Value, Projectile.Center - Main.screenPosition, source, lightColor * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver2, new Vector2(16, 16), Projectile.scale, SpriteEffects.None);


            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item69, Projectile.Center);
            for (int i = 0; i < 100; i++)
            {
                Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Ash, Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-7, 7)).noGravity = true;
            }
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            
            //SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.type == ModContent.ProjectileType<BrimstoneBarrage>() && proj.hostile && proj.Hitbox.Intersects(Projectile.Hitbox))
                {
                    proj.Kill();
                    proj.netUpdate = true;
                }
            }
            if (Projectile.timeLeft > 270)
            {
                Projectile.Opacity = 0;
            }
            else
            {
                Projectile.Opacity = 1;
            }
            if (Projectile.timeLeft == 270)
            {
                SoundEngine.PlaySound(SoundID.Item69, Projectile.Center);
                Projectile.velocity = new Vector2(Projectile.ai[0], Projectile.ai[1]);
            }
            Projectile.velocity *= 0.98f;
            base.AI();
        }
    }
}
