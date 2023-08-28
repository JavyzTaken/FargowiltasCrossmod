using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatBuffs;

using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AbyssTentacle : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.ShadowFlame;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.MaxUpdates = 3;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Generic;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 600);
            if (Projectile.owner != -1)
            {
                int healAmount = damageDone / 10;
                if (healAmount > 20) healAmount = 50;
                Main.player[Projectile.owner].HealEffect(damageDone / 50);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                if (Math.Abs(Projectile.velocity.X) < 1f)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                if (Math.Abs(Projectile.velocity.Y) < 1f)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            return false;
        }
        //copy pasted vanilla shadowflame hexdoll ai but different dust
        public override void AI()
        {
            Vector2 center13 = Projectile.Center;
            Projectile.scale = 1f - Projectile.localAI[0];
            Projectile.width = (int)(20f * Projectile.scale);
            Projectile.height = Projectile.width;
            Projectile.position.X = center13.X - Projectile.width / 2;
            Projectile.position.Y = center13.Y - Projectile.height / 2;

            ref float ptr = ref Projectile.localAI[0];

            if (Projectile.localAI[0] < 0.1)
            {
                ptr = ref Projectile.localAI[0];
                ptr += 0.01f;
            }
            else
            {
                ptr = ref Projectile.localAI[0];
                ptr += 0.025f;
            }
            if (Projectile.localAI[0] >= 0.95f)
            {
                Projectile.Kill();
            }
            ptr = ref Projectile.velocity.X;
            ptr += Projectile.ai[0] * 1.5f;
            ptr = ref Projectile.velocity.Y;
            ptr += Projectile.ai[1] * 1.5f;
            float velocityLimit = 16f;
            if (Projectile.owner != -1)
            {
                if (Main.player[Projectile.owner].HasBuff(ModContent.BuffType<AbyssalMadness>()))
                {
                    velocityLimit = 20f;
                }
            }
            if (Projectile.velocity.Length() > velocityLimit)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= velocityLimit;
            }
            ptr = ref Projectile.ai[0];
            ptr *= 1.05f;
            ptr = ref Projectile.ai[1];
            ptr *= 1.05f;
            if (Projectile.scale < 1f)
            {
                int i = 0;
                while (i < Projectile.scale * 10f)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.BlueTorch, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(170, 170, 170), 2);
                    dust.position = (dust.position + Projectile.Center) / 2f;
                    dust.noGravity = true;
                    dust.noLight = true;
                    Dust dust2 = dust;
                    dust2.velocity *= 0.1f;
                    dust2 = dust;
                    dust2.velocity -= Projectile.velocity * (1.3f - Projectile.scale);
                    dust.fadeIn = 100 + Projectile.owner;
                    dust2 = dust;
                    dust2.scale += Projectile.scale * 0.75f;
                    int num3 = i;
                    i = num3 + 1;
                }
                return;
            }
        }
    }
}
