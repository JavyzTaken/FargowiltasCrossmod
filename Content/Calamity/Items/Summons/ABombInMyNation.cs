using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.PlaguebringerGoliath;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ABombInMyNation : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/Abombination";
        public override int NPCType => ModContent.NPCType<PlaguebringerGoliath>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<Abombination>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<Abombination>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
