using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.Perforator;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class RedStainedWormFood : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/BloodyWormFood";
        public override int NPCType => ModContent.NPCType<PerforatorHive>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<BloodyWormFood>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<BloodyWormFood>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
