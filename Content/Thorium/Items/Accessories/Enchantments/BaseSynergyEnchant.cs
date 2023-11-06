using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public abstract class BaseSynergyEnchant : BaseEnchant
    {
        internal abstract bool SynergyActive(CrossplayerThorium DLCPlayer);
        protected abstract Color SynergyColor1 { get; }
        protected abstract Color SynergyColor2 { get; }
        internal abstract int SynergyEnch { get; }

        int drawTimer = 0;
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.LocalPlayer.ThoriumDLC().SynergyEffect(Item.type) && Main.LocalPlayer.armor.Contains(Item))
            {
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                    float modifier = 0.25f + ((float)Math.Sin(drawTimer / 30f) / 2);
                    Color glowColor = Color.Lerp(SynergyColor1, SynergyColor2, modifier) * 0.5f;

                    Texture2D texture = Terraria.GameContent.TextureAssets.Item[Item.type].Value;
                    Main.EntitySpriteDraw(texture, position + afterimageOffset, null, glowColor, 0, texture.Size() * 0.5f, Item.scale, SpriteEffects.None, 0f);
                }
            }
            drawTimer++;
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
    }
}