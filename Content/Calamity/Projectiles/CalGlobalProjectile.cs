using CalamityMod.Events;
using CalamityMod.World;
using FargowiltasCrossmod.Core.Calamity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    public class CalGlobalProjectile : GlobalProjectile
    {
        public override void AI(Projectile projectile)
        {
            if (projectile.type == 872 && (CalamityWorld.revenge || BossRushEvent.BossRushActive) && projectile.timeLeft > 570 && ModContent.GetInstance<CalamityConfig>().RevVanillaAIDisabled)
            {
                projectile.velocity /= 1.015525f;
            }
        }
    }
}
