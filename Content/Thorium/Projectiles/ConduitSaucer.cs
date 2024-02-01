using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
	[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class ConduitSaucer : ModProjectile
    {
        public override void SetDefaults()
        {
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.width = 24;
			Projectile.height = 16;
			Main.projFrames[Type] = 4;
        }

		public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.UFOMinion}";

		public int fireDelay;
        public override void PostAI()
		{
			if (fireDelay-- > 0) return;

			fireDelay = 30;

			Projectile closest = null;
			float closestDist = float.MaxValue;
			for (int n = 0; n < Main.maxProjectiles; n++)
            {
                Projectile proj = Main.projectile[n];
                if (proj.active && !proj.friendly && proj.hostile && proj.damage > 0)
                {
					float dist2 = proj.Center.DistanceSQ(Projectile.Center);
					if (dist2 < 4096f && Collision.CanHitLine(proj.Center, 2, 2, Projectile.Center, 2, 2) && closest == null || dist2 < closestDist)
                    {
						closestDist = dist2;
						closest = proj;
                    }
                }
            }

			if (closest == null) return;

			closest.damage -= 24; // maybe add combatText indicator for this
			for (int i = 0; i < 48; i++)
            {
				float lerp = Main.rand.NextFloat();
				Dust.NewDustPerfect(Projectile.Center * lerp + (closest.Center * (1 - lerp)), DustID.MartianSaucerSpark).noGravity = true;
            }

			if (closest.damage <= 0)
            {
				closest.Kill();
            }
        }

		public override void AI()
		{
			float num;
			num = 0f;
			float num2;
			num2 = 0f;
			float num3;
			num3 = 20f;
			float num4;
			float num5;
			num5 = 0.69f;
			num4 = 5f;
			if (!Main.player[Projectile.owner].dead)// && !Main.player[Projectile.owner].HasEffect<ConduitEffect>())
			{
				Projectile.timeLeft = 2;
			}
			if (Projectile.ai[0] == 2f)
			{
				Projectile.ai[1] -= 1f;
				Projectile.tileCollide = false;
				if (Projectile.ai[1] > 3f)
				{
					int num6;
					num6 = Dust.NewDust(Projectile.Center, 0, 0, 220 + Main.rand.Next(2), Projectile.velocity.X, Projectile.velocity.Y, 100);
					Main.dust[num6].scale = 0.5f + (float)Main.rand.NextDouble() * 0.3f;
					Main.dust[num6].velocity /= 2.5f;
					Main.dust[num6].noGravity = true;
					Main.dust[num6].noLight = true;
					Main.dust[num6].frame.Y = 80;
				}
				if (Projectile.ai[1] != 0f)
				{
					return;
				}
				Projectile.ai[1] = 30f;
				Projectile.ai[0] = 0f;
				Projectile.velocity /= 5f;
				Projectile.velocity.Y = 0f;
				Projectile.extraUpdates = 0;
				Projectile.numUpdates = 0;
				Projectile.netUpdate = true;
				Projectile.extraUpdates = 0;
				Projectile.numUpdates = 0;
			}
			if (Projectile.extraUpdates > 1)
			{
				Projectile.extraUpdates = 0;
			}
			if (Projectile.numUpdates > 1)
			{
				Projectile.numUpdates = 0;
			}
			Projectile.localAI[0] -= 1f;
			float num10;
			num10 = 0.05f;
			float num11;
			num11 = Projectile.width;
			for (int m = 0; m < 1000; m++)
			{
				if (m != Projectile.whoAmI && Main.projectile[m].active && Main.projectile[m].owner == Projectile.owner && Main.projectile[m].type == Projectile.type && Math.Abs(Projectile.position.X - Main.projectile[m].position.X) + Math.Abs(Projectile.position.Y - Main.projectile[m].position.Y) < num11)
				{
					if (Projectile.position.X < Main.projectile[m].position.X)
					{
						Projectile.velocity.X -= num10;
					}
					else
					{
						Projectile.velocity.X += num10;
					}
					if (Projectile.position.Y < Main.projectile[m].position.Y)
					{
						Projectile.velocity.Y -= num10;
					}
					else
					{
						Projectile.velocity.Y += num10;
					}
				}
			}
			Vector2 vector;
			vector = Projectile.position;
			//float num12;
			//num12 = 300f;
			bool flag;
			flag = false;
			Projectile.tileCollide = true;
			Vector2 center;
			center = Main.player[Projectile.owner].Center;
			Vector2 vector2;
			vector2 = new(0.5f, 0f);
            int num21;
			num21 = 1200;
			Player player;
			player = Main.player[Projectile.owner];
			if (Vector2.Distance(player.Center, Projectile.Center) > (float)num21)
			{
				Projectile.ai[0] = 1f;
				Projectile.netUpdate = true;
			}
			if (Projectile.ai[0] == 1f)
			{
				Projectile.tileCollide = false;
			}
			bool flag4;
			flag4 = false;
			if (Projectile.ai[0] >= 2f)
			{
				if (Projectile.ai[0] == 2f && Projectile.type == 963)
				{
					SoundEngine.PlaySound(SoundID.AbigailAttack, Projectile.Center);
				}
				Projectile.ai[0] += 1f;
				if (flag4)
				{
					Projectile.localAI[1] = Projectile.ai[0] / num4;
				}
				if (!flag)
				{
					Projectile.ai[0] += 1f;
				}
				if (Projectile.ai[0] > num4)
				{
					Projectile.ai[0] = 0f;
					Projectile.netUpdate = true;
					if (flag && Projectile.type == 963 && (vector - Projectile.Center).Length() < 50f)
					{
						Projectile.ai[0] = 2f;
					}
				}
				Projectile.velocity *= num5;
			}
			else if (flag)
			{
				Vector2 v;
				float num22;
				v = vector - Vector2.UnitY * 80f;
				int num23;
				num23 = (int)v.Y / 16;
				if (num23 < 0)
				{
					num23 = 0;
				}
				Tile tile;
				tile = Main.tile[(int)v.X / 16, num23];
				if (tile != null && tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
				{
					v += Vector2.UnitY * 16f;
					tile = Main.tile[(int)v.X / 16, (int)v.Y / 16];
					if (tile != null && tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
					{
						v += Vector2.UnitY * 16f;
					}
				}
				v -= Projectile.Center;
				num22 = v.Length();
				v = v.SafeNormalize(Vector2.Zero);
				if (num22 > 300f && num22 <= 800f && Projectile.localAI[0] == 0f)
				{
					Projectile.ai[0] = 2f;
					Projectile.ai[1] = (int)(num22 / 10f);
					Projectile.extraUpdates = (int)Projectile.ai[1];
					Projectile.velocity = v * 10f;
					Projectile.localAI[0] = 60f;
					return;
				}
				if (num22 > 200f)
				{
					float num26;
					num26 = 6f + num2 * num;
					v *= num26;
					float num27;
					num27 = num3 * 2f;
					Projectile.velocity.X = (Projectile.velocity.X * num27 + v.X) / (num27 + 1f);
					Projectile.velocity.Y = (Projectile.velocity.Y * num27 + v.Y) / (num27 + 1f);
				}
				if (num22 > 70f && num22 < 130f)
				{
					float num29;
					num29 = 7f;
					if (num22 < 100f)
					{
						num29 = -3f;
					}
					v *= num29;
					Projectile.velocity = (Projectile.velocity * 20f + v) / 21f;
					if (Math.Abs(v.X) > Math.Abs(v.Y))
					{
						Projectile.velocity.X = (Projectile.velocity.X * 10f + v.X) / 11f;
					}
				}
				else
				{
					Projectile.velocity *= 0.97f;
				}
			}
			else
			{
				if (!Collision.CanHitLine(Projectile.Center, 1, 1, Main.player[Projectile.owner].Center, 1, 1))
				{
					Projectile.ai[0] = 1f;
				}
				float num31;
				num31 = 6f;
				if (Projectile.ai[0] == 1f)
				{
					num31 = 15f;
				}
				Vector2 center2;
				center2 = Projectile.Center;
				Vector2 v2;
				v2 = player.Center - center2 + new Vector2(0f, -60f);
				float num34;
				num34 = v2.Length();
				if (num34 > 200f && num31 < 9f)
				{
					num31 = 9f;
				}
				if (num34 > 300f && num31 < 12f)
				{
					num31 = 12f;
				}
				if (num34 < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
				{
					Projectile.ai[0] = 0f;
					Projectile.netUpdate = true;
				}
				if (num34 > 2000f)
				{
					Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
					Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.width / 2);
				}
				if (num34 > 70f)
				{
					v2 = v2.SafeNormalize(Vector2.Zero);
					v2 *= num31;
					Projectile.velocity = (Projectile.velocity * 20f + v2) / 21f;
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
				if (num34 > 250f)
				{
					float x;
					x = Main.player[Projectile.owner].velocity.X;
					float y;
					y = Main.player[Projectile.owner].velocity.Y;
					if ((Projectile.velocity.X < 0f && x >= 0f) || (Projectile.velocity.X >= 0f && x < 0f))
					{
						Projectile.velocity.X *= 0.95f;
					}
					else
					{
						Projectile.velocity.X += x * 0.125f;
					}
					if ((Projectile.velocity.Y < 0f && y >= 0f) || (Projectile.velocity.Y >= 0f && y < 0f))
					{
						Projectile.velocity.Y *= 0.95f;
					}
					else
					{
						Projectile.velocity.Y += y * 0.125f;
					}
					if (Projectile.velocity.Length() > num31)
					{
						Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * num31;
					}
				}
			}
			Projectile.rotation = Projectile.velocity.X * 0.05f;
			Projectile.frameCounter++;
			int num43;
			num43 = 3;
			if (Projectile.frameCounter >= 4 * num43)
			{
				Projectile.frameCounter = 0;
			}
			Projectile.frame = Projectile.frameCounter / num43;
			if (Projectile.velocity.X > 0f)
			{
				Projectile.spriteDirection = (Projectile.direction = -1);
			}
			else if (Projectile.velocity.X < 0f)
			{
				Projectile.spriteDirection = (Projectile.direction = 1);
			}
			if (Projectile.ai[1] > 0f)
			{
				Projectile.ai[1] += 1f;
				if (!Main.rand.NextBool(3))
				{
					Projectile.ai[1] += 1f;
				}
			}
			if (Projectile.ai[1] > 40f)
			{
				Projectile.ai[1] = 0f;
				Projectile.netUpdate = true;
			}
			if (!flag)
			{
				return;
			}
			if (Math.Abs((vector - Projectile.Center).ToRotation() - (float)Math.PI / 2f) > (float)Math.PI / 4f)
			{
				Projectile.velocity += (vector - Projectile.Center - Vector2.UnitY * 80f).SafeNormalize(Vector2.Zero);
			}
		}
    }
}
