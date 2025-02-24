using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.SunkenSea;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SulphurBearTrap : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<CragmawMire>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<CorrodedFossil>(15).AddRecipeGroup(RecipeGroupID.IronBar, 5).AddTile(TileID.Anvils).Register();
        }
        public override bool CanUseItem(Player player)
        {
            if (!player.Calamity().ZoneSulphur) return false;
            return base.CanUseItem(player);
        }
    }
}
