
using CalamityMod;
using CalamityMod.NPCs.Abyss;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ColossalTentacle : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<ColossalSquid>();
        public override void AddRecipes()
        {
        }
        public override bool CanUseItem(Player player)
        {
            if (!player.Calamity().ZoneAbyss) return false;
            return base.CanUseItem(player);
        }
    }
}
