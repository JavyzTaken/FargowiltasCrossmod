using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Rogue;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration
{
    [JITWhenModsEnabled("CalamityMod")]
    [ExtendsFromMod("CalamityMod")]
    public class ExplorationTurbulence : TurbulanceProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Turbulance";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = true;
            Projectile.friendly = false;
        }
        public override void Kill(int timeLeft)
        {

        }
    }
}
