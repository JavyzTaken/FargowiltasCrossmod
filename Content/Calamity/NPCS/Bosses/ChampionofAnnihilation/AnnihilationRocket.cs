using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofAnnihilation
{
    [JITWhenModsEnabled("CalamityMod")]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AnnihilationRocket : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/MiniRocket";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Main.projFrames[Type] = 4;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.timeLeft = Main.rand.Next(75, 100);
            Projectile.scale = 2;
        }
        public override void Kill(int timeLeft)
        {
            Vector2 pos = Projectile.Center + new Vector2(Main.rand.Next(-100, 100), -1200);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<AnnihilationLaser>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(400), 0, Main.myPlayer, 0, pos.AngleTo(Projectile.Center) - MathHelper.ToRadians(90));
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 50; i++)
            {
                Dust.NewDustDirect(Projectile.Center + new Vector2(0, Main.rand.Next(0, 100)).RotateRandom(2 * Math.PI), 0, 0, DustID.Torch, Scale: 2).noGravity = true;
                Dust.NewDustDirect(Projectile.Center + new Vector2(0, Main.rand.Next(0, 100)).RotateRandom(2 * Math.PI), 0, 0, DustID.Smoke, Scale: 1).noGravity = true;
            }
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 1 && Projectile.timeLeft > 30)
            {
                Projectile.timeLeft = 30;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Dust dust = Dust.NewDustDirect(Projectile.position, 1, 1, DustID.Torch, Scale: 2);
            Dust dust2 = Dust.NewDustDirect(Projectile.position, 1, 1, DustID.Smoke, Scale: 1);
            dust.noGravity = true;
            dust.velocity /= 3;
            dust2.velocity /= 3;
            dust2.noGravity = true;
            if (Projectile.timeLeft < 60 && Projectile.ai[0] != 1)
            {
                Projectile.velocity /= 1.05f;
            }
        }
    }
}
