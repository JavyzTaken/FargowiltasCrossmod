using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Rogue;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration
{
    [JITWhenModsEnabled("CalamityMod")]
    [ExtendsFromMod("CalamityMod")]
    public class ExplorationDust : DuststormCloud
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/DuststormCloud";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 360;
        }
    }
}
