using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.Bumblebirb;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BirbPheromones : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/ExoticPheromones";
        public override int NPCType => ModContent.NPCType<Bumblefuck>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<ExoticPheromones>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<ExoticPheromones>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
