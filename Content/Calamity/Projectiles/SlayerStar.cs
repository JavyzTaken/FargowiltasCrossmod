using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class SlayerStar : ModProjectile
    {
        int killTime;
        public override string Texture => "CalamityMod/Projectiles/Typeless/NebulaStar";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("God Slayer's Star");
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
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
                if (Projectile.ai[0] == 1 && killTime % 5 == 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, Main.rand.Next(20, 200)).RotatedByRandom(MathHelper.TwoPi), Vector2.Zero, ModContent.ProjectileType<AuricExplosion>(), 1000, 0, Main.myPlayer, 1);
                }
            }
        }
    }
}
