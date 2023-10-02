using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Rogue;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ExplorationTurbulence : TurbulanceProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Turbulance";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = true;
            Projectile.friendly = false;
        }
        public override void OnKill(int timeLeft)
        {

        }
    }
}
