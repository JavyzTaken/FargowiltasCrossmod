using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration
{
    
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ExplorationShot : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.timeLeft = 1000;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.scale = 2;
            Projectile.extraUpdates = 5;
        }
        public override string Texture => "CalamityMod/Projectiles/Ranged/FlashRoundProj";
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile coin = null;
            Projectile.ai[0] = 0;
            if (Projectile.ai[1] == -1 || !Main.projectile[(int)Projectile.ai[1]].active && Main.projectile[(int)Projectile.ai[1]].type == ModContent.ProjectileType<ExplorationCoin>() || Projectile.Hitbox.Intersects(Main.projectile[(int)Projectile.ai[1]].Hitbox))
            {

                for (int i = 0; i < Main.projectile.Length; i++)
                {

                    Projectile the = Main.projectile[i];
                    if (the.Hitbox.Intersects(Projectile.Hitbox) && the.type == ModContent.ProjectileType<ExplorationCoin>())
                    {

                        the.Kill();
                    }
                    if (the.type == ModContent.ProjectileType<ExplorationCoin>() && (coin == null || the.Distance(Projectile.Center) < coin.Distance(Projectile.Center)) && !the.Hitbox.Intersects(Projectile.Hitbox) && the.active)
                    {
                        coin = the;
                        Projectile.ai[1] = coin.whoAmI;

                    }

                }

                if (coin != null && coin.active)
                {
                    coin.velocity *= 0;
                    Projectile.velocity = (coin.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 10;
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/UltrablingHit"), Projectile.Center);

                }
                if (coin == null && Projectile.ai[1] != -1 && Projectile.Hitbox.Intersects(Main.projectile[(int)Projectile.ai[1]].Hitbox))
                {

                    Projectile.velocity = (Main.player[0].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 10;
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/UltrablingHit"), Projectile.Center);
                }
            }
        }
    }
}
