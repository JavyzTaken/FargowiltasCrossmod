using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class LifeBloomTree : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 600;
            Projectile.height = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.position -= new Vector2(150, 150);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Vector2.UnitX * 40f, Vector2.Zero, ModContent.ProjectileType<LifeBloomBallista>(), 150, 2f, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Vector2.UnitX * 40f, Vector2.Zero, ModContent.ProjectileType<LifeBloomWard>(), 150, 2f, Projectile.owner);
		}
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class LifeBloomBallista : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.DD2BallistraTowerT3}";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 54;
            Projectile.aiStyle = 134;
            Projectile.timeLeft = 36000;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.manualDirectionChange = true;
            //Projectile.sentry = true;
            Projectile.netImportant = true;
            AIType = ProjectileID.DD2BallistraTowerT3;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = Main.projFrames[ProjectileID.DD2BallistraTowerT3];
        }

        public override void AI()
        {
            base.AI();
            Projectile.spriteDirection = 1;
            Projectile.velocity.Y -= 0.2f;
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class LifeBloomWard : ModProjectile
    {
        public override void SetDefaults()
        {
			Projectile.width = 26;
			Projectile.height = 44;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
        }

        public override void SetStaticDefaults()
        {
			Main.projFrames[Type] = 8;
        }

        public override void AI()
        {
			if (!Main.projectile[(int)Projectile.ai[0]].active || Main.projectile[(int)Projectile.ai[0]].type != ModContent.ProjectileType<LifeBloomTree>())
			{
				Projectile.Kill();
				return;
			}
			Projectile.ai[0] += 1f;
			Projectile.rotation += (float)Math.PI / 300f;
			Projectile.scale = Projectile.ai[0] / 100f;
			if (Projectile.scale > 1f)
			{
				Projectile.scale = 1f;
			}
			Projectile.alpha = (int)(255f * (1f - Projectile.scale));
			float num;
			num = 300f;
			if (Projectile.ai[0] >= 100f)
			{
				num = MathHelper.Lerp(300f, 600f, (Projectile.ai[0] - 100f) / 200f);
			}
			if (num > 600f)
			{
				num = 600f;
			}
			if (Projectile.ai[0] >= 500f)
			{
				Projectile.alpha = (int)MathHelper.Lerp(0f, 255f, (Projectile.ai[0] - 500f) / 100f);
				num = MathHelper.Lerp(600f, 1200f, (Projectile.ai[0] - 500f) / 100f);
				Projectile.rotation += (float)Math.PI / 300f;
			}
			int num2;
			num2 = 163;
			if (Projectile.ai[2] == 1f)
			{
				float num3;
				num3 = (float)Math.Sin(Projectile.ai[0] % 120f * ((float)Math.PI * 2f) / 120f) * 0.5f + 0.5f;
				if (Main.rand.NextFloat() < num3)
				{
					num2 = 70;
				}
			}
			if (Main.rand.NextBool(4))
			{
				float num4;
				num4 = num;
				Vector2 vector;
				vector = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
				float num5;
				num5 = Main.rand.Next(3, 9);
				vector.Normalize();
				int num6;
				num6 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, num2, 0f, 0f, 100);
				Main.dust[num6].noGravity = true;
				Main.dust[num6].position = Projectile.Center + vector * num4;
				if (Main.rand.NextBool(8))
				{
					Main.dust[num6].velocity = vector * (0f - num5) * 3f;
					Main.dust[num6].scale += 0.5f;
				}
				else
				{
					Main.dust[num6].velocity = vector * (0f - num5);
				}
			}
			if (Main.rand.NextBool(2))
			{
				Vector2 vector2;
				vector2 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
				float num7;
				num7 = Main.rand.Next(3, 9);
				vector2.Normalize();
				int num8;
				num8 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, num2, 0f, 0f, 100, default(Color), 1.5f);
				Main.dust[num8].noGravity = true;
				Main.dust[num8].position = Projectile.Center + vector2 * 30f;
				if (Main.rand.NextBool(8))
				{
					Main.dust[num8].velocity = vector2 * (0f - num7) * 3f;
					Main.dust[num8].scale += 0.5f;
				}
				else
				{
					Main.dust[num8].velocity = vector2 * (0f - num7);
				}
			}
			if (Projectile.ai[0] >= 30f && Main.netMode != 2)
			{
				Player player;
				player = Main.player[Main.myPlayer];
				if (player.active && !player.dead && Projectile.Distance(player.Center) <= num && player.FindBuffIndex(165) == -1)
				{
					player.AddBuff(165, 120);
				}
			}
			if (Projectile.ai[0] >= 30f && Projectile.ai[0] % 10f == 0f && Main.netMode != 1)
			{
				for (int i = 0; i < 200; i++)
				{
					NPC nPC;
					nPC = Main.npc[i];
					if (nPC.type != 488 && nPC.active && Projectile.Distance(nPC.Center) <= num)
					{
						if (nPC.townNPC && (nPC.FindBuffIndex(165) == -1 || nPC.buffTime[nPC.FindBuffIndex(165)] <= 20))
						{
							nPC.AddBuff(165, 120);
						}
						else if (!nPC.friendly && nPC.lifeMax > 5 && !nPC.dontTakeDamage && (nPC.FindBuffIndex(186) == -1 || nPC.buffTime[nPC.FindBuffIndex(186)] <= 20) && (nPC.dryadBane || Collision.CanHit(Projectile.Center, 1, 1, nPC.position, nPC.width, nPC.height)))
						{
							nPC.AddBuff(186, 120);
						}
					}
				}
			}
			if (Projectile.ai[0] >= 570f)
			{
				Projectile.Kill();
			}
		}
    }
}