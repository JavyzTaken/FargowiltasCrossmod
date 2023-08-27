using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using System.Linq;
using Terraria.Utilities;

namespace FargowiltasCrossmod.Content.Thorium.Items
{
    [ExtendsFromMod("ThoriumMod")]
    public class ThoriumGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            // gilded sight item effect
            if (Main.netMode != NetmodeID.Server && Main.player[Main.myPlayer].HasBuff<Buffs.GildedSightDB>() && !Buffs.GildedSightDB.GildedItems.Contains(item.type))
            {
                int type = Buffs.GildedSightDB.GildedItems[item.type % Buffs.GildedSightDB.GildedItems.Length];
                Vector2 Pos = item.position - Main.screenPosition;

                string texturePath;
                if (type == ModContent.ItemType<ThoriumMod.Items.Thorium.ThoriumOre>()) texturePath = "ThoriumMod/Items/Thorium/ThoriumOre";
                else if (type == ModContent.ItemType<ThoriumMod.Items.Thorium.ThoriumBar>()) texturePath = "ThoriumMod/Items/Thorium/ThoriumBar";
                else texturePath = $"Terraria/Images/Item_{type}";

                Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;

                spriteBatch.Draw(texture, Pos + new Vector2(0, item.height - texture.Height), lightColor);
                return false;
            }
            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override void PostUpdate(Item item)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Player player = Main.player[Main.myPlayer];
                if (player.GetModPlayer<CrossplayerThorium>().GildedMonicle)
                {
                    Lighting.AddLight(item.position, new Vector3(0.6f, 0.6f, 0.6f));
                }
            }
        }
    }
}
