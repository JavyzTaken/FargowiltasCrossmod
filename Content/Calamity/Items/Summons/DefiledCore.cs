using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.Providence;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DefiledCore : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/ProfanedCore";
        public override int NPCType => ModContent.NPCType<Providence>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<ProfanedCore>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<ProfanedCore>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
