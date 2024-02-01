using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;

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
			Projectile.timeLeft = 7200; // 2 minutes
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(80, 12), Vector2.Zero, ModContent.ProjectileType<LifeBloomBallista>(), 150, 2f, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(-88, 32), Vector2.Zero, ModContent.ProjectileType<LifeBloomBallista>(), 150, 2f, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(-114, 102), Vector2.Zero, ModContent.ProjectileType<LifeBloomBallista>(), 150, 2f, Projectile.owner);
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Vector2.UnitY * 66f, Vector2.Zero, ModContent.ProjectileType<LifeBloomWard>(), 150, 2f, Projectile.owner);
		}

        public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			if (!player.active || player.dead || !player.HasEffect<LifeBloomEffect>())
            {
				Projectile.Kill();
            }
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class LifeBloomBallista : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.DD2BallistraTowerT3}";
        public override void SetDefaults()
        {
			Projectile.CloneDefaults(ProjectileID.DD2BallistraTowerT3);
            Projectile.sentry = false;
            AIType = ProjectileID.DD2BallistraTowerT3;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Color color = Projectile.GetAlpha(lightColor);
			SpriteEffects dir = SpriteEffects.None;

			Vector2 position = Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
			Texture2D texture = TextureAssets.Projectile[ProjectileID.DD2BallistraTowerT3].Value;

			Rectangle rectangle22 = texture.Frame(1, Main.projFrames[ProjectileID.DD2BallistraTowerT3], 0, Projectile.frame);
			Vector2 origin25 = rectangle22.Size() / 2f;

			if (Projectile.spriteDirection == -1)
			{
				dir ^= SpriteEffects.FlipVertically;
			}

			position.Y += 1f;
			if (!dir.HasFlag(SpriteEffects.FlipVertically))
			{
				origin25.Y += 4f;
			}
			else
			{
				origin25.Y -= 4f;
			}
			origin25.X += dir.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt() * 5;

			Main.EntitySpriteDraw(texture, position, rectangle22, color, Projectile.rotation, origin25, Projectile.scale, dir);
			return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
			if (source is EntitySource_Parent parentSource)
            {
				Projectile.ai[2] = parentSource.Entity.whoAmI;
            }
			//Projectile.direction = (int)Projectile.ai[1];
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = Main.projFrames[ProjectileID.DD2BallistraTowerT3];
        }

        public override void AI()
        {
            base.AI();

			if (Main.projectile[(int)Projectile.ai[2]].active)
            {
				Projectile.timeLeft = 2;
            }
			
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

        public override void PostAI()
        {
			Projectile.frame = (Projectile.frameCounter++ / 10) % 8;
        }

        public override void OnSpawn(IEntitySource source)
		{
			if (source is EntitySource_Parent parentSource)
			{
				Projectile.ai[0] = parentSource.Entity.whoAmI;
			}
		}

        /*
		 * Projectile.ai[:
		 * 0 - parent tree whoAmI
		 * 1 - timer for drawing
		 * 2 - unused
		 */

        public override void PostDraw(Color lightColor)
		{
			float outerRange = 300f;
			if (Projectile.ai[1] >= 100f)
			{
				outerRange = MathHelper.Lerp(300f, 600f, (Projectile.ai[1] - 100f) / 200f);
			}
			if (outerRange > 600f)
			{
				outerRange = 600f;
			}

			float rotation = Projectile.ai[1] * MathF.PI / 300f;
			Texture2D texture = TextureAssets.Projectile[ProjectileID.DryadsWardCircle].Value;
			int num = (int)(Projectile.ai[1] / 6f);
			Vector2 spinningPoint = new(0f, 0f - outerRange);

			// inner ring
			for (int i = 0; i < 10; i++)
			{
				Rectangle srcRect = texture.Frame(1, 5, 0, (num + i) % 5);
				float num241 = rotation + (float)Math.PI / 5f * (float)i;
				Vector2 leafPos = spinningPoint.RotatedBy(num241) / 3f + Projectile.Center;
				Color alpha7 = Projectile.GetAlpha(Lighting.GetColor(leafPos.ToTileCoordinates()));
				alpha7.A /= 2;

				Main.EntitySpriteDraw(texture, leafPos - Main.screenPosition, srcRect, alpha7, num241, srcRect.Size() / 2f, Projectile.scale, SpriteEffects.None);
			}

			// outer ring
			for (int i = 0; i < 20; i++)
			{
				Rectangle srcRect = texture.Frame(1, 5, 0, (num + i) % 5);
				float num243 = 0f - rotation + (float)Math.PI / 10f * (float)i;
				num243 *= 2f;
				Vector2 leafPos = spinningPoint.RotatedBy(num243) + Projectile.Center;
				Color alpha7 = Projectile.GetAlpha(Lighting.GetColor(leafPos.ToTileCoordinates()));
				alpha7.A /= 2;
				Main.EntitySpriteDraw(texture, leafPos - Main.screenPosition, srcRect, alpha7, num243, srcRect.Size() / 2f, Projectile.scale, SpriteEffects.None);
			}
		}

        public override void AI()
		{
			if (Main.projectile[(int)Projectile.ai[0]].active)
			{
				Projectile.timeLeft = 2;
			}

			Projectile.ai[1]++;
			// fading in stuff
			Projectile.scale = MathF.Min(Projectile.ai[1] / 100f, 1f);
			Projectile.alpha = (int)(255f * (1f - Projectile.scale));

			float outerRange = 300f;
			if (Projectile.ai[1] >= 100f)
			{
				outerRange = MathHelper.Lerp(300f, 600f, (Projectile.ai[1] - 100f) / 200f);
			}
			if (outerRange > 600f)
			{
				outerRange = 600f;
			}

			if (Main.rand.NextBool(4))
			{
				Vector2 vector = Main.rand.NextVector2CircularEdge(10, 10);
				float num = Main.rand.Next(3, 9);

				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DryadsWard, 0f, 0f, 100);
				dust.noGravity = true;
				dust.position = Projectile.Center + vector * outerRange;
				dust.velocity = vector * num;

				if (Main.rand.NextBool(8))
				{
					dust.velocity *= 3f;
					dust.scale += 0.5f;
				}
			}

			if (Main.rand.NextBool(2))
			{
				Vector2 vector = Main.rand.NextVector2CircularEdge(10, 10);
				float num = -Main.rand.Next(3, 9);
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DryadsWard, 0f, 0f, 100, default, 1.5f);
				dust.noGravity = true;
				dust.position = Projectile.Center + vector * 30f;
				dust.velocity = vector * num;

				if (Main.rand.NextBool(8))
				{
					dust.velocity *= 3f;
					dust.scale += 0.5f;
				}
			}

			if (Projectile.ai[1] >= 30f && Main.netMode != NetmodeID.Server)
			{
				Player player;
				player = Main.player[Main.myPlayer];
				if (player.active && !player.dead && Projectile.Distance(player.Center) <= outerRange && player.FindBuffIndex(165) == -1)
				{
					player.AddBuff(165, 120);
				}
			}

			if (Projectile.ai[1] >= 30f && Projectile.ai[1] % 10f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
			{
				for (int i = 0; i < 200; i++)
				{
					NPC nPC;
					nPC = Main.npc[i];
					if (nPC.type != NPCID.TargetDummy && nPC.active && Projectile.Distance(nPC.Center) <= outerRange)
					{
						if (nPC.townNPC && (nPC.FindBuffIndex(165) == -1 || nPC.buffTime[nPC.FindBuffIndex(BuffID.DryadsWard)] <= 20))
						{
							nPC.AddBuff(BuffID.DryadsWard, 120);
						}
						else if (!nPC.friendly && nPC.lifeMax > 5 && !nPC.dontTakeDamage && (nPC.FindBuffIndex(BuffID.DryadsWardDebuff) == -1 || nPC.buffTime[nPC.FindBuffIndex(BuffID.DryadsWardDebuff)] <= 20) && (nPC.dryadBane || Collision.CanHit(Projectile.Center, 1, 1, nPC.position, nPC.width, nPC.height)))
						{
							nPC.AddBuff(BuffID.DryadsWardDebuff, 120);
						}
					}
				}
			}
		}
    }
}