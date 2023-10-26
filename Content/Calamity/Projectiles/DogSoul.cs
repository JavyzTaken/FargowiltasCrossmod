using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Terraria.Utilities;
using CalamityMod.Projectiles.Rogue;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class DogSoul : ModProjectile
    {
        int killTime;
        public override string Texture => "CalamityMod/NPCs/DevourerofGods/DevourerofGodsHead";
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
        }
        public override void AI()
        {
            killTime++;
            if (killTime >= 30)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    Player player = Main.player[Projectile.owner];
                    player.Center = Projectile.Center;
                }
                Projectile.Kill();
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 72; i++)
            {
                int dustType = Main.rand.Next(new int[3] { 180, 173, 244 });
                Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, dustType, 0f, 1f, 0, default, 1f);
                dust.velocity *= 3f;
                dust.scale *= 1.15f;
            }
        }
    }
}
