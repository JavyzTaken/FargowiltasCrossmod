using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.SunkenSea;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class StormIdol : BaseSummon
    {
        //FUCK you fabsol for making me type this
        public override int NPCType => ModContent.NPCType<ThiccWaifu>();
        public override void AddRecipes()
        {
        }
    }
}
