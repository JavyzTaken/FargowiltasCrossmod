using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Projectiles.Magic;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod("CalamityMod")]
    public class PrisMissile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Prismatic Missile");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ai[0] = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 pos = target.Center + new Vector2(Main.rand.Next(-100, 100), -800);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, new Vector2(0, 1).RotatedBy(pos.AngleFrom(target.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20))) + MathHelper.PiOver2), ModContent.ProjectileType<DeathhailBeam>(), 300, 0, Main.myPlayer, 0);
                Projectile.ai[0] = target.whoAmI; Projectile.ai[1] = 5;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation();
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, Scale: 1.5f).noGravity = true;
            if (Projectile.timeLeft == 5 && Projectile.penetrate > 1)
            {
                Projectile.penetrate = 1;
            }
            if (Projectile.penetrate == 1)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust s = Dust.NewDustDirect(Projectile.Center + new Vector2(0, Main.rand.Next(0, 100)).RotatedByRandom(MathHelper.TwoPi), 0, 0, DustID.Smoke);
                    Dust d = Dust.NewDustDirect(Projectile.Center + new Vector2(0, Main.rand.Next(0, 100)).RotatedByRandom(MathHelper.TwoPi), 0, 0, DustID.DemonTorch);
                    Dust d2 = Dust.NewDustDirect(Projectile.Center + new Vector2(0, Main.rand.Next(0, 100)).RotatedByRandom(MathHelper.TwoPi), 0, 0, DustID.DemonTorch);
                    d.noGravity = true;
                    d.scale = 2;
                    d2.noGravity = true;
                    d2.scale = 2;
                    s.noGravity = true;
                    s.scale = 2;
                }

                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                Vector2 center = Projectile.Center;
                Projectile.width *= 5;
                Projectile.height *= 5;
                Projectile.Center = center;
                Projectile.penetrate = -1;
                Projectile.timeLeft = 120;
                Projectile.alpha = 255;
                Projectile.velocity *= 0;
            }
            if (Projectile.penetrate == -1 && Projectile.timeLeft % 5 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                NPC npc = Projectile.FindTargetWithinRange(500);
                if (npc != null) Projectile.ai[0] = npc.whoAmI;
                Vector2 pos = Projectile.Center + new Vector2(Main.rand.Next(-100, 100), -800);
                if (Projectile.ai[0] != -1 && Main.npc[(int)Projectile.ai[0]].active)
                {
                    pos = Main.npc[(int)Projectile.ai[0]].Center + new Vector2(Main.rand.Next(-100, 100), -800);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, new Vector2(0, 1).RotatedBy(pos.AngleFrom(Main.npc[(int)Projectile.ai[0]].Center + new Vector2(Main.rand.Next(-30, 30), Main.rand.Next(-30, 30))) + MathHelper.PiOver2), ModContent.ProjectileType<DeathhailBeam>(), 300, 0, Main.myPlayer, 0);
                }
                else
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, new Vector2(0, 1).RotatedBy(pos.AngleFrom(Projectile.Center + new Vector2(Main.rand.Next(-30, 30), Main.rand.Next(-30, 30))) + MathHelper.PiOver2), ModContent.ProjectileType<DeathhailBeam>(), 300, 0, Main.myPlayer, 0);
            }
            if (Projectile.penetrate > 1)
            {
                NPC npc = Projectile.FindTargetWithinRange(1000);
                int n = -1;
                if (npc != null)
                {
                    n = npc.whoAmI;
                }
                if (n != -1 && n != Projectile.ai[0])
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Main.npc[n].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20, 0.03f);
                }
                else
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity.SafeNormalize(Vector2.Zero) * 20, 0.03f);
                }
                if (Projectile.ai[1] > 0)
                {
                    Projectile.ai[1]--;
                    Vector2 pos = Projectile.Center + new Vector2(Main.rand.Next(-100, 100), -800);
                    SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, new Vector2(0, 1).RotatedBy(pos.AngleFrom(Projectile.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20))) + MathHelper.PiOver2), ModContent.ProjectileType<DeathhailBeam>(), 300, 0, Main.myPlayer, 0);
                }
                if (Projectile.ai[1] == 0)
                {
                    Projectile.ai[0] = -1;
                }
            }
        }

        public override bool? CanDamage()
        {
            if (Projectile.penetrate < 0 && Projectile.timeLeft < 117) return false;
            return base.CanDamage();
        }
        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
        }
    }
}