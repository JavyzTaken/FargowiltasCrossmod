using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.HiveMind;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HiveTumor : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/Teratoma";
        public override int NPCType => ModContent.NPCType<HiveMind>();
        public override string NPCName => "Hive Mind";
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<Teratoma>().AddTile(TileID.WorkBenches).Register();
        }
    }
}
