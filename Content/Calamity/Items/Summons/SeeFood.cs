using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.AquaticScourge;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SeeFood : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/Seafood";
        public override int NPCType => ModContent.NPCType<AquaticScourgeHead>();
        public override string NPCName => "Aquatic Scourge";
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<Seafood>().AddTile(TileID.WorkBenches).Register();
        }
    }
}
