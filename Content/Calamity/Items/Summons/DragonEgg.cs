using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.Yharon;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DragonEgg : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/YharonEgg";
        public override int NPCType => ModContent.NPCType<Yharon>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<YharonEgg>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<YharonEgg>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
