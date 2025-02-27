
using CalamityMod;
using CalamityMod.NPCs.NormalNPCs;
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
    public class AbandonedRemote : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<ArmoredDiggerHead>();
        public override void AddRecipes()
        {
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
        public override bool CanUseItem(Player player)
        {
            if (player.Center.Y / 16 < Main.worldSurface) return false;
            return base.CanUseItem(player);
        }
    }
}
