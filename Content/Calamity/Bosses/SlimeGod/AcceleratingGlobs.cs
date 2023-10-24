using System;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    internal class AcceleratingGlobs
    {
        public static void AI(Projectile projectile, int var)
        {
            if (projectile.velocity.Length() < 16f)
            {
                float accel = 1.0225f;
                projectile.velocity *= accel;
            }
            else
            {
                projectile.velocity = Vector2.Normalize(projectile.velocity) * 16;
            }

            if (projectile.timeLeft < 60)
            {
                projectile.Opacity = MathHelper.Lerp(0f, 0.8f, (float)projectile.timeLeft / 60f);
            }

            if (Main.rand.NextBool())
            {
                Color crimson = var == 0 ? Color.Crimson : Color.Lavender;
                crimson.A = 150;
                int num2 = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 4, 0f, 0f, projectile.alpha, crimson);
                Main.dust[num2].noGravity = true;
            }

            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.05f;
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AcceleratingCrimulanGlob : UnstableCrimulanGlob
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/UnstableCrimulanGlob";
        public override void AI()
        {
            AcceleratingGlobs.AI(Projectile, 0);
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AcceleratingEbonianGlob : UnstableEbonianGlob
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/UnstableEbonianGlob";
        public override void AI()
        {
            AcceleratingGlobs.AI(Projectile, 1);
        }
    }
}
