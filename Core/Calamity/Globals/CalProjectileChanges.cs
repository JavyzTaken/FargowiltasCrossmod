using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Common.Systems;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalProjectileChanges : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void SetDefaults(Projectile entity)
        {
            if (entity.ModProjectile != null && entity.ModProjectile is BaseLaserbeamProjectile)
            {
                entity.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCheck =
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
                entity.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
            }
            if (new List<int>() {ModContent.ProjectileType<Flarenado>(), ModContent.ProjectileType<BrimstoneMonster>(), ModContent.ProjectileType<YharonBulletHellVortex>(),
            ModContent.ProjectileType<OldDukeVortex>(), ModContent.ProjectileType<Infernado>(),ModContent.ProjectileType<Infernado2>(),ModContent.ProjectileType<InfernadoRevenge>(),
            ModContent.ProjectileType<SkyFlareRevenge>(), ModContent.ProjectileType<ProvidenceCrystal>(), ModContent.ProjectileType<AresGaussNukeProjectileBoom>(),}.Contains(entity.type))
            {
                entity.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
            }
            if (BossRushEvent.BossRushActive && new List<int> { ModContent.ProjectileType<DeviSparklingLove>(), ModContent.ProjectileType<DeviBigDeathray>(), ModContent.ProjectileType<PlanteraTentacle>(), ProjectileID.PhantasmalDeathray }.Contains(entity.type))
            {
                entity.extraUpdates += 1;

            }
        }
        public static List<int> TungstenExclude = new List<int>
        {
            ModContent.ProjectileType<BladecrestOathswordProj>(),
            ModContent.ProjectileType<OldLordClaymoreProj>()
        };
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (TungstenExclude.Contains(projectile.type))
            {
                //projectile.FargoSouls().TungstenScale = 1;
                float scale = projectile.FargoSouls().TungstenScale;
                projectile.position = projectile.Center;
                projectile.width = (int)(projectile.width / scale);
                projectile.height = (int)(projectile.height / scale);
                projectile.Center = projectile.position;
                projectile.scale /= scale;
            }
            if (projectile.FargoSouls().TungstenScale != 1)
            {
                Player player = Main.player[projectile.owner];
                Item item = player.HeldItem;
                if (item != null && item.DamageType == ModContent.GetInstance<TrueMeleeDamageClass>() || item.DamageType == ModContent.GetInstance<TrueMeleeNoSpeedDamageClass>())
                {
                    float scale = CalItemBalance.TrueMeleeTungstenScaleNerf(player);
                    projectile.position = projectile.Center;
                    projectile.width = (int)(projectile.width / scale);
                    projectile.height = (int)(projectile.height / scale);
                    projectile.Center = projectile.position;
                    projectile.scale /= scale;
                }
            }

            if (DLCCalamityConfig.Instance.BalanceRework && projectile.type == ModContent.ProjectileType<SlimeBall>() && !Main.player.Any(p => p.active && p.FargoSouls() != null && p.FargoSouls().SupremeDeathbringerFairy))
            {
                if (projectile.ModProjectile != null)
                {
                    typeof(SlimeBall).GetField("oil", FargoSoulsUtil.UniversalBindingFlags).SetValue(projectile.ModProjectile, false);
                }
            }
        }
        public override bool PreAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (TungstenExclude.Contains(projectile.type))
            {
                //projectile.FargoSouls().TungstenScale = 1;
            }
            #region Balance Changes config
            if (ModContent.GetInstance<DLCCalamityConfig>().BalanceRework)
            {
                //add defense damage to fargo enemies. setting this in SetDefaults crashes the game for some reason
                if (projectile.ModProjectile != null)
                {
                    if (projectile.ModProjectile.Mod == ModCompatibility.SoulsMod.Mod && projectile.hostile)
                    {
                        ModCompatibility.Calamity.Mod.Call("SetDefenseDamageProjectile", projectile, true);
                    }
                }
                if (BossRushEvent.BossRushActive && projectile.hostile && projectile.damage < 75 && projectile.damage != 0)
                {
                    projectile.damage = 75;
                }
                if (BossRushEvent.BossRushActive && projectile.hostile && projectile.damage > 100 && NPC.AnyNPCs(NPCID.HallowBoss))
                {
                    projectile.damage = 100;
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
            if (DLCCalamityConfig.Instance.BalanceRework)
            {
                if (projectile.type == ModContent.ProjectileType<BlushieStaffProj>())
                    modifiers.FinalDamage *= 0.7f;
                if (projectile.type == ModContent.ProjectileType<EternityCircle>() || projectile.type == ModContent.ProjectileType<EternityCrystal>()
                    || projectile.type == ModContent.ProjectileType<EternityHex>() || projectile.type == ModContent.ProjectileType<EternityHoming>()
                    || projectile.type == ModContent.ProjectileType<DirectStrike>() && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<EternityBook>()] > 0)
                    modifiers.FinalDamage *= 0.4f;
                if (projectile.type == ModContent.ProjectileType<AngelBolt>() || projectile.type == ModContent.ProjectileType<AngelicAllianceArchangel>() ||
                    projectile.type == ModContent.ProjectileType<AngelOrb>() || projectile.type == ModContent.ProjectileType<AngelRay>())
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
                List<int> profanedCrystalProjs = new List<int>()
                {
                    ModContent.ProjectileType<ProfanedCrystalMageFireball>(),
                    ModContent.ProjectileType<ProfanedCrystalMageFireballSplit>(),
                    ModContent.ProjectileType<ProfanedCrystalMeleeSpear>(),
                    ModContent.ProjectileType<ProfanedCrystalRangedHuges>(),
                    ModContent.ProjectileType<ProfanedCrystalRangedSmalls>(),
                    ModContent.ProjectileType<ProfanedCrystalRogueShard>(),
                    ModContent.ProjectileType<ProfanedCrystalWhip>(),
                    ModContent.ProjectileType<MiniGuardianAttack>(),
                    ModContent.ProjectileType<MiniGuardianDefense>(),
                    ModContent.ProjectileType<MiniGuardianFireball>(),
                    ModContent.ProjectileType<MiniGuardianFireballSplit>(),
                    ModContent.ProjectileType<MiniGuardianHealer>(),
                    ModContent.ProjectileType<MiniGuardianHolyRay>(),
                    ModContent.ProjectileType<MiniGuardianRock>(),
                    ModContent.ProjectileType<MiniGuardianSpear>(),
                    ModContent.ProjectileType<MiniGuardianStars>(),
                };
                if (profanedCrystalProjs.Contains(projectile.type) && Main.player[projectile.owner].Calamity().profanedCrystal)
                {
                    modifiers.FinalDamage *= 0.4f;
                    for (int i = 0; i < Main.player[projectile.owner].ownedProjectileCounts.Length; i++)
                    {
                        if (ContentSamples.ProjectilesByType[i].minionSlots > 0 && Main.player[projectile.owner].ownedProjectileCounts[i] > 0)
                        {
                            modifiers.FinalDamage *= 0.3f;
                        }
                    }
                    //Main.NewText(Main.player[projectile.owner].Calamity().pscState);

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
            if (DLCCalamityConfig.Instance.EternityPriorityOverRev)
            {
                if (projectile.type == ProjectileID.HallowBossLastingRainbow && (CalamityWorld.revenge || BossRushEvent.BossRushActive) && projectile.timeLeft > 570 && DLCWorldSavingSystem.E_EternityRev)
                {
                    projectile.velocity /= 1.015525f;
                }
                if (projectile.type == ProjectileID.CultistBossIceMist && DLCWorldSavingSystem.EternityDeath && projectile.ai[1] == 1)
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
        }
    }
}
