using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.SunkenSea;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class WyrmTablet : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<EidolonWyrmHead>();
        public override void AddRecipes()
        {
        }
        public override bool CanUseItem(Player player)
        {
            if (!player.Calamity().ZoneAbyss) return false;
            return base.CanUseItem(player);
        }
        public override bool? UseItem(Player player)
        {
            FargoSoulsUtil.SpawnBossNetcoded(player, NPCType);
            return base.UseItem(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }
}
