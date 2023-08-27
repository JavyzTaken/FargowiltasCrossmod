using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using FargowiltasSouls.Content.Bosses.MutantBoss;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDevastation
{
    [JITWhenModsEnabled("CalamityMod")]
    [ExtendsFromMod("CalamityMod")]
    public class ReaverChakram : MutantRetirang
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/MangroveChakram";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //DisplayName.SetDefault("Reaver Chakram");
        }
        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = -1;
            Projectile.width = 46;
            Projectile.height = 38;

        }

        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {

        }
        public override void AI()
        {
            base.AI();


        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
