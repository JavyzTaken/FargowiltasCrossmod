using Terraria.ModLoader;
using Terraria;
using CalamityMod.Projectiles.Magic;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDesolation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class SulphurCloudHostile : MiasmaGas
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/MiasmaGas";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = false;
            Projectile.hostile = true;
        }
    }
}
