using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using ThoriumMod.Empowerments;
using ThoriumMod.Utilities;

namespace FargowiltasCrossmod.Content.Thorium.PlayerLayers
{
	// This layout of making orbital player layers for drawing is stolen from Thorium btw
	[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
	public class OrbitalLayerFront : OrbitalLayerBase
	{
		public override bool Front => true;
	}
	[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
	public class OrbitalLayerBack : OrbitalLayerBase
	{
		public override bool Front => false;
	}

	[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
	public abstract class OrbitalLayerBase : PlayerDrawLayer
    {
		public abstract bool Front { get; }

		public override Position GetDefaultPosition()
		{
			if (!this.Front)
			{
				return new Between(PlayerDrawLayers.JimsCloak, PlayerDrawLayers.MountBack);
			}
			return new Between(PlayerDrawLayers.CaptureTheGem, PlayerDrawLayers.BeetleBuff);
		}

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			if (drawPlayer.dead || drawInfo.shadow != 0f)
			{
				return false;
			}
			var DLCPlayer = drawPlayer.ThoriumDLC();
			return DLCPlayer.NoviceClericCrosses > 0;
		}

        protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			var DLCPlayer = drawPlayer.ThoriumDLC();

			if (DLCPlayer.NoviceClericCrosses > 0 && (DLCPlayer.NoviceClericEnch || DLCPlayer.EbonEnch))
			{
				// This code is modified from thorium code in OrbitalLayerBase.cs
				Texture2D texture5 = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Thorium/PlayerDrawLayers/Cross").Value;
				Vector2 rotation = DLCPlayer.crossOrbitalRotation;
				float addrotation4 = MathF.Tau / DLCPlayer.NoviceClericCrosses;

				bool alternating = DLCPlayer.SynergyEffect(DLCPlayer.NoviceClericEnchItem.type);

				for (int n = 0; n < DLCPlayer.NoviceClericCrosses; n++)
				{
					if (n > 0)
					{
						rotation = Utils.RotatedBy(rotation, (double)addrotation4, default);
					}

					if ((!Front && rotation.Y < 0f) || (Front && rotation.Y >= 0f))
					{
						Vector2 rotationPos = new(rotation.X * (drawPlayer.width + texture5.Width * 0.3f + (alternating ? 16f : 1f)) - 2f, drawPlayer.mount.PlayerOffsetHitbox * 2 + rotation.Y * 15f);
						Vector2 drawPos = drawInfo.Position + drawPlayer.Size / 2f + rotationPos;
						drawPos.Y += 4f;
						float scale5 = 0.87f + rotation.Y * 0.13f;
						Vector2 origin5 = new Vector2(7, 9) * scale5;
						SpriteEffects effect = (drawPlayer.gravDir == -1f) ? SpriteEffects.FlipVertically : SpriteEffects.None;
						Color color5 = Color.White;
						color5 = Color.Lerp(color5, color5 * 0.7f, Math.Abs(rotation.Y - 1f) * 0.5f);
						color5.A = 190;
						bool crossType = !alternating || n % 2 == 0;
						Rectangle rect = new(crossType ? 0 : 14, 0, 14, 18);
						DrawData data5 = new(texture5, drawPos - Main.screenPosition, rect, color5, 0f, origin5, scale5, effect, 0);
						drawInfo.DrawDataCache.Add(data5);
					}
				}
			}
		}
    }
}