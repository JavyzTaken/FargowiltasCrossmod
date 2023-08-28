
using CalamityMod;
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
    public class GemTechRogue : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/GemTechRedGem";
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
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.timeLeft = 300;
            Projectile.scale = 2;
            Projectile.penetrate = -1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[2] = 1;
            Projectile.velocity *= 0;
            Projectile.friendly = false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, TorchID.Red);
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch).noGravity = true;
            
            if (Projectile.ai[2] == 1 && Projectile.owner >= 0 && Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
            {
                Player owner = Main.player[Projectile.owner];
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 25, 0.04f);
                if (Projectile.Hitbox.Intersects(owner.Hitbox))
                {
                    owner.Calamity().rogueStealth += 0.5f;
                    Projectile.Kill();
                    CombatText.NewText(new Rectangle((int)owner.Center.X, (int)owner.Center.Y - 20, 0, 0), Color.Yellow, (50).ToString());
                }
            }
            else
            {
                Vector2 targetVel = new Vector2(Projectile.ai[0], Projectile.ai[1]);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVel, 0.02f);
            }
        }

    }
}
