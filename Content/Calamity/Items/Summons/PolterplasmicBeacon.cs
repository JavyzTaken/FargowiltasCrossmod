using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.Polterghast;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PolterplasmicBeacon : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/NecroplasmicBeacon";
        public override int NPCType => ModContent.NPCType<Polterghast>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<NecroplasmicBeacon>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<NecroplasmicBeacon>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
    }
}
