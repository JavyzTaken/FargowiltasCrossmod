using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod("ThoriumMod")]
    public class GraniteCore : ModProjectile
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
            Projectile.timeLeft = 1200;
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Center = Projectile.position;
        }

        public override void AI()
        {
            Projectile.velocity = Projectile.oldVelocity;
            Projectile.frame = (int)MathF.Floor(Projectile.timeLeft / 15) % 3;

            var DLCPlayer = Main.player[Projectile.owner].GetModPlayer<CrossplayerThorium>();

            if (!DLCPlayer.GraniteCores.Contains(Projectile.whoAmI) || DLCPlayer.Player.dead)
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.ai[0] < 0) return;

            if (!Main.projectile[(int)Projectile.ai[0]].active || Main.projectile[(int)Projectile.ai[0]].type != Projectile.type)
            {
                Projectile.Kill();
                return;
            }

            Vector2 nextCorePos = Main.projectile[(int)Projectile.ai[0]].Center;
            float DistanceTo = Projectile.Center.Distance(nextCorePos);

            if (DistanceTo <= LinkedCoreMaxDist)
            {
                float DustIncr = 1 / (DistanceTo / 64);

                for (float i = DustIncr; i < 1; i += DustIncr)
                {
                    float salt = Main.rand.NextFloat(-DustIncr, 0);
                    Vector2 dustPos = Projectile.Center * (i + salt) + nextCorePos * (1 - i - salt);
                    Dust.NewDustDirect(dustPos, 0, 0, DustID.Electric).noGravity = true;
                }
            }
        }

        const float LinkedCoreMaxDist = 1024;
        public override void Kill(int timeLeft)
        {
            Projectile.friendly = true;
            Projectile.damage = 50;

            if (Projectile.owner == Main.myPlayer)
            {
                var DLCPlayer = Main.player[Projectile.owner].GetModPlayer<CrossplayerThorium>();
                Projectile.damage += 15 * DLCPlayer.GraniteCores.Count;
                DLCPlayer.GraniteCores.Remove(Projectile.whoAmI);
            }

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
