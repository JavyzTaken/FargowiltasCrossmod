using CalamityMod;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.AstrumDeus;
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
    public class AstrumCor : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<AstrumDeusHead>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<Starcore>().AddTile(TileID.WorkBenches).DisableDecraft().Register();
            Recipe.Create(ModContent.ItemType<Starcore>()).AddIngredient(Type).AddTile(TileID.WorkBenches).AddCondition(CalamityConditions.DownedAstrumDeus).DisableDecraft().Register();
        }
        public override bool? UseItem(Player player)
        {
            FargoSoulsUtil.SpawnBossNetcoded(player, NPCType);
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }
}
