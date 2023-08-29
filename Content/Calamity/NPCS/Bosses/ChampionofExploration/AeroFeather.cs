using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AeroFeather : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/StickyFeather";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
        }
        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void Kill(int timeLeft)
        {

        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
            if (Projectile.velocity.Y < 20)
            {
                Projectile.velocity.Y += 0.5f;
            }
        }
    }
}
