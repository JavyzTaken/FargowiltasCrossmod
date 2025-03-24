using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    public class BrimstoneBall : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/BrimstoneBall";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.rotation += 0.12f * Projectile.direction;

            if (Projectile.velocity.Length() < 17)
                Projectile.velocity *= 1.1f;

            Lighting.AddLight(Projectile.Center, 0.25f, 0f, 0f);

            for (int i = 0; i < 2; i++)
            {
                Vector2 dspeed = -Projectile.velocity * 0.7f;
                int brimDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 150, default, 1.1f);
                Main.dust[brimDust].noGravity = true;
                Main.dust[brimDust].velocity = dspeed;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 90);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color backglowColor = Color.Red;

            Projectile.DrawProjectileWithBackglow(backglowColor, lightColor, 2f);
            return false;
        }
    }
}
