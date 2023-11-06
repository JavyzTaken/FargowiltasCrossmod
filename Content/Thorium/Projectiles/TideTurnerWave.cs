using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)] 
    public class TideTurnerWave : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Sharknado;
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        //public override void ModifyDamageHitbox(ref Rectangle hitbox)
        //{
        //    (hitbox.Width, hitbox.Height) = (hitbox.Height, hitbox.Width);
        //}

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = Projectile.ai[0] / 6;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.frame = (int)Projectile.ai[0] % 6;
        }

        public override void PostAI()
        {
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 0 && Projectile.ai[1] == 0f)
            {
                int timeAlive = 600 - Projectile.timeLeft;
                if ((Projectile.velocity * timeAlive).Length() > (Projectile.height / 2) * ((2 * Projectile.ai[0] - 1) / 6))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                                             Projectile.Center - (Vector2.Normalize(Projectile.velocity) * (Projectile.height / 2) * ((2 * Projectile.ai[0] - 1) / 6)),
                                             Projectile.velocity,
                                             Type,
                                             Projectile.damage,
                                             3f,
                                             Projectile.owner,
                                             Projectile.ai[0] - 1,
                                             0f);
                    Projectile.ai[1] = 1f;
                }
            }


            Projectile.position += Vector2.Normalize(Projectile.velocity) * Projectile.width * 0.0025f * Projectile.ai[0];
            Projectile.scale += 0.01f;
            Projectile.velocity *= 1.0025f;
        }
    }
}