using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.AquaticScourge;
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
    public class SeeFood : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/Seafood";
        public override int NPCType => ModContent.NPCType<AquaticScourgeHead>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<Seafood>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<Seafood>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
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
