
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class GemTechMeleeWave : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.TerraBlade2Shot;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 300;
            Main.projFrames[Type] = 4;
            Projectile.scale = 1.5f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            for (int i = -3; i < 2; i++)
            {
                Projectile.Opacity = 0.5f;
                if (i != 0) Projectile.Opacity = 0.2f;
                if (i == 2) Projectile.Opacity = 0.1f;
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition + new Vector2(-50, 0).RotatedBy(Projectile.rotation), new Rectangle(0, 0, t.Width(), t.Height() / 4), new Color(250, 150, 0) * Projectile.Opacity, Projectile.rotation + MathHelper.ToRadians(i * 10), new Vector2(t.Width() / 2, t.Height() / 8), Projectile.scale, SpriteEffects.None);
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition + new Vector2(-50, 0).RotatedBy(Projectile.rotation), new Rectangle(0, t.Height() / 4, t.Width(), t.Height() / 4), new Color(250, 200, 0) * Projectile.Opacity, Projectile.rotation + MathHelper.ToRadians(i * 10), new Vector2(t.Width() / 2, t.Height() / 8), Projectile.scale, SpriteEffects.None);
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition + new Vector2(-50, 0).RotatedBy(Projectile.rotation), new Rectangle(0, t.Height() / 4 * 2, t.Width(), t.Height() / 4), new Color(250, 100, 0) * Projectile.Opacity, Projectile.rotation + MathHelper.ToRadians(i * 10), new Vector2(t.Width() / 2, t.Height() / 8), Projectile.scale, SpriteEffects.None);
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition + new Vector2(-50, 0).RotatedBy(Projectile.rotation), new Rectangle(0, t.Height() / 4 * 3, t.Width(), t.Height() / 4), new Color(250, 250, 250) * Projectile.Opacity, Projectile.rotation + MathHelper.ToRadians(i * 10), new Vector2(t.Width() / 2, t.Height() / 8), Projectile.scale, SpriteEffects.None);
            }
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, TorchID.Yellow);
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch).noGravity = true;
        }

    }
}
