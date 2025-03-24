using CalamityMod;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Common
{
    public static partial class DLCUtils
    {
        public static bool HostCheck => Main.netMode != NetmodeID.MultiplayerClient;

        public static int ClosestNPCExcludingOne(Vector2 position, float range, int exclude, bool lineOfSight)
        {
            int target = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n == null || !n.active || n.friendly || n.lifeMax <= 5 || i == exclude) continue;
                if (lineOfSight && !Collision.CanHitLine(n.Center, 1, 1, position, 1, 1))
                {
                    continue;
                }
                if (target == -1 || (n.Distance(position) < Main.npc[target].Distance(position)))
                {
                    target = i;
                }
            }
            return target;
        }
        //copy psated from fargo but changed so it can be from any mod
        public static void DropSummon(NPC npc, string mod, string itemName, bool downed, ref bool droppedSummonFlag, bool prerequisite = true)
        {
            if (WorldSavingSystem.EternityMode && prerequisite && !downed && Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && !droppedSummonFlag)
            {
                Player player = Main.player[npc.target];
                if (ModContent.TryFind(mod, itemName, out ModItem modItem))
                {
                    if (!CalDLCWorldSavingSystem.DroppedSummon.Contains(npc.type))
                    {
                        if (!Main.LocalPlayer.InventoryHas(modItem.Type))
                        {
                            CalDLCWorldSavingSystem.DroppedSummon.Add(npc.type);
                            Item.NewItem(npc.GetSource_Loot(), player.Hitbox, modItem.Type);
                            droppedSummonFlag = true;
                        }
                    }
                }

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
        public static void DrawBackglow(Asset<Texture2D> texture, Color color, Vector2 position, Vector2 origin, float rotation = 0, float scale = 1, SpriteEffects spriteEffects = SpriteEffects.None, int iterations = 12, float offsetMult = 1, Rectangle? sourceRectangle = null)
        {
            for (int j = 0; j < iterations; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * offsetMult;
                Color glowColor = color with { A = 0 } * 0.7f;


                Main.EntitySpriteDraw(texture.Value, position + afterimageOffset - Main.screenPosition, sourceRectangle, glowColor, rotation, origin, scale, spriteEffects);
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
        public static void DrawBackglow(Asset<Texture2D> texture, Color color, Vector2 position, Vector2 origin, Vector2 scale, float rotation = 0, SpriteEffects spriteEffects = SpriteEffects.None, int iterations = 12, float offsetMult = 1, Rectangle? sourceRectangle = null)
        {
            for (int j = 0; j < iterations; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * offsetMult;
                Color glowColor = color with { A = 0 } * 0.7f;


                Main.EntitySpriteDraw(texture.Value, position + afterimageOffset - Main.screenPosition, sourceRectangle, glowColor, rotation, origin, scale, spriteEffects);
            }
        }

        #region Extension Methods
        /// <summary>
        /// Enqueues all entries of the list to the queue, in a random order.
        /// </summary>
        public static void RandomFromList<T>(this Queue<T> queue, IEnumerable<T> list)
        {
            foreach (T a in list.OrderBy(a => Main.rand.Next()))
            {
                queue.Enqueue(a);
            }
        }
        public static void RandomFromListExcept<T>(this Queue<T> queue, IEnumerable<T> list, params T[] exclude) => queue.RandomFromList(list.Except(exclude));

        // Because apparently SoundStyle.Volume doesn't allow for going past a certain threshold...
        /// <summary>
        /// Modifies the volume of a played sound slot instance.
        /// </summary>
        /// <param name="soundSlot">The sound slot to affect.</param>
        /// <param name="volumeFactor">The volume modifier factor.</param>
        public static SlotId WithVolumeBoost(this SlotId soundSlot, float volumeFactor)
        {
            if (SoundEngine.TryGetActiveSound(soundSlot, out ActiveSound? sound) && sound is not null)
                sound.Volume *= volumeFactor;

            return soundSlot;
        }

        /// <summary>
        /// Calculates the intersection points between a ellipse and a line.
        /// </summary>
        /// <param name="start">The line's pivot point.</param>
        /// <param name="direction">The line's direction.</param>
        /// <param name="ellipseCenter">The center position of the ellipse.</param>
        /// <param name="ellipseSize">The size of the ellipse.</param>
        /// <param name="ellipseRotation">The rotation of the ellipse.</param>
        /// <param name="solutionA">The first resulting solution.</param>
        /// <param name="solutionB">The second resulting solution.</param>
        public static void LineEllipseIntersectionCheck(Vector2 start, Vector2 direction, Vector2 ellipseCenter, Vector2 ellipseSize, float ellipseRotation, out Vector2 solutionA, out Vector2 solutionB)
        {
            // Taken by solving solutions from the following two equations:
            // y - v = m * (x - u)
            // (x / w)^2 + (y / h)^2 = 1

            // Rearranging terms in the linear equation results in the following definition for y:
            // y = m * (x - u) + v

            // In order to solve for x, it's simply a matter of plugging in this equation in for y in the ellipse equation, like so:
            // (x / w)^2 + ((m * (x - u) + v) / h)^2 = 1

            // And now, for solving...
            // Just go to some online website to get the result. I'm not writing out all the diabolical algebra steps in a code comment.
            // https://www.symbolab.com/solver/step-by-step/%5Cleft(%5Cfrac%7Bx%7D%7Bw%7D%5Cright)%5E%7B2%7D%2B%5Cleft(%5Cfrac%7Bm%5Cleft(x-u%5Cright)%2Bv%7D%7Bh%7D%5Cright)%5E%7B2%7D%3D1?or=input

            // Rotating the actual ellipse in the above equation makes everything kind of brain-melting so instead just do a little bit of a relativistic magic and
            // do a reverse-rotation on the line for the same effect in practice.
            start = start.RotatedBy(-ellipseRotation, ellipseCenter);
            direction = direction.RotatedBy(-ellipseRotation);

            float m = direction.Y / direction.X;
            float u = start.X - ellipseCenter.X;
            float v = start.Y - ellipseCenter.Y;
            float w = ellipseSize.X;
            float h = ellipseSize.Y;

            float numeratorFirstHalf = -w * (m.Squared() * u * -2f + m * v * 2f);
            float numeratorSecondHalf = MathF.Sqrt(-m.Squared() * u.Squared() + m * u * v * 2f + m.Squared() * w.Squared() + h.Squared() - v.Squared()) * h * 2f;
            float denominator = (m.Squared() * w.Squared() + h.Squared()) * 2f;

            float xSolutionA = (numeratorFirstHalf - numeratorSecondHalf) * w / denominator;
            float xSolutionB = (numeratorFirstHalf + numeratorSecondHalf) * w / denominator;

            // Now that the two solution X values are known, it's simply a matter of plugging X back into the linear equation to get Y.
            float ySolutionA = m * (xSolutionA - u) + v;
            float ySolutionB = m * (xSolutionB - u) + v;

            solutionA = new Vector2(xSolutionA, ySolutionA).RotatedBy(ellipseRotation) + ellipseCenter;
            solutionB = new Vector2(xSolutionB, ySolutionB).RotatedBy(ellipseRotation) + ellipseCenter;
        }

        #endregion
    }
}
