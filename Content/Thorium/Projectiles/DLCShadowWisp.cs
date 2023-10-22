using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ThoriumMod.Utilities;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
	[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
	public class DLCShadowWisp : ThoriumMod.Projectiles.Healer.ShadowWisp
	{
		public override bool PreDraw(ref Color lightColor)
		{
			Rectangle src = new((int)Projectile.ai[2] * 20, Projectile.frame * 30, 20, 30);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), src, Projectile.GetAlpha(lightColor), Projectile.rotation, src.Size() / 2, Projectile.scale, SpriteEffects.None);
			return false;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Lighting.AddLight(Projectile.Center, 0.65f, 0.4f, 0.7f);
			if (player.dead || (!player.GetModPlayer<CrossplayerThorium>().WarlockEnch && !player.GetModPlayer<CrossplayerThorium>().SacredEnch))
			{
				Projectile.Kill();
				return;
			}
			if (Projectile.localAI[0] == 1f && Main.myPlayer == Projectile.owner)
			{
				IEntitySource source_FromThis = Projectile.GetSource_FromThis(null);
				Vector2 vector = Main.MouseWorld - Projectile.Center;
				float speed = 12f;
				float mag = vector.Length();
				if (mag > speed)
				{
					mag = speed / mag;
					vector *= mag;
				}
				if ((int)Projectile.ai[2] == 2) vector *= 2;

				Projectile.NewProjectile(source_FromThis, Projectile.Center, vector, ModContent.ProjectileType<DLCShadowWispPro>(), Projectile.damage, 2f, Projectile.owner, 0f, 0f, Projectile.ai[2]);
				Projectile.Kill();
				return;
			}
			Projectile.timeLeft = 2;

			float num533 = Projectile.position.X;
			float num534 = Projectile.position.Y;
			bool flag = false;
			int num535 = 500;
			if (Projectile.ai[1] != 0f || Projectile.friendly)
			{
				num535 = 1400;
			}
			if (Math.Abs(Projectile.Center.X - player.Center.X) + Math.Abs(Projectile.Center.Y - player.Center.Y) > (float)num535)
			{
				Projectile.ai[0] = 1f;
			}
			Projectile.tileCollide = false;
			if (!flag)
			{
				Projectile.friendly = true;
				float num536 = 8f;
				if (Projectile.ai[0] == 1f)
				{
					num536 = 12f;
				}
				Vector2 vector2 = Projectile.Center;
				float num537 = player.Center.X - vector2.X;
				float num538 = player.Center.Y - vector2.Y - 60f;
				float num539 = (float)Math.Sqrt((double)(num537 * num537 + num538 * num538));
				if (num539 < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
				{
					Projectile.ai[0] = 0f;
				}
				if (num539 > 2000f)
				{
					Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
					Projectile.position.Y = player.Center.Y - (float)(Projectile.width / 2);
				}
				if (num539 > 70f)
				{
					num539 = num536 / num539;
					num537 *= num539;
					num538 *= num539;
					Projectile.velocity.X = (Projectile.velocity.X * 20f + num537) / 21f;
					Projectile.velocity.Y = (Projectile.velocity.Y * 20f + num538) / 21f;
				}
				else
				{
					if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
					{
						Projectile.velocity.X = -0.15f;
						Projectile.velocity.Y = -0.05f;
					}
					Projectile.velocity *= 1.01f;
				}
				Projectile.friendly = false;
				Projectile.rotation = Projectile.velocity.X * 0.05f;
				if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
				{
					Projectile.spriteDirection = -Projectile.direction;
					return;
				}
			}
			else
			{
				if (Projectile.ai[1] == -1f)
				{
					Projectile.ai[1] = 17f;
				}
				if (Projectile.ai[1] > 0f)
				{
					Projectile.ai[1] -= 1f;
				}
				if (Projectile.ai[1] == 0f)
				{
					Projectile.friendly = true;
					float num540 = 8f;
					Vector2 vector3 = Projectile.Center;
					float num541 = num533 - vector3.X;
					float num542 = num534 - vector3.Y;
					float num543 = (float)Math.Sqrt((double)(num541 * num541 + num542 * num542));
					if (num543 < 100f)
					{
						num540 = 10f;
					}
					num543 = num540 / num543;
					num541 *= num543;
					num542 *= num543;
					Projectile.velocity.X = (Projectile.velocity.X * 14f + num541) / 15f;
					Projectile.velocity.Y = (Projectile.velocity.Y * 14f + num542) / 15f;
				}
				else
				{
					Projectile.friendly = false;
					if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 10f)
					{
						Projectile.velocity *= 1.05f;
					}
				}
				Projectile.rotation = Projectile.velocity.X * 0.05f;
				if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
				{
					Projectile.spriteDirection = -Projectile.direction;
					return;
				}
			}
		}

		// Token: 0x06002C6E RID: 11374 RVA: 0x00176C1C File Offset: 0x00174E1C
		public override void PostAI()
		{
			Projectile.frameCounter++;
			if (Projectile.frameCounter > 4)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
			}
			if (Projectile.frame >= 4)
			{
				Projectile.frame = 0;
				return;
			}
		}
	}
}
