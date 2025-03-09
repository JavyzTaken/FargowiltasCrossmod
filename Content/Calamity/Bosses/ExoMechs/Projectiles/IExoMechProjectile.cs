using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    public interface IExoMechProjectile
    {
        /// <summary>
        /// The damage of damage this Exo Mech projectile inflicts upon players.
        /// </summary>
        public ExoMechDamageSource DamageType
        {
            get;
        }
    }
}
