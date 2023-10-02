using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using CalamityMod;


namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class FearsomeFeather : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fearsome Feather");
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
            Projectile.penetrate = 3;
            Projectile.alpha = 255;
            Projectile.aiStyle = 93;
            AIType = 514;
        }
        public override void AI()
        {
            if (Projectile.timeLeft < 320)
            {
                Projectile.tileCollide = true;
            }
            CalamityUtils.HomeInOnNPC(Projectile, true, 150f, 12f, 20f);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(in SoundID.Item14, Projectile.position);
            for (int i = 0; i < 15; i++)
            {
                int exzploDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple, 0f, 0f, 100, default, 1.2f);
                Dust dustLmao = Main.dust[exzploDust];
                dustLmao.velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[exzploDust].scale = 0.5f;
                    Main.dust[exzploDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}