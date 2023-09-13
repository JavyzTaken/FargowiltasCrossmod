using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public partial class CalamityGlobalProjectile : GlobalProjectile
    {
        public override void SetDefaults(Projectile entity)
        {
            base.SetDefaults(entity);
        }
        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            base.OnSpawn(projectile, source);
        }
        public override void Kill(Projectile projectile, int timeLeft)
        {
            base.Kill(projectile, timeLeft);
        }
        public override bool PreAI(Projectile projectile)
        {
            return base.PreAI(projectile);
        }
        public override void AI(Projectile projectile)
        {
            
            GemTechMinionEffect(projectile);
        }
    }
}
