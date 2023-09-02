using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod("ThoriumMod")]
    public class SpiritTrapperSpirit : ModProjectile
    {
        public override string Texture => "ThoriumMod/Items/Misc/SpiritDroplet";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = false;
            Projectile.friendly = true;
			Projectile.tileCollide = false;
		}

        public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			Lighting.AddLight(Projectile.Center, 0.3f, 0.4f, 0.7f);
			if (player.dead || !player.GetModPlayer<CrossplayerThorium>().SpiritTrapperEnch)
			{
				Projectile.Kill();
				return;
			}
			Projectile.timeLeft = 2;

			if (Math.Abs(Projectile.Center.X - player.Center.X) + Math.Abs(Projectile.Center.Y - player.Center.Y) > 1400)
			{
				Projectile.ai[0] = 1f;
			}
			float num536;
			num536 = 8f;
			if (base.Projectile.ai[0] == 1f)
			{
				num536 = 12f;
			}
			Vector2 vector2;
			vector2 = new Vector2(base.Projectile.position.X + (float)base.Projectile.width * 0.5f, base.Projectile.position.Y + (float)base.Projectile.height * 0.5f);
			float num537;
			num537 = player.Center.X - vector2.X;
			float num538;
			num538 = player.Center.Y - vector2.Y - 60f;
			float num539;
			num539 = (float)Math.Sqrt((double)(num537 * num537 + num538 * num538));
			if (num539 < 100f && base.Projectile.ai[0] == 1f && !Collision.SolidCollision(base.Projectile.position, base.Projectile.width, base.Projectile.height))
			{
				base.Projectile.ai[0] = 0f;
			}
			if (num539 > 2000f)
			{
				base.Projectile.position.X = player.Center.X - (float)(base.Projectile.width / 2);
				base.Projectile.position.Y = player.Center.Y - (float)(base.Projectile.width / 2);
			}
			if (num539 > 70f)
			{
				num539 = num536 / num539;
				num537 *= num539;
				num538 *= num539;
				base.Projectile.velocity.X = (base.Projectile.velocity.X * 20f + num537) / 21f;
				base.Projectile.velocity.Y = (base.Projectile.velocity.Y * 20f + num538) / 21f;
			}
			else
			{
				if (base.Projectile.velocity.X == 0f && base.Projectile.velocity.Y == 0f)
				{
					base.Projectile.velocity.X = -0.15f;
					base.Projectile.velocity.Y = -0.05f;
				}
				base.Projectile.velocity *= 1.01f;
			}
			base.Projectile.friendly = false;
			base.Projectile.rotation = base.Projectile.velocity.X * 0.05f;
			if ((double)Math.Abs(base.Projectile.velocity.X) > 0.2)
			{
				base.Projectile.spriteDirection = -base.Projectile.direction;
			}

			//if (base.Projectile.ai[1] == -1f)
			//{
			//	base.Projectile.ai[1] = 17f;
			//}
			//if (base.Projectile.ai[1] > 0f)
			//{
			//	base.Projectile.ai[1] -= 1f;
			//}
			//if (base.Projectile.ai[1] == 0f)
			//{
			//	base.Projectile.friendly = true;
			//	float num540;
			//	num540 = 8f;
			//	Vector2 vector3;
			//	vector3 = new Vector2(base.Projectile.position.X + (float)base.Projectile.width * 0.5f, base.Projectile.position.Y + (float)base.Projectile.height * 0.5f);
			//	float num541;
			//	num541 = Projectile.position.X - vector3.X;
			//	float num542;
			//	num542 = Projectile.position.Y - vector3.Y;
			//	float num543;
			//	num543 = (float)Math.Sqrt((double)(num541 * num541 + num542 * num542));
			//	if (num543 < 100f)
			//	{
			//		num540 = 10f;
			//	}
			//	num543 = num540 / num543;
			//	num541 *= num543;
			//	num542 *= num543;
			//	base.Projectile.velocity.X = (base.Projectile.velocity.X * 14f + num541) / 15f;
			//	base.Projectile.velocity.Y = (base.Projectile.velocity.Y * 14f + num542) / 15f;
			//}
			//else
			//{
			//	base.Projectile.friendly = false;
			//	if (Math.Abs(base.Projectile.velocity.X) + Math.Abs(base.Projectile.velocity.Y) < 10f)
			//	{
			//		base.Projectile.velocity *= 1.05f;
			//	}
			//}
			//base.Projectile.rotation = base.Projectile.velocity.X * 0.05f;
			//if ((double)Math.Abs(base.Projectile.velocity.X) > 0.2)
			//{
			//	base.Projectile.spriteDirection = -base.Projectile.direction;
			//}
		}
    }
}
