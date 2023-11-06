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
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity / 2f * i, 74, Vector2.Zero);
                dust.scale /= 2;
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
                var thoriumOwner = owner.Thorium();
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
                    // too far, teleport
                    Projectile.Center = owner.Center;
                }
                else if (ownerDist < 8f)
                {
                    // too close, give random direction
                    Projectile.velocity += Main.rand.NextVector2CircularEdge(1, 1) * 8f ;
                }
                else if (ownerDist < 56)
                {
                    // closer than valid range, move away 
                    Projectile.velocity -= (owner.Center - Projectile.Center) / ownerDist;
                }
                else if (ownerDist > 72)
                {
                    // further than valid range, move closer
                    Projectile.velocity += 2 * (owner.Center - Projectile.Center) / ownerDist;

                    float dot = Vector2.Dot(Projectile.velocity, owner.Center - Projectile.Center);
                    if (dot < 0f)
                    {
                        Projectile.velocity *= 0.95f;
                    }
                }
                else
                {
                    // in valid range, orbit
                    Projectile.velocity += ((owner.Center - Projectile.Center) / ownerDist).RotatedBy(((Projectile.whoAmI % 2 * 2) - 1) * MathHelper.PiOver2);

                    float speed = Projectile.velocity.Length();
                    if (speed > 12)
                    {
                        Projectile.velocity *= (12 / speed);
                    }
                }
            }
            else
            {
                Player target = Main.player[(int)Projectile.ai[0]];
                if (!target.active || target.dead || Projectile.ai[0] == Projectile.owner)
                {
                    Projectile.ai[0] = -1f;
                    return;
                }

                Projectile.penetrate = 1;
                Projectile.velocity = Projectile.DirectionTo(target.Center + target.velocity * 8f);

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