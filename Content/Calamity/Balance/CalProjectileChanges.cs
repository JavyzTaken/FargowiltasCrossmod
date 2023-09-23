using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Systems;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.ModPlayers;
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
        public override void SetDefaults(Projectile entity)
        {
            if (entity.ModProjectile != null && entity.ModProjectile is BaseLaserbeamProjectile)
            {
                entity.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCheck =
                Projectile =>
                {
                    float num6 = 0f;
                    if (entity.ModProjectile.CanDamage() != false && Collision.CheckAABBvLineCollision(Main.LocalPlayer.Hitbox.TopLeft(), Main.LocalPlayer.Hitbox.Size(), Projectile.Center,
                        Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale + Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().GrazeRadius * 2f + Player.defaultHeight, ref num6))
                    {
                        return true;
                    }
                    return false;
                };
                entity.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
            }
            if (new List<int>() {ModContent.ProjectileType<Flarenado>(), ModContent.ProjectileType<BrimstoneMonster>(), ModContent.ProjectileType<YharonBulletHellVortex>(),
            ModContent.ProjectileType<OldDukeVortex>(), ModContent.ProjectileType<Infernado>(),ModContent.ProjectileType<Infernado2>(),ModContent.ProjectileType<InfernadoRevenge>(),
            ModContent.ProjectileType<SkyFlareRevenge>(), ModContent.ProjectileType<ProvidenceCrystal>(), ModContent.ProjectileType<AresGaussNukeProjectileBoom>(),}.Contains(entity.type))
            {
                entity.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
            }
        }
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
                if (BossRushEvent.BossRushActive && projectile.hostile && projectile.damage > 500)
                {
                    projectile.damage = 500;
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
