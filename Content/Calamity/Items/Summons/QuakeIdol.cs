
using CalamityMod.NPCs.NormalNPCs;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class QuakeIdol : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<Horse>();
        public override void AddRecipes()
        {
        }
        public override bool CanUseItem(Player player)
        {
            if (player.Center.Y/16 < Main.worldSurface) return false;
            return base.CanUseItem(player);
        }
    }
}
