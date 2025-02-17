using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Leviathan;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SirensPearl : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<Anahita>();
        public override string NPCName => Language.GetTextValue("Mods.CalamityMod.BossChecklistIntegration.Leviathan.EntryName");
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient(ItemID.WhitePearl).AddIngredient<PerennialBar>(5).AddTile(TileID.WorkBenches).Register();
        }
    }
}
