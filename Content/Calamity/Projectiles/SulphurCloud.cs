using Terraria;
using Terraria.ModLoader;
using CalamityMod;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class SulphurCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("How did you get killed by this");
            Main.projFrames[Projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 600f)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] > 32f)
                {
                    Projectile.ai[1] = 0f;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Player player = Main.player[Projectile.owner];
                        int rainX = (int)(Projectile.position.X + 14f + Main.rand.Next(Projectile.width - 28));
                        int rainY = (int)(Projectile.position.Y + Projectile.height + 4f);
                        int damage = (int)player.GetBestClassDamage().ApplyTo(20f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), rainX, rainY, 0f, 5f, ModContent.ProjectileType<SulphurRain>(), damage, 0f, Projectile.owner);
                    }
                }
            }


        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Main.myPlayer];
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.DirtyPop = false;
        }
    }
}
