using CalamityMod.NPCs.Cryogen;
using CalamityMod.Particles;
using FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PermafrostRitual : BaseArena
    {
        private const float realRotation = -MathHelper.Pi / 180f;

        //public override string Texture => "CalamityMod/Projectiles/Summon/IceClasperSummonProjectile"; //"CalamityMod/Projectiles/Magic/IceCluster";
        public PermafrostRitual() : base(realRotation, 1200f, ModContent.NPCType<PermafrostBoss>(), DustID.SnowflakeIce) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        protected override void Movement(NPC npc)
        {
            Projectile.velocity = npc.Center - Projectile.Center;

            Projectile.velocity /= 40f;

            if (npc.ai[2] == (float)PermafrostBoss.Attacks.PawCharge)
                Projectile.velocity /= 40f;

            rotationPerTick = realRotation;
        }

        public override void AI()
        {
            base.AI();
            Projectile.rotation -= 1f;

            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = Projectile.Center + rot.ToRotationVector2().RotatedBy(MathHelper.TwoPi * i / 10f) * (threshold * Projectile.scale / 2f + Main.rand.NextFloat(-50, 50));
                CalamityMod.Particles.Particle snowflake = new SnowflakeSparkle(pos, Projectile.velocity, Color.White, new Color(75, 177, 250), Main.rand.NextFloat(0.3f, 1.5f), 20, 0.5f);
                GeneralParticleHandler.SpawnParticle(snowflake);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor * Projectile.Opacity * (targetPlayer == Main.myPlayer ? 1f : 0.15f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Chilled, 60 * 2);
            target.AddBuff(BuffID.Frostburn, 60 * 2);
        }
    }
}