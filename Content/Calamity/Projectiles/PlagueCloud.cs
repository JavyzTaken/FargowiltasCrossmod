using CalamityMod.Particles;
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
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    public class PlagueCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 800;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.height = Projectile.width = 100;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0;
        }
        public override string Texture => "CalamityMod/ExtraTextures/SmallGreyscaleCircle";
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, Color.Green with {  } * Projectile.Opacity, 0, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void AI()
        {
            if (Projectile.ai[1] == 0)
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 18, 0.03f);
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0.6f, 0.03f);
                if (Main.rand.NextBool())
                {
                    Particle part = new TimedSmokeParticle(Projectile.Center + new Vector2(0, Main.rand.NextFloat(0, 25)).RotatedByRandom(MathHelper.TwoPi) * Projectile.scale, Vector2.Zero, Color.Green * 0, Color.LimeGreen, 1, 1, 100, Main.rand.NextFloat(-0.02f, 0.02f));
                    GeneralParticleHandler.SpawnParticle(part);
                }
            }
        }
    }
}
