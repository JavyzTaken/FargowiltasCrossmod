using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.CalClone;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BlightedEye : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/EyeofDesolation";
        public override int NPCType => ModContent.NPCType<CalamitasClone>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<EyeofDesolation>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<EyeofDesolation>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
