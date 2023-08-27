using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Fargowiltas.Projectiles;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Audio;
using ThoriumMod.NPCs;

namespace FargowiltasCrossmod.Content.Thorium.Items.Summons
{
    [ExtendsFromMod("ThoriumMod")]
    public class GildedSummon : ModItem
    {
        public override string Texture => "SoulsBetterDLC/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            
            Item.ResearchUnlockCount = 3;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime || player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight || player.ZoneUnderworldHeight;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 20;
            Item.value = Item.sellPrice(0, 0, 2);
            Item.rare = ItemRarityID.Yellow;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.consumable = true;
            Item.shoot = ModContent.ProjectileType<SpawnProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 pos = new Vector2((int)player.position.X + Main.rand.Next(-800, 800), (int)player.position.Y + Main.rand.Next(-1000, -250));
            Projectile.NewProjectile(player.GetSource_ItemUse(source.Item), pos, Vector2.Zero, ModContent.ProjectileType<SpawnProj>(), 0, 0, Main.myPlayer, ModContent.NPCType<GildedBat>());

            pos = new Vector2((int)player.position.X + Main.rand.Next(-800, 800), (int)player.position.Y + Main.rand.Next(-1000, -250));
            Projectile.NewProjectile(player.GetSource_ItemUse(source.Item), pos, Vector2.Zero, ModContent.ProjectileType<SpawnProj>(), 0, 0, Main.myPlayer, ModContent.NPCType<GildedLycan>());

            pos = new Vector2((int)player.position.X + Main.rand.Next(-800, 800), (int)player.position.Y + Main.rand.Next(-1000, -250));
            Projectile.NewProjectile(player.GetSource_ItemUse(source.Item), pos, Vector2.Zero, ModContent.ProjectileType<SpawnProj>(), 0, 0, Main.myPlayer, ModContent.NPCType<GildedSlime>());

            SoundEngine.PlaySound(SoundID.Roar, player.position);

            return true;
        }
    }
}
