
using CalamityMod;
using CalamityMod.NPCs.AcidRain;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class NuclearChunk : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<NuclearTerror>();
        public override string NPCName => Language.GetTextValue("Mods.CalamityMod.NPCs.NuclearTerror.DisplayName");
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
