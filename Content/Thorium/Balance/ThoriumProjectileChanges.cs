using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Balance
{
    public class ThoriumProjectileChanges : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return true;
        }
    }
}
