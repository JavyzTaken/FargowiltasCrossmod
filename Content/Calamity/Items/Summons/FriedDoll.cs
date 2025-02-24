using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.BrimstoneElemental;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class FriedDoll : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/CharredIdol";
        public override int NPCType => ModContent.NPCType<BrimstoneElemental>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<CharredIdol>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<CharredIdol>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
