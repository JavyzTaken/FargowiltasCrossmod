using CalamityMod.Items.Materials;
using CalamityMod.NPCs.SupremeCalamitas;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EyeofExtinction : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/EyeofDesolation";
        public override int NPCType => ModContent.NPCType<SupremeCalamitas>();
        public override string NPCName => "Supreme Calamitas";
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<AshesofCalamity>(10).AddIngredient<AuricBar>(4).AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<AshesofCalamity>(), 10).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
