using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDevastation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class FireChain : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Flamelash;
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.alpha = 255;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)Projectile.ai[1];
        }
        public override void OnKill(int timeLeft)
        {

        }

        public override void AI()
        {
            Projectile.position.X = (int)Projectile.position.X + 9 * Projectile.spriteDirection;

            if (Projectile.position.X % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Projectile.timeLeft > Projectile.ai[1] / 2)
                {
                    Projectile explode = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileID.SolarWhipSwordExplosion, 0, 0);
                    explode.ai[1] = 1;
                }
                else if (Projectile.timeLeft < Projectile.ai[1] / 2)
                {
                    Projectile fire = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, -25), new Vector2(0, -10), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.YharonFireball2>(), 50, 0);

                }
            }
            if (Projectile.timeLeft == Projectile.ai[1] / 2)
            {
                Projectile.position.X -= Projectile.ai[1] / 2 * (9 * Projectile.spriteDirection);
            }
        }
    }
}
