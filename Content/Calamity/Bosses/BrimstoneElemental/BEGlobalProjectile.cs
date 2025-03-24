using CalamityMod.Dusts;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Content.Calamity.Bosses.AquaticScourge;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BEGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int Timer = 0;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return (entity.type == ModContent.ProjectileType<BrimstoneBarrage>() || entity.type == ModContent.ProjectileType<BrimstoneHellfireball>() || entity.type == ModContent.ProjectileType<BrimstoneFireblast>() || entity.type == ModContent.ProjectileType<HellfireExplosion>());
        }
        public static bool EternityBrimmy => CalamityGlobalNPC.brimstoneElemental >= 0 && Main.npc[CalamityGlobalNPC.brimstoneElemental] is NPC n && n.type == ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>() && n.TryGetDLCBehavior(out BrimstoneEternity emode) && emode != null;
        public override void SetDefaults(Projectile proj)
        {
            if (EternityBrimmy)
            {
                if (proj.light < 1f)
                    proj.light = 1f;
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[projectile.type];
            if (EternityBrimmy) 
            {
                Vector2 frameSize = Vector2.Zero;
                if (projectile.type == ModContent.ProjectileType<BrimstoneBarrage>()) 
                {
                    frameSize = new Vector2(18, 44);
                }
                if (projectile.type == ModContent.ProjectileType<BrimstoneFireblast>())
                {
                    frameSize = new Vector2(36, 50);
                }
                if (projectile.type == ModContent.ProjectileType<BrimstoneHellfireball>())
                {
                    frameSize = new Vector2(t.Width(), t.Height() / 6);
                }

                DrawBlackBorder(t, projectile.Center, frameSize / 2, projectile.rotation, projectile.scale, offsetMult: 3, sourceRectangle: new Rectangle(0, (int)frameSize.Y * projectile.frame, (int)frameSize.X, (int)frameSize.Y));
            }
            return base.PreDraw(projectile, ref lightColor);
        }
        public override bool PreAI(Projectile projectile)
        {
            if (!EternityBrimmy)
                return true;
            int timer = WorldSavingSystem.MasochistModeReal ? 30 : 70;
            if (projectile.type == ModContent.ProjectileType<BrimstoneBarrage>() && ++Timer > timer)
            {
                if (projectile.velocity.Length() < 20)
                    projectile.velocity *= 1.02f;
            }
            if (projectile.type == ModContent.ProjectileType<BrimstoneHellfireball>())
            {
                if (projectile.Hitbox.Intersects(new Rectangle((int)projectile.ai[0], (int)projectile.ai[1], Player.defaultWidth, Player.defaultHeight)))
                    projectile.tileCollide = true;

                // Animation
                projectile.frameCounter++;
                if (projectile.frameCounter > 9)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 6)
                    projectile.frame = 0;

                // Fade in
                if (projectile.alpha > 5)
                    projectile.alpha -= 15;
                if (projectile.alpha < 5)
                    projectile.alpha = 5;

                // Rotation
                projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
                projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) - MathHelper.ToRadians(90f) * projectile.direction;

                if (projectile.velocity.Length() < 16f)
                    projectile.velocity *= 1.01f;

                Lighting.AddLight(projectile.Center, 0.5f, 0f, 0f);

                if (projectile.localAI[0] == 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item20, projectile.Center);
                    projectile.localAI[0] += 1f;
                }
                return false;
            }
            if (projectile.type == ModContent.ProjectileType<HellfireExplosion>())
            {
                Lighting.AddLight(projectile.Center, 0.75f, 0f, 0f);
                if (projectile.localAI[0] == 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item20, projectile.Center);
                    projectile.localAI[0] += 1f;
                }
                bool xflag = false;
                bool yflag = false;
                if (projectile.velocity.X < 0f && projectile.position.X < projectile.ai[0])
                {
                    xflag = true;
                }
                if (projectile.velocity.X > 0f && projectile.position.X > projectile.ai[0])
                {
                    xflag = true;
                }
                if (projectile.velocity.Y < 0f && projectile.position.Y < projectile.ai[1])
                {
                    yflag = true;
                }
                if (projectile.velocity.Y > 0f && projectile.position.Y > projectile.ai[1])
                {
                    yflag = true;
                }
                if (xflag && yflag)
                {
                    projectile.Kill();
                }
                float projTimer = 25f;
                if (projectile.ai[0] > 180f)
                {
                    projTimer -= (projectile.ai[0] - 180f) / 2f;
                }
                if (projTimer <= 0f)
                {
                    projTimer = 0f;
                    projectile.Kill();
                }
                projTimer *= 0.7f;
                projectile.ai[0] += 4f;
                int timerCounter = 0;
                while ((float)timerCounter < projTimer)
                {
                    float rando1 = (float)Main.rand.Next(-10, 11);
                    float rando2 = (float)Main.rand.Next(-10, 11);
                    float rando3 = (float)Main.rand.Next(3, 9);
                    float randoAdjuster = (float)Math.Sqrt((double)(rando1 * rando1 + rando2 * rando2));
                    randoAdjuster = rando3 / randoAdjuster;
                    rando1 *= randoAdjuster;
                    rando2 *= randoAdjuster;
                    int dust = Main.rand.NextBool() ? (int)CalamityDusts.Brimstone : 155;
                    int brimDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, dust, 0f, 0f, 100, default, 1.5f);
                    Main.dust[brimDust].noGravity = true;
                    Main.dust[brimDust].position.X = projectile.Center.X;
                    Main.dust[brimDust].position.Y = projectile.Center.Y;
                    Dust d = Main.dust[brimDust];
                    d.position.X += (float)Main.rand.Next(-10, 11);
                    d.position.Y += (float)Main.rand.Next(-10, 11);
                    Main.dust[brimDust].velocity.X = rando1;
                    Main.dust[brimDust].velocity.Y = rando2;
                    timerCounter++;
                }
                return false;
            }
            return base.PreAI(projectile);
        }
        public override void PostAI(Projectile projectile)
        {
            
        }
        public static void DrawBlackBorder(Asset<Texture2D> texture, Vector2 position, Vector2 origin, float rotation = 0, float scale = 1, SpriteEffects spriteEffects = SpriteEffects.None, int iterations = 12, float offsetMult = 1, Rectangle? sourceRectangle = null)
        {
            for (int j = 0; j < iterations; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * offsetMult;
                Color glowColor = new(40, 24, 48);
                glowColor *= 0.7f;

                Main.EntitySpriteDraw(texture.Value, position + afterimageOffset - Main.screenPosition, sourceRectangle, glowColor, rotation, origin, scale, spriteEffects);
            }
        }
    }
}
