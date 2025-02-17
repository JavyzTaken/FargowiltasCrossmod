using CalamityMod.Items.DraedonMisc;
using CalamityMod.NPCs.ExoMechs;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PortableCodebreaker : BaseSummon
    {
        //public override string Texture => "CalamityMod/Items/Materials/ExoPrism";
        public override int NPCType => ModContent.NPCType<Draedon>();
        public override string NPCName => Language.GetTextValue("Mods.CalamityMod.NPCs.Draedon.DisplayName");
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<AuricQuantumCoolingCell>().AddTile(TileID.WorkBenches).Register();
        }
    }
}
