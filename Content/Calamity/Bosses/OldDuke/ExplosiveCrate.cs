﻿using CalamityMod.Sounds;
using FargowiltasCrossmod.Core;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

[JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
[ExtendsFromMod(ModCompatibility.Calamity.Name)]
public class ExplosiveCrate : ModProjectile
{
    /// <summary>
    /// How long this crate has existed for, in frames.
    /// </summary>
    public ref float Time => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 6;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 42;
        Projectile.height = 42;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 360;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        Projectile.frame = Projectile.identity % Main.projFrames[Type];

        // Explode if a player is being collided with.
        Player player = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
        if (Projectile.Hitbox.Intersects(player.Hitbox))
        {
            Projectile.Kill();
            ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 1.5f);
        }

        AquaticUrchinProjectile.StandardFlyMotion(Projectile, (int)Time);
        Time++;
    }

    public override bool? CanDamage() => Projectile.scale >= 1f;

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(CommonCalamitySounds.PlagueBoomSound with { MaxInstances = 0 }, Projectile.Center);

        for (int i = 0; i < 32; i++)
        {
            Dust spark = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20f, 20f), Main.rand.NextBool() ? 31 : 6);
            spark.velocity = Main.rand.NextVector2Circular(24f, 6f) - Vector2.UnitY * Main.rand.NextFloat(6f, 33f);
            spark.scale = Main.rand.NextFloat(0.75f, 1.3f);
        }

        for (int i = 0; i < 25; i++)
        {
            float fireScale = Main.rand.NextFloat(0.4f, 0.7f);
            OldDukeFireParticleSystemManager.ParticleSystem.CreateNew(Projectile.Center, Main.rand.NextVector2Circular(20f, 20f), new Vector2(70f, 100f) * fireScale, Color.Orange);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D glowmask = ModContent.Request<Texture2D>($"{Texture}Glow").Value;
        LumUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, positionClumpInterpolant: 0.5f);
        LumUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], Color.White, positionClumpInterpolant: 0.5f, texture: glowmask);
        return false;
    }
}
