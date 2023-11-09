using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class KluexOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Kluex Orb");
            Main.projFrames[Type] = 5;
        }

        int orbType => (int)Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.timeLeft = 360;
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 60;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override bool PreAI()
        {
            Projectile.frame = (Projectile.frameCounter++ / 10) % 5;

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
        }

        //public override bool PreDraw(ref Color lightColor)
        //{
        //    if (Projectile.position.X < 128 || Projectile.position.X > Main.tile.Width * 16 - 128 || Projectile.position.Y < 128 || Projectile.position.Y > Main.tile.Height * 16 - 128)
        //        return false;

        //    Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        //    int Yframe = Projectile.timeLeft <= 70 ? Projectile.timeLeft < 60 ? 2 : 1 : 0;
        //    Rectangle rect = new(orbType == StaffHeal ? 42 : 0, Yframe * 36, 40, 34);
        //    Vector2 origin = new Vector2(Projectile.width, Projectile.height) / 2f;
        //    Color drawColor = Projectile.GetAlpha(lightColor);
        //    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rect, drawColor, Projectile.rotation, origin, 0.5f, SpriteEffects.None, 0);
        //    return false;
        //}

        public override void OnKill(int timeLeft)
        {
            Vector2 targetPos = Vector2.Zero;
            int targetIndex = 0;
            float targetDist = 9211600f;
            for (int i = 0; i < Main.player.Length; i++)
            {
                //if (i == Projectile.owner) continue;
                Player target2 = Main.player[i];
                if (Projectile.Center.DistanceSQ(target2.Center) < targetDist)
                {
                    targetPos = target2.Center;
                    targetDist = Projectile.Center.DistanceSQ(targetPos);
                    targetIndex = i;
                }
            }

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.Center.DirectionTo(targetPos) * 12f, ModContent.ProjectileType<KluexBlast>(), Projectile.damage, 1f, Projectile.owner, Projectile.ai[0], targetIndex);
            }
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
            }
        }
    }

    public class KluexBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Kluex Orb");
            Main.projFrames[Type] = 4;
        }

        int orbType => (int)Projectile.ai[0];

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 9;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 3600;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        //public override bool PreDraw(ref Color lightColor)
        //{
        //    if (Projectile.position.X < 128 || Projectile.position.X > Main.tile.Width * 16 - 128 || Projectile.position.Y < 128 || Projectile.position.Y > Main.tile.Height * 16 - 128)
        //        return false;

        //    Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        //    Color drawColor = Projectile.GetAlpha(lightColor);
        //    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), texture.frame, drawColor, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);

        //    return false;
        //}

        public override void AI()
        {
            Projectile.frame = (Projectile.frameCounter++ / 10) % 4;
            Projectile.rotation = Projectile.velocity.ToRotation();
            base.AI();
        }
    }
}
