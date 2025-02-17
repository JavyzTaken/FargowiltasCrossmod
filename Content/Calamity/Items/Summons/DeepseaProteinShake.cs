﻿
using CalamityMod;
using CalamityMod.NPCs.Abyss;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DeepseaProteinShake : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<ReaperShark>();
        public override string NPCName => Language.GetTextValue("Mods.CalamityMod.NPCs.ReaperShark.DisplayName");
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
