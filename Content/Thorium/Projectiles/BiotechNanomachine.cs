using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class BiotechNanomachine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 8;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.damage = 0;
        }

        public override void PostAI()
        {
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 74, Vector2.Zero);
            }

            if (++Projectile.frameCounter > 15)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                Projectile.frame %= 2;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (owner.active && !owner.dead)
            {
                Projectile.timeLeft = 2;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[0] == -1)
            {
                var thoriumOwner = owner.GetModPlayer<ThoriumMod.ThoriumPlayer>();
                if (thoriumOwner.pocketTarget != -1)
                {
                    if (Main.player[thoriumOwner.pocketTarget].statLife < Main.player[thoriumOwner.pocketTarget].statLifeMax)
                    {
                        Projectile.ai[0] = thoriumOwner.pocketTarget;
                    }
                }

                if (Projectile.ai[0] == -1)
                {
                    Projectile.ai[0] = FindPlayerNotOwner();
                }

                if (Projectile.ai[0] != -1) return;

                Projectile.penetrate = -1;

                float ownerDist = Projectile.Distance(owner.Center);
                if (ownerDist > 2000)
                {
                    Main.NewText("1");
                    Projectile.Center = owner.Center;
                }
                else if (ownerDist < 8f)
                {
                    Main.NewText("2");
                    Projectile.position += Main.rand.NextVector2CircularEdge(1, 1) * 8f ;
                }
                else if (ownerDist < 56)
                {
                    Main.NewText("3");
                    Projectile.velocity -= (owner.Center - Projectile.Center) / ownerDist;
                }
                else if (ownerDist > 72)
                {
                    Main.NewText("4");
                    Projectile.velocity += 2 * (owner.Center - Projectile.Center) / ownerDist;

                    //float dot = Vector2.Dot(Projectile.velocity, owner.Center - Projectile.Center);
                    //if (dot < 0f)
                    //{
                    //}
                }
                else
                {
                    Main.NewText("5");
                    Projectile.velocity = ((owner.Center - Projectile.Center) / ownerDist).RotatedBy(MathHelper.PiOver2);
                }
            }
            else
            {
                Player target = Main.player[(int)Projectile.ai[0]];
                if (!target.active || target.dead)
                {
                    Projectile.ai[0] = -1f;
                    return;
                }

                Projectile.penetrate = 1;
                Projectile.velocity += Projectile.DirectionTo(target.Center + target.velocity * 8f);

                Projectile.DLCHeal(5);
            }
        }

        int FindPlayerNotOwner()
        {
            float dist = 1600;
            int best = -1;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player other = Main.player[i];
                if (i == Projectile.owner || !other.active || other.dead || other.statLife >= other.statLifeMax) continue;

                float nextdist = Projectile.Distance(other.Center);
                if (nextdist < dist)
                {
                    best = i;
                    dist = nextdist;
                }
            }

            return best;
        }
    }
}