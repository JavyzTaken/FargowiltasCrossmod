using CalamityMod.Particles;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.DesertScourge
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ScourgeSandstream : ModProjectile
    {
        public int Time = 0;
        public override string Texture => FargoSoulsUtil.EmptyTexture;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;

            Projectile.light = 1;
        }
        public override void AI()
        {

            Time++;
            // visuals
            if (Main.rand.NextBool(5))
            {
                MediumMistParticle SandCloud = new MediumMistParticle(Projectile.Center, (-Projectile.velocity * 0.2f).RotatedByRandom(0.2f), Color.Peru, Color.PeachPuff, MathHelper.Clamp(Main.rand.NextFloat(1.1f, 1.5f) - Time * 0.02f, 0.5f, 2), 120f, Main.rand.NextFloat(0.03f, -0.03f));
                GeneralParticleHandler.SpawnParticle(SandCloud);
            }
            for (int i = 0; i < 4; i++)
            {
                float DustArea = MathHelper.Clamp(3 - Time * 0.03f, 1, 3);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(DustArea, DustArea), Main.rand.NextBool(3) ? 216 : 32);
                dust.noGravity = true;
                dust.velocity = new Vector2(0.5f, 0.5f).RotatedByRandom(100) + Projectile.velocity * 0.3f;
                dust.scale = MathHelper.Clamp(Main.rand.NextFloat(1.4f, 1.9f) - Time * 0.01f, 0.9f, 1.5f);
            }

            if (Main.rand.NextBool(4))
            {
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 216 : 32);
                dust2.noGravity = false;
                dust2.velocity = new Vector2(0, Main.rand.NextFloat(1, 4));
                dust2.scale = Main.rand.NextFloat(0.3f, 0.5f);
                dust2.fadeIn = 0.7f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            float numberOfDusts = 20f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(Main.rand.NextFloat(1.5f, 5.5f), 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 9.1f));
                Vector2 velOffset = new Vector2(Main.rand.NextFloat(1.5f, 5.5f), 0).RotatedBy(rot * Main.rand.NextFloat(3.1f, 9.1f));
                MediumMistParticle SandCloud = new MediumMistParticle(Projectile.Center + offset, velOffset * Main.rand.NextFloat(1.5f, 3f), Color.Peru, Color.PeachPuff, Main.rand.NextFloat(0.9f, 1.2f), 160f, Main.rand.NextFloat(0.03f, -0.03f));
                GeneralParticleHandler.SpawnParticle(SandCloud);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool() ? 288 : 207, new Vector2(velOffset.X, velOffset.Y));
                dust.noGravity = false;
                dust.velocity = velOffset;
                dust.scale = Main.rand.NextFloat(1.2f, 1.6f);
            }
        }
    }
}
