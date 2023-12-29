using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Golem
{
    public class BouncingFireball : ModProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.aiStyle = ProjAIStyleID.Bounce;
            AIType = ProjectileID.Fireball;
            Projectile.timeLeft = 200;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
            target.AddBuff(BuffID.OnFire3, 200);
        }
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Fireball;
        public override void AI()
        {
            base.AI();
        }
    }
}
