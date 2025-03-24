using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.Cryogen;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CryingKey : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/CryoKey";
        public override int NPCType => ModContent.NPCType<Cryogen>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<CryoKey>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<CryoKey>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
