using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GraniteExplosion : ModProjectile
    {
        public override string Texture => "ThoriumMod/Projectiles/Boss/GraniteCharge";
        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.damage = 32;
            Projectile.timeLeft = 6;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ThoriumMod.Buffs.GraniteSurge>(), 240);
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = Projectile.ai[0];
            Projectile.Center = Projectile.position;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.friendly = true;
            Projectile.damage = 50;

            Projectile.Damage();

            for (int i = 0; i < 16; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center + Vector2.UnitY.RotatedByRandom(MathF.PI) * (float)Main.rand.NextDouble() * 16, 40, 40, DustID.Granite, 0f, 0f, 100, default, 1.5f);
                dust.noGravity = true;
            }
            for (int i = 0; i < 16; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center + Vector2.UnitY.RotatedByRandom(MathF.PI) * (float)Main.rand.NextDouble() * 32, 40, 40, DustID.Electric, 0f, 0f, 100, default, 1.5f);
                dust.noGravity = true;
            }
        }
    }
}
