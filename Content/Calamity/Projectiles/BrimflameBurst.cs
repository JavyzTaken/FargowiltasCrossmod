using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    public class BrimflameBurst : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0;
            Projectile.DamageType = DamageClass.Generic;
            base.SetDefaults();
        }
        public override string Texture => "CalamityMod/Projectiles/Boss/BrimstoneHellfireball";
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            
           
            Asset<Texture2D> fog = TextureAssets.Projectile[ModContent.ProjectileType<RancorFog>()];
            
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (int i = 0; i < 5; i++)
            {
                Main.EntitySpriteDraw(fog.Value, Projectile.Center - Main.screenPosition + new Vector2(0, 25).RotatedBy(Projectile.rotation), null, Color.Red, Projectile.rotation + i, fog.Size() / 2, Projectile.scale * 0.3f, SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 492 / 6 * Projectile.frame, 34, 492 / 6), Color.White, Projectile.rotation, new Vector2(t.Width(), t.Height() / 6) / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            base.OnHitPlayer(target, info);
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.Opacity < 1 && Projectile.timeLeft > 50)
            {
                Projectile.Opacity += 0.1f;
            }
            if (Projectile.Opacity > 0 && Projectile.timeLeft < 10)
            {
                Projectile.Opacity -= 0.1f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0, 0));
            base.AI();
            
        }
    }
}
