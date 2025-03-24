using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.DesertScourge
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SandChunk : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 40;
            Projectile.scale = 1.5f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (DLCUtils.HostCheck)
                for (int i = 0; i < 10; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, new Vector2(7, 0).RotatedBy(MathHelper.ToRadians(i * 36)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.GreatSandBlast>(), Projectile.damage, 0);

                }
            for (int i = 0; i < 100; i++)
            {
                Vector2 speed = new Vector2(Main.rand.Next(0, 10), 0).RotatedByRandom(MathHelper.TwoPi);
                Dust.NewDust(Projectile.Center, 0, 0, DustID.Sand, speed.X, speed.Y);
            }
        }
        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length());
            Projectile.velocity.Y += Projectile.velocity.Y > 10 ? 0 : 0.2f;
            if (Main.rand.NextBool(5))
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Sand).noGravity = true;
            }
        }
    }
}
