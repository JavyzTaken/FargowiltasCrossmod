
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class GelKunai : ModProjectile
    {
        int killTime;
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/GelDart";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Statis' Kunai");
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.alpha = 100;
        }
        public override void AI()
        {
            killTime++;
            if (killTime >= 110)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255) Projectile.Kill();
            }
        }
    }
}
