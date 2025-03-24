using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.ProfanedGuardians;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DefiledShard : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/ProfanedShard";
        public override int NPCType => ModContent.NPCType<ProfanedGuardianCommander>();
        public override string NPCName => Language.GetTextValue("Mods.CalamityMod.BossChecklistIntegration.ProfanedGuardians.EntryName");
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<ProfanedShard>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<ProfanedShard>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
