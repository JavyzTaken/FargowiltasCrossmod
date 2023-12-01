using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;

namespace FargowiltasCrossmod.Core.Utils
{
    public static partial class DLCUtils
    {
        public static bool HostCheck => Main.netMode != NetmodeID.MultiplayerClient;
        //copy psated from fargo but changed so it can be from any mod
        public static void DropSummon(NPC npc, string mod, string itemName, bool downed, ref bool droppedSummonFlag, bool prerequisite = true)
        {
            if (WorldSavingSystem.EternityMode && prerequisite && !downed && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummonFlag)
            {
                Player player = Main.player[npc.target];

                if (ModContent.TryFind(mod, itemName, out ModItem modItem))
                    Item.NewItem(npc.GetSource_Loot(), player.Hitbox, modItem.Type);
                droppedSummonFlag = true;
            }
        }
        /// <summary>
        /// Draws a backglow effect
        /// </summary>
        /// <param name="texture">The texture to be used </param>
        /// <param name="color">The color of the backglow. an alpha of 0.7 will be applied to this</param>
        /// <param name="position">The position to draw the backglow at</param>
        /// <param name="origin">The origin of rotation and scaling of the backglow</param>
        /// <param name="rotation">The rotation to draw the backglow at</param>
        /// <param name="scale">The scale to draw the backglow at</param>
        /// <param name="spriteEffects">The sprite effects to use when drawing the backglow</param>
        /// <param name="iterations">Number of times backglow drawing is iterated through</param>
        /// <param name="offsetMult">How much the offset is multiplied by (bigger number = backglow stretches farther from center)</param>
        public static void DrawBackglow(Asset<Texture2D> texture, Color color, Vector2 position, Vector2 origin, float rotation = 0, float scale = 1, SpriteEffects spriteEffects = SpriteEffects.None, int iterations = 12, float offsetMult = 1)
        {
            for (int j = 0; j < iterations; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * offsetMult;
                Color glowColor = color with { A = 0 } * 0.7f;


                Main.EntitySpriteDraw(texture.Value, position + afterimageOffset - Main.screenPosition, null, glowColor, rotation, origin, scale, spriteEffects);
            }
        }
        /// <summary>
        /// Draws a backglow effect
        /// </summary>
        /// <param name="texture">The texture to be used </param>
        /// <param name="color">The color of the backglow. an alpha of 0.7 will be applied to this</param>
        /// <param name="position">The position to draw the backglow at</param>
        /// <param name="origin">The origin of rotation and scaling of the backglow</param>
        /// <param name="rotation">The rotation to draw the backglow at</param>
        /// <param name="scale">The scale to draw the backglow at</param>
        /// <param name="spriteEffects">The sprite effects to use when drawing the backglow</param>
        /// <param name="iterations">Number of times backglow drawing is iterated through</param>
        /// <param name="offsetMult">How much the offset is multiplied by (bigger number = backglow stretches farther from center)</param>
        public static void DrawBackglow(Asset<Texture2D> texture, Color color, Vector2 position, Vector2 origin, Vector2 scale, float rotation = 0, SpriteEffects spriteEffects = SpriteEffects.None, int iterations = 12, float offsetMult = 1)
        {
            for (int j = 0; j < iterations; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * offsetMult;
                Color glowColor = color with { A = 0 } * 0.7f;


                Main.EntitySpriteDraw(texture.Value, position + afterimageOffset - Main.screenPosition, null, glowColor, rotation, origin, scale, spriteEffects);
            }
        }

        #region Extension Methods
        /// <summary>
        /// Enqueues all entries of the list to the queue, in a random order.
        /// </summary>
        public static void RandomFromList<T>(this Queue<T> queue, List<T> list)
        {
            foreach (T a in list.OrderBy(a => Main.rand.Next()).ToList())
            {
                queue.Enqueue(a);
            }
        }
        public static void RandomFromListExcept<T>(this Queue<T> queue, List<T> list, params T[] exclude) => queue.RandomFromList(list.Except(exclude).ToList());

        #endregion
    }
}
