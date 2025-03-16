using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Fargowiltas.NPCs;
using Fargowiltas;
using FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCProjectileChanges : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override void SetDefaults(Projectile entity)
        {
            if (entity.ModProjectile != null && entity.ModProjectile is BaseLaserbeamProjectile)
            {
                entity.FargoSouls().GrazeCheck =
                Projectile =>
                {
                    float num6 = 0f;
                    if (entity.ModProjectile.CanDamage() != false && Collision.CheckAABBvLineCollision(Main.LocalPlayer.Hitbox.TopLeft(), Main.LocalPlayer.Hitbox.Size(), Projectile.Center,
                        Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale + Main.LocalPlayer.FargoSouls().GrazeRadius * 2f + Player.defaultHeight, ref num6))
                    {
                        return true;
                    }
                    return false;
                };
                entity.FargoSouls().DeletionImmuneRank = 1;
            }

            //if (entity.ModProjectile != null && entity.ModProjectile.Mod == ModCompatibility.Calamity.Mod) // Global disables on all Calamity projectiles. Currently: Adamantite Enchantment
            //    entity.FargoSouls().CanSplit = false;


            if (new List<int>() {ModContent.ProjectileType<Flarenado>(), ModContent.ProjectileType<BrimstoneMonster>(), ModContent.ProjectileType<YharonBulletHellVortex>(),
            ModContent.ProjectileType<OldDukeVortex>(), ModContent.ProjectileType<Infernado>(),ModContent.ProjectileType<Infernado2>(),ModContent.ProjectileType<InfernadoRevenge>(),
            ModContent.ProjectileType<SkyFlareRevenge>(), ModContent.ProjectileType<ProvidenceCrystal>(), ModContent.ProjectileType<AresGaussNukeProjectileBoom>(),}.Contains(entity.type))
            {
                entity.FargoSouls().DeletionImmuneRank = 2;
            }
            if (BossRushEvent.BossRushActive && new List<int> { ModContent.ProjectileType<DeviSparklingLove>(), ModContent.ProjectileType<DeviBigDeathray>(), ModContent.ProjectileType<PlanteraTentacle>(), ProjectileID.PhantasmalDeathray }.Contains(entity.type))
            {
                entity.extraUpdates += 1;

            }
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.type == ModContent.ProjectileType<RainExplosion>() && source is EntitySource_Parent parent && parent.Entity is Projectile parentProj && parentProj.GetGlobalProjectile<CalDLCProjectileChanges>().Ricoshot)
            {
                projectile.hostile = false;
                projectile.friendly = true;
            }

            if (projectile.type == ModContent.ProjectileType<SlimeBall>() && !Main.player.Any(p => p.active && p.FargoSouls() != null && p.FargoSouls().SupremeDeathbringerFairy))
            {
                if (projectile.ModProjectile != null)
                {
                    typeof(SlimeBall).GetField("oil", LumUtils.UniversalBindingFlags).SetValue(projectile.ModProjectile, false);
                }
            }
            

        }
        public bool Ricoshot = false;
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override bool PreAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (projectile.type == ModContent.ProjectileType<RainLightning>() || projectile.type == ModContent.ProjectileType<CursedLightning>())
            {
                if (Main.projectile.FirstOrDefault(p => p.TypeAlive(ModContent.ProjectileType<RicoshotCoin>()) && projectile.Colliding(projectile.Hitbox, p.Hitbox)) is Projectile coin && coin != null)
                {
                    
                    int n = FargoSoulsUtil.FindClosestHostileNPC(projectile.Center, 1000, true, true);
                    if (n.IsWithinBounds(Main.maxNPCs))
                    {
                        NPC npc = Main.npc[n];
                        if (npc.Alive())
                        {
                            
                            projectile.velocity = projectile.DirectionTo(npc.Center) * projectile.velocity.Length() * 2;
                        }
                        else
                        {
                            projectile.velocity = Main.rand.NextVector2Unit() * projectile.velocity.Length() * 2;
                        }
                        SoundEngine.PlaySound(new("CalamityMod/Sounds/Custom/UltrablingHit") { PitchVariance = 0.5f }, projectile.Center);
                        projectile.ai[0] = projectile.velocity.ToRotation();
                        projectile.damage *= Ricoshot ? 2 : 10;
                        projectile.hostile = false;
                        projectile.friendly = true;
                        Main.player[coin.owner].Calamity().GeneralScreenShakePower = 10;
                        coin.Kill();
                        Ricoshot = true;
                    }
                }
            }
            if (Fargowiltas.Fargowiltas.SwarmActive && Fargowiltas.Fargowiltas.SwarmItemsUsed >= 1 && projectile.hostile && projectile.damage > 200)
            {
                projectile.damage = FargoSoulsUtil.ScaledProjectileDamage(400);
            }
            #region Balance Changes
            //add defense damage to fargo enemies. setting this in SetDefaults crashes the game for some reason
            if (projectile.ModProjectile != null)
            {
                bool defenseDamage = false;

                if (projectile.ModProjectile is BaseDeathray)
                    defenseDamage = true;
                if (CalDLCSets.Projectiles.DefenseDamage[projectile.type])
                    defenseDamage = true;
                if ((EModeGlobalNPC.abomBoss.IsWithinBounds(Main.maxNPCs) && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<AbomBoss>())) ||
                    (EModeGlobalNPC.mutantBoss.IsWithinBounds(Main.maxNPCs) && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>())))
                    defenseDamage = true;

                if (defenseDamage)
                    projectile.Calamity().DealsDefenseDamage = true;
            }
            if (BossRushEvent.BossRushActive && projectile.hostile && projectile.damage < 100 && projectile.damage != 0)
            {
                projectile.damage = 100;
            }
            if (BossRushEvent.BossRushActive && projectile.hostile && projectile.damage > 100 && NPC.AnyNPCs(NPCID.HallowBoss))
            {
                projectile.damage = 100;
            }
            return true;
            #endregion
        }
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (CalDLCSets.Projectiles.MultipartShredder[projectile.type] && target.type == ModContent.NPCType<DarkEnergy>())
            {
                modifiers.FinalDamage *= 0.2f;
            }

            if (projectile.type == ModContent.ProjectileType<BlushieStaffProj>())
                modifiers.FinalDamage *= 0.7f;
            if (CalDLCSets.Projectiles.EternityBookProj[projectile.type] || (projectile.type == ModContent.ProjectileType<DirectStrike>() && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<EternityBook>()] > 0))
                modifiers.FinalDamage *= 0.4f;
            if (CalDLCSets.Projectiles.AngelAllianceProj[projectile.type])
            {
                modifiers.FinalDamage *= 0.2f;
            }
            if (projectile.type == ModContent.ProjectileType<AndromedaDeathRay>())
            {
                modifiers.FinalDamage *= 0.45f;
            }
            if (projectile.type == ModContent.ProjectileType<AndromedaRegislash>())
            {
                modifiers.FinalDamage *= 0.8f;
            }

            if (CalDLCSets.Projectiles.ProfanedCrystalProj[projectile.type] && Main.player[projectile.owner].Calamity().profanedCrystal)
            {
                modifiers.FinalDamage *= 0.4f;
                for (int i = 0; i < Main.player[projectile.owner].ownedProjectileCounts.Length; i++)
                {
                    if (ContentSamples.ProjectilesByType[i].minionSlots > 0 && Main.player[projectile.owner].ownedProjectileCounts[i] > 0)
                    {
                        modifiers.FinalDamage *= 0.3f;
                    }
                }
            }

        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            /*
            if (projectile.type == ModContent.ProjectileType<FragmentRitual>())
            {
                EDeathMLCore ml = Main.npc[(int)projectile.ai[1]].GetGlobalNPC<EDeathMLCore>();
                MoonLordCore ml2 = Main.npc[(int)projectile.ai[1]].GetGlobalNPC<MoonLordCore>();
                if (ml != null && ml2 != null && ml.roguePhase == true && ml2.VulnerabilityState == 4)
                {
                    Texture2D texture2D13 = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/MeldConstruct").Value;
                    int num156 = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/MeldConstruct").Value.Height; //ypos of lower right corner of sprite to draw
                    int y3 = 0; //ypos of upper left corner of sprite to draw
                    Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
                    Vector2 origin2 = rectangle.Size() / 2f;

                    Color color26 = projectile.GetAlpha(lightColor);

                    const int max = 32;
                    for (int x = 0; x < max; x++)
                    {
                        if (x < projectile.localAI[0])
                            continue;
                        Vector2 drawOffset = new(600 * projectile.scale / 2f, 0);//.RotatedBy(Projectile.ai[0]);
                        drawOffset = drawOffset.RotatedBy(2f * Math.PI / max * (x + 1) - Math.PI / 2);
                        Main.EntitySpriteDraw(texture2D13, projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0);
                    }
                    return false;
                }
            }
            */
            return base.PreDraw(projectile, ref lightColor);
        }
        public override void AI(Projectile projectile)
        {
            if (CalDLCConfig.Instance.EternityPriorityOverRev)
            {
                switch (projectile.type)
                {
                    case ProjectileID.HallowBossLastingRainbow:
                        {
                            if ((CalamityWorld.revenge || BossRushEvent.BossRushActive) && projectile.timeLeft > 570 && CalDLCWorldSavingSystem.E_EternityRev)
                            {
                                projectile.velocity /= 1.015525f;
                            }
                        }
                        break;
                    case ProjectileID.CultistBossIceMist:
                        {
                            if (CalDLCWorldSavingSystem.EternityDeath && projectile.ai[1] == 1)
                            {
                                int p = Player.FindClosest(projectile.position, projectile.width, projectile.height);
                                //projectile.ai[1] = 1;

                                if (p >= 0)
                                {
                                    if (projectile.velocity.Length() < 10)
                                    {
                                        projectile.velocity *= 1.1f;
                                    }
                                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0).RotatedBy(projectile.velocity.ToRotation().AngleTowards(projectile.AngleTo(Main.player[p].Center), 0.04f));
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
