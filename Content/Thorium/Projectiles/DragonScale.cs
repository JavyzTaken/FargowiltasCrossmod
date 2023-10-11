using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    public class DragonScale : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        public override void PostAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}