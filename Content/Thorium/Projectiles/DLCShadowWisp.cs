using System;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
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

			if (player.dead || (!player.HasEffect<WarlockEffect>() && !player.HasEffect<SacredEffect>()))
			{
				Projectile.Kill();
				return;
			}

			// shoot the thing
			if (Projectile.localAI[0] == 1f && Main.myPlayer == Projectile.owner)
			{
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(12, 12), ModContent.ProjectileType<DLCShadowWispPro>(), Projectile.damage, 2f, Projectile.owner, 0f, 0f, Projectile.ai[2]);
				Projectile.Kill();
				return;
			}

			Projectile.timeLeft = 2;

			Vector2 orbCenter = player.Center - Vector2.UnitY * 128f;
			const float orbOuterRadius = 32f * 1;
			const float orbInnerRadius = 24f * 1;

			float currentDist = Projectile.Center.Distance(orbCenter);

			if (currentDist > orbInnerRadius)
            {
				if (currentDist > orbOuterRadius) // entirly outside
                {
					Vector2 homePos = (Projectile.Center.DirectionTo(orbCenter) * orbInnerRadius).RotatedBy(MathF.PI / 2 * ((Projectile.whoAmI % 2) * 2 - 1)) + orbCenter;
					Projectile.velocity = Projectile.Center.DirectionTo(homePos) * MathF.Min(MathF.Max(1, Projectile.velocity.Length() * 1.1f), 14f);
                    //float theta = MathHelper.Lerp(Projectile.velocity.ToRotation() + 2 * MathF.PI, Projectile.Center.DirectionTo(orbCenter).ToRotation() + 2 * MathF.PI, 0.1f);
                    //Projectile.velocity = (Vector2.UnitX * 8f).RotatedBy(theta);

                    //if (Projectile.velocity.Length() > 16f)
                    //{
                    //    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 16f;
                    //}
                }
				else // between inner and outer
                {
					float dot = Vector2.Dot(Projectile.DirectionTo(orbCenter), Projectile.velocity);
					if (dot < 0)
                    {
						Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Min(dot / Projectile.velocity.Length(), MathF.PI / 9f));
						//Projectile.velocity -= Vector2.Normalize(Projectile.Center - orbCenter) * 2f;
                        Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 4f;
                    }
                }
            }

			if (currentDist < orbOuterRadius) // inside orb, try to not go outside.
			{
				Projectile.Center += player.velocity;
			}
		}

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
