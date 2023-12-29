using CalamityMod;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.WallofFlesh
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HomingSickle : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.DemonScythe;
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.timeLeft = 1000;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Projectile.DrawProjectileWithBackglow(Color.Magenta, lightColor, 4f);
            //Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.Center);
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.rotation += MathHelper.ToRadians(30);
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch).noGravity = true;
            if (Projectile.velocity.Length() < 16) Projectile.velocity *= 1.025f;
            int p = Player.FindClosest(Projectile.Center, 1, 1);
            if (Projectile.ai[0] > 30 && Projectile.ai[0] < 130 && p >= 0)
            {
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(Utils.AngleTowards(Projectile.velocity.ToRotation(), Projectile.AngleTo(Main.player[p].Center), 0.025f));
            }
        }
    }
}
