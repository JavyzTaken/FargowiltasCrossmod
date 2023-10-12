using CalamityMod.Items.DraedonMisc;
using CalamityMod.NPCs.ExoMechs;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ExoBattery : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/Materials/ExoPrism";
        public override int NPCType => ModContent.NPCType<Draedon>();
        public override string NPCName => "Draedon";
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<AuricQuantumCoolingCell>().AddTile(TileID.WorkBenches).Register();
        }
    }
}
