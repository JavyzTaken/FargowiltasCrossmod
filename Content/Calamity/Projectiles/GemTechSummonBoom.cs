
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
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class GemTechSummonBoom : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/GlowRing";
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
            Projectile.width = 45;
            Projectile.height = 45;
            Projectile.timeLeft = 40;
            Projectile.scale = 0.01f;
            Projectile.Opacity = 0.5f;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, new Color(130, 130, 255) * Projectile.Opacity, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, TorchID.Ice);
            Vector2 center = Projectile.Center;
            Projectile.width = (int)(65 * Projectile.scale);
            Projectile.height = (int)(65 * Projectile.scale);
            Projectile.Center = center;
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch).noGravity = true;
            double x = 1 - (Projectile.timeLeft / 40f);
            Projectile.scale = MathHelper.Lerp(0, 5f, (float)(1 - Math.Pow(1 - x, 5)));
            if (Projectile.timeLeft < 20)
            {
                Projectile.Opacity -= 0.05f;
            }
        }

    }
}
