using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using CalamityMod.Projectiles.Rogue;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
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
