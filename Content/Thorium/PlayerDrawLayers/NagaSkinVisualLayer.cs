using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasCrossmod.Content.Thorium.PlayerLayers
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class NagaSkinVisualLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.JimsCloak, PlayerDrawLayers.MountBack);
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();

            return !player.dead && DLCPlayer.NagaSkinEnch;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();

            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Chains_0").Value;
            Vector2 origin = new(texture.Width / 2, 0);

            for (int i = 0; i < DLCPlayer.nagaSkinLegs.Length; i++)
            {
                Vector2 lead = DLCPlayer.nagaSkinLegs[i];
                Vector2 joint = (DLCPlayer.nagaSkinLegs[i] + player.Center) / 2 - Vector2.UnitY * 32f;

                float rotation1 = lead.DirectionTo(joint).ToRotation() - MathHelper.PiOver2;
                Vector2 scale1 = new(1, lead.Distance(joint) / texture.Height);

                drawInfo.DrawDataCache.Add(new DrawData(texture, lead - Main.screenPosition, texture.Bounds, Color.Gray, rotation1, origin, scale1, SpriteEffects.None));

                float rotation2 = joint.DirectionTo(player.Center - Vector2.UnitY * player.gfxOffY).ToRotation() - MathHelper.PiOver2;
                Vector2 scale2 = new(1, joint.Distance(player.Center - Vector2.UnitY * player.gfxOffY) / texture.Height);

                drawInfo.DrawDataCache.Add(new DrawData(texture, joint - Main.screenPosition, texture.Bounds, Color.Red, rotation2, origin, scale2, SpriteEffects.None));
            }
        }
    }
}