using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.QueenSlime
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HallowedSlimeSpike : ModProjectile
    {
        public override void SetDefaults()
        {
            Main.projFrames[Type] = 3;
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Projectile.alpha = 50;
        }
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.QueenSlimeMinionBlueSpike;
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) / 2 - Main.screenPosition, new Rectangle(0, 22 * Projectile.frame, 10, 22), lightColor * (1 - (i / 5f)), Projectile.rotation, new Vector2(10, 22) / 2, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 22 * Projectile.frame, 10, 22), lightColor, Projectile.rotation, new Vector2(10, 22) / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0]++;
                Projectile.frame = Main.rand.Next(0, 3);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            base.AI();
        }
    }
}
