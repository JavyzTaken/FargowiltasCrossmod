using CalamityMod.Items.LoreItems;
using FargowiltasCrossmod.Core;
using Luminance.Core.Hooking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExoMechLoreItemRewritter : GlobalItem
    {
        public override void Load()
        {
            HookHelper.ModifyMethodWithIL(typeof(Main).GetMethod("MouseText_DrawItemTooltip", LumUtils.UniversalBindingFlags), DrawBlackBoxForLoreItem);
        }

        public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            if (item.type == ModContent.ItemType<LoreExoMechs>())
            {
                DrawExoMechLore(new(x, y));
                return true;
            }

            return true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<LoreExoMechs>())
            {
                tooltips.RemoveAll(t => t.Name.Contains("Tooltip") || t.Name.Contains("Lore") || t.Name == "Favorite" || t.Name == "FavoriteDesc");
            }
        }

        private void DrawBlackBoxForLoreItem(ILContext c)
        {
            ILCursor cursor = new(c);

            // Search for the number 81, since the box is drawn with an RGBA value of (23, 25, 81, 255).
            if (!cursor.TryGotoNext(i => i.MatchLdcI4(81)))
            {
                Mod.Logger.Error("Could not apply the IL edit for the tooltip box for the Exo Mech lore item! The blue value of 81 could not be located.");
                return;
            }

            // Search for the created color, after the opacity multiplication of 0.925.
            MethodInfo? colorFloatMultiply = typeof(Color).GetMethod("op_Multiply", [typeof(Color), typeof(float)]);
            if (colorFloatMultiply is null || !cursor.TryGotoNext(MoveType.After, i => i.MatchCall(colorFloatMultiply)))
            {
                Mod.Logger.Error("Could not apply the IL edit for the tooltip box for the Exo Mech lore item! The Color object creation could not be located.");
                return;
            }

            // Take in the newly created color and modify it safely.
            cursor.EmitDelegate((Color originalColor) =>
            {
                if (Main.HoverItem.type == ModContent.ItemType<LoreExoMechs>())
                    return new(30, 30, 30);

                return originalColor;
            });

            // Go back to the first Vector2.Zero load. This is used to express the dimensions of the box, and must be expanded a bit in order to
            // ensure that the exo mechs lore item image properly renders, given that there's no actual text to properly define the boundaries.
            cursor.Goto(0);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<Vector2>("get_Zero")))
            {
                Mod.Logger.Error("Could not apply the IL edit for the tooltip box for the Exo Mech lore item! The Vector2.Zero load could not be located.");
                return;
            }
            cursor.EmitDelegate((Vector2 originalBaseDimensions) =>
            {
                if (Main.HoverItem.type == ModContent.ItemType<LoreExoMechs>())
                {
                    originalBaseDimensions.X += 454f;
                    originalBaseDimensions.Y += 612f;
                }

                return originalBaseDimensions;
            });
        }

        /// <summary>
        /// Draws the Exo Mech lore item's tooltip.
        /// </summary>
        /// <param name="item">The lore item</param>
        /// <param name="baseDrawPosition">The base draw position of the tooltip</param>
        private static void DrawExoMechLore(Vector2 baseDrawPosition)
        {
            Texture2D poem = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/DraedonPoem").Value;
            Main.spriteBatch.Draw(poem, baseDrawPosition + Vector2.UnitY * 20f, Color.White);
        }
    }
}
