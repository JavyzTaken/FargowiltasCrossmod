using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Systems;
using FargowiltasSouls.Content.Projectiles.Masomode;
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
            if (ModContent.GetInstance<Core.Calamity.CalamityConfig>().BalanceRework)
            {
                //add defense damage to fargo enemies. setting this in SetDefaults crashes the game for some reason
                if (projectile.ModProjectile != null)
                {
                    if (projectile.ModProjectile.Mod == ModLoader.GetMod(ModCompatibility.SoulsMod.Name) && projectile.hostile)
                    {
                        ModLoader.GetMod(ModCompatibility.Calamity.Name).Call("SetDefenseDamageProjectile", projectile, true);
                    }
                }
                if (BossRushEvent.BossRushActive && projectile.hostile && projectile.damage < 225 && projectile.damage != 0)
                {
                    projectile.damage = 225;
                }
                if (BossRushEvent.BossRushActive && projectile.hostile && projectile.damage > 1000)
                {
                    projectile.damage = 1000;
                }
            }
            return true;
            #endregion
        }
        public static List<int> MultipartShredders = new List<int>
        {
            ModContent.ProjectileType<CelestialRuneFireball>()

        };
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (MultipartShredders.Contains(projectile.type) && target.type == ModContent.NPCType<DarkEnergy>())
            {
                modifiers.FinalDamage *= 0.2f;
            }
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
