using CalamityMod.Dusts;
using FargowiltasCrossmod.Assets.Particles;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke;

public class TrasherProjectile : ModProjectile
{
    /// <summary>
    /// How long this trasher has existed for, in frames.
    /// </summary>
    public ref float Time => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 40;
        Projectile.height = 40;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
        CooldownSlot = ImmunityCooldownID.Bosses;
    }

    public override void AI()
    {
        AquaticUrchinProjectile.StandardFlyMotion(Projectile, (int)Time, false);

        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.spriteDirection = Projectile.velocity.X.NonZeroSign();
        if (Projectile.spriteDirection == -1)
            Projectile.rotation += MathHelper.Pi;

        Time++;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

        for (int i = 0; i < 24; i++)
        {
            Vector2 vomitVelocity = Main.rand.NextVector2Circular(9f, 15f);
            ModContent.GetInstance<BileMetaball>().CreateParticle(Projectile.Center, vomitVelocity, Main.rand.NextFloat(32f, 44f), Main.rand.NextFloat());
        }

        for (int i = 0; i < 15; i++)
        {
            Dust toxicDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.SulphurousSeaAcid, 0f, 0f, 100, default, 2f);
            toxicDust.velocity *= new Vector2(3f, 6f);

            if (Main.rand.NextBool())
            {
                toxicDust.scale = 0.5f;
                toxicDust.fadeIn = Main.rand.NextFloat(1f, 2f);
            }
        }

        if (Main.netMode != NetmodeID.Server)
        {
            Mod calamity = ModContent.GetInstance<CalamityMod.CalamityMod>();
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.2f, calamity.Find<ModGore>("Trasher").Type, Projectile.scale);
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.2f, calamity.Find<ModGore>("Trasher2").Type, Projectile.scale);
        }
    }

    public override bool? CanDamage() => Projectile.scale >= 1f;

    public override bool PreDraw(ref Color lightColor)
    {
        LumUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor);
        return false;
    }
}
