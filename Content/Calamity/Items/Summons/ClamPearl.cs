using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.NPCs.SunkenSea;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ClamPearl : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<GiantClam>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<SeaPrism>(20).AddIngredient<PearlShard>(5).AddTile(TileID.Anvils).Register();
        }
        public override bool CanUseItem(Player player)
        {
            if (!player.Calamity().ZoneSunkenSea) return false;
            return base.CanUseItem(player);
        }
    }
}
