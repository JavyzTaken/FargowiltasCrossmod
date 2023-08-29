using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDevastation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ReaverSpike : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/HellionSpike";
        public override string GlowTexture => "CalamityMod/Projectiles/Melee/HellionSpike";
        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 100;
            Projectile.aiStyle = -1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            base.ModifyDamageHitbox(ref hitbox);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.ai[0] + MathHelper.PiOver4;
            Projectile.ai[1] += 0.04f;

            if (Projectile.ai[1] <= 1)
            {
                Projectile.velocity = Vector2.Lerp(new Vector2(10, 0).RotatedBy(Projectile.ai[0]), Vector2.Zero, Projectile.ai[1]);
            }
            if (Projectile.ai[1] >= 2 && Projectile.ai[1] <= 2.25)
            {
                Projectile.velocity = Vector2.Lerp(Vector2.Zero, new Vector2(20, 0).RotatedBy(Projectile.ai[0]), (Projectile.ai[1] - 2) * 4);
            }
        }
    }
}
