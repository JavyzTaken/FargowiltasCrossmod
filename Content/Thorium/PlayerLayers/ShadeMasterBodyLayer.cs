using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.PlayerLayers
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class ShadeMasterBodyLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(Terraria.DataStructures.PlayerDrawLayers.FrontAccFront);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead || !drawInfo.drawPlayer.ThoriumDLC().ShadeMode)
            {
                return;
            }

            //var DLCPlayer = drawInfo.drawPlayer.ThoriumDLC();

            //drawInfo.drawPlayer.head = DLCPlayer.shadeMasterDataCopy.head;
            //drawInfo.drawPlayer.body = DLCPlayer.shadeMasterDataCopy.body;
            //drawInfo.drawPlayer.legs = DLCPlayer.shadeMasterDataCopy.legs;

            //int wingFrame = drawInfo.drawPlayer.wingFrame;
            //Rectangle bodyFrame = drawInfo.drawPlayer.bodyFrame;
            //Rectangle legFrame = drawInfo.drawPlayer.legFrame;
            //drawInfo.drawPlayer.wingFrame = -1;
            //drawInfo.drawPlayer.bodyFrame.X = drawInfo.drawPlayer.bodyFrame.Y = 0;
            //drawInfo.drawPlayer.legFrame.X = drawInfo.drawPlayer.legFrame.Y = 0;

            ////Vector2 bodyRelativePos = drawInfo.drawPlayer.ThoriumDLC().shadeMasterBodyCenter - drawInfo.drawPlayer.Center;

            //Main.PlayerRenderer.DrawPlayer(Main.Camera, drawInfo.drawPlayer, drawInfo.Position, 3, drawInfo.rotationOrigin);
            ////for (int j = 0; j < drawInfo.DrawDataCache.Count; j++)
            ////{
            ////    DrawData currentData = drawInfo.DrawDataCache[j];
            ////    Rectangle? srcRect = currentData.sourceRect;
            ////    if (srcRect.HasValue && j == drawInfo.head)
            ////    {
            ////        srcRect = srcRect.HasValue ? new(0, 0, currentData.sourceRect.Value.Width, currentData.sourceRect.Value.Height) : null;
            ////    }
            ////    Main.EntitySpriteDraw(currentData.texture, currentData.position + bodyRelativePos, srcRect, currentData.color, drawInfo.drawPlayer.fullRotation, currentData.origin, currentData.scale, currentData.effect, 0);
            ////}

            //drawInfo.drawPlayer.head = 120; // EquipLoader.GetEquipSlot(Mod, ShadeMasterEnchItem.Name, EquipType.Head);
            //drawInfo.drawPlayer.body = 81; // EquipLoader.GetEquipSlot(Mod, ShadeMasterEnchItem.Name, EquipType.Body);
            //drawInfo.drawPlayer.legs = 169; // EquipLoader.GetEquipSlot(Mod, ShadeMasterEnchItem.Name, EquipType.Legs); 
            //drawInfo.drawPlayer.wingFrame = wingFrame;
            //drawInfo.drawPlayer.bodyFrame = bodyFrame;
            //drawInfo.drawPlayer.legFrame = legFrame;
        }
    }
}
