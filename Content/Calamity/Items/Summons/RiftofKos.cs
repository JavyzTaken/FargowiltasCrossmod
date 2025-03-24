using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.CeaselessVoid;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class RiftofKos : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<CeaselessVoid>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<RuneofKos>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<RuneofKos>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
