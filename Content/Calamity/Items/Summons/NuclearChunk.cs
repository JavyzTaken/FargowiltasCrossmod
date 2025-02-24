
using CalamityMod;
using CalamityMod.NPCs.AcidRain;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class NuclearChunk : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<NuclearTerror>();
        public override void AddRecipes()
        {
        }
        public override bool CanUseItem(Player player)
        {
            if (!player.Calamity().ZoneSulphur) return false;
            return base.CanUseItem(player);
        }
    }
}
