using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Projectiles.Ranged;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExaltation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class PhantomSpirit : BloodflareSoul
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/BloodflareSoul";
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 300;
            Projectile.scale = 1.5f;
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            Player target = Main.player[owner.target];
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Projectile.timeLeft < 260 && Projectile.timeLeft > 160)
            {
                float grah = 1 - (Projectile.timeLeft - 160) / 100f;
                Projectile.velocity = Vector2.Lerp(Vector2.Zero, (target.Center - target.velocity * 10 - Projectile.Center).SafeNormalize(Vector2.Zero) * 20, grah * grah);
            }
            //base.AI();
        }
    }
}
