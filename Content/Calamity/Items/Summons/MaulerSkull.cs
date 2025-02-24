
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.AcidRain;

using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MaulerSkull : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<Mauler>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<CorrodedFossil>(10).AddIngredient<NuclearChunk>().AddTile(TileID.Anvils).Register();
        }
        public override bool CanUseItem(Player player)
        {
            if (!player.Calamity().ZoneSulphur) return false;
            return base.CanUseItem(player);
        }
    }
}
