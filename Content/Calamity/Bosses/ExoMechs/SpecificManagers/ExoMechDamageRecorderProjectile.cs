using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers
{
    public class ExoMechDamageRecorderProjectile : GlobalProjectile
    {
        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (projectile.ModProjectile is not null and IExoMechProjectile mechProj && target.TryGetModPlayer(out ExoMechDamageRecorderPlayer recorderPlayer))
                recorderPlayer.AddDamageFromSource(mechProj.DamageType, info.Damage);
        }
    }
}
