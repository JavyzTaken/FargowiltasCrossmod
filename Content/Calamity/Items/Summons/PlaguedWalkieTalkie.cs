using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.SunkenSea;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PlaguedWalkieTalkie : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<PlaguebringerMiniboss>();
        public override void AddRecipes()
        {
        }
        public override bool CanUseItem(Player player)
        {
            if (!player.ZoneJungle) return false;
            return base.CanUseItem(player);
        }
    }
}
