﻿using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Content.Calamity.Bosses.MoonLord;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Systems;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
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
            if (entity.type == ModContent.ProjectileType<DeviSparklingLove>())
            {
                entity.extraUpdates += 1;
            }
        }
        public override bool PreAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            /*
            //rogue projectiles affected by vortex jammed debuff
            if (!projectile.hostile && !projectile.trap && !projectile.npcProj)
            {
                if (modPlayer.Jammed && projectile.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()) && projectile.type != ProjectileID.ConfettiGun)
                {
                    Projectile.NewProjectile(Entity.InheritSource(projectile), projectile.Center, projectile.velocity, ProjectileID.ConfettiGun, 0, 0f, projectile.owner);
                    projectile.active = false;
                }

            }
            */
            #region Balance Changes config
            if (ModContent.GetInstance<Core.Calamity.DLCCalamityConfig>().BalanceRework)
            {
                //add defense damage to fargo enemies. setting this in SetDefaults crashes the game for some reason
                if (projectile.ModProjectile != null)
                {
                    if (projectile.ModProjectile.Mod == ModCompatibility.SoulsMod.Mod && projectile.hostile)
                    {
                        ModCompatibility.Calamity.Mod.Call("SetDefenseDamageProjectile", projectile, true);
                    }
                }
                if (BossRushEvent.BossRushActive && projectile.hostile && projectile.damage < 100 && projectile.damage != 0)
                {
                    projectile.damage = 100;
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
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
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
                        /*const int max = 4;
                        for (int i = 0; i < max; i++)
                        {
                            Color color27 = color26;
                            color27 *= (float)(max - i) / max;
                            Vector2 value4 = Projectile.Center + drawOffset.RotatedBy(-rotationPerTick * i);
                            float num165 = Projectile.rotation;
                            Main.EntitySpriteDraw(texture2D13, value4 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
                        }*/
                        Main.EntitySpriteDraw(texture2D13, projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0);
                    }
                    return false;
                }
            }
            return base.PreDraw(projectile, ref lightColor);
        }
        public override void AI(Projectile projectile)
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
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0).RotatedBy(Utils.AngleTowards(projectile.velocity.ToRotation(), projectile.AngleTo(Main.player[p].Center), 0.04f));
                }
            }
        }
    }
}
