using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.ProfanedGuardians;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DefiledShard : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/ProfanedShard";
        public override int NPCType => ModContent.NPCType<ProfanedGuardianCommander>();
        public override string NPCName => "Profaned Guardians";
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<ProfanedShard>().AddTile(TileID.WorkBenches).Register();
        }
    }
}
