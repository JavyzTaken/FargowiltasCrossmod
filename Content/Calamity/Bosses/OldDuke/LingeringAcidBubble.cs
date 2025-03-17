using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

public class LingeringAcidBubble : ModProjectile
{
    /// <summary>
    /// How long this bubble has existed for, in frames.
    /// </summary>
    public ref float Time => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 7;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
        Projectile.scale = Main.rand?.NextFloat(0.85f, 1.4f) ?? 1f;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        Projectile.velocity *= 1.009f;
        Projectile.frame = (int)Time / 5 % Main.projFrames[Type];

        Time++;
    }

    public override void OnKill(int timeLeft)
    {
        // Pop!
        SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);

        for (int i = 0; i < 12; i++)
        {
            Dust bubble = Dust.NewDustPerfect(Projectile.Center, DustID.BubbleBurst_Green);
            bubble.velocity = Main.rand.NextVector2Circular(3f, 3f);
        }
    }

    public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

    public override bool? CanDamage() => Time >= 16f;
}
