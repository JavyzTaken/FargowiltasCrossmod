using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofAnnihilation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class Corvid : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/PowerfulRaven";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }
        public override void SetDefaults()
        {
            Main.projFrames[Type] = 5;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.timeLeft = 300;
            DrawOffsetX = -15;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {

        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame == 4) Projectile.frame = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            // Player target = Main.player[(int)Projectile.ai[0]];

            if (Projectile.timeLeft == 299)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.ai[0] = Main.rand.NextBool() ? 1 : -1;
                    Projectile.ai[1] = Main.rand.Next(0, 10);

                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(45 * Projectile.ai[0]));
                }
                Projectile.netUpdate = true;
            }
            if (Projectile.timeLeft == 299 - Projectile.ai[1])
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(45 * Projectile.ai[0] * -1));
            }
            if (Projectile.velocity.X < 0)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = Projectile.rotation + MathHelper.Pi;
            }
            else
            {
                Projectile.spriteDirection = 1;
            }
        }
    }
}
