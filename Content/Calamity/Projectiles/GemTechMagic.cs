
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
    public class GemTechMagic : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/GemTechPurpleGem";
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
            Projectile.scale = 1.5f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Asset<Texture2D> alt = TextureAssets.Projectile[ModContent.ProjectileType<GemTechSummon>()];
            if (Projectile.ai[1] == 0)
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            else
                Main.EntitySpriteDraw(alt.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, alt.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            Projectile.ai[0] = -1;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[1] == 0)
            Lighting.AddLight(Projectile.Center, TorchID.Purple);
            else
                Lighting.AddLight(Projectile.Center, TorchID.Ice);
            NPC target = null;
            Player owner = null;
            if (Projectile.owner >= 0) {
                owner = Main.player[Projectile.owner];
            }
            if (Projectile.ai[0] >= 0)
            target = Main.npc[(int)Projectile.ai[0]];
            if (Projectile.velocity.Length() < 0.5f)
            {
                if (Projectile.FindTargetWithinRange(800) != null)
                Projectile.ai[0] = Projectile.FindTargetWithinRange(800).whoAmI;
                if (Projectile.ai[1] == 1)
                {
                    Projectile.ai[1] = 2;
                }
            }
            if (target != null && target.active && target.CanBeChasedBy(Projectile) && Projectile.ai[1] == 0)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20, 0.08f);
            }
            else if (Projectile.ai[1] == 2 && owner != null && owner.active && !owner.dead)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20, 0.08f);
                if (owner.Hitbox.Intersects(Projectile.Hitbox))
                {
                    Projectile.Kill();
                    owner.ManaEffect(50);
                }
            }
            else
            {
                Projectile.velocity /= 1.02f;
            }
            if (Projectile.ai[1] == 0)
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch).noGravity = true;
            else
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch).noGravity = true;
        }

    }
}
