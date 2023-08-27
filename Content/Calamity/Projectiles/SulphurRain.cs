
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class SulphurRain : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dirty Rain Drop");
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 3;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.alpha = 50;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CalamityMod.Buffs.StatDebuffs.ArmorCrunch>(), 600);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<CalamityMod.Buffs.StatDebuffs.ArmorCrunch>(), 600);
        }
    }
}
