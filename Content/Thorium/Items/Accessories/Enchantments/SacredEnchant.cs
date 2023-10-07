using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod("ThoriumMod")]
    public class SacredEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Pink;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.SacredEnch = true;
            DLCPlayer.SacredEnchItem = Item;
        }

        int drawTimer = 0;
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var DLCPlayer = Main.LocalPlayer.GetModPlayer<CrossplayerThorium>();
            if (DLCPlayer.WarlockEnch && DLCPlayer.SacredEnch)
            {
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                    float modifier = 0.25f + ((float)Math.Sin(drawTimer / 30f) / 2);
                    Color glowColor = Color.Lerp(Color.Orange with { A = 0 }, Color.Black with { A = 0 }, modifier) * 0.5f;

                    Texture2D texture = Terraria.GameContent.TextureAssets.Item[Item.type].Value;
                    Main.EntitySpriteDraw(texture, position + afterimageOffset, null, glowColor, 0, texture.Size() * 0.5f, Item.scale, SpriteEffects.None, 0f);
                }
            }
            drawTimer++;
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
    }
}