using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.OldDuke;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BloodyWorm : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/BloodwormItem";
        public override int NPCType => ModContent.NPCType<OldDuke>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<BloodwormItem>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<BloodwormItem>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
