using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public abstract class BaseSynergyEnchant<T> : BaseEnchant where T : AccessoryEffect
    {
        internal abstract int SynergyEnch { get; }

        int drawTimer = 0;
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            //if ((Main.LocalPlayer.HasEffect<SilkEffect>() || Main.LocalPlayer.HasEffect<T>()) && Main.LocalPlayer.armor.Contains(Item))
            //{
            //    //for (int j = 0; j < 12; j++)
            //    //{
            //    //    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
            //    //    float modifier = 0.25f + ((float)Math.Sin(drawTimer / 30f) / 2);
            //    //    //Color glowColor = Color.Lerp(SynergyColor1, SynergyColor2, modifier) * 0.5f;

            //    //    Texture2D texture = Terraria.GameContent.TextureAssets.Item[Item.type].Value;
            //    //    Main.EntitySpriteDraw(texture, position + afterimageOffset, null, nameColor with {A = 100}, 0, texture.Size() * 0.5f, Item.scale, SpriteEffects.None, 0f);
            //    //}
            //    Texture2D ebonTexture = ModContent.Request<Texture2D>(Texture + "_Alt").Value;

            //    Main.EntitySpriteDraw(ebonTexture, position, null, drawColor, 0, ebonTexture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            //    return false;
            //}
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // Can somebody tell me why the fuck this doesnt work for warlock and granite?
            if ((Main.LocalPlayer.HasEffect<SilkEffect>() || Main.LocalPlayer.HasEffect<T>()) && Main.LocalPlayer.armor.Contains(Item))
            {
                Texture2D altTexture = ModContent.Request<Texture2D>(Texture + "_Alt").Value;

                Color color = drawColor with { A = (byte)(127.5f + 127.5 * MathF.Sin(drawTimer * MathF.PI / 60f)) };
                Main.EntitySpriteDraw(altTexture, position, null, color , 0, altTexture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            }
            drawTimer++;
        }
    }

    public abstract class SynergyEffect<T> : AccessoryEffect where T : AccessoryEffect
    {
        public bool SynergyActive(Player player) => player.GetModPlayer<AccessoryEffectPlayer>().Active(this) && (player.HasEffect<SilkEffect>() || player.HasEffect<T>());
    }
}