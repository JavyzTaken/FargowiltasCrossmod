using Terraria;
using Terraria.ModLoader;
using CalamityMod;
using Terraria.ID;


namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class FathomCloud : ModProjectile
    {
        private int timerDead;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("How did you get killed by THIS");
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
            timerDead++;
            if (timerDead > 600)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Projectile.ai[0] += 1f;
                if (Projectile.ai[0] > 16f)
                {
                    Projectile.ai[0] = 0f;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Player player = Main.player[Projectile.owner];
                        int rainX = (int)(Projectile.position.X + 14f + Main.rand.Next(Projectile.width - 28));
                        int rainY = (int)(Projectile.position.Y + Projectile.height + 4f);
                        int damage = (int)player.GetBestClassDamage().ApplyTo(60f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), rainX, rainY, 0f, 5f, ModContent.ProjectileType<SulphurRain>(), damage, 0f, Projectile.owner);
                    }
                }
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] > 64f)
                {
                    Projectile.ai[1] = 0f;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Player player = Main.player[Projectile.owner];
                        int lightnDamage = (int)player.GetBestClassDamage().ApplyTo(100f);
                        int lightning = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.position.Y, 0f, 15f, ProjectileID.CultistBossLightningOrbArc, lightnDamage, 0f, Projectile.owner, ai0: 1.5f, ai1: Main.rand.Next(0, 100));
                        if (Main.projectile.IndexInRange(lightning))
                        {
                            Main.projectile[lightning].friendly = true;
                            Main.projectile[lightning].hostile = false;
                            Main.projectile[lightning].penetrate = 5;
                            Main.projectile[lightning].usesLocalNPCImmunity = true;
                            Main.projectile[lightning].localNPCHitCooldown = 100;
                        }
                    }
                }
            }


        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Main.myPlayer];
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.NastyPop = false;
        }
    }
}
