using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
	[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
	public class DLCShadowWispPro : ThoriumMod.Projectiles.Healer.ShadowWispPro
	{
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(153, 60, false);
		}

		public override void AI()
		{
			int mode = (int)Projectile.ai[2];

			for (int num363 = 0; num363 < 3; num363++)
			{
				float num364 = Projectile.velocity.X / 3f * (float)num363;
				float num365 = Projectile.velocity.Y / 3f * (float)num363;
				int dustType = mode switch
				{
					0 => DustID.Shadowflame,
					1 => DustID.CursedTorch,
					2 => Main.rand.NextBool() ? DustID.Shadowflame : DustID.CursedTorch,
					_ => DustID.Shadowflame
				};
				int num366 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 255, default(Color), 1.25f);
				Main.dust[num366].position.X = Projectile.Center.X - num364;
				Main.dust[num366].position.Y = Projectile.Center.Y - num365;
				Main.dust[num366].velocity *= 0f;
				Main.dust[num366].noGravity = true;
			}

			Vector2 homePos = Projectile.Center;
			float num369 = 500f;
			float healHomeRange = 1500f;
			bool homePosFound = false;
			int num374;

			if (mode == 0 || mode == 2)
			{
				for (int num370 = 0; num370 < 200; num370 = num374 + 1)
				{
					NPC npc = Main.npc[num370];
					if (npc.CanBeChasedBy(null, false) && Projectile.DistanceSQ(npc.Center) < num369 * num369 && Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1))
					{
						float num371 = npc.Center.X;
						float num372 = npc.Center.Y;
						float num373 = Math.Abs(Projectile.Center.X - num371) + Math.Abs(Projectile.Center.Y - num372);
						if (num373 < num369)
						{
							num369 = num373;
							homePos = new(num371, num372);
							homePosFound = true;
						}
					}
					num374 = num370;
				}
			}
			else if (mode == 1)
            {
				Projectile.DLCHeal(3);
				for (int i = 0; i < Main.maxPlayers; i++)
                {
					if (i == Projectile.owner) continue;
					Player other = Main.player[i]; 

					if (Projectile.DistanceSQ(other.Center) < healHomeRange * healHomeRange && Collision.CanHit(Projectile.Center, 1, 1, other.Center, 1, 1))
					{
						float num371 = other.Center.X;
						float num372 = other.Center.Y;
						float num373 = Math.Abs(Projectile.Center.X - num371) + Math.Abs(Projectile.Center.Y - num372);
						if (num373 < healHomeRange)
						{
							healHomeRange = num373;
							homePos = new(num371, num372);
							homePosFound = true;
						}
					}
                }
            }

			if (homePosFound)
			{
				float num375 = 8f;
				Vector2 vector38 = Projectile.Center;
				Vector2 homeVec = homePos - vector38;
				//float num376 = num367 - vector38.X;
				//float num377 = num368 - vector38.Y;
				float len = homeVec.Length();
				if (len > 0f)
				{
					len = num375 / len;
				}

				
				Projectile.velocity = (Projectile.velocity * 20f + homeVec * len) / 21f;
				
				//Projectile.velocity.X = (Projectile.velocity.X * 20f + num376) / 21f;
				//Projectile.velocity.Y = (Projectile.velocity.Y * 20f + num377) / 21f;
			}
		}

		public override void OnKill(int timeLeft)
		{
			int dustType = (int)Projectile.ai[2] switch
			{
				0 => DustID.Shadowflame,
				1 => DustID.GreenTorch,
				2 => Main.rand.NextBool() ? DustID.Shadowflame : DustID.GreenTorch,
				_ => DustID.Shadowflame
			};
			for (int u = 0; u < 15; u++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, (float)Main.rand.Next(-4, 4), (float)Main.rand.Next(-4, 4), 255, default(Color), 1.5f);
				Main.dust[dust].noGravity = true;
			}

			if ((int)Projectile.ai[2] == 2)
            {
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Projectile.velocity, Projectile.type, 0, 0, Projectile.owner, 0, 0, 1);
            }
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Rectangle src = new((int)Projectile.ai[2] * 14, 0, 14, 28);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), src, Projectile.GetAlpha(lightColor), Projectile.rotation, src.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
