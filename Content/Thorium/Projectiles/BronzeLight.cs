using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DLCLightStrike : ModProjectile
    {
        public override string Texture => "ThoriumMod/Projectiles/LightStrike";
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] > 0f)
            {
                NPC nextTarget = FindNearestOtherTarget(target);

                Vector2 direction = nextTarget == null ? Main.rand.NextVector2CircularEdge(1, 1) : Projectile.Center.DirectionTo(nextTarget.Center);
                direction *= Projectile.velocity.Length();

                Projectile.ai[0]--;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction, Projectile.type, (int)(Projectile.damage * 0.8f), Projectile.knockBack * 0.8f, Projectile.owner, Projectile.ai[0], Projectile.ai[1]);
            }

            if (Projectile.ai[1] == 1f)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<GraniteExplosion>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0.5f);
            }

            //npc.immune[base.Projectile.owner] = 2;
        }

        NPC FindNearestOtherTarget(NPC ignore)
        {
            float bestDist = 640;
            float bestWithSurgeDist = 640;
            NPC best = null;
            NPC bestWithSurge = null;

            int surgeType = ModContent.BuffType<ThoriumMod.Buffs.GraniteSurge>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i == ignore.whoAmI || !Main.npc[i].active) continue;

                float newDist = Projectile.Center.Distance(Main.npc[i].Center);

                if (Main.npc[i].HasBuff(surgeType) && newDist < bestWithSurgeDist)
                {
                    bestWithSurge = Main.npc[i];
                    bestWithSurgeDist = newDist;
                }
                else
                {
                    if (newDist < bestDist)
                    {
                        best = Main.npc[i];
                        bestDist = newDist;
                    }
                }
            }

            if (best == null) return bestWithSurge;

            return best;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 3;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            //Projectile.extraUpdates = 6;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 30;
            AIType = 14;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color?(Color.White);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(base.Projectile.position, base.Projectile.width, base.Projectile.height, DustID.Teleporter, (float)Main.rand.Next(-6, 6), (float)Main.rand.Next(-6, 6), 125, default(Color), 1f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool PreAI()
        {
            DelegateMethods.v3_1 = new Vector3(0.6f, 1f, 1f) * 0.2f;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 10f, 8f, DelegateMethods.CastLightOpen);
            if (Projectile.alpha > 0)
            {
                SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
                Projectile.alpha = 0;
                Projectile.scale = 1.1f;
                //Projectile.frame = Main.rand.Next(14);
                float num111;
                num111 = 16f;
                for (int num112 = 0; (float)num112 < num111; num112++)
                {
                    Vector2 spinningpoint5;
                    spinningpoint5 = Vector2.UnitX * 0f;
                    spinningpoint5 += -Vector2.UnitY.RotatedBy((float)num112 * ((float)MathF.PI * 2f / num111)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(Projectile.velocity.ToRotation());
                    int num113;
                    num113 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Teleporter);
                    Main.dust[num113].scale = 1.5f;
                    Main.dust[num113].noGravity = true;
                    Main.dust[num113].position = Projectile.Center + spinningpoint5;
                    Main.dust[num113].velocity = spinningpoint5.SafeNormalize(Vector2.UnitY);
                }
            }

            return base.PreAI();
        }

        public override void PostAI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 10)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
        }
    }
}