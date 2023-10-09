using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.Ravager;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class NoisyWhistle : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/DeathWhistle";
        public override int NPCType => ModContent.NPCType<RavagerBody>();
        public override string NPCName => "Ravager";
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<DeathWhistle>().AddTile(TileID.WorkBenches).Register();
        }
    }
}
