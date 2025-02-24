using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.AstrumAureus;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ChunkyStardust : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/AstralChunk";
        public override int NPCType => ModContent.NPCType<AstrumAureus>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<AstralChunk>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<AstralChunk>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
