using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.DesertScourge;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MedallionoftheDesert : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/DesertMedallion";
        public override int NPCType => ModContent.NPCType<DesertScourgeHead>();
        public override string NPCName => "Desert Scourge";
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<DesertMedallion>().AddTile(TileID.WorkBenches).Register();
        }
    }
}
