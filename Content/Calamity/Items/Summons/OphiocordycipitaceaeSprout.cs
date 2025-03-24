using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.Crabulon;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class OphiocordycipitaceaeSprout : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/DecapoditaSprout";
        public override int NPCType => ModContent.NPCType<Crabulon>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<DecapoditaSprout>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<DecapoditaSprout>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
