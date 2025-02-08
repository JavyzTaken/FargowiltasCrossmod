using Terraria;
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
        Projectile.width = 28;
        Projectile.height = 28;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 210;
        Projectile.scale = Main.rand?.NextFloat(0.85f, 1.4f) ?? 1f;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        Projectile.velocity *= 1.01f;
        Projectile.frame = (int)Time / 5 % Main.projFrames[Type];

        Time++;
    }

    public override bool? CanDamage() => Time >= 16f;
}
