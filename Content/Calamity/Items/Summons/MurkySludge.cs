using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.SlimeGod;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MurkySludge : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/OverloadedSludge";
        public override int NPCType => ModContent.NPCType<SlimeGodCore>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<OverloadedSludge>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<OverloadedSludge>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
