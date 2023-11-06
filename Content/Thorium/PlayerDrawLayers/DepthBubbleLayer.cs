using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasCrossmod.Content.Thorium.PlayerLayers
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DepthBubbleLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var DLCPlayer = player.ThoriumDLC();

            return !player.dead && DLCPlayer.DepthBubble > 0;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var DLCPlayer = player.ThoriumDLC();

            Texture2D texture = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Thorium/PlayerDrawLayers/DepthBubble").Value;
            drawInfo.DrawDataCache.Add(new DrawData(texture, player.Center - Main.screenPosition - texture.Size() / 2, Color.LightSkyBlue with { A = 100 }));
        }
    }
}