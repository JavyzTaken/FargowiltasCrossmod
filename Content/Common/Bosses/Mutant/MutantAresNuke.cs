using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Common.Bosses.Mutant
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MutantAresNuke : MutantNuke
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/AresGaussNukeProjectile";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = Main.projFrames[ModContent.ProjectileType<AresGaussNukeProjectile>()];
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 1f;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            base.AI();
        }
        public override bool? CanDamage() => false;
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 60 * 5);
            base.OnHitPlayer(target, info);
        }
        public override void PostAI()
        {
            base.PostAI();
            Projectile.rotation = Projectile.rotation + MathHelper.Pi;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(AresGaussNuke.NukeExplosionSound with { MaxInstances = 0 }, Projectile.Center, null);
            if (Main.netMode != NetmodeID.Server)
            {
                Gore.NewGore(Projectile.GetSource_Death((string)null), Projectile.position, Projectile.velocity, ModCompatibility.Calamity.Mod.Find<ModGore>("AresGaussNuke1").Type, 1f);
                Gore.NewGore(Projectile.GetSource_Death((string)null), Projectile.position, Projectile.velocity, ModCompatibility.Calamity.Mod.Find<ModGore>("AresGaussNuke3").Type, 1f);
            }
            for (int i = 0; i < 3; i++)
            {
                Projectile explosion = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis((string)null), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AresGaussNukeProjectileBoom>(), 0, 0f, Main.myPlayer, 0f, 0f, 0f);
                if (explosion.whoAmI.WithinBounds(Main.maxProjectiles))
                {
                    explosion.ai[1] = 560f + (float)i * 90f;
                    explosion.localAI[1] = 0.25f;
                    explosion.Opacity = MathHelper.Lerp(0.18f, 0.6f, (float)i / 7f) + Utils.NextFloat(Main.rand, -0.08f, 0.08f);
                    explosion.netUpdate = true;
                }
            }
        }

    }
}
