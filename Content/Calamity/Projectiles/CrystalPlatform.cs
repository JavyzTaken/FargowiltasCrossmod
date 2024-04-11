using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    public class CrystalPlatform : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_" + NPCID.DD2EterniaCrystal;
        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.width = 58;
            Projectile.height = 44;
            Projectile.tileCollide = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void AI()
        {
            foreach (Player player in Main.player)
            {
                if (player.velocity.Y > 0 && player.Hitbox.Intersects(Projectile.Hitbox) && !player.justJumped && !player.controlDown && !player.GoingDownWithGrapple && player.oldPosition.Y + player.height - 5 < Projectile.Top.Y)
                {
                    player.position.Y = Projectile.position.Y - player.height + 2;
                    player.velocity.Y = 0;
                    if (Math.Abs(player.velocity.X) < 0.01f)
                    {
                        player.legFrame.Y = 0;
                        player.legFrameCounter = 0;
                    }
                    player.wingFrame = 0;
                    player.wingFrameCounter = 0;
                    player.bodyFrame.Y = 0;
                    player.bodyFrameCounter = 0;
                }
            }
        }
    }
}
