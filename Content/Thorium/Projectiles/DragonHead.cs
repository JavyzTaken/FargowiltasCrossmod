using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DragonHead : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 86;
            Projectile.height = 62;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public int HeadNum => (int)Projectile.ai[0];
        public int Timer => (int)Projectile.ai[1];

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.HasEffect<DragonEffect>())
            {
                return;
            }

            Projectile.timeLeft = 2;

            Vector2 relativeHome = HeadNum switch
            {
                0 => Vector2.UnitY * -50,
                1 => new(-50, -40),
                2 => new(50, -40),
                _ => Vector2.Zero
            };

            Projectile.Center = player.Center + relativeHome;
            Projectile.velocity = Vector2.Zero;
            if (!Main.rand.NextBool(10)) { 
                Projectile.ai[1]++;
                Projectile.frameCounter++;
            }


            NPC target = null;
            if (player.HasMinionAttackTargetNPC)
            {
                target = Main.npc[player.MinionAttackTargetNPC];
            }

            if (target == null || !target.active)
            {
                float targetDist = 16f * 128f;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].friendly)
                    {
                        float dist2 = Main.npc[i].Distance(Projectile.Center);
                        if (dist2 < targetDist)
                        {
                            target = Main.npc[i];
                            targetDist = dist2;
                        }
                    }
                }
            }

            if (target != null)
            {
                Vector2 targetDir = Projectile.Center.DirectionTo(target.Center);
                Projectile.rotation = targetDir.ToRotation();

                if (Timer > 16)
                {
                    Projectile.ai[1] = 0f;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Normalize(targetDir) * 16, ProjectileID.Flames, 30, 0.2f, Projectile.owner);
                    proj.friendly = true;
                    proj.hostile = false;
                }
            }
            else
            {
                Projectile.rotation = player.direction == 1 ? 0 : MathF.PI;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int currentFrame = (Projectile.frameCounter / 15) % 4;
            Rectangle rect = new(0, currentFrame * Projectile.height, Projectile.width, Projectile.height);
            SpriteEffects effect = MathF.Abs(Projectile.rotation) > MathHelper.PiOver2 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, rect, lightColor, Projectile.rotation, rect.Size() * 0.5f, 1f, effect);
            return false;
        }
    }
}