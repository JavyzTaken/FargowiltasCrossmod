using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.World;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EyeofExtinction : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<SupremeCalamitas>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<AshesofCalamity>(10).AddIngredient<AuricBar>(4).AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<AshesofCalamity>(), 10).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
        public override bool? UseItem(Player player)
        {
            Vector2 ritualSpawnPosition = player.Center;
            SoundEngine.PlaySound(SCalAltar.SummonSound, ritualSpawnPosition);
            Projectile.NewProjectile(new EntitySource_WorldEvent(), ritualSpawnPosition, Vector2.Zero, ModContent.ProjectileType<SCalRitualDrama>(), 0, 0f, Main.myPlayer, 0, 0);
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }
}
