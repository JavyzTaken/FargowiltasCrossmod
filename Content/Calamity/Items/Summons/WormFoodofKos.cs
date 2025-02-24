using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.StormWeaver;
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
    public class WormFoodofKos : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<StormWeaverHead>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<RuneofKos>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<RuneofKos>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
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
