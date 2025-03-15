using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimstoneFireball : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/BrimstoneHellfireball";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = false;
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.hostile = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            if (Projectile.Hitbox.Intersects(new Rectangle((int)Projectile.ai[0], (int)Projectile.ai[1], Player.defaultWidth, Player.defaultHeight)))
                Projectile.tileCollide = true;

            // Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 9)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 6)
                Projectile.frame = 0;

            // Fade in
            if (Projectile.alpha > 5)
                Projectile.alpha -= 15;
            if (Projectile.alpha < 5)
                Projectile.alpha = 5;

            // Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) - MathHelper.ToRadians(90f) * Projectile.direction;

            Movement();

            Lighting.AddLight(Projectile.Center, 0.5f, 0f, 0f);

            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SupremeCalamitas.HellblastSound with { Pitch = -0.1f, Volume = 4 }, Projectile.Center);
                Projectile.localAI[0] += 1f;
            }

            int brimDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 170, default, 1.1f);
            Main.dust[brimDust].noGravity = true;
            Main.dust[brimDust].velocity *= 0.5f;
            Main.dust[brimDust].velocity += Projectile.velocity * 0.1f;
        }
        public virtual void Movement()
        {
            if (Projectile.velocity.Length() < 16f)
                Projectile.velocity *= 1.01f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 50, 50, Projectile.alpha);
        }

        public override void OnKill(int timeLeft)
        {
            //if (Projectile.owner == Main.myPlayer)
            //    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HellfireExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 150);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.type == ModContent.NPCType<Cataclysm>() || target.type == ModContent.NPCType<Catastrophe>())
                return base.CanHitNPC(target);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FlatBonusDamage += 100;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.Kill();
        }
    }
}
