using CalamityMod;
using CalamityMod.Events;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Balance
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalProjectileChanges : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private bool DefenseDamageSet = false;
        public override bool PreAI(Projectile projectile)
        {
            #region Balance Changes config
            if (ModContent.GetInstance<Core.Calamity.CalamityConfig>().BalanceChanges)
            {
                //add defense damage to fargo enemies. setting this in SetDefaults crashes the game for some reason
                if (projectile.ModProjectile != null)
                {
                    if (projectile.ModProjectile.Mod == ModLoader.GetMod(ModCompatibility.SoulsMod.Name) && projectile.hostile)
                    {
                        ModLoader.GetMod(ModCompatibility.Calamity.Name).Call("SetDefenseDamageProjectile", projectile, true);
                    }
                }
            }
            return true;
            #endregion
        }
        public override void AI(Projectile projectile)
        {
            if (projectile.type == 872 && (CalamityWorld.revenge || BossRushEvent.BossRushActive) && projectile.timeLeft > 570 && WorldSavingSystem.E_EternityRev)
            {
                projectile.velocity /= 1.015525f;
            }
        }
    }
}
